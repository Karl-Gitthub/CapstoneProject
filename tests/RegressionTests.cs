using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject;
using CapstoneProject.Infrastructure;
using CapstoneProject.Services;

internal static class RegressionTests
{
    private static int passed;
    private static string fixtureDirectory;
    private static void Check(string name, Action test)
    {
        test(); passed++; Console.WriteLine("PASS " + name);
    }
    private static void Equal<T>(T expected,T actual) { if(!Equals(expected,actual)) throw new Exception("Expected " + expected + ", got " + actual); }
    private static void Throws<T>(Action test) where T:Exception { try { test(); } catch(T) { return; } throw new Exception("Expected " + typeof(T).Name); }
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Check("decimal average is not truncated",()=>Equal(95.666666666666666666666666667m,GradeCalculator.Average(96,96,95)));
            Check("zero is a valid entered grade",()=>Equal(0m,GradeCalculator.Average(0,0,0)));
            Check("negative grades rejected",()=>Throws<ArgumentOutOfRangeException>(()=>GradeCalculator.Average(-1,90,90)));
            Check("grades above 100 rejected",()=>Throws<ArgumentOutOfRangeException>(()=>GradeCalculator.Average(101,90,90)));
            decimal[] thresholds={0m,74.99m,75m,77.99m,78m,79.99m,80m,83.99m,84m,86.99m,87m,88.99m,89m,91.99m,92m,93.99m,94m,95.99m,96m,100m};
            decimal[] converted={5m,5m,3m,3m,2.75m,2.75m,2.5m,2.5m,2.25m,2.25m,2m,2m,1.75m,1.75m,1.5m,1.5m,1.25m,1.25m,1m,1m};
            for(int i=0;i<thresholds.Length;i++) { int index=i; Check("conversion at " + thresholds[i],()=>Equal(converted[index],GradeCalculator.ConvertAverage(thresholds[index]))); }
            Check("birthday age before birthday",()=>Equal(19,StudentRecord.AgeOn(new DateTime(2006,11,13),new DateTime(2026,11,12))));
            Check("birthday age on birthday",()=>Equal(20,StudentRecord.AgeOn(new DateTime(2006,11,12),new DateTime(2026,11,12))));
            Check("missing student number rejected",()=>{ var s=Student(); s.Id=""; Throws<ArgumentException>(s.Validate); });
            Check("future birth date rejected",()=>{ var s=Student(); s.BirthDate=DateTime.Today.AddDays(1); Throws<ArgumentException>(s.Validate); });
            Check("invalid email rejected",()=>{ var s=Student(); s.Email="invalid"; Throws<ArgumentException>(s.Validate); });
            Check("unsupported program rejected",()=>{ var s=Student(); s.Program="other"; Throws<ArgumentException>(s.Validate); });
            Check("PBKDF2 verifies correct password only",()=>{ var hash=PasswordHash.Create("Synthetic!2026"); Equal(true,PasswordHash.Verify("Synthetic!2026",hash)); Equal(false,PasswordHash.Verify("wrong",hash)); });
            Check("password salts are unique",()=>Equal(false,PasswordHash.Create("same")==PasswordHash.Create("same")));
            Check("malformed hash fails closed",()=>Equal(false,PasswordHash.Verify("anything","pbkdf2$210000$bad$bad")));
            WorkspaceTests.Units(Check);
            if(args.Contains("--unit-only")) { Console.WriteLine(passed+" passed; Access integration and UI rendering explicitly skipped."); return 0; }
            string repo=args[0];
            fixtureDirectory=Path.Combine(repo,"artifacts","tests",Guid.NewGuid().ToString("N")); Directory.CreateDirectory(fixtureDirectory);
            int seedArgument = Array.IndexOf(args, "--seed-directory");
            if (seedArgument < 0 || seedArgument + 1 >= args.Length) throw new ArgumentException("Run scripts/Test.ps1 to generate fresh synthetic test databases.");
            string seedDirectory = args[seedArgument + 1];
            if (!File.Exists(Path.Combine(seedDirectory, "synthetic-seed.json"))) throw new ArgumentException("Synthetic seed manifest is missing.");
            foreach(var file in new[]{"dbStudentManagement.accdb","dbUserManagement.accdb","db_GradingManagement.accdb","dbCourseManagement.accdb"}) File.Copy(Path.Combine(seedDirectory,file),Path.Combine(fixtureDirectory,file));
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR",fixtureDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory",fixtureDirectory);
            Clear("dbStudentManagement.accdb",new[]{"tbl_BSITStud_Credentials","tbl_BSITStud_Address","tbl_BSITStud_ContactDetail","tbl_BSITStud_SchoolBackground","tbl_BSITStud_Detail"});
            Clear("dbUserManagement.accdb",new[]{"tbl_UserLogin"});
            Clear("db_GradingManagement.accdb",new[]{"tbl_Course","tbl_StudentDetail","tbl_FacultyDetail"});
            SchemaMigrator.Ensure();
            using(var db=Database.Open("dbStudentManagement.accdb")) {
                if(!db.GetSchema("Indexes").AsEnumerable().Any(r=>Convert.ToString(r["TABLE_NAME"])=="tbl_BSITStud_Detail" && Convert.ToString(r["COLUMN_NAME"])=="studid" && Convert.ToBoolean(r["PRIMARY_KEY"]))) throw new Exception("Student identifier primary key is required.");
            }
            using(var db=Database.Open("db_GradingManagement.accdb")) Database.Execute(db,null,"ALTER TABLE tbl_Course ALTER COLUMN average DOUBLE");
            var students=new StudentRepository();
            Check("student persists all five related records",()=>{ students.Create(Student()); foreach(var table in new[]{"tbl_BSITStud_Detail","tbl_BSITStud_Address","tbl_BSITStud_ContactDetail","tbl_BSITStud_SchoolBackground","tbl_BSITStud_Credentials"}) Equal(1,Count(table,"SYN-001")); });
            Check("quoted names survive a database round trip",()=>Equal("O'Neil",Convert.ToString(students.Search("SYN-001","").Rows[0]["studlname"])));
            Check("search input cannot alter SQL",()=>Equal(0,students.Search("' OR 1=1 --","").Rows.Count));
            Check("program filters are applied",()=>Equal(0,students.Search("SYN-001","BSBA").Rows.Count));
            Check("duplicate ID rejected without partial writes",()=>{ Throws<ArgumentException>(()=>students.Create(Student())); Equal(1,Count("tbl_BSITStud_Detail","SYN-001")); });
            Check("last-table failure rolls back every student insert",()=>{
                using(var db=Database.Open("dbStudentManagement.accdb")) Database.Execute(db,null,"ALTER TABLE tbl_BSITStud_Credentials DROP COLUMN studcertojt");
                var s=Student(); s.Id="SYN-ROLLBACK";
                Throws<System.Data.OleDb.OleDbException>(()=>students.Create(s));
                foreach(var table in new[]{"tbl_BSITStud_Detail","tbl_BSITStud_Address","tbl_BSITStud_ContactDetail","tbl_BSITStud_SchoolBackground","tbl_BSITStud_Credentials"}) Equal(0,Count(table,s.Id));
                using(var db=Database.Open("dbStudentManagement.accdb")) Database.Execute(db,null,"ALTER TABLE tbl_BSITStud_Credentials ADD COLUMN studcertojt TEXT(255)");
            });
            using(var db=Database.Open("dbUserManagement.accdb")) Database.Execute(db,null,"INSERT INTO tbl_UserLogin(login_user,login_password,login_usertype) VALUES (?,?,?)","synthetic.registrar","Synthetic!2026","REGISTRAR");
            var auth=new AuthenticationService();
            Check("invalid credentials return no session",()=>Equal<UserSession>(null,auth.SignIn("synthetic.registrar","wrong")));
            Check("login injection cannot authenticate",()=>Equal<UserSession>(null,auth.SignIn("' OR 1=1 --","anything")));
            Check("role is derived from the stored account",()=>Equal("REGISTRAR",auth.SignIn("synthetic.registrar","Synthetic!2026").Role));
            Check("legacy password upgrades on successful sign-in",()=>{
                using(var db=Database.Open("dbUserManagement.accdb")) using(var cmd=Database.Command(db,null,"SELECT login_password FROM tbl_UserLogin WHERE login_user=?","synthetic.registrar")) Equal(true,Convert.ToString(cmd.ExecuteScalar()).StartsWith("pbkdf2$"));
                Equal("REGISTRAR",auth.SignIn("synthetic.registrar","Synthetic!2026").Role);
            });
            using(var db=Database.Open("db_GradingManagement.accdb")) {
                Database.Execute(db,null,"INSERT INTO tbl_FacultyDetail(fac_id,fac_fname,fac_lname) VALUES (?,?,?)","FAC-SYN","Synthetic","Faculty");
                Database.Execute(db,null,"INSERT INTO tbl_StudentDetail(stud_id,stud_fname,stud_lname) VALUES (?,?,?)","SYN-001","Synthetic","Student");
                foreach(var semester in new[]{"1","2"}) Database.Execute(db,null,"INSERT INTO tbl_Course(course_code,school_year,sem,stud_id,fac_id,prelim,midterm,final,average,convert_average) VALUES (?,?,?,?,?,?,?,?,?,?)","IT101","2026-2027",semester,"SYN-001","FAC-SYN","80","80","80",80m,"2.50");
            }
            var grades=new GradeRepository(); var enrollment=grades.Search("IT101","2026-2027","FAC-SYN").Rows[0]; int courseId=Convert.ToInt32(enrollment["course_id"]);
            Check("decimal average persists without truncation",()=>{ grades.Save(enrollment,96,96,95); var saved=grades.Search("IT101","2026-2027","FAC-SYN").AsEnumerable().Single(r=>Convert.ToInt32(r["course_id"])==courseId); if(Math.Abs(Convert.ToDecimal(saved["average"])-GradeCalculator.Average(96,96,95))>0.000000001m) throw new Exception("Average lost precision"); Equal("1.25",Convert.ToString(saved["convert_average"])); });
            Check("saving one enrollment preserves the other semester",()=>{ var other=grades.Search("IT101","2026-2027","FAC-SYN").AsEnumerable().Single(r=>Convert.ToInt32(r["course_id"])!=courseId); Equal(80m,Convert.ToDecimal(other["average"])); });
            Check("stale grade update is rejected",()=>Throws<DBConcurrencyException>(()=>grades.Save(enrollment,70,70,70)));
            Check("failing averages save as 5.00",()=>{ var current=grades.Search("IT101","2026-2027","FAC-SYN").AsEnumerable().Single(r=>Convert.ToInt32(r["course_id"])==courseId); grades.Save(current,74,74,74); Equal("5.00",Convert.ToString(grades.Search("IT101","2026-2027","FAC-SYN").AsEnumerable().Single(r=>Convert.ToInt32(r["course_id"])==courseId)["convert_average"])); });
            WorkspaceTests.Integration(Check,repo);
            Application.EnableVisualStyles();
            string screenshots=Path.Combine(repo,"docs","screenshots"); Directory.CreateDirectory(screenshots);
            foreach(float factor in new[]{1f,1.25f,1.5f,2f}) {
                Check("layout render at " + factor,()=>{
                    Capture(new LoginForm(),Path.Combine(screenshots,"login-"+factor.ToString("0.##",System.Globalization.CultureInfo.InvariantCulture)+".png"),factor);
                    Capture(new RegistrarDirectoryForm(true),Path.Combine(screenshots,"students-"+factor.ToString("0.##",System.Globalization.CultureInfo.InvariantCulture)+".png"),factor);
                });
            }
            Capture(new StudentInformationSheet(),Path.Combine(screenshots,"add-student.png"),1);
            Console.WriteLine(passed+" passed. Integration tests used disposable Access copies. UI scaling renders are simulated, not OS DPI changes.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    private static StudentRecord Student() { return new StudentRecord { Id="SYN-001",FirstName="Synthetic",LastName="O'Neil",Program="BSIT",BirthDate=new DateTime(2006,2,1),RegistrationDate=DateTime.Today,Email="synthetic@example.invalid",YearLevel="2" }; }
    private static void Clear(string file,string[] tables) { using(var db=Database.Open(file)) foreach(var table in tables) Database.Execute(db,null,"DELETE FROM ["+table+"]"); }
    private static int Count(string table,string id) { using(var db=Database.Open("dbStudentManagement.accdb")) using(var cmd=Database.Command(db,null,"SELECT COUNT(*) FROM ["+table+"] WHERE studid=?",id)) return Convert.ToInt32(cmd.ExecuteScalar()); }
    private static void Capture(Form form,string output,float factor)
    {
        using(form)
        {
            form.AutoScaleMode=AutoScaleMode.None;
            form.StartPosition=FormStartPosition.Manual; form.Location=new Point(-20000,-20000); form.ShowInTaskbar=false;
            form.Show(); Application.DoEvents();
            form.Scale(new SizeF(factor,factor)); form.PerformLayout(); Application.DoEvents();
            using(var image=new Bitmap(form.Width,form.Height)) {
                form.DrawToBitmap(image,new Rectangle(0,0,image.Width,image.Height));
                var colors=new System.Collections.Generic.HashSet<int>();
                for(int y=40;y<image.Height-10;y+=15) for(int x=10;x<image.Width-10;x+=15) colors.Add(image.GetPixel(x,y).ToArgb());
                if(colors.Count<5) throw new Exception("Form content was not rendered.");
                image.Save(output,System.Drawing.Imaging.ImageFormat.Png);
            }
            form.Hide();
            if(form.Controls.Count==0) throw new Exception("Empty form");
        }
    }
}
