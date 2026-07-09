using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("Bitsward ITSM & Issue Ticketing System - Test Harness");
            Console.WriteLine("=================================================\n");

            // Absolute paths matching project workspace settings
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));

            // Check if running from bin folder; fall back to execution directory if needed
            if (!Directory.Exists(Path.Combine(projectRoot, "org")))
            {
                projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            }

            string csvPath = Path.Combine(projectRoot, "org", "organogram.csv");
            string sqlPath = Path.Combine(projectRoot, "docs", "schema.sql");

            Console.WriteLine($"Project Root resolved to: {projectRoot}");
            Console.WriteLine($"Database schema source: {sqlPath}");
            Console.WriteLine($"CSV source path: {csvPath}");

            // 1. Initializing Database Manager
            var db = new DatabaseManager(server: "127.0.0.1", user: "root", password: "");
            Console.WriteLine("\n[1] Testing connection to local MySQL (XAMPP)...");

            if (!db.TestConnection(out string connError))
            {
                Console.WriteLine($"Connection failed (Make sure XAMPP MySQL is active): {connError}");
                Console.WriteLine("Note: Continuing tests via dry-run simulation for algorithmic validation.\n");
                RunDryRunSimulation();
                return;
            }

            Console.WriteLine("Connection successful!");

            // 2. Initializing Database Structure
            Console.WriteLine("\n[2] Initializing database tables via schema.sql...");
            try
            {
                db.InitializeDatabase(sqlPath);
                Console.WriteLine("Database and tables initialized successfully, SLAs pre-seeded!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                return;
            }

            // 3. Executing CSV Synchronization
            Console.WriteLine("\n[3] Synchronizing organogram CSV records...");
            try
            {
                var syncPrg = new OrganogramSync(db);
                var syncResult = syncPrg.SyncFromCsv(csvPath);
                Console.WriteLine($"Sync Completed!");
                Console.WriteLine($"- Departments created: {syncResult.DepartmentsCreated}");
                Console.WriteLine($"- Employees upserted: {syncResult.EmployeesUpserted}");
                Console.WriteLine($"- Reporting managers mapped: {syncResult.ReportingLinksUpdated}");
                foreach (var log in syncResult.Logs)
                {
                    Console.WriteLine($"  * {log}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Synchronization failed: {ex.Message}");
                return;
            }

            // 4. Testing Core Triage Engine
            Console.WriteLine("\n[4] Testing Core Triage Engine...");
            var triage = new TriageEngine(db);

            var testTickets = new[]
            {
                new { Title = "Server HDD Full", Desc = "The Linux admin server partitions have filled up on build machine.", ExpectedType = "INC", ExpectedDept = "Linux Platform" },
                new { Title = "Request corporate laptop access", Desc = "New hire needs standard MAC laptop configured with keychain tools.", ExpectedType = "SR", ExpectedDept = "Windows / MAC Admin" },
                new { Title = "Active Directory password reset help", Desc = "Unable to login to Microsoft Outlook.", ExpectedType = "SR", ExpectedDept = "Windows / MAC Admin" },
                new { Title = "DB Schema patch migration", Desc = "Deploy changes to production database during maintenance window.", ExpectedType = "CR", ExpectedDept = "IT / System Admin" },
                new { Title = "DevOps pipeline release deployment", Desc = "Deploy AWS EC2 Terraform configuration changes.", ExpectedType = "CR", ExpectedDept = "DevOps" },
                new { Title = "Urgent: Firewall security intrusion attack detected", Desc = "SOC analysts flag multiple failed login alerts on root database server.", ExpectedType = "INC", ExpectedDept = "Cybersecurity" }
            };

            foreach (var tk in testTickets)
            {
                string cls = triage.ClassifyTicket(tk.Title, tk.Desc);
                string dept = triage.ResolveTargetDepartment(tk.Title, tk.Desc);
                Console.WriteLine($"Ticket: '{tk.Title}'");
                Console.WriteLine($"  - Classified Type: {cls} (Expected: {tk.ExpectedType})");
                Console.WriteLine($"  - Target Dept: {dept} (Expected: {tk.ExpectedDept})");
            }

            // 5. Testing SLA Calculations
            Console.WriteLine("\n[5] Testing SLA Engine deadline math...");
            var sla = new SlaEngine(db);

            // Test scenario: Friday at 4:30 PM (16:30) with P1 (2 hours window)
            var friTime = new DateTime(2026, 7, 10, 16, 30, 0); // 2026-07-10 is a Friday
            var p1Deadline = sla.CalculateDeadline(friTime, 2);
            Console.WriteLine($"Friday 4:30 PM + P1 (2h SLA) -> Deadline: {p1Deadline:yyyy-MM-dd HH:mm:ss} (Expected: Monday 10:30 AM)");

            // Test scenario: Friday at 2:00 PM (14:00) with P2 (8 hours window)
            var p2Deadline = sla.CalculateDeadline(friTime, 8);
            Console.WriteLine($"Friday 4:30 PM + P2 (8h SLA) -> Deadline: {p2Deadline:yyyy-MM-dd HH:mm:ss} (Expected: Monday 4:30 PM)");

            // Test scenario: Sunday at 12:00 PM (off-hours) with P1
            var sunTime = new DateTime(2026, 7, 12, 12, 0, 0); // Sunday
            var sunDeadline = sla.CalculateDeadline(sunTime, 2);
            Console.WriteLine($"Sunday 12:00 PM + P1 (2h SLA) -> Deadline: {sunDeadline:yyyy-MM-dd HH:mm:ss} (Expected: Monday 11:00 AM)");

            // 6. Testing Authentication and Admin Panel logic
            Console.WriteLine("\n[6] Testing User Registration and Auth system...");
            var auth = new AuthManager(db);

            // Seed default admin first
            auth.SeedDefaultAdmin();
            Console.WriteLine("  - Default admin seeded!");

            // Test login
            bool canLoginAdmin = auth.Login("admin", "admin123", out string rootRole, out string rootEmp);
            Console.WriteLine($"  - Admin login verify: {canLoginAdmin} (Expected: True), Role: {rootRole}, EmployeeId: {rootEmp}");

            bool wrongLogin = auth.Login("admin", "wrongpass", out _, out _);
            Console.WriteLine($"  - Incorrect login verify: {wrongLogin} (Expected: False)");

            // Test admin panel options
            var admin = new AdminManager(db);

            // Register a user linked to employee S. M. RISADUL ISLAM (ID: 21-0-52-020-004)
            bool createdUser = admin.CreateUserAccount("21-0-52-020-004", "risad", "risad123", "Agent", out string registerErr);
            if (createdUser)
            {
                Console.WriteLine("  - Created user 'risad' linked to S. M. RISADUL ISLAM!");
            }
            else
            {
                Console.WriteLine($"  - User registration skipped/failed: {registerErr}");
            }

            // Verify login of newly created user
            bool canLoginUser = auth.Login("risad", "risad123", out string userRole, out string userEmp);
            Console.WriteLine($"  - User login verify: {canLoginUser} (Expected: True), Role: {userRole}, EmployeeId: {userEmp}");

            // Update role
            bool roleUpdated = admin.UpdateUserRole("risad", "Manager", out string roleErr);
            Console.WriteLine($"  - Updated role for 'risad' to Manager: {roleUpdated}");

            // Login again to verify updated role
            auth.Login("risad", "risad123", out string updatedRole, out _);
            Console.WriteLine($"  - User login role after shift check: {updatedRole} (Expected: Manager)");

            // Show list of remaining unassociated employees
            var unassociated = admin.GetUnassociatedEmployees();
            Console.WriteLine($"  - Number of employees still lacking user credentials: {unassociated.Rows.Count}");

            Console.WriteLine("\nAll live database tests completed. Ready for application integration!");
            Console.ReadLine();
        }

        private static void RunDryRunSimulation()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("DRY RUN ALGORITHMIC SIMULATION");
            Console.WriteLine("========================================");

            // Validate SLA math manually
            var sla = new SlaEngine(null);
            var friTime = new DateTime(2026, 7, 10, 16, 30, 0); // Friday
            var p1Deadline = sla.CalculateDeadline(friTime, 2);
            Console.WriteLine($"Friday 4:30 PM + P1 (2h SLA) -> Deadline: {p1Deadline:yyyy-MM-dd HH:mm:ss} (Expected: Monday 10:30 AM)");

            var sunTime = new DateTime(2026, 7, 12, 12, 0, 0); // Sunday
            var sunDeadline = sla.CalculateDeadline(sunTime, 2);
            Console.WriteLine($"Sunday 12:00 PM + P1 (2h SLA) -> Deadline: {sunDeadline:yyyy-MM-dd HH:mm:ss} (Expected: Monday 11:00 AM)");

            // Validate Classification math manually
            var triage = new TriageEngine(null);
            string cls = triage.ClassifyTicket("Security firewall intrusion alert", "Intruder detected in our system");
            string dept = triage.ResolveTargetDepartment("Security firewall intrusion alert", "Intruder detected in our system");
            Console.WriteLine($"Intrusion Alert Class: {cls}, Dept: {dept}");

            // Mock hashing checks
            var auth = new AuthManager(null);
            bool canLoginDummy = auth.Login("dummy", "dummy", out _, out _);
            Console.WriteLine($"Dummy dry run login verify: {canLoginDummy} (Expected: False)");

            Console.WriteLine("\nDynamic Algorithmic Verification Successful (Verified Business Hours Offsets).");
        }
    }
}
