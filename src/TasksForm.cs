using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Tasks Management Form — view and update tasks split from tickets.
    /// Can be opened globally (all tasks) or filtered to a specific ticket.
    /// Supports intelligent real-time keyword search across all fields.
    /// </summary>
    public partial class TasksForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly int _filterTicketId; // -1 = show all tasks
        private const string TaskSearchPlaceholder = "Search tasks by ID, Title, Assignee, Status, Ticket, Date...";
        private DataTable _dtTasks = null;
        private int _selectedTaskId = -1;

        public TasksForm(DatabaseManager db, int filterTicketId = -1)
        {
            InitializeComponent();
            _db = db;
            _filterTicketId = filterTicketId;
        }

        private void TasksForm_Load(object sender, EventArgs e)
        {
            if (_filterTicketId != -1)
            {
                lblHeader.Text = $"📋  Task Manager — Ticket #{_filterTicketId}";
            }

            // Modern theme styling
            ModernStyle.StyleForm(this);
            panelHeader.Paint += PanelHeader_Paint;
            panelSearch.Paint += PanelCard_Paint;
            panelBottom.Paint += PanelCard_Paint;
            ModernStyle.StyleComboBox(cmbTaskStatus);
            ModernStyle.StyleButton(btnUpdateStatus, ThemeColors.SuccessGreen, ThemeColors.Darken(ThemeColors.SuccessGreen, 15), Color.White);
            ModernStyle.StyleButton(btnClose, ThemeColors.CriticalRed, ThemeColors.Darken(ThemeColors.CriticalRed, 15), Color.White);

            InitializeTaskSearch();
            LoadTasks();
        }

        private void PanelHeader_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new LinearGradientBrush(
                panel.ClientRectangle,
                ThemeColors.ElectricBlue, ThemeColors.Teal,
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void PanelCard_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using (var path = GdiPlus.CreateRoundedRectanglePath(rect, 8))
            {
                using (var brush = new SolidBrush(ThemeColors.CardSurface))
                    g.FillPath(brush, path);
                using (var pen = new Pen(ThemeColors.BorderSubtle, 1))
                    g.DrawPath(pen, path);
            }
        }

        private void InitializeTaskSearch()
        {
            ModernStyle.StyleTextBox(txtSearch);
            IntelligentSearchHelper.SetupSearchPlaceholder(txtSearch, TaskSearchPlaceholder);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyTaskSearchFilter();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ApplyTaskSearchFilter();
            }
            else if (e.KeyCode == Keys.Down && gridTasks.Rows.Count > 0)
            {
                gridTasks.Focus();
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ApplyTaskSearchFilter();
            txtSearch.Focus();
        }

        private void ApplyTaskSearchFilter()
        {
            if (_dtTasks == null) return;

            string query = IntelligentSearchHelper.GetCleanSearchQuery(txtSearch, TaskSearchPlaceholder);
            string rowFilter = IntelligentSearchHelper.BuildRowFilter(query, "TaskID", "Title", "Status", "Assignee", "TicketID", "TicketTitle", "CreatedAt");

            IntelligentSearchHelper.ApplyFilter(_dtTasks, rowFilter);

            int totalCount = _dtTasks.Rows.Count;
            int filteredCount = _dtTasks.DefaultView.Count;

            if (string.IsNullOrEmpty(query))
            {
                lblCount.ForeColor = Color.FromArgb(150, 160, 175);
                lblCount.Text = $"{totalCount} task{(totalCount == 1 ? "" : "s")} found";
                lblSearchCount.Text = string.Empty;
            }
            else
            {
                lblCount.ForeColor = ThemeColors.ElectricBlue;
                lblCount.Text = $"Showing {filteredCount} of {totalCount} task{(totalCount == 1 ? "" : "s")}";
                lblSearchCount.Text = $"Matched: {filteredCount}";
            }
        }

        private void LoadTasks()
        {
            try
            {
                string query;
                MySqlParameter[] parameters;

                if (_filterTicketId != -1)
                {
                    query = @"
                        SELECT t.id AS TaskID, t.title AS Title, t.status AS Status,
                                COALESCE(e.name, 'Unassigned') AS Assignee,
                                t.ticket_id AS TicketID, t.created_at AS CreatedAt,
                                tk.title AS TicketTitle
                        FROM tasks t
                        LEFT JOIN employees e ON t.assigned_employee_id = e.id
                        LEFT JOIN tickets tk ON t.ticket_id = tk.id
                        WHERE t.ticket_id = @ticketId
                        ORDER BY t.created_at DESC";
                    parameters = new MySqlParameter[] { new MySqlParameter("@ticketId", _filterTicketId) };
                }
                else
                {
                    query = @"
                        SELECT t.id AS TaskID, t.title AS Title, t.status AS Status,
                                COALESCE(e.name, 'Unassigned') AS Assignee,
                                t.ticket_id AS TicketID, t.created_at AS CreatedAt,
                                tk.title AS TicketTitle
                        FROM tasks t
                        LEFT JOIN employees e ON t.assigned_employee_id = e.id
                        LEFT JOIN tickets tk ON t.ticket_id = tk.id
                        ORDER BY t.created_at DESC";
                    parameters = new MySqlParameter[0];
                }

                var dt = _db.ExecuteQuery(query, parameters);
                _dtTasks = dt;
                gridTasks.DataSource = dt;
                ConfigureTaskGrid();
                ApplyTaskSearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tasks:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureTaskGrid()
        {
            if (gridTasks.Columns.Count == 0) return;
            gridTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            SetColumnWidth(gridTasks, "TaskID", 60);
            SetColumnWidth(gridTasks, "Status", 110);
            SetColumnWidth(gridTasks, "Assignee", 130);
            SetColumnWidth(gridTasks, "TicketID", 70);
            SetColumnWidth(gridTasks, "CreatedAt", 135);

            var titleCol = FindColumn(gridTasks, "Title");
            if (titleCol != null) titleCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (_filterTicketId == -1)
            {
                SetColumnWidth(gridTasks, "TicketTitle", 150);
            }
            else
            {
                var tkCol = FindColumn(gridTasks, "TicketTitle");
                if (tkCol != null) tkCol.Visible = false;
            }

            var dateCol = FindColumn(gridTasks, "CreatedAt");
            if (dateCol != null) dateCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Headers style
            gridTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            gridTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            gridTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            gridTasks.ColumnHeadersHeight = 32;
            gridTasks.EnableHeadersVisualStyles = false;

            // Rows style
            gridTasks.DefaultCellStyle.BackColor = Color.White;
            gridTasks.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            gridTasks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            gridTasks.DefaultCellStyle.SelectionForeColor = Color.FromArgb(3, 105, 161);
            gridTasks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            gridTasks.GridColor = Color.FromArgb(226, 232, 240);
            gridTasks.RowTemplate.Height = 28;

            gridTasks.CellPainting -= GridTasks_CellPainting;
            gridTasks.CellPainting += GridTasks_CellPainting;
        }

        private void GridTasks_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = gridTasks.Columns[e.ColumnIndex];
            if (col != null && string.Equals(col.Name, "Status", StringComparison.OrdinalIgnoreCase))
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string val = e.Value?.ToString() ?? "";
                Color bg = Color.FromArgb(45, 55, 72);
                Color fg = Color.White;

                string upper = val.ToUpperInvariant();
                switch (upper)
                {
                    case "PENDING":
                        bg = ThemeColors.WarningOrange;
                        fg = Color.FromArgb(40, 30, 10);
                        break;
                    case "IN PROGRESS":
                        bg = ThemeColors.ElectricBlue;
                        fg = Color.White;
                        break;
                    case "DONE":
                        bg = ThemeColors.SuccessGreen;
                        fg = Color.FromArgb(10, 40, 20);
                        break;
                    case "CANCELLED":
                        bg = ThemeColors.CriticalRed;
                        fg = Color.White;
                        break;
                }

                int badgeW = Math.Min(e.CellBounds.Width - 12, 90);
                int badgeH = 20;
                int x = e.CellBounds.X + (e.CellBounds.Width - badgeW) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - badgeH) / 2;
                var badgeRect = new Rectangle(x, y, badgeW, badgeH);

                GdiPlus.DrawStatusBadge(e.Graphics, badgeRect, val, bg, fg, 6);
            }
        }

        private void GridTasks_SelectionChanged(object sender, EventArgs e)
        {
            if (gridTasks.SelectedRows.Count > 0)
            {
                var row = gridTasks.SelectedRows[0];
                var idCell = FindCell(row, "TaskID");
                var statusCell = FindCell(row, "Status");
                var titleCell = FindCell(row, "Title");
                var assigneeCell = FindCell(row, "Assignee");

                if (idCell?.Value != null && idCell.Value != DBNull.Value)
                {
                    _selectedTaskId = Convert.ToInt32(idCell.Value);
                    string assignee = assigneeCell?.Value?.ToString() ?? "Unassigned";
                    lblSelectedTask.Text = $"Task #{_selectedTaskId}: {titleCell?.Value} (Assignee: {assignee})";
                    lblSelectedTask.ForeColor = ThemeColors.TextPrimary;

                    string currentStatus = statusCell?.Value?.ToString() ?? "";
                    cmbTaskStatus.SelectedItem = currentStatus;
                }
            }
            else
            {
                _selectedTaskId = -1;
                lblSelectedTask.Text = "Select a task to manage...";
                lblSelectedTask.ForeColor = ThemeColors.TextMuted;
            }
        }

        private void GridTasks_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.BeginInvoke(new Action(() => ConfigureTaskGrid()));
        }

        private void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (_selectedTaskId == -1)
            {
                MessageBox.Show("Please select a task to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbTaskStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newStatus = cmbTaskStatus.SelectedItem.ToString();

            try
            {
                string query = "UPDATE tasks SET status = @status WHERE id = @id";
                _db.ExecuteNonQuery(query, new MySqlParameter[] {
                    new MySqlParameter("@status", newStatus),
                    new MySqlParameter("@id", _selectedTaskId)
                });

                // Record audit log for task status update
                try
                {
                    object tIdObj = _db.ExecuteScalar("SELECT ticket_id FROM tasks WHERE id = @id", new MySqlParameter[] { new MySqlParameter("@id", _selectedTaskId) });
                    int parentTicketId = (tIdObj != null && tIdObj != DBNull.Value) ? Convert.ToInt32(tIdObj) : 0;
                    string auditSql = "INSERT INTO audit_logs (ticket_id, employee_id, action, details) VALUES (@ticketId, 'System', 'Update Sub-Task', @details)";
                    _db.ExecuteNonQuery(auditSql, new MySqlParameter[] {
                        new MySqlParameter("@ticketId", parentTicketId > 0 ? (object)parentTicketId : DBNull.Value),
                        new MySqlParameter("@details", $"Sub-task #{_selectedTaskId} status changed to '{newStatus}'")
                    });
                }
                catch { }

                ModernToast.Show(this, $"Task #{_selectedTaskId} status updated to '{newStatus}'", ToastType.Success);
                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update task:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static void SetColumnWidth(DataGridView grid, string colName, int width)
        {
            if (grid == null || grid.Columns == null) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        col.Width = width;
                        break;
                    }
                }
            }
            catch { }
        }

        private static DataGridViewColumn FindColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null) return null;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    return col;
            }
            return null;
        }

        private static DataGridViewCell FindCell(DataGridViewRow row, string colName)
        {
            if (row == null) return null;
            var grid = row.DataGridView;
            if (grid != null)
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                        return row.Cells[col.Index];
                }
            }
            return null;
        }
    }
}
