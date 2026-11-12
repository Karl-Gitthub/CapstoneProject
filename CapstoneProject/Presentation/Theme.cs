using System.Drawing;
using System.Windows.Forms;

namespace CapstoneProject.Presentation
{
    internal static class Theme
    {
        public static readonly Color Navy = Color.FromArgb(25, 49, 78);
        public static readonly Color Ink = Color.FromArgb(33, 43, 54);
        public static readonly Color Muted = Color.FromArgb(91, 105, 119);
        public static readonly Color Surface = Color.FromArgb(244, 246, 248);
        public static Label Label(string text, float size = 10, bool bold = false)
        {
            return new Label { Text = text, AutoSize = true, Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular), ForeColor = Ink, Margin = new Padding(0, 0, 0, 8) };
        }
        public static Button Button(string text, bool primary = false)
        {
            var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(100, 36), Padding = new Padding(12, 4, 12, 4), FlatStyle = FlatStyle.Flat, BackColor = primary ? Navy : Color.White, ForeColor = primary ? Color.White : Ink, Margin = new Padding(0, 0, 8, 0), UseVisualStyleBackColor = false };
            button.FlatAppearance.BorderColor = primary ? Navy : Color.FromArgb(194, 203, 212);
            return button;
        }
        public static TableLayoutPanel Stack() { return new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Dock = DockStyle.Top, Margin = Padding.Empty, Padding = Padding.Empty }; }
        public static void Field(TableLayoutPanel layout, string label, Control control)
        {
            layout.Controls.Add(Label(label));
            control.Dock = DockStyle.Top; control.Margin = new Padding(0, 0, 0, 16); control.AccessibleName = label;
            layout.Controls.Add(control);
        }
    }
}
