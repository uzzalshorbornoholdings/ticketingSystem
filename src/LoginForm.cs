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

                // Step 1.2: Ensure backward-compatible column migration for PIR fields
                try
                {
                    var dtPir = _dbManager.ExecuteQuery("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='bitsward_tickets' AND TABLE_NAME='change_requests' AND COLUMN_NAME='pir_status'");
                    if (dtPir.Rows.Count == 0)
                    {
                        _dbManager.ExecuteNonQuery("ALTER TABLE change_requests ADD COLUMN pir_status VARCHAR(50) DEFAULT 'Pending', ADD COLUMN pir_notes TEXT NULL;");
                    }
                }
                catch { }

                // Step 1.3: Ensure ticket_attachments table exists
                try
                {
                    _dbManager.ExecuteNonQuery(@"
                        CREATE TABLE IF NOT EXISTS ticket_attachments (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            ticket_id INT NOT NULL,
                            employee_id VARCHAR(50) NOT NULL,
                            file_name VARCHAR(255) NOT NULL,
                            file_path VARCHAR(500) NOT NULL,
                            file_size BIGINT NOT NULL,
                            file_type VARCHAR(50) NOT NULL,
                            created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE CASCADE,
                            FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE
                        ) ENGINE=InnoDB;");
                }
                catch { }

                // Step 1.4: Relax audit_logs table foreign key and column nullability so records are never lost
                try
                {
                    string checkFkSql = @"
                        SELECT CONSTRAINT_NAME 
                        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                        WHERE TABLE_SCHEMA = 'bitsward_tickets' 
                          AND TABLE_NAME = 'audit_logs' 
                          AND COLUMN_NAME = 'employee_id' 
                          AND REFERENCED_TABLE_NAME IS NOT NULL;";
                    var dtFk = _dbManager.ExecuteQuery(checkFkSql);
                    foreach (System.Data.DataRow r in dtFk.Rows)
                    {
                        string constraintName = r["CONSTRAINT_NAME"].ToString();
                        try
                        {
                            _dbManager.ExecuteNonQuery($"ALTER TABLE audit_logs DROP FOREIGN KEY `{constraintName}`;");
                        }
                        catch { }
                    }
                    _dbManager.ExecuteNonQuery("ALTER TABLE audit_logs MODIFY COLUMN employee_id VARCHAR(100) NULL;");
                    _dbManager.ExecuteNonQuery("ALTER TABLE audit_logs MODIFY COLUMN ticket_id INT NULL;");
                }
                catch { }

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

                // Step 2.5: Ensure admin account is linked to CTO employee (MGT-001)
                try
                {
                    string linkAdminSql = @"
                        UPDATE users u 
                        JOIN employees e ON e.id = 'MGT-001' 
                        SET u.employee_id = 'MGT-001' 
                        WHERE u.username = 'admin' AND (u.employee_id IS NULL OR u.employee_id = '');";
                    _dbManager.ExecuteNonQuery(linkAdminSql);
                }
                catch { }
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
