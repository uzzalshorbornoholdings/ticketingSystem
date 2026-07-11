using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Audit Log Viewer — displays the full history of all ticket actions.
    /// Can be opened globally or filtered to a specific ticket ID.
    /// </summary>
    public partial class AuditLogForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly int _filterTicketId; // -1 = show all logs

        public AuditLogForm(DatabaseManager db, int filterTicketId = -1)
        {
            InitializeComponent();
            _db = db;
            _filterTicketId = filterTicketId;
        }

        private void AuditLogForm_Load(object sender, EventArgs e)
        {
            if (_filterTicketId != -1)
            {
                lblHeader.Text = $"🔍  Audit Log — Ticket #{_filterTicketId}";
            }
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                string query;
                MySqlParameter[] parameters;

                if (_filterTicketId != -1)
                {
                    query = @"
                        SELECT a.id AS LogID, a.ticket_id AS TicketID, 
                               e.name AS Employee, u.role AS Role,
                               a.action AS Action, a.details AS Details,
                               a.created_at AS Timestamp
                        FROM audit_logs a
                        LEFT JOIN employees e ON a.employee_id = e.id
                        LEFT JOIN users u ON u.employee_id = a.employee_id
                        WHERE a.ticket_id = @ticketId
                        ORDER BY a.created_at DESC";
                    parameters = new MySqlParameter[] { new MySqlParameter("@ticketId", _filterTicketId) };
                }
                else
                {
                    query = @"
                        SELECT a.id AS LogID, a.ticket_id AS TicketID,
                               e.name AS Employee, u.role AS Role,
                               a.action AS Action, a.details AS Details,
                               a.created_at AS Timestamp
                        FROM audit_logs a
                        LEFT JOIN employees e ON a.employee_id = e.id
                        LEFT JOIN users u ON u.employee_id = a.employee_id
                        ORDER BY a.created_at DESC
                        LIMIT 500";
                    parameters = new MySqlParameter[0];
                }

                var dt = _db.ExecuteQuery(query, parameters);
                gridLogs.DataSource = dt;
                ConfigureLogGrid();
                lblCount.Text = $"{dt.Rows.Count} log entr{(dt.Rows.Count == 1 ? "y" : "ies")} found";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load audit logs:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureLogGrid()
        {
            if (gridLogs.Columns.Count == 0) return;
            gridLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            SetColumnWidth(gridLogs, "LogID", 50);
            SetColumnWidth(gridLogs, "TicketID", 65);
            SetColumnWidth(gridLogs, "Employee", 140);
            SetColumnWidth(gridLogs, "Role", 80);
            SetColumnWidth(gridLogs, "Action", 120);
            SetColumnWidth(gridLogs, "Timestamp", 135);

            var detailsCol = FindColumn(gridLogs, "Details");
            if (detailsCol != null) detailsCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var dateCol = FindColumn(gridLogs, "Timestamp");
            if (dateCol != null) dateCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            // Header style
            gridLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 43, 54);
            gridLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            gridLogs.EnableHeadersVisualStyles = false;

            // Row style
            gridLogs.DefaultCellStyle.BackColor = Color.FromArgb(28, 32, 40);
            gridLogs.DefaultCellStyle.ForeColor = Color.White;
            gridLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            gridLogs.DefaultCellStyle.SelectionForeColor = Color.White;
            gridLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 47);
            gridLogs.GridColor = Color.FromArgb(50, 58, 70);
        }

        private void GridLogs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.BeginInvoke(new Action(() => ConfigureLogGrid()));
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
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
    }
}
