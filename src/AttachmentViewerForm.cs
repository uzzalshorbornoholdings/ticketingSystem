using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Interactive viewer modal for inspecting, opening, exporting, and managing
    /// attachments and screenshots associated with a specific ticket.
    /// </summary>
    public partial class AttachmentViewerForm : Form
    {
        private readonly DatabaseManager _db;
        private readonly AttachmentManager _attachmentManager;
        private readonly int _ticketId;
        private readonly string _ticketTitle;
        private readonly string _employeeId;
        private readonly string _userRole;

        private int _selectedAttachmentId = -1;
        private string _selectedFilePath = string.Empty;
        private string _selectedFileName = string.Empty;

        public event EventHandler AttachmentsChanged;

        public AttachmentViewerForm(DatabaseManager db, AttachmentManager attachmentManager, int ticketId, string ticketTitle, string employeeId, string userRole)
        {
            InitializeComponent();
            _db = db;
            _attachmentManager = attachmentManager;
            _ticketId = ticketId;
            _ticketTitle = ticketTitle;
            _employeeId = employeeId;
            _userRole = userRole;
        }

        private void AttachmentViewerForm_Load(object sender, EventArgs e)
        {
            lblSubHeader.Text = $"Ticket #{_ticketId} — {_ticketTitle}";
            LoadAttachments();
        }

        public void LoadAttachments()
        {
            try
            {
                var dt = _attachmentManager.GetAttachments(_ticketId);

                // Add a formatted display column for friendly file size
                if (!dt.Columns.Contains("SizeFormatted"))
                {
                    dt.Columns.Add("SizeFormatted", typeof(string));
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["FileSize"] != DBNull.Value)
                        {
                            long bytes = Convert.ToInt64(row["FileSize"]);
                            row["SizeFormatted"] = AttachmentManager.FormatFileSize(bytes);
                        }
                    }
                }

                gridAttachments.DataSource = dt;
                ConfigureAttachmentGrid();

                int count = dt.Rows.Count;
                lblCount.Text = $"{count} attachment{(count == 1 ? "" : "s")}";

                if (count == 0)
                {
                    ClearPreview();
                    ToggleActionButtons(false);
                }
                else
                {
                    ToggleActionButtons(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load attachments:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureAttachmentGrid()
        {
            if (gridAttachments.Columns.Count == 0) return;
            gridAttachments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            SetColumnWidth(gridAttachments, "AttachmentID", 50);
            SetColumnWidth(gridAttachments, "SizeFormatted", 80);
            SetColumnWidth(gridAttachments, "UploadedBy", 120);
            SetColumnWidth(gridAttachments, "UploadDate", 125);

            var nameCol = FindColumn(gridAttachments, "FileName");
            if (nameCol != null) nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Hide raw columns
            HideColumn(gridAttachments, "TicketID");
            HideColumn(gridAttachments, "FilePath");
            HideColumn(gridAttachments, "FileSize");
            HideColumn(gridAttachments, "FileType");
            HideColumn(gridAttachments, "UploaderRole");

            var dateCol = FindColumn(gridAttachments, "UploadDate");
            if (dateCol != null) dateCol.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Headers style
            gridAttachments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 43, 54);
            gridAttachments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridAttachments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
            gridAttachments.EnableHeadersVisualStyles = false;

            // Row styles
            gridAttachments.DefaultCellStyle.BackColor = Color.FromArgb(28, 32, 40);
            gridAttachments.DefaultCellStyle.ForeColor = Color.White;
            gridAttachments.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            gridAttachments.DefaultCellStyle.SelectionForeColor = Color.White;
            gridAttachments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(33, 38, 47);
            gridAttachments.GridColor = Color.FromArgb(50, 58, 70);
        }

        private void GridAttachments_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.BeginInvoke(new Action(() => ConfigureAttachmentGrid()));
        }

        private void GridAttachments_SelectionChanged(object sender, EventArgs e)
        {
            if (gridAttachments.SelectedRows.Count > 0)
            {
                var row = gridAttachments.SelectedRows[0];
                var idCell = FindCell(row, "AttachmentID");
                var pathCell = FindCell(row, "FilePath");
                var nameCell = FindCell(row, "FileName");
                var sizeCell = FindCell(row, "SizeFormatted");
                var uploaderCell = FindCell(row, "UploadedBy");
                var dateCell = FindCell(row, "UploadDate");

                if (idCell?.Value != null && idCell.Value != DBNull.Value)
                {
                    _selectedAttachmentId = Convert.ToInt32(idCell.Value);
                    _selectedFilePath = pathCell?.Value?.ToString() ?? string.Empty;
                    _selectedFileName = nameCell?.Value?.ToString() ?? string.Empty;

                    DisplayPreview(_selectedFilePath, _selectedFileName, sizeCell?.Value?.ToString(), uploaderCell?.Value?.ToString(), dateCell?.Value);
                    ToggleActionButtons(true);
                    return;
                }
            }

            ClearPreview();
            ToggleActionButtons(false);
        }

        private void DisplayPreview(string relativePath, string fileName, string sizeText, string uploader, object dateObj)
        {
            ClearImagePreview();

            string fullPath = _attachmentManager.ResolvePhysicalPath(relativePath);
            string ext = Path.GetExtension(fileName).ToLowerInvariant();

            bool isImage = ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif" || ext == ".ico";

            if (isImage && File.Exists(fullPath))
            {
                try
                {
                    // Load image into memory stream so file handle is released
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var img = Image.FromStream(fs))
                    {
                        picPreview.Image = new Bitmap(img);
                        lblPreviewDetails.Text = $"📄 {fileName}\n📏 {img.Width} × {img.Height} px ({sizeText})\n👤 Uploaded by: {uploader}\n🕒 {dateObj}";
                    }
                    return;
                }
                catch { }
            }

            // Non-image or failed image load
            picPreview.Image = null;
            lblPreviewDetails.Text = $"📄 {fileName}\n📦 Size: {sizeText}\n👤 Uploaded by: {uploader}\n🕒 {dateObj}\n\n(Click 'Open' below to launch in default app)";
        }

        private void ClearPreview()
        {
            ClearImagePreview();
            lblPreviewDetails.Text = "Select an attachment to preview...";
            _selectedAttachmentId = -1;
            _selectedFilePath = string.Empty;
            _selectedFileName = string.Empty;
        }

        private void ClearImagePreview()
        {
            if (picPreview.Image != null)
            {
                var oldImg = picPreview.Image;
                picPreview.Image = null;
                oldImg.Dispose();
            }
        }

        private void ToggleActionButtons(bool enabled)
        {
            btnOpen.Enabled = enabled;
            btnSaveAs.Enabled = enabled;
            btnDelete.Enabled = enabled;
        }

        private void GridAttachments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnOpen_Click(sender, e);
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath)) return;

            if (!_attachmentManager.OpenAttachment(_selectedFilePath, out string error))
            {
                MessageBox.Show($"Unable to open attachment:\n{error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveAs_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath) || string.IsNullOrEmpty(_selectedFileName)) return;

            string fullPath = _attachmentManager.ResolvePhysicalPath(_selectedFilePath);
            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Source file not found on disk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = _selectedFileName;
                sfd.Filter = "All Files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(fullPath, sfd.FileName, true);
                        MessageBox.Show($"File successfully saved to:\n{sfd.FileName}", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export file:\n{ex.Message}", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnAddFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select File to Attach";
                ofd.Filter = "All Files (*.*)|*.*|Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Log & Text Files (*.txt;*.log)|*.txt;*.log|Document Files (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    int uploadedCount = 0;
                    foreach (string file in ofd.FileNames)
                    {
                        if (_attachmentManager.SaveFileAttachment(_ticketId, _employeeId, file, out int _, out string _, out string error))
                        {
                            uploadedCount++;
                        }
                        else
                        {
                            MessageBox.Show($"Failed to attach {Path.GetFileName(file)}:\n{error}", "Upload Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    if (uploadedCount > 0)
                    {
                        LoadAttachments();
                        AttachmentsChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void BtnPasteScreenshot_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show("No screenshot image found in clipboard.\n\nTip: Press [Win + Shift + S] or [PrtScn] to capture your screen first, then click here to paste!",
                                "Clipboard Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Image clipImg = Clipboard.GetImage();
                if (clipImg != null)
                {
                    if (_attachmentManager.SaveClipboardImage(_ticketId, _employeeId, clipImg, out int _, out string savedName, out string error))
                    {
                        clipImg.Dispose();
                        LoadAttachments();
                        AttachmentsChanged?.Invoke(this, EventArgs.Empty);
                        MessageBox.Show($"Screenshot '{savedName}' successfully attached!", "Screenshot Captured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        clipImg.Dispose();
                        MessageBox.Show($"Failed to save screenshot:\n{error}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error pasting screenshot:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedAttachmentId == -1) return;

            var res = MessageBox.Show($"Are you sure you want to delete attachment '{_selectedFileName}'?",
                                      "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                ClearImagePreview(); // Release image lock before delete
                if (_attachmentManager.DeleteAttachment(_selectedAttachmentId, _employeeId, out string error))
                {
                    LoadAttachments();
                    AttachmentsChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show($"Failed to delete attachment:\n{error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            ClearImagePreview();
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearImagePreview();
            base.OnFormClosed(e);
        }

        private static void SetColumnWidth(DataGridView grid, string colName, int width)
        {
            if (grid == null || grid.Columns == null) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        col.Width = width;
                        break;
                    }
                }
            }
            catch { }
        }

        private static void HideColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null) return;
            try
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    {
                        col.Visible = false;
                        break;
                    }
                }
            }
            catch { }
        }

        private static DataGridViewColumn FindColumn(DataGridView grid, string colName)
        {
            if (grid == null || grid.Columns == null) return null;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                    return col;
            }
            return null;
        }

        private static DataGridViewCell FindCell(DataGridViewRow row, string colName)
        {
            if (row == null) return null;
            var grid = row.DataGridView;
            if (grid != null)
            {
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (string.Equals(col.Name, colName, StringComparison.OrdinalIgnoreCase))
                        return row.Cells[col.Index];
                }
            }
            return null;
        }
    }
}
