using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Pure C# PDF 1.4 Document Generator for SLA Compliance and Executive Reporting.
    /// Creates standard, cross-platform PDF files with dynamic multi-page pagination and zero dependencies.
    /// </summary>
    public static class PdfReportExporter
    {
        private const double PageWidth = 595.28;  // A4 width in points
        private const double PageHeight = 841.89; // A4 height in points

        private class PdfPageStream
        {
            private readonly StringBuilder _sb = new StringBuilder();

            // Coordinate conversion: converts Top-Down Y into PDF Bottom-Up Y
            private double ConvY(double topY) => PageHeight - topY;

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

        /// <summary>
        /// Generates a professional multi-page SLA compliance report PDF.
        /// Handles dynamic pagination so all tickets are exported across multiple pages without truncation.
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
            double cardH = 55;

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

            // Section 2: Priority Breakdown
            double prioY = 220;
            p1.DrawText("F2", 12, 30, prioY, "2. SLA Performance by Priority Level", 0.15, 0.25, 0.40);
            p1.DrawLine(30, prioY + 7, PageWidth - 30, prioY + 7, 0.80, 0.85, 0.90, 1.5);

            double tblY = prioY + 18;
            p1.DrawRect(30, tblY, PageWidth - 60, 20, 0.20, 0.30, 0.45, true, false);
            p1.DrawText("F2", 9, 38, tblY + 14, "PRIORITY", 1, 1, 1);
            p1.DrawText("F2", 9, 130, tblY + 14, "SLA TARGET", 1, 1, 1);
            p1.DrawText("F2", 9, 215, tblY + 14, "VOLUME", 1, 1, 1);
            p1.DrawText("F2", 9, 290, tblY + 14, "WITHIN SLA", 1, 1, 1);
            p1.DrawText("F2", 9, 375, tblY + 14, "BREACHED", 1, 1, 1);
            p1.DrawText("F2", 9, 455, tblY + 14, "COMPLIANCE %", 1, 1, 1);

            double rowY = tblY + 20;
            int prioRowIndex = 0;
            foreach (var p in priorities)
            {
                double bg = (prioRowIndex++ % 2 == 0) ? 0.98 : 0.94;
                p1.DrawRect(30, rowY, PageWidth - 60, 18, bg, bg, bg, true, false);
                p1.DrawText("F2", 8.5, 38, rowY + 13, p.Priority ?? "", 0.2, 0.2, 0.2);
                p1.DrawText("F1", 8.5, 135, rowY + 13, $"{p.TargetHours} Hours", 0.3, 0.3, 0.3);
                p1.DrawText("F1", 8.5, 225, rowY + 13, p.Total.ToString(), 0.3, 0.3, 0.3);
                p1.DrawText("F1", 8.5, 305, rowY + 13, p.WithinSla.ToString(), 0.15, 0.55, 0.2);
                p1.DrawText("F1", 8.5, 390, rowY + 13, p.Breached.ToString(), p.Breached > 0 ? 0.75 : 0.3, 0.15, 0.15);
                p1.DrawText("F2", 8.5, 470, rowY + 13, $"{p.CompliancePercentage:0.0}%", p.CompliancePercentage >= 90 ? 0.15 : 0.75, p.CompliancePercentage >= 90 ? 0.55 : 0.15, 0.2);
                p1.DrawLine(30, rowY + 18, PageWidth - 30, rowY + 18, 0.88, 0.90, 0.92, 0.5);
                rowY += 18;
            }

            // Section 3: Department Breakdown
            double deptY = rowY + 20;
            p1.DrawText("F2", 12, 30, deptY, "3. Department Workload & Compliance", 0.15, 0.25, 0.40);
            p1.DrawLine(30, deptY + 7, PageWidth - 30, deptY + 7, 0.80, 0.85, 0.90, 1.5);

            double deptTblY = deptY + 18;
            p1.DrawRect(30, deptTblY, PageWidth - 60, 20, 0.20, 0.30, 0.45, true, false);
            p1.DrawText("F2", 9, 38, deptTblY + 14, "DEPARTMENT NAME", 1, 1, 1);
            p1.DrawText("F2", 9, 240, deptTblY + 14, "TOTAL TICKETS", 1, 1, 1);
            p1.DrawText("F2", 9, 340, deptTblY + 14, "MET SLA", 1, 1, 1);
            p1.DrawText("F2", 9, 415, deptTblY + 14, "BREACHES", 1, 1, 1);
            p1.DrawText("F2", 9, 480, deptTblY + 14, "COMPLIANCE %", 1, 1, 1);

            double deptRowY = deptTblY + 20;
            int deptIdx = 0;
            foreach (var d in departments)
            {
                if (deptRowY > PageHeight - 70) break; // Don't overflow bottom margin
                double bg = (deptIdx++ % 2 == 0) ? 0.98 : 0.94;
                p1.DrawRect(30, deptRowY, PageWidth - 60, 18, bg, bg, bg, true, false);
                p1.DrawText("F1", 8.5, 38, deptRowY + 13, d.DepartmentName ?? "", 0.2, 0.2, 0.2);
                p1.DrawText("F1", 8.5, 260, deptRowY + 13, d.Total.ToString(), 0.3, 0.3, 0.3);
                p1.DrawText("F1", 8.5, 355, deptRowY + 13, d.WithinSla.ToString(), 0.15, 0.55, 0.2);
                p1.DrawText("F1", 8.5, 430, deptRowY + 13, d.Breached.ToString(), d.Breached > 0 ? 0.75 : 0.3, 0.15, 0.15);
                p1.DrawText("F2", 8.5, 495, deptRowY + 13, $"{d.CompliancePercentage:0.0}%", d.CompliancePercentage >= 90 ? 0.15 : 0.75, d.CompliancePercentage >= 90 ? 0.55 : 0.15, 0.2);
                p1.DrawLine(30, deptRowY + 18, PageWidth - 30, deptRowY + 18, 0.88, 0.90, 0.92, 0.5);
                deptRowY += 18;
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
            // BUILD FINAL PDF FILE STREAM (STRICT PDF 1.4)
            // ==========================================
            WritePdfFile(filePath, pages);
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
    }
}
