using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Interactive dialog for searching, filtering, and selecting created system users
    /// across departments to manually assign an issue during ticket creation.
    /// </summary>
    public partial class AssigneeSearchDialog : Form
    {
        private readonly DatabaseManager _db;
        private readonly string _initialSelectedEmployeeId;
        private DataTable _usersTable;
        private DataView _usersView;

        public string SelectedEmployeeId { get; private set; }
        public string SelectedUsername { get; private set; }
        public string SelectedDisplayName { get; private set; }
        public string SelectedRole { get; private set; }
        public string SelectedDepartment { get; private set; }

        public AssigneeSearchDialog(DatabaseManager db, string initialSelectedEmployeeId = null)
        {
            InitializeComponent();
            _db = db;
            _initialSelectedEmployeeId = initialSelectedEmployeeId;
        }

        private void AssigneeSearchDialog_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                if (_db == null)
                {
                    lblSelectedInfo.Text = "Database connection unavailable.";
                    return;
                }

                string query = @"
                    SELECT 
                        u.id AS UserId, 
                        u.username AS Username, 
                        u.role AS Role, 
                        u.employee_id AS EmployeeId,
                        COALESCE(e.name, u.username) AS EmployeeName, 
                        COALESCE(e.designation, u.role) AS Designation, 
                        COALESCE(d.name, 'Unassigned') AS DepartmentName,
                        (SELECT COUNT(*) FROM tickets t WHERE t.assigned_employee_id = u.employee_id AND t.status IN ('Open', 'Assigned', 'In Progress')) AS ActiveTickets
                    FROM users u
                    LEFT JOIN employees e ON u.employee_id = e.id
                    LEFT JOIN departments d ON e.department_id = d.id
                    WHERE u.employee_id IS NOT NULL
                    ORDER BY u.username ASC";

                _usersTable = _db.ExecuteQuery(query);
                _usersView = _usersTable.DefaultView;
                gridUsers.DataSource = _usersView;

                ConfigureUserGrid();
                UpdateUserCount();

                // Restore initial selection if matching
                if (!string.IsNullOrEmpty(_initialSelectedEmployeeId))
                {
                    SelectRowByEmployeeId(_initialSelectedEmployeeId);
                }
                else
                {
                    UpdateSelectionInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load created users:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureUserGrid()
        {
            if (gridUsers.Columns.Count == 0) return;

            gridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            SetColumnWidth(gridUsers, "EmployeeId", 80);
            SetColumnWidth(gridUsers, "Username", 100);
            SetColumnWidth(gridUsers, "Role", 85);
            SetColumnWidth(gridUsers, "ActiveTickets", 90);

            var nameCol = FindColumn(gridUsers, "EmployeeName");
            if (nameCol != null)
            {
                nameCol.HeaderText = "Full Name";
                nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            var deptCol = FindColumn(gridUsers, "DepartmentName");
            if (deptCol != null)
            {
                deptCol.HeaderText = "Department";
                deptCol.Width = 140;
            }

            var desigCol = FindColumn(gridUsers, "Designation");
            if (desigCol != null)
            {
                desigCol.HeaderText = "Designation";
                desigCol.Width = 130;
            }

            var empIdCol = FindColumn(gridUsers, "EmployeeId");
            if (empIdCol != null) empIdCol.HeaderText = "Emp ID";

            var loadCol = FindColumn(gridUsers, "ActiveTickets");
            if (loadCol != null)
            {
                loadCol.HeaderText = "Active Load";
                loadCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            HideColumn(gridUsers, "UserId");
        }

        private void FilterUsers()
        {
            if (_usersView == null) return;

            string search = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                _usersView.RowFilter = string.Empty;
            }
            else
            {
                string safe = EscapeLikeValue(search);
                _usersView.RowFilter = string.Format(
                    "EmployeeName LIKE '%{0}%' OR Username LIKE '%{0}%' OR Role LIKE '%{0}%' OR DepartmentName LIKE '%{0}%' OR Designation LIKE '%{0}%' OR EmployeeId LIKE '%{0}%'",
                    safe);
            }

            UpdateUserCount();
            UpdateSelectionInfo();
        }

        private void UpdateUserCount()
        {
            int count = gridUsers.Rows.Count;
            lblUserCount.Text = $"Showing {count} user{(count == 1 ? "" : "s")}";
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                FilterUsers();
            }
            else if (e.KeyCode == Keys.Down && gridUsers.Rows.Count > 0)
            {
                gridUsers.Focus();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            FilterUsers();
        }

        private void GridUsers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionInfo();
        }

        private void UpdateSelectionInfo()
        {
            if (gridUsers.SelectedRows.Count > 0)
            {
                var row = gridUsers.SelectedRows[0];
                var nameCell = FindCell(row, "EmployeeName");
                var userCell = FindCell(row, "Username");
                var roleCell = FindCell(row, "Role");
                var deptCell = FindCell(row, "DepartmentName");
                var idCell = FindCell(row, "EmployeeId");

                string name = nameCell?.Value?.ToString() ?? "User";
                string user = userCell?.Value?.ToString() ?? "";
                string role = roleCell?.Value?.ToString() ?? "";
                string dept = deptCell?.Value?.ToString() ?? "";
                string empId = idCell?.Value?.ToString() ?? "";

                lblSelectedInfo.Text = $"Selected: {name} (@{user} — {role}, {dept} | {empId})";
                lblSelectedInfo.ForeColor = Color.FromArgb(46, 204, 113);
                btnSelect.Enabled = true;
            }
            else
            {
                lblSelectedInfo.Text = "Selected: Auto-Assign (Smart 3-Tier Routing)";
                lblSelectedInfo.ForeColor = Color.FromArgb(189, 195, 199);
                btnSelect.Enabled = false;
            }
        }

        private void GridUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnSelect_Click(sender, e);
            }
        }

        private void GridUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BtnSelect_Click(sender, e);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user from the list to assign, or click 'Auto-Assign'.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = gridUsers.SelectedRows[0];
            var idCell = FindCell(row, "EmployeeId");
            var nameCell = FindCell(row, "EmployeeName");
            var userCell = FindCell(row, "Username");
            var roleCell = FindCell(row, "Role");
            var deptCell = FindCell(row, "DepartmentName");

            SelectedEmployeeId = idCell?.Value?.ToString();
            SelectedUsername = userCell?.Value?.ToString();
            SelectedRole = roleCell?.Value?.ToString();
            SelectedDepartment = deptCell?.Value?.ToString();

            string name = nameCell?.Value?.ToString() ?? SelectedUsername;
            SelectedDisplayName = $"{name} (@{SelectedUsername} - {SelectedRole})";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SelectedEmployeeId = null;
            SelectedUsername = null;
            SelectedRole = null;
            SelectedDepartment = null;
            SelectedDisplayName = null;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SelectRowByEmployeeId(string empId)
        {
            if (string.IsNullOrEmpty(empId)) return;
            foreach (DataGridViewRow row in gridUsers.Rows)
            {
                var cell = FindCell(row, "EmployeeId");
                if (cell?.Value != null && string.Equals(cell.Value.ToString(), empId, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    gridUsers.CurrentCell = row.Cells[0];
                    UpdateSelectionInfo();
                    return;
                }
            }
        }

        private static string EscapeLikeValue(string value)
        {
            return value.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
        }

        private static void SetColumnWidth(DataGridView grid, string colName, int width)
        {
            if (grid == null || grid.Columns == null) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        col.Width = width;
                        break;
                    }
                }
            }
            catch { }
        }

        private static void HideColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col.Visible = false;
                        break;
                    }
                }
            }
            catch { }
        }

        private static DataGridViewColumn FindColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null) return null;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    return col;
            }
            return null;
        }

        private static DataGridViewCell FindCell(DataGridViewRow row, string colName)
        {
            if (row == null) return null;
            var grid = row.DataGridView;
            if (grid != null)
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                        return row.Cells[col.Index];
                }
            }
            return null;
        }
    }
}
