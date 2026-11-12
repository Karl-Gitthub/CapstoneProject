using System;

namespace CapstoneProject.Services
{
    public static class GradeCalculator
    {
        public static decimal Average(decimal prelim, decimal midterm, decimal final)
        {
            Validate(prelim); Validate(midterm); Validate(final);
            return (prelim + midterm + final) / 3m;
        }
        public static decimal ConvertAverage(decimal average)
        {
            Validate(average);
            if (average >= 96m) return 1m;
            if (average >= 94m) return 1.25m;
            if (average >= 92m) return 1.5m;
            if (average >= 89m) return 1.75m;
            if (average >= 87m) return 2m;
            if (average >= 84m) return 2.25m;
            if (average >= 80m) return 2.5m;
            if (average >= 78m) return 2.75m;
            if (average >= 75m) return 3m;
            return 5m;
        }
        public static void Validate(decimal grade)
        {
            if (grade < 0m || grade > 100m) throw new ArgumentOutOfRangeException("grade", "Enter a grade from 0 to 100. Missing grades must not be treated as zero.");
        }
    }
}
