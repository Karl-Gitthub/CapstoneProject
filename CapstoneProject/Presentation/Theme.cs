using System.Drawing;
using System.Windows.Forms;

namespace CapstoneProject.Presentation
{
    internal static class Theme
    {
        public static readonly Color Navy = ColorTranslator.FromHtml("#12335B");
        public static readonly Color Gold = ColorTranslator.FromHtml("#E7B641");
        public static readonly Color Blue = ColorTranslator.FromHtml("#245B93");
        public static readonly Color Pale = ColorTranslator.FromHtml("#EDF3F9");
        public static readonly Color Attention = ColorTranslator.FromHtml("#FFF5D8");
        public static readonly Color Border = ColorTranslator.FromHtml("#D8E0E8");
        public static readonly Color Ink = ColorTranslator.FromHtml("#203247");
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
        public static TableLayoutPanel Stack()
        {
            return new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Dock = DockStyle.Top, Margin = Padding.Empty, Padding = Padding.Empty };
        }
        public static void Field(TableLayoutPanel layout, string label, Control control)
        {
            layout.Controls.Add(Label(label));
            control.Dock = DockStyle.Top;
            control.Margin = new Padding(0, 0, 0, 16);
            control.AccessibleName = label;
            layout.Controls.Add(control);
        }
        public static DataGridView Grid(string name, bool editable = false)
        {
            var grid = new DataGridView { Name = name, AccessibleName = name, Dock = DockStyle.Fill, ReadOnly = !editable, AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, AutoGenerateColumns = true, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 38, Margin = Padding.Empty };
            grid.RowTemplate.Height = 36;
            grid.GridColor = Border;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Pale;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 239, 248);
            grid.DefaultCellStyle.SelectionForeColor = Ink;
            grid.DefaultCellStyle.Padding = new Padding(8, 2, 8, 2);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Pale;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted;
            grid.DataError += (s, e) => { e.ThrowException = false; if (e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count) grid.Rows[e.RowIndex].ErrorText = "Check the entered value."; };
            return grid;
        }
    }
}
