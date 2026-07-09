using System;
using System.Windows.Forms;

namespace BitswardITSM.Core
{
    public partial class LoginForm : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly AuthManager _authManager;

        public LoginForm()
        {
            InitializeComponent();
            
            // Core database interface
            _dbManager = new DatabaseManager("localhost", "root", "");
            _authManager = new AuthManager(_dbManager);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Ensure base admin exists
                // Since this runs during Form Load, the admin user 'admin' (password: admin123)
                // is initialized automatically if it is the first launch.
                _authManager.SeedDefaultAdmin();
            }
            catch (Exception ex)
            {
                lblError.Text = "Database initialization error: Checks local database logs.";
                MessageBox.Show($"DB Connection failed. Ensure local XAMPP MySQL is active.\nError Details: {ex.Message}", 
                                "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Username and Password are required!";
                return;
            }

            try
            {
                // Verify user logins
                if (_authManager.Login(username, password, out string role, out string employeeId))
                {
                    // Clean input form
                    txtPassword.Text = string.Empty;
                    txtUsername.Text = string.Empty;

                    // Launch Main Dashboard screen
                    var mainForm = new MainForm(role, employeeId, username, _dbManager);
                    mainForm.FormClosed += (s, args) => this.Show(); // Show login form again on logout/close
                    
                    this.Hide();
                    mainForm.Show();
                }
                else
                {
                    lblError.Text = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error during connection validation.";
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
