namespace BitswardITSM.Core
{
    partial class AttachmentViewerForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblSubHeader;
        private System.Windows.Forms.DataGridView gridAttachments;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Label lblPreviewDetails;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Button btnPasteScreenshot;
        private System.Windows.Forms.Button btnDelete;
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
            this.lblSubHeader = new System.Windows.Forms.Label();
            this.gridAttachments = new System.Windows.Forms.DataGridView();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewDetails = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnPasteScreenshot = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.gridAttachments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Ticket Attachments & Screenshots";
            this.Size = new System.Drawing.Size(900, 560);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.Load += new System.EventHandler(this.AttachmentViewerForm_Load);

            // Header Panel
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 56;
            this.panelHeader.Controls.Add(this.lblHeader);
            this.panelHeader.Controls.Add(this.lblSubHeader);

            this.lblHeader.Text = "📎  Ticket Attachments & Diagnostic Files";
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(12, 6);
            this.lblHeader.Size = new System.Drawing.Size(550, 24);

            this.lblSubHeader.Text = "Ticket #0 — Issue";
            this.lblSubHeader.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubHeader.ForeColor = System.Drawing.Color.FromArgb(220, 235, 250);
            this.lblSubHeader.Location = new System.Drawing.Point(14, 30);
            this.lblSubHeader.Size = new System.Drawing.Size(860, 20);

            // Grid Attachments
            this.gridAttachments.Location = new System.Drawing.Point(12, 66);
            this.gridAttachments.Size = new System.Drawing.Size(520, 390);
            this.gridAttachments.ReadOnly = true;
            this.gridAttachments.AllowUserToAddRows = false;
            this.gridAttachments.AllowUserToDeleteRows = false;
            this.gridAttachments.RowHeadersVisible = false;
            this.gridAttachments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAttachments.MultiSelect = false;
            this.gridAttachments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridAttachments.BackgroundColor = System.Drawing.Color.FromArgb(28, 32, 40);
            this.gridAttachments.SelectionChanged += new System.EventHandler(this.GridAttachments_SelectionChanged);
            this.gridAttachments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridAttachments_CellDoubleClick);
            this.gridAttachments.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridAttachments_DataBindingComplete);

            // Preview Panel
            this.panelPreview.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelPreview.Location = new System.Drawing.Point(542, 66);
            this.panelPreview.Size = new System.Drawing.Size(330, 390);
            this.panelPreview.Controls.Add(this.picPreview);
            this.panelPreview.Controls.Add(this.lblPreviewDetails);

            // Picture Box
            this.picPreview.BackColor = System.Drawing.Color.FromArgb(20, 24, 30);
            this.picPreview.Location = new System.Drawing.Point(10, 10);
            this.picPreview.Size = new System.Drawing.Size(310, 270);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Preview Details Label
            this.lblPreviewDetails.Text = "Select an attachment to preview...";
            this.lblPreviewDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreviewDetails.ForeColor = System.Drawing.Color.FromArgb(200, 207, 214);
            this.lblPreviewDetails.Location = new System.Drawing.Point(10, 290);
            this.lblPreviewDetails.Size = new System.Drawing.Size(310, 90);

            // Bottom Panel
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(37, 43, 54);
            this.panelBottom.Location = new System.Drawing.Point(12, 465);
            this.panelBottom.Size = new System.Drawing.Size(860, 45);
            this.panelBottom.Controls.Add(this.lblCount);
            this.panelBottom.Controls.Add(this.btnOpen);
            this.panelBottom.Controls.Add(this.btnSaveAs);
            this.panelBottom.Controls.Add(this.btnAddFile);
            this.panelBottom.Controls.Add(this.btnPasteScreenshot);
            this.panelBottom.Controls.Add(this.btnDelete);
            this.panelBottom.Controls.Add(this.btnClose);

            // Count label
            this.lblCount.Text = "0 attachment(s)";
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(160, 175, 190);
            this.lblCount.Location = new System.Drawing.Point(10, 13);
            this.lblCount.Size = new System.Drawing.Size(120, 20);

            // Open button
            this.btnOpen.Text = "📂 Open";
            this.btnOpen.Location = new System.Drawing.Point(140, 8);
            this.btnOpen.Size = new System.Drawing.Size(85, 30);
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnOpen.ForeColor = System.Drawing.Color.White;
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.FlatAppearance.BorderSize = 0;
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);

            // Save As button
            this.btnSaveAs.Text = "💾 Save As";
            this.btnSaveAs.Location = new System.Drawing.Point(232, 8);
            this.btnSaveAs.Size = new System.Drawing.Size(90, 30);
            this.btnSaveAs.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveAs.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnSaveAs.ForeColor = System.Drawing.Color.White;
            this.btnSaveAs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAs.FlatAppearance.BorderSize = 0;
            this.btnSaveAs.Click += new System.EventHandler(this.BtnSaveAs_Click);

            // Add File button
            this.btnAddFile.Text = "📎 Attach File";
            this.btnAddFile.Location = new System.Drawing.Point(330, 8);
            this.btnAddFile.Size = new System.Drawing.Size(110, 30);
            this.btnAddFile.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddFile.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnAddFile.ForeColor = System.Drawing.Color.White;
            this.btnAddFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFile.FlatAppearance.BorderSize = 0;
            this.btnAddFile.Click += new System.EventHandler(this.BtnAddFile_Click);

            // Paste Screenshot button
            this.btnPasteScreenshot.Text = "📸 Paste Screenshot";
            this.btnPasteScreenshot.Location = new System.Drawing.Point(448, 8);
            this.btnPasteScreenshot.Size = new System.Drawing.Size(150, 30);
            this.btnPasteScreenshot.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnPasteScreenshot.BackColor = System.Drawing.Color.FromArgb(155, 89, 182);
            this.btnPasteScreenshot.ForeColor = System.Drawing.Color.White;
            this.btnPasteScreenshot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPasteScreenshot.FlatAppearance.BorderSize = 0;
            this.btnPasteScreenshot.Click += new System.EventHandler(this.BtnPasteScreenshot_Click);

            // Delete button
            this.btnDelete.Text = "🗑️ Delete";
            this.btnDelete.Location = new System.Drawing.Point(606, 8);
            this.btnDelete.Size = new System.Drawing.Size(85, 30);
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);

            // Close button
            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(760, 8);
            this.btnClose.Size = new System.Drawing.Size(90, 30);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(127, 140, 141);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);

            // Add to form
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.gridAttachments);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.panelBottom);

            ((System.ComponentModel.ISupportInitialize)(this.gridAttachments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
