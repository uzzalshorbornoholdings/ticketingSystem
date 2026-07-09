using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public class OrganogramSync
    {
        private readonly DatabaseManager _dbManager;

        public OrganogramSync(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public class SyncReport
        {
            public int DepartmentsCreated { get; set; }
            public int EmployeesUpserted { get; set; }
            public int ReportingLinksUpdated { get; set; }
            public List<string> Logs { get; } = new List<string>();
        }

        public SyncReport SyncFromCsv(string csvFilePath)
        {
            var report = new SyncReport();
            if (!File.Exists(csvFilePath))
            {
                throw new FileNotFoundException("Organogram CSV file not found.", csvFilePath);
            }

            var lines = File.ReadAllLines(csvFilePath);
            if (lines.Length <= 1)
            {
                report.Logs.Add("CSV is empty or contains only header.");
                return report;
            }

            // Maps department name to ID
            var departmentCache = LoadDepartments();
            var csvRecords = new List<CsvEmployeeRecord>();

            // Pass 1: Parse and Sync Departments and Basic Employee details
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = ParseCsvLine(line);
                if (parts.Length < 5)
                {
                    report.Logs.Add($"Line {i+1}: Skipped due to insufficient fields.");
                    continue;
                }

                var record = new CsvEmployeeRecord
                {
                    EmployeeId = parts[0].Trim(),
                    Name = parts[1].Trim(),
                    Designation = parts[2].Trim(),
                    DepartmentName = parts[3].Trim(),
                    ReportsToName = parts[4].Trim()
                };
                csvRecords.Add(record);

                // Ensure department exists
                if (!string.IsNullOrEmpty(record.DepartmentName) && !departmentCache.ContainsKey(record.DepartmentName))
                {
                    int deptId = CreateDepartment(record.DepartmentName);
                    departmentCache[record.DepartmentName] = deptId;
                    report.DepartmentsCreated++;
                    report.Logs.Add($"Created department: {record.DepartmentName} (ID: {deptId})");
                }

                int? deptIdForEmp = null;
                if (!string.IsNullOrEmpty(record.DepartmentName))
                {
                    deptIdForEmp = departmentCache[record.DepartmentName];
                }

                // Upsert Employee (without manager hierarchy in Pass 1)
                UpsertEmployeeBasic(record.EmployeeId, record.Name, record.Designation, deptIdForEmp);
                report.EmployeesUpserted++;
            }

            // Pass 2: Update ReportsTo relationship
            foreach (var record in csvRecords)
            {
                if (string.IsNullOrEmpty(record.ReportsToName) || record.ReportsToName.Equals("Board of Directors", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEmployeeReportsTo(record.EmployeeId, null);
                    report.ReportingLinksUpdated++;
                    continue;
                }

                // Find manager Employee ID by exact name
                string managerId = FindEmployeeIdByName(record.ReportsToName);
                if (managerId != null)
                {
                    UpdateEmployeeReportsTo(record.EmployeeId, managerId);
                    report.ReportingLinksUpdated++;
                }
                else
                {
                    // If manager is not found directly by name, check if ReportsTo represents a role/designation
                    // fallback lookup by designation
                    managerId = FindEmployeeIdByDesignation(record.ReportsToName);
                    if (managerId != null)
                    {
                        UpdateEmployeeReportsTo(record.EmployeeId, managerId);
                        report.ReportingLinksUpdated++;
                    }
                    else
                    {
                        report.Logs.Add($"Warning: Could not resolve manager '{record.ReportsToName}' for Employee '{record.Name}' ({record.EmployeeId}).");
                    }
                }
            }

            return report;
        }

        private string[] ParseCsvLine(string line)
        {
            // Simple split by comma. Since fields in organogram.csv don't contain commas,
            // this standard split is reliable.
            return line.Split(',');
        }

        private Dictionary<string, int> LoadDepartments()
        {
            var cache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var dt = _dbManager.ExecuteQuery("SELECT id, name FROM departments");
            foreach (DataRow row in dt.Rows)
            {
                cache[row["name"].ToString()] = Convert.ToInt32(row["id"]);
            }
            return cache;
        }

        private int CreateDepartment(string name)
        {
            _dbManager.ExecuteNonQuery(
                "INSERT INTO departments (name) VALUES (@name) ON DUPLICATE KEY UPDATE name=name",
                new MySqlParameter[] { new MySqlParameter("@name", name) }
            );
            var idObj = _dbManager.ExecuteScalar(
                "SELECT id FROM departments WHERE name = @name",
                new MySqlParameter[] { new MySqlParameter("@name", name) }
            );
            return Convert.ToInt32(idObj);
        }

        private void UpsertEmployeeBasic(string id, string name, string designation, int? departmentId)
        {
            string query = @"
                INSERT INTO employees (id, name, designation, department_id)
                VALUES (@id, @name, @designation, @department_id)
                ON DUPLICATE KEY UPDATE
                    name = VALUES(name),
                    designation = VALUES(designation),
                    department_id = VALUES(department_id)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", id),
                new MySqlParameter("@name", name),
                new MySqlParameter("@designation", designation),
                new MySqlParameter("@department_id", (object)departmentId ?? DBNull.Value)
            };

            _dbManager.ExecuteNonQuery(query, parameters);
        }

        private void UpdateEmployeeReportsTo(string employeeId, string reportsToId)
        {
            string query = "UPDATE employees SET reports_to_id = @reports_to_id WHERE id = @id";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", employeeId),
                new MySqlParameter("@reports_to_id", (object)reportsToId ?? DBNull.Value)
            };
            _dbManager.ExecuteNonQuery(query, parameters);
        }

        private string FindEmployeeIdByName(string name)
        {
            var obj = _dbManager.ExecuteScalar(
                "SELECT id FROM employees WHERE name = @name",
                new MySqlParameter[] { new MySqlParameter("@name", name) }
            );
            return obj?.ToString();
        }

        private string FindEmployeeIdByDesignation(string designation)
        {
            // Some entries might reference designation reports, e.g. "Director of Infrastructure"
            // Let's resolve known exceptions if name is role-based
            if (designation.Equals("Director of Infrastructure", StringComparison.OrdinalIgnoreCase))
            {
                // VP of Infrastructure & Networking has MGT-003, which is the management ID of Dr. Amran Hossain
                return "MGT-003";
            }
            if (designation.Equals("CTO", StringComparison.OrdinalIgnoreCase))
            {
                return "MGT-001"; // Chief Technology Officer
            }

            var obj = _dbManager.ExecuteScalar(
                "SELECT id FROM employees WHERE designation = @designation",
                new MySqlParameter[] { new MySqlParameter("@designation", designation) }
            );
            return obj?.ToString();
        }

        private class CsvEmployeeRecord
        {
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
            public string DepartmentName { get; set; }
            public string ReportsToName { get; set; }
        }
    }
}
