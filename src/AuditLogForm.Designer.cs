namespace BitswardITSM.Core
{
    partial class AuditLogForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.DataGridView gridLogs;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;

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
            this.gridLogs = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Audit Log Viewer";
            this.Size = new System.Drawing.Size(960, 580);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.Load += new System.EventHandler(this.AuditLogForm_Load);

            // Header
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 48;
            this.panelHeader.Controls.Add(this.lblHeader);

            this.lblHeader.Text = "📋  Audit Log — All System Events";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Grid
            this.gridLogs.Location = new System.Drawing.Point(10, 58);
            this.gridLogs.Size = new System.Drawing.Size(936, 440);
            this.gridLogs.ReadOnly = true;
            this.gridLogs.AllowUserToAddRows = false;
            this.gridLogs.AllowUserToDeleteRows = false;
            this.gridLogs.RowHeadersVisible = false;
            this.gridLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLogs.MultiSelect = false;
            this.gridLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridLogs.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.gridLogs.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridLogs_DataBindingComplete);

            // Bottom panel
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelBottom.Location = new System.Drawing.Point(10, 507);
            this.panelBottom.Size = new System.Drawing.Size(936, 36);

            this.lblCount.Text = "";
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(150, 160, 175);
            this.lblCount.Location = new System.Drawing.Point(10, 10);
            this.lblCount.Size = new System.Drawing.Size(380, 18);
            this.panelBottom.Controls.Add(this.lblCount);

            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.Location = new System.Drawing.Point(710, 5);
            this.btnRefresh.Size = new System.Drawing.Size(100, 26);
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            this.panelBottom.Controls.Add(this.btnRefresh);

            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(820, 5);
            this.btnClose.Size = new System.Drawing.Size(106, 26);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.panelBottom.Controls.Add(this.btnClose);

            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.gridLogs);
            this.Controls.Add(this.panelBottom);

            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
