using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;

namespace SpliceExecution
{
    public static class Helper
    {
        public static void RunProcessSync(ProcessStartInfo psi)
        {
            var p = new Process()
            {
                StartInfo = psi
            };
            p.Start();
            p.WaitForExit();
        }

        public static void RunProcessSync(string exec, string args)
        {
            var psi = new ProcessStartInfo(exec, args)
            {
                UseShellExecute = false
            };

            RunProcessSync(psi);
        }

        public static void KillProcess(string processName)
        {
            RunProcessSync("killall", $"-s SIGKILL {processName}");
        }

        public  static void ClearDatabase()
        {
            var connection = new MySqlConnection
            {
                ConnectionString = "server=localhost;user id=mediaware;password=mediaware;SslMode=None;database=ccms;"
            };
            connection.Open();
            var commands = new [] {
                new MySqlCommand("set FOREIGN_KEY_CHECKS=0;", connection),
                new MySqlCommand("truncate breaks;", connection),
                new MySqlCommand("truncate channels;", connection),
                new MySqlCommand("truncate playlists;", connection),
                new MySqlCommand("truncate records;", connection),
                new MySqlCommand("truncate windows;", connection)
            };

            foreach (var cmd in commands)
            {
                cmd.ExecuteNonQuery();
            }
        }
        
        public static void RemoveFiles(IEnumerable<FileInfo> files)
        {
               foreach (var fi in files)
            {
                try
                {
                    fi.Attributes = FileAttributes.Normal;
                    fi.Delete();
                }
                catch {}
            }
        }
    }
}
