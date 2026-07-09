// This file contains the original console test harness used to validate
// the core engines (DB, CSV Sync, Triage, SLA, Auth) before WinForms integration.
// It is NOT compiled into the project - kept for reference / debugging.
//
// To run manually: copy to a .NET Console App project and reference the same assemblies.

using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    class TestHarness
    {
        static void Main_TestHarness(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("Bitsward ITSM & Issue Ticketing System - Test Harness");
            Console.WriteLine("=================================================\n");

            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\")); 
            if (!Directory.Exists(Path.Combine(projectRoot, "org")))
                projectRoot = AppDomain.CurrentDomain.BaseDirectory;

            string csvPath = Path.Combine(projectRoot, "org", "organogram.csv");
            string sqlPath = Path.Combine(projectRoot, "docs", "schema.sql");

            // 1. DB Connection
            var db = new DatabaseManager("127.0.0.1", "root", "");
            if (!db.TestConnection(out string connError))
            {
                Console.WriteLine($"Connection failed: {connError}");
                return;
            }
            Console.WriteLine("[1] DB Connection OK");

            // 2. Init Schema
            db.InitializeDatabase(sqlPath);
            Console.WriteLine("[2] Schema initialized OK");

            // 3. CSV Sync
            var sync = new OrganogramSync(db);
            var result = sync.SyncFromCsv(csvPath);
            Console.WriteLine($"[3] Sync: {result.EmployeesUpserted} employees, {result.DepartmentsCreated} departments");

            // 4. Triage
            var triage = new TriageEngine(db);
            string cls = triage.ClassifyTicket("Firewall intrusion detected", "SOC flagged root login alerts");
            string dept = triage.ResolveTargetDepartment("Firewall intrusion detected", "SOC flagged root login alerts");
            Console.WriteLine($"[4] Triage: {cls} -> {dept}");

            // 5. SLA
            var sla = new SlaEngine(db);
            var deadline = sla.CalculateDeadline(new DateTime(2026, 7, 10, 16, 30, 0), 2);
            Console.WriteLine($"[5] SLA P1 deadline (Fri 4:30PM): {deadline:yyyy-MM-dd HH:mm} (Expected: Mon 10:30 AM)");

            // 6. Auth
            var auth = new AuthManager(db);
            auth.SeedDefaultAdmin();
            bool login = auth.Login("admin", "admin123", out string role, out string empId);
            Console.WriteLine($"[6] Auth login: {login} Role: {role}");

            Console.WriteLine("\nAll tests passed!");
            Console.ReadLine();
        }
    }
}
