using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public class TriageEngine
    {
        private readonly DatabaseManager _db;

        public TriageEngine(DatabaseManager db)
        {
            _db = db;
        }

        /// <summary>
        /// Classifies a ticket type as Incident (INC), Service Request (SR), or Change Request (CR) based on keywords.
        /// </summary>
        public string ClassifyTicket(string title, string description)
        {
            string content = (title + " " + description).ToLower();

            // CR search criteria
            string[] crKeywords = { "deploy", "patch", "upgrade", "migration", "maintenance window", "rollout", "release deployment", "config change" };
            foreach (var kw in crKeywords)
            {
                if (content.Contains(kw)) return "CR";
            }

            // SR search criteria
            string[] srKeywords = { "request", "access", "procure", "new employee", "order", "provision", "add user", "create account", "reset password", "install software", "need licence", "help", "how to" };
            foreach (var kw in srKeywords)
            {
                if (content.Contains(kw)) return "SR";
            }

            // Default to INC
            return "INC";
        }

        /// <summary>
        /// Resolves which department the issue should be routed to based on keywords.
        /// </summary>
        public string ResolveTargetDepartment(string title, string description)
        {
            string content = (title + " " + description).ToLower();

            // 1. Cybersecurity / SOC
            string[] securityKws = { "security", "breach", "firewall", "siem", "soc", "phishing", "attack", "hacked", "malware", "virus", "intrusion", "leak" };
            foreach (var kw in securityKws)
            {
                if (content.Contains(kw)) return "Cybersecurity";
            }

            // 2. NOC / Network Admin
            string[] networkKws = { "switch", "router", "wifi", "network", "bandwidth", "ip address", "dns", "gateway", "vlan", "internet offline" };
            foreach (var kw in networkKws)
            {
                if (content.Contains(kw)) return "NOC / Network Admin";
            }

            // 3. Linux Platform
            string[] linuxKws = { "linux", "ubuntu", "redhat", "centos", "cron", "bash script", "ssh key", "sudoers" };
            foreach (var kw in linuxKws)
            {
                if (content.Contains(kw)) return "Linux Platform";
            }

            // 4. Windows / MAC Admin
            string[] clientKws = { "windows", "mac", "active directory", "ad account", "laptop", "pc", "desktop", "os patching", "outlook", "office", "macos", "keychain" };
            foreach (var kw in clientKws)
            {
                if (content.Contains(kw)) return "Windows / MAC Admin";
            }

            // 5. DevOps
            string[] devopsKws = { "ci/cd", "pipeline", "docker", "cloud", "aws", "kubernetes", "jenkins", "terraform", "ec2" };
            foreach (var kw in devopsKws)
            {
                if (content.Contains(kw)) return "DevOps";
            }

            // 6. QA
            string[] qaKws = { "test", "bug report", "qa", "verification", "crash in build", "automated test", "selenium" };
            foreach (var kw in qaKws)
            {
                if (content.Contains(kw)) return "QA";
            }

            // Default fallback is L1 Support / general IT
            return "IT / System Admin";
        }

        /// <summary>
        /// Assigns the ticket using the 3-tier smart assignment logic.
        /// Returns the ID of the assigned employee.
        /// </summary>
        public string AssignTicket(int ticketId, string creatorId, string targetDeptName)
        {
            string assignedEmployeeId = null;

            // Tier 1: Direct Manager mapping check
            assignedEmployeeId = CheckDirectManagerAssignment(creatorId, targetDeptName);
            if (assignedEmployeeId != null)
            {
                SaveAssignment(ticketId, assignedEmployeeId);
                return assignedEmployeeId;
            }

            // Tier 2: Workload Balancing
            assignedEmployeeId = CheckWorkloadBalancing(targetDeptName);
            if (assignedEmployeeId != null)
            {
                SaveAssignment(ticketId, assignedEmployeeId);
                return assignedEmployeeId;
            }

            // Tier 3: Department Head Fallback
            assignedEmployeeId = FindDepartmentHead(targetDeptName);
            if (assignedEmployeeId != null)
            {
                SaveAssignment(ticketId, assignedEmployeeId);
                return assignedEmployeeId;
            }

            // Ultimate Fallback: CTO (Dr. Md. Shafiqul Islam, ID: MGT-001)
            assignedEmployeeId = "MGT-001";
            SaveAssignment(ticketId, assignedEmployeeId);
            return assignedEmployeeId;
        }

        private string CheckDirectManagerAssignment(string creatorId, string targetDeptName)
        {
            string managerIdQuery = @"
                SELECT reports_to_id 
                FROM employees 
                WHERE id = @creatorId";

            var managerIdObj = _db.ExecuteScalar(managerIdQuery, new MySqlParameter[] { new MySqlParameter("@creatorId", creatorId) });
            if (managerIdObj == null || managerIdObj == DBNull.Value) return null;

            string managerId = managerIdObj.ToString();

            // Verify if the manager belongs to the target department
            string checkDeptQuery = @"
                SELECT e.id 
                FROM employees e
                INNER JOIN departments d ON e.department_id = d.id
                WHERE e.id = @managerId AND d.name = @deptName";

            var matchedManagerObj = _db.ExecuteScalar(checkDeptQuery, new MySqlParameter[] {
                new MySqlParameter("@managerId", managerId),
                new MySqlParameter("@deptName", targetDeptName)
            });

            return matchedManagerObj?.ToString();
        }

        private string CheckWorkloadBalancing(string targetDeptName)
        {
            // Select employees in the target department sorted by active ticket workload count ascending
            string workloadQuery = @"
                SELECT e.id, COUNT(t.id) as active_count
                FROM employees e
                INNER JOIN departments d ON e.department_id = d.id
                LEFT JOIN tickets t ON t.assigned_employee_id = e.id 
                    AND t.status IN ('Open', 'Triage', 'Assigned', 'In Progress')
                WHERE d.name = @deptName
                GROUP BY e.id
                ORDER BY active_count ASC";

            var dt = _db.ExecuteQuery(workloadQuery, new MySqlParameter[] { new MySqlParameter("@deptName", targetDeptName) });
            if (dt.Rows.Count > 0)
            {
                // We pick the employee with the lowest active load
                // We exclude department heads from general workload balancing if other staff are present
                if (dt.Rows.Count > 1)
                {
                    // Look for non-head staff first
                    foreach (DataRow row in dt.Rows)
                    {
                        string empId = row["id"].ToString();
                        if (!IsDepartmentHead(empId))
                        {
                            return empId;
                        }
                    }
                }
                return dt.Rows[0]["id"].ToString();
            }

            return null;
        }

        private bool IsDepartmentHead(string employeeId)
        {
            var designationObj = _db.ExecuteScalar(
                "SELECT designation FROM employees WHERE id = @id",
                new MySqlParameter[] { new MySqlParameter("@id", employeeId) }
            );
            if (designationObj == null) return false;

            string desig = designationObj.ToString().ToLower();
            return desig.Contains("head of") || desig.Contains("director") || desig.Contains("vp of");
        }

        private string FindDepartmentHead(string targetDeptName)
        {
            string headQuery = @"
                SELECT e.id
                FROM employees e
                INNER JOIN departments d ON e.department_id = d.id
                WHERE d.name = @deptName 
                  AND (e.designation LIKE '%Head of%' OR e.designation LIKE '%Director%' OR e.designation LIKE '%VP of%')
                LIMIT 1";

            var headObj = _db.ExecuteScalar(headQuery, new MySqlParameter[] { new MySqlParameter("@deptName", targetDeptName) });
            return headObj?.ToString();
        }

        private void SaveAssignment(int ticketId, string employeeId)
        {
            string query = "UPDATE tickets SET assigned_employee_id = @assigned_employee_id, status = 'Assigned' WHERE id = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", ticketId),
                new MySqlParameter("@assigned_employee_id", employeeId)
            };
            _db.ExecuteNonQuery(query, parameters);
        }
    }
}
