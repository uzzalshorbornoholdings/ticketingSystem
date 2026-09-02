using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Tasks Management Form — view and update tasks split from tickets.
    /// Can be opened globally (all tasks) or filtered to a specific ticket.
    /// </summary>
    public partial class TasksForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly int _filterTicketId; // -1 = show all tasks
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
                lblHeader.Text = $"Task Manager — Ticket #{_filterTicketId}";
            }
            LoadTasks();
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
                               t.ticket_id AS TicketID, t.created_at AS CreatedAt
                        FROM tasks t
                        WHERE t.ticket_id = @ticketId
                        ORDER BY t.created_at DESC";
                    parameters = new MySqlParameter[] { new MySqlParameter("@ticketId", _filterTicketId) };
                }
                else
                {
                    query = @"
                        SELECT t.id AS TaskID, t.title AS Title, t.status AS Status,
                               t.ticket_id AS TicketID, t.created_at AS CreatedAt,
                               tk.title AS TicketTitle
                        FROM tasks t
                        LEFT JOIN tickets tk ON t.ticket_id = tk.id
                        ORDER BY t.created_at DESC";
                    parameters = new MySqlParameter[0];
                }

                var dt = _db.ExecuteQuery(query, parameters);
                gridTasks.DataSource = dt;
                ConfigureTaskGrid();
                lblCount.Text = $"{dt.Rows.Count} task(s) found";
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

            SetColumnWidth(gridTasks, "TaskID", 55);
            SetColumnWidth(gridTasks, "Status", 100);
            SetColumnWidth(gridTasks, "TicketID", 70);
            SetColumnWidth(gridTasks, "CreatedAt", 135);

            var titleCol = FindColumn(gridTasks, "Title");
            if (titleCol != null) titleCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (_filterTicketId == -1)
            {
                SetColumnWidth(gridTasks, "TicketTitle", 150);
            }

            var dateCol = FindColumn(gridTasks, "CreatedAt");
            if (dateCol != null) dateCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Style header
            gridTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 43, 54);
            gridTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            gridTasks.EnableHeadersVisualStyles = false;

            // Row styles
            gridTasks.DefaultCellStyle.BackColor = Color.FromArgb(28, 32, 40);
            gridTasks.DefaultCellStyle.ForeColor = Color.White;
            gridTasks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            gridTasks.DefaultCellStyle.SelectionForeColor = Color.White;
            gridTasks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 47);
            gridTasks.GridColor = Color.FromArgb(50, 58, 70);
        }

        private void GridTasks_SelectionChanged(object sender, EventArgs e)
        {
            if (gridTasks.SelectedRows.Count > 0)
            {
                var row = gridTasks.SelectedRows[0];
                var idCell = FindCell(row, "TaskID");
                var statusCell = FindCell(row, "Status");
                var titleCell = FindCell(row, "Title");

                if (idCell?.Value != null && idCell.Value != DBNull.Value)
                {
                    _selectedTaskId = Convert.ToInt32(idCell.Value);
                    lblSelectedTask.Text = $"Task: {titleCell?.Value} | Status: {statusCell?.Value}";

                    string currentStatus = statusCell?.Value?.ToString() ?? "";
                    cmbTaskStatus.SelectedItem = currentStatus;
                }
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

                lblCount.ForeColor = Color.LightGreen;
                lblCount.Text = $"✅ Task #{_selectedTaskId} updated to '{newStatus}'";
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
