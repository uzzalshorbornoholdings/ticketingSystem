using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Pure C# PDF 1.4 Document Generator for SLA Compliance, Executive Reporting, and Ticket Dossier Generation.
    /// Creates standard, cross-platform PDF files with dynamic multi-page pagination and zero external dependencies.
    /// </summary>
    public static class PdfReportExporter
    {
        private const double PageWidth = 595.28;  // A4 width in points
        private const double PageHeight = 841.89; // A4 height in points

        #region Data Models for Ticket Dossier

        public class TicketDossierData
        {
            public int TicketId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public string Department { get; set; }
            public string AssigneeName { get; set; }
            public string CreatorName { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public int SlaTargetHours { get; set; }
            public DateTime SlaDeadline { get; set; }
            public string SlaStatus { get; set; } // Compliant, Breached, At Risk, In Progress
            public string GeneratedBy { get; set; } = "System";

            // Change Request Data (if applicable)
            public bool IsChangeRequest { get; set; }
            public string RiskScore { get; set; }
            public bool CabApproved { get; set; }
            public DateTime? WindowStart { get; set; }
            public DateTime? WindowEnd { get; set; }
            public string PirStatus { get; set; }
            public string PirNotes { get; set; }

            // Sub-tasks
            public List<TicketTaskItem> Tasks { get; set; } = new List<TicketTaskItem>();

            // Attachments
            public List<TicketAttachmentItem> Attachments { get; set; } = new List<TicketAttachmentItem>();

            // Thread Comments / Audit Entries
            public List<TicketThreadItem> Threads { get; set; } = new List<TicketThreadItem>();
        }

        public class TicketTaskItem
        {
            public int TaskId { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class TicketAttachmentItem
        {
            public string FileName { get; set; }
            public long FileSizeBytes { get; set; }
            public string FileSizeFormatted { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UploadedAt { get; set; }
            public string UploadedBy { get; set; }
        }

        public class TicketThreadItem
        {
            public string AuthorName { get; set; }
            public string Role { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        #endregion

        #region PDF Low-Level Graphics & Page Stream

        public class PdfPageStream
        {
            private readonly StringBuilder _sb = new StringBuilder();

            // Coordinate conversion: converts Top-Down Y into PDF Bottom-Up Y
            public double ConvY(double topY) => PageHeight - topY;

            public void DrawRect(double x, double topY, double width, double height, double r, double g, double b, bool fill, bool stroke)
            {
                double pdfY = ConvY(topY + height);
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} {3:0.##} re", x, pdfY, width, height));
                if (fill && stroke)
                {
                    _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} rg", r, g, b));
                    _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} RG", r * 0.8, g * 0.8, b * 0.8));
                    _sb.AppendLine("b");
                }
                else if (fill)
                {
                    _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} rg", r, g, b));
                    _sb.AppendLine("f");
                }
                else if (stroke)
                {
                    _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} RG", r, g, b));
                    _sb.AppendLine("S");
                }
            }

            public void DrawLine(double x1, double topY1, double x2, double topY2, double r, double g, double b, double lineWidth = 1.0)
            {
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} w", lineWidth));
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} RG", r, g, b));
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} m {2:0.##} {3:0.##} l S", x1, ConvY(topY1), x2, ConvY(topY2)));
            }

            public void DrawText(string fontTag, double fontSize, double x, double topY, string text, double r = 0, double g = 0, double b = 0)
            {
                if (string.IsNullOrEmpty(text)) return;
                double pdfY = ConvY(topY);
                string escaped = EscapePdfText(text);

                _sb.AppendLine("BT");
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "/{0} {1:0.##} Tf", fontTag, fontSize));
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} rg", r, g, b));
                _sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} Td", x, pdfY));
                _sb.AppendLine(string.Format("({0}) Tj", escaped));
                _sb.AppendLine("ET");
            }

            public byte[] GetBytes()
            {
                return Encoding.ASCII.GetBytes(_sb.ToString());
            }
        }

        #endregion

        #region Public Export APIs

        /// <summary>
        /// Generates a professional multi-page SLA compliance report PDF.
        /// Handles dynamic pagination so all tickets and breakdowns are exported cleanly across multiple pages.
        /// </summary>
        public static void ExportSlaReportToPdf(
            string filePath,
            SlaReportManager.SlaSummaryMetrics summary,
            List<SlaReportManager.PriorityBreakdownItem> priorities,
            List<SlaReportManager.DepartmentBreakdownItem> departments,
            DataTable detailedAuditTable)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));
            if (priorities == null) priorities = new List<SlaReportManager.PriorityBreakdownItem>();
            if (departments == null) departments = new List<SlaReportManager.DepartmentBreakdownItem>();
            if (detailedAuditTable == null) detailedAuditTable = new DataTable();

            // Calculate total pages dynamically
            const int rowsPerPage = 35;
            int totalTickets = detailedAuditTable.Rows.Count;
            int auditPagesCount = totalTickets > 0 ? (int)Math.Ceiling(totalTickets / (double)rowsPerPage) : 1;
            int totalPages = 1 + auditPagesCount;

            var pages = new List<PdfPageStream>();

            // ==========================================
            // PAGE 1: Executive Dashboard & Breakdowns
            // ==========================================
            var p1 = new PdfPageStream();
            pages.Add(p1);

            // Header Banner
            p1.DrawRect(0, 0, PageWidth, 70, 0.12, 0.28, 0.49, true, false); // Navy Blue Banner
            p1.DrawText("F2", 18, 30, 32, "BITSWARD ITSM - SLA COMPLIANCE REPORT", 1, 1, 1);
            p1.DrawText("F1", 10, 30, 52, "Executive IT Service Level Agreement & Performance Intelligence", 0.85, 0.90, 0.95);

            // Metadata Strip
            p1.DrawRect(30, 80, PageWidth - 60, 24, 0.94, 0.95, 0.96, true, false);
            p1.DrawText("F1", 8.5, 38, 96, $"Report Date: {summary.GeneratedAt:yyyy-MM-dd HH:mm:ss}   |   Scope: {summary.FilterDescription}   |   Status: Executive Audit", 0.35, 0.40, 0.45);

            // Section 1: Executive KPIs
            p1.DrawText("F2", 12, 30, 125, "1. Executive Key Performance Indicators", 0.15, 0.25, 0.40);
            p1.DrawLine(30, 132, PageWidth - 30, 132, 0.80, 0.85, 0.90, 1.5);

            double cardY = 142;
            double cardW = (PageWidth - 60 - 32) / 5.0; // 5 cards with 8pt margin
            double cardH = 52;

            // Card 1: Compliance %
            double compR = summary.ComplianceRatePercentage >= 90 ? 0.15 : (summary.ComplianceRatePercentage >= 75 ? 0.85 : 0.75);
            double compG = summary.ComplianceRatePercentage >= 90 ? 0.65 : (summary.ComplianceRatePercentage >= 75 ? 0.55 : 0.15);
            double compB = summary.ComplianceRatePercentage >= 90 ? 0.25 : (summary.ComplianceRatePercentage >= 75 ? 0.10 : 0.15);
            DrawKpiCard(p1, 30 + 0 * (cardW + 8), cardY, cardW, cardH, "SLA COMPLIANCE", $"{summary.ComplianceRatePercentage:0.0}%", compR, compG, compB);

            // Card 2: Total Tickets
            DrawKpiCard(p1, 30 + 1 * (cardW + 8), cardY, cardW, cardH, "TOTAL VOLUME", summary.TotalTickets.ToString(), 0.18, 0.35, 0.55);

            // Card 3: Met SLA
            DrawKpiCard(p1, 30 + 2 * (cardW + 8), cardY, cardW, cardH, "MET SLA", summary.TicketsWithinSla.ToString(), 0.15, 0.65, 0.25);

            // Card 4: Breaches
            DrawKpiCard(p1, 30 + 3 * (cardW + 8), cardY, cardW, cardH, "BREACHES", summary.TicketsBreached.ToString(), 0.75, 0.15, 0.15);

            // Card 5: Avg Time
            DrawKpiCard(p1, 30 + 4 * (cardW + 8), cardY, cardW, cardH, "AVG RESOLUTION", $"{summary.AverageResolutionHours:0.0}h", 0.35, 0.45, 0.55);

            // Visual Compliance Progress Bar
            double barY = cardY + cardH + 8;
            double barW = PageWidth - 60;
            p1.DrawRect(30, barY, barW, 6, 0.90, 0.92, 0.94, true, false);
            double fillW = Math.Max(2, (summary.ComplianceRatePercentage / 100.0) * barW);
            p1.DrawRect(30, barY, fillW, 6, compR, compG, compB, true, false);

            // Section 2: Priority Breakdown
            double prioY = barY + 22;
            p1.DrawText("F2", 12, 30, prioY, "2. SLA Performance by Priority Level", 0.15, 0.25, 0.40);
            p1.DrawLine(30, prioY + 7, PageWidth - 30, prioY + 7, 0.80, 0.85, 0.90, 1.5);

            double tblY = prioY + 16;
            p1.DrawRect(30, tblY, PageWidth - 60, 18, 0.20, 0.30, 0.45, true, false);
            p1.DrawText("F2", 8.5, 38, tblY + 13, "PRIORITY", 1, 1, 1);
            p1.DrawText("F2", 8.5, 130, tblY + 13, "SLA TARGET", 1, 1, 1);
            p1.DrawText("F2", 8.5, 215, tblY + 13, "VOLUME", 1, 1, 1);
            p1.DrawText("F2", 8.5, 290, tblY + 13, "WITHIN SLA", 1, 1, 1);
            p1.DrawText("F2", 8.5, 375, tblY + 13, "BREACHED", 1, 1, 1);
            p1.DrawText("F2", 8.5, 455, tblY + 13, "COMPLIANCE %", 1, 1, 1);

            double rowY = tblY + 18;
            int prioRowIndex = 0;
            foreach (var p in priorities)
            {
                double bg = (prioRowIndex++ % 2 == 0) ? 0.98 : 0.94;
                p1.DrawRect(30, rowY, PageWidth - 60, 16, bg, bg, bg, true, false);
                p1.DrawText("F2", 8, 38, rowY + 12, p.Priority ?? "", 0.2, 0.2, 0.2);
                p1.DrawText("F1", 8, 135, rowY + 12, $"{p.TargetHours} Hours", 0.3, 0.3, 0.3);
                p1.DrawText("F1", 8, 225, rowY + 12, p.Total.ToString(), 0.3, 0.3, 0.3);
                p1.DrawText("F1", 8, 305, rowY + 12, p.WithinSla.ToString(), 0.15, 0.55, 0.2);
                p1.DrawText("F1", 8, 390, rowY + 12, p.Breached.ToString(), p.Breached > 0 ? 0.75 : 0.3, 0.15, 0.15);
                p1.DrawText("F2", 8, 470, rowY + 12, $"{p.CompliancePercentage:0.0}%", p.CompliancePercentage >= 90 ? 0.15 : 0.75, p.CompliancePercentage >= 90 ? 0.55 : 0.15, 0.2);
                p1.DrawLine(30, rowY + 16, PageWidth - 30, rowY + 16, 0.88, 0.90, 0.92, 0.5);
                rowY += 16;
            }

            // Section 3: Department Breakdown (Adaptive 2-column layout if >6 departments)
            double deptY = rowY + 18;
            p1.DrawText("F2", 12, 30, deptY, "3. Department Workload & Compliance", 0.15, 0.25, 0.40);
            p1.DrawLine(30, deptY + 7, PageWidth - 30, deptY + 7, 0.80, 0.85, 0.90, 1.5);

            double deptTblY = deptY + 16;
            bool useTwoCols = departments.Count > 6;

            if (!useTwoCols)
            {
                p1.DrawRect(30, deptTblY, PageWidth - 60, 18, 0.20, 0.30, 0.45, true, false);
                p1.DrawText("F2", 8.5, 38, deptTblY + 13, "DEPARTMENT NAME", 1, 1, 1);
                p1.DrawText("F2", 8.5, 240, deptTblY + 13, "TOTAL TICKETS", 1, 1, 1);
                p1.DrawText("F2", 8.5, 340, deptTblY + 13, "MET SLA", 1, 1, 1);
                p1.DrawText("F2", 8.5, 415, deptTblY + 13, "BREACHES", 1, 1, 1);
                p1.DrawText("F2", 8.5, 480, deptTblY + 13, "COMPLIANCE %", 1, 1, 1);

                double deptRowY = deptTblY + 18;
                int deptIdx = 0;
                foreach (var d in departments)
                {
                    if (deptRowY > PageHeight - 65) break;
                    double bg = (deptIdx++ % 2 == 0) ? 0.98 : 0.94;
                    p1.DrawRect(30, deptRowY, PageWidth - 60, 16, bg, bg, bg, true, false);
                    p1.DrawText("F1", 8, 38, deptRowY + 12, d.DepartmentName ?? "", 0.2, 0.2, 0.2);
                    p1.DrawText("F1", 8, 260, deptRowY + 12, d.Total.ToString(), 0.3, 0.3, 0.3);
                    p1.DrawText("F1", 8, 355, deptRowY + 12, d.WithinSla.ToString(), 0.15, 0.55, 0.2);
                    p1.DrawText("F1", 8, 430, deptRowY + 12, d.Breached.ToString(), d.Breached > 0 ? 0.75 : 0.3, 0.15, 0.15);
                    p1.DrawText("F2", 8, 495, deptRowY + 12, $"{d.CompliancePercentage:0.0}%", d.CompliancePercentage >= 90 ? 0.15 : 0.75, d.CompliancePercentage >= 90 ? 0.55 : 0.15, 0.2);
                    p1.DrawLine(30, deptRowY + 16, PageWidth - 30, deptRowY + 16, 0.88, 0.90, 0.92, 0.5);
                    deptRowY += 16;
                }
            }
            else
            {
                // 2-Column Department Grid
                double colW = (PageWidth - 60 - 15) / 2.0;

                // Col 1 Header
                p1.DrawRect(30, deptTblY, colW, 18, 0.20, 0.30, 0.45, true, false);
                p1.DrawText("F2", 8, 35, deptTblY + 13, "DEPARTMENT", 1, 1, 1);
                p1.DrawText("F2", 8, 140, deptTblY + 13, "TOTAL", 1, 1, 1);
                p1.DrawText("F2", 8, 185, deptTblY + 13, "MET", 1, 1, 1);
                p1.DrawText("F2", 8, 230, deptTblY + 13, "COMP %", 1, 1, 1);

                // Col 2 Header
                double col2X = 30 + colW + 15;
                p1.DrawRect(col2X, deptTblY, colW, 18, 0.20, 0.30, 0.45, true, false);
                p1.DrawText("F2", 8, col2X + 5, deptTblY + 13, "DEPARTMENT", 1, 1, 1);
                p1.DrawText("F2", 8, col2X + 110, deptTblY + 13, "TOTAL", 1, 1, 1);
                p1.DrawText("F2", 8, col2X + 155, deptTblY + 13, "MET", 1, 1, 1);
                p1.DrawText("F2", 8, col2X + 200, deptTblY + 13, "COMP %", 1, 1, 1);

                int half = (int)Math.Ceiling(departments.Count / 2.0);
                double dRowY1 = deptTblY + 18;
                for (int i = 0; i < half; i++)
                {
                    var d = departments[i];
                    double bg = (i % 2 == 0) ? 0.98 : 0.94;
                    p1.DrawRect(30, dRowY1, colW, 16, bg, bg, bg, true, false);
                    string name = d.DepartmentName ?? "";
                    if (name.Length > 16) name = name.Substring(0, 14) + "..";
                    p1.DrawText("F1", 7.5, 35, dRowY1 + 12, name, 0.2, 0.2, 0.2);
                    p1.DrawText("F1", 7.5, 145, dRowY1 + 12, d.Total.ToString(), 0.3, 0.3, 0.3);
                    p1.DrawText("F1", 7.5, 190, dRowY1 + 12, d.WithinSla.ToString(), 0.15, 0.55, 0.2);
                    p1.DrawText("F2", 7.5, 235, dRowY1 + 12, $"{d.CompliancePercentage:0.0}%", d.CompliancePercentage >= 90 ? 0.15 : 0.75, 0.35, 0.2);
                    p1.DrawLine(30, dRowY1 + 16, 30 + colW, dRowY1 + 16, 0.88, 0.90, 0.92, 0.5);
                    dRowY1 += 16;
                }

                double dRowY2 = deptTblY + 18;
                for (int i = half; i < departments.Count; i++)
                {
                    var d = departments[i];
                    double bg = (i % 2 == 0) ? 0.98 : 0.94;
                    p1.DrawRect(col2X, dRowY2, colW, 16, bg, bg, bg, true, false);
                    string name = d.DepartmentName ?? "";
                    if (name.Length > 16) name = name.Substring(0, 14) + "..";
                    p1.DrawText("F1", 7.5, col2X + 5, dRowY2 + 12, name, 0.2, 0.2, 0.2);
                    p1.DrawText("F1", 7.5, col2X + 115, dRowY2 + 12, d.Total.ToString(), 0.3, 0.3, 0.3);
                    p1.DrawText("F1", 7.5, col2X + 160, dRowY2 + 12, d.WithinSla.ToString(), 0.15, 0.55, 0.2);
                    p1.DrawText("F2", 7.5, col2X + 205, dRowY2 + 12, $"{d.CompliancePercentage:0.0}%", d.CompliancePercentage >= 90 ? 0.15 : 0.75, 0.35, 0.2);
                    p1.DrawLine(col2X, dRowY2 + 16, col2X + colW, dRowY2 + 16, 0.88, 0.90, 0.92, 0.5);
                    dRowY2 += 16;
                }
            }

            DrawPageFooter(p1, 1, totalPages);

            // ==============================================================
            // PAGES 2..N: Dynamic Multi-Page Detailed Ticket SLA Audit Trail
            // ==============================================================
            int currentRow = 0;
            int currentPageNum = 2;

            while (currentRow < totalTickets || (totalTickets == 0 && currentPageNum == 2))
            {
                var auditPage = new PdfPageStream();
                pages.Add(auditPage);

                // Page Header Banner
                auditPage.DrawRect(0, 0, PageWidth, 45, 0.12, 0.28, 0.49, true, false);
                auditPage.DrawText("F2", 13, 30, 28, $"4. Detailed Ticket SLA Audit Trail  (Page {currentPageNum} of {totalPages})", 1, 1, 1);

                double t2Y = 55;
                auditPage.DrawRect(30, t2Y, PageWidth - 60, 20, 0.20, 0.30, 0.45, true, false);
                auditPage.DrawText("F2", 8.5, 35, t2Y + 14, "ID", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 65, t2Y + 14, "ISSUE TITLE", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 235, t2Y + 14, "PRIORITY", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 285, t2Y + 14, "DEPARTMENT", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 390, t2Y + 14, "CREATED", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 458, t2Y + 14, "DEADLINE", 1, 1, 1);
                auditPage.DrawText("F2", 8.5, 520, t2Y + 14, "SLA STATUS", 1, 1, 1);

                double row2Y = t2Y + 20;

                if (totalTickets == 0)
                {
                    auditPage.DrawText("F1", 9, 38, row2Y + 20, "No ticket records matched the specified filter criteria.", 0.4, 0.4, 0.4);
                    DrawPageFooter(auditPage, currentPageNum, totalPages);
                    break;
                }

                int pageRows = 0;
                while (currentRow < totalTickets && pageRows < rowsPerPage)
                {
                    DataRow row = detailedAuditTable.Rows[currentRow];

                    int id = row["TicketID"] != DBNull.Value ? Convert.ToInt32(row["TicketID"]) : 0;
                    string title = row["Title"] != DBNull.Value ? row["Title"].ToString() : "Untitled";
                    if (title.Length > 30) title = title.Substring(0, 28) + "..";
                    string priority = row["Priority"] != DBNull.Value ? row["Priority"].ToString() : "P3";
                    string dept = row["Department"] != DBNull.Value ? row["Department"].ToString() : "General";
                    if (dept.Length > 17) dept = dept.Substring(0, 15) + "..";
                    string createdStr = row["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(row["CreatedAt"]).ToString("MM-dd HH:mm") : "-";
                    string deadlineStr = row["Deadline"] != DBNull.Value ? Convert.ToDateTime(row["Deadline"]).ToString("MM-dd HH:mm") : "-";
                    string slaStatus = row["SlaStatus"] != DBNull.Value ? row["SlaStatus"].ToString() : "In Progress";

                    double bg = (pageRows % 2 == 0) ? 0.98 : 0.94;
                    auditPage.DrawRect(30, row2Y, PageWidth - 60, 18, bg, bg, bg, true, false);

                    auditPage.DrawText("F2", 8, 35, row2Y + 13, $"#{id}", 0.25, 0.30, 0.35);
                    auditPage.DrawText("F1", 8, 65, row2Y + 13, title, 0.2, 0.2, 0.2);
                    auditPage.DrawText("F2", 8, 240, row2Y + 13, priority, 0.3, 0.3, 0.3);
                    auditPage.DrawText("F1", 7.5, 285, row2Y + 13, dept, 0.3, 0.3, 0.3);
                    auditPage.DrawText("F1", 7.5, 390, row2Y + 13, createdStr, 0.35, 0.35, 0.35);
                    auditPage.DrawText("F1", 7.5, 458, row2Y + 13, deadlineStr, 0.35, 0.35, 0.35);

                    // Badge color for SLA Status
                    double badgeR = 0.2, badgeG = 0.5, badgeB = 0.2;
                    if (slaStatus == "Breached") { badgeR = 0.8; badgeG = 0.15; badgeB = 0.15; }
                    else if (slaStatus == "At Risk") { badgeR = 0.75; badgeG = 0.55; badgeB = 0.1; }
                    else if (slaStatus == "In Progress") { badgeR = 0.2; badgeG = 0.45; badgeB = 0.75; }

                    auditPage.DrawText("F2", 7.5, 520, row2Y + 13, slaStatus, badgeR, badgeG, badgeB);
                    auditPage.DrawLine(30, row2Y + 18, PageWidth - 30, row2Y + 18, 0.88, 0.90, 0.92, 0.5);

                    row2Y += 18;
                    currentRow++;
                    pageRows++;
                }

                DrawPageFooter(auditPage, currentPageNum, totalPages);
                currentPageNum++;
            }

            // ==========================================
            // BUILD FINAL PDF FILE STREAM
            // ==========================================
            WritePdfFile(filePath, pages);
        }

        /// <summary>
        /// Generates a comprehensive Incident Dossier PDF for an individual ticket.
        /// Includes full metadata, change governance (if CR), sub-tasks, attachments register, and conversational timeline.
        /// </summary>
        public static void ExportTicketDossierToPdf(string filePath, TicketDossierData dossier)
        {
            if (dossier == null) throw new ArgumentNullException(nameof(dossier));

            var pages = new List<PdfPageStream>();
            var p1 = new PdfPageStream();
            pages.Add(p1);

            double curY = 0;

            // Header Banner
            p1.DrawRect(0, 0, PageWidth, 65, 0.12, 0.28, 0.49, true, false);
            p1.DrawText("F2", 16, 30, 28, "BITSWARD ITSM - TICKET INCIDENT DOSSIER", 1, 1, 1);
            p1.DrawText("F1", 9.5, 30, 48, $"Official Resolution Dossier  |  Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} by {dossier.GeneratedBy}", 0.85, 0.90, 0.95);

            curY = 75;

            // Ticket Identity & Status Strip
            p1.DrawRect(30, curY, PageWidth - 60, 42, 0.94, 0.95, 0.97, true, false);
            p1.DrawRect(30, curY, 5, 42, 0.20, 0.50, 0.80, true, false);

            string typeLabel = dossier.Type == "INC" ? "INCIDENT" : (dossier.Type == "CR" ? "CHANGE REQUEST" : "SERVICE REQUEST");
            p1.DrawText("F2", 13, 42, curY + 18, $"TICKET #{dossier.TicketId}: {dossier.Title}", 0.15, 0.25, 0.40);
            p1.DrawText("F2", 8.5, 42, curY + 34, $"TYPE: {typeLabel}   |   PRIORITY: {dossier.Priority}   |   STATUS: {dossier.Status.ToUpper()}   |   SLA: {dossier.SlaStatus.ToUpper()}", 0.35, 0.40, 0.45);

            curY += 50;

            // Metadata Grid (2 Columns)
            p1.DrawText("F2", 11, 30, curY, "1. Ticket Metadata & Assignment Information", 0.15, 0.25, 0.40);
            p1.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
            curY += 14;

            double metaH = 68;
            p1.DrawRect(30, curY, PageWidth - 60, metaH, 0.98, 0.98, 0.99, true, true);
            double col1X = 40;
            double col2X = 310;

            p1.DrawText("F2", 8.5, col1X, curY + 15, "Requester / Creator:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col1X + 95, curY + 15, dossier.CreatorName ?? "System", 0.15, 0.15, 0.15);

            p1.DrawText("F2", 8.5, col1X, curY + 32, "Assigned Engineer:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col1X + 95, curY + 32, dossier.AssigneeName ?? "Unassigned", 0.15, 0.15, 0.15);

            p1.DrawText("F2", 8.5, col1X, curY + 49, "Department:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col1X + 95, curY + 49, dossier.Department ?? "General", 0.15, 0.15, 0.15);

            p1.DrawText("F2", 8.5, col2X, curY + 15, "Created Timestamp:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col2X + 95, curY + 15, $"{dossier.CreatedAt:yyyy-MM-dd HH:mm:ss}", 0.15, 0.15, 0.15);

            p1.DrawText("F2", 8.5, col2X, curY + 32, "SLA Resolution Target:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col2X + 95, curY + 32, $"{dossier.SlaTargetHours} Business Hours", 0.15, 0.15, 0.15);

            p1.DrawText("F2", 8.5, col2X, curY + 49, "SLA Deadline:", 0.3, 0.35, 0.4);
            p1.DrawText("F1", 8.5, col2X + 95, curY + 49, $"{dossier.SlaDeadline:yyyy-MM-dd HH:mm}", 0.15, 0.15, 0.15);

            curY += metaH + 14;

            // Section 2: Problem Description
            p1.DrawText("F2", 11, 30, curY, "2. Incident & Problem Description", 0.15, 0.25, 0.40);
            p1.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
            curY += 14;

            var descLines = WrapText(dossier.Description ?? "No description provided.", PageWidth - 85, 8.5);
            double descBoxH = Math.Max(35, descLines.Count * 13 + 14);
            p1.DrawRect(30, curY, PageWidth - 60, descBoxH, 0.96, 0.97, 0.98, true, true);

            double dLineY = curY + 14;
            foreach (var line in descLines)
            {
                p1.DrawText("F1", 8.5, 40, dLineY, line, 0.2, 0.2, 0.2);
                dLineY += 13;
            }

            curY += descBoxH + 14;

            // Section 3: Change Request Governance (if applicable)
            if (dossier.IsChangeRequest)
            {
                p1.DrawText("F2", 11, 30, curY, "3. Change Management & Governance", 0.15, 0.25, 0.40);
                p1.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
                curY += 14;

                double crH = 50;
                p1.DrawRect(30, curY, PageWidth - 60, crH, 0.98, 0.98, 0.99, true, true);

                p1.DrawText("F2", 8.5, 40, curY + 15, "Risk Score:", 0.3, 0.35, 0.4);
                p1.DrawText("F2", 8.5, 120, curY + 15, dossier.RiskScore ?? "Medium", 0.8, 0.4, 0.1);

                p1.DrawText("F2", 8.5, 200, curY + 15, "CAB Approved:", 0.3, 0.35, 0.4);
                p1.DrawText("F2", 8.5, 280, curY + 15, dossier.CabApproved ? "YES (Authorized)" : "NO / Pending", dossier.CabApproved ? 0.15 : 0.75, dossier.CabApproved ? 0.55 : 0.15, 0.2);

                p1.DrawText("F2", 8.5, 40, curY + 34, "Maint. Window:", 0.3, 0.35, 0.4);
                string winStr = dossier.WindowStart.HasValue ? $"{dossier.WindowStart:yyyy-MM-dd HH:mm} - {dossier.WindowEnd:yyyy-MM-dd HH:mm}" : "Not Scheduled";
                p1.DrawText("F1", 8, 120, curY + 34, winStr, 0.2, 0.2, 0.2);

                p1.DrawText("F2", 8.5, 360, curY + 34, "PIR Status:", 0.3, 0.35, 0.4);
                p1.DrawText("F1", 8, 425, curY + 34, dossier.PirStatus ?? "Pending", 0.2, 0.2, 0.2);

                curY += crH + 14;
            }

            // Section 4: Sub-Tasks (if any)
            if (dossier.Tasks != null && dossier.Tasks.Count > 0)
            {
                p1.DrawText("F2", 11, 30, curY, "4. Sub-Tasks & Action Items", 0.15, 0.25, 0.40);
                p1.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
                curY += 14;

                p1.DrawRect(30, curY, PageWidth - 60, 16, 0.20, 0.30, 0.45, true, false);
                p1.DrawText("F2", 8, 35, curY + 11, "TASK ID", 1, 1, 1);
                p1.DrawText("F2", 8, 85, curY + 11, "TASK TITLE", 1, 1, 1);
                p1.DrawText("F2", 8, 380, curY + 11, "STATUS", 1, 1, 1);
                p1.DrawText("F2", 8, 470, curY + 11, "CREATED", 1, 1, 1);
                curY += 16;

                int tIdx = 0;
                foreach (var t in dossier.Tasks)
                {
                    double bg = (tIdx++ % 2 == 0) ? 0.98 : 0.94;
                    p1.DrawRect(30, curY, PageWidth - 60, 15, bg, bg, bg, true, false);
                    p1.DrawText("F2", 7.5, 35, curY + 11, $"#{t.TaskId}", 0.3, 0.35, 0.4);
                    string tTitle = t.Title ?? "";
                    if (tTitle.Length > 48) tTitle = tTitle.Substring(0, 45) + "..";
                    p1.DrawText("F1", 7.5, 85, curY + 11, tTitle, 0.2, 0.2, 0.2);
                    p1.DrawText("F2", 7.5, 380, curY + 11, t.Status ?? "Pending", 0.2, 0.45, 0.7);
                    p1.DrawText("F1", 7.5, 470, curY + 11, $"{t.CreatedAt:MM-dd HH:mm}", 0.35, 0.35, 0.35);
                    p1.DrawLine(30, curY + 15, PageWidth - 30, curY + 15, 0.88, 0.90, 0.92, 0.5);
                    curY += 15;
                }
                curY += 14;
            }

            // Section 5: Attachments Register (if any)
            if (dossier.Attachments != null && dossier.Attachments.Count > 0)
            {
                if (curY > PageHeight - 120)
                {
                    // Spill over to next page
                    DrawPageFooter(pages[pages.Count - 1], pages.Count, 0); // Temporary totalPages
                    var newP = new PdfPageStream();
                    pages.Add(newP);
                    DrawSubsequentPageHeader(newP, dossier.TicketId, dossier.Title);
                    curY = 55;
                }

                var activePage = pages[pages.Count - 1];
                activePage.DrawText("F2", 11, 30, curY, "5. Attached Files & Screenshots", 0.15, 0.25, 0.40);
                activePage.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
                curY += 14;

                activePage.DrawRect(30, curY, PageWidth - 60, 16, 0.20, 0.30, 0.45, true, false);
                activePage.DrawText("F2", 8, 35, curY + 11, "FILE NAME", 1, 1, 1);
                activePage.DrawText("F2", 8, 340, curY + 11, "SIZE", 1, 1, 1);
                activePage.DrawText("F2", 8, 410, curY + 11, "UPLOADED BY", 1, 1, 1);
                activePage.DrawText("F2", 8, 485, curY + 11, "DATE", 1, 1, 1);
                curY += 16;

                int aIdx = 0;
                foreach (var att in dossier.Attachments)
                {
                    double bg = (aIdx++ % 2 == 0) ? 0.98 : 0.94;
                    activePage.DrawRect(30, curY, PageWidth - 60, 15, bg, bg, bg, true, false);
                    string fn = att.FileName ?? "";
                    if (fn.Length > 45) fn = fn.Substring(0, 42) + "..";
                    activePage.DrawText("F1", 7.5, 35, curY + 11, fn, 0.2, 0.2, 0.2);
                    activePage.DrawText("F1", 7.5, 340, curY + 11, att.FileSizeFormatted ?? "", 0.3, 0.3, 0.3);
                    activePage.DrawText("F1", 7.5, 410, curY + 11, att.UploadedBy ?? "System", 0.3, 0.3, 0.3);
                    activePage.DrawText("F1", 7.5, 485, curY + 11, $"{att.UploadedAt:MM-dd HH:mm}", 0.35, 0.35, 0.35);
                    activePage.DrawLine(30, curY + 15, PageWidth - 30, curY + 15, 0.88, 0.90, 0.92, 0.5);
                    curY += 15;
                }
                curY += 14;
            }

            // Section 6: Conversational Audit Trail / Timeline
            if (curY > PageHeight - 140)
            {
                var newP = new PdfPageStream();
                pages.Add(newP);
                DrawSubsequentPageHeader(newP, dossier.TicketId, dossier.Title);
                curY = 55;
            }

            var timelinePage = pages[pages.Count - 1];
            timelinePage.DrawText("F2", 11, 30, curY, "6. Activity Log & Collaboration Thread", 0.15, 0.25, 0.40);
            timelinePage.DrawLine(30, curY + 5, PageWidth - 30, curY + 5, 0.80, 0.85, 0.90, 1.2);
            curY += 16;

            if (dossier.Threads == null || dossier.Threads.Count == 0)
            {
                timelinePage.DrawText("F1", 8.5, 40, curY + 10, "No commentary or thread entries recorded for this ticket.", 0.45, 0.45, 0.45);
            }
            else
            {
                foreach (var th in dossier.Threads)
                {
                    var msgLines = WrapText(th.Message ?? "", PageWidth - 90, 8);
                    double cardH = Math.Max(28, msgLines.Count * 11 + 18);

                    if (curY + cardH > PageHeight - 65)
                    {
                        var nextP = new PdfPageStream();
                        pages.Add(nextP);
                        DrawSubsequentPageHeader(nextP, dossier.TicketId, dossier.Title);
                        curY = 55;
                    }

                    var curPage = pages[pages.Count - 1];
                    curPage.DrawRect(30, curY, PageWidth - 60, cardH, 0.97, 0.98, 0.99, true, true);
                    curPage.DrawRect(30, curY, 3, cardH, 0.20, 0.45, 0.75, true, false);

                    curPage.DrawText("F2", 8, 38, curY + 11, $"{th.AuthorName ?? "System"} ({th.Role ?? "User"})", 0.2, 0.35, 0.55);
                    curPage.DrawText("F1", 7.5, PageWidth - 140, curY + 11, $"{th.CreatedAt:yyyy-MM-dd HH:mm}", 0.5, 0.5, 0.5);

                    double lineY = curY + 23;
                    foreach (var line in msgLines)
                    {
                        curPage.DrawText("F1", 8, 38, lineY, line, 0.2, 0.2, 0.2);
                        lineY += 11;
                    }

                    curY += cardH + 6;
                }
            }

            // Draw footers on all pages with accurate total page count
            int finalTotalPages = pages.Count;
            for (int i = 0; i < finalTotalPages; i++)
            {
                DrawPageFooter(pages[i], i + 1, finalTotalPages);
            }

            // Build PDF binary
            WritePdfFile(filePath, pages);
        }

        /// <summary>
        /// Helper routine to fetch all related ticket data and construct a TicketDossierData model.
        /// </summary>
        public static TicketDossierData FetchTicketDossier(DatabaseManager db, int ticketId, SlaEngine slaEngine, string generatedBy = "System")
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            string query = @"
                SELECT 
                    t.id AS TicketID,
                    t.title AS Title,
                    t.description AS Description,
                    t.type AS Type,
                    t.priority AS Priority,
                    t.status AS Status,
                    t.created_at AS CreatedAt,
                    t.updated_at AS UpdatedAt,
                    COALESCE(d.name, 'General') AS Department,
                    COALESCE(assignee.name, 'Unassigned') AS AssigneeName,
                    COALESCE(creator.name, 'System') AS CreatorName,
                    COALESCE(s.resolution_hours, 24) AS SlaHours
                FROM tickets t
                LEFT JOIN employees assignee ON t.assigned_employee_id = assignee.id
                LEFT JOIN departments d ON assignee.department_id = d.id
                LEFT JOIN employees creator ON t.creator_employee_id = creator.id
                LEFT JOIN slas s ON t.sla_id = s.id
                WHERE t.id = @id";

            var dt = db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            var dossier = new TicketDossierData
            {
                TicketId = ticketId,
                Title = row["Title"]?.ToString(),
                Description = row["Description"]?.ToString(),
                Type = row["Type"]?.ToString(),
                Priority = row["Priority"]?.ToString(),
                Status = row["Status"]?.ToString(),
                Department = row["Department"]?.ToString(),
                AssigneeName = row["AssigneeName"]?.ToString(),
                CreatorName = row["CreatorName"]?.ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : Convert.ToDateTime(row["CreatedAt"]),
                SlaTargetHours = Convert.ToInt32(row["SlaHours"]),
                GeneratedBy = generatedBy
            };

            // SLA calculation
            if (slaEngine != null)
            {
                dossier.SlaDeadline = slaEngine.CalculateDeadline(dossier.CreatedAt, dossier.SlaTargetHours);
                bool isResolved = string.Equals(dossier.Status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(dossier.Status, "Closed", StringComparison.OrdinalIgnoreCase);
                if (isResolved)
                {
                    dossier.SlaStatus = dossier.UpdatedAt <= dossier.SlaDeadline ? "Compliant" : "Breached";
                }
                else
                {
                    if (DateTime.Now > dossier.SlaDeadline) dossier.SlaStatus = "Breached";
                    else if (slaEngine.IsNearBreach(dossier.CreatedAt, dossier.Priority)) dossier.SlaStatus = "At Risk";
                    else dossier.SlaStatus = "In Progress";
                }
            }

            // CR Details
            if (dossier.Type == "CR")
            {
                dossier.IsChangeRequest = true;
                try
                {
                    var crDt = db.ExecuteQuery("SELECT risk_score, cab_approved, maintenance_window_start, maintenance_window_end, pir_status, pir_notes FROM change_requests WHERE ticket_id = @id", new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
                    if (crDt.Rows.Count > 0)
                    {
                        var crRow = crDt.Rows[0];
                        dossier.RiskScore = crRow["risk_score"]?.ToString();
                        dossier.CabApproved = crRow["cab_approved"] != DBNull.Value && Convert.ToBoolean(crRow["cab_approved"]);
                        if (crRow["maintenance_window_start"] != DBNull.Value) dossier.WindowStart = Convert.ToDateTime(crRow["maintenance_window_start"]);
                        if (crRow["maintenance_window_end"] != DBNull.Value) dossier.WindowEnd = Convert.ToDateTime(crRow["maintenance_window_end"]);
                        dossier.PirStatus = crRow["pir_status"]?.ToString();
                        dossier.PirNotes = crRow["pir_notes"]?.ToString();
                    }
                }
                catch { }
            }

            // Tasks
            try
            {
                var tasksDt = db.ExecuteQuery("SELECT id, title, status, created_at FROM tasks WHERE ticket_id = @id ORDER BY created_at ASC", new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
                foreach (DataRow tRow in tasksDt.Rows)
                {
                    dossier.Tasks.Add(new TicketTaskItem
                    {
                        TaskId = Convert.ToInt32(tRow["id"]),
                        Title = tRow["title"]?.ToString(),
                        Status = tRow["status"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(tRow["created_at"])
                    });
                }
            }
            catch { }

            // Attachments
            try
            {
                var attDt = db.ExecuteQuery(@"
                    SELECT a.file_name, a.file_size, a.created_at, 
                           COALESCE(e.name, u.username, a.employee_id, 'System') AS UploadedBy
                    FROM ticket_attachments a
                    LEFT JOIN users u ON (a.employee_id = u.employee_id OR a.employee_id = u.username)
                    LEFT JOIN employees e ON (a.employee_id = e.id OR u.employee_id = e.id)
                    WHERE a.ticket_id = @id
                    ORDER BY a.created_at ASC", new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
                foreach (DataRow aRow in attDt.Rows)
                {
                    long size = aRow["file_size"] != DBNull.Value ? Convert.ToInt64(aRow["file_size"]) : 0;
                    dossier.Attachments.Add(new TicketAttachmentItem
                    {
                        FileName = aRow["file_name"]?.ToString(),
                        FileSizeBytes = size,
                        FileSizeFormatted = AttachmentManager.FormatFileSize(size),
                        CreatedAt = Convert.ToDateTime(aRow["created_at"]),
                        UploadedAt = Convert.ToDateTime(aRow["created_at"]),
                        UploadedBy = aRow["UploadedBy"]?.ToString()
                    });
                }
            }
            catch { }

            // Thread Messages
            try
            {
                var threadDt = db.ExecuteQuery(@"
                    SELECT th.message, th.created_at,
                           COALESCE(e.name, u.username, th.employee_id, 'System') AS AuthorName,
                           COALESCE(u.role, 'User') AS Role
                    FROM ticket_threads th
                    LEFT JOIN users u ON (th.employee_id = u.employee_id OR th.employee_id = u.username)
                    LEFT JOIN employees e ON (th.employee_id = e.id OR u.employee_id = e.id)
                    WHERE th.ticket_id = @id
                    ORDER BY th.created_at ASC", new MySqlParameter[] { new MySqlParameter("@id", ticketId) });
                foreach (DataRow thRow in threadDt.Rows)
                {
                    dossier.Threads.Add(new TicketThreadItem
                    {
                        AuthorName = thRow["AuthorName"]?.ToString(),
                        Role = thRow["Role"]?.ToString(),
                        Message = thRow["message"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(thRow["created_at"])
                    });
                }
            }
            catch { }

            return dossier;
        }

        #endregion

        #region PDF Rendering Helpers

        private static void DrawSubsequentPageHeader(PdfPageStream page, int ticketId, string title)
        {
            page.DrawRect(0, 0, PageWidth, 40, 0.12, 0.28, 0.49, true, false);
            string cleanTitle = title ?? "";
            if (cleanTitle.Length > 40) cleanTitle = cleanTitle.Substring(0, 37) + "..";
            page.DrawText("F2", 11, 30, 25, $"BITSWARD ITSM  |  Ticket #{ticketId}: {cleanTitle} (Continued)", 1, 1, 1);
        }

        private static void DrawKpiCard(PdfPageStream page, double x, double topY, double width, double height, string label, string value, double r, double g, double b)
        {
            page.DrawRect(x, topY, width, height, 0.95, 0.96, 0.97, true, false);
            page.DrawRect(x, topY, width, 3.5, r, g, b, true, false);
            page.DrawText("F2", 7.5, x + 6, topY + 18, label, 0.45, 0.50, 0.55);
            page.DrawText("F2", 13, x + 6, topY + 40, value, r, g, b);
        }

        private static void DrawPageFooter(PdfPageStream page, int pageNum, int totalPages)
        {
            double footerY = PageHeight - 35;
            page.DrawLine(30, footerY, PageWidth - 30, footerY, 0.80, 0.85, 0.90, 0.75);
            page.DrawText("F1", 8, 30, footerY + 16, "Bitsward ITSM Enterprise Compliance Engine  |  Confidential & Proprietary", 0.50, 0.55, 0.60);
            page.DrawText("F1", 8, PageWidth - 85, footerY + 16, $"Page {pageNum} of {totalPages}", 0.50, 0.55, 0.60);
        }

        private static void WritePdfFile(string filePath, List<PdfPageStream> pages)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fs))
            {
                var offsets = new List<long>();

                Action<string> WriteStr = s => {
                    byte[] bytes = Encoding.ASCII.GetBytes(s);
                    bw.Write(bytes);
                };

                // 1. PDF Header
                WriteStr("%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");

                // 2. Catalog (Obj 1)
                offsets.Add(fs.Position);
                WriteStr("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

                // 3. Pages Tree (Obj 2)
                offsets.Add(fs.Position);
                var kidsSb = new StringBuilder();
                int pageCount = pages.Count;
                for (int i = 0; i < pageCount; i++)
                {
                    int pageObjId = 6 + i * 2;
                    kidsSb.Append($"{pageObjId} 0 R ");
                }
                WriteStr($"2 0 obj\n<< /Type /Pages /Kids [{kidsSb.ToString().TrimEnd()}] /Count {pageCount} >>\nendobj\n");

                // 4. Font Helvetica (Obj 3)
                offsets.Add(fs.Position);
                WriteStr("3 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

                // 5. Font Helvetica-Bold (Obj 4)
                offsets.Add(fs.Position);
                WriteStr("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

                // 6. Shared Resources Dictionary (Obj 5) with Font & ProcSet
                offsets.Add(fs.Position);
                WriteStr("5 0 obj\n<< /Font << /F1 3 0 R /F2 4 0 R >> /ProcSet [/PDF /Text] >>\nendobj\n");

                // 7. Pages & Content Streams (Starting at Obj 6)
                for (int i = 0; i < pageCount; i++)
                {
                    int pageObjId = 6 + i * 2;
                    int contentObjId = pageObjId + 1;
                    byte[] contentBytes = pages[i].GetBytes();

                    // Page Object
                    offsets.Add(fs.Position);
                    WriteStr($"{pageObjId} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth.ToString("0.##", CultureInfo.InvariantCulture)} {PageHeight.ToString("0.##", CultureInfo.InvariantCulture)}] /Contents {contentObjId} 0 R /Resources 5 0 R >>\nendobj\n");

                    // Content Stream Object
                    offsets.Add(fs.Position);
                    WriteStr($"{contentObjId} 0 obj\n<< /Length {contentBytes.Length} >>\nstream\r\n");
                    bw.Write(contentBytes);
                    WriteStr("\r\nendstream\nendobj\n");
                }

                // 8. Cross-Reference Table
                long xrefOffset = fs.Position;
                int totalObjects = offsets.Count + 1;
                WriteStr($"xref\r\n0 {totalObjects}\r\n");
                WriteStr("0000000000 65535 f \r\n");
                foreach (long offset in offsets)
                {
                    WriteStr(string.Format(CultureInfo.InvariantCulture, "{0:D10} 00000 n\r\n", offset));
                }

                // 9. Trailer
                WriteStr($"trailer\r\n<< /Size {totalObjects} /Root 1 0 R >>\r\nstartxref\r\n{xrefOffset}\r\n%%EOF\r\n");
            }
        }

        private static List<string> WrapText(string text, double maxWidth, double fontSize)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            double avgCharWidth = fontSize * 0.52;
            int maxCharsPerLine = Math.Max(10, (int)(maxWidth / avgCharWidth));

            string[] rawLines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            foreach (var rawLine in rawLines)
            {
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    lines.Add("");
                    continue;
                }

                string[] words = rawLine.Split(' ');
                var currentLine = new StringBuilder();

                foreach (var word in words)
                {
                    if (currentLine.Length == 0)
                    {
                        if (word.Length > maxCharsPerLine)
                        {
                            for (int i = 0; i < word.Length; i += maxCharsPerLine)
                            {
                                int len = Math.Min(maxCharsPerLine, word.Length - i);
                                lines.Add(word.Substring(i, len));
                            }
                        }
                        else
                        {
                            currentLine.Append(word);
                        }
                    }
                    else if (currentLine.Length + 1 + word.Length <= maxCharsPerLine)
                    {
                        currentLine.Append(" " + word);
                    }
                    else
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        currentLine.Append(word);
                    }
                }

                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine.ToString());
                }
            }

            return lines;
        }

        private static string EscapePdfText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c == '(') sb.Append("\\(");
                else if (c == ')') sb.Append("\\)");
                else if (c == '\\') sb.Append("\\\\");
                else if (c >= 32 && c <= 126) sb.Append(c);
                else if (c == '—' || c == '–') sb.Append("-");
                else if (c == '“' || c == '”') sb.Append("\"");
                else if (c == '‘' || c == '’') sb.Append("'");
                else if (c == '•') sb.Append("*");
                else sb.Append(' ');
            }
            return sb.ToString();
        }

        #endregion
    }
}
