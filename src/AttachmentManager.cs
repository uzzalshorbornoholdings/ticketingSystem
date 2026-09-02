using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    /// <summary>
    /// Manages file attachments and screenshot captures for tickets.
    /// Handles physical file storage in the attachments/ folder and metadata persistence in MySQL.
    /// </summary>
    public class AttachmentManager
    {
        private readonly DatabaseManager _db;
        private readonly string _storageDir;

        public AttachmentManager(DatabaseManager db)
        {
            _db = db;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _storageDir = Path.Combine(baseDir, "attachments");
            EnsureDirectoryExists();
        }

        public string StorageDirectory => _storageDir;

        private void EnsureDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_storageDir))
                {
                    Directory.CreateDirectory(_storageDir);
                }
            }
            catch { }
        }

        /// <summary>
        /// Saves a file from disk as a ticket attachment.
        /// </summary>
        public bool SaveFileAttachment(int ticketId, string employeeId, string sourceFilePath, out int attachmentId, out string savedFileName, out string errorMessage)
        {
            attachmentId = -1;
            savedFileName = string.Empty;
            errorMessage = string.Empty;

            if (!File.Exists(sourceFilePath))
            {
                errorMessage = "Source file does not exist.";
                return false;
            }

            try
            {
                EnsureDirectoryExists();

                string originalName = Path.GetFileName(sourceFilePath);
                string ext = Path.GetExtension(sourceFilePath);
                string safeName = Path.GetFileNameWithoutExtension(sourceFilePath);
                string uniqueFileName = $"att_{ticketId}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";
                string destinationPath = Path.Combine(_storageDir, uniqueFileName);

                File.Copy(sourceFilePath, destinationPath, true);

                var fileInfo = new FileInfo(destinationPath);
                long fileSize = fileInfo.Length;
                string fileType = GetMimeType(ext);

                string relativePath = Path.Combine("attachments", uniqueFileName);

                string insertSql = @"
                    INSERT INTO ticket_attachments (ticket_id, employee_id, file_name, file_path, file_size, file_type, created_at)
                    VALUES (@ticketId, @empId, @fileName, @filePath, @fileSize, @fileType, NOW())";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@ticketId", ticketId),
                    new MySqlParameter("@empId", string.IsNullOrEmpty(employeeId) ? (object)DBNull.Value : employeeId),
                    new MySqlParameter("@fileName", originalName),
                    new MySqlParameter("@filePath", relativePath),
                    new MySqlParameter("@fileSize", fileSize),
                    new MySqlParameter("@fileType", fileType)
                };

                _db.ExecuteNonQuery(insertSql, parameters);

                object lastIdObj = _db.ExecuteScalar("SELECT LAST_INSERT_ID()");
                if (lastIdObj != null && lastIdObj != DBNull.Value)
                {
                    attachmentId = Convert.ToInt32(lastIdObj);
                }

                savedFileName = originalName;

                // Log audit trail
                LogAudit(ticketId, employeeId, "Upload Attachment", $"Attached file '{originalName}' ({FormatFileSize(fileSize)})");

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Saves an image from the Windows clipboard directly as a PNG attachment.
        /// </summary>
        public bool SaveClipboardImage(int ticketId, string employeeId, Image image, out int attachmentId, out string savedFileName, out string errorMessage)
        {
            attachmentId = -1;
            savedFileName = string.Empty;
            errorMessage = string.Empty;

            if (image == null)
            {
                errorMessage = "No valid image provided.";
                return false;
            }

            try
            {
                EnsureDirectoryExists();

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string uniqueFileName = $"screenshot_{ticketId}_{timestamp}_{Guid.NewGuid().ToString("N").Substring(0, 8)}.png";
                string destinationPath = Path.Combine(_storageDir, uniqueFileName);

                image.Save(destinationPath, ImageFormat.Png);

                var fileInfo = new FileInfo(destinationPath);
                long fileSize = fileInfo.Length;
                string originalName = $"Screenshot_{timestamp}.png";
                string relativePath = Path.Combine("attachments", uniqueFileName);

                string insertSql = @"
                    INSERT INTO ticket_attachments (ticket_id, employee_id, file_name, file_path, file_size, file_type, created_at)
                    VALUES (@ticketId, @empId, @fileName, @filePath, @fileSize, 'image/png', NOW())";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@ticketId", ticketId),
                    new MySqlParameter("@empId", string.IsNullOrEmpty(employeeId) ? (object)DBNull.Value : employeeId),
                    new MySqlParameter("@fileName", originalName),
                    new MySqlParameter("@filePath", relativePath),
                    new MySqlParameter("@fileSize", fileSize)
                };

                _db.ExecuteNonQuery(insertSql, parameters);

                object lastIdObj = _db.ExecuteScalar("SELECT LAST_INSERT_ID()");
                if (lastIdObj != null && lastIdObj != DBNull.Value)
                {
                    attachmentId = Convert.ToInt32(lastIdObj);
                }

                savedFileName = originalName;

                // Log audit trail
                LogAudit(ticketId, employeeId, "Upload Attachment", $"Attached clipboard screenshot '{originalName}' ({FormatFileSize(fileSize)})");

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Retrieves all attachments for a specific ticket.
        /// </summary>
        public DataTable GetAttachments(int ticketId)
        {
            string query = @"
                SELECT a.id AS AttachmentID, a.ticket_id AS TicketID,
                       a.file_name AS FileName, a.file_path AS FilePath,
                       a.file_size AS FileSize, a.file_type AS FileType,
                       a.created_at AS UploadDate,
                       e.name AS UploadedBy, u.role AS UploaderRole
                FROM ticket_attachments a
                LEFT JOIN employees e ON a.employee_id = e.id
                LEFT JOIN users u ON u.employee_id = a.employee_id
                WHERE a.ticket_id = @ticketId
                ORDER BY a.created_at DESC";

            return _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });
        }

        /// <summary>
        /// Returns the count of attachments for a specific ticket.
        /// </summary>
        public int GetAttachmentCount(int ticketId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM ticket_attachments WHERE ticket_id = @ticketId";
                object result = _db.ExecuteScalar(query, new MySqlParameter[] { new MySqlParameter("@ticketId", ticketId) });
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Resolves the absolute physical path of a stored attachment.
        /// </summary>
        public string ResolvePhysicalPath(string relativeOrFullPath)
        {
            if (string.IsNullOrEmpty(relativeOrFullPath)) return string.Empty;

            if (Path.IsPathRooted(relativeOrFullPath))
            {
                return relativeOrFullPath;
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(Path.Combine(baseDir, relativeOrFullPath));
        }

        /// <summary>
        /// Opens the attachment with the operating system's default viewer/editor.
        /// </summary>
        public bool OpenAttachment(string relativeOrFullPath, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                string fullPath = ResolvePhysicalPath(relativeOrFullPath);
                if (!File.Exists(fullPath))
                {
                    errorMessage = $"Attachment file not found at: {fullPath}";
                    return false;
                }

                var psi = new ProcessStartInfo(fullPath)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Deletes an attachment record and its physical file.
        /// </summary>
        public bool DeleteAttachment(int attachmentId, string employeeId, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                // First retrieve ticket_id and path
                string selectSql = "SELECT ticket_id, file_path, file_name FROM ticket_attachments WHERE id = @id";
                var dt = _db.ExecuteQuery(selectSql, new MySqlParameter[] { new MySqlParameter("@id", attachmentId) });
                if (dt.Rows.Count == 0)
                {
                    errorMessage = "Attachment not found.";
                    return false;
                }

                int ticketId = Convert.ToInt32(dt.Rows[0]["ticket_id"]);
                string relativePath = dt.Rows[0]["file_path"].ToString();
                string fileName = dt.Rows[0]["file_name"].ToString();

                // Delete DB record
                string deleteSql = "DELETE FROM ticket_attachments WHERE id = @id";
                _db.ExecuteNonQuery(deleteSql, new MySqlParameter[] { new MySqlParameter("@id", attachmentId) });

                // Attempt physical file removal
                try
                {
                    string fullPath = ResolvePhysicalPath(relativePath);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch { }

                LogAudit(ticketId, employeeId, "Delete Attachment", $"Deleted attachment '{fileName}'");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private void LogAudit(int ticketId, string employeeId, string action, string details)
        {
            if (string.IsNullOrEmpty(employeeId)) return;
            try
            {
                string query = "INSERT INTO audit_logs (ticket_id, employee_id, action, details) VALUES (@ticketId, @empId, @action, @details)";
                _db.ExecuteNonQuery(query, new MySqlParameter[] {
                    new MySqlParameter("@ticketId", ticketId),
                    new MySqlParameter("@empId", employeeId),
                    new MySqlParameter("@action", action),
                    new MySqlParameter("@details", details)
                });
            }
            catch { }
        }

        /// <summary>
        /// Formats raw byte count into human-readable format.
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            if (bytes >= 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
            if (bytes >= 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0):F1} MB";
            if (bytes >= 1024)
                return $"{bytes / 1024.0:F0} KB";
            return $"{bytes} B";
        }

        /// <summary>
        /// Determines standard MIME types from file extensions.
        /// </summary>
        private static string GetMimeType(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return "application/octet-stream";
            string ext = extension.ToLowerInvariant().TrimStart('.');
            switch (ext)
            {
                case "png": return "image/png";
                case "jpg":
                case "jpeg": return "image/jpeg";
                case "gif": return "image/gif";
                case "bmp": return "image/bmp";
                case "ico": return "image/x-icon";
                case "pdf": return "application/pdf";
                case "txt":
                case "log": return "text/plain";
                case "json": return "application/json";
                case "xml": return "application/xml";
                case "zip": return "application/zip";
                case "csv": return "text/csv";
                default: return "application/octet-stream";
            }
        }
    }
}
