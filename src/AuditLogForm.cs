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
                        SELECT a.id AS LogID, 
                               COALESCE(a.ticket_id, 0) AS TicketID, 
                               COALESCE(e.name, u.username, a.employee_id, 'System') AS Employee, 
                               COALESCE(u.role, 'User') AS Role,
                               a.action AS Action, 
                               COALESCE(a.details, '') AS Details,
                               a.created_at AS Timestamp
                        FROM audit_logs a
                        LEFT JOIN users u ON (a.employee_id = u.employee_id OR a.employee_id = u.username)
                        LEFT JOIN employees e ON (a.employee_id = e.id OR u.employee_id = e.id)
                        WHERE a.ticket_id = @ticketId
                        ORDER BY a.created_at DESC";
                    parameters = new MySqlParameter[] { new MySqlParameter("@ticketId", _filterTicketId) };
                }
                else
                {
                    query = @"
                        SELECT a.id AS LogID, 
                               COALESCE(a.ticket_id, 0) AS TicketID,
                               COALESCE(e.name, u.username, a.employee_id, 'System') AS Employee, 
                               COALESCE(u.role, 'User') AS Role,
                               a.action AS Action, 
                               COALESCE(a.details, '') AS Details,
                               a.created_at AS Timestamp
                        FROM audit_logs a
                        LEFT JOIN users u ON (a.employee_id = u.employee_id OR a.employee_id = u.username)
                        LEFT JOIN employees e ON (a.employee_id = e.id OR u.employee_id = e.id)
                        ORDER BY a.created_at DESC
                        LIMIT 1000";
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

        private void BtnExportLogs_Click(object sender, EventArgs e)
        {
            var dt = gridLogs.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No audit log data to export.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv|Excel Spreadsheet (*.xls)|*.xls";
                sfd.FileName = $"Audit_Logs_Export_{DateTime.Now:yyyyMMdd_HHmmss}";
                sfd.Title = "Export Audit Logs";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                        if (ext == ".xls")
                        {
                            // Simple XML Spreadsheet export for audit logs
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
                            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                            sb.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                            sb.AppendLine("  <Styles>");
                            sb.AppendLine("    <Style ss:ID=\"Default\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\"/></Style>");
                            sb.AppendLine("    <Style ss:ID=\"Header\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#34495E\" ss:Pattern=\"Solid\"/></Style>");
                            sb.AppendLine("  </Styles>");
                            sb.AppendLine("  <Worksheet ss:Name=\"Audit Logs\">");
                            sb.AppendLine("    <Table>");

                            // Headers
                            sb.AppendLine("      <Row>");
                            foreach (DataColumn col in dt.Columns)
                            {
                                sb.AppendLine($"        <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">{EscapeXml(col.ColumnName)}</Data></Cell>");
                            }
                            sb.AppendLine("      </Row>");

                            // Data
                            foreach (DataRow row in dt.Rows)
                            {
                                sb.AppendLine("      <Row>");
                                foreach (DataColumn col in dt.Columns)
                                {
                                    object val = row[col];
                                    string strVal = val == System.DBNull.Value ? "" : (val is DateTime dtv ? dtv.ToString("yyyy-MM-dd HH:mm:ss") : val.ToString());
                                    sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{EscapeXml(strVal)}</Data></Cell>");
                                }
                                sb.AppendLine("      </Row>");
                            }

                            sb.AppendLine("    </Table>");
                            sb.AppendLine("  </Worksheet>");
                            sb.AppendLine("</Workbook>");

                            System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            ExcelReportExporter.ExportToCsv(dt, sfd.FileName);
                        }

                        lblCount.Text = $"Exported {dt.Rows.Count} logs to {System.IO.Path.GetFileName(sfd.FileName)}";
                        MessageBox.Show($"Audit logs exported successfully!\n\nFile: {sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export audit logs:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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

        private static string EscapeXml(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&apos;");
        }
    }
}
