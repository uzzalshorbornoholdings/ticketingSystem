using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public partial class MainForm : Form
    {
        private readonly string _userRole;
        private readonly string _employeeId;
        private readonly string _username;
        private readonly DatabaseManager _db;
        private readonly SlaEngine _slaEngine;
        private readonly TriageEngine _triageEngine;

        private int _selectedTicketId = -1;
        private string _selectedTicketType = null;
        private Timer _lockTimer;
        private readonly HashSet<int> _notifiedTicketIds = new HashSet<int>();

        public MainForm(string role, string employeeId, string username, DatabaseManager db)
        {
            InitializeComponent();
            _userRole = role;
            _employeeId = employeeId;
            _username = username;
            _db = db;
            _slaEngine = new SlaEngine(_db);
            _triageEngine = new TriageEngine(_db);

            // Configure state check timer for ticket soft lock refresh
            _lockTimer = new Timer();
            _lockTimer.Interval = 30000; // 30 seconds
            _lockTimer.Tick += LockTimer_Tick;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblUserContext.Text = $"Welcome, {_username} ({_userRole}) | ID: {_employeeId ?? "N/A"}";

            // Show Admin control button only if the role is Admin
            btnNavAdmin.Visible = (_userRole == "Admin");

            // Pre-populate notified ticket IDs to avoid alert spamming for historical tickets on startup
            if (!string.IsNullOrEmpty(_employeeId))
            {
                try
                {
                    string query = "SELECT id FROM tickets WHERE assigned_employee_id = @empId";
                    var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@empId", _employeeId) });
                    foreach (DataRow row in dt.Rows)
                    {
                        _notifiedTicketIds.Add(Convert.ToInt32(row["id"]));
                    }
                }
                catch { }
            }

            LoadQueueData();
            _lockTimer.Start();
        }

        private void LoadQueueData()
        {
            // Subscribe to DataBindingComplete before staging DataSource
            gridIncidents.DataBindingComplete -= Grid_DataBindingComplete;
            gridIncidents.DataBindingComplete += Grid_DataBindingComplete;

            gridServiceRequests.DataBindingComplete -= Grid_DataBindingComplete;
            gridServiceRequests.DataBindingComplete += Grid_DataBindingComplete;

            gridChanges.DataBindingComplete -= Grid_DataBindingComplete;
            gridChanges.DataBindingComplete += Grid_DataBindingComplete;

            gridIncidents.DataSource = FetchTicketsByType("INC");
            gridServiceRequests.DataSource = FetchTicketsByType("SR");
            gridChanges.DataSource = FetchTicketsByType("CR");

            ClearDetails();
        }

        private DataTable FetchTicketsByType(string type)
        {
            string query = @"
                SELECT 
                    t.id AS ID, 
                    t.title AS Title, 
                    t.priority AS Priority, 
                    t.status AS Status, 
                    creator.name AS Creator, 
                    assignee.name AS Assignee,
                    t.created_at AS CreatedAt,
                    t.locked_by AS LockedBy
                FROM tickets t
                LEFT JOIN employees creator ON t.creator_employee_id = creator.id
                LEFT JOIN employees assignee ON t.assigned_employee_id = assignee.id
                WHERE t.type = @type
                ORDER BY t.created_at DESC";

            return _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@type", type) });
        }

        private void ConfigureGrids(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;

            // Must disable auto-sizing before setting explicit widths to avoid layout crashes
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Null-safe helper — MySQL column alias casing can vary by driver version
            SetColumnWidth(grid, "ID", 45);
            SetColumnWidth(grid, "Priority", 65);
            SetColumnWidth(grid, "Status", 85);
            SetColumnWidth(grid, "Creator", 110);
            SetColumnWidth(grid, "Assignee", 110);

            var dateCol = FindColumn(grid, "CreatedAt");
            if (dateCol != null) dateCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Let the Title column fill remaining space dynamically
            var titleCol = FindColumn(grid, "Title");
            if (titleCol != null) titleCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Style header row
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;

            // Row styling
            grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            grid.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(33, 38, 47);
            grid.GridColor = System.Drawing.Color.FromArgb(50, 58, 70);
        }

        /// <summary>Safely sets column width — no crash if column name doesn't exist.</summary>
        private static void SetColumnWidth(DataGridView grid, string colName, int width)
        {
            if (grid == null || grid.Columns == null || string.IsNullOrEmpty(colName)) return;

            try
            {
                // Case-insensitive safe lookup without LINQ (avoids adding new using directives)
                DataGridViewColumn col = null;
                foreach (DataGridViewColumn c in grid.Columns)
                {
                    if (string.Equals(c.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col = c;
                        break;
                    }
                }

                if (col != null)
                {
                    // Safely set auto size mode to None before setting width to prevent layout-lock crashes
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = width;
                }
            }
            catch
            {
                // Avoid layout/initialize exceptions during grid load
            }
        }

        private static DataGridViewColumn FindColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null || string.IsNullOrEmpty(colName)) return null;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                {
                    return col;
                }
            }
            return null;
        }

        private static DataGridViewCell FindCell(DataGridViewRow row, string colName)
        {
            if (row == null || string.IsNullOrEmpty(colName)) return null;
            var grid = row.DataGridView;
            if (grid != null)
            {
                var col = FindColumn(grid, colName);
                if (col != null)
                {
                    return row.Cells[col.Index];
                }
            }
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.OwningColumn != null && string.Equals(cell.OwningColumn.Name, colName, StringComparison.OrdinalIgnoreCase))
                {
                    return cell;
                }
            }
            return null;
        }

        private void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid != null)
            {
                // Defer grid configuration to avoid layout conflicts during active data binding/load phases
                this.BeginInvoke(new Action(() => ConfigureGrids(grid)));
            }
        }

        private void TabControlQueues_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearDetails();
        }

        private void GridIncidents_SelectionChanged(object sender, EventArgs e)
        {
            HandleGridSelection(gridIncidents, "INC");
        }

        private void GridServiceRequests_SelectionChanged(object sender, EventArgs e)
        {
            HandleGridSelection(gridServiceRequests, "SR");
        }

        private void GridChanges_SelectionChanged(object sender, EventArgs e)
        {
            HandleGridSelection(gridChanges, "CR");
        }

        private void HandleGridSelection(DataGridView grid, string type)
        {
            if (grid == null || grid.DataSource == null || grid.Columns.Count == 0) return;
            if (grid.SelectedRows.Count > 0)
            {
                var row = grid.SelectedRows[0];
                var idCell = FindCell(row, "ID");
                if (idCell?.Value != null && idCell.Value != DBNull.Value)
                {
                    int ticketId = Convert.ToInt32(idCell.Value);
                    _selectedTicketType = type;
                    DisplayTicketDetails(ticketId);
                }
            }
        }

        private void DisplayTicketDetails(int ticketId)
        {
            _selectedTicketId = ticketId;

             // Fetch Ticket Record
             string query = @"
                 SELECT t.title, t.description, t.type AS Type, t.priority, t.status, t.created_at, 
                        t.assigned_employee_id, assignee.name AS AssigneeName,
                        t.locked_by, locker.name AS LockerName, t.locked_until
                 FROM tickets t
                 LEFT JOIN employees assignee ON t.assigned_employee_id = assignee.id
                 LEFT JOIN employees locker ON t.locked_by = locker.id
                 WHERE t.id = @id";
 
             var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
             if (dt.Rows.Count == 0) return;
 
             var row = dt.Rows[0];
             string type = row["Type"].ToString();
             lblDetailTitle.Text = $"[{ticketId}] " + row["title"].ToString();
             lblDetailDesc.Text = row["description"].ToString();
             lblDetailPriority.Text = "Priority: " + row["priority"].ToString();
             lblDetailStatus.Text = "Status: " + row["status"].ToString();
 
             // Calculate SLA deadlines
             DateTime createdAt = Convert.ToDateTime(row["created_at"]);
             string priority = row["priority"].ToString();
             var config = _slaEngine.GetSlaConfig(priority);
             DateTime deadline = _slaEngine.CalculateDeadline(createdAt, config.ResolutionHours);
             lblDetailSla.Text = $"SLA Deadline: {deadline:yyyy-MM-dd HH:mm} (" + priority + ")";
 
             if (_slaEngine.IsBreached(createdAt, null, priority))
             {
                 lblDetailSla.ForeColor = Color.Red;
                 lblDetailSla.Text += " [BREACHED]";
             }
             else
             {
                 lblDetailSla.ForeColor = Color.FromArgb(200, 207, 214);
             }
 
             lblDetailAssignee.Text = "Assignee: " + (row["AssigneeName"] != DBNull.Value ? row["AssigneeName"].ToString() : "Unassigned");
 
             // Configure Change Request specific layout and fetch CAB/Risk details
             bool isCR = (type == "CR");
             ConfigureCRPanel(isCR);
             if (isCR)
             {
                 DisplayCRDetails(ticketId);
             }
 
             // Evaluate Soft Lock state
             string lockedById = row["locked_by"].ToString();
             if (!string.IsNullOrEmpty(lockedById) && row["locked_until"] != DBNull.Value)
             {
                 DateTime lockedUntil = Convert.ToDateTime(row["locked_until"]);
                 if (DateTime.Now < lockedUntil)
                 {
                     if (lockedById != _employeeId)
                     {
                         lblLockIndicator.Text = $"⚠️ Ticket locked by: {row["LockerName"]} (Read-only)";
                         ToggleActionButtons(false);
                     }
                     else
                     {
                         lblLockIndicator.Text = "🔒 You have locked this ticket.";
                         ToggleActionButtons(true);
                     }
                 }
                 else
                 {
                     AcquireSoftLock(ticketId);
                 }
             }
             else
             {
                 AcquireSoftLock(ticketId);
             }
 
             LoadThreadHistory(ticketId);
        }

        private void AcquireSoftLock(int ticketId)
        {
            if (string.IsNullOrEmpty(_employeeId)) return;

            DateTime lockUntil = DateTime.Now.AddMinutes(2); // Lock for 2 minutes
            string query = "UPDATE tickets SET locked_by = @locked_by, locked_until = @locked_until WHERE id = @id";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@id", ticketId),
                new MySqlParameter("@locked_by", _employeeId),
                new MySqlParameter("@locked_until", lockUntil)
            });

            lblLockIndicator.Text = "🔒 You have locked this ticket.";
            ToggleActionButtons(true);
        }

        private void ReleaseSoftLock(int ticketId)
        {
            if (string.IsNullOrEmpty(_employeeId)) return;

            string query = "UPDATE tickets SET locked_by = NULL, locked_until = NULL WHERE id = @id AND locked_by = @locked_by";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@id", ticketId),
                new MySqlParameter("@locked_by", _employeeId)
            });
        }

        private void ToggleActionButtons(bool enabled)
        {
            btnAssignToMe.Enabled = enabled;
            btnChangeStatus.Enabled = enabled;
            btnSendThread.Enabled = enabled;
            btnCreateSubTask.Enabled = enabled;
        }

        private void LoadThreadHistory(int ticketId)
        {
            txtThreadHistory.Clear();
            string query = @"
                SELECT r.message, r.created_at, e.name AS AuthorName, u.role
                FROM ticket_threads r
                LEFT JOIN employees e ON r.employee_id = e.id
                LEFT JOIN users u ON u.employee_id = e.id
                WHERE r.ticket_id = @ticketId
                ORDER BY r.created_at ASC";

            var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });
            foreach (DataRow row in dt.Rows)
            {
                string author = row["AuthorName"].ToString();
                string role = row["role"] != DBNull.Value ? $" ({row["role"]})" : "";
                DateTime time = Convert.ToDateTime(row["created_at"]);
                string msg = row["message"].ToString();

                txtThreadHistory.SelectionColor = Color.LightBlue;
                txtThreadHistory.AppendText($"[{time:yyyy-MM-dd HH:mm}] {author}{role}:\n");
                txtThreadHistory.SelectionColor = Color.White;
                txtThreadHistory.AppendText($"{msg}\n\n");
            }
        }

        private void BtnAssignToMe_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1 || string.IsNullOrEmpty(_employeeId)) return;

            string query = "UPDATE tickets SET assigned_employee_id = @empId, status = 'Assigned' WHERE id = @id";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@id", _selectedTicketId),
                new MySqlParameter("@empId", _employeeId)
            });

            // Log change in Audit Log table
            LogAuditTrail(_selectedTicketId, "Assign Ticket", $"Assigned to {_username}");

            DisplayTicketDetails(_selectedTicketId);
            LoadQueueData();
        }

        private void BtnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1 || cmbStatusEdit.SelectedItem == null) return;

            string newStatus = cmbStatusEdit.SelectedItem.ToString();
            string query = "UPDATE tickets SET status = @status WHERE id = @id";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@id", _selectedTicketId),
                new MySqlParameter("@status", newStatus)
            });

            LogAuditTrail(_selectedTicketId, "Update Status", $"Status changed to {newStatus}");

            DisplayTicketDetails(_selectedTicketId);
            LoadQueueData();
        }

        private void BtnSendThread_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1 || string.IsNullOrEmpty(_employeeId) || string.IsNullOrWhiteSpace(txtThreadInput.Text)) return;

            string msg = txtThreadInput.Text.Trim();
            string query = "INSERT INTO ticket_threads (ticket_id, employee_id, message) VALUES (@ticketId, @empId, @msg)";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@ticketId", _selectedTicketId),
                new MySqlParameter("@empId", _employeeId),
                new MySqlParameter("@msg", msg)
            });

            txtThreadInput.Clear();
            LoadThreadHistory(_selectedTicketId);
        }

        private void BtnCreateSubTask_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;

            string taskTitle = PromptDialog.ShowDialog("Enter sub-task title:", "Split Ticket to Task");
            if (string.IsNullOrWhiteSpace(taskTitle)) return;

            string query = "INSERT INTO tasks (ticket_id, title, status) VALUES (@ticketId, @title, 'Pending')";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@ticketId", _selectedTicketId),
                new MySqlParameter("@title", taskTitle)
            });

            LogAuditTrail(_selectedTicketId, "Create Sub-Task", $"Subtask created: {taskTitle}");

            MessageBox.Show("Sub-task successfully split and registered!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogAuditTrail(int ticketId, string action, string details)
        {
            if (string.IsNullOrEmpty(_employeeId)) return;

            string query = "INSERT INTO audit_logs (ticket_id, employee_id, action, details) VALUES (@ticketId, @empId, @action, @details)";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@ticketId", ticketId),
                new MySqlParameter("@empId", _employeeId),
                new MySqlParameter("@action", action),
                new MySqlParameter("@details", details)
            });
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e.RowIndex < 0) return;

            string colName = grid.Columns[e.ColumnIndex].Name;

            // Color-code Priority column
            if (string.Equals(colName, "Priority", StringComparison.OrdinalIgnoreCase))
            {
                string val = e.Value?.ToString();
                if (val == "P1") e.CellStyle.ForeColor = Color.OrangeRed;
                else if (val == "P2") e.CellStyle.ForeColor = Color.Orange;
                else if (val == "P3") e.CellStyle.ForeColor = Color.Goldenrod;
                e.FormattingApplied = true;
            }

            // SLA-based row coloring on the Status column
            if (string.Equals(colName, "Status", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var row = grid.Rows[e.RowIndex];

                    // Null-safe cell retrieval (column name case may vary by MySQL driver version)
                    var createdAtCell = FindCell(row, "CreatedAt");
                    var priorityCell = FindCell(row, "Priority");

                    if (createdAtCell?.Value == null || createdAtCell.Value == DBNull.Value) return;
                    if (priorityCell?.Value == null || priorityCell.Value == DBNull.Value) return;

                    DateTime createdAt = Convert.ToDateTime(createdAtCell.Value);
                    string priority = priorityCell.Value.ToString();

                    if (_slaEngine.IsBreached(createdAt, null, priority))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(255, 210, 210);
                        e.CellStyle.ForeColor = Color.DarkRed;
                        e.FormattingApplied = true;
                    }
                    else if (_slaEngine.IsNearBreach(createdAt, priority))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(255, 239, 166);
                        e.CellStyle.ForeColor = Color.Brown;
                        e.FormattingApplied = true;
                    }
                }
                catch
                {
                    // Suppress formatting errors — never crash the grid paint cycle
                }
            }
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedTicketId != -1)
            {
                // Refresh lock timestamp
                AcquireSoftLock(_selectedTicketId);
            }
            CheckNewAssignments();
        }

        private void ClearDetails()
        {
            if (_selectedTicketId != -1)
            {
                ReleaseSoftLock(_selectedTicketId);
            }

            _selectedTicketId = -1;
            lblDetailTitle.Text = "Select a ticket to view";
            lblDetailDesc.Text = string.Empty;
            lblDetailPriority.Text = "Priority: -";
            lblDetailStatus.Text = "Status: -";
            lblDetailSla.Text = "SLA Deadline: -";
            lblDetailAssignee.Text = "Assignee: -";
            lblLockIndicator.Text = string.Empty;
            txtThreadHistory.Clear();
            ToggleActionButtons(false);
            ConfigureCRPanel(false);
        }

        private void BtnNavTickets_Click(object sender, EventArgs e) { } // Already on tickets grid

        private void BtnNavTasks_Click(object sender, EventArgs e)
        {
            _lockTimer.Stop();
            var tasksForm = new TasksForm(_db);
            tasksForm.ShowDialog();
            _lockTimer.Start();
        }

        private void BtnNavAudit_Click(object sender, EventArgs e)
        {
            var auditForm = new AuditLogForm(_db);
            auditForm.ShowDialog();
        }

        private void BtnNavChanges_Click(object sender, EventArgs e)
        {
            tabControlQueues.SelectedIndex = 2; // Jump to Change Requests tab
        }

        private void BtnNavAdmin_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId != -1) ReleaseSoftLock(_selectedTicketId);
            _lockTimer.Stop();

            var adminForm = new AdminForm(_db);
            adminForm.ShowDialog();

            _lockTimer.Start();
            LoadQueueData();
        }

        private void BtnNavLogout_Click(object sender, EventArgs e)
        {
            ClearDetails();
            _lockTimer.Stop();
            this.Close();
        }

        private void BtnNewTicket_Click(object sender, EventArgs e)
        {
            using (var dlg = new NewTicketDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 1. Triage and classify ticket type based on keywords
                        string ticketType = _triageEngine.ClassifyTicket(dlg.TicketTitle, dlg.TicketDescription);

                        // 2. Resolve SLA configuration and deadline
                        var slaConfig = _slaEngine.GetSlaConfig(dlg.TicketPriority);

                        // 3. Resolve target department based on keywords
                        string targetDept = _triageEngine.ResolveTargetDepartment(dlg.TicketTitle, dlg.TicketDescription);

                        // 4. Save to tickets database table (initially Open)
                        string insertQuery = @"
                            INSERT INTO tickets (title, description, type, priority, sla_id, creator_employee_id, status)
                            VALUES (@title, @desc, @type, @priority, @sla_id, @creator, 'Open')";
                        
                        var insertParams = new MySqlParameter[] {
                            new MySqlParameter("@title", dlg.TicketTitle),
                            new MySqlParameter("@desc", dlg.TicketDescription),
                            new MySqlParameter("@type", ticketType),
                            new MySqlParameter("@priority", dlg.TicketPriority),
                            new MySqlParameter("@sla_id", slaConfig.Id > 0 ? (object)slaConfig.Id : DBNull.Value),
                            new MySqlParameter("@creator", string.IsNullOrEmpty(_employeeId) ? (object)DBNull.Value : _employeeId)
                        };

                        _db.ExecuteNonQuery(insertQuery, insertParams);

                        // Get the generated ID
                        object lastIdObj = _db.ExecuteScalar("SELECT LAST_INSERT_ID()");
                        if (lastIdObj != null && lastIdObj != DBNull.Value)
                        {
                            int ticketId = Convert.ToInt32(lastIdObj);

                            // 5. Run the Smart 3-tier Assignment Engine
                            string assignedTo = _triageEngine.AssignTicket(ticketId, _employeeId, targetDept);

                            // 6. Log the Audit trail
                            LogAuditTrail(ticketId, "Create Ticket", $"Ticket created and triaged as {ticketType} for {targetDept}. Assigned to {assignedTo}");
                            
                            // Check if this was a Change Request to add change_requests row
                            if (ticketType == "CR")
                            {
                                string crQuery = "INSERT INTO change_requests (ticket_id, risk_score) VALUES (@ticketId, 'Low')";
                                _db.ExecuteNonQuery(crQuery, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });
                            }
                        }

                        MessageBox.Show($"Ticket successfully submitted and assigned to the correct IT staff!", "Ticket Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Reload lists and select the appropriate tab
                        LoadQueueData();
                        if (ticketType == "INC") tabControlQueues.SelectedIndex = 0;
                        else if (ticketType == "SR") tabControlQueues.SelectedIndex = 1;
                        else if (ticketType == "CR") tabControlQueues.SelectedIndex = 2;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to submit ticket:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CheckNewAssignments()
        {
            if (string.IsNullOrEmpty(_employeeId)) return;

            try
            {
                string query = "SELECT id, title, type FROM tickets WHERE assigned_employee_id = @empId AND status = 'Assigned'";
                var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@empId", _employeeId) });

                foreach (DataRow row in dt.Rows)
                {
                    int ticketId = Convert.ToInt32(row["id"]);
                    if (!_notifiedTicketIds.Contains(ticketId))
                    {
                        _notifiedTicketIds.Add(ticketId);
                        ShowToastNotification(ticketId, row["title"].ToString(), row["type"].ToString());
                    }
                }
            }
            catch { }
        }

        private void ShowToastNotification(int ticketId, string title, string type)
        {
            this.BeginInvoke(new Action(() => {
                var toast = new ToastNotification(ticketId, title, type);
                toast.Show();
            }));
        }

        private Panel panelCRControls;
        private Label lblRisk;
        private Label lblCAB;
        private Label lblWindow;
        private Button btnRisk;
        private Button btnCABApprove;
        private Button btnSchedule;

        private Label lblPIRStatus;
        private Button btnPIRSuccess;
        private Button btnPIRRollback;
        private Button btnViewPIR;

        private void InitializeCRPanel()
        {
            if (panelCRControls != null) return;

            panelCRControls = new Panel
            {
                Location = new Point(15, 275),
                Width = 413,
                Height = 130,
                BackColor = Color.FromArgb(44, 62, 80),
                Visible = false
            };

            var lblHeader = new Label
            {
                Text = "🛠️ CAB REVIEW & RISK ENGINE",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(236, 240, 241),
                Location = new Point(8, 6),
                Width = 250,
                Height = 15
            };
            panelCRControls.Controls.Add(lblHeader);

            lblRisk = new Label
            {
                Text = "Risk: Low",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(200, 207, 214),
                Location = new Point(8, 25),
                Width = 120,
                Height = 15
            };
            panelCRControls.Controls.Add(lblRisk);

            btnRisk = new Button
            {
                Text = "Assess Risk",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(8, 44),
                Width = 85,
                Height = 24
            };
            btnRisk.FlatAppearance.BorderSize = 0;
            btnRisk.Click += BtnAssessRisk_Click;
            panelCRControls.Controls.Add(btnRisk);

            lblCAB = new Label
            {
                Text = "CAB: Pending",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(200, 207, 214),
                Location = new Point(135, 25),
                Width = 120,
                Height = 15
            };
            panelCRControls.Controls.Add(lblCAB);

            btnCABApprove = new Button
            {
                Text = "Approve (CAB)",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(135, 44),
                Width = 95,
                Height = 24
            };
            btnCABApprove.FlatAppearance.BorderSize = 0;
            btnCABApprove.Click += BtnCABApprove_Click;
            panelCRControls.Controls.Add(btnCABApprove);

            lblWindow = new Label
            {
                Text = "Window: None",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(200, 207, 214),
                Location = new Point(255, 25),
                Width = 150,
                Height = 15
            };
            panelCRControls.Controls.Add(lblWindow);

            btnSchedule = new Button
            {
                Text = "Schedule Window",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(255, 44),
                Width = 110,
                Height = 24
            };
            btnSchedule.FlatAppearance.BorderSize = 0;
            btnSchedule.Click += BtnScheduleWindow_Click;
            panelCRControls.Controls.Add(btnSchedule);

            // PIR Separator line label
            var lblPIRHeader = new Label
            {
                Text = "🔍 POST-IMPLEMENTATION REVIEW (PIR)",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(189, 195, 199),
                Location = new Point(8, 77),
                Width = 300,
                Height = 14
            };
            panelCRControls.Controls.Add(lblPIRHeader);

            lblPIRStatus = new Label
            {
                Text = "PIR: Pending",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(318, 77),
                Width = 90,
                Height = 14
            };
            panelCRControls.Controls.Add(lblPIRStatus);

            btnPIRSuccess = new Button
            {
                Text = "✓ PIR Success",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(8, 96),
                Width = 100,
                Height = 24
            };
            btnPIRSuccess.FlatAppearance.BorderSize = 0;
            btnPIRSuccess.Click += BtnPIRSuccess_Click;
            panelCRControls.Controls.Add(btnPIRSuccess);

            btnPIRRollback = new Button
            {
                Text = "↩ PIR Rollback",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(115, 96),
                Width = 105,
                Height = 24
            };
            btnPIRRollback.FlatAppearance.BorderSize = 0;
            btnPIRRollback.Click += BtnPIRRollback_Click;
            panelCRControls.Controls.Add(btnPIRRollback);

            btnViewPIR = new Button
            {
                Text = "📄 PIR Notes",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(226, 96),
                Width = 100,
                Height = 24
            };
            btnViewPIR.FlatAppearance.BorderSize = 0;
            btnViewPIR.Click += BtnViewPIR_Click;
            panelCRControls.Controls.Add(btnViewPIR);

            this.panelTicketDetails.Controls.Add(panelCRControls);
        }

        private void ConfigureCRPanel(bool isCR)
        {
            InitializeCRPanel();

            if (isCR)
            {
                panelCRControls.Visible = true;
                txtThreadHistory.Location = new Point(15, 415);
                txtThreadHistory.Height = 132;
            }
            else
            {
                panelCRControls.Visible = false;
                txtThreadHistory.Location = new Point(15, 310);
                txtThreadHistory.Height = 237;
            }
        }

        private void DisplayCRDetails(int ticketId)
        {
            string query = "SELECT risk_score, cab_approved, maintenance_window_start, maintenance_window_end FROM change_requests WHERE ticket_id = @ticketId";
            var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                string risk = row["risk_score"].ToString();
                bool approved = Convert.ToBoolean(row["cab_approved"]);
                
                lblRisk.Text = $"Risk: {risk}";
                if (risk == "High") lblRisk.ForeColor = Color.OrangeRed;
                else if (risk == "Medium") lblRisk.ForeColor = Color.Yellow;
                else lblRisk.ForeColor = Color.LightGreen;

                lblCAB.Text = $"CAB: {(approved ? "Approved ✅" : "Pending ⏳")}";
                lblCAB.ForeColor = approved ? Color.LightGreen : Color.Orange;
                btnCABApprove.Enabled = !approved;

                if (row["maintenance_window_start"] != DBNull.Value)
                {
                    DateTime start = Convert.ToDateTime(row["maintenance_window_start"]);
                    DateTime end = Convert.ToDateTime(row["maintenance_window_end"]);
                    lblWindow.Text = $"Window: {start:MM-dd HH:mm} to {end:MM-dd HH:mm}";
                    lblWindow.ForeColor = Color.White;
                }
                else
                {
                    lblWindow.Text = "Window: None scheduled";
                    lblWindow.ForeColor = Color.Silver;
                }

                // PIR Status display
                string pirStatus = row.Table.Columns.Contains("pir_status") && row["pir_status"] != DBNull.Value
                    ? row["pir_status"].ToString() : "Pending";
                lblPIRStatus.Text = $"PIR: {pirStatus}";
                if (pirStatus == "Success") { lblPIRStatus.ForeColor = Color.LightGreen; btnPIRSuccess.Enabled = false; btnPIRRollback.Enabled = false; }
                else if (pirStatus == "Rollback") { lblPIRStatus.ForeColor = Color.OrangeRed; btnPIRSuccess.Enabled = false; btnPIRRollback.Enabled = false; }
                else { lblPIRStatus.ForeColor = Color.Silver; btnPIRSuccess.Enabled = true; btnPIRRollback.Enabled = true; }
            }
            else
            {
                string insertQuery = "INSERT INTO change_requests (ticket_id, risk_score) VALUES (@ticketId, 'Low')";
                try
                {
                    _db.ExecuteNonQuery(insertQuery, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });
                    DisplayCRDetails(ticketId);
                }
                catch { }
            }
        }

        private void BtnAssessRisk_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;

            // Q1
            string q1 = PromptDialog.ShowDialog("Is this change on Production (Type: PROD) or Staging/Dev (Type: STG)?", "Risk Profiler - Step 1/3");
            if (string.IsNullOrEmpty(q1)) return;
            int score = q1.ToUpper().Contains("PROD") ? 3 : 1;

            // Q2
            string q2 = PromptDialog.ShowDialog("What is the scope of disruption? (Type: FULL for outage, MINOR for lag)", "Risk Profiler - Step 2/3");
            if (string.IsNullOrEmpty(q2)) return;
            score += q2.ToUpper().Contains("FULL") ? 3 : 1;

            // Q3
            string q3 = PromptDialog.ShowDialog("Is there an approved rollback plan? (Type: YES or NO)", "Risk Profiler - Step 3/3");
            if (string.IsNullOrEmpty(q3)) return;
            score += q3.ToUpper().Contains("NO") ? 3 : 1;

            string risk = "Low";
            if (score >= 8) risk = "High";
            else if (score >= 5) risk = "Medium";

            string query = "UPDATE change_requests SET risk_score = @risk WHERE ticket_id = @ticketId";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@risk", risk),
                new MySqlParameter("@ticketId", _selectedTicketId)
            });

            LogAuditTrail(_selectedTicketId, "Assess Risk", $"Risk assessed as {risk} (Score: {score}/9)");
            DisplayCRDetails(_selectedTicketId);
        }

        private void BtnCABApprove_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;

            if (_userRole != "Admin" && _userRole != "Manager")
            {
                MessageBox.Show("Only CAB Board members (Admin/Manager) can approve Change Requests.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE change_requests SET cab_approved = 1 WHERE ticket_id = @ticketId";
            _db.ExecuteNonQuery(query, new MySqlParameter[] { new MySqlParameter("@ticketId", _selectedTicketId) });

            LogAuditTrail(_selectedTicketId, "CAB Approve", $"CAB Change Request approved by {_username}");
            DisplayCRDetails(_selectedTicketId);
        }

        private void BtnScheduleWindow_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;

            string startStr = PromptDialog.ShowDialog("Enter Maintenance Window Start (YYYY-MM-DD HH:MM):", "Schedule Maintenance");
            if (string.IsNullOrEmpty(startStr)) return;

            if (!DateTime.TryParse(startStr, out DateTime start))
            {
                MessageBox.Show("Invalid date format. Use YYYY-MM-DD HH:MM.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string durationStr = PromptDialog.ShowDialog("Enter Duration (in hours):", "Schedule Maintenance");
            if (string.IsNullOrEmpty(durationStr)) return;

            if (!double.TryParse(durationStr, out double hours) || hours <= 0)
            {
                MessageBox.Show("Invalid duration. Enter a positive number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime end = start.AddHours(hours);

            string query = "UPDATE change_requests SET maintenance_window_start = @start, maintenance_window_end = @end WHERE ticket_id = @ticketId";
            _db.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@start", start),
                new MySqlParameter("@end", end),
                new MySqlParameter("@ticketId", _selectedTicketId)
            });

            LogAuditTrail(_selectedTicketId, "Schedule CR", $"Maintenance window scheduled: {start:yyyy-MM-dd HH:mm} to {end:yyyy-MM-dd HH:mm}");
            DisplayCRDetails(_selectedTicketId);
        }

        private void BtnPIRSuccess_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;
            string notes = PromptDialog.ShowDialog("Enter PIR Success summary notes (optional):", "PIR — Post-Implementation Review");
            UpdatePIRStatus("Success", notes);
        }

        private void BtnPIRRollback_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;
            string notes = PromptDialog.ShowDialog("Enter Rollback reason and steps taken:", "PIR — Rollback Triggered");
            if (string.IsNullOrWhiteSpace(notes))
            {
                MessageBox.Show("Rollback notes are required to document what went wrong.", "Notes Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UpdatePIRStatus("Rollback", notes);
        }

        private void UpdatePIRStatus(string status, string notes)
        {
            try
            {
                string query = "UPDATE change_requests SET pir_status = @status, pir_notes = @notes WHERE ticket_id = @ticketId";
                _db.ExecuteNonQuery(query, new MySqlParameter[] {
                    new MySqlParameter("@status", status),
                    new MySqlParameter("@notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes),
                    new MySqlParameter("@ticketId", _selectedTicketId)
                });
                LogAuditTrail(_selectedTicketId, $"PIR {status}", notes ?? "No notes provided");
                DisplayCRDetails(_selectedTicketId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update PIR status:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewPIR_Click(object sender, EventArgs e)
        {
            if (_selectedTicketId == -1) return;
            try
            {
                string query = "SELECT pir_status, pir_notes FROM change_requests WHERE ticket_id = @ticketId";
                var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@ticketId", _selectedTicketId) });
                if (dt.Rows.Count == 0) { MessageBox.Show("No PIR record found.", "PIR Notes"); return; }
                var row = dt.Rows[0];
                string pirStatus = row["pir_status"] != DBNull.Value ? row["pir_status"].ToString() : "Pending";
                string pirNotes = row["pir_notes"] != DBNull.Value ? row["pir_notes"].ToString() : "(No notes recorded)";
                MessageBox.Show($"PIR Status: {pirStatus}\n\nNotes:\n{pirNotes}", $"PIR Report — Ticket #{_selectedTicketId}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load PIR notes:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Premium form for submitting new tickets with keyword auto-triage.
    /// </summary>
    public class NewTicketDialog : Form
    {
        private TextBox txtTitle;
        private RichTextBox txtDescription;
        private ComboBox cmbPriority;
        private Button btnSubmit;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblPriority;
        private Label lblHeader;

        public string TicketTitle { get; private set; }
        public string TicketDescription { get; private set; }
        public string TicketPriority { get; private set; }

        public NewTicketDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Report Device Issue / Create Ticket";
            this.Size = new Size(500, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(28, 32, 40);

            lblHeader = new Label
            {
                Text = "Report a Device Issue",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 128, 185),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblTitle = new Label
            {
                Text = "Issue Title:",
                ForeColor = Color.FromArgb(200, 207, 214),
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Location = new Point(20, 70),
                Width = 450,
                Height = 20
            };
            txtTitle = new TextBox
            {
                Location = new Point(20, 95),
                Width = 445,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(37, 43, 54),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPriority = new Label
            {
                Text = "Priority:",
                ForeColor = Color.FromArgb(200, 207, 214),
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Location = new Point(20, 135),
                Width = 450,
                Height = 20
            };
            cmbPriority = new ComboBox
            {
                Location = new Point(20, 160),
                Width = 200,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(37, 43, 54),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPriority.Items.AddRange(new object[] { "P1", "P2", "P3", "P4" });
            cmbPriority.SelectedIndex = 2;

            lblDescription = new Label
            {
                Text = "Description & Details:",
                ForeColor = Color.FromArgb(200, 207, 214),
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Location = new Point(20, 200),
                Width = 450,
                Height = 20
            };
            txtDescription = new RichTextBox
            {
                Location = new Point(20, 225),
                Width = 445,
                Height = 100,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(37, 43, 54),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSubmit = new Button
            {
                Text = "Submit Ticket",
                DialogResult = DialogResult.OK,
                Location = new Point(245, 340),
                Width = 105,
                Height = 32,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Please enter a title for the issue.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                TicketTitle = txtTitle.Text.Trim();
                TicketDescription = txtDescription.Text.Trim();
                TicketPriority = cmbPriority.SelectedItem.ToString();
                this.Close();
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(360, 340),
                Width = 105,
                Height = 32,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.Close(); };

            this.Controls.Add(lblHeader);
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblPriority);
            this.Controls.Add(cmbPriority);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSubmit);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnSubmit;
            this.CancelButton = btnCancel;
        }
    }

    /// <summary>
    /// Premium Toast Notification panel for active user assignment notifications.
    /// </summary>
    public class ToastNotification : Form
    {
        private Timer closeTimer;
        private Label lblBell;
        private Label lblTitle;
        private Label lblMessage;

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;
                baseParams.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return baseParams;
            }
        }

        public ToastNotification(int ticketId, string ticketTitle, string type)
        {
            this.Size = new Size(320, 90);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(37, 43, 54);

            Panel accentBorder = toPanelBorder();
            this.Controls.Add(accentBorder);

            lblBell = new Label
            {
                Text = "🔔",
                Font = new Font("Segoe UI", 16F),
                ForeColor = Color.Gold,
                Location = new Point(12, 12),
                Size = new Size(35, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblBell);

            lblTitle = new Label
            {
                Text = $"New Ticket Assigned ({type})",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 12),
                Size = new Size(260, 20)
            };
            this.Controls.Add(lblTitle);

            lblMessage = new Label
            {
                Text = $"[{ticketId}] {ticketTitle}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(200, 207, 214),
                Location = new Point(50, 34),
                Size = new Size(260, 45)
            };
            this.Controls.Add(lblMessage);

            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width - 10, workingArea.Bottom - this.Height - 10);

            closeTimer = new Timer();
            closeTimer.Interval = 4000;
            closeTimer.Tick += (s, e) => {
                closeTimer.Stop();
                this.Close();
            };
            closeTimer.Start();

            this.MouseEnter += (s, e) => closeTimer.Stop();
            this.MouseLeave += (s, e) => closeTimer.Start();
        }

        private Panel toPanelBorder()
        {
            return new Panel
            {
                BackColor = Color.FromArgb(41, 128, 185),
                Dock = DockStyle.Left,
                Width = 6
            };
        }
    }

    /// <summary>
    /// Helper helper framework to show popup prompts.
    /// </summary>
    public static class PromptDialog
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button() { Text = "Submit", Left = 280, Width = 80, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}