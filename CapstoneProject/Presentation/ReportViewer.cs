using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CapstoneProject.Presentation
{
    public sealed class ReportViewer : Form
    {
        private readonly DataTable table;
        private readonly string reportTitle, context;
        private int rowIndex, pageNumber;
        public ReportViewer(string title, DataTable table, string context)
        {
            this.table = table.Copy();
            reportTitle = title;
            this.context = context;
            Text = title + " · Internal report";
            Font = new Font("Segoe UI", 10);
            ClientSize = new Size(1120, 760);
            MinimumSize = new Size(860, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(24) };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var heading = Theme.Label(title, 21, true);
            heading.ForeColor = Theme.Navy;
            shell.Controls.Add(heading, 0, 0);
            var note = Theme.Label(context + "\nInternal copy — verify against the school's approved template before official use.", 9);
            note.MaximumSize = new Size(1000, 0);
            shell.Controls.Add(note, 0, 1);
            var grid = Theme.Grid("Report records");
            grid.DataSource = this.table;
            shell.Controls.Add(grid, 0, 2);
            var bar = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(0, 16, 0, 0) };
            var csv = Theme.Button("Export CSV");
            var preview = Theme.Button("Print preview", true);
            var print = Theme.Button("Print…");
            bar.Controls.Add(preview);
            bar.Controls.Add(print);
            bar.Controls.Add(csv);
            shell.Controls.Add(bar, 0, 3);
            Controls.Add(shell);
            csv.Click += (s, e) => { using (var dialog = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv", FileName = title.Replace(' ', '-') + ".csv" }) if (dialog.ShowDialog(this) == DialogResult.OK) { try { File.WriteAllText(dialog.FileName, Csv(this.table), new UTF8Encoding(true)); MessageBox.Show(this, "CSV exported.", "Report export"); } catch (IOException) { MessageBox.Show(this, "The file could not be written. Choose another location or close the existing file.", "Export failed"); } } };
            preview.Click += (s, e) => { using (var document = Document()) using (var dialog = new PrintPreviewDialog { Document = document, Width = 1150, Height = 850, UseAntiAlias = true }) try { dialog.ShowDialog(this); } catch (InvalidPrinterException) { MessageBox.Show(this, "No printer is configured. You can still export CSV.", "Print preview"); } };
            print.Click += (s, e) => { using (var document = Document()) using (var dialog = new PrintDialog { Document = document, UseEXDialog = true }) if (dialog.ShowDialog(this) == DialogResult.OK) try { document.Print(); } catch (InvalidPrinterException) { MessageBox.Show(this, "The selected printer is unavailable.", "Print failed"); } };
        }
        public static string CsvCell(string value)
        {
            value = value ?? "";
            var significant = value.TrimStart();
            if (significant.Length > 0 && "=+-@".Contains(significant[0]))
                value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        public static string Csv(DataTable table)
        {
            var output = new StringBuilder();
            output.AppendLine(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => CsvCell(c.ColumnName))));
            foreach (DataRow row in table.Rows)
                output.AppendLine(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => CsvCell(Convert.ToString(row[c], CultureInfo.InvariantCulture)))));
            return output.ToString();
        }
        public static int RowsPerPage(int height)
        {
            return Math.Max(1, (height - 180) / 30);
        }
        private PrintDocument Document()
        {
            var document = new PrintDocument { DocumentName = reportTitle };
            document.DefaultPageSettings.Landscape = table.Columns.Count > 5;
            document.DefaultPageSettings.Margins = new Margins(45, 45, 45, 45);
            document.BeginPrint += (s, e) => { rowIndex = 0; pageNumber = 0; };
            document.PrintPage += (s, e) =>
            {
                pageNumber++;
                var bounds = e.MarginBounds;
                using (var heading = new Font("Segoe UI", 14, FontStyle.Bold))
                using (var font = new Font("Segoe UI", 9))
                using (var bold = new Font("Segoe UI", 9, FontStyle.Bold))
                using (var brush = new SolidBrush(Theme.Ink))
                using (var navy = new SolidBrush(Theme.Navy))
                using (var pale = new SolidBrush(Theme.Pale))
                using (var pen = new Pen(Theme.Border))
                {
                    var g = e.Graphics;
                    g.DrawString("SYSTEMS PLUS COMPUTER COLLEGE", heading, navy, bounds.Left, bounds.Top);
                    g.DrawString(reportTitle, bold, brush, bounds.Left, bounds.Top + 30);
                    g.DrawString(context, font, brush, new RectangleF(bounds.Left, bounds.Top + 51, bounds.Width, 38));
                    float y = bounds.Top + 100, width = bounds.Width / (float)Math.Max(1, table.Columns.Count);
                    g.FillRectangle(pale, bounds.Left, y, bounds.Width, 30);
                    using (var format = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                            g.DrawString(table.Columns[i].ColumnName, bold, brush, new RectangleF(bounds.Left + i * width + 5, y + 6, width - 10, 25), format);
                        y += 30;
                        int end = Math.Min(table.Rows.Count, rowIndex + RowsPerPage(bounds.Height));
                        for (; rowIndex < end; rowIndex++)
                        {
                            for (int i = 0; i < table.Columns.Count; i++)
                                g.DrawString(Convert.ToString(table.Rows[rowIndex][i]), font, brush, new RectangleF(bounds.Left + i * width + 5, y + 6, width - 10, 25), format);
                            y += 30;
                            g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                        }
                    }
                    g.DrawString("Internal copy · " + DateTime.Today.ToString("dd MMM yyyy") + " · Page " + pageNumber, font, brush, bounds.Left, bounds.Bottom - 22);
                }
                e.HasMorePages = rowIndex < table.Rows.Count;
            };
            return document;
        }
    }
}
