using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

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

        private void LoadUserData()
        {
            try
            {
                DataTable dt = _adminManager.GetAllUsers();
                gridUsers.DataSource = dt;
                
                if (gridUsers.Columns.Count > 0)
                {
                    gridUsers.Columns["UserId"].Width = 50;
                    gridUsers.Columns["Username"].Width = 100;
                    gridUsers.Columns["Role"].Width = 80;
                    gridUsers.Columns["EmployeeId"].Width = 100;
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
                    gridUnassociated.Columns["EmployeeId"].Width = 100;
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
                _selectedUsername = row.Cells["Username"].Value.ToString();
                string currentRole = row.Cells["Role"].Value.ToString();
                
                lblSelectedUser.Text = $"User: {_selectedUsername}";
                cmbRoleEdit.SelectedItem = currentRole;
            }
        }

        private void GridUnassociated_SelectionChanged(object sender, System.EventArgs e)
        {
            if (gridUnassociated.SelectedRows.Count > 0)
            {
                var row = gridUnassociated.SelectedRows[0];
                _selectedEmployeeId = row.Cells["EmployeeId"].Value.ToString();
                string name = row.Cells["Name"].Value.ToString();
                string designation = row.Cells["Designation"].Value.ToString();
                
                lblSelectedEmpInfo.Text = $"Provision: {name} ({designation}) | ID: {_selectedEmployeeId}";
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
