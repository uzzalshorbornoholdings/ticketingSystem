using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BitswardITSM.Core
{
    // ═══════════════════════════════════════════════════════════════════════
    //  ModernTheme — Centralized UI Design System
    //  Color palette, GDI+ rounded painting, control styling helpers,
    //  and slide-in Toast notification component.
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>Centralized color palette for a professional, clean, print-friendly enterprise light theme.</summary>
    public static class ThemeColors
    {
        // ── Base Backgrounds ──
        public static readonly Color BaseDark            = Color.FromArgb(248, 250, 252);   // #F8FAFC (Clean Slate Off-White)
        public static readonly Color BaseLight           = Color.FromArgb(248, 250, 252);   // #F8FAFC
        public static readonly Color CardSurface         = Color.FromArgb(255, 255, 255);   // #FFFFFF (Pure Crisp White)
        public static readonly Color SidebarDark         = Color.FromArgb(30, 41, 59);      // #1E293B (Deep Slate Navy Navigation)
        public static readonly Color InputBackground     = Color.FromArgb(255, 255, 255);   // #FFFFFF

        // ── Accent Colors ──
        public static readonly Color ElectricBlue        = Color.FromArgb(37, 99, 235);     // #2563EB (Royal Sapphire Blue)
        public static readonly Color Teal                = Color.FromArgb(13, 148, 136);    // #0D9488 (Emerald Teal)
        public static readonly Color Purple              = Color.FromArgb(124, 58, 237);    // #7C3AED (Deep Violet)
        public static readonly Color Indigo              = Color.FromArgb(79, 70, 229);     // #4F46E5 (Indigo)

        // ── Semantic Status ──
        public static readonly Color SuccessGreen        = Color.FromArgb(22, 163, 74);     // #16A34A (Forest Green)
        public static readonly Color WarningOrange       = Color.FromArgb(217, 119, 6);     // #D97706 (Amber Gold)
        public static readonly Color CriticalRed         = Color.FromArgb(220, 38, 38);     // #DC2626 (Crimson Red)
        public static readonly Color InfoCyan            = Color.FromArgb(2, 132, 199);     // #0284C7 (Sky Cyan)

        // ── Text ──
        public static readonly Color TextPrimary         = Color.FromArgb(15, 23, 42);      // #0F172A (Deep Slate Charcoal - Optimal Print & Screen Contrast)
        public static readonly Color TextSecondary       = Color.FromArgb(71, 85, 105);     // #475569 (Medium Slate)
        public static readonly Color TextMuted           = Color.FromArgb(100, 116, 139);   // #64748B (Slate Muted)

        // ── Borders ──
        public static readonly Color BorderSubtle        = Color.FromArgb(226, 232, 240);   // #E2E8F0 (Crisp Light Slate Border)
        public static readonly Color BorderFocused       = Color.FromArgb(37, 99, 235);     // #2563EB (Focus Highlight)

        // ── Row Alternation ──
        public static readonly Color RowAlt              = Color.FromArgb(248, 250, 252);   // #F8FAFC
        public static readonly Color RowSelected         = Color.FromArgb(224, 242, 254);   // #E0F2FE (Soft Blue Selection)

        // ── Chat Bubble Backgrounds ──
        public static readonly Color BubbleUser          = Color.FromArgb(239, 246, 255);   // #EFF6FF (Crisp Sky Tint)
        public static readonly Color BubbleAdmin         = Color.FromArgb(245, 243, 255);   // #F5F3FF (Crisp Lavender Tint)

        // ── Button Hover Offsets ──
        public static Color Lighten(Color c, int amount)
        {
            return Color.FromArgb(c.A,
                Math.Min(255, c.R + amount),
                Math.Min(255, c.G + amount),
                Math.Min(255, c.B + amount));
        }

        public static Color Darken(Color c, int amount)
        {
            return Color.FromArgb(c.A,
                Math.Max(0, c.R - amount),
                Math.Max(0, c.G - amount),
                Math.Max(0, c.B - amount));
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  GDI+ Drawing Helpers
    // ═══════════════════════════════════════════════════════════════════════

    public static class GdiPlus
    {
        /// <summary>Creates a rounded rectangle GraphicsPath.</summary>
        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Draws a modern card with optional colored top accent stripe.</summary>
        public static void DrawCard(Graphics g, Rectangle bounds, Color bg, Color border, Color? topAccent, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                using (var brush = new SolidBrush(bg))
                    g.FillPath(brush, path);

                using (var pen = new Pen(border, 1))
                    g.DrawPath(pen, path);
            }

            // Colored top accent stripe
            if (topAccent.HasValue)
            {
                var accentRect = new Rectangle(bounds.X + radius, bounds.Y, bounds.Width - radius * 2, 3);
                using (var brush = new SolidBrush(topAccent.Value))
                    g.FillRectangle(brush, accentRect);
            }
        }

        /// <summary>Paints a gradient header across given bounds.</summary>
        public static void DrawGradientHeader(Graphics g, Rectangle bounds, Color startColor, Color endColor,
                                              string title, string subtitle, Font titleFont, Font subFont)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Horizontal))
                g.FillRectangle(brush, bounds);

            if (!string.IsNullOrEmpty(title))
            {
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(title, titleFont, textBrush, bounds.X + 20, bounds.Y + bounds.Height / 2 - titleFont.Height + 4);
                    if (!string.IsNullOrEmpty(subtitle))
                    {
                        g.DrawString(subtitle, subFont, textBrush, bounds.X + 22, bounds.Y + bounds.Height / 2 + 4);
                    }
                }
            }
        }

        /// <summary>Draws a small rounded status badge with high-contrast text.</summary>
        public static void DrawStatusBadge(Graphics g, Rectangle bounds, string text, Color bg, Color textCol, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                using (var brush = new SolidBrush(bg))
                    g.FillPath(brush, path);
                using (var pen = new Pen(Color.FromArgb(20, textCol), 1))
                    g.DrawPath(pen, path);
            }

            using (var font = new Font("Segoe UI Semibold", 8f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(textCol))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, textBrush, bounds, sf);
            }
        }

        /// <summary>Fills a rounded rectangle with the given solid color.</summary>
        public static void FillRoundedRect(Graphics g, Rectangle bounds, Color color, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedRectanglePath(bounds, radius))
            using (var brush = new SolidBrush(color))
                g.FillPath(brush, path);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Control Styling Extensions
    // ═══════════════════════════════════════════════════════════════════════

    public static class ModernStyle
    {
        // ── Standard Fonts ──
        private static readonly Font _titleFont     = new Font("Segoe UI", 18f, FontStyle.Bold);
        private static readonly Font _subtitleFont  = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);
        private static readonly Font _bodyFont      = new Font("Segoe UI", 10f);
        private static readonly Font _badgeFont     = new Font("Segoe UI", 8f, FontStyle.Bold);

        public static Font TitleFont    { get { return _titleFont; } }
        public static Font SubtitleFont { get { return _subtitleFont; } }
        public static Font BodyFont     { get { return _bodyFont; } }
        public static Font BadgeFont    { get { return _badgeFont; } }

        /// <summary>Apply modern flat styling with rounded hover/click visuals to a Button.</summary>
        public static void StyleButton(Button btn, Color bg, Color hover, Color text, int radius = 6)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bg;
            btn.ForeColor = text;
            btn.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = hover;
            btn.FlatAppearance.MouseDownBackColor = ThemeColors.Darken(bg, 20);
            btn.TextAlign = ContentAlignment.MiddleCenter;

            // Paint rounded background via owner-draw Paint event
            btn.Paint += (s, e) =>
            {
                var b = (Button)s;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                var bgColor = b.ClientRectangle.Contains(b.PointToClient(Cursor.Position)) ? hover : bg;

                using (var path = GdiPlus.CreateRoundedRectanglePath(rect, radius))
                {
                    using (var brush = new SolidBrush(bgColor))
                        g.FillPath(brush, path);
                }

                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                if (b.TextAlign == ContentAlignment.MiddleLeft)
                {
                    sf.Alignment = StringAlignment.Near;
                    var textRect = new Rectangle(12, 0, b.Width - 12, b.Height);
                    using (var textBrush = new SolidBrush(b.ForeColor))
                        g.DrawString(b.Text, b.Font, textBrush, textRect, sf);
                }
                else
                {
                    using (var textBrush = new SolidBrush(b.ForeColor))
                        g.DrawString(b.Text, b.Font, textBrush, rect, sf);
                }
            };
        }

        /// <summary>Applies modern sidebar navigation button styling.</summary>
        public static void StyleSidebarButton(Button btn, bool isActive = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(12, 0, 0, 0);

            if (isActive)
            {
                btn.BackColor = ThemeColors.ElectricBlue;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = ThemeColors.Darken(ThemeColors.ElectricBlue, 15);
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(203, 213, 225); // Slate 300 on dark sidebar
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 55, 72);
            }
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(51, 65, 85);
        }

        /// <summary>Apply modern styling to a TextBox with crisp light borders and focus accents.</summary>
        public static void StyleTextBox(TextBox tb)
        {
            tb.BackColor = Color.White;
            tb.ForeColor = ThemeColors.TextPrimary;
            tb.Font = new Font("Segoe UI", 10f);
            tb.BorderStyle = BorderStyle.FixedSingle;

            tb.GotFocus += (s, e) =>
            {
                var t = (TextBox)s;
                if (t.Parent != null) t.Parent.Invalidate(new Rectangle(t.Left - 2, t.Top - 2, t.Width + 4, t.Height + 4), false);
            };
            tb.LostFocus += (s, e) =>
            {
                var t = (TextBox)s;
                if (t.Parent != null) t.Parent.Invalidate(new Rectangle(t.Left - 2, t.Top - 2, t.Width + 4, t.Height + 4), false);
            };
        }

        /// <summary>Apply modern light theme to a DataGridView with clean borders, optimal contrast, and print-ready clarity.</summary>
        public static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = ThemeColors.CardSurface;
            grid.GridColor = ThemeColors.BorderSubtle;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;

            // Header Styling
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249); // Slate 100
            grid.ColumnHeadersDefaultCellStyle.ForeColor = ThemeColors.TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 4, 4);
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 36;

            // Default Cell Style
            grid.DefaultCellStyle.BackColor = ThemeColors.CardSurface;
            grid.DefaultCellStyle.ForeColor = ThemeColors.TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = ThemeColors.RowSelected;
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(3, 105, 161); // Deep Blue text on light blue selection
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grid.RowTemplate.Height = 32;

            // Alternating Rows
            grid.AlternatingRowsDefaultCellStyle.BackColor = ThemeColors.RowAlt;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = ThemeColors.TextPrimary;

            // Custom cell painting for status/priority badges
            grid.CellPainting += GridCellPainting;
        }

        /// <summary>Custom cell painter that renders Status and Priority columns as clean print-friendly rounded badges.</summary>
        private static void GridCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Value == null) return;

            string colName = grid.Columns[e.ColumnIndex].Name;
            string value = e.Value.ToString();

            bool isStatus = string.Equals(colName, "Status", StringComparison.OrdinalIgnoreCase);
            bool isPriority = string.Equals(colName, "Priority", StringComparison.OrdinalIgnoreCase);

            if (!isStatus && !isPriority) return;

            e.Handled = true;

            // Paint background first
            using (var bgBrush = new SolidBrush(e.CellStyle.BackColor))
                e.Graphics.FillRectangle(bgBrush, e.CellBounds);

            // Paint selection highlight
            if ((e.State & DataGridViewElementStates.Selected) != 0)
            {
                using (var selBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
                    e.Graphics.FillRectangle(selBrush, e.CellBounds);
            }

            Color badgeBg, badgeFg;
            GetBadgeColors(isStatus, value, out badgeBg, out badgeFg);

            // Draw the badge
            int badgeW = Math.Min(e.CellBounds.Width - 12, 90);
            int badgeH = 22;
            int x = e.CellBounds.X + (e.CellBounds.Width - badgeW) / 2;
            int y = e.CellBounds.Y + (e.CellBounds.Height - badgeH) / 2;
            var badgeRect = new Rectangle(x, y, badgeW, badgeH);

            GdiPlus.DrawStatusBadge(e.Graphics, badgeRect, value, badgeBg, badgeFg, 6);
        }

        private static void GetBadgeColors(bool isStatus, string value, out Color bg, out Color fg)
        {
            fg = Color.White;
            string v = value.ToUpperInvariant();

            if (isStatus)
            {
                switch (v)
                {
                    case "NEW":         bg = Color.FromArgb(219, 234, 254); fg = Color.FromArgb(30, 64, 175); return; // Soft Blue
                    case "ASSIGNED":    bg = Color.FromArgb(224, 242, 254); fg = Color.FromArgb(3, 105, 161); return; // Soft Cyan
                    case "IN PROGRESS": bg = Color.FromArgb(254, 243, 199); fg = Color.FromArgb(146, 64, 14); return; // Soft Amber
                    case "RESOLVED":    bg = Color.FromArgb(220, 252, 231); fg = Color.FromArgb(22, 101, 52); return; // Soft Emerald
                    case "CLOSED":      bg = Color.FromArgb(241, 245, 249); fg = Color.FromArgb(71, 85, 105); return; // Slate
                    default:            bg = Color.FromArgb(241, 245, 249); fg = Color.FromArgb(71, 85, 105); return;
                }
            }
            else // Priority
            {
                switch (v)
                {
                    case "P1":
                    case "CRITICAL":   bg = Color.FromArgb(254, 226, 226); fg = Color.FromArgb(153, 27, 27); return; // Soft Red
                    case "P2":
                    case "HIGH":       bg = Color.FromArgb(255, 237, 213); fg = Color.FromArgb(154, 52, 18); return; // Soft Orange
                    case "P3":
                    case "MEDIUM":     bg = Color.FromArgb(254, 249, 195); fg = Color.FromArgb(133, 77, 14); return; // Soft Yellow
                    case "P4":
                    case "LOW":        bg = Color.FromArgb(224, 242, 254); fg = Color.FromArgb(7, 89, 133);  return; // Soft Cyan
                    default:           bg = Color.FromArgb(241, 245, 249); fg = Color.FromArgb(71, 85, 105); return;
                }
            }
        }

        /// <summary>Apply modern light styling to a TabControl with electric blue active indicators.</summary>
        public static void StyleTabControl(TabControl tab)
        {
            tab.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab.ItemSize = new Size(140, 36);
            tab.SizeMode = TabSizeMode.Fixed;
            tab.Padding = new Point(12, 6);

            tab.DrawItem += (s, e) =>
            {
                var tc = (TabControl)s;
                bool isSelected = (e.Index == tc.SelectedIndex);
                var bounds = tc.GetTabRect(e.Index);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Tab background
                Color tabBg = isSelected ? Color.White : Color.FromArgb(241, 245, 249);
                using (var brush = new SolidBrush(tabBg))
                    g.FillRectangle(brush, bounds);

                // Active indicator line
                if (isSelected)
                {
                    using (var pen = new Pen(ThemeColors.ElectricBlue, 3))
                        g.DrawLine(pen, bounds.Left + 4, bounds.Bottom - 2, bounds.Right - 4, bounds.Bottom - 2);
                }

                // Text
                Color textColor = isSelected ? ThemeColors.ElectricBlue : ThemeColors.TextSecondary;
                Font textFont = isSelected
                    ? new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold)
                    : new Font("Segoe UI", 9.5f);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                using (var textBrush = new SolidBrush(textColor))
                    g.DrawString(tc.TabPages[e.Index].Text, textFont, textBrush, bounds, sf);

                if (!isSelected) textFont.Dispose();
            };
        }

        /// <summary>Apply the complete light theme to a Form (background, double-buffer, etc).</summary>
        public static void StyleForm(Form form)
        {
            form.BackColor = ThemeColors.BaseLight;
            form.ForeColor = ThemeColors.TextPrimary;
            form.Font = _bodyFont;

            // Enable double-buffering via reflection (reduces flicker)
            typeof(Form).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(form, true);
        }

        /// <summary>Apply theme to a Panel (card surface).</summary>
        public static void StylePanel(Panel panel, Color? bg = null)
        {
            panel.BackColor = bg ?? ThemeColors.CardSurface;

            // Enable double-buffering on the panel
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panel, true);
        }

        /// <summary>Style a RichTextBox for modern light theme and crisp document reading.</summary>
        public static void StyleRichTextBox(RichTextBox rtb)
        {
            rtb.BackColor = Color.White;
            rtb.ForeColor = ThemeColors.TextPrimary;
            rtb.Font = new Font("Segoe UI", 9.5f);
            rtb.BorderStyle = BorderStyle.None;
            rtb.ReadOnly = true;
        }

        /// <summary>Style a ComboBox for modern light theme.</summary>
        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.BackColor = Color.White;
            cmb.ForeColor = ThemeColors.TextPrimary;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.Font = new Font("Segoe UI", 9.5f);
        }

        /// <summary>Style a Label as a section header.</summary>
        public static void StyleSectionLabel(Label lbl)
        {
            lbl.ForeColor = ThemeColors.TextSecondary;
            lbl.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        }

        /// <summary>Style a Label as a large title.</summary>
        public static void StyleTitleLabel(Label lbl)
        {
            lbl.ForeColor = ThemeColors.TextPrimary;
            lbl.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Modern Toast Notification Engine
    //  Multi-instance stackable slide-in toasts with auto-dismiss,
    //  hover-to-pause, click callbacks, and robust timer lifecycle management.
    // ═══════════════════════════════════════════════════════════════════════

    public enum ToastType { Info, Success, Warning, Error }

    public class ModernToast : Form
    {
        private Timer _slideTimer;
        private Timer _dismissTimer;
        private Timer _fadeTimer;
        private int _targetY;
        private int _targetX;
        private bool _isFading = false;
        private Action _onClickAction;
        private readonly int _durationMs;

        // Static registry of currently active toast windows to manage vertical stacking
        private static readonly List<ModernToast> _openToasts = new List<ModernToast>();
        private static readonly object _stackLock = new object();

        private static readonly Dictionary<ToastType, Color> _toastColors = new Dictionary<ToastType, Color>
        {
            { ToastType.Info,    ThemeColors.ElectricBlue },
            { ToastType.Success, ThemeColors.SuccessGreen },
            { ToastType.Warning, ThemeColors.WarningOrange },
            { ToastType.Error,   ThemeColors.CriticalRed }
        };

        private static readonly Dictionary<ToastType, string> _toastIcons = new Dictionary<ToastType, string>
        {
            { ToastType.Info,    "ℹ️" },
            { ToastType.Success, "✅" },
            { ToastType.Warning, "⚠️" },
            { ToastType.Error,   "❌" }
        };

        private ModernToast(int durationMs)
        {
            _durationMs = durationMs;
        }

        /// <summary>
        /// Displays a modern, non-blocking toast notification. Supports multi-toast stacking,
        /// interactive click-to-action callbacks, and hover-to-pause.
        /// </summary>
        public static void Show(Form parent, string message, ToastType type, int durationMs = 3500, Action onClick = null, string title = null, bool playSound = false)
        {
            if (parent != null && parent.IsDisposed) return;

            // Marshall to UI thread if necessary
            if (parent != null && parent.InvokeRequired)
            {
                if (parent.IsHandleCreated && !parent.IsDisposed)
                {
                    try
                    {
                        parent.BeginInvoke(new Action(() => Show(parent, message, type, durationMs, onClick, title, playSound)));
                    }
                    catch { }
                }
                return;
            }

            try
            {
                if (playSound)
                {
                    try { System.Media.SystemSounds.Asterisk.Play(); } catch { }
                }

                var toast = new ModernToast(durationMs);
                toast.SetupToast(parent, message, type, durationMs, onClick, title);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ModernToast Error] {ex.Message}");
            }
        }

        private void SetupToast(Form parent, string message, ToastType type, int durationMs, Action onClick, string title)
        {
            _onClickAction = onClick;
            Color accentColor = _toastColors.ContainsKey(type) ? _toastColors[type] : ThemeColors.ElectricBlue;
            string icon = _toastIcons.ContainsKey(type) ? _toastIcons[type] : "ℹ️";

            // Form styling
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(370, string.IsNullOrEmpty(title) ? 68 : 82);
            this.BackColor = ThemeColors.CardSurface;
            this.Opacity = 0.96;

            // Rounded corner region
            using (var path = GdiPlus.CreateRoundedRectanglePath(new Rectangle(0, 0, this.Width, this.Height), 10))
            {
                this.Region = new Region(path);
            }

            // Left accent border panel
            var accentBar = new Panel
            {
                BackColor = accentColor,
                Size = new Size(6, this.Height),
                Location = new Point(0, 0)
            };
            this.Controls.Add(accentBar);

            // Icon label
            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 16f),
                ForeColor = accentColor,
                AutoSize = false,
                Size = new Size(36, 36),
                Location = new Point(12, string.IsNullOrEmpty(title) ? 16 : 22),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblIcon);

            int textLeft = 52;
            int textWidth = this.Width - textLeft - 28;

            if (!string.IsNullOrEmpty(title))
            {
                var lblTitle = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    ForeColor = ThemeColors.TextPrimary,
                    AutoSize = false,
                    Size = new Size(textWidth, 22),
                    Location = new Point(textLeft, 10),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoEllipsis = true
                };
                this.Controls.Add(lblTitle);
                if (onClick != null) WireClickEvents(lblTitle);
            }

            // Message label
            var lblMsg = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", string.IsNullOrEmpty(title) ? 9.5f : 8.75f),
                ForeColor = string.IsNullOrEmpty(title) ? ThemeColors.TextPrimary : ThemeColors.TextSecondary,
                AutoSize = false,
                Size = new Size(textWidth, string.IsNullOrEmpty(title) ? 48 : 42),
                Location = new Point(textLeft, string.IsNullOrEmpty(title) ? 10 : 32),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            this.Controls.Add(lblMsg);

            // Close button (X)
            var lblClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = ThemeColors.TextMuted,
                Size = new Size(20, 20),
                Location = new Point(this.Width - 24, 6),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            lblClose.MouseEnter += (s, e) => lblClose.ForeColor = ThemeColors.CriticalRed;
            lblClose.MouseLeave += (s, e) => lblClose.ForeColor = ThemeColors.TextMuted;
            lblClose.Click += (s, e) => FadeAndClose();
            this.Controls.Add(lblClose);

            // Wire click actions if interactive
            if (onClick != null)
            {
                this.Cursor = Cursors.Hand;
                WireClickEvents(this);
                WireClickEvents(lblIcon);
                WireClickEvents(lblMsg);
                WireClickEvents(accentBar);
            }

            // Hover-to-pause dismissal timer
            WireHoverEvents(this);
            WireHoverEvents(lblIcon);
            WireHoverEvents(lblMsg);
            WireHoverEvents(accentBar);

            // Calculate screen working area safely
            Rectangle workingArea;
            try
            {
                workingArea = parent != null && !parent.IsDisposed && parent.Visible
                    ? Screen.FromControl(parent).WorkingArea
                    : Screen.PrimaryScreen.WorkingArea;
            }
            catch
            {
                workingArea = Screen.PrimaryScreen.WorkingArea;
            }

            // Determine vertical position in toast stack
            lock (_stackLock)
            {
                // Prune any disposed references
                _openToasts.RemoveAll(t => t.IsDisposed || t._isFading);

                int stackOffset = 0;
                foreach (var openToast in _openToasts)
                {
                    stackOffset += openToast.Height + 10;
                }

                _targetX = workingArea.Right - this.Width - 18;
                _targetY = workingArea.Bottom - this.Height - 14 - stackOffset;

                // If stack exceeds screen height, wrap or clamp
                if (_targetY < workingArea.Top + 20)
                {
                    _targetY = workingArea.Top + 20;
                }

                _openToasts.Add(this);
            }

            // Position initial off-screen location
            this.Location = new Point(_targetX, workingArea.Bottom + 10);

            // Slide-in animation timer
            _slideTimer = new Timer();
            _slideTimer.Interval = 15;
            _slideTimer.Tick += (s, e) =>
            {
                if (this.IsDisposed || !this.IsHandleCreated)
                {
                    StopAndDisposeTimer(ref _slideTimer);
                    return;
                }

                if (this.Top > _targetY)
                {
                    int step = Math.Max(3, (this.Top - _targetY) / 3);
                    this.Top -= step;
                }
                else
                {
                    this.Top = _targetY;
                    StopAndDisposeTimer(ref _slideTimer);
                }
            };

            // Auto-dismiss timer
            _dismissTimer = new Timer();
            _dismissTimer.Interval = durationMs;
            _dismissTimer.Tick += (s, e) =>
            {
                StopAndDisposeTimer(ref _dismissTimer);
                FadeAndClose();
            };

            this.Show();
            _slideTimer.Start();
            _dismissTimer.Start();
        }

        private void WireClickEvents(Control ctrl)
        {
            if (ctrl == null) return;
            ctrl.Cursor = Cursors.Hand;
            ctrl.Click += (s, e) =>
            {
                try
                {
                    _onClickAction?.Invoke();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Toast Click Error] {ex.Message}");
                }
                FadeAndClose();
            };
        }

        private void WireHoverEvents(Control ctrl)
        {
            if (ctrl == null) return;
            ctrl.MouseEnter += (s, e) =>
            {
                if (_dismissTimer != null && _dismissTimer.Enabled)
                {
                    _dismissTimer.Stop();
                }
            };

            ctrl.MouseLeave += (s, e) =>
            {
                // Check if cursor has left the bounds of this toast entirely
                Point p = this.PointToClient(Cursor.Position);
                if (!this.ClientRectangle.Contains(p))
                {
                    if (!_isFading && _dismissTimer != null && !_dismissTimer.Enabled)
                    {
                        _dismissTimer.Interval = Math.Min(1800, _durationMs);
                        _dismissTimer.Start();
                    }
                }
            };
        }

        private void FadeAndClose()
        {
            if (_isFading || this.IsDisposed) return;
            _isFading = true;

            StopAndDisposeTimer(ref _slideTimer);
            StopAndDisposeTimer(ref _dismissTimer);

            lock (_stackLock)
            {
                _openToasts.Remove(this);
                // Reposition remaining open toasts smoothly
                RepositionOpenToasts();
            }

            _fadeTimer = new Timer();
            _fadeTimer.Interval = 20;
            _fadeTimer.Tick += (s, e) =>
            {
                if (this.IsDisposed || !this.IsHandleCreated)
                {
                    StopAndDisposeTimer(ref _fadeTimer);
                    return;
                }

                if (this.Opacity > 0.1)
                {
                    this.Opacity -= 0.12;
                }
                else
                {
                    StopAndDisposeTimer(ref _fadeTimer);
                    try
                    {
                        this.Close();
                        this.Dispose();
                    }
                    catch { }
                }
            };
            _fadeTimer.Start();
        }

        private static void RepositionOpenToasts()
        {
            lock (_stackLock)
            {
                _openToasts.RemoveAll(t => t.IsDisposed || t._isFading);
                for (int i = 0; i < _openToasts.Count; i++)
                {
                    var toast = _openToasts[i];
                    if (toast.IsDisposed || !toast.IsHandleCreated) continue;

                    Rectangle workingArea;
                    try
                    {
                        workingArea = Screen.FromControl(toast).WorkingArea;
                    }
                    catch
                    {
                        workingArea = Screen.PrimaryScreen.WorkingArea;
                    }

                    int newTargetY = workingArea.Bottom - toast.Height - 14;
                    for (int j = 0; j < i; j++)
                    {
                        newTargetY -= (_openToasts[j].Height + 10);
                    }

                    toast._targetY = Math.Max(workingArea.Top + 20, newTargetY);
                    toast.AnimateToTargetY();
                }
            }
        }

        private void AnimateToTargetY()
        {
            if (this.IsDisposed || !this.IsHandleCreated || _isFading) return;

            var shiftTimer = new Timer();
            shiftTimer.Interval = 15;
            shiftTimer.Tick += (s, e) =>
            {
                if (this.IsDisposed || !this.IsHandleCreated || _isFading)
                {
                    StopAndDisposeTimer(ref shiftTimer);
                    return;
                }

                if (Math.Abs(this.Top - _targetY) > 2)
                {
                    int step = (this.Top < _targetY) 
                        ? Math.Max(2, (_targetY - this.Top) / 3) 
                        : -Math.Max(2, (this.Top - _targetY) / 3);
                    this.Top += step;
                }
                else
                {
                    this.Top = _targetY;
                    StopAndDisposeTimer(ref shiftTimer);
                }
            };
            shiftTimer.Start();
        }

        private static void StopAndDisposeTimer(ref Timer timer)
        {
            if (timer != null)
            {
                try
                {
                    timer.Stop();
                    timer.Dispose();
                }
                catch { }
                timer = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAndDisposeTimer(ref _slideTimer);
                StopAndDisposeTimer(ref _dismissTimer);
                StopAndDisposeTimer(ref _fadeTimer);
                lock (_stackLock)
                {
                    _openToasts.Remove(this);
                }
            }
            base.Dispose(disposing);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW — prevent taskbar button
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE — prevent stealing focus
                return cp;
            }
        }

        protected override bool ShowWithoutActivation { get { return true; } }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Intelligent Real-Time Search Engine Helper
    //  Builds safe multi-token, multi-column ADO.NET RowFilter expressions
    //  supporting text, numbers, and dates.
    // ═══════════════════════════════════════════════════════════════════════

    public static class IntelligentSearchHelper
    {
        /// <summary>
        /// Constructs a safe, multi-token, multi-column ADO.NET RowFilter expression.
        /// Each space-separated token must match at least one of the specified columns (AND across tokens, OR across columns).
        /// </summary>
        public static string BuildRowFilter(string searchText, params string[] columnNames)
        {
            if (string.IsNullOrWhiteSpace(searchText) || columnNames == null || columnNames.Length == 0)
                return string.Empty;

            string[] tokens = searchText.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return string.Empty;

            var tokenClauses = new List<string>();

            foreach (var rawToken in tokens)
            {
                string token = EscapeLikeValue(rawToken);
                if (string.IsNullOrEmpty(token)) continue;

                var colClauses = new List<string>();
                foreach (var col in columnNames)
                {
                    if (string.IsNullOrWhiteSpace(col)) continue;
                    // Wrap in CONVERT to safely support strings, integers, and dates
                    colClauses.Add($"CONVERT([{col}], 'System.String') LIKE '%{token}%'");
                }

                if (colClauses.Count > 0)
                {
                    tokenClauses.Add("(" + string.Join(" OR ", colClauses) + ")");
                }
            }

            return tokenClauses.Count > 0 ? string.Join(" AND ", tokenClauses) : string.Empty;
        }

        /// <summary>
        /// Escapes special characters for ADO.NET RowFilter LIKE expression (' [ ] % *).
        /// </summary>
        public static string EscapeLikeValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (char c in value)
            {
                if (c == '\'') sb.Append("''");
                else if (c == '[') sb.Append("[[]");
                else if (c == ']') sb.Append("[]]");
                else if (c == '%') sb.Append("[%]");
                else if (c == '*') sb.Append("[*]");
                else sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Applies row filter safely to a DataTable or DataView, handling exceptions gracefully.
        /// </summary>
        public static void ApplyFilter(DataTable dt, string rowFilter)
        {
            if (dt == null) return;
            try
            {
                dt.DefaultView.RowFilter = rowFilter ?? string.Empty;
            }
            catch
            {
                dt.DefaultView.RowFilter = string.Empty;
            }
        }

        /// <summary>
        /// Attaches modern placeholder behavior to a TextBox.
        /// </summary>
        public static void SetupSearchPlaceholder(TextBox textBox, string placeholderText)
        {
            if (textBox == null || string.IsNullOrEmpty(placeholderText)) return;

            bool isPlaceholder = string.IsNullOrEmpty(textBox.Text) || textBox.Text == placeholderText;
            if (isPlaceholder)
            {
                textBox.Text = placeholderText;
                textBox.ForeColor = ThemeColors.TextMuted;
            }

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = ThemeColors.TextPrimary;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = ThemeColors.TextMuted;
                }
            };
        }

        /// <summary>
        /// Gets the clean query string from a search box, ignoring the placeholder text.
        /// </summary>
        public static string GetCleanSearchQuery(TextBox textBox, string placeholderText)
        {
            if (textBox == null) return string.Empty;
            string text = textBox.Text.Trim();
            if (text == placeholderText) return string.Empty;
            return text;
        }
    }
}
