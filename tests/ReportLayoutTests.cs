using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CapstoneProject.Presentation;

internal static class ReportLayoutTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static DataTable Grades()
    {
        var table = new DataTable();
        foreach (var column in new[] { "Student number", "Last name", "First name", "Course code", "Academic year", "Semester", "Average", "Equivalent" }) table.Columns.Add(column);
        table.Rows.Add("UAT-BSIT-20261113A-01", "Dela Cruz", "Miguel", "SAMPLE-BSIT-201", "2026-2027", "First semester", "95.5", "1.25");
        table.Rows.Add("UAT-BSIT-20261113A-01", "Dela Cruz", "Miguel", "SAMPLE-BSIT-202", "2026-2027", "First semester", "0", "5.00");
        return table;
    }
    public static void Run(Action<string, Action> check)
    {
        foreach (var factor in new[] { 1, 2 })
        foreach (var caption in new[] { "Registration approvals", "Password assistance" })
        {
            var scale = factor; var text = caption;
            check(text + " fits a wrapped navigation button at scale " + scale, () => {
                using (var form = new Form { Font = new Font("Segoe UI", 10 * scale), AutoScaleMode = AutoScaleMode.None })
                {
                    var factory = typeof(SchoolWorkspace).GetMethod("NavigationButton", BindingFlags.NonPublic | BindingFlags.Static);
                    var button = (Button)factory.Invoke(null, new object[] { text, 160 * scale });
                    form.Controls.Add(button); form.PerformLayout();
                    var required = TextRenderer.MeasureText(text, button.Font, new Size(button.Width - button.Padding.Horizontal, int.MaxValue), TextFormatFlags.WordBreak);
                    Require(button.AutoSize && button.Height >= required.Height + button.Padding.Vertical, "Caption is clipped");
                }
            });
        }
        check("report grid preserves long identifiers at minimum size", () => {
            using (var report = new ReportViewer("Copy of grades", Grades(), "Synthetic layout fixture"))
            {
                report.Size = report.MinimumSize;
                report.ShowInTaskbar = false; report.StartPosition = FormStartPosition.Manual; report.Location = new Point(-20000, -20000);
                report.Show(); Application.DoEvents();
                var grid = report.Controls.Find("Report records", true).OfType<DataGridView>().Single();
                foreach (var column in new[] { "Student number", "Course code" })
                {
                    var width = TextRenderer.MeasureText(Convert.ToString(grid.Rows[0].Cells[column].Value), grid.Font, Size.Empty, TextFormatFlags.NoPadding).Width;
                    Require(grid.Columns[column].Width >= width + grid.DefaultCellStyle.Padding.Horizontal, "Identifier column is too narrow");
                }
                report.Close();
            }
        });
        check("print layout renders complete grade rows with proportional columns", () => {
            using (var report = new ReportViewer("Copy of grades", Grades(), "UAT-BSIT-20261113A-01 · 2026-2027 · First semester"))
            using (var bitmap = new Bitmap(1100, 850))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Segoe UI", 9))
            using (var bold = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                var widths = (float[])typeof(ReportViewer).GetMethod("ColumnWidths", Private).Invoke(report, new object[] { graphics, font, bold, 1010f });
                Require(widths[0] > widths[6] && Math.Abs(widths.Sum() - 1010) < 0.1, "Identifiers did not receive appropriate print width");
                Require(!Page(report, graphics, new Rectangle(45, 45, 1010, 760)) && Position(report) == 2, "Grade report lost rows");
            }
        });
        check("wrapped evaluation rows paginate without skipping records", () => {
            var table = new DataTable(); table.Columns.Add("Course code"); table.Columns.Add("Recorded attempts");
            for (int i = 0; i < 35; i++) table.Rows.Add("SAMPLE-BSBA-" + i, string.Join("; ", Enumerable.Repeat("2026-2027 First semester: 1.25", 12)));
            using (var report = new ReportViewer("Evaluation", table, "Synthetic pagination fixture"))
            using (var bitmap = new Bitmap(600, 500))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                int pages = Pages(report, graphics, new Rectangle(30, 30, 540, 440));
                Require(pages > 1 && Position(report) == 35, "Wrapped evaluation lost rows");
            }
        });
        check("an oversized report row continues across pages and completes", () => {
            var table = new DataTable(); table.Columns.Add("Recorded attempts");
            table.Rows.Add(string.Join(" ", Enumerable.Repeat("A complete recorded course attempt.", 600)));
            table.Rows.Add("Final row must still be rendered.");
            using (var report = new ReportViewer("Evaluation", table, "Synthetic oversized-row fixture"))
            using (var bitmap = new Bitmap(400, 450))
            using (var graphics = Graphics.FromImage(bitmap))
                Require(Pages(report, graphics, new Rectangle(30, 30, 340, 390)) > 1 && Position(report) == 2, "Oversized row did not finish");
        });
        check("empty report finishes on one page", () => {
            var table = Grades(); table.Rows.Clear();
            using (var report = new ReportViewer("Empty report", table, "No saved rows"))
            using (var bitmap = new Bitmap(1100, 850))
            using (var graphics = Graphics.FromImage(bitmap))
                Require(!Page(report, graphics, new Rectangle(45, 45, 1010, 760)) && Position(report) == 0, "Empty report repeated pages");
        });
    }
    private static int Position(ReportViewer report) { return (int)typeof(ReportViewer).GetField("rowIndex", Private).GetValue(report); }
    private static bool Page(ReportViewer report, Graphics graphics, Rectangle bounds)
    {
        return (bool)typeof(ReportViewer).GetMethod("RenderPage", Private).Invoke(report, new object[] { graphics, bounds });
    }
    private static int Pages(ReportViewer report, Graphics graphics, Rectangle bounds)
    {
        for (int page = 1; page <= 100; page++) { graphics.Clear(Color.White); if (!Page(report, graphics, bounds)) return page; }
        throw new Exception("Report pagination made no bounded progress");
    }
}
