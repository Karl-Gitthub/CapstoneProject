using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapstoneProject.Presentation
{
    public partial class SchoolWorkspace
    {
        private static DataTable Project(DataTable source, params string[] columns)
        {
            var result = new DataTable();
            foreach (var column in columns)
                result.Columns.Add(Header(column), source.Columns[column].DataType);
            foreach (DataRow row in source.Rows)
                result.Rows.Add(columns.Select(c => row[c]).ToArray());
            return result;
        }
        private void OpenReport(string name, DataTable table, string context)
        {
            using (var viewer = new ReportViewer(name, table, context))
                viewer.ShowDialog(this);
        }
        private async Task ReportsPage()
        {
            var page = Page("Choose a report and verify its saved source records before printing or exporting.", Auto(), Fill(), Auto());
            var bar = Bar();
            var type = Choice("Report type", "Copy of grades", "Academic history", "Computed grades", "Alphalist");
            var student = Input("Student number", 180);
            student.Text = selectedStudent;
            var year = Input("Academic year", 150);
            year.Text = AcademicYear;
            var semester = Semester();
            bar.Controls.Add(Labeled("Document", type));
            bar.Controls.Add(Labeled("Student number (individual reports)", student));
            bar.Controls.Add(Labeled("Academic year", year));
            bar.Controls.Add(Labeled("Semester", semester));
            page.Controls.Add(bar, 0, 0);
            var grid = Theme.Grid("Report source records");
            page.Controls.Add(grid, 0, 1);
            var actions = Bar();
            var load = Theme.Button("Load saved records", true);
            var preview = Theme.Button("Print / export report");
            actions.Controls.Add(load);
            actions.Controls.Add(preview);
            page.Controls.Add(actions, 0, 2);
            string reportName = "", reportContext = "";
            load.Click += async (s, e) =>
            {
                string selection = type.Text, id = student.Text.Trim(), academicYear = year.Text.Trim(), term = semester.Text;
                await LoadData(() =>
                {
                    if (selection == "Copy of grades" || selection == "Academic history")
                    {
                        if (id == "")
                            throw new ArgumentException("Enter a student number for this report.");
                        service.Student(id);
                    }
                    var rows = service.Grades(selection == "Copy of grades" || selection == "Academic history" ? id : "", selection == "Academic history" ? "" : academicYear, selection == "Academic history" ? "" : term);
                    if (selection == "Alphalist")
                    {
                        var names = rows.AsEnumerable().GroupBy(r => Value(r, "stud_id")).Select(g => g.First()).OrderBy(r => Value(r, "stud_lname")).ThenBy(r => Value(r, "stud_fname"));
                        var list = rows.Clone();
                        foreach (var row in names)
                            list.ImportRow(row);
                        return Project(list, "stud_id", "stud_lname", "stud_fname", "stud_program", "yearlevel");
                    }
                    return Project(rows, "stud_id", "stud_lname", "stud_fname", "course_code", "school_year", "sem", "average", "convert_average");
                }, data => { Bind(grid, data); ReportViewer.SizeReportColumns(grid); reportName = selection; reportContext = (id == "" ? "Program records" : id) + " · " + (selection == "Academic history" ? "Complete recorded history" : academicYear + " · " + term); if (data.Rows.Count == 0) feedback.Text = "No saved records match this report context."; });
            };
            preview.Click += (s, e) => { if (!(grid.DataSource is DataTable table) || reportName == "") { NeedSelection(); return; } OpenReport(reportName, table, reportContext); };
            await Task.CompletedTask;
        }
    }
}
