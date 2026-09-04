using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Interactive dialog for splitting a ticket into a sub-task with intelligent employee assignment search.
    /// </summary>
    public partial class CreateSubTaskDialog : Form
    {
        private readonly DatabaseManager _db;
        private readonly int _ticketId;
        private readonly string _ticketTitle;

        public string TaskTitle { get; private set; }
        public string AssignedEmployeeId { get; private set; }
        public string AssignedDisplayName { get; private set; }

        public CreateSubTaskDialog(DatabaseManager db, int ticketId, string ticketTitle, string defaultAssigneeId = null)
        {
            InitializeComponent();
            _db = db;
            _ticketId = ticketId;
            _ticketTitle = ticketTitle;
            AssignedEmployeeId = defaultAssigneeId;
        }

        private void CreateSubTaskDialog_Load(object sender, EventArgs e)
        {
            lblHeaderSubtitle.Text = $"Split ticket #{_ticketId} ({(_ticketTitle.Length > 45 ? _ticketTitle.Substring(0, 42) + "..." : _ticketTitle)}) into an assigned sub-task";

            // Visual styling
            panelHeader.Paint += PanelHeader_Paint;
            panelAssigneeCard.Paint += PanelAssigneeCard_Paint;
            ModernStyle.StyleTextBox(txtTaskTitle);

            // Fetch default assignee name if pre-assigned
            if (!string.IsNullOrEmpty(AssignedEmployeeId) && _db != null)
            {
                try
                {
                    string q = "SELECT name, designation FROM employees WHERE id = @id";
                    var dt = _db.ExecuteQuery(q, new MySql.Data.MySqlClient.MySqlParameter[] { new MySql.Data.MySqlClient.MySqlParameter("@id", AssignedEmployeeId) });
                    if (dt.Rows.Count > 0)
                    {
                        string name = dt.Rows[0]["name"].ToString();
                        string desig = dt.Rows[0]["designation"]?.ToString();
                        AssignedDisplayName = $"{name} ({desig})";
                    }
                }
                catch { }
            }

            UpdateAssigneeDisplay();
            txtTaskTitle.Focus();
        }

        private void PanelHeader_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new LinearGradientBrush(
                panel.ClientRectangle,
                ThemeColors.ElectricBlue, ThemeColors.Teal,
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void PanelAssigneeCard_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using (var path = GdiPlus.CreateRoundedRectanglePath(rect, 8))
            {
                using (var brush = new SolidBrush(ThemeColors.CardSurface))
                    g.FillPath(brush, path);
                using (var pen = new Pen(ThemeColors.BorderSubtle, 1))
                    g.DrawPath(pen, path);
            }
        }

        private void UpdateAssigneeDisplay()
        {
            if (!string.IsNullOrEmpty(AssignedEmployeeId))
            {
                lblSelectedAssignee.Text = $"👤 {(!string.IsNullOrEmpty(AssignedDisplayName) ? AssignedDisplayName : AssignedEmployeeId)}";
                lblSelectedAssignee.ForeColor = ThemeColors.SuccessGreen;
                btnClearAssignee.Visible = true;
            }
            else
            {
                lblSelectedAssignee.Text = "👤 Unassigned (Click button below to select an employee)";
                lblSelectedAssignee.ForeColor = ThemeColors.TextMuted;
                btnClearAssignee.Visible = false;
            }
        }

        private void BtnSearchAssignee_Click(object sender, EventArgs e)
        {
            if (_db == null)
            {
                MessageBox.Show("Database connection is not available for employee search.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var searchDlg = new AssigneeSearchDialog(_db, AssignedEmployeeId))
            {
                if (searchDlg.ShowDialog(this) == DialogResult.OK)
                {
                    AssignedEmployeeId = searchDlg.SelectedEmployeeId;
                    AssignedDisplayName = searchDlg.SelectedDisplayName;
                    UpdateAssigneeDisplay();
                }
            }
        }

        private void BtnClearAssignee_Click(object sender, EventArgs e)
        {
            AssignedEmployeeId = null;
            AssignedDisplayName = null;
            UpdateAssigneeDisplay();
        }

        private void TxtTaskTitle_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTaskTitle.Text))
            {
                lblValidation.Text = string.Empty;
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                lblValidation.Text = "⚠️ Please enter a sub-task title or action item.";
                txtTaskTitle.Focus();
                return;
            }

            TaskTitle = title;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
