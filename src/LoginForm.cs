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
                // Resolve schema.sql path relative to the executable (supporting multiple folder depths)
                string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
                // Try 3 levels up (e.g. from bin\Debug\ to project root)
                string schemaPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\..\docs\schema.sql"));

                // Fallback 1: Try 4 levels up (e.g. deeper output folders)
                if (!System.IO.File.Exists(schemaPath))
                    schemaPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\..\..\docs\schema.sql"));

                // Fallback 2: Try 2 levels up
                if (!System.IO.File.Exists(schemaPath))
                    schemaPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\docs\schema.sql"));

                // Fallback 3: look in the same folder as the exe (for deployment)
                if (!System.IO.File.Exists(schemaPath))
                    schemaPath = System.IO.Path.Combine(exeDir, "schema.sql");

                // Step 1: Initialize database schema (CREATE IF NOT EXISTS — safe to run every launch)
                if (System.IO.File.Exists(schemaPath))
                    _dbManager.InitializeDatabase(schemaPath);

                // Step 1.5: Auto-sync organogram before seeding default admin
                string csvPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\..\org\organogram.csv"));
                if (!System.IO.File.Exists(csvPath))
                    csvPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\..\..\org\organogram.csv"));
                if (!System.IO.File.Exists(csvPath))
                    csvPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDir, @"..\..\org\organogram.csv"));
                if (!System.IO.File.Exists(csvPath))
                    csvPath = System.IO.Path.Combine(exeDir, "organogram.csv");

                if (System.IO.File.Exists(csvPath))
                {
                    var sync = new OrganogramSync(_dbManager);
                    sync.SyncFromCsv(csvPath);
                }

                // Step 2: Seed default admin if no users exist yet
                _authManager.SeedDefaultAdmin();
            }
            catch (Exception ex)
            {
                lblError.Text = "Database initialization error. Check XAMPP MySQL is running.";
                MessageBox.Show(
                    $"DB Connection failed. Ensure local XAMPP MySQL is active.\nError Details: {ex.Message}",
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
