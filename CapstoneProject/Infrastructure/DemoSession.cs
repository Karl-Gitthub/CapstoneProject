using System;
using System.IO;

namespace CapstoneProject.Infrastructure
{
    public static class DemoSession
    {
        public const string Marker = "CapstoneProject demo session v1";
        private static string Root
        {
            get
            {
                string configured = Environment.GetEnvironmentVariable("CAPSTONE_DEMO_ROOT");
                if (!string.IsNullOrWhiteSpace(configured)) return Path.GetFullPath(configured);
                var data = new DirectoryInfo(Path.GetFullPath(Database.DirectoryPath));
                // Discover managed demo data even through the ordinary launcher or
                // direct executable. Staging and archived sets are not active data.
                return string.Equals(data.Name, "data", StringComparison.OrdinalIgnoreCase)
                    && data.Parent != null && File.Exists(Path.Combine(data.Parent.FullName, ".demo-session"))
                    ? data.Parent.FullName : null;
            }
        }
        public static bool Enabled => Root != null;

        // Held for the complete application lifetime, including idle login windows.
        // Several demo processes can share it; reset requires exclusive access.
        public static IDisposable Open()
        {
            string root = Root;
            if (root == null) return null;
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
