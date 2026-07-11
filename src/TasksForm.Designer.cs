namespace BitswardITSM.Core
{
    partial class TasksForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
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
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Task Manager";
            this.Size = new System.Drawing.Size(820, 540);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.Load += new System.EventHandler(this.TasksForm_Load);

            // Header Panel
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 48;
            this.panelHeader.Controls.Add(this.lblHeader);

            this.lblHeader.Text = "📋  Task Manager — All Tasks";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // DataGridView
            this.gridTasks.Location = new System.Drawing.Point(10, 58);
            this.gridTasks.Size = new System.Drawing.Size(796, 360);
            this.gridTasks.ReadOnly = true;
            this.gridTasks.AllowUserToAddRows = false;
            this.gridTasks.AllowUserToDeleteRows = false;
            this.gridTasks.RowHeadersVisible = false;
            this.gridTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTasks.MultiSelect = false;
            this.gridTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTasks.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.gridTasks.SelectionChanged += new System.EventHandler(this.GridTasks_SelectionChanged);
            this.gridTasks.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridTasks_DataBindingComplete);

            // Bottom Panel
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelBottom.Location = new System.Drawing.Point(10, 428);
            this.panelBottom.Size = new System.Drawing.Size(796, 75);

            // Selected task label
            this.lblSelectedTask.Text = "Select a task to manage...";
            this.lblSelectedTask.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSelectedTask.ForeColor = System.Drawing.Color.FromArgb(200, 207, 214);
            this.lblSelectedTask.Location = new System.Drawing.Point(10, 8);
            this.lblSelectedTask.Size = new System.Drawing.Size(450, 18);
            this.panelBottom.Controls.Add(this.lblSelectedTask);

            // Count label
            this.lblCount.Text = "";
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(150, 160, 175);
            this.lblCount.Location = new System.Drawing.Point(10, 28);
            this.lblCount.Size = new System.Drawing.Size(350, 18);
            this.panelBottom.Controls.Add(this.lblCount);

            // Status label
            this.lblStatusLabel.Text = "Update Status:";
            this.lblStatusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusLabel.ForeColor = System.Drawing.Color.White;
            this.lblStatusLabel.Location = new System.Drawing.Point(10, 50);
            this.lblStatusLabel.Size = new System.Drawing.Size(100, 18);
            this.panelBottom.Controls.Add(this.lblStatusLabel);

            // Status combo
            this.cmbTaskStatus.Location = new System.Drawing.Point(115, 47);
            this.cmbTaskStatus.Size = new System.Drawing.Size(140, 24);
            this.cmbTaskStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTaskStatus.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.cmbTaskStatus.ForeColor = System.Drawing.Color.White;
            this.cmbTaskStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskStatus.Items.AddRange(new object[] { "Pending", "In Progress", "Done", "Cancelled" });
            this.panelBottom.Controls.Add(this.cmbTaskStatus);

            // Update button
            this.btnUpdateStatus.Text = "Update Status";
            this.btnUpdateStatus.Location = new System.Drawing.Point(265, 46);
            this.btnUpdateStatus.Size = new System.Drawing.Size(110, 26);
            this.btnUpdateStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUpdateStatus.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnUpdateStatus.ForeColor = System.Drawing.Color.White;
            this.btnUpdateStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateStatus.FlatAppearance.BorderSize = 0;
            this.btnUpdateStatus.Click += new System.EventHandler(this.BtnUpdateStatus_Click);
            this.panelBottom.Controls.Add(this.btnUpdateStatus);

            // Close button
            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(680, 46);
            this.btnClose.Size = new System.Drawing.Size(106, 26);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.panelBottom.Controls.Add(this.btnClose);

            // Wire up
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.gridTasks);
            this.Controls.Add(this.panelBottom);

            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
