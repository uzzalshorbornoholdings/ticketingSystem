namespace BitswardITSM.Core
{
    partial class ReportsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;

        // Filters Panel
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.Label lblDateRange;
        private System.Windows.Forms.ComboBox cboDateRange;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.ComboBox cboDepartment;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.ComboBox cboPriority;
        private System.Windows.Forms.Button btnApplyFilters;

        // KPI Cards Panel
        private System.Windows.Forms.Panel panelKpi;
        private System.Windows.Forms.Panel cardCompliance;
        private System.Windows.Forms.Label lblComplianceTitle;
        private System.Windows.Forms.Label lblComplianceValue;
        private System.Windows.Forms.Panel cardTotal;
        private System.Windows.Forms.Label lblTotalTitle;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Panel cardAvgTime;
        private System.Windows.Forms.Label lblAvgTimeTitle;
        private System.Windows.Forms.Label lblAvgTimeValue;
        private System.Windows.Forms.Panel cardBreaches;
        private System.Windows.Forms.Label lblBreachesTitle;
        private System.Windows.Forms.Label lblBreachesValue;

        // Tabs
        private System.Windows.Forms.TabControl tabReports;
        private System.Windows.Forms.TabPage tabBreakdown;
        private System.Windows.Forms.DataGridView gridPriorityBreakdown;
        private System.Windows.Forms.DataGridView gridDeptBreakdown;
        private System.Windows.Forms.Label lblPriorityHeader;
        private System.Windows.Forms.Label lblDeptHeader;
        private System.Windows.Forms.TabPage tabAuditTrail;
        private System.Windows.Forms.DataGridView gridDetailedAudit;

        // Bottom Actions
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Button btnExportTicketPdf;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

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
            this.panelFilters = new System.Windows.Forms.Panel();
            this.lblDateRange = new System.Windows.Forms.Label();
            this.cboDateRange = new System.Windows.Forms.ComboBox();
            this.lblDepartment = new System.Windows.Forms.Label();
            this.cboDepartment = new System.Windows.Forms.ComboBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.cboPriority = new System.Windows.Forms.ComboBox();
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.panelKpi = new System.Windows.Forms.Panel();
            this.cardCompliance = new System.Windows.Forms.Panel();
            this.lblComplianceTitle = new System.Windows.Forms.Label();
            this.lblComplianceValue = new System.Windows.Forms.Label();
            this.cardTotal = new System.Windows.Forms.Panel();
            this.lblTotalTitle = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.cardAvgTime = new System.Windows.Forms.Panel();
            this.lblAvgTimeTitle = new System.Windows.Forms.Label();
            this.lblAvgTimeValue = new System.Windows.Forms.Label();
            this.cardBreaches = new System.Windows.Forms.Panel();
            this.lblBreachesTitle = new System.Windows.Forms.Label();
            this.lblBreachesValue = new System.Windows.Forms.Label();
            this.tabReports = new System.Windows.Forms.TabControl();
            this.tabBreakdown = new System.Windows.Forms.TabPage();
            this.gridPriorityBreakdown = new System.Windows.Forms.DataGridView();
            this.gridDeptBreakdown = new System.Windows.Forms.DataGridView();
            this.lblPriorityHeader = new System.Windows.Forms.Label();
            this.lblDeptHeader = new System.Windows.Forms.Label();
            this.tabAuditTrail = new System.Windows.Forms.TabPage();
            this.gridDetailedAudit = new System.Windows.Forms.DataGridView();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnExportTicketPdf = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.panelHeader.SuspendLayout();
            this.panelFilters.SuspendLayout();
            this.panelKpi.SuspendLayout();
            this.cardCompliance.SuspendLayout();
            this.cardTotal.SuspendLayout();
            this.cardAvgTime.SuspendLayout();
            this.cardBreaches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPriorityBreakdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDeptBreakdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetailedAudit)).BeginInit();
            this.tabReports.SuspendLayout();
            this.tabBreakdown.SuspendLayout();
            this.tabAuditTrail.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();

            // ===============================================================
            // FORM
            // ===============================================================
            this.Text = "SLA Compliance Reports & Analytics";
            this.ClientSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.Load += new System.EventHandler(this.ReportsForm_Load);

            // ===============================================================
            // HEADER
            // ===============================================================
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(31, 73, 125);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 52;
            this.panelHeader.Controls.Add(this.lblHeader);

            this.lblHeader.Text = "\U0001F4CA  SLA Compliance — Executive Analytics Dashboard";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ===============================================================
            // FILTER BAR
            // ===============================================================
            this.panelFilters.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelFilters.Location = new System.Drawing.Point(0, 52);
            this.panelFilters.Size = new System.Drawing.Size(1080, 44);

            this.lblDateRange.Text = "Period:";
            this.lblDateRange.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDateRange.ForeColor = System.Drawing.Color.FromArgb(180, 190, 200);
            this.lblDateRange.Location = new System.Drawing.Point(14, 12);
            this.lblDateRange.AutoSize = true;
            this.panelFilters.Controls.Add(this.lblDateRange);

            this.cboDateRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDateRange.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboDateRange.Location = new System.Drawing.Point(65, 9);
            this.cboDateRange.Size = new System.Drawing.Size(140, 25);
            this.cboDateRange.Items.AddRange(new object[] { "All Time", "Last 7 Days", "Last 30 Days", "Last 90 Days" });
            this.cboDateRange.SelectedIndex = 0;
            this.panelFilters.Controls.Add(this.cboDateRange);

            this.lblDepartment.Text = "Dept:";
            this.lblDepartment.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDepartment.ForeColor = System.Drawing.Color.FromArgb(180, 190, 200);
            this.lblDepartment.Location = new System.Drawing.Point(225, 12);
            this.lblDepartment.AutoSize = true;
            this.panelFilters.Controls.Add(this.lblDepartment);

            this.cboDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDepartment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboDepartment.Location = new System.Drawing.Point(270, 9);
            this.cboDepartment.Size = new System.Drawing.Size(175, 25);
            this.panelFilters.Controls.Add(this.cboDepartment);

            this.lblPriority.Text = "Priority:";
            this.lblPriority.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPriority.ForeColor = System.Drawing.Color.FromArgb(180, 190, 200);
            this.lblPriority.Location = new System.Drawing.Point(465, 12);
            this.lblPriority.AutoSize = true;
            this.panelFilters.Controls.Add(this.lblPriority);

            this.cboPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPriority.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboPriority.Location = new System.Drawing.Point(520, 9);
            this.cboPriority.Size = new System.Drawing.Size(130, 25);
            this.cboPriority.Items.AddRange(new object[] { "All Priorities", "P1", "P2", "P3", "P4" });
            this.cboPriority.SelectedIndex = 0;
            this.panelFilters.Controls.Add(this.cboPriority);

            this.btnApplyFilters.Text = "\U0001F504 Apply Filters";
            this.btnApplyFilters.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyFilters.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnApplyFilters.ForeColor = System.Drawing.Color.White;
            this.btnApplyFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyFilters.FlatAppearance.BorderSize = 0;
            this.btnApplyFilters.Location = new System.Drawing.Point(670, 7);
            this.btnApplyFilters.Size = new System.Drawing.Size(120, 30);
            this.btnApplyFilters.Click += new System.EventHandler(this.BtnApplyFilters_Click);
            this.panelFilters.Controls.Add(this.btnApplyFilters);

            // ===============================================================
            // KPI CARDS ROW
            // ===============================================================
            this.panelKpi.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.panelKpi.Location = new System.Drawing.Point(0, 96);
            this.panelKpi.Size = new System.Drawing.Size(1080, 85);

            int cardW = 245;
            int cardH = 68;
            int cardSpacing = 15;
            int cardStartX = 20;
            int cardTopY = 8;

            // Card: Compliance Rate
            SetupKpiCard(this.cardCompliance, this.lblComplianceTitle, this.lblComplianceValue,
                "SLA COMPLIANCE RATE", "—", cardStartX, cardTopY, cardW, cardH,
                System.Drawing.Color.FromArgb(39, 174, 96), System.Drawing.Color.FromArgb(30, 50, 38));
            this.panelKpi.Controls.Add(this.cardCompliance);

            // Card: Total Tickets
            SetupKpiCard(this.cardTotal, this.lblTotalTitle, this.lblTotalValue,
                "TOTAL TICKETS EVALUATED", "—", cardStartX + (cardW + cardSpacing), cardTopY, cardW, cardH,
                System.Drawing.Color.FromArgb(41, 128, 185), System.Drawing.Color.FromArgb(25, 40, 55));
            this.panelKpi.Controls.Add(this.cardTotal);

            // Card: Avg Resolution Time
            SetupKpiCard(this.cardAvgTime, this.lblAvgTimeTitle, this.lblAvgTimeValue,
                "AVG RESOLUTION TIME", "—", cardStartX + 2 * (cardW + cardSpacing), cardTopY, cardW, cardH,
                System.Drawing.Color.FromArgb(142, 68, 173), System.Drawing.Color.FromArgb(40, 25, 50));
            this.panelKpi.Controls.Add(this.cardAvgTime);

            // Card: Breaches
            SetupKpiCard(this.cardBreaches, this.lblBreachesTitle, this.lblBreachesValue,
                "SLA BREACHES", "—", cardStartX + 3 * (cardW + cardSpacing), cardTopY, cardW, cardH,
                System.Drawing.Color.FromArgb(192, 57, 43), System.Drawing.Color.FromArgb(50, 25, 25));
            this.panelKpi.Controls.Add(this.cardBreaches);

            // ===============================================================
            // TABBED DATA VIEWS
            // ===============================================================
            this.tabReports.Location = new System.Drawing.Point(12, 185);
            this.tabReports.Size = new System.Drawing.Size(1056, 475);
            this.tabReports.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tabReports.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            // --- Tab 1: Breakdown ---
            this.tabBreakdown.Text = "  Priority & Department Breakdown  ";
            this.tabBreakdown.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.tabBreakdown.Padding = new System.Windows.Forms.Padding(6);

            this.lblPriorityHeader.Text = "\U0001F3AF  SLA Performance by Priority Level";
            this.lblPriorityHeader.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblPriorityHeader.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblPriorityHeader.Location = new System.Drawing.Point(10, 8);
            this.lblPriorityHeader.AutoSize = true;
            this.tabBreakdown.Controls.Add(this.lblPriorityHeader);

            this.gridPriorityBreakdown.Location = new System.Drawing.Point(10, 32);
            this.gridPriorityBreakdown.Size = new System.Drawing.Size(1022, 170);
            this.gridPriorityBreakdown.ReadOnly = true;
            this.gridPriorityBreakdown.AllowUserToAddRows = false;
            this.gridPriorityBreakdown.AllowUserToDeleteRows = false;
            this.gridPriorityBreakdown.RowHeadersVisible = false;
            this.gridPriorityBreakdown.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPriorityBreakdown.MultiSelect = false;
            this.gridPriorityBreakdown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridPriorityBreakdown.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.tabBreakdown.Controls.Add(this.gridPriorityBreakdown);

            this.lblDeptHeader.Text = "\U0001F3E2  Department Compliance";
            this.lblDeptHeader.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblDeptHeader.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.lblDeptHeader.Location = new System.Drawing.Point(10, 212);
            this.lblDeptHeader.AutoSize = true;
            this.tabBreakdown.Controls.Add(this.lblDeptHeader);

            this.gridDeptBreakdown.Location = new System.Drawing.Point(10, 236);
            this.gridDeptBreakdown.Size = new System.Drawing.Size(1022, 196);
            this.gridDeptBreakdown.ReadOnly = true;
            this.gridDeptBreakdown.AllowUserToAddRows = false;
            this.gridDeptBreakdown.AllowUserToDeleteRows = false;
            this.gridDeptBreakdown.RowHeadersVisible = false;
            this.gridDeptBreakdown.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDeptBreakdown.MultiSelect = false;
            this.gridDeptBreakdown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDeptBreakdown.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.tabBreakdown.Controls.Add(this.gridDeptBreakdown);

            this.tabReports.Controls.Add(this.tabBreakdown);

            // --- Tab 2: Detailed Audit ---
            this.tabAuditTrail.Text = "  Detailed Ticket SLA Audit  ";
            this.tabAuditTrail.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.tabAuditTrail.Padding = new System.Windows.Forms.Padding(6);

            this.gridDetailedAudit.Location = new System.Drawing.Point(10, 10);
            this.gridDetailedAudit.Size = new System.Drawing.Size(1022, 425);
            this.gridDetailedAudit.ReadOnly = true;
            this.gridDetailedAudit.AllowUserToAddRows = false;
            this.gridDetailedAudit.AllowUserToDeleteRows = false;
            this.gridDetailedAudit.RowHeadersVisible = false;
            this.gridDetailedAudit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDetailedAudit.MultiSelect = false;
            this.gridDetailedAudit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDetailedAudit.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.tabAuditTrail.Controls.Add(this.gridDetailedAudit);

            this.tabReports.Controls.Add(this.tabAuditTrail);

            // ===============================================================
            // BOTTOM ACTIONS
            // ===============================================================
            this.panelActions.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Height = 48;

            this.lblStatus.Text = "";
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(150, 160, 175);
            this.lblStatus.Location = new System.Drawing.Point(14, 14);
            this.lblStatus.Size = new System.Drawing.Size(380, 20);
            this.panelActions.Controls.Add(this.lblStatus);

            this.btnExportTicketPdf.Text = "📄 Ticket PDF";
            this.btnExportTicketPdf.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportTicketPdf.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnExportTicketPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportTicketPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportTicketPdf.FlatAppearance.BorderSize = 0;
            this.btnExportTicketPdf.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportTicketPdf.Location = new System.Drawing.Point(495, 9);
            this.btnExportTicketPdf.Size = new System.Drawing.Size(115, 30);
            this.btnExportTicketPdf.Click += new System.EventHandler(this.BtnExportTicketPdf_Click);
            this.panelActions.Controls.Add(this.btnExportTicketPdf);

            this.btnExportPdf.Text = "Export PDF";
            this.btnExportPdf.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportPdf.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnExportPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPdf.FlatAppearance.BorderSize = 0;
            this.btnExportPdf.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportPdf.Location = new System.Drawing.Point(618, 9);
            this.btnExportPdf.Size = new System.Drawing.Size(112, 30);
            this.btnExportPdf.Click += new System.EventHandler(this.BtnExportPdf_Click);
            this.panelActions.Controls.Add(this.btnExportPdf);

            this.btnExportExcel.Text = "Export Excel";
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportExcel.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnExportExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.FlatAppearance.BorderSize = 0;
            this.btnExportExcel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportExcel.Location = new System.Drawing.Point(738, 9);
            this.btnExportExcel.Size = new System.Drawing.Size(110, 30);
            this.btnExportExcel.Click += new System.EventHandler(this.BtnExportExcel_Click);
            this.panelActions.Controls.Add(this.btnExportExcel);

            this.btnExportCsv.Text = "Export CSV";
            this.btnExportCsv.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportCsv.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnExportCsv.ForeColor = System.Drawing.Color.White;
            this.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportCsv.FlatAppearance.BorderSize = 0;
            this.btnExportCsv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportCsv.Location = new System.Drawing.Point(856, 9);
            this.btnExportCsv.Size = new System.Drawing.Size(100, 30);
            this.btnExportCsv.Click += new System.EventHandler(this.BtnExportCsv_Click);
            this.panelActions.Controls.Add(this.btnExportCsv);

            this.btnClose.Text = "Close";
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Location = new System.Drawing.Point(964, 9);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.panelActions.Controls.Add(this.btnClose);

            // ===============================================================
            // ADD CONTROLS TO FORM
            // panelActions added first so Dock.Bottom is processed correctly
            // ===============================================================
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFilters);
            this.Controls.Add(this.panelKpi);
            this.Controls.Add(this.tabReports);

            ((System.ComponentModel.ISupportInitialize)(this.gridPriorityBreakdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDeptBreakdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetailedAudit)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            this.panelKpi.ResumeLayout(false);
            this.cardCompliance.ResumeLayout(false);
            this.cardTotal.ResumeLayout(false);
            this.cardAvgTime.ResumeLayout(false);
            this.cardBreaches.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabBreakdown.ResumeLayout(false);
            this.tabBreakdown.PerformLayout();
            this.tabAuditTrail.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Helper to set up a styled KPI card panel with a title label and value label.
        /// </summary>
        private void SetupKpiCard(System.Windows.Forms.Panel card, System.Windows.Forms.Label titleLbl, System.Windows.Forms.Label valueLbl,
            string title, string value, int x, int y, int w, int h,
            System.Drawing.Color accentColor, System.Drawing.Color bgColor)
        {
            card.BackColor = bgColor;
            card.Location = new System.Drawing.Point(x, y);
            card.Size = new System.Drawing.Size(w, h);

            // Top accent stripe
            var stripe = new System.Windows.Forms.Panel();
            stripe.BackColor = accentColor;
            stripe.Location = new System.Drawing.Point(0, 0);
            stripe.Size = new System.Drawing.Size(w, 3);
            card.Controls.Add(stripe);

            titleLbl.Text = title;
            titleLbl.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            titleLbl.ForeColor = System.Drawing.Color.FromArgb(140, 150, 165);
            titleLbl.Location = new System.Drawing.Point(12, 10);
            titleLbl.AutoSize = true;
            card.Controls.Add(titleLbl);

            valueLbl.Text = value;
            valueLbl.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            valueLbl.ForeColor = accentColor;
            valueLbl.Location = new System.Drawing.Point(12, 30);
            valueLbl.AutoSize = true;
            card.Controls.Add(valueLbl);
        }
    }
}
