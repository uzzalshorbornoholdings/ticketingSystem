using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace BitswardITSM.Core
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _serverConnectionString; // Used for DB creation before database exists
        private const string DatabaseName = "bitsward_tickets";

        public DatabaseManager(string server = "localhost", string user = "root", string password = "")
        {
            _serverConnectionString = $"Server={server};Uid={user};Pwd={password};AllowUserVariables=True;";
            _connectionString = $"Server={server};Database={DatabaseName};Uid={user};Pwd={password};AllowUserVariables=True;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Initializes the database structure by running schema.sql.
        /// </summary>
        /// <param name="schemaSqlPath">Absolute path to schema.sql file.</param>
        public void InitializeDatabase(string schemaSqlPath)
        {
            if (!File.Exists(schemaSqlPath))
            {
                throw new FileNotFoundException("Schema file not found.", schemaSqlPath);
            }

            string script = File.ReadAllText(schemaSqlPath);

            // Step 1: Create Database if not exists using server connection string
            using (var serverConn = new MySqlConnection(_serverConnectionString))
            {
                serverConn.Open();
                using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {DatabaseName};", serverConn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Step 2: Run seed/schema script using the direct database connection
            using (var conn = GetConnection())
            {
                conn.Open();
                // MySqlScript handles multi-statement scripts including delimiters correctly
                var scriptExecutor = new MySqlScript(conn, script);
                scriptExecutor.Execute();
            }
        }

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string query, MySqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            var dataTable = new DataTable();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }
    }
}
