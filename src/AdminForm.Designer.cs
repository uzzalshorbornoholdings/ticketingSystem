namespace BitswardITSM.Core
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControlAdmin;
        private System.Windows.Forms.TabPage tabManageUsers;
        private System.Windows.Forms.TabPage tabRegisterUser;
        
        private System.Windows.Forms.DataGridView gridUsers;
        private System.Windows.Forms.Panel panelRoleUpdate;
        private System.Windows.Forms.Label lblSelectedUser;
        private System.Windows.Forms.ComboBox cmbRoleEdit;
        private System.Windows.Forms.Button btnUpdateRole;
        
        private System.Windows.Forms.DataGridView gridUnassociated;
        private System.Windows.Forms.Panel panelRegisterControls;
        private System.Windows.Forms.Label lblSelectedEmpInfo;
        private System.Windows.Forms.TextBox txtNewUsername;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.ComboBox cmbNewRole;
        private System.Windows.Forms.Button btnCreateUser;
        private System.Windows.Forms.Label lblUsernameLabel;
        private System.Windows.Forms.Label lblPasswordLabel;
        private System.Windows.Forms.Label lblRoleLabel;
        
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblAdminTitle;
        private System.Windows.Forms.Label lblStatusMsg;

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
            this.tabControlAdmin = new System.Windows.Forms.TabControl();
            this.tabManageUsers = new System.Windows.Forms.TabPage();
            this.gridUsers = new System.Windows.Forms.DataGridView();
            this.panelRoleUpdate = new System.Windows.Forms.Panel();
            this.btnUpdateRole = new System.Windows.Forms.Button();
            this.cmbRoleEdit = new System.Windows.Forms.ComboBox();
            this.lblSelectedUser = new System.Windows.Forms.Label();
            this.tabRegisterUser = new System.Windows.Forms.TabPage();
            this.gridUnassociated = new System.Windows.Forms.DataGridView();
            this.panelRegisterControls = new System.Windows.Forms.Panel();
            this.lblRoleLabel = new System.Windows.Forms.Label();
            this.lblPasswordLabel = new System.Windows.Forms.Label();
            this.lblUsernameLabel = new System.Windows.Forms.Label();
            this.btnCreateUser = new System.Windows.Forms.Button();
            this.cmbNewRole = new System.Windows.Forms.ComboBox();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.txtNewUsername = new System.Windows.Forms.TextBox();
            this.lblSelectedEmpInfo = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblAdminTitle = new System.Windows.Forms.Label();
            this.lblStatusMsg = new System.Windows.Forms.Label();
            this.tabControlAdmin.SuspendLayout();
            this.tabManageUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            this.panelRoleUpdate.SuspendLayout();
            this.tabRegisterUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnassociated)).BeginInit();
            this.panelRegisterControls.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlAdmin
            // 
            this.tabControlAdmin.Controls.Add(this.tabManageUsers);
            this.tabControlAdmin.Controls.Add(this.tabRegisterUser);
            this.tabControlAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlAdmin.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlAdmin.Location = new System.Drawing.Point(0, 50);
            this.tabControlAdmin.Name = "tabControlAdmin";
            this.tabControlAdmin.SelectedIndex = 0;
            this.tabControlAdmin.Size = new System.Drawing.Size(784, 511);
            this.tabControlAdmin.TabIndex = 0;
            this.tabControlAdmin.SelectedIndexChanged += new System.EventHandler(this.TabControlAdmin_SelectedIndexChanged);
            // 
            // tabManageUsers
            // 
            this.tabManageUsers.Controls.Add(this.gridUsers);
            this.tabManageUsers.Controls.Add(this.panelRoleUpdate);
            this.tabManageUsers.Location = new System.Drawing.Point(4, 26);
            this.tabManageUsers.Name = "tabManageUsers";
            this.tabManageUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabManageUsers.Size = new System.Drawing.Size(776, 481);
            this.tabManageUsers.TabIndex = 0;
            this.tabManageUsers.Text = "👤 Manage Users";
            this.tabManageUsers.UseVisualStyleBackColor = true;
            // 
            // gridUsers
            // 
            this.gridUsers.AllowUserToAddRows = false;
            this.gridUsers.AllowUserToDeleteRows = false;
            this.gridUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridUsers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.gridUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUsers.Location = new System.Drawing.Point(3, 3);
            this.gridUsers.Name = "gridUsers";
            this.gridUsers.ReadOnly = true;
            this.gridUsers.RowHeadersVisible = false;
            this.gridUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridUsers.Size = new System.Drawing.Size(770, 395);
            this.gridUsers.TabIndex = 0;
            this.gridUsers.SelectionChanged += new System.EventHandler(this.GridUsers_SelectionChanged);
            // 
            // panelRoleUpdate
            // 
            this.panelRoleUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.panelRoleUpdate.Controls.Add(this.btnUpdateRole);
            this.panelRoleUpdate.Controls.Add(this.cmbRoleEdit);
            this.panelRoleUpdate.Controls.Add(this.lblSelectedUser);
            this.panelRoleUpdate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRoleUpdate.Location = new System.Drawing.Point(3, 398);
            this.panelRoleUpdate.Name = "panelRoleUpdate";
            this.panelRoleUpdate.Size = new System.Drawing.Size(770, 80);
            this.panelRoleUpdate.TabIndex = 1;
            // 
            // btnUpdateRole
            // 
            this.btnUpdateRole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnUpdateRole.FlatAppearance.BorderSize = 0;
            this.btnUpdateRole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateRole.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateRole.ForeColor = System.Drawing.Color.White;
            this.btnUpdateRole.Location = new System.Drawing.Point(380, 25);
            this.btnUpdateRole.Name = "btnUpdateRole";
            this.btnUpdateRole.Size = new System.Drawing.Size(120, 30);
            this.btnUpdateRole.TabIndex = 2;
            this.btnUpdateRole.Text = "Modify Role";
            this.btnUpdateRole.UseVisualStyleBackColor = false;
            this.btnUpdateRole.Click += new System.EventHandler(this.BtnUpdateRole_Click);
            // 
            // cmbRoleEdit
            // 
            this.cmbRoleEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.cmbRoleEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRoleEdit.ForeColor = System.Drawing.Color.White;
            this.cmbRoleEdit.FormattingEnabled = true;
            this.cmbRoleEdit.Items.AddRange(new object[] {
            "Admin",
            "Manager",
            "Agent",
            "User"});
            this.cmbRoleEdit.Location = new System.Drawing.Point(230, 28);
            this.cmbRoleEdit.Name = "cmbRoleEdit";
            this.cmbRoleEdit.Size = new System.Drawing.Size(130, 25);
            this.cmbRoleEdit.TabIndex = 1;
            // 
            // lblSelectedUser
            // 
            this.lblSelectedUser.AutoSize = true;
            this.lblSelectedUser.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedUser.ForeColor = System.Drawing.Color.White;
            this.lblSelectedUser.Location = new System.Drawing.Point(20, 31);
            this.lblSelectedUser.Name = "lblSelectedUser";
            this.lblSelectedUser.Size = new System.Drawing.Size(161, 17);
            this.lblSelectedUser.TabIndex = 0;
            this.lblSelectedUser.Text = "Select user to modify role...";
            // 
            // tabRegisterUser
            // 
            this.tabRegisterUser.Controls.Add(this.gridUnassociated);
            this.tabRegisterUser.Controls.Add(this.panelRegisterControls);
            this.tabRegisterUser.Location = new System.Drawing.Point(4, 26);
            this.tabRegisterUser.Name = "tabRegisterUser";
            this.tabRegisterUser.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegisterUser.Size = new System.Drawing.Size(776, 481);
            this.tabRegisterUser.TabIndex = 1;
            this.tabRegisterUser.Text = "➕ Provision Logic Account";
            this.tabRegisterUser.UseVisualStyleBackColor = true;
            // 
            // gridUnassociated
            // 
            this.gridUnassociated.AllowUserToAddRows = false;
            this.gridUnassociated.AllowUserToDeleteRows = false;
            this.gridUnassociated.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridUnassociated.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.gridUnassociated.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUnassociated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUnassociated.Location = new System.Drawing.Point(3, 3);
            this.gridUnassociated.Name = "gridUnassociated";
            this.gridUnassociated.ReadOnly = true;
            this.gridUnassociated.RowHeadersVisible = false;
            this.gridUnassociated.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridUnassociated.Size = new System.Drawing.Size(770, 325);
            this.gridUnassociated.TabIndex = 0;
            this.gridUnassociated.SelectionChanged += new System.EventHandler(this.GridUnassociated_SelectionChanged);
            // 
            // panelRegisterControls
            // 
            this.panelRegisterControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.panelRegisterControls.Controls.Add(this.lblRoleLabel);
            this.panelRegisterControls.Controls.Add(this.lblPasswordLabel);
            this.panelRegisterControls.Controls.Add(this.lblUsernameLabel);
            this.panelRegisterControls.Controls.Add(this.btnCreateUser);
            this.panelRegisterControls.Controls.Add(this.cmbNewRole);
            this.panelRegisterControls.Controls.Add(this.txtNewPassword);
            this.panelRegisterControls.Controls.Add(this.txtNewUsername);
            this.panelRegisterControls.Controls.Add(this.lblSelectedEmpInfo);
            this.panelRegisterControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRegisterControls.Location = new System.Drawing.Point(3, 328);
            this.panelRegisterControls.Name = "panelRegisterControls";
            this.panelRegisterControls.Size = new System.Drawing.Size(770, 150);
            this.panelRegisterControls.TabIndex = 1;
            // 
            // lblRoleLabel
            // 
            this.lblRoleLabel.AutoSize = true;
            this.lblRoleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblRoleLabel.Location = new System.Drawing.Point(400, 60);
            this.lblRoleLabel.Name = "lblRoleLabel";
            this.lblRoleLabel.Size = new System.Drawing.Size(37, 17);
            this.lblRoleLabel.TabIndex = 7;
            this.lblRoleLabel.Text = "Role:";
            // 
            // lblPasswordLabel
            // 
            this.lblPasswordLabel.AutoSize = true;
            this.lblPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblPasswordLabel.Location = new System.Drawing.Point(210, 60);
            this.lblPasswordLabel.Name = "lblPasswordLabel";
            this.lblPasswordLabel.Size = new System.Drawing.Size(69, 17);
            this.lblPasswordLabel.TabIndex = 6;
            this.lblPasswordLabel.Text = "Password:";
            // 
            // lblUsernameLabel
            // 
            this.lblUsernameLabel.AutoSize = true;
            this.lblUsernameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblUsernameLabel.Location = new System.Drawing.Point(20, 60);
            this.lblUsernameLabel.Name = "lblUsernameLabel";
            this.lblUsernameLabel.Size = new System.Drawing.Size(72, 17);
            this.lblUsernameLabel.TabIndex = 5;
            this.lblUsernameLabel.Text = "Username:";
            // 
            // btnCreateUser
            // 
            this.btnCreateUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCreateUser.FlatAppearance.BorderSize = 0;
            this.btnCreateUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateUser.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateUser.ForeColor = System.Drawing.Color.White;
            this.btnCreateUser.Location = new System.Drawing.Point(580, 77);
            this.btnCreateUser.Name = "btnCreateUser";
            this.btnCreateUser.Size = new System.Drawing.Size(160, 30);
            this.btnCreateUser.TabIndex = 4;
            this.btnCreateUser.Text = "Generate Login Account";
            this.btnCreateUser.UseVisualStyleBackColor = false;
            this.btnCreateUser.Click += new System.EventHandler(this.BtnCreateUser_Click);
            // 
            // cmbNewRole
            // 
            this.cmbNewRole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.cmbNewRole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNewRole.ForeColor = System.Drawing.Color.White;
            this.cmbNewRole.FormattingEnabled = true;
            this.cmbNewRole.Items.AddRange(new object[] {
            "Admin",
            "Manager",
            "Agent",
            "User"});
            this.cmbNewRole.Location = new System.Drawing.Point(403, 80);
            this.cmbNewRole.Name = "cmbNewRole";
            this.cmbNewRole.Size = new System.Drawing.Size(140, 25);
            this.cmbNewRole.TabIndex = 3;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.ForeColor = System.Drawing.Color.White;
            this.txtNewPassword.Location = new System.Drawing.Point(213, 80);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(160, 25);
            this.txtNewPassword.TabIndex = 2;
            this.txtNewPassword.UseSystemPasswordChar = true;
            // 
            // txtNewUsername
            // 
            this.txtNewUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtNewUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewUsername.ForeColor = System.Drawing.Color.White;
            this.txtNewUsername.Location = new System.Drawing.Point(23, 80);
            this.txtNewUsername.Name = "txtNewUsername";
            this.txtNewUsername.Size = new System.Drawing.Size(160, 25);
            this.txtNewUsername.TabIndex = 1;
            // 
            // lblSelectedEmpInfo
            // 
            this.lblSelectedEmpInfo.AutoSize = true;
            this.lblSelectedEmpInfo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedEmpInfo.ForeColor = System.Drawing.Color.White;
            this.lblSelectedEmpInfo.Location = new System.Drawing.Point(20, 20);
            this.lblSelectedEmpInfo.Name = "lblSelectedEmpInfo";
            this.lblSelectedEmpInfo.Size = new System.Drawing.Size(252, 17);
            this.lblSelectedEmpInfo.TabIndex = 0;
            this.lblSelectedEmpInfo.Text = "Select an employee from the list above...";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panelTop.Controls.Add(this.lblAdminTitle);
            this.panelTop.Controls.Add(this.lblStatusMsg);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(784, 50);
            this.panelTop.TabIndex = 1;
            // 
            // lblAdminTitle
            // 
            this.lblAdminTitle.AutoSize = true;
            this.lblAdminTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdminTitle.ForeColor = System.Drawing.Color.White;
            this.lblAdminTitle.Location = new System.Drawing.Point(12, 12);
            this.lblAdminTitle.Name = "lblAdminTitle";
            this.lblAdminTitle.Size = new System.Drawing.Size(236, 25);
            this.lblAdminTitle.TabIndex = 0;
            this.lblAdminTitle.Text = "🛡️ ITSM Administration Console";
            // 
            // lblStatusMsg
            // 
            this.lblStatusMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusMsg.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblStatusMsg.Location = new System.Drawing.Point(400, 16);
            this.lblStatusMsg.Name = "lblStatusMsg";
            this.lblStatusMsg.Size = new System.Drawing.Size(372, 23);
            this.lblStatusMsg.TabIndex = 1;
            this.lblStatusMsg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tabControlAdmin);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ITSM System Admin Console";
            this.Load += new System.EventHandler(this.AdminForm_Load);
            this.tabControlAdmin.ResumeLayout(false);
            this.tabManageUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            this.panelRoleUpdate.ResumeLayout(false);
            this.panelRoleUpdate.PerformLayout();
            this.tabRegisterUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUnassociated)).EndInit();
            this.panelRegisterControls.ResumeLayout(false);
            this.panelRegisterControls.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
