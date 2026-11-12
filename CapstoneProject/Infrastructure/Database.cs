using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace CapstoneProject.Infrastructure
{
    public static class Database
    {
        // Explicit local working copies only; never fall back to a source database.
        public static string DirectoryPath { get { return Environment.GetEnvironmentVariable("CAPSTONE_DATA_DIR") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); } }
        public static OleDbConnection Open(string file)
        {
            var connection = new OleDbConnection(ConnectionString(file));
            connection.Open();
            return connection;
        }
        public static string ConnectionString(string file)
        {
            if (Path.GetFileName(file) != file) throw new ArgumentException("Use a database filename.");
            var builder = new OleDbConnectionStringBuilder();
            builder.Provider = "Microsoft.ACE.OLEDB.12.0";
            builder.DataSource = Path.Combine(DirectoryPath, file);
            return builder.ConnectionString;
        }
        public static OleDbCommand Command(OleDbConnection connection, OleDbTransaction transaction, string sql, params object[] values)
        {
            var command = new OleDbCommand(sql, connection, transaction);
            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i] ?? DBNull.Value;
                var type = value is DateTime ? OleDbType.Date : value is int ? OleDbType.Integer : value is decimal || value is double ? OleDbType.Double : OleDbType.VarWChar;
                var parameter = command.Parameters.Add("p" + i, type);
                if (type == OleDbType.VarWChar) parameter.Size = Math.Max(1, Convert.ToString(value).Length);
                parameter.Value = value is decimal ? (object)Convert.ToDouble(value) : value;
            }
            return command;
        }
        public static void Execute(OleDbConnection connection, OleDbTransaction transaction, string sql, params object[] values)
        {
            using (var command = Command(connection, transaction, sql, values)) command.ExecuteNonQuery();
        }
        public static string FailureMessage { get { return "The operation could not be completed. Your input has been kept. Check the local database setup and try again."; } }
    }
}
