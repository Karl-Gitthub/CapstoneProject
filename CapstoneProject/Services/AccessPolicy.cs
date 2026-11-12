using System;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public static class AccessPolicy
    {
        public static string Program(UserSession user)
        {
            return user.Role == "BSIT PROGRAM COORDINATOR" ? "BSIT" : user.Role == "BSBA PROGRAM COORDINATOR" ? "BSBA" : "";
        }
        public static bool Registrar(UserSession user)
        {
            return user.Role == "REGISTRAR" || user.Role == "ADMINISTRATOR";
        }
        public static bool Coordinator(UserSession user)
        {
            return Program(user) != "";
        }
        public static void Current(UserSession user)
        {
            if (user == null)
                throw new UnauthorizedAccessException("Sign in to continue.");
            var rows = Database.Query(SchemaMigrator.Accounts, "SELECT ID,login_usertype,app_disabled,app_version FROM tbl_UserLogin WHERE login_user=?", user.Username).Rows;
            if (rows.Count != 1 || Convert.ToInt32(rows[0]["ID"]) != user.AccountId || Convert.ToString(rows[0]["login_usertype"]) != user.Role || (rows[0]["app_disabled"] != DBNull.Value && Convert.ToBoolean(rows[0]["app_disabled"])) || (rows[0]["app_version"] == DBNull.Value ? 0 : Convert.ToInt32(rows[0]["app_version"])) != user.Version)
                throw new UnauthorizedAccessException("Your account access changed. Sign out and sign in again.");
            if (user.MustChangePassword)
                throw new UnauthorizedAccessException("Change your temporary password before opening records.");
        }
        public static void Require(bool condition)
        {
            if (!condition)
                throw new UnauthorizedAccessException("Your account does not have permission for this action.");
        }
        public static void Student(UserSession user, string program)
        {
            Require(Registrar(user) || Coordinator(user) && Program(user) == program);
        }
    }
}
