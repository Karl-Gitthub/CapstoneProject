using System;
using System.Data;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class GradeRepository
    {
        public DataTable Search(string course, string year, string faculty)
        {
            using (var connection = Database.Open("db_GradingManagement.accdb"))
            using (var command = Database.Command(connection, null,
                // ACE cannot join Memo columns directly. Use short-text expressions for
                // the join, then compare full identifiers to prevent prefix collisions.
                "SELECT c.stud_id,s.stud_fname,s.stud_mname,s.stud_lname,c.prelim,c.midterm,c.final,c.average,c.convert_average,c.course_id,c.sem,c.school_year FROM tbl_Course c INNER JOIN tbl_StudentDetail s ON Left(c.stud_id,255)=Left(s.stud_id,255) WHERE c.stud_id=s.stud_id AND c.course_code=? AND c.school_year=? AND c.fac_id=? ORDER BY s.stud_lname", course, year, faculty))
            using (var adapter = new System.Data.OleDb.OleDbDataAdapter(command))
            {
                var table = new DataTable(); adapter.Fill(table); return table;
            }
        }
        // Snapshot contains the selected enrollment ID and the values read from storage.
        // A concurrent edit or deleted row causes a rollback; never overwrite another term.
        public void Save(DataRow snapshot, decimal prelim, decimal midterm, decimal final)
        {
            decimal average = GradeCalculator.Average(prelim, midterm, final);
            using (var connection = Database.Open("db_GradingManagement.accdb"))
            using (var transaction = connection.BeginTransaction())
            using (var command = Database.Command(connection, transaction,
                "UPDATE tbl_Course SET prelim=?,midterm=?,final=?,average=?,convert_average=? WHERE course_id=? AND (prelim=? OR (prelim IS NULL AND ? IS NULL)) AND (midterm=? OR (midterm IS NULL AND ? IS NULL)) AND (final=? OR (final IS NULL AND ? IS NULL)) AND (average=? OR (average IS NULL AND ? IS NULL)) AND (convert_average=? OR (convert_average IS NULL AND ? IS NULL))",
                prelim.ToString(System.Globalization.CultureInfo.InvariantCulture),midterm.ToString(System.Globalization.CultureInfo.InvariantCulture),final.ToString(System.Globalization.CultureInfo.InvariantCulture),average,GradeCalculator.ConvertAverage(average).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                Convert.ToInt32(snapshot["course_id"]),snapshot["prelim"],snapshot["prelim"],snapshot["midterm"],snapshot["midterm"],snapshot["final"],snapshot["final"],snapshot["average"],snapshot["average"],snapshot["convert_average"],snapshot["convert_average"]))
            {
                if (command.ExecuteNonQuery() != 1) throw new DBConcurrencyException("This enrollment changed. Search again before saving your grades.");
                transaction.Commit();
            }
        }
    }
}
