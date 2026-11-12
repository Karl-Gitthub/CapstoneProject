using System;
using System.IO;

namespace CapstoneProject.Infrastructure
{
    public static class DemoSession
    {
        public const string Marker = "CapstoneProject demo session v1";
        public static bool Enabled => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CAPSTONE_DEMO_ROOT"));

        // Held for the complete application lifetime, including idle login windows.
        // Several demo processes can share it; reset requires exclusive access.
        public static IDisposable Open()
        {
            if (!Enabled) return null;
            string root = Path.GetFullPath(Environment.GetEnvironmentVariable("CAPSTONE_DEMO_ROOT"));
            string expected = Path.GetFullPath(Path.Combine(root, "data")).TrimEnd(Path.DirectorySeparatorChar);
            if (!string.Equals(Path.GetFullPath(Database.DirectoryPath).TrimEnd(Path.DirectorySeparatorChar), expected, StringComparison.OrdinalIgnoreCase)
                || !File.Exists(Path.Combine(root, ".demo-session"))
                || File.ReadAllText(Path.Combine(root, ".demo-session")).Trim() != Marker)
                throw new InvalidOperationException("The demo directory is not valid. Open it through scripts/Start-Demo.ps1.");
            try
            {
                return new FileStream(Path.Combine(root, ".demo-in-use.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (IOException ex)
            {
                throw new IOException("The demo is being prepared or reset. Try opening it again when that operation finishes.", ex);
            }
        }
    }
}
