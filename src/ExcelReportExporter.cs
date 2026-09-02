using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Generates multi-worksheet formatted Excel spreadsheets (SpreadsheetML .xls) and CSV files.
    /// Fully native with zero external dependencies and strict invariant culture formatting.
    /// </summary>
    public static class ExcelReportExporter
    {
        /// <summary>
        /// Exports the complete SLA compliance report into a multi-sheet styled XML Spreadsheet (.xls).
        /// </summary>
        public static void ExportSlaReportToExcel(
            string filePath,
            SlaReportManager.SlaSummaryMetrics summary,
            List<SlaReportManager.PriorityBreakdownItem> priorities,
            List<SlaReportManager.DepartmentBreakdownItem> departments,
            DataTable detailedAuditTable,
            DataTable auditLogsTable = null)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));
            if (priorities == null) priorities = new List<SlaReportManager.PriorityBreakdownItem>();
            if (departments == null) departments = new List<SlaReportManager.DepartmentBreakdownItem>();
            if (detailedAuditTable == null) detailedAuditTable = new DataTable();

            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            sb.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            sb.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");

            // Define Styles
            sb.AppendLine("  <Styles>");
            sb.AppendLine("    <Style ss:ID=\"Default\" ss:Name=\"Normal\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Color=\"#333333\"/></Style>");
            
            // Title Style
            sb.AppendLine("    <Style ss:ID=\"TitleStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"15\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#1F497D\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/></Style>");
            
            // Section Header Style
            sb.AppendLine("    <Style ss:ID=\"SectionHeaderStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"11\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#2980B9\" ss:Pattern=\"Solid\"/><Alignment ss:Vertical=\"Center\"/></Style>");
            
            // Grid Header Style
            sb.AppendLine("    <Style ss:ID=\"HeaderStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#34495E\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#BDC3C7\"/></Borders></Style>");
            
            // KPI Label & Value Styles
            sb.AppendLine("    <Style ss:ID=\"KpiLabelStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9\" ss:Color=\"#7F8C8D\"/><Interior ss:Color=\"#EAEDED\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"KpiValueStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"14\" ss:Bold=\"1\" ss:Color=\"#2C3E50\"/><Interior ss:Color=\"#EAEDED\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"KpiGreenStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"14\" ss:Bold=\"1\" ss:Color=\"#27AE60\"/><Interior ss:Color=\"#EAEDED\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"KpiRedStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"14\" ss:Bold=\"1\" ss:Color=\"#C0392B\"/><Interior ss:Color=\"#EAEDED\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/></Style>");

            // Status Cell Styles
            sb.AppendLine("    <Style ss:ID=\"CompliantCell\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9.5\" ss:Bold=\"1\" ss:Color=\"#196F3D\"/><Interior ss:Color=\"#D4EFDF\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"BreachedCell\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9.5\" ss:Bold=\"1\" ss:Color=\"#922B21\"/><Interior ss:Color=\"#FADBD8\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"AtRiskCell\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9.5\" ss:Bold=\"1\" ss:Color=\"#7D6608\"/><Interior ss:Color=\"#FCF3CF\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"InProgressCell\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9.5\" ss:Color=\"#2980B9\"/><Interior ss:Color=\"#EBF5FB\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\"/></Style>");

            // Generic Data Styles
            sb.AppendLine("    <Style ss:ID=\"DataCenter\"><Alignment ss:Horizontal=\"Center\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"DataRight\"><Alignment ss:Horizontal=\"Right\"/></Style>");
            sb.AppendLine("    <Style ss:ID=\"MetaStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"9\" ss:Italic=\"1\" ss:Color=\"#7F8C8D\"/></Style>");
            sb.AppendLine("  </Styles>");

            // ==========================================
            // WORKSHEET 1: SLA Summary & KPIs
            // ==========================================
            sb.AppendLine("  <Worksheet ss:Name=\"SLA Summary and KPIs\">");
            sb.AppendLine("    <Table ss:DefaultRowHeight=\"20\">");
            sb.AppendLine("      <Column ss:Width=\"140\"/>");
            sb.AppendLine("      <Column ss:Width=\"110\"/>");
            sb.AppendLine("      <Column ss:Width=\"110\"/>");
            sb.AppendLine("      <Column ss:Width=\"110\"/>");
            sb.AppendLine("      <Column ss:Width=\"120\"/>");
            sb.AppendLine("      <Column ss:Width=\"120\"/>");

            // Title Banner
            sb.AppendLine("      <Row ss:Height=\"36\">");
            sb.AppendLine("        <Cell ss:MergeAcross=\"5\" ss:StyleID=\"TitleStyle\"><Data ss:Type=\"String\">BITSWARD ITSM - SLA COMPLIANCE &amp; EXECUTIVE SUMMARY</Data></Cell>");
            sb.AppendLine("      </Row>");

            // Metadata Row
            sb.AppendLine("      <Row ss:Height=\"22\">");
            sb.AppendLine($"        <Cell ss:MergeAcross=\"5\" ss:StyleID=\"MetaStyle\"><Data ss:Type=\"String\">Generated: {summary.GeneratedAt:yyyy-MM-dd HH:mm:ss}  |  Period: {EscapeXml(summary.FilterDescription)}  |  Classification: Executive Internal</Data></Cell>");
            sb.AppendLine("      </Row>");
            sb.AppendLine("      <Row ss:Height=\"10\"></Row>"); // Empty separator

            // KPI Scorecards Block (2 Rows)
            sb.AppendLine("      <Row ss:Height=\"18\">");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">TOTAL TICKETS</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">COMPLIANCE RATE</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">WITHIN SLA</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">SLA BREACHES</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">AT RISK / NEAR BREACH</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"KpiLabelStyle\"><Data ss:Type=\"String\">AVG RESOLUTION (HRS)</Data></Cell>");
            sb.AppendLine("      </Row>");

            string compStyle = summary.ComplianceRatePercentage >= 90 ? "KpiGreenStyle" : (summary.ComplianceRatePercentage >= 75 ? "KpiValueStyle" : "KpiRedStyle");
            string compStr = summary.ComplianceRatePercentage.ToString("0.0", CultureInfo.InvariantCulture) + "%";
            string avgStr = summary.AverageResolutionHours.ToString("0.0", CultureInfo.InvariantCulture);

            sb.AppendLine("      <Row ss:Height=\"28\">");
            sb.AppendLine($"        <Cell ss:StyleID=\"KpiValueStyle\"><Data ss:Type=\"Number\">{summary.TotalTickets}</Data></Cell>");
            sb.AppendLine($"        <Cell ss:StyleID=\"{compStyle}\"><Data ss:Type=\"String\">{compStr}</Data></Cell>");
            sb.AppendLine($"        <Cell ss:StyleID=\"KpiGreenStyle\"><Data ss:Type=\"Number\">{summary.TicketsWithinSla}</Data></Cell>");
            sb.AppendLine($"        <Cell ss:StyleID=\"KpiRedStyle\"><Data ss:Type=\"Number\">{summary.TicketsBreached}</Data></Cell>");
            sb.AppendLine($"        <Cell ss:StyleID=\"KpiValueStyle\"><Data ss:Type=\"Number\">{summary.TicketsNearBreach}</Data></Cell>");
            sb.AppendLine($"        <Cell ss:StyleID=\"KpiValueStyle\"><Data ss:Type=\"String\">{avgStr}h</Data></Cell>");
            sb.AppendLine("      </Row>");
            sb.AppendLine("      <Row ss:Height=\"15\"></Row>");

            // Priority Breakdown Section
            sb.AppendLine("      <Row ss:Height=\"26\">");
            sb.AppendLine("        <Cell ss:MergeAcross=\"5\" ss:StyleID=\"SectionHeaderStyle\"><Data ss:Type=\"String\">SLA Performance Breakdown by Priority Level</Data></Cell>");
            sb.AppendLine("      </Row>");
            sb.AppendLine("      <Row ss:Height=\"22\">");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Priority</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">SLA Target (Hrs)</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Total Volume</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Met SLA</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Breached</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Compliance Rate</Data></Cell>");
            sb.AppendLine("      </Row>");

            foreach (var p in priorities)
            {
                string pCompStr = p.CompliancePercentage.ToString("0.0", CultureInfo.InvariantCulture) + "%";
                sb.AppendLine("      <Row>");
                sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{EscapeXml(p.Priority ?? "")}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{p.TargetHours}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{p.Total}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{p.WithinSla}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{p.Breached}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{pCompStr}</Data></Cell>");
                sb.AppendLine("      </Row>");
            }
            sb.AppendLine("      <Row ss:Height=\"15\"></Row>");

            // Department Breakdown Section
            sb.AppendLine("      <Row ss:Height=\"26\">");
            sb.AppendLine("        <Cell ss:MergeAcross=\"5\" ss:StyleID=\"SectionHeaderStyle\"><Data ss:Type=\"String\">Department SLA Compliance Breakdown</Data></Cell>");
            sb.AppendLine("      </Row>");
            sb.AppendLine("      <Row ss:Height=\"22\">");
            sb.AppendLine("        <Cell ss:MergeAcross=\"1\" ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Department</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Total Assigned</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Within SLA</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Breached</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Compliance %</Data></Cell>");
            sb.AppendLine("      </Row>");

            foreach (var d in departments)
            {
                string dCompStr = d.CompliancePercentage.ToString("0.0", CultureInfo.InvariantCulture) + "%";
                sb.AppendLine("      <Row>");
                sb.AppendLine($"        <Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String\">{EscapeXml(d.DepartmentName ?? "")}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{d.Total}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{d.WithinSla}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{d.Breached}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{dCompStr}</Data></Cell>");
                sb.AppendLine("      </Row>");
            }

            sb.AppendLine("    </Table>");
            sb.AppendLine("  </Worksheet>");

            // ==========================================
            // WORKSHEET 2: Detailed Ticket SLA Audit
            // ==========================================
            sb.AppendLine("  <Worksheet ss:Name=\"Detailed Ticket SLA Audit\">");
            sb.AppendLine("    <Table ss:DefaultRowHeight=\"18\">");
            sb.AppendLine("      <Column ss:Width=\"60\"/>");  // TicketID
            sb.AppendLine("      <Column ss:Width=\"220\"/>"); // Title
            sb.AppendLine("      <Column ss:Width=\"65\"/>");  // Type
            sb.AppendLine("      <Column ss:Width=\"55\"/>");  // Priority
            sb.AppendLine("      <Column ss:Width=\"130\"/>"); // Department
            sb.AppendLine("      <Column ss:Width=\"120\"/>"); // Assignee
            sb.AppendLine("      <Column ss:Width=\"85\"/>");  // Status
            sb.AppendLine("      <Column ss:Width=\"125\"/>"); // CreatedAt
            sb.AppendLine("      <Column ss:Width=\"125\"/>"); // Deadline
            sb.AppendLine("      <Column ss:Width=\"110\"/>"); // ResolvedAt
            sb.AppendLine("      <Column ss:Width=\"85\"/>");  // DurationHours
            sb.AppendLine("      <Column ss:Width=\"95\"/>");  // SlaStatus

            sb.AppendLine("      <Row ss:Height=\"24\">");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Ticket ID</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Issue Title</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Type</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Priority</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Department</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Assignee</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Status</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Created At</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">SLA Deadline</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Resolved At</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Duration (h)</Data></Cell>");
            sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">SLA Status</Data></Cell>");
            sb.AppendLine("      </Row>");

            foreach (DataRow row in detailedAuditTable.Rows)
            {
                int id = row["TicketID"] != DBNull.Value ? Convert.ToInt32(row["TicketID"]) : 0;
                string title = EscapeXml(row["Title"]?.ToString() ?? "");
                string type = EscapeXml(row["Type"]?.ToString() ?? "");
                string priority = EscapeXml(row["Priority"]?.ToString() ?? "");
                string dept = EscapeXml(row["Department"]?.ToString() ?? "");
                string assignee = EscapeXml(row["Assignee"]?.ToString() ?? "");
                string status = EscapeXml(row["Status"]?.ToString() ?? "");
                string createdStr = row["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd HH:mm") : "-";
                string deadlineStr = row["Deadline"] != DBNull.Value ? Convert.ToDateTime(row["Deadline"]).ToString("yyyy-MM-dd HH:mm") : "-";
                string resolved = EscapeXml(row["ResolvedAt"]?.ToString() ?? "-");
                double duration = row["DurationHours"] != DBNull.Value ? Convert.ToDouble(row["DurationHours"]) : 0.0;
                string durationStr = duration.ToString("0.0", CultureInfo.InvariantCulture);
                string slaStatus = row["SlaStatus"]?.ToString() ?? "";

                string statusStyle = "Default";
                if (slaStatus == "Compliant") statusStyle = "CompliantCell";
                else if (slaStatus == "Breached") statusStyle = "BreachedCell";
                else if (slaStatus == "At Risk") statusStyle = "AtRiskCell";
                else if (slaStatus == "In Progress") statusStyle = "InProgressCell";

                sb.AppendLine("      <Row>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"Number\">{id}</Data></Cell>");
                sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{title}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{type}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{priority}</Data></Cell>");
                sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{dept}</Data></Cell>");
                sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{assignee}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{status}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{createdStr}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{deadlineStr}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{resolved}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"DataRight\"><Data ss:Type=\"String\">{durationStr}</Data></Cell>");
                sb.AppendLine($"        <Cell ss:StyleID=\"{statusStyle}\"><Data ss:Type=\"String\">{slaStatus}</Data></Cell>");
                sb.AppendLine("      </Row>");
            }

            sb.AppendLine("    </Table>");
            sb.AppendLine("  </Worksheet>");

            // ==========================================
            // WORKSHEET 3: Audit Trail Log (Optional)
            // ==========================================
            if (auditLogsTable != null && auditLogsTable.Rows.Count > 0)
            {
                sb.AppendLine("  <Worksheet ss:Name=\"Audit Trail Logs\">");
                sb.AppendLine("    <Table ss:DefaultRowHeight=\"18\">");
                sb.AppendLine("      <Column ss:Width=\"55\"/>");
                sb.AppendLine("      <Column ss:Width=\"65\"/>");
                sb.AppendLine("      <Column ss:Width=\"130\"/>");
                sb.AppendLine("      <Column ss:Width=\"80\"/>");
                sb.AppendLine("      <Column ss:Width=\"120\"/>");
                sb.AppendLine("      <Column ss:Width=\"320\"/>");
                sb.AppendLine("      <Column ss:Width=\"130\"/>");

                sb.AppendLine("      <Row ss:Height=\"24\">");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Log ID</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Ticket ID</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Actor</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Role</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Action</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Details</Data></Cell>");
                sb.AppendLine("        <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">Timestamp</Data></Cell>");
                sb.AppendLine("      </Row>");

                foreach (DataRow row in auditLogsTable.Rows)
                {
                    string logId = row["LogID"] != DBNull.Value ? row["LogID"].ToString() : "";
                    string ticketId = (row["TicketID"] != DBNull.Value && row["TicketID"].ToString() != "0") ? row["TicketID"].ToString() : "-";
                    string emp = EscapeXml(row["Employee"]?.ToString() ?? "System");
                    string role = EscapeXml(row["Role"]?.ToString() ?? "User");
                    string act = EscapeXml(row["Action"]?.ToString() ?? "");
                    string details = EscapeXml(row["Details"]?.ToString() ?? "");
                    string ts = row["Timestamp"] != DBNull.Value ? Convert.ToDateTime(row["Timestamp"]).ToString("yyyy-MM-dd HH:mm:ss") : "";

                    sb.AppendLine("      <Row>");
                    sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{logId}</Data></Cell>");
                    sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{ticketId}</Data></Cell>");
                    sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{emp}</Data></Cell>");
                    sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{role}</Data></Cell>");
                    sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{act}</Data></Cell>");
                    sb.AppendLine($"        <Cell><Data ss:Type=\"String\">{details}</Data></Cell>");
                    sb.AppendLine($"        <Cell ss:StyleID=\"DataCenter\"><Data ss:Type=\"String\">{ts}</Data></Cell>");
                    sb.AppendLine("      </Row>");
                }

                sb.AppendLine("    </Table>");
                sb.AppendLine("  </Worksheet>");
            }

            sb.AppendLine("</Workbook>");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exports any DataTable to standard Comma-Separated Values (CSV).
        /// </summary>
        public static void ExportToCsv(DataTable dataTable, string filePath)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));

            var sb = new StringBuilder();

            // Header line
            var headerList = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                headerList.Add(EscapeCsv(col.ColumnName));
            }
            sb.AppendLine(string.Join(",", headerList));

            // Data rows
            foreach (DataRow row in dataTable.Rows)
            {
                var rowList = new List<string>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    object val = row[col];
                    string strVal;
                    if (val == DBNull.Value)
                    {
                        strVal = "";
                    }
                    else if (val is DateTime dt)
                    {
                        strVal = dt.ToString("yyyy-MM-dd HH:mm");
                    }
                    else if (val is double dbl)
                    {
                        strVal = dbl.ToString("0.0", CultureInfo.InvariantCulture);
                    }
                    else if (val is float flt)
                    {
                        strVal = flt.ToString("0.0", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        strVal = val.ToString();
                    }
                    rowList.Add(EscapeCsv(strVal));
                }
                sb.AppendLine(string.Join(",", rowList));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
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

        private static string EscapeCsv(string field)
        {
            if (field == null) return "\"\"";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }
    }
}
