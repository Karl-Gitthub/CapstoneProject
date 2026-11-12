using System;
using System.Globalization;

namespace CapstoneProject.Services
{
    public static class GradeCalculator
    {
        // Period columns have an invariant decimal-string contract. Never accept
        // thousands separators: "8.5" must not become 85 on a comma-decimal PC.
        private const NumberStyles GradeStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        public static bool TryParseStored(string value, out decimal grade)
        {
            return decimal.TryParse(value, GradeStyles, CultureInfo.InvariantCulture, out grade);
        }
        public static bool TryParseInput(string value, out decimal grade)
        {
            return decimal.TryParse(value, GradeStyles, CultureInfo.CurrentCulture, out grade);
        }
        public static decimal ParseStored(string value)
        {
            decimal grade;
            if (!TryParseStored(value, out grade))
                throw new ArgumentException("A grade is not a valid stored decimal. Reload the class and review its grades before saving.");
            Validate(grade);
            return grade;
        }
        public static decimal Average(decimal prelim, decimal midterm, decimal final)
        {
            Validate(prelim);
            Validate(midterm);
            Validate(final);
            return (prelim + midterm + final) / 3m;
        }
        public static decimal ConvertAverage(decimal average)
        {
            Validate(average);
            if (average >= 96m)
                return 1m;
            if (average >= 94m)
                return 1.25m;
            if (average >= 92m)
                return 1.5m;
            if (average >= 89m)
                return 1.75m;
            if (average >= 87m)
                return 2m;
            if (average >= 84m)
                return 2.25m;
            if (average >= 80m)
                return 2.5m;
            if (average >= 78m)
                return 2.75m;
            if (average >= 75m)
                return 3m;
            return 5m;
        }
        public static void Validate(decimal grade)
        {
            if (grade < 0m || grade > 100m)
                throw new ArgumentOutOfRangeException("grade", "Enter a grade from 0 to 100. Missing grades must not be treated as zero.");
        }
    }
}
