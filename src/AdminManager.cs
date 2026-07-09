using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public class AdminManager
    {
        private readonly DatabaseManager _db;
        private readonly AuthManager _auth;

        public AdminManager(DatabaseManager db)
        {
            _db = db;
            _auth = new AuthManager(db);
        }

        /// <summary>
        /// Retrieves all registered user accounts with employee, designation and department descriptions.
        /// </summary>
        public DataTable GetAllUsers()
        {
            string query = @"
                SELECT 
                    u.id AS UserId, 
                    u.username AS Username, 
                    u.role AS Role, 
                    u.employee_id AS EmployeeId,
                    e.name AS EmployeeName, 
                    e.designation AS Designation, 
                    d.name AS DepartmentName
                FROM users u
                LEFT JOIN employees e ON u.employee_id = e.id
                LEFT JOIN departments d ON e.department_id = d.id
                ORDER BY u.username ASC";

            return _db.ExecuteQuery(query);
        }

        /// <summary>
        /// Retrieves employees who do not have an active user login associated.
        /// Useful for seeding options in the admin panel user registration dropdown.
        /// </summary>
        public DataTable GetUnassociatedEmployees()
        {
            string query = @"
                SELECT 
                    e.id AS EmployeeId, 
                    e.name AS Name, 
                    e.designation AS Designation, 
                    d.name AS DepartmentName
                FROM employees e
                LEFT JOIN departments d ON e.department_id = d.id
                LEFT JOIN users u ON u.employee_id = e.id
                WHERE u.id IS NULL
                ORDER BY e.name ASC";

            return _db.ExecuteQuery(query);
        }

        /// <summary>
        /// Creates a user account linked to an employee.
        /// </summary>
        public bool CreateUserAccount(string employeeId, string username, string password, string role, out string error)
        {
            return _auth.RegisterUser(employeeId, username, password, role, out error);
        }

        /// <summary>
        /// Updates the privilege role parameter of a registered system user.
        /// </summary>
        public bool UpdateUserRole(string username, string newRole, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newRole))
            {
                error = "Username and role are required.";
                return false;
            }

            // Verify if role is valid
            string cleanedRole = newRole.Trim();
            if (cleanedRole != "Admin" && cleanedRole != "Manager" && cleanedRole != "Agent" && cleanedRole != "User")
            {
                error = "Invalid role specified. Valid roles are: Admin, Manager, Agent, User.";
                return false;
            }

            string updateQuery = "UPDATE users SET role = @role WHERE username = @username";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@role", cleanedRole),
                new MySqlParameter("@username", username.ToLower().Trim())
            };

            try
            {
                int rows = _db.ExecuteNonQuery(updateQuery, parameters);
                if (rows > 0)
                {
                    return true;
                }
                error = "User not found.";
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
