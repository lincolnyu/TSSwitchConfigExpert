#region EXEC_TYPES

//#define SPLICER_DEBUG
#define STREAMWRITER_DEBUG
#define INGESTOR_DEBUG

#endregion

#region User switches

//#define KILLALL
//#define KATE_SCHS
//#define KATE_CONFIGS
//#define CLEAR_DATABASE
//#define KILL_VLCS
//#define TEST_WRITE_CONFIG

//#define INGEST_ONLY
//#define STREAM_INPUT_ONLY
//#define RUN_INPUT_VLC_ONLY
//#define RUN_SPLICE_VLCS_ONLY
//#define SPLICE_ONLY
//#define RUN_ALL_BUT_TAIL

#endregion

#if INGEST_ONLY
#   define INGEST
#   undef STREAM_INPTUS
#   undef RUN_INPUT_VLCS

#   undef SPLICE
#   undef ADD_SPLICE_VLCS
#   undef TAIL_DEBUG
#elif STREAM_INPUT_ONLY
#   undef INGEST
#   define STREAM_INPTUS
#   define RUN_INPUT_VLCS
#   undef SPLICE
#   undef ADD_SPLICE_VLCS
#   undef TAIL_DEBUG
#elif RUN_INPUT_VLC_ONLY
#   undef INGEST
#   undef STREAM_INPTUS
#   define RUN_INPUT_VLCS
#   undef SPLICE
#   undef ADD_SPLICE_VLCS
#   undef TAIL_DEBUG
#elif SPLICE_ONLY
#   undef INGEST
#   undef STREAM_INPTUS
#   undef RUN_INPUT_VLCS
#   define SPLICE
#   define ADD_SPLICE_VLCS
#   undef TAIL_DEBUG
#elif RUN_SPLICE_VLCS_ONLY
#   undef INGEST
#   undef STREAM_INPTUS
#   undef RUN_INPUT_VLCS
#   undef SPLICE
#   define ADD_SPLICE_VLCS
#   undef TAIL_DEBUG
#else
#   define INGEST
#   define STREAM_INPTUS
#   define RUN_INPUT_VLCS
#   define SPLICE
#   define ADD_SPLICE_VLCS
#   if RUN_ALL_BUT_TAIL
#       undef TAIL_DEBUG
#   else
#       define TAIL_DEBUG
#   endif
#endif

#region User overrides
#endregion

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Xml;
using SpliceConfiguration;
using SpliceExecution;

namespace TSSWitchConfigExpert
{
    class Program
    {
        static int SpliceVlcInterval = 3;

        const string DebugConfig = "debug64";
        const string ReleaseConfig = "release64-nolicense";

        const string VlcExecName = "vlc"; // cvlc

        static string StreamWriterBuildConfig = 
#if STREAMWRITER_DEBUG
            DebugConfig;
#else
            ReleaseConfig;
#endif
       
        static string SplicerBuildConfig = 
#if SPLICER_DEBUG
            DebugConfig;
#else
            ReleaseConfig;
#endif

        static string IngestorBuildConfig =
#if INGESTOR_DEBUG
            DebugConfig;
#else
            ReleaseConfig;
#endif

        static string BaseFolder = "/Data/video_files/multitriggers";
        static string StreamwriterExec = $"/code/splice/src/application/splicer2server/build/xenial_testing/{StreamWriterBuildConfig}/application/tools/streamwriter";
        static string SourceFolder = Path.Combine(BaseFolder, "Source");
        static string HDSource = Path.Combine(SourceFolder, "HBOAdriaHD_6AM.ts");
        static string SDSource = Path.Combine(SourceFolder, "HBOAdriaSD_6AM.ts");

        static string SplicerBuildFolder = $"/code/splice_copy2/src/application/splicer2server/build/xenial_testing/{SplicerBuildConfig}";
        static string SplicerExec = Path.Combine(SplicerBuildFolder, "application/splicer2server/splicer2server_app/splicer2server_app");
        //static string ConfigFolder = BaseFolder;
        static string ConfigFolder = "/Data/config/config.git";
        static string ConfigPath = Path.Combine(ConfigFolder, "splicer2server.xml");

        static string IngestorBuildFolder = $"/code/splice_copy2/src/application/splicer2server/build/xenial_testing/{IngestorBuildConfig}";
        static string IngestorExecName = "ccms_ingestor";
        static string IngestorExec = Path.Combine(IngestorBuildFolder, "application/splicer2server/tools/Ingestor/CCMS", IngestorExecName);

        static string SCHBackupFolder = Path.Combine(BaseFolder, "SCHBak");

        static string CCMSIngestFolder = "/opt/mediaware/ingest/ccms";

        static string[] OutputAddresses = new[] 
        {
            "127.0.0.1:9000",
            "127.0.0.1:9010",
            "127.0.0.1:9020",
            "127.0.0.1:9030",
            "127.0.0.1:9040",
            "127.0.0.1:9050",
            "127.0.0.1:9060",
            "127.0.0.1:9070",
            "127.0.0.1:9080",
            "127.0.0.1:9090"
        };

        static string[] SCHBakFiles = new[]
        {
            "C2102108.sch",
            "C2102208.sch",
            "C2103108.sch",
            "C2103208.sch",
            "C2104108.sch",
            "C2104208.sch",
            "C2105108.sch",
            "C2105208.sch",
            "C2106108.sch",
            "C2106208.sch"
        };

        static void RunProcessSync(string exec, string args)
        {
            var psi = new ProcessStartInfo(exec, args)
            {
                UseShellExecute = false
            };

            RunProcessSync(psi);
        }
        
        static void RunProcessSync(ProcessStartInfo psi)
        {
            var p = new Process()
            {
                StartInfo = psi
            };
            p.Start();
            p.WaitForExit();
        }
        static void KillProcess(string processName)
        {
            RunProcessSync("killall", $"-s SIGKILL {processName}");
        }

        static string GetDateStr()
        {
            return $"{DateTime.UtcNow.Month:D2}{DateTime.UtcNow.Day:D2}";
        }

        static Tuple<char, string> GetMonthCharAndDateCode(string dateStr)
        {
            char[] monthChars = {'1','2','3','4','5','6','7','8','9','a','b','c'};
            string dayCode = dateStr.Substring(2, 2);
            var month = int.Parse(dateStr.Substring(0, 2));
            var monthChar = monthChars[month-1];
            return new Tuple<char, string>(monthChar, dayCode);
        }
        

        static void StreamInputs()
        {
            KillProcess("streamwriter");
            Process.Start(StreamwriterExec, $"--input {SDSource} -s udp://127.0.0.1:5000 -s udp://127.0.0.1:5001 -l --nosap");
            Process.Start(StreamwriterExec, $"--input {HDSource} -s udp://127.0.0.1:5100 -s udp://127.0.0.1:5101 -l --nosap");
        }
        static void RunSplicer(bool wait=false)
        {
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("* Restarting splicer                                                            *");
            Console.WriteLine("********************************************************************************");
            KillProcess("splicer2server_app");
            var psi = new ProcessStartInfo(SplicerExec)
            {
                WorkingDirectory = ConfigFolder,
                Arguments = "--splicer_config ./splicer2server_config.xml --logging_config ./logging.xml",
            };
            psi.EnvironmentVariables["LD_LIBRARY_PATH"] = "/opt/mediaware/instream2/lib";
            if (wait)
            {
                RunProcessSync(psi);
            }
            else
            {
                Process.Start(psi);
            }
        }

        static void KillAllVLCs()
        {
            KillProcess("vlc");
        }

        static void RunInputVLCs()
        {
            KillAllVLCs();

            Process.Start(VlcExecName, "--video-x=0 --video-y=0 --video-title=SD_in udp://@127.0.0.1:5001");
            Process.Start(VlcExecName, "--video-x=0 --video-y=0 --video-title=HD_in udp://@127.0.0.1:5101 ");
        }

        static void AddSpliceVLCs()
        {
            var interval = SpliceVlcInterval;
            var countDown = 1;
            foreach (var addr in OutputAddresses)
            {
                if (--countDown > 0) continue;
                countDown = interval;
                var args = $"--video-x=0 --video-y=0 --video-title={addr} udp://@{addr}";
                Process.Start(VlcExecName, args);
            }
        }

        static void RunAllVLCs()
        {
            KillAllVLCs();

            RunInputVLCs();
            
            AddSpliceVLCs();
        }


        static void ClearDatabase()
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

        static string RenameSCHToDate(string sch, Tuple<char, string> dateCode)
        {
            var suffix = sch.Substring(3);
            return $"{dateCode.Item1}{dateCode.Item2}{suffix}";
        }

        static void PullSCHs(string dateStr)
        {
            var dateCode = GetMonthCharAndDateCode(dateStr);
            // remove all existing SCHs
            var di = new DirectoryInfo(BaseFolder);
            var existing = di.GetFiles().Where(p=>p.Extension.ToLower() == ".sch");
            RemoveFiles(existing);
            Console.WriteLine($"dateStr: {dateStr}");
            foreach (var sch in SCHBakFiles)
            {
                var src = Path.Combine(SCHBackupFolder, sch);
                var targetSch = RenameSCHToDate(sch, dateCode);
                var dst = Path.Combine(BaseFolder, targetSch);
                ModifyAndCopy(src, dst, dateStr);
                var ccmsDst = Path.Combine(CCMSIngestFolder, targetSch);
                File.Copy(dst, ccmsDst, true);
            }
        }

        private static void ModifyAndCopy(string srcFilePath, string dstFilePath, string dateStr)
        {
            using (var srcFile = new FileStream(srcFilePath, FileMode.Open))
            using (var srcStream = new StreamReader(srcFile))
            using (var dstFile = new FileStream(dstFilePath, FileMode.Create))
            using (var dstStream = new StreamWriter(dstFile))
            {
                while (!srcStream.EndOfStream)
                {
                    var line = srcStream.ReadLine();
                    if (line == null || line.Length < 76) continue;
                    if (line.StartsWith("REM"))
                    {
                        dstStream.WriteLine(line);
                        continue;
                    }
                    var newLine = string.Format("{0}{1}{2}", line.Substring(0, 4), dateStr, line.Substring(8));
                    dstStream.WriteLine(newLine);
                }
            }
        }

        static void RemoveFiles(IEnumerable<FileInfo> files)
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

        static void ClearCCMSFolder()
        {
            var di = new DirectoryInfo(CCMSIngestFolder);
            RemoveFiles(di.GetFiles());
        }

        static void RestartIngestor()
        {
            KillProcess(IngestorExecName);
            ClearCCMSFolder();
            ClearDatabase();
            Process.Start(IngestorExec, "--config /Data/video_files/cobalt/config/ccmsingest1.xml");
        }

        static void Ingest()
        {
            var dateStr = GetDateStr();
            PullSCHs(dateStr);
        }

        static void TailDebugLog()
        {
            RunProcessSync("tail", "-F /data/logs/debug.log");
        }

        static void KillAll()
        {
            KillAllVLCs();
            KillProcess("splicer2server_app");
            KillProcess("streamwriter");
            KillProcess(IngestorExecName);
        }

        static void KateConfigs()
        {
            Process.Start("kate", Path.Combine(ConfigFolder, "splicer2server_config.xml"));
            Process.Start("kate", Path.Combine(ConfigFolder, "logging.xml"));
        }

        static void KateSCHs()
        {
            var dateStr = GetDateStr();
            var dateCode = GetMonthCharAndDateCode(dateStr);
            var sb = new StringBuilder();
            foreach (var sch in SCHBakFiles)
            {
                var src = Path.Combine(SCHBackupFolder, sch);
                var targetSch = RenameSCHToDate(sch, dateCode);
                var dst = Path.Combine(BaseFolder, targetSch);
                sb.Append(dst);
                sb.Append(" ");
            }
            sb.Remove(sb.Length-1, 1);
            Process.Start("kate", sb.ToString());
        }

        static void TestWriteConfig()
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                //OmitXmlDeclaration = true,
                //NewLineOnAttributes = true
            };
            const string configLocation = 
                "/Data/config/config.git/splicer2server_config.xml";
                //@"c:\temp\splicer2server_config.xml";

            using (var fs = new FileStream(configLocation, FileMode.Create))
            using (var xw = XmlWriter.Create(fs, settings))
            {
                //var config = Tests.TestGenerateConfig();
                var config = Tests.TestExpertGenerateConfig();
                config.WriteToXml(xw);
            }
        }

        static void Main(string[] args)
        {
#if !USE_TESTS
            const int outputVlcStep = 1;
            const int outputVlcInterval = 10000;

            Tests.GenerateConfigWithInputs(out var config, out var inputProgramToProfiles, out var inputStreamInfi, out var getCCMS);
            var expert = new Expert
            {
                SplicerConfig = config,
            };

            var runner = new RandomMultiSingleOutRunner(
                expert, 
                inputProgramToProfiles, 
                getCCMS)
            {
                CCMSTempDirectory = "/Data/video_files/multitriggers/SCHRun",
                CCMSIngestDirectory = "/opt/mediaware/ingest/ccms",
                ConfigDirectory = "/Data/config/config.git",
                SplicerExecDirctory = "/code/splice_copy2/src/application/splicer2server/build/xenial_testing/release64-nolicense/application/splicer2server/splicer2server_app",
                IngestorExecDirectory = "/code/splice_copy2/src/application/splicer2server/build/xenial_testing/release64-nolicense/application/splicer2server/tools/Ingestor/CCMS",
                IngestConfigPath = "/Data/video_files/cobalt/config/ccmsingest1.xml",
                StreamWriterExecDirectory = "/code/splice/src/application/splicer2server/build/xenial_testing/release64-nolicense/application/tools",
            };

            foreach (var inputStreamInfo in inputStreamInfi)
            {
                runner.InputStreamInfi.Add(inputStreamInfo.SupportedInput, inputStreamInfo);
            }

            var cmd = args.Length > 0 ? args[0] : "all";
            var notkillall = args.Contains("--nokillall");
            switch (cmd)
            {
            case "clear":
                runner.KillAll();
                break;
            case "build": // build splice
                if (!notkillall) runner.KillAll();
                runner.BuildSplicer();
                runner.BuildStreamWriter();
                runner.BuildIngestor();
                break;
            case "write": // write config and ccms
                if (!notkillall) runner.KillAll();
                runner.WriteConfig();
                runner.ClearCCMSTempFolder();
                runner.WriteCCMSFiles();
                break;
            case "restart_ingest":
                runner.RestartIngestor();
                break;
            case "splice_only":
                runner.GenerateConfig();
                runner.RestartInputs();
                runner.RestartSplicer();
                break;
            case "splice_build":
                if (!notkillall) runner.KillAll();
                runner.BuildSplicer();
                runner.WriteConfig();
                runner.ClearCCMSTempFolder();
                runner.WriteCCMSFiles();
                runner.RestartIngestor();
                runner.Ingest();
                runner.RestartInputs();
                runner.RestartSplicer();
                break;
            case "splice":
                if (!notkillall) runner.KillAll();
                runner.WriteConfig();
                runner.ClearCCMSTempFolder();
                runner.WriteCCMSFiles();
                runner.RestartIngestor();
                runner.Ingest();
                runner.RestartInputs();
                runner.RestartSplicer();
                break;
            case "vlc":
                runner.GenerateConfig();
                runner.RestartAllVlcs(outputVlcStep,5000,outputVlcInterval);
                break;
            case "all":
                if (!notkillall) runner.KillAll();
                runner.WriteConfig();
                runner.ClearCCMSTempFolder();
                runner.WriteCCMSFiles();
                runner.RestartIngestor();
                runner.Ingest();
                runner.RestartInputs();
                runner.RestartSplicer();
                runner.RestartAllVlcs(outputVlcStep,5000,outputVlcInterval);
                break;
            }
#else            
#if KATE_CONFIGS
            KateConfigs();
#elif KATE_SCHS
            KateSCHs();
#elif CLEAR_DATABASE            
            ClearDatabase();
#elif KILLALL
            KillAll();
#elif KILL_VLCS
            KillAllVLCs();
#elif TEST_WRITE_CONFIG
            TestWriteConfig();
#else
#if INGEST
            RestartIngestor();
            Ingest();
#endif
#if STREAM_INPTUS
            StreamInputs();
#endif
#if STREAM_INPUTS || RUN_INPUT_VLCS            
            RunInputVLCs();
#endif
#if SPLICE || ADD_SPLICE_VLCS
            AddSpliceVLCs();
#endif
#if SPLICE
#if TAIL_DEBUG
            RunSplicer(false);
#else
            RunSplicer(true);
#endif
#endif
#if TAIL_DEBUG
            TailDebugLog();
#endif
#endif
#endif
        }
    }
}
