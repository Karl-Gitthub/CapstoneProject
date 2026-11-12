using System;
using System.Security.Cryptography;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class UserSession
    {
        public string Username { get; private set; }
        public string Role { get; private set; }
        public UserSession(string username, string role) { Username = username; Role = role; }
    }
    public sealed class AuthenticationService
    {
        public UserSession SignIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;
            using (var connection = Database.Open("dbUserManagement.accdb"))
            {
                int id; string stored, role;
                using (var command = Database.Command(connection, null, "SELECT ID, login_password, login_usertype FROM tbl_UserLogin WHERE login_user = ?", username.Trim()))
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    id = Convert.ToInt32(reader[0]); stored = Convert.ToString(reader[1]); role = Convert.ToString(reader[2]);
                    if (reader.Read()) return null; // Ambiguous account names fail closed.
                }
                if (!PasswordHash.Verify(password, stored)) return null;
                if (role != "REGISTRAR" && role != "BSIT PROGRAM COORDINATOR" && role != "BSBA PROGRAM COORDINATOR" && role != "FACULTY") return null;
                if (!stored.StartsWith("pbkdf2$", StringComparison.Ordinal))
                    Database.Execute(connection, null, "UPDATE tbl_UserLogin SET login_password = ? WHERE ID = ? AND login_password = ?", PasswordHash.Create(password), id, stored);
                return new UserSession(username.Trim(), role);
            }
        }
    }
    public static class PasswordHash
    {
        private const int Iterations = 210000;
        public static string Create(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                return "pbkdf2$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(derive.GetBytes(32));
        }
        public static bool Verify(string password, string stored)
        {
            if (!stored.StartsWith("pbkdf2$", StringComparison.Ordinal)) return ConstantEquals(System.Text.Encoding.UTF8.GetBytes(password), System.Text.Encoding.UTF8.GetBytes(stored));
            try
            {
                var parts = stored.Split('$');
                int iterations;
                if (parts.Length != 4 || !int.TryParse(parts[1], out iterations) || iterations < 100000 || iterations > 1000000) return false;
                using (var derive = new Rfc2898DeriveBytes(password, Convert.FromBase64String(parts[2]), iterations, HashAlgorithmName.SHA256))
                    return ConstantEquals(derive.GetBytes(32), Convert.FromBase64String(parts[3]));
            }
            catch (FormatException) { return false; }
            catch (ArgumentException) { return false; }
        }
        private static bool ConstantEquals(byte[] a, byte[] b)
        {
            int difference = a.Length ^ b.Length;
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++) difference |= a[i] ^ b[i];
            return difference == 0;
        }
    }
}
