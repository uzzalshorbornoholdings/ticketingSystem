using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public partial class AdminForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly AdminManager _adminManager;
        
        private string _selectedUsername = null;
        private string _selectedEmployeeId = null;

        public AdminForm(DatabaseManager db)
        {
            InitializeComponent();
            _db = db;
            _adminManager = new AdminManager(db);
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadUserData();
            LoadUnassociatedEmployees();
        }

        private void TabControlAdmin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlAdmin.SelectedIndex == 0)
            {
                LoadUserData();
            }
            else
            {
                LoadUnassociatedEmployees();
            }
            lblStatusMsg.Text = string.Empty;
        }

        private static void SetColumnWidth(DataGridView grid, string colName, int width)
        {
            if (grid == null || grid.Columns == null || string.IsNullOrEmpty(colName)) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Safely set auto size mode to None before setting width to prevent layout-lock crashes
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        col.Width = width;
                        break;
                    }
                }
            }
            catch
            {
                // Suppress exceptions
            }
        }

        private static DataGridViewCell FindCell(DataGridViewRow row, string colName)
        {
            if (row == null || string.IsNullOrEmpty(colName)) return null;
            var grid = row.DataGridView;
            if (grid != null)
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        return row.Cells[col.Index];
                    }
                }
            }
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.OwningColumn != null && string.Equals(cell.OwningColumn.Name, colName, StringComparison.OrdinalIgnoreCase))
                {
                    return cell;
                }
            }
            return null;
        }

        private void LoadUserData()
        {
            try
            {
                DataTable dt = _adminManager.GetAllUsers();
                gridUsers.DataSource = dt;
                
                if (gridUsers.Columns.Count > 0)
                {
                    SetColumnWidth(gridUsers, "UserId", 50);
                    SetColumnWidth(gridUsers, "Username", 100);
                    SetColumnWidth(gridUsers, "Role", 80);
                    SetColumnWidth(gridUsers, "EmployeeId", 100);
                }
                
                lblSelectedUser.Text = "Select user to modify role...";
                _selectedUsername = null;
                cmbRoleEdit.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUnassociatedEmployees()
        {
            try
            {
                DataTable dt = _adminManager.GetUnassociatedEmployees();
                gridUnassociated.DataSource = dt;
                
                if (gridUnassociated.Columns.Count > 0)
                {
                    SetColumnWidth(gridUnassociated, "EmployeeId", 100);
                }
                
                lblSelectedEmpInfo.Text = "Select an employee from the list above...";
                _selectedEmployeeId = null;
                txtNewUsername.Clear();
                txtNewPassword.Clear();
                cmbNewRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load employees: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridUsers_SelectionChanged(object sender, System.EventArgs e)
        {
            if (gridUsers.SelectedRows.Count > 0)
            {
                var row = gridUsers.SelectedRows[0];
                var userCell = FindCell(row, "Username");
                var roleCell = FindCell(row, "Role");
                _selectedUsername = userCell?.Value?.ToString() ?? string.Empty;
                string currentRole = roleCell?.Value?.ToString() ?? string.Empty;
                
                lblSelectedUser.Text = $"User: {_selectedUsername}";
                cmbRoleEdit.SelectedItem = currentRole;
            }
        }

        private void GridUnassociated_SelectionChanged(object sender, System.EventArgs e)
        {
            if (gridUnassociated.SelectedRows.Count > 0)
            {
                var row = gridUnassociated.SelectedRows[0];
                var empIdCell = FindCell(row, "EmployeeId");
                var nameCell = FindCell(row, "Name");
                var desigCell = FindCell(row, "Designation");

                _selectedEmployeeId = empIdCell?.Value?.ToString();
                string name = nameCell?.Value?.ToString() ?? string.Empty;
                string designation = desigCell?.Value?.ToString() ?? string.Empty;
                
                lblSelectedEmpInfo.Text = $"Provision: {name} ({designation}) | ID: {_selectedEmployeeId ?? "N/A"}";
            }
        }

        private void BtnUpdateRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUsername))
            {
                MessageBox.Show("Please select a user from the directory first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRoleEdit.SelectedItem == null)
            {
                MessageBox.Show("Please select a new role designation.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newRole = cmbRoleEdit.SelectedItem.ToString();
            
            // Safety check to prevent administrator lockout or removing self-admin role
            if (_selectedUsername == "admin" && newRole != "Admin")
            {
                MessageBox.Show("For safety reasons, the built-in 'admin' super-user must maintain the Admin role.", 
                                "Security Safeguard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_adminManager.UpdateUserRole(_selectedUsername, newRole, out string error))
            {
                // Record administrative audit trail
                try
                {
                    string auditSql = "INSERT INTO audit_logs (ticket_id, employee_id, action, details) VALUES (NULL, 'Admin', 'Update Role', @details)";
                    _db.ExecuteNonQuery(auditSql, new MySqlParameter[] {
                        new MySqlParameter("@details", $"User '{_selectedUsername}' role changed to '{newRole}'")
                    });
                }
                catch { }

                lblStatusMsg.ForeColor = Color.LimeGreen;
                lblStatusMsg.Text = $"Successfully updated role for user '{_selectedUsername}' to {newRole}!";
                LoadUserData();
            }
            else
            {
                MessageBox.Show($"Failed to update user role:\n{error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedEmployeeId))
            {
                MessageBox.Show("Please select an employee starting record.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtNewUsername.Text.Trim();
            string password = txtNewPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and Password credentials are required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbNewRole.SelectedItem == null)
            {
                MessageBox.Show("Please assign a role designation.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = cmbNewRole.SelectedItem.ToString();

            if (_adminManager.CreateUserAccount(_selectedEmployeeId, username, password, role, out string error))
            {
                // Record administrative audit trail
                try
                {
                    string auditSql = "INSERT INTO audit_logs (ticket_id, employee_id, action, details) VALUES (NULL, 'Admin', 'Create User', @details)";
                    _db.ExecuteNonQuery(auditSql, new MySqlParameter[] {
                        new MySqlParameter("@details", $"Created user account '{username}' with role '{role}' (Employee: {_selectedEmployeeId})")
                    });
                }
                catch { }

                lblStatusMsg.ForeColor = Color.LimeGreen;
                lblStatusMsg.Text = $"Registered user account '{username}' successfully!";
                LoadUnassociatedEmployees();
            }
            else
            {
                MessageBox.Show($"Failed to register account:\n{error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
