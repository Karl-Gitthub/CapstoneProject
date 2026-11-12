using System;
using System.Data;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class StudentRepository
    {
        public DataTable Search(string search, string program)
        {
            using (var connection = Database.Open("dbStudentManagement.accdb"))
            using (var command = Database.Command(connection, null,
                "SELECT TOP 200 studid, studfname, studmname, studlname, studprogram, yearlevel FROM tbl_BSITStud_Detail WHERE (InStr(1, studid, ?) > 0 OR InStr(1, studfname, ?) > 0 OR InStr(1, studlname, ?) > 0) AND (? = '' OR studprogram = ?) ORDER BY studlname, studfname",
                search.Trim(), search.Trim(), search.Trim(), program, program))
            using (var adapter = new System.Data.OleDb.OleDbDataAdapter(command))
            {
                var table = new DataTable(); adapter.Fill(table); return table;
            }
        }
        public void Create(StudentRecord s)
        {
            s.Validate();
            using (var connection = Database.Open("dbStudentManagement.accdb"))
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = Database.Command(connection, transaction, "SELECT COUNT(*) FROM tbl_BSITStud_Detail WHERE studid = ?", s.Id))
                    if (Convert.ToInt32(command.ExecuteScalar()) != 0) throw new ArgumentException("That student number already exists.");
                Database.Execute(connection, transaction, "INSERT INTO tbl_BSITStud_Detail (studid,studfname,studmname,studlname,studsuffix,studdatereg,studbdate,studage,studnationality,studgender,studprogram,yearlevel) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)",
                    s.Id, s.FirstName, s.MiddleName, s.LastName, s.Suffix, s.RegistrationDate.Date, s.BirthDate.Date, s.Age, s.Nationality, s.Gender, s.Program, s.YearLevel);
                Database.Execute(connection, transaction, "INSERT INTO tbl_BSITStud_Address (studid,studaddnum,studaddstreet,studaddbrgy,studaddcity,studaddprovince,studaddzip) VALUES (?,?,?,?,?,?,?)",
                    s.Id,s.StreetNumber,s.Street,s.Barangay,s.City,s.Province,s.Zip);
                Database.Execute(connection, transaction, "INSERT INTO tbl_BSITStud_ContactDetail (studid,studcontactnum,studcontactadd) VALUES (?,?,?)", s.Id,s.Phone,s.Email);
                Database.Execute(connection, transaction, "INSERT INTO tbl_BSITStud_SchoolBackground (studid,studprevprim_schoolname,studprev_sy,studprevsec_schoolname,studprevsec_sy) VALUES (?,?,?,?,?)", s.Id,s.PrimarySchool,s.PrimaryYear,s.SecondarySchool,s.SecondaryYear);
                Database.Execute(connection, transaction, "INSERT INTO tbl_BSITStud_Credentials (studid,studthesis,studcredgm,studcredhd,studhsdiploma,studbirthcert,studcopyofgrades,studcredtor,studcredf137,studcredf138,studcertojt) VALUES (?,?,?,?,?,?,?,?,?,?,?)",
                    s.Id,s.Thesis,s.GoodMoral,s.HonorableDismissal,s.Diploma,s.BirthCertificate,s.CopyOfGrades,s.Transcript,s.Form137,s.Form138,s.Ojt);
                transaction.Commit();
            }
        }
    }
}
