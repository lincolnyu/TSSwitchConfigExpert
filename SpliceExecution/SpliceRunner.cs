using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SpliceConfiguration;

namespace SpliceExecution
{
    public abstract class SpliceRunner
    {
        public string VlcExec {get; set;} = "cvlc";

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

        public abstract SplicerConfig Config {get;}

        public List<CCMSFile> CCMSFiles {get; protected set;}
        
        public abstract void WriteConfig();

        public abstract void WriteCCMSFiles();

        public virtual void RestartIngester()
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

        public virtual void ClearCCMSIngestFolder()
        {
            var di = new DirectoryInfo(CCMSIngestDirectory);
            Helper.RemoveFiles(di.GetFiles());
        }

        public virtual void RunSplicer(bool sync=false)
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

        public virtual void RunInputVlcs()
        {
            foreach (var input in Config.Inputs)
            {
                var uri = UriToVlcUri(input.Uri);
                var title = input.Name;
                Process.Start(VlcExec, $"--video-x=0 --video-y=0 --video-title={title} {uri}");
            }
        }

        public virtual void RunOutputVlcs()
        {
            foreach(var output in Config.Outputs)
            {
                var uri = UriToVlcUri(output.Uri);
                var title = output.Name;
                Process.Start(VlcExec, $"--video-x=0 --video-y=0 --video-title={title} {uri}");
            }
        }
    }
}
