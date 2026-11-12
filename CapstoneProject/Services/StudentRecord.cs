using System;
using System.Net.Mail;

namespace CapstoneProject.Services
{
    public sealed class StudentRecord
    {
        public string Id, FirstName, MiddleName, LastName, Suffix, Nationality, Gender, Program, YearLevel;
        public string StreetNumber, Street, Barangay, City, Province, Zip, Phone, Email;
        public string PrimarySchool, PrimaryYear, SecondarySchool, SecondaryYear;
        public string Thesis, GoodMoral, HonorableDismissal, Diploma, BirthCertificate, CopyOfGrades, Transcript, Form137, Form138, Ojt;
        public DateTime BirthDate, RegistrationDate;
        public int Version;
        public int Age
        {
            get
            {
                return AgeOn(BirthDate, RegistrationDate);
            }
        }
        public static int AgeOn(DateTime birthDate, DateTime date)
        {
            int age = date.Year - birthDate.Year;
            return birthDate.Date > date.Date.AddYears(-age) ? age - 1 : age;
        }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("Student number, first name and last name are required.");
            if (Program != "BSIT" && Program != "BSBA")
                throw new ArgumentException("Select BSIT or BSBA.");
            if (BirthDate.Year < 1900 || RegistrationDate.Year < 1900 || BirthDate.Date > RegistrationDate.Date || RegistrationDate.Date > DateTime.Today)
                throw new ArgumentException("Check the birth date and registration date.");
            foreach (var field in GetType().GetFields())
                if (field.FieldType == typeof(string) && ((string)field.GetValue(this) ?? "").Length > 255)
                    throw new ArgumentException("Fields must be 255 characters or fewer.");
            if (!string.IsNullOrWhiteSpace(Email))
            {
                MailAddress address;
                try
                {
                    address = new MailAddress(Email);
                }
                catch (FormatException) { throw new ArgumentException("Enter a valid email address."); }
                if (address.Address != Email)
                    throw new ArgumentException("Enter a valid email address.");
            }
        }
    }
}
