namespace BitswardITSM.Core
{
    partial class TasksForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Label lblSearchCount;
        private System.Windows.Forms.DataGridView gridTasks;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblSelectedTask;
        private System.Windows.Forms.Label lblStatusLabel;
        private System.Windows.Forms.ComboBox cmbTaskStatus;
        private System.Windows.Forms.Button btnUpdateStatus;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblCount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.lblSearchCount = new System.Windows.Forms.Label();
            this.gridTasks = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblSelectedTask = new System.Windows.Forms.Label();
            this.lblStatusLabel = new System.Windows.Forms.Label();
            this.cmbTaskStatus = new System.Windows.Forms.ComboBox();
            this.btnUpdateStatus = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Task Manager";
            this.Size = new System.Drawing.Size(880, 580);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.Load += new System.EventHandler(this.TasksForm_Load);

            // Header Panel
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 48;
            this.panelHeader.Controls.Add(this.lblHeader);

            this.lblHeader.Text = "📋  Task Manager — All Tasks";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Search Panel
            this.panelSearch.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelSearch.Location = new System.Drawing.Point(10, 54);
            this.panelSearch.Size = new System.Drawing.Size(844, 36);
            this.panelSearch.Controls.Add(this.lblSearchCount);
            this.panelSearch.Controls.Add(this.btnClearSearch);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.lblSearchIcon);

            this.lblSearchIcon.Text = "🔍";
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblSearchIcon.Location = new System.Drawing.Point(8, 8);
            this.lblSearchIcon.Size = new System.Drawing.Size(22, 20);

            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.txtSearch.Location = new System.Drawing.Point(32, 6);
            this.txtSearch.Size = new System.Drawing.Size(650, 24);
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSearch_KeyDown);

            this.btnClearSearch.Text = "✖";
            this.btnClearSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearSearch.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnClearSearch.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.FlatAppearance.BorderSize = 0;
            this.btnClearSearch.Location = new System.Drawing.Point(688, 6);
            this.btnClearSearch.Size = new System.Drawing.Size(26, 24);
            this.btnClearSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);

            this.lblSearchCount.Text = "";
            this.lblSearchCount.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblSearchCount.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblSearchCount.Location = new System.Drawing.Point(720, 8);
            this.lblSearchCount.Size = new System.Drawing.Size(115, 20);
            this.lblSearchCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // DataGridView
            this.gridTasks.Location = new System.Drawing.Point(10, 96);
            this.gridTasks.Size = new System.Drawing.Size(844, 368);
            this.gridTasks.ReadOnly = true;
            this.gridTasks.AllowUserToAddRows = false;
            this.gridTasks.AllowUserToDeleteRows = false;
            this.gridTasks.RowHeadersVisible = false;
            this.gridTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTasks.MultiSelect = false;
            this.gridTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTasks.BackgroundColor = System.Drawing.Color.White;
            this.gridTasks.SelectionChanged += new System.EventHandler(this.GridTasks_SelectionChanged);
            this.gridTasks.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridTasks_DataBindingComplete);

            // Bottom Panel
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelBottom.Location = new System.Drawing.Point(10, 472);
            this.panelBottom.Size = new System.Drawing.Size(844, 62);

            // Selected task label
            this.lblSelectedTask.Text = "Select a task to manage...";
            this.lblSelectedTask.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSelectedTask.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblSelectedTask.Location = new System.Drawing.Point(10, 6);
            this.lblSelectedTask.Size = new System.Drawing.Size(500, 18);
            this.panelBottom.Controls.Add(this.lblSelectedTask);

            // Count label
            this.lblCount.Text = "";
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblCount.Location = new System.Drawing.Point(520, 6);
            this.lblCount.Size = new System.Drawing.Size(314, 18);
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.panelBottom.Controls.Add(this.lblCount);

            // Status label
            this.lblStatusLabel.Text = "Update Status:";
            this.lblStatusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusLabel.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblStatusLabel.Location = new System.Drawing.Point(10, 31);
            this.lblStatusLabel.Size = new System.Drawing.Size(95, 20);
            this.panelBottom.Controls.Add(this.lblStatusLabel);

            // Status combo
            this.cmbTaskStatus.Location = new System.Drawing.Point(110, 28);
            this.cmbTaskStatus.Size = new System.Drawing.Size(140, 24);
            this.cmbTaskStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTaskStatus.BackColor = System.Drawing.Color.White;
            this.cmbTaskStatus.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.cmbTaskStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskStatus.Items.AddRange(new object[] { "Pending", "In Progress", "Done", "Cancelled" });
            this.panelBottom.Controls.Add(this.cmbTaskStatus);

            // Update button
            this.btnUpdateStatus.Text = "Update Status";
            this.btnUpdateStatus.Location = new System.Drawing.Point(260, 27);
            this.btnUpdateStatus.Size = new System.Drawing.Size(120, 26);
            this.btnUpdateStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUpdateStatus.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnUpdateStatus.ForeColor = System.Drawing.Color.White;
            this.btnUpdateStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateStatus.FlatAppearance.BorderSize = 0;
            this.btnUpdateStatus.Click += new System.EventHandler(this.BtnUpdateStatus_Click);
            this.panelBottom.Controls.Add(this.btnUpdateStatus);

            // Close button
            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(730, 27);
            this.btnClose.Size = new System.Drawing.Size(104, 26);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.panelBottom.Controls.Add(this.btnClose);

            // Wire up
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.gridTasks);
            this.Controls.Add(this.panelBottom);

            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
