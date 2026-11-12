using System;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    internal static class GradeBatch
    {
        public static void Save(IEnumerable<DataRow> rows, string actor, string faculty)
        {
            using (var db = Database.Open(SchemaMigrator.Academic))
            using (var transaction = db.BeginTransaction())
            {
                foreach (var row in rows)
                {
                    var periods = new List<decimal?>();
                    foreach (var field in new[] { "prelim", "midterm", "final" })
                        periods.Add(string.IsNullOrWhiteSpace(Convert.ToString(row[field])) ? (decimal?)null : GradeCalculator.ParseStored(Convert.ToString(row[field])));
                    decimal? average = periods.TrueForAll(x => x.HasValue) ? GradeCalculator.Average(periods[0].Value, periods[1].Value, periods[2].Value) : (decimal?)null;
                    var values = new List<object> { periods[0]?.ToString(CultureInfo.InvariantCulture), periods[1]?.ToString(CultureInfo.InvariantCulture), periods[2]?.ToString(CultureInfo.InvariantCulture), average.HasValue ? (object)average.Value : DBNull.Value, average.HasValue ? GradeCalculator.ConvertAverage(average.Value).ToString("0.00", CultureInfo.InvariantCulture) : null, Convert.ToInt32(row["course_id", DataRowVersion.Original]) };
                    string sql = "UPDATE tbl_Course SET prelim=?,midterm=?,final=?,average=?,convert_average=? WHERE course_id=?";
                    foreach (var field in new[] { "prelim", "midterm", "final", "average", "convert_average", "fac_id", "school_year", "sem", "stud_id" })
                    {
                        sql += " AND ([" + field + "]=? OR ([" + field + "] IS NULL AND ? IS NULL))";
                        values.Add(row[field, DataRowVersion.Original]);
                        values.Add(row[field, DataRowVersion.Original]);
                    }
                    if (faculty != null)
                    {
                        sql += " AND fac_id=?";
                        values.Add(faculty);
                    }
                    using (var command = Database.Command(db, transaction, sql, values.ToArray()))
                    if (command.ExecuteNonQuery() != 1)
                        throw new DBConcurrencyException("A grade record or teaching assignment changed. Reload the class. None of this batch was saved.");
                    SchemaMigrator.Audit(db, transaction, actor, "Grades saved", Convert.ToString(row["course_id"]));
                }
                transaction.Commit();
            }
        }
    }
}
