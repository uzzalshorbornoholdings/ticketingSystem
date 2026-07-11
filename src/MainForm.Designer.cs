namespace BitswardITSM.Core
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnNavTickets;
        private System.Windows.Forms.Button btnNavTasks;
        private System.Windows.Forms.Button btnNavChanges;
        private System.Windows.Forms.Button btnNavAdmin;
        private System.Windows.Forms.Button btnNavLogout;
        private System.Windows.Forms.Button btnNewTicket;
        
        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblUserContext;
        
        private System.Windows.Forms.Panel panelMainContent;
        private System.Windows.Forms.TabControl tabControlQueues;
        private System.Windows.Forms.TabPage tabIncidents;
        private System.Windows.Forms.TabPage tabServiceRequests;
        private System.Windows.Forms.TabPage tabChanges;
        
        private System.Windows.Forms.DataGridView gridIncidents;
        private System.Windows.Forms.DataGridView gridServiceRequests;
        private System.Windows.Forms.DataGridView gridChanges;
        
        private System.Windows.Forms.Panel panelTicketDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblDetailDesc;
        private System.Windows.Forms.Label lblDetailPriority;
        private System.Windows.Forms.Label lblDetailStatus;
        private System.Windows.Forms.Label lblDetailSla;
        private System.Windows.Forms.Label lblDetailAssignee;
        private System.Windows.Forms.Button btnAssignToMe;
        private System.Windows.Forms.Button btnChangeStatus;
        private System.Windows.Forms.ComboBox cmbStatusEdit;
        
        private System.Windows.Forms.Label lblLockIndicator;
        private System.Windows.Forms.RichTextBox txtThreadHistory;
        private System.Windows.Forms.TextBox txtThreadInput;
        private System.Windows.Forms.Button btnSendThread;
        private System.Windows.Forms.Button btnCreateSubTask;
        
        private System.Windows.Forms.SplitContainer splitContainerDashboard;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnNavTickets = new System.Windows.Forms.Button();
            this.btnNavTasks = new System.Windows.Forms.Button();
            this.btnNavChanges = new System.Windows.Forms.Button();
            this.btnNavAdmin = new System.Windows.Forms.Button();
            this.btnNavLogout = new System.Windows.Forms.Button();
            this.btnNewTicket = new System.Windows.Forms.Button();
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblUserContext = new System.Windows.Forms.Label();
            this.panelMainContent = new System.Windows.Forms.Panel();
            this.splitContainerDashboard = new System.Windows.Forms.SplitContainer();
            this.tabControlQueues = new System.Windows.Forms.TabControl();
            this.tabIncidents = new System.Windows.Forms.TabPage();
            this.gridIncidents = new System.Windows.Forms.DataGridView();
            this.tabServiceRequests = new System.Windows.Forms.TabPage();
            this.gridServiceRequests = new System.Windows.Forms.DataGridView();
            this.tabChanges = new System.Windows.Forms.TabPage();
            this.gridChanges = new System.Windows.Forms.DataGridView();
            this.panelTicketDetails = new System.Windows.Forms.Panel();
            this.btnCreateSubTask = new System.Windows.Forms.Button();
            this.btnSendThread = new System.Windows.Forms.Button();
            this.txtThreadInput = new System.Windows.Forms.TextBox();
            this.txtThreadHistory = new System.Windows.Forms.RichTextBox();
            this.lblLockIndicator = new System.Windows.Forms.Label();
            this.cmbStatusEdit = new System.Windows.Forms.ComboBox();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.btnAssignToMe = new System.Windows.Forms.Button();
            this.lblDetailAssignee = new System.Windows.Forms.Label();
            this.lblDetailSla = new System.Windows.Forms.Label();
            this.lblDetailStatus = new System.Windows.Forms.Label();
            this.lblDetailPriority = new System.Windows.Forms.Label();
            this.lblDetailDesc = new System.Windows.Forms.Label();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.panelSidebar.SuspendLayout();
            this.panelTopHeader.SuspendLayout();
            this.panelMainContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDashboard)).BeginInit();
            this.splitContainerDashboard.Panel1.SuspendLayout();
            this.splitContainerDashboard.Panel2.SuspendLayout();
            this.splitContainerDashboard.SuspendLayout();
            this.tabControlQueues.SuspendLayout();
            this.tabIncidents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridIncidents)).BeginInit();
            this.tabServiceRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridServiceRequests)).BeginInit();
            this.tabChanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridChanges)).BeginInit();
            this.panelTicketDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.panelSidebar.Controls.Add(this.btnNavTickets);
            this.panelSidebar.Controls.Add(this.btnNavTasks);
            this.panelSidebar.Controls.Add(this.btnNavChanges);
            this.panelSidebar.Controls.Add(this.btnNavAdmin);
            this.panelSidebar.Controls.Add(this.btnNewTicket);
            this.panelSidebar.Controls.Add(this.btnNavLogout);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 60);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(200, 621);
            this.panelSidebar.TabIndex = 0;
            // 
            // btnNavTickets
            // 
            this.btnNavTickets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnNavTickets.FlatAppearance.BorderSize = 0;
            this.btnNavTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavTickets.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavTickets.ForeColor = System.Drawing.Color.White;
            this.btnNavTickets.Location = new System.Drawing.Point(10, 20);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(180, 45);
            this.btnNavTickets.TabIndex = 0;
            this.btnNavTickets.Text = "🎟️ ITSM Tickets";
            this.btnNavTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavTickets.UseVisualStyleBackColor = false;
            this.btnNavTickets.Click += new System.EventHandler(this.BtnNavTickets_Click);
            // 
            // btnNavTasks
            // 
            this.btnNavTasks.FlatAppearance.BorderSize = 0;
            this.btnNavTasks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavTasks.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavTasks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(207)))), ((int)(((byte)(214)))));
            this.btnNavTasks.Location = new System.Drawing.Point(10, 80);
            this.btnNavTasks.Name = "btnNavTasks";
            this.btnNavTasks.Size = new System.Drawing.Size(180, 45);
            this.btnNavTasks.TabIndex = 1;
            this.btnNavTasks.Text = "📋 Tasks List";
            this.btnNavTasks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavTasks.UseVisualStyleBackColor = true;
            this.btnNavTasks.Click += new System.EventHandler(this.BtnNavTasks_Click);
            // 
            // btnNavChanges
            // 
            this.btnNavChanges.FlatAppearance.BorderSize = 0;
            this.btnNavChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavChanges.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavChanges.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(207)))), ((int)(((byte)(214)))));
            this.btnNavChanges.Location = new System.Drawing.Point(10, 140);
            this.btnNavChanges.Name = "btnNavChanges";
            this.btnNavChanges.Size = new System.Drawing.Size(180, 45);
            this.btnNavChanges.TabIndex = 2;
            this.btnNavChanges.Text = "⚙️ CR Changes";
            this.btnNavChanges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavChanges.UseVisualStyleBackColor = true;
            this.btnNavChanges.Click += new System.EventHandler(this.BtnNavChanges_Click);
            // 
            // btnNavAdmin
            // 
            this.btnNavAdmin.FlatAppearance.BorderSize = 0;
            this.btnNavAdmin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavAdmin.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavAdmin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(207)))), ((int)(((byte)(214)))));
            this.btnNavAdmin.Location = new System.Drawing.Point(10, 200);
            this.btnNavAdmin.Name = "btnNavAdmin";
            this.btnNavAdmin.Size = new System.Drawing.Size(180, 45);
            this.btnNavAdmin.TabIndex = 3;
            this.btnNavAdmin.Text = "🛡️ Admin Panel";
            this.btnNavAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavAdmin.UseVisualStyleBackColor = true;
            this.btnNavAdmin.Click += new System.EventHandler(this.BtnNavAdmin_Click);
            // 
            // btnNewTicket
            // 
            this.btnNewTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnNewTicket.FlatAppearance.BorderSize = 0;
            this.btnNewTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewTicket.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewTicket.ForeColor = System.Drawing.Color.White;
            this.btnNewTicket.Location = new System.Drawing.Point(10, 260);
            this.btnNewTicket.Name = "btnNewTicket";
            this.btnNewTicket.Size = new System.Drawing.Size(180, 45);
            this.btnNewTicket.TabIndex = 5;
            this.btnNewTicket.Text = "➕ Report Issue";
            this.btnNewTicket.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewTicket.UseVisualStyleBackColor = false;
            this.btnNewTicket.Click += new System.EventHandler(this.BtnNewTicket_Click);
            // 
            // btnNavLogout
            // 
            this.btnNavLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnNavLogout.FlatAppearance.BorderSize = 0;
            this.btnNavLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnNavLogout.Location = new System.Drawing.Point(0, 576);
            this.btnNavLogout.Name = "btnNavLogout";
            this.btnNavLogout.Size = new System.Drawing.Size(200, 45);
            this.btnNavLogout.TabIndex = 4;
            this.btnNavLogout.Text = "🚪 Sign Out";
            this.btnNavLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavLogout.UseVisualStyleBackColor = true;
            this.btnNavLogout.Click += new System.EventHandler(this.BtnNavLogout_Click);
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panelTopHeader.Controls.Add(this.lblHeaderTitle);
            this.panelTopHeader.Controls.Add(this.lblUserContext);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(1184, 60);
            this.panelTopHeader.TabIndex = 1;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.Location = new System.Drawing.Point(12, 15);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(298, 30);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "ITSM Operational Dashboard";
            // 
            // lblUserContext
            // 
            this.lblUserContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserContext.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserContext.ForeColor = System.Drawing.Color.White;
            this.lblUserContext.Location = new System.Drawing.Point(744, 20);
            this.lblUserContext.Name = "lblUserContext";
            this.lblUserContext.Size = new System.Drawing.Size(428, 23);
            this.lblUserContext.TabIndex = 1;
            this.lblUserContext.Text = "User: Guest";
            this.lblUserContext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelMainContent
            // 
            this.panelMainContent.Controls.Add(this.splitContainerDashboard);
            this.panelMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContent.Location = new System.Drawing.Point(200, 60);
            this.panelMainContent.Name = "panelMainContent";
            this.panelMainContent.Size = new System.Drawing.Size(984, 621);
            this.panelMainContent.TabIndex = 2;
            // 
            // splitContainerDashboard
            // 
            this.splitContainerDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.splitContainerDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDashboard.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDashboard.Name = "splitContainerDashboard";
            // 
            // splitContainerDashboard.Panel1
            // 
            this.splitContainerDashboard.Panel1.Controls.Add(this.tabControlQueues);
            this.splitContainerDashboard.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainerDashboard.Panel2
            // 
            this.splitContainerDashboard.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.splitContainerDashboard.Panel2.Controls.Add(this.panelTicketDetails);
            this.splitContainerDashboard.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainerDashboard.Size = new System.Drawing.Size(984, 621);
            this.splitContainerDashboard.SplitterDistance = 520;
            this.splitContainerDashboard.TabIndex = 0;
            // 
            // tabControlQueues
            // 
            this.tabControlQueues.Controls.Add(this.tabIncidents);
            this.tabControlQueues.Controls.Add(this.tabServiceRequests);
            this.tabControlQueues.Controls.Add(this.tabChanges);
            this.tabControlQueues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlQueues.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlQueues.Location = new System.Drawing.Point(10, 10);
            this.tabControlQueues.Name = "tabControlQueues";
            this.tabControlQueues.SelectedIndex = 0;
            this.tabControlQueues.Size = new System.Drawing.Size(500, 601);
            this.tabControlQueues.TabIndex = 0;
            this.tabControlQueues.SelectedIndexChanged += new System.EventHandler(this.TabControlQueues_SelectedIndexChanged);
            // 
            // tabIncidents
            // 
            this.tabIncidents.Controls.Add(this.gridIncidents);
            this.tabIncidents.Location = new System.Drawing.Point(4, 26);
            this.tabIncidents.Name = "tabIncidents";
            this.tabIncidents.Padding = new System.Windows.Forms.Padding(3);
            this.tabIncidents.Size = new System.Drawing.Size(492, 571);
            this.tabIncidents.TabIndex = 0;
            this.tabIncidents.Text = "‼️ Incidents (INC)";
            this.tabIncidents.UseVisualStyleBackColor = true;
            // 
            // gridIncidents
            // 
            this.gridIncidents.AllowUserToAddRows = false;
            this.gridIncidents.AllowUserToDeleteRows = false;
            this.gridIncidents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridIncidents.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.gridIncidents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridIncidents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridIncidents.Location = new System.Drawing.Point(3, 3);
            this.gridIncidents.Name = "gridIncidents";
            this.gridIncidents.ReadOnly = true;
            this.gridIncidents.RowHeadersVisible = false;
            this.gridIncidents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridIncidents.Size = new System.Drawing.Size(486, 565);
            this.gridIncidents.TabIndex = 0;
            this.gridIncidents.SelectionChanged += new System.EventHandler(this.GridIncidents_SelectionChanged);
            this.gridIncidents.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Grid_CellFormatting);
            // 
            // tabServiceRequests
            // 
            this.tabServiceRequests.Controls.Add(this.gridServiceRequests);
            this.tabServiceRequests.Location = new System.Drawing.Point(4, 26);
            this.tabServiceRequests.Name = "tabServiceRequests";
            this.tabServiceRequests.Padding = new System.Windows.Forms.Padding(3);
            this.tabServiceRequests.Size = new System.Drawing.Size(492, 571);
            this.tabServiceRequests.TabIndex = 1;
            this.tabServiceRequests.Text = "🙋 Requests (SR)";
            this.tabServiceRequests.UseVisualStyleBackColor = true;
            // 
            // gridServiceRequests
            // 
            this.gridServiceRequests.AllowUserToAddRows = false;
            this.gridServiceRequests.AllowUserToDeleteRows = false;
            this.gridServiceRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridServiceRequests.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.gridServiceRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridServiceRequests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridServiceRequests.Location = new System.Drawing.Point(3, 3);
            this.gridServiceRequests.Name = "gridServiceRequests";
            this.gridServiceRequests.ReadOnly = true;
            this.gridServiceRequests.RowHeadersVisible = false;
            this.gridServiceRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridServiceRequests.Size = new System.Drawing.Size(486, 565);
            this.gridServiceRequests.TabIndex = 0;
            this.gridServiceRequests.SelectionChanged += new System.EventHandler(this.GridServiceRequests_SelectionChanged);
            this.gridServiceRequests.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Grid_CellFormatting);
            // 
            // tabChanges
            // 
            this.tabChanges.Controls.Add(this.gridChanges);
            this.tabChanges.Location = new System.Drawing.Point(4, 26);
            this.tabChanges.Name = "tabChanges";
            this.tabChanges.Padding = new System.Windows.Forms.Padding(3);
            this.tabChanges.Size = new System.Drawing.Size(492, 571);
            this.tabChanges.TabIndex = 2;
            this.tabChanges.Text = "⚙️ Changes (CR)";
            this.tabChanges.UseVisualStyleBackColor = true;
            // 
            // gridChanges
            // 
            this.gridChanges.AllowUserToAddRows = false;
            this.gridChanges.AllowUserToDeleteRows = false;
            this.gridChanges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridChanges.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.gridChanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridChanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridChanges.Location = new System.Drawing.Point(3, 3);
            this.gridChanges.Name = "gridChanges";
            this.gridChanges.ReadOnly = true;
            this.gridChanges.RowHeadersVisible = false;
            this.gridChanges.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridChanges.Size = new System.Drawing.Size(486, 565);
            this.gridChanges.TabIndex = 0;
            this.gridChanges.SelectionChanged += new System.EventHandler(this.GridChanges_SelectionChanged);
            this.gridChanges.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Grid_CellFormatting);
            // 
            // panelTicketDetails
            // 
            this.panelTicketDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.panelTicketDetails.Controls.Add(this.btnCreateSubTask);
            this.panelTicketDetails.Controls.Add(this.btnSendThread);
            this.panelTicketDetails.Controls.Add(this.txtThreadInput);
            this.panelTicketDetails.Controls.Add(this.txtThreadHistory);
            this.panelTicketDetails.Controls.Add(this.lblLockIndicator);
            this.panelTicketDetails.Controls.Add(this.cmbStatusEdit);
            this.panelTicketDetails.Controls.Add(this.btnChangeStatus);
            this.panelTicketDetails.Controls.Add(this.btnAssignToMe);
            this.panelTicketDetails.Controls.Add(this.lblDetailAssignee);
            this.panelTicketDetails.Controls.Add(this.lblDetailSla);
            this.panelTicketDetails.Controls.Add(this.lblDetailStatus);
            this.panelTicketDetails.Controls.Add(this.lblDetailPriority);
            this.panelTicketDetails.Controls.Add(this.lblDetailDesc);
            this.panelTicketDetails.Controls.Add(this.lblDetailTitle);
            this.panelTicketDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTicketDetails.Location = new System.Drawing.Point(10, 10);
            this.panelTicketDetails.Name = "panelTicketDetails";
            this.panelTicketDetails.Size = new System.Drawing.Size(440, 601);
            this.panelTicketDetails.TabIndex = 0;
            // 
            // btnCreateSubTask
            // 
            this.btnCreateSubTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateSubTask.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnCreateSubTask.FlatAppearance.BorderSize = 0;
            this.btnCreateSubTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateSubTask.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateSubTask.ForeColor = System.Drawing.Color.White;
            this.btnCreateSubTask.Location = new System.Drawing.Point(313, 563);
            this.btnCreateSubTask.Name = "btnCreateSubTask";
            this.btnCreateSubTask.Size = new System.Drawing.Size(115, 30);
            this.btnCreateSubTask.TabIndex = 13;
            this.btnCreateSubTask.Text = "➕ Split Task";
            this.btnCreateSubTask.UseVisualStyleBackColor = false;
            this.btnCreateSubTask.Click += new System.EventHandler(this.BtnCreateSubTask_Click);
            // 
            // btnSendThread
            // 
            this.btnSendThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSendThread.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSendThread.FlatAppearance.BorderSize = 0;
            this.btnSendThread.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendThread.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendThread.ForeColor = System.Drawing.Color.White;
            this.btnSendThread.Location = new System.Drawing.Point(232, 563);
            this.btnSendThread.Name = "btnSendThread";
            this.btnSendThread.Size = new System.Drawing.Size(75, 30);
            this.btnSendThread.TabIndex = 12;
            this.btnSendThread.Text = "Post Comment";
            this.btnSendThread.UseVisualStyleBackColor = false;
            this.btnSendThread.Click += new System.EventHandler(this.BtnSendThread_Click);
            // 
            // txtThreadInput
            // 
            this.txtThreadInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreadInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtThreadInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtThreadInput.ForeColor = System.Drawing.Color.White;
            this.txtThreadInput.Location = new System.Drawing.Point(15, 565);
            this.txtThreadInput.Name = "txtThreadInput";
            this.txtThreadInput.Size = new System.Drawing.Size(211, 25);
            this.txtThreadInput.TabIndex = 11;
            // 
            // txtThreadHistory
            // 
            this.txtThreadHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreadHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtThreadHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtThreadHistory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThreadHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtThreadHistory.Location = new System.Drawing.Point(15, 310);
            this.txtThreadHistory.Name = "txtThreadHistory";
            this.txtThreadHistory.ReadOnly = true;
            this.txtThreadHistory.Size = new System.Drawing.Size(413, 237);
            this.txtThreadHistory.TabIndex = 10;
            this.txtThreadHistory.Text = "";
            // 
            // lblLockIndicator
            // 
            this.lblLockIndicator.AutoSize = true;
            this.lblLockIndicator.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockIndicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.lblLockIndicator.Location = new System.Drawing.Point(12, 280);
            this.lblLockIndicator.Name = "lblLockIndicator";
            this.lblLockIndicator.Size = new System.Drawing.Size(0, 15);
            this.lblLockIndicator.TabIndex = 9;
            // 
            // cmbStatusEdit
            // 
            this.cmbStatusEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.cmbStatusEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStatusEdit.ForeColor = System.Drawing.Color.White;
            this.cmbStatusEdit.FormattingEnabled = true;
            this.cmbStatusEdit.Items.AddRange(new object[] {
            "Open",
            "Triage",
            "Assigned",
            "In Progress",
            "Resolved",
            "Closed"});
            this.cmbStatusEdit.Location = new System.Drawing.Point(177, 243);
            this.cmbStatusEdit.Name = "cmbStatusEdit";
            this.cmbStatusEdit.Size = new System.Drawing.Size(120, 25);
            this.cmbStatusEdit.TabIndex = 8;
            // 
            // btnChangeStatus
            // 
            this.btnChangeStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnChangeStatus.FlatAppearance.BorderSize = 0;
            this.btnChangeStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeStatus.ForeColor = System.Drawing.Color.White;
            this.btnChangeStatus.Location = new System.Drawing.Point(303, 240);
            this.btnChangeStatus.Name = "btnChangeStatus";
            this.btnChangeStatus.Size = new System.Drawing.Size(110, 30);
            this.btnChangeStatus.TabIndex = 7;
            this.btnChangeStatus.Text = "Update Status";
            this.btnChangeStatus.UseVisualStyleBackColor = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.BtnChangeStatus_Click);
            // 
            // btnAssignToMe
            // 
            this.btnAssignToMe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnAssignToMe.FlatAppearance.BorderSize = 0;
            this.btnAssignToMe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAssignToMe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAssignToMe.ForeColor = System.Drawing.Color.White;
            this.btnAssignToMe.Location = new System.Drawing.Point(15, 240);
            this.btnAssignToMe.Name = "btnAssignToMe";
            this.btnAssignToMe.Size = new System.Drawing.Size(110, 30);
            this.btnAssignToMe.TabIndex = 6;
            this.btnAssignToMe.Text = "Claim Ticket";
            this.btnAssignToMe.UseVisualStyleBackColor = false;
            this.btnAssignToMe.Click += new System.EventHandler(this.BtnAssignToMe_Click);
            // 
            // lblDetailAssignee
            // 
            this.lblDetailAssignee.AutoSize = true;
            this.lblDetailAssignee.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailAssignee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblDetailAssignee.Location = new System.Drawing.Point(12, 195);
            this.lblDetailAssignee.Name = "lblDetailAssignee";
            this.lblDetailAssignee.Size = new System.Drawing.Size(63, 17);
            this.lblDetailAssignee.TabIndex = 5;
            this.lblDetailAssignee.Text = "Assignee: ";
            // 
            // lblDetailSla
            // 
            this.lblDetailSla.AutoSize = true;
            this.lblDetailSla.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailSla.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblDetailSla.Location = new System.Drawing.Point(12, 165);
            this.lblDetailSla.Name = "lblDetailSla";
            this.lblDetailSla.Size = new System.Drawing.Size(92, 17);
            this.lblDetailSla.TabIndex = 4;
            this.lblDetailSla.Text = "SLA Deadline: ";
            // 
            // lblDetailStatus
            // 
            this.lblDetailStatus.AutoSize = true;
            this.lblDetailStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblDetailStatus.Location = new System.Drawing.Point(12, 135);
            this.lblDetailStatus.Name = "lblDetailStatus";
            this.lblDetailStatus.Size = new System.Drawing.Size(46, 17);
            this.lblDetailStatus.TabIndex = 3;
            this.lblDetailStatus.Text = "Status: ";
            // 
            // lblDetailPriority
            // 
            this.lblDetailPriority.AutoSize = true;
            this.lblDetailPriority.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailPriority.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblDetailPriority.Location = new System.Drawing.Point(12, 105);
            this.lblDetailPriority.Name = "lblDetailPriority";
            this.lblDetailPriority.Size = new System.Drawing.Size(51, 17);
            this.lblDetailPriority.TabIndex = 2;
            this.lblDetailPriority.Text = "Priority: ";
            // 
            // lblDetailDesc
            // 
            this.lblDetailDesc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblDetailDesc.Location = new System.Drawing.Point(12, 45);
            this.lblDetailDesc.Name = "lblDetailDesc";
            this.lblDetailDesc.Size = new System.Drawing.Size(413, 50);
            this.lblDetailDesc.TabIndex = 1;
            this.lblDetailDesc.Text = "Description details...";
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailTitle.ForeColor = System.Drawing.Color.White;
            this.lblDetailTitle.Location = new System.Drawing.Point(11, 12);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Size = new System.Drawing.Size(162, 21);
            this.lblDetailTitle.TabIndex = 0;
            this.lblDetailTitle.Text = "Select a ticket to view";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1184, 681);
            this.Controls.Add(this.panelMainContent);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelTopHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitsward ITSM Core Operations Platform";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelMainContent.ResumeLayout(false);
            this.splitContainerDashboard.Panel1.ResumeLayout(false);
            this.splitContainerDashboard.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDashboard)).EndInit();
            this.splitContainerDashboard.ResumeLayout(false);
            this.tabControlQueues.ResumeLayout(false);
            this.tabIncidents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridIncidents)).EndInit();
            this.tabServiceRequests.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridServiceRequests)).EndInit();
            this.tabChanges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridChanges)).EndInit();
            this.panelTicketDetails.ResumeLayout(false);
            this.panelTicketDetails.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
