MySql.Data Connector Library
=============================

The project references MySql.Data.dll from this /lib folder.

HOW TO OBTAIN:
--------------
Option 1 (Recommended) - NuGet Package Manager:
  In Visual Studio: Tools > NuGet Package Manager > Package Manager Console
  Run: Install-Package MySql.Data

Option 2 - XAMPP Installation:
  If you have XAMPP installed, look for the MySQL connector in:
  C:\xampp\php\ext\   (this is the PHP extension, NOT for .NET - ignore it)

  Instead use the official MySQL Connector/NET:
  https://dev.mysql.com/downloads/connector/net/

Option 3 - Standalone Download:
  Download MySQL Connector/NET 8.x from:
  https://dev.mysql.com/downloads/connector/net/
  Install and copy MySql.Data.dll from the installation directory to this /lib folder.

XAMPP MySQL Server Settings (used by DatabaseManager.cs):
----------------------------------------------------------
  Server:   localhost or 127.0.0.1
  Port:     3306 (default)
  Username: root
  Password: (empty by default in XAMPP)
  Database: bitsward_tickets (auto-created by schema.sql on first run)
