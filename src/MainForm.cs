using System;
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

            LoadQueueData();
            _lockTimer.Start();
        }

        private void LoadQueueData()
        {
            gridIncidents.DataSource = FetchTicketsByType("INC");
            gridServiceRequests.DataSource = FetchTicketsByType("SR");
            gridChanges.DataSource = FetchTicketsByType("CR");

            ConfigureGrids(gridIncidents);
            ConfigureGrids(gridServiceRequests);
            ConfigureGrids(gridChanges);

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
            if (grid.Columns.Count > 0)
            {
                grid.Columns["ID"].Width = 40;
                grid.Columns["Priority"].Width = 60;
                grid.Columns["Status"].Width = 80;
                grid.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
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
            if (grid.SelectedRows.Count > 0)
            {
                var row = grid.SelectedRows[0];
                int ticketId = Convert.ToInt32(row.Cells["ID"].Value);
                _selectedTicketType = type;
                DisplayTicketDetails(ticketId);
            }
        }

        private void DisplayTicketDetails(int ticketId)
        {
            _selectedTicketId = ticketId;

            // Fetch Ticket Record
            string query = @"
                SELECT t.title, t.description, t.priority, t.status, t.created_at, 
                       t.assigned_employee_id, assignee.name AS AssigneeName,
                       t.locked_by, locker.name AS LockerName, t.locked_until
                FROM tickets t
                LEFT JOIN employees assignee ON t.assigned_employee_id = assignee.id
                LEFT JOIN employees locker ON t.locked_by = locker.id
                WHERE t.id = @id";

            var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
            if (dt.Rows.Count == 0) return;

            var row = dt.Rows[0];
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

            // Highlight cells based on SLA priority and warning alerts
            if (grid.Columns[e.ColumnIndex].Name == "Priority")
            {
                string val = e.Value?.ToString();
                if (val == "P1") e.CellStyle.ForeColor = Color.Red;
                else if (val == "P2") e.CellStyle.ForeColor = Color.Orange;
            }

            if (grid.Columns[e.ColumnIndex].Name == "Status")
            {
                // Inspect SLA warnings
                var row = grid.Rows[e.RowIndex];
                DateTime createdAt = Convert.ToDateTime(row.Cells["CreatedAt"].Value);
                string priority = row.Cells["Priority"].Value.ToString();

                var config = _slaEngine.GetSlaConfig(priority);

                if (_slaEngine.IsBreached(createdAt, null, priority))
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 210, 210); // Light Red
                    e.CellStyle.ForeColor = Color.DarkRed;
                }
                else if (_slaEngine.IsNearBreach(createdAt, priority))
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 239, 166); // Light Yellow
                    e.CellStyle.ForeColor = Color.Brown;
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
        }

        private void BtnNavTickets_Click(object sender, EventArgs e) { } // Already on tickets grid
        
        private void BtnNavTasks_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tasks List navigation clicked. Tasks are managed directly from the Ticket details splits panel.", "System Operations");
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
