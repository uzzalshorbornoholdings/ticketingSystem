using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// SLA Compliance Reports & Analytics Dashboard.
    /// Provides interactive KPI views, filter controls, and PDF/Excel/CSV export.
    /// </summary>
    public partial class ReportsForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly SlaReportManager _reportManager;

        // Cached data from last query run
        private DataTable _rawTickets;
        private DataTable _detailedAuditTable;
        private SlaReportManager.SlaSummaryMetrics _summary;
        private List<SlaReportManager.PriorityBreakdownItem> _priorities;
        private List<SlaReportManager.DepartmentBreakdownItem> _departments;

        public ReportsForm(DatabaseManager db)
        {
            InitializeComponent();
            _db = db;
            _reportManager = new SlaReportManager(db);
        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            PopulateDepartmentFilter();
            RunAnalysis();
        }

        private void PopulateDepartmentFilter()
        {
            cboDepartment.Items.Clear();
            cboDepartment.Items.Add("All Departments");

            var departments = _reportManager.GetAllDepartmentNames();
            foreach (var dept in departments)
            {
                cboDepartment.Items.Add(dept);
            }

            cboDepartment.SelectedIndex = 0;
        }

        private void BtnApplyFilters_Click(object sender, EventArgs e)
        {
            RunAnalysis();
        }

        /// <summary>
        /// Core analysis pipeline: fetch → compute → display.
        /// </summary>
        private void RunAnalysis()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Parse date range filter
                DateTime? fromDate = null;
                DateTime? toDate = DateTime.Now;
                string periodLabel = cboDateRange.SelectedItem?.ToString() ?? "All Time";

                switch (periodLabel)
                {
                    case "Last 7 Days":
                        fromDate = DateTime.Now.AddDays(-7);
                        break;
                    case "Last 30 Days":
                        fromDate = DateTime.Now.AddDays(-30);
                        break;
                    case "Last 90 Days":
                        fromDate = DateTime.Now.AddDays(-90);
                        break;
                    case "All Time":
                        toDate = null;
                        break;
                }

                string deptFilter = cboDepartment.SelectedItem?.ToString();
                string prioFilter = cboPriority.SelectedItem?.ToString();

                // Build filter description
                string filterDesc = periodLabel;
                if (!string.IsNullOrEmpty(deptFilter) && deptFilter != "All Departments")
                    filterDesc += $" | Dept: {deptFilter}";
                if (!string.IsNullOrEmpty(prioFilter) && prioFilter != "All Priorities")
                    filterDesc += $" | Priority: {prioFilter}";

                // Fetch and compute
                _rawTickets = _reportManager.GetFilteredTickets(fromDate, toDate, deptFilter, prioFilter);
                _detailedAuditTable = _reportManager.GenerateDetailedSlaAuditTable(_rawTickets);
                _summary = _reportManager.ComputeSummaryMetrics(_detailedAuditTable, filterDesc);
                _priorities = _reportManager.ComputePriorityBreakdown(_detailedAuditTable);
                _departments = _reportManager.ComputeDepartmentBreakdown(_detailedAuditTable);

                // Display KPI Cards
                UpdateKpiCards();

                // Populate Priority Breakdown grid
                PopulatePriorityGrid();

                // Populate Department Breakdown grid
                PopulateDeptGrid();

                // Populate Detailed Audit grid
                gridDetailedAudit.DataSource = _detailedAuditTable;
                ConfigureDetailedAuditGrid();

                lblStatus.Text = $"{_summary.TotalTickets} tickets analyzed  |  Generated: {_summary.GeneratedAt:yyyy-MM-dd HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating SLA analytics:\n{ex.Message}", "Analysis Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateKpiCards()
        {
            if (_summary == null) return;

            lblComplianceValue.Text = $"{_summary.ComplianceRatePercentage:0.0}%";
            if (_summary.ComplianceRatePercentage >= 90)
            {
                lblComplianceValue.ForeColor = Color.FromArgb(39, 174, 96);
            }
            else if (_summary.ComplianceRatePercentage >= 75)
            {
                lblComplianceValue.ForeColor = Color.FromArgb(230, 126, 34);
            }
            else
            {
                lblComplianceValue.ForeColor = Color.FromArgb(192, 57, 43);
            }

            lblTotalValue.Text = _summary.TotalTickets.ToString();
            lblAvgTimeValue.Text = $"{_summary.AverageResolutionHours:0.0}h";
            lblBreachesValue.Text = _summary.TicketsBreached.ToString();

            if (_summary.TicketsBreached > 0)
            {
                lblBreachesValue.ForeColor = Color.FromArgb(192, 57, 43);
            }
            else
            {
                lblBreachesValue.ForeColor = Color.FromArgb(39, 174, 96);
            }
        }

        private void PopulatePriorityGrid()
        {
            var dt = new DataTable("PriorityBreakdown");
            dt.Columns.Add("Priority", typeof(string));
            dt.Columns.Add("SLA Target (Hours)", typeof(int));
            dt.Columns.Add("Total Volume", typeof(int));
            dt.Columns.Add("Within SLA", typeof(int));
            dt.Columns.Add("Breached", typeof(int));
            dt.Columns.Add("Compliance %", typeof(string));
            dt.Columns.Add("Avg Resolution (h)", typeof(string));

            foreach (var p in _priorities)
            {
                dt.Rows.Add(p.Priority, p.TargetHours, p.Total, p.WithinSla, p.Breached, $"{p.CompliancePercentage:0.0}%", $"{p.AverageHours:0.0}");
            }

            gridPriorityBreakdown.DataSource = dt;
            ConfigureBreakdownGrid(gridPriorityBreakdown);
        }

        private void PopulateDeptGrid()
        {
            var dt = new DataTable("DepartmentBreakdown");
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Total Assigned", typeof(int));
            dt.Columns.Add("Within SLA", typeof(int));
            dt.Columns.Add("Breached", typeof(int));
            dt.Columns.Add("Compliance %", typeof(string));

            foreach (var d in _departments)
            {
                dt.Rows.Add(d.DepartmentName, d.Total, d.WithinSla, d.Breached, $"{d.CompliancePercentage:0.0}%");
            }

            gridDeptBreakdown.DataSource = dt;
            ConfigureBreakdownGrid(gridDeptBreakdown);
        }

        private void ConfigureBreakdownGrid(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(3, 105, 161);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.GridColor = Color.FromArgb(226, 232, 240);

            // Color-code compliance and breach columns
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                // Color compliance % column
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    string colName = grid.Columns[i].Name;
                    if (colName.Contains("Compliance"))
                    {
                        string val = row.Cells[i].Value?.ToString() ?? "";
                        if (val.EndsWith("%"))
                        {
                            double num;
                            if (double.TryParse(val.TrimEnd('%'), out num))
                            {
                                if (num >= 90)
                                    row.Cells[i].Style.ForeColor = Color.FromArgb(22, 163, 74);
                                else if (num >= 75)
                                    row.Cells[i].Style.ForeColor = Color.FromArgb(217, 119, 6);
                                else
                                    row.Cells[i].Style.ForeColor = Color.FromArgb(220, 38, 38);
                            }
                        }
                    }

                    if (colName.Contains("Breached") || colName.Contains("Breach"))
                    {
                        int breachVal;
                        if (int.TryParse(row.Cells[i].Value?.ToString(), out breachVal) && breachVal > 0)
                        {
                            row.Cells[i].Style.ForeColor = Color.FromArgb(220, 38, 38);
                            row.Cells[i].Style.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                        }
                    }
                }
            }
        }

        private void ConfigureDetailedAuditGrid()
        {
            if (gridDetailedAudit.Columns.Count == 0) return;

            gridDetailedAudit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gridDetailedAudit.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            gridDetailedAudit.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            gridDetailedAudit.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            gridDetailedAudit.EnableHeadersVisualStyles = false;
            gridDetailedAudit.DefaultCellStyle.BackColor = Color.White;
            gridDetailedAudit.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            gridDetailedAudit.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            gridDetailedAudit.DefaultCellStyle.SelectionForeColor = Color.FromArgb(3, 105, 161);
            gridDetailedAudit.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            gridDetailedAudit.GridColor = Color.FromArgb(226, 232, 240);

            SetColumnWidth(gridDetailedAudit, "TicketID", 60);
            SetColumnWidth(gridDetailedAudit, "Title", 180);
            SetColumnWidth(gridDetailedAudit, "Type", 50);
            SetColumnWidth(gridDetailedAudit, "Priority", 55);
            SetColumnWidth(gridDetailedAudit, "Department", 110);
            SetColumnWidth(gridDetailedAudit, "Assignee", 100);
            SetColumnWidth(gridDetailedAudit, "Status", 80);
            SetColumnWidth(gridDetailedAudit, "CreatedAt", 120);
            SetColumnWidth(gridDetailedAudit, "Deadline", 120);
            SetColumnWidth(gridDetailedAudit, "ResolvedAt", 100);
            SetColumnWidth(gridDetailedAudit, "DurationHours", 75);
            SetColumnWidth(gridDetailedAudit, "SlaStatus", 80);

            // Format date columns
            var createdCol = FindColumn(gridDetailedAudit, "CreatedAt");
            if (createdCol != null) createdCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            var deadlineCol = FindColumn(gridDetailedAudit, "Deadline");
            if (deadlineCol != null) deadlineCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Color SLA Status cells
            var slaStatusCol = FindColumn(gridDetailedAudit, "SlaStatus");
            if (slaStatusCol != null)
            {
                foreach (DataGridViewRow row in gridDetailedAudit.Rows)
                {
                    if (row.IsNewRow) continue;
                    var cell = row.Cells[slaStatusCol.Index];
                    string status = cell.Value?.ToString() ?? "";
                    switch (status)
                    {
                        case "Compliant":
                            cell.Style.BackColor = Color.FromArgb(212, 239, 223);
                            cell.Style.ForeColor = Color.FromArgb(25, 111, 61);
                            cell.Style.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            break;
                        case "Breached":
                            cell.Style.BackColor = Color.FromArgb(250, 219, 216);
                            cell.Style.ForeColor = Color.FromArgb(146, 43, 33);
                            cell.Style.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            break;
                        case "At Risk":
                            cell.Style.BackColor = Color.FromArgb(252, 243, 207);
                            cell.Style.ForeColor = Color.FromArgb(125, 102, 8);
                            cell.Style.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            break;
                        case "In Progress":
                            cell.Style.BackColor = Color.FromArgb(235, 245, 251);
                            cell.Style.ForeColor = Color.FromArgb(41, 128, 185);
                            break;
                    }
                }
            }
        }

        // ===================================================================
        // EXPORT HANDLERS
        // ===================================================================

        private void BtnExportTicketPdf_Click(object sender, EventArgs e)
        {
            int selectedTicketId = -1;

            if (gridDetailedAudit.SelectedRows.Count > 0)
            {
                var row = gridDetailedAudit.SelectedRows[0];
                var idCell = row.Cells["TicketID"];
                if (idCell?.Value != null && int.TryParse(idCell.Value.ToString(), out int tid))
                {
                    selectedTicketId = tid;
                }
            }
            else if (gridDetailedAudit.CurrentRow != null)
            {
                var idCell = gridDetailedAudit.CurrentRow.Cells["TicketID"];
                if (idCell?.Value != null && int.TryParse(idCell.Value.ToString(), out int tid))
                {
                    selectedTicketId = tid;
                }
            }

            if (selectedTicketId <= 0)
            {
                MessageBox.Show("Please select a ticket record from the 'Detailed Ticket SLA Audit' tab first to generate its Incident Dossier PDF.", "Select Ticket", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabReports.SelectedTab = tabAuditTrail;
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var slaEngine = new SlaEngine(_db);
                var dossier = PdfReportExporter.FetchTicketDossier(_db, selectedTicketId, slaEngine, "Reports Dashboard");

                if (dossier == null)
                {
                    MessageBox.Show($"Ticket #{selectedTicketId} could not be found.", "Ticket Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Documents (*.pdf)|*.pdf";
                    sfd.FileName = $"Ticket_{selectedTicketId}_Incident_Dossier_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    sfd.Title = $"Export Incident Dossier for Ticket #{selectedTicketId}";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        PdfReportExporter.ExportTicketDossierToPdf(sfd.FileName, dossier);
                        lblStatus.Text = $"Ticket PDF exported: {System.IO.Path.GetFileName(sfd.FileName)}";
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        
                        var res = MessageBox.Show($"Ticket #{selectedTicketId} Dossier exported successfully!\n\nFile: {sfd.FileName}\n\nWould you like to open it now?", "Export Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (res == DialogResult.Yes)
                        {
                            try { System.Diagnostics.Process.Start(sfd.FileName); } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export Ticket Dossier PDF:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnExportPdf_Click(object sender, EventArgs e)
        {
            if (_summary == null || _detailedAuditTable == null)
            {
                MessageBox.Show("No data to export. Click 'Apply Filters' first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF Documents (*.pdf)|*.pdf";
                sfd.FileName = $"SLA_Compliance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                sfd.Title = "Export SLA Compliance Report as PDF";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        PdfReportExporter.ExportSlaReportToPdf(sfd.FileName, _summary, _priorities, _departments, _detailedAuditTable);
                        lblStatus.Text = $"PDF exported: {System.IO.Path.GetFileName(sfd.FileName)}";
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        MessageBox.Show($"PDF report exported successfully!\n\nFile: {sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export PDF:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (_summary == null || _detailedAuditTable == null)
            {
                MessageBox.Show("No data to export. Click 'Apply Filters' first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Spreadsheet (*.xls)|*.xls";
                sfd.FileName = $"SLA_Compliance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                sfd.Title = "Export SLA Report as Excel";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        // Optionally fetch audit logs for the third worksheet
                        DataTable auditLogs = null;
                        try
                        {
                            auditLogs = _db.ExecuteQuery(@"
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
                                LIMIT 1000");
                        }
                        catch { }

                        ExcelReportExporter.ExportSlaReportToExcel(sfd.FileName, _summary, _priorities, _departments, _detailedAuditTable, auditLogs);
                        lblStatus.Text = $"Excel exported: {System.IO.Path.GetFileName(sfd.FileName)}";
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        MessageBox.Show($"Excel report exported successfully!\n\nFile: {sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export Excel:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            if (_detailedAuditTable == null || _detailedAuditTable.Rows.Count == 0)
            {
                MessageBox.Show("No data to export. Click 'Apply Filters' first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = $"SLA_Ticket_Audit_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                sfd.Title = "Export SLA Audit Data as CSV";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        ExcelReportExporter.ExportToCsv(_detailedAuditTable, sfd.FileName);
                        lblStatus.Text = $"CSV exported: {System.IO.Path.GetFileName(sfd.FileName)}";
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        MessageBox.Show($"CSV file exported successfully!\n\nFile: {sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export CSV:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ===================================================================
        // GRID HELPER UTILITIES
        // ===================================================================

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
