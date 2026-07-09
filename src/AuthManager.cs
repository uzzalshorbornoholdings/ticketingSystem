using System;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public class AuthManager
    {
        private readonly DatabaseManager _db;

        public AuthManager(DatabaseManager db)
        {
            _db = db;
        }

        /// <summary>
        /// Registers a user in the database. Hashes the password with a dynamic random salt.
        /// </summary>
        public bool RegisterUser(string employeeId, string username, string password, string role, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                error = "Username, password, and role are required fields.";
                return false;
            }

            // Check if username already exists
            string checkUserQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            var userCount = Convert.ToInt32(_db.ExecuteScalar(checkUserQuery, new MySqlParameter[] { new MySqlParameter("@username", username) }));

            if (userCount > 0)
            {
                error = "Username already exists.";
                return false;
            }

            // Check if employee is already linked to a user account
            if (!string.IsNullOrEmpty(employeeId))
            {
                string checkEmpQuery = "SELECT COUNT(*) FROM users WHERE employee_id = @empId";
                var empCount = Convert.ToInt32(_db.ExecuteScalar(checkEmpQuery, new MySqlParameter[] { new MySqlParameter("@empId", employeeId) }));
                if (empCount > 0)
                {
                    error = "This employee already has a registered user account.";
                    return false;
                }
            }

            // Generate salt and hash
            string salt = GenerateSalt();
            string hash = HashPassword(password, salt);

            string insertQuery = @"
                INSERT INTO users (employee_id, username, password_hash, salt, role)
                VALUES (@employee_id, @username, @password_hash, @salt, @role)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@employee_id", string.IsNullOrEmpty(employeeId) ? (object)DBNull.Value : employeeId),
                new MySqlParameter("@username", username.ToLower().Trim()),
                new MySqlParameter("@password_hash", hash),
                new MySqlParameter("@salt", salt),
                new MySqlParameter("@role", role)
            };

            try
            {
                int rows = _db.ExecuteNonQuery(insertQuery, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Validates login credentials. On success returns true with the user's role and linked employeeId.
        /// </summary>
        public bool Login(string username, string password, out string role, out string employeeId)
        {
            role = null;
            employeeId = null;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            string query = "SELECT employee_id, password_hash, salt, role FROM users WHERE username = @username";
            var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@username", username.ToLower().Trim()) });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            var row = dt.Rows[0];
            string dbHash = row["password_hash"].ToString();
            string dbSalt = row["salt"].ToString();
            string computedHash = HashPassword(password, dbSalt);

            if (dbHash == computedHash)
            {
                role = row["role"].ToString();
                employeeId = row["employee_id"] == DBNull.Value ? null : row["employee_id"].ToString();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Auto-seeds a default admin account (admin / admin123) if no users are registered.
        /// </summary>
        public void SeedDefaultAdmin()
        {
            string checkCountQuery = "SELECT COUNT(*) FROM users";
            int count = Convert.ToInt32(_db.ExecuteScalar(checkCountQuery));

            if (count == 0)
            {
                // Link it to CTO employee (MGT-001) if present, otherwise null
                string ctoCheck = "SELECT COUNT(*) FROM employees WHERE id = 'MGT-001'";
                bool rootEmpExists = Convert.ToInt32(_db.ExecuteScalar(ctoCheck)) > 0;

                string employeeId = rootEmpExists ? "MGT-001" : null;
                RegisterUser(employeeId, "admin", "admin123", "Admin", out _);
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return BitConverter.ToString(saltBytes).Replace("-", "").ToLower();
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] val = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(val);
                var sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
