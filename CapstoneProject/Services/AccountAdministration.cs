using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class AccountAdministration
    {
        public static readonly string[] Roles = { "REGISTRAR", "BSIT PROGRAM COORDINATOR", "BSBA PROGRAM COORDINATOR", "FACULTY", "ADMINISTRATOR" };
        public static void ValidatePassword(string password)
        {
            if (password == null || password.Length < 12 || password.Length > 128 || !password.Any(char.IsLetter) || !password.Any(char.IsDigit))
                throw new ArgumentException("Use 12–128 characters, including letters and numbers.");
        }
        private static void Username(string username)
        {
            if (!Regex.IsMatch(username ?? "", "^[A-Za-z0-9._-]{3,60}$"))
                throw new ArgumentException("Use 3–60 letters, numbers, periods, hyphens or underscores for the username.");
        }
        public void Request(string username, string name, string email, string department, string reason, string password)
        {
            username = (username ?? "").Trim();
            Username(username);
            ValidatePassword(password);
            if (string.IsNullOrWhiteSpace(name) || name.Length > 255 || string.IsNullOrWhiteSpace(reason) || reason.Length > 1000 || department.Length > 80)
                throw new ArgumentException("Enter your name, department and a reason of up to 1,000 characters.");
            try
            {
                if (new MailAddress(email).Address != email)
                    throw new FormatException();
            }
            catch (FormatException) { throw new ArgumentException("Enter a valid staff email address."); }
            if (email.Length > 255)
                throw new ArgumentException("Email address is too long.");
            string hash = PasswordHash.Create(password);
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            {
                // A neutral response avoids disclosing accounts and prevents overwriting
                // another person's pending request or chosen password.
                if (Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE login_user=?", username).Rows.Count > 0 || Database.Query(db, transaction, "SELECT request_id FROM app_access_requests WHERE username_text=? AND request_state='Pending'", username).Rows.Count > 0)
                    return;
                Database.Execute(db, transaction, "INSERT INTO app_access_requests(username_text,full_name,email_text,department_text,reason_text,password_hash,request_state,review_note,created_utc) VALUES (?,?,?,?,?,?,?,'',?)", username, name.Trim(), email, department, reason, hash, "Pending", DateTime.UtcNow);
                SchemaMigrator.Audit(db, transaction, username, "Access requested", username);
                transaction.Commit();
            }
        }
        public DataTable Accounts(UserSession user)
        {
            Admin(user);
            return Database.Query(SchemaMigrator.Accounts, "SELECT ID,login_user,login_fullname,login_usertype,app_faculty_id,app_disabled,app_version FROM tbl_UserLogin ORDER BY login_user");
        }
        public DataTable Requests(UserSession user)
        {
            Admin(user);
            return Database.Query(SchemaMigrator.Accounts, "SELECT request_id,username_text,full_name,email_text,department_text,reason_text,request_state,review_note,created_utc FROM app_access_requests WHERE request_state='Pending' ORDER BY created_utc");
        }
        public void RequestRecovery(string username, string email)
        {
            Username(username);
            try
            {
                if (new MailAddress(email).Address != email)
                    throw new FormatException();
            }
            catch (FormatException) { throw new ArgumentException("Enter a valid staff email address."); }
            if (email.Length > 255)
                throw new ArgumentException("Email address is too long.");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            {
                if (Database.Query(db, transaction, "SELECT request_id FROM app_recovery_requests WHERE username_text=? AND request_state='Pending'", username).Rows.Count > 0)
                    return;
                Database.Execute(db, transaction, "INSERT INTO app_recovery_requests(username_text,email_text,request_state,review_note,created_utc) VALUES (?,?,'Pending','',?)", username, email, DateTime.UtcNow);
                transaction.Commit();
            }
        }
        public DataTable RecoveryRequests(UserSession user)
        {
            Admin(user);
            return Database.Query(SchemaMigrator.Accounts, "SELECT request_id,username_text,email_text,created_utc FROM app_recovery_requests WHERE request_state='Pending' ORDER BY created_utc");
        }
        public void ResolveRecovery(UserSession user, int requestId, bool approve, string password, string note)
        {
            Admin(user);
            if (approve)
                ValidatePassword(password);
            if (string.IsNullOrWhiteSpace(note) || note.Length > 1000)
                throw new ArgumentException("Record how identity was verified, or why the request was rejected (maximum 1,000 characters).");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            {
                LockAdmin(db, transaction, user);
                var requests = Database.Query(db, transaction, "SELECT username_text FROM app_recovery_requests WHERE request_id=? AND request_state='Pending'", requestId);
                if (requests.Rows.Count != 1)
                    throw new DBConcurrencyException("The request has already been resolved.");
                string username = Convert.ToString(requests.Rows[0][0]);
                if (approve)
                {
                    var accounts = Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE login_user=?", username);
                    if (accounts.Rows.Count != 1)
                        throw new ArgumentException("No unique account matches this request. Reject it with a review note.");
                    if (Convert.ToInt32(accounts.Rows[0][0]) == user.AccountId)
                        throw new ArgumentException("Use Change password for your own signed-in administrator account.");
                    Database.Execute(db, transaction, "UPDATE tbl_UserLogin SET login_password=?,app_must_change=?,app_version=app_version+1 WHERE ID=?", PasswordHash.Create(password), true, Convert.ToInt32(accounts.Rows[0][0]));
                }
                using (var command = Database.Command(db, transaction, "UPDATE app_recovery_requests SET request_state=?,review_note=? WHERE request_id=? AND request_state='Pending'", approve ? "Resolved" : "Rejected", note, requestId))
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The request changed. Reload the list.");
                SchemaMigrator.Audit(db, transaction, user.Username, approve ? "Password assistance resolved" : "Password assistance rejected", username);
                transaction.Commit();
            }
        }
        private static void Admin(UserSession user)
        {
            AccessPolicy.Current(user);
            AccessPolicy.Require(user.Role == "ADMINISTRATOR");
        }
        private static void LockAdmin(System.Data.OleDb.OleDbConnection db, System.Data.OleDb.OleDbTransaction transaction, UserSession user)
        {
            Database.Execute(db, transaction, "UPDATE app_security_lock SET revision=revision+1 WHERE lock_key='admin'");
            var rows = Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE ID=? AND login_usertype='ADMINISTRATOR' AND app_version=? AND (app_disabled=FALSE OR app_disabled IS NULL)", user.AccountId, user.Version);
            if (rows.Rows.Count != 1)
                throw new UnauthorizedAccessException("Administrator access changed. Sign in again.");
        }
        public void Review(UserSession user, int requestId, bool approve, string role, string facultyId, string note)
        {
            Admin(user);
            if (!Roles.Contains(role))
                throw new ArgumentException("Select a supported role.");
            if (!approve && string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Enter a reason before rejecting the request.");
            if (note.Length > 1000 || facultyId.Length > 255)
                throw new ArgumentException("The entered value is too long.");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            {
                LockAdmin(db, transaction, user);
                var rows = Database.Query(db, transaction, "SELECT * FROM app_access_requests WHERE request_id=? AND request_state='Pending'", requestId).Rows;
                if (rows.Count != 1)
                    throw new DBConcurrencyException("This request was already reviewed. Reload the list.");
                var row = rows[0];
                string username = Convert.ToString(row["username_text"]);
                if (approve)
                {
                    if (Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE login_user=?", username).Rows.Count > 0)
                        throw new ArgumentException("That username is already in use.");
                    if (role == "FACULTY")
                    {
                        if (Database.Query(SchemaMigrator.Academic, "SELECT ID FROM tbl_FacultyDetail WHERE fac_id=?", facultyId).Rows.Count != 1)
                            throw new ArgumentException("Select an existing faculty identifier for a faculty account.");
                    }
                    Database.Execute(db, transaction, "INSERT INTO app_username_keys(username_text) VALUES (?)", username);
                    Database.Execute(db, transaction, "INSERT INTO tbl_UserLogin(login_user,login_password,login_usertype,login_fullname,app_disabled,app_must_change,app_faculty_id,app_version) VALUES (?,?,?,?,?,?,?,0)", username, row["password_hash"], role, row["full_name"], false, false, facultyId);
                }
                using (var command = Database.Command(db, transaction, "UPDATE app_access_requests SET request_state=?,review_note=?,password_hash=NULL WHERE request_id=? AND request_state='Pending'", approve ? "Approved" : "Rejected", note, requestId))
                    if (command.ExecuteNonQuery() != 1)
                        throw new DBConcurrencyException("This request changed. Reload the list.");
                SchemaMigrator.Audit(db, transaction, user.Username, approve ? "Access approved: " + role : "Access rejected", username);
                transaction.Commit();
            }
        }
        public void SetAccess(UserSession user, int accountId, int version, string role, string facultyId, bool disabled)
        {
            Admin(user);
            if (!Roles.Contains(role))
                throw new ArgumentException("Select a supported role.");
            if (accountId == user.AccountId && (disabled || role != "ADMINISTRATOR"))
                throw new ArgumentException("You cannot disable or demote your own administrator account.");
            if (role == "FACULTY" && Database.Query(SchemaMigrator.Academic, "SELECT ID FROM tbl_FacultyDetail WHERE fac_id=?", facultyId).Rows.Count != 1)
                throw new ArgumentException("Select a valid faculty identifier.");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            using (var command = Database.Command(db, transaction, "UPDATE tbl_UserLogin SET login_usertype=?,app_faculty_id=?,app_disabled=?,app_version=app_version+1 WHERE ID=? AND app_version=?", role, facultyId, disabled, accountId, version))
            {
                LockAdmin(db, transaction, user);
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The account changed. Reload before editing access.");
                SchemaMigrator.Audit(db, transaction, user.Username, "Account access changed", accountId.ToString());
                transaction.Commit();
                if (accountId == user.AccountId)
                    user.Version++;
            }
        }
        public void ResetPassword(UserSession user, int accountId, int version, string password)
        {
            Admin(user);
            ValidatePassword(password);
            if (accountId == user.AccountId)
                throw new ArgumentException("Use Change password for your own account.");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            using (var command = Database.Command(db, transaction, "UPDATE tbl_UserLogin SET login_password=?,app_must_change=?,app_version=app_version+1 WHERE ID=? AND app_version=?", PasswordHash.Create(password), true, accountId, version))
            {
                LockAdmin(db, transaction, user);
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The account changed. Reload before resetting the password.");
                SchemaMigrator.Audit(db, transaction, user.Username, "Temporary password set", accountId.ToString());
                transaction.Commit();
            }
        }
        public void ChangePassword(UserSession user, string current, string replacement)
        {
            ValidatePassword(replacement);
            var verified = new AuthenticationService().SignIn(user.Username, current);
            if (verified == null || verified.AccountId != user.AccountId || verified.Version != user.Version)
                throw new ArgumentException("Your current password could not be verified.");
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            using (var command = Database.Command(db, transaction, "UPDATE tbl_UserLogin SET login_password=?,app_must_change=?,app_version=app_version+1 WHERE ID=? AND app_version=?", PasswordHash.Create(replacement), false, user.AccountId, user.Version))
            {
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The account changed. Sign in again.");
                SchemaMigrator.Audit(db, transaction, user.Username, "Password changed", user.Username);
                transaction.Commit();
                user.Version++;
                user.MustChangePassword = false;
            }
        }
        public static void Bootstrap(string username, string name, string password)
        {
            Username(username);
            ValidatePassword(password);
            using (var db = Database.Open(SchemaMigrator.Accounts))
            using (var transaction = db.BeginTransaction())
            {
                Database.Execute(db, transaction, "UPDATE app_security_lock SET revision=revision+1 WHERE lock_key='admin'");
                if (Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE login_usertype='ADMINISTRATOR'").Rows.Count > 0)
                    throw new InvalidOperationException("An administrator already exists. Use the authenticated account administration screen.");
                if (Database.Query(db, transaction, "SELECT ID FROM tbl_UserLogin WHERE login_user=?", username).Rows.Count > 0)
                    throw new ArgumentException("Choose a username that is not already in use.");
                Database.Execute(db, transaction, "INSERT INTO app_username_keys(username_text) VALUES (?)", username);
                Database.Execute(db, transaction, "INSERT INTO tbl_UserLogin(login_user,login_password,login_usertype,login_fullname,app_disabled,app_must_change,app_version) VALUES (?,?,?,?,?,?,0)", username, PasswordHash.Create(password), "ADMINISTRATOR", name, false, false);
                SchemaMigrator.Audit(db, transaction, "Local setup", "First administrator provisioned", username);
                transaction.Commit();
            }
        }
    }
}
