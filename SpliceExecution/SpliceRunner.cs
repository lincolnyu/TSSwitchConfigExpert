using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using SpliceConfiguration;

namespace SpliceExecution
{
    public abstract class SpliceRunner
    {
        public class InputStreamInfo
        {
            public Input SupportedInput {get; set;}

            public string TSPath {get; set;}

            public List<string> AdditionalUri {get;} = new List<string>();
        }

        public Dictionary<Input, InputStreamInfo> InputStreamInfi = new Dictionary<Input, InputStreamInfo>();

        public string VlcExec {get; set;} = "vlc";

        public string SplicerExecDirctory {get; set;}
        public string SplicerExecName {get; set;} = "splicer2server_app";
        public string SplicerExecPath => Path.Combine(SplicerExecDirctory, SplicerExecName);

        public string ConfigDirectory { get; set;}

        public string ConfigFileName { get;set;} = "splicer2server_config.xml";
        public string LoggingConfigFileName {get; set;} = "logging.xml";

        public string ConfigFilePath => Path.Combine(ConfigDirectory, ConfigFileName);

        // where to put output CCMS files temporarily before ingestion 
        public string CCMSTempDirectory {get; set;}

        public string CCMSIngestDirectory {get;set;}

        public string IngestConfigPath {get;set;}

        public string IngestorExecDirectory {get;set;}
        public string IngestorExecName {get;set;} = "ccms_ingestor";
        public string IngestorExecPath => Path.Combine(IngestorExecDirectory, IngestorExecName);


        public string StreamWriterExecName {get; set;} = "streamwriter";
        public string StreamWriterExecDirectory {get;set;}
        public string StreamWriterExecPath => Path.Combine(StreamWriterExecDirectory, StreamWriterExecName);

        public abstract SplicerConfig Config {get;}

        public List<CCMSFile> CCMSFiles {get; protected set;}

        public abstract void GenerateConfig();
        
        public abstract void WriteCCMSFiles();

        protected SpliceRunner(bool killall = false)
        {
            if (killall)
            {
                KillAll();
            }
        }

        public virtual void WriteConfig()
        {
            GenerateConfig();

            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                //OmitXmlDeclaration = true,
                //NewLineOnAttributes = true
            };

            using (var fsConfig = new FileStream(ConfigFilePath, FileMode.Create))
            using (var xwConfig = XmlWriter.Create(fsConfig, xmlSettings))
            {
                Config.WriteToXml(xwConfig);
            }
        }


        public virtual void KillAll()
        {
            KillAllVlcs();
            Helper.KillProcess(SplicerExecName);
            Helper.KillProcess(StreamWriterExecName);
            Helper.KillProcess(IngestorExecName);
        }

        public virtual void RestartIngestor()
        {
            Helper.KillProcess(IngestorExecName);
            ClearCCMSIngestFolder();
            Helper.ClearDatabase();

            Process.Start(IngestorExecPath, $"--config {IngestConfigPath}");
        }

        public virtual void Ingest(bool deleteSource = false)
        {
            foreach (var file in CCMSFiles)
            {
                var src = Path.Combine(CCMSTempDirectory, file.FileName);
                var dst = Path.Combine(CCMSIngestDirectory, file.FileName);
                if (File.Exists(dst))
                {
                    File.Delete(dst);
                }
                if (deleteSource)
                {
                    File.Move(src, dst);
                }
                else
                {
                    File.Copy(src, dst);
                }
            }
        }

        public virtual void ClearCCMSTempFolder()
        {
            var di = new DirectoryInfo(CCMSTempDirectory);
            Helper.RemoveFiles(di.GetFiles());
        }

        public virtual void ClearCCMSIngestFolder()
        {
            var di = new DirectoryInfo(CCMSIngestDirectory);
            Helper.RemoveFiles(di.GetFiles());
        }

        public virtual void RestartInputs()
        {
            Helper.KillProcess(StreamWriterExecName);
            foreach (var input in Config.Inputs)
            {
                if (InputStreamInfi.TryGetValue(input, out var info))
                {
                    var uriSb = new StringBuilder($" -s {info.SupportedInput.Uri}");
                    foreach (var uri in info.AdditionalUri)
                    {
                        uriSb.Append(" -s ");
                        uriSb.Append(uri);
                    }
                    Process.Start(StreamWriterExecPath, $"--input {info.TSPath}{uriSb.ToString()} -l --nosap");
                }
            }
        }
        public virtual void RestartSplicer(bool sync=false)
        {
            Helper.KillProcess(SplicerExecName);
            var psi = new ProcessStartInfo(SplicerExecPath)
            {
                WorkingDirectory = ConfigDirectory,
                Arguments = $"--splicer_config ./{ConfigFileName} --logging_config ./{LoggingConfigFileName}",
                
            };
            psi.EnvironmentVariables["LD_LIBRARY_PATH"] = "/opt/mediaware/instream2/lib";
            if (sync)
            {
                Helper.RunProcessSync(psi);
            }
            else
            {
                Process.Start(psi);
            }
        }

        public virtual void RestartAllVlcs(int outputStep = 1, int delayMs = 0, int intervalMs = 0)
        {
            KillAllVlcs();
            if (delayMs > 0)
            {
                Thread.Sleep(delayMs);
            }
            RunInputVlcs(intervalMs);
            if (intervalMs > 0)
            {
                Thread.Sleep(intervalMs);
            }
            RunOutputVlcs(outputStep, intervalMs);
        }

        private static string UriToVlcUri(string uri)
        {
            // uri should be like udp://127.0.0.1...
            var slashesPos = uri.IndexOf("//");
            if (slashesPos < 0) return uri;
            if (slashesPos + 2 < uri.Length && char.IsDigit(uri[slashesPos+2]))
            {
                return $"{uri.Substring(0, slashesPos+2)}@{uri.Substring(slashesPos+2)}";
            }
            return uri;
        }

        public virtual void KillAllVlcs()
        {
            Helper.KillProcess("vlc");
        }

        public virtual void RunInputVlcs(int intervalMs = 0)
        {
            foreach (var input in Config.Inputs)
            {
                if (InputStreamInfi.TryGetValue(input, out var streamInfo) && streamInfo.AdditionalUri.Count > 0)
                {
                    var uri = UriToVlcUri(streamInfo.AdditionalUri.First());
                    var title = input.Name;
                    Process.Start(VlcExec, $"--video-x=0 --video-y=0 --video-title={title} {uri}");
                    if (intervalMs > 0)
                    {
                        Thread.Sleep(intervalMs);
                    }
                }
            }
        }

        public virtual void RunOutputVlcs(int step = 1, int intervalMs = 0)
        {
            var countDown = 1;
            foreach(var output in Config.Outputs)
            {
                if (--countDown > 0)
                {
                    continue;
                }
                var uri = UriToVlcUri(output.Uri);
                var title = output.Name;
                Process.Start(VlcExec, $"--video-x=0 --video-y=0 --video-title={title} {uri}");
                if (intervalMs > 0)
                {
                    Thread.Sleep(intervalMs);
                }
                countDown = step;
            }
        }

        public virtual void BuildSplicer()
        {
            var buildDir = Path.Combine(SplicerExecDirctory, "../../..");
            Directory.SetCurrentDirectory(buildDir);
            Helper.RunProcessSync("remedy", "../../../../../src.remedy splicer2server_app --regen");
            Helper.RunProcessSync("remedy", "../../../../../src.remedy splicer2server_app");
        }

        public virtual void BuildStreamWriter()
        {
            var buildDir = Path.Combine(StreamWriterExecDirectory, "../..");
            Directory.SetCurrentDirectory(buildDir);
            Helper.RunProcessSync("remedy", "../../../../../src.remedy streamwriter --regen");
            Helper.RunProcessSync("remedy", "../../../../../src.remedy streamwriter");
        }

        public virtual void BuildIngestor()
        {
            var buildDir = Path.Combine(IngestorExecDirectory, "../../../../..");
            Directory.SetCurrentDirectory(buildDir);
            Helper.RunProcessSync("remedy", "../../../../../src.remedy splicer2server_ccms_ingestor --regen");
            Helper.RunProcessSync("remedy", "../../../../../src.remedy splicer2server_ccms_ingestor");
        }
    }
}
