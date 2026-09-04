namespace BitswardITSM.Core
{
    partial class CreateSubTaskDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblHeaderSubtitle;

        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.Label lblTaskTitleHeader;
        private System.Windows.Forms.TextBox txtTaskTitle;

        private System.Windows.Forms.Label lblAssigneeHeader;
        private System.Windows.Forms.Panel panelAssigneeCard;
        private System.Windows.Forms.Label lblSelectedAssignee;
        private System.Windows.Forms.Button btnSearchAssignee;
        private System.Windows.Forms.Button btnClearAssignee;

        private System.Windows.Forms.Label lblValidation;

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;

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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.panelBody = new System.Windows.Forms.Panel();
            this.lblValidation = new System.Windows.Forms.Label();
            this.panelAssigneeCard = new System.Windows.Forms.Panel();
            this.btnClearAssignee = new System.Windows.Forms.Button();
            this.btnSearchAssignee = new System.Windows.Forms.Button();
            this.lblSelectedAssignee = new System.Windows.Forms.Label();
            this.lblAssigneeHeader = new System.Windows.Forms.Label();
            this.txtTaskTitle = new System.Windows.Forms.TextBox();
            this.lblTaskTitleHeader = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.panelBody.SuspendLayout();
            this.panelAssigneeCard.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panelHeader.Controls.Add(this.lblHeaderSubtitle);
            this.panelHeader.Controls.Add(this.lblHeaderTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(534, 65);
            this.panelHeader.TabIndex = 0;
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(18, 38);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(262, 15);
            this.lblHeaderSubtitle.TabIndex = 1;
            this.lblHeaderSubtitle.Text = "Split parent ticket into an assigned sub-task item";
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.Location = new System.Drawing.Point(16, 12);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(183, 25);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "➕  Create Sub-Task";
            // 
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.panelBody.Controls.Add(this.lblValidation);
            this.panelBody.Controls.Add(this.panelAssigneeCard);
            this.panelBody.Controls.Add(this.lblAssigneeHeader);
            this.panelBody.Controls.Add(this.txtTaskTitle);
            this.panelBody.Controls.Add(this.lblTaskTitleHeader);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(0, 65);
            this.panelBody.Name = "panelBody";
            this.panelBody.Padding = new System.Windows.Forms.Padding(20, 15, 20, 10);
            this.panelBody.Size = new System.Drawing.Size(534, 255);
            this.panelBody.TabIndex = 1;
            // 
            // lblValidation
            // 
            this.lblValidation.AutoSize = true;
            this.lblValidation.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblValidation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblValidation.Location = new System.Drawing.Point(20, 96);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(0, 15);
            this.lblValidation.TabIndex = 4;
            // 
            // panelAssigneeCard
            // 
            this.panelAssigneeCard.BackColor = System.Drawing.Color.White;
            this.panelAssigneeCard.Controls.Add(this.btnClearAssignee);
            this.panelAssigneeCard.Controls.Add(this.btnSearchAssignee);
            this.panelAssigneeCard.Controls.Add(this.lblSelectedAssignee);
            this.panelAssigneeCard.Location = new System.Drawing.Point(20, 142);
            this.panelAssigneeCard.Name = "panelAssigneeCard";
            this.panelAssigneeCard.Size = new System.Drawing.Size(494, 90);
            this.panelAssigneeCard.TabIndex = 3;
            // 
            // btnClearAssignee
            // 
            this.btnClearAssignee.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.btnClearAssignee.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearAssignee.FlatAppearance.BorderSize = 0;
            this.btnClearAssignee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAssignee.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearAssignee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnClearAssignee.Location = new System.Drawing.Point(175, 48);
            this.btnClearAssignee.Name = "btnClearAssignee";
            this.btnClearAssignee.Size = new System.Drawing.Size(75, 28);
            this.btnClearAssignee.TabIndex = 2;
            this.btnClearAssignee.Text = "✖ Clear";
            this.btnClearAssignee.UseVisualStyleBackColor = false;
            this.btnClearAssignee.Visible = false;
            this.btnClearAssignee.Click += new System.EventHandler(this.BtnClearAssignee_Click);
            // 
            // btnSearchAssignee
            // 
            this.btnSearchAssignee.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSearchAssignee.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchAssignee.FlatAppearance.BorderSize = 0;
            this.btnSearchAssignee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchAssignee.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSearchAssignee.ForeColor = System.Drawing.Color.White;
            this.btnSearchAssignee.Location = new System.Drawing.Point(14, 48);
            this.btnSearchAssignee.Name = "btnSearchAssignee";
            this.btnSearchAssignee.Size = new System.Drawing.Size(150, 28);
            this.btnSearchAssignee.TabIndex = 1;
            this.btnSearchAssignee.Text = "👤 Search & Assign...";
            this.btnSearchAssignee.UseVisualStyleBackColor = false;
            this.btnSearchAssignee.Click += new System.EventHandler(this.BtnSearchAssignee_Click);
            // 
            // lblSelectedAssignee
            // 
            this.lblSelectedAssignee.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblSelectedAssignee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSelectedAssignee.Location = new System.Drawing.Point(12, 12);
            this.lblSelectedAssignee.Name = "lblSelectedAssignee";
            this.lblSelectedAssignee.Size = new System.Drawing.Size(468, 26);
            this.lblSelectedAssignee.TabIndex = 0;
            this.lblSelectedAssignee.Text = "👤 Unassigned (Click button below to select an employee)";
            this.lblSelectedAssignee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAssigneeHeader
            // 
            this.lblAssigneeHeader.AutoSize = true;
            this.lblAssigneeHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblAssigneeHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblAssigneeHeader.Location = new System.Drawing.Point(20, 122);
            this.lblAssigneeHeader.Name = "lblAssigneeHeader";
            this.lblAssigneeHeader.Size = new System.Drawing.Size(130, 15);
            this.lblAssigneeHeader.TabIndex = 2;
            this.lblAssigneeHeader.Text = "TASK ASSIGNEE (OPTIONAL)";
            // 
            // txtTaskTitle
            // 
            this.txtTaskTitle.BackColor = System.Drawing.Color.White;
            this.txtTaskTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTaskTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTaskTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtTaskTitle.Location = new System.Drawing.Point(20, 38);
            this.txtTaskTitle.Multiline = true;
            this.txtTaskTitle.Name = "txtTaskTitle";
            this.txtTaskTitle.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTaskTitle.Size = new System.Drawing.Size(494, 52);
            this.txtTaskTitle.TabIndex = 1;
            this.txtTaskTitle.TextChanged += new System.EventHandler(this.TxtTaskTitle_TextChanged);
            // 
            // lblTaskTitleHeader
            // 
            this.lblTaskTitleHeader.AutoSize = true;
            this.lblTaskTitleHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblTaskTitleHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTaskTitleHeader.Location = new System.Drawing.Point(20, 18);
            this.lblTaskTitleHeader.Name = "lblTaskTitleHeader";
            this.lblTaskTitleHeader.Size = new System.Drawing.Size(185, 15);
            this.lblTaskTitleHeader.TabIndex = 0;
            this.lblTaskTitleHeader.Text = "SUB-TASK TITLE / ACTION ITEM *";
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnCreate);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 320);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(534, 50);
            this.panelBottom.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnCancel.Location = new System.Drawing.Point(274, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreate.FlatAppearance.BorderSize = 0;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCreate.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Location = new System.Drawing.Point(384, 10);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(130, 30);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "➕ Create Task";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.BtnCreate_Click);
            // 
            // CreateSubTaskDialog
            // 
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(534, 370);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateSubTaskDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Split Ticket to Task";
            this.Load += new System.EventHandler(this.CreateSubTaskDialog_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelBody.ResumeLayout(false);
            this.panelBody.PerformLayout();
            this.panelAssigneeCard.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
