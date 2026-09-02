using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// SLA Compliance & Executive Reporting Engine.
    /// Analyzes resolution times, SLA breach rates, and department performance metrics.
    /// </summary>
    public class SlaReportManager
    {
        private readonly DatabaseManager _db;
        private readonly SlaEngine _slaEngine;

        public class SlaSummaryMetrics
        {
            public int TotalTickets { get; set; }
            public int TicketsWithinSla { get; set; }
            public int TicketsBreached { get; set; }
            public int TicketsNearBreach { get; set; }
            public int OpenTickets { get; set; }
            public int ResolvedTickets { get; set; }
            public double ComplianceRatePercentage { get; set; }
            public double AverageResolutionHours { get; set; }
            public DateTime GeneratedAt { get; set; } = DateTime.Now;
            public string FilterDescription { get; set; } = "All Data";
        }

        public class PriorityBreakdownItem
        {
            public string Priority { get; set; }
            public int TargetHours { get; set; }
            public int Total { get; set; }
            public int WithinSla { get; set; }
            public int Breached { get; set; }
            public double CompliancePercentage { get; set; }
            public double AverageHours { get; set; }
        }

        public class DepartmentBreakdownItem
        {
            public string DepartmentName { get; set; }
            public int Total { get; set; }
            public int WithinSla { get; set; }
            public int Breached { get; set; }
            public double CompliancePercentage { get; set; }
        }

        public SlaReportManager(DatabaseManager db)
        {
            _db = db;
            _slaEngine = new SlaEngine(db);
        }

        /// <summary>
        /// Retrieves raw ticket rows filtered by date range, department, and priority.
        /// </summary>
        public DataTable GetFilteredTickets(DateTime? fromDate = null, DateTime? toDate = null, string departmentName = null, string priority = null)
        {
            var clauses = new List<string>();
            var parameters = new List<MySqlParameter>();

            if (fromDate.HasValue)
            {
                clauses.Add("t.created_at >= @fromDate");
                parameters.Add(new MySqlParameter("@fromDate", fromDate.Value.Date));
            }

            if (toDate.HasValue)
            {
                clauses.Add("t.created_at <= @toDate");
                parameters.Add(new MySqlParameter("@toDate", toDate.Value.Date.AddDays(1).AddSeconds(-1)));
            }

            if (!string.IsNullOrEmpty(departmentName) && departmentName != "All Departments")
            {
                clauses.Add("d.name = @deptName");
                parameters.Add(new MySqlParameter("@deptName", departmentName));
            }

            if (!string.IsNullOrEmpty(priority) && priority != "All Priorities")
            {
                clauses.Add("t.priority = @priority");
                parameters.Add(new MySqlParameter("@priority", priority));
            }

            string whereClause = clauses.Count > 0 ? "WHERE " + string.Join(" AND ", clauses) : "";

            string query = $@"
                SELECT 
                    t.id AS TicketID,
                    t.title AS Title,
                    t.type AS Type,
                    t.priority AS Priority,
                    t.status AS Status,
                    COALESCE(d.name, 'Unassigned') AS Department,
                    COALESCE(assignee.name, 'Unassigned') AS Assignee,
                    COALESCE(creator.name, 'System') AS Creator,
                    t.created_at AS CreatedAt,
                    t.updated_at AS UpdatedAt,
                    s.resolution_hours AS SlaHours
                FROM tickets t
                LEFT JOIN employees assignee ON t.assigned_employee_id = assignee.id
                LEFT JOIN departments d ON assignee.department_id = d.id
                LEFT JOIN employees creator ON t.creator_employee_id = creator.id
                LEFT JOIN slas s ON t.sla_id = s.id
                {whereClause}
                ORDER BY t.created_at DESC";

            return _db.ExecuteQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Analyzes ticket data and produces detailed SLA metrics including deadlines and breach status.
        /// </summary>
        public DataTable GenerateDetailedSlaAuditTable(DataTable rawTickets)
        {
            var auditTable = new DataTable("DetailedSlaAudit");
            auditTable.Columns.Add("TicketID", typeof(int));
            auditTable.Columns.Add("Title", typeof(string));
            auditTable.Columns.Add("Type", typeof(string));
            auditTable.Columns.Add("Priority", typeof(string));
            auditTable.Columns.Add("Department", typeof(string));
            auditTable.Columns.Add("Assignee", typeof(string));
            auditTable.Columns.Add("Status", typeof(string));
            auditTable.Columns.Add("CreatedAt", typeof(DateTime));
            auditTable.Columns.Add("Deadline", typeof(DateTime));
            auditTable.Columns.Add("ResolvedAt", typeof(string));
            auditTable.Columns.Add("DurationHours", typeof(double));
            auditTable.Columns.Add("SlaStatus", typeof(string)); // 'Compliant', 'Breached', 'At Risk', 'In Progress'

            foreach (DataRow row in rawTickets.Rows)
            {
                int ticketId = Convert.ToInt32(row["TicketID"]);
                string title = row["Title"]?.ToString();
                string type = row["Type"]?.ToString();
                string priority = row["Priority"]?.ToString();
                string dept = row["Department"]?.ToString();
                string assignee = row["Assignee"]?.ToString();
                string status = row["Status"]?.ToString();
                DateTime createdAt = Convert.ToDateTime(row["CreatedAt"]);
                DateTime updatedAt = Convert.ToDateTime(row["UpdatedAt"]);
                int slaHours = row["SlaHours"] != DBNull.Value ? Convert.ToInt32(row["SlaHours"]) : 24;

                DateTime deadline = _slaEngine.CalculateDeadline(createdAt, slaHours);

                bool isResolved = string.Equals(status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(status, "Closed", StringComparison.OrdinalIgnoreCase);

                string resolvedAtStr = isResolved ? updatedAt.ToString("yyyy-MM-dd HH:mm") : "-";
                double durationHours = isResolved ? Math.Round((updatedAt - createdAt).TotalHours, 1) : Math.Round((DateTime.Now - createdAt).TotalHours, 1);

                string slaStatus;
                if (isResolved)
                {
                    slaStatus = updatedAt <= deadline ? "Compliant" : "Breached";
                }
                else
                {
                    if (DateTime.Now > deadline)
                    {
                        slaStatus = "Breached";
                    }
                    else if (_slaEngine.IsNearBreach(createdAt, priority))
                    {
                        slaStatus = "At Risk";
                    }
                    else
                    {
                        slaStatus = "In Progress";
                    }
                }

                auditTable.Rows.Add(ticketId, title, type, priority, dept, assignee, status, createdAt, deadline, resolvedAtStr, durationHours, slaStatus);
            }

            return auditTable;
        }

        /// <summary>
        /// Computes high-level KPI summary statistics.
        /// </summary>
        public SlaSummaryMetrics ComputeSummaryMetrics(DataTable detailedAuditTable, string filterDescription = "All Time")
        {
            var summary = new SlaSummaryMetrics
            {
                TotalTickets = detailedAuditTable.Rows.Count,
                FilterDescription = filterDescription
            };

            if (summary.TotalTickets == 0) return summary;

            int compliantCount = 0;
            int breachedCount = 0;
            int nearBreachCount = 0;
            int openCount = 0;
            int resolvedCount = 0;
            double totalResolvedDuration = 0;

            foreach (DataRow row in detailedAuditTable.Rows)
            {
                string slaStatus = row["SlaStatus"]?.ToString();
                string status = row["Status"]?.ToString();
                double duration = Convert.ToDouble(row["DurationHours"]);

                bool isResolved = string.Equals(status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(status, "Closed", StringComparison.OrdinalIgnoreCase);

                if (isResolved)
                {
                    resolvedCount++;
                    totalResolvedDuration += duration;
                }
                else
                {
                    openCount++;
                }

                if (slaStatus == "Compliant") compliantCount++;
                else if (slaStatus == "Breached") breachedCount++;
                else if (slaStatus == "At Risk") nearBreachCount++;
            }

            summary.TicketsWithinSla = compliantCount;
            summary.TicketsBreached = breachedCount;
            summary.TicketsNearBreach = nearBreachCount;
            summary.OpenTickets = openCount;
            summary.ResolvedTickets = resolvedCount;

            int evaluatedCount = compliantCount + breachedCount;
            summary.ComplianceRatePercentage = evaluatedCount > 0
                ? Math.Round(((double)compliantCount / evaluatedCount) * 100.0, 1)
                : 100.0;

            summary.AverageResolutionHours = resolvedCount > 0
                ? Math.Round(totalResolvedDuration / resolvedCount, 1)
                : 0.0;

            return summary;
        }

        /// <summary>
        /// Computes SLA performance breakdown per priority level (P1-P4).
        /// </summary>
        public List<PriorityBreakdownItem> ComputePriorityBreakdown(DataTable detailedAuditTable)
        {
            var map = new Dictionary<string, PriorityBreakdownItem>
            {
                { "P1", new PriorityBreakdownItem { Priority = "P1 (Critical)", TargetHours = 2 } },
                { "P2", new PriorityBreakdownItem { Priority = "P2 (High)", TargetHours = 8 } },
                { "P3", new PriorityBreakdownItem { Priority = "P3 (Medium)", TargetHours = 24 } },
                { "P4", new PriorityBreakdownItem { Priority = "P4 (Low)", TargetHours = 72 } }
            };

            var durations = new Dictionary<string, List<double>>
            {
                { "P1", new List<double>() },
                { "P2", new List<double>() },
                { "P3", new List<double>() },
                { "P4", new List<double>() }
            };

            foreach (DataRow row in detailedAuditTable.Rows)
            {
                string p = row["Priority"]?.ToString()?.ToUpper() ?? "P3";
                if (!map.ContainsKey(p)) p = "P3";

                string slaStatus = row["SlaStatus"]?.ToString();
                string status = row["Status"]?.ToString();
                double duration = Convert.ToDouble(row["DurationHours"]);

                var item = map[p];
                item.Total++;

                if (slaStatus == "Compliant") item.WithinSla++;
                else if (slaStatus == "Breached") item.Breached++;

                bool isResolved = string.Equals(status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(status, "Closed", StringComparison.OrdinalIgnoreCase);
                if (isResolved)
                {
                    durations[p].Add(duration);
                }
            }

            var list = new List<PriorityBreakdownItem>();
            foreach (var kvp in map)
            {
                int eval = kvp.Value.WithinSla + kvp.Value.Breached;
                kvp.Value.CompliancePercentage = eval > 0
                    ? Math.Round(((double)kvp.Value.WithinSla / eval) * 100.0, 1)
                    : 100.0;

                var dList = durations[kvp.Key];
                if (dList.Count > 0)
                {
                    double sum = 0;
                    foreach (var d in dList) sum += d;
                    kvp.Value.AverageHours = Math.Round(sum / dList.Count, 1);
                }

                list.Add(kvp.Value);
            }

            return list;
        }

        /// <summary>
        /// Computes department-level compliance metrics.
        /// </summary>
        public List<DepartmentBreakdownItem> ComputeDepartmentBreakdown(DataTable detailedAuditTable)
        {
            var map = new Dictionary<string, DepartmentBreakdownItem>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in detailedAuditTable.Rows)
            {
                string dept = row["Department"]?.ToString();
                if (string.IsNullOrEmpty(dept)) dept = "Unassigned";

                if (!map.ContainsKey(dept))
                {
                    map[dept] = new DepartmentBreakdownItem { DepartmentName = dept };
                }

                var item = map[dept];
                item.Total++;

                string slaStatus = row["SlaStatus"]?.ToString();
                if (slaStatus == "Compliant") item.WithinSla++;
                else if (slaStatus == "Breached") item.Breached++;
            }

            var list = new List<DepartmentBreakdownItem>();
            foreach (var item in map.Values)
            {
                int eval = item.WithinSla + item.Breached;
                item.CompliancePercentage = eval > 0
                    ? Math.Round(((double)item.WithinSla / eval) * 100.0, 1)
                    : 100.0;
                list.Add(item);
            }

            list.Sort((a, b) => b.Total.CompareTo(a.Total));
            return list;
        }

        /// <summary>
        /// Retrieves all registered department names for filtering.
        /// </summary>
        public List<string> GetAllDepartmentNames()
        {
            var list = new List<string>();
            try
            {
                var dt = _db.ExecuteQuery("SELECT name FROM departments ORDER BY name ASC");
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(row["name"].ToString());
                }
            }
            catch { }
            return list;
        }
    }
}
