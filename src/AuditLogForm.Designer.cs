namespace BitswardITSM.Core
{
    partial class AuditLogForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Label lblSearchCount;
        private System.Windows.Forms.DataGridView gridLogs;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnExportLogs;
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
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.lblSearchCount = new System.Windows.Forms.Label();
            this.gridLogs = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnExportLogs = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Audit Log Viewer";
            this.Size = new System.Drawing.Size(960, 580);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.Load += new System.EventHandler(this.AuditLogForm_Load);

            // Header
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 48;
            this.panelHeader.Controls.Add(this.lblHeader);

            this.lblHeader.Text = "📋  Audit Log — All System Events";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Search Panel
            this.panelSearch.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelSearch.Location = new System.Drawing.Point(10, 54);
            this.panelSearch.Size = new System.Drawing.Size(936, 36);
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
            this.txtSearch.Size = new System.Drawing.Size(730, 24);
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);

            this.btnClearSearch.Text = "✖";
            this.btnClearSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearSearch.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnClearSearch.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.FlatAppearance.BorderSize = 0;
            this.btnClearSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearSearch.Location = new System.Drawing.Point(768, 6);
            this.btnClearSearch.Size = new System.Drawing.Size(28, 24);
            this.btnClearSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);

            this.lblSearchCount.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSearchCount.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblSearchCount.Location = new System.Drawing.Point(802, 9);
            this.lblSearchCount.Size = new System.Drawing.Size(126, 18);
            this.lblSearchCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // Grid
            this.gridLogs.Location = new System.Drawing.Point(10, 96);
            this.gridLogs.Size = new System.Drawing.Size(936, 404);
            this.gridLogs.ReadOnly = true;
            this.gridLogs.AllowUserToAddRows = false;
            this.gridLogs.AllowUserToDeleteRows = false;
            this.gridLogs.RowHeadersVisible = false;
            this.gridLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLogs.MultiSelect = false;
            this.gridLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridLogs.BackgroundColor = System.Drawing.Color.White;
            this.gridLogs.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridLogs_DataBindingComplete);

            // Bottom panel
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelBottom.Location = new System.Drawing.Point(10, 507);
            this.panelBottom.Size = new System.Drawing.Size(936, 36);

            this.lblCount.Text = "";
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblCount.Location = new System.Drawing.Point(10, 10);
            this.lblCount.Size = new System.Drawing.Size(380, 18);
            this.panelBottom.Controls.Add(this.lblCount);

            this.btnExportLogs.Text = "📁 Export Logs";
            this.btnExportLogs.Location = new System.Drawing.Point(595, 5);
            this.btnExportLogs.Size = new System.Drawing.Size(106, 26);
            this.btnExportLogs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExportLogs.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnExportLogs.ForeColor = System.Drawing.Color.White;
            this.btnExportLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportLogs.FlatAppearance.BorderSize = 0;
            this.btnExportLogs.Click += new System.EventHandler(this.BtnExportLogs_Click);
            this.panelBottom.Controls.Add(this.btnExportLogs);

            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.Location = new System.Drawing.Point(710, 5);
            this.btnRefresh.Size = new System.Drawing.Size(100, 26);
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            this.panelBottom.Controls.Add(this.btnRefresh);

            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(820, 5);
            this.btnClose.Size = new System.Drawing.Size(106, 26);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.panelBottom.Controls.Add(this.btnClose);

            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.gridLogs);
            this.Controls.Add(this.panelBottom);

            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
