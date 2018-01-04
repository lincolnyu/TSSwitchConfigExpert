using System.Collections.Generic;
using System.IO;
using SpliceConfiguration;

namespace SpliceExecution
{
    public class RandomMultiSingleOutRunner : SpliceRunner
    {
        public delegate string GetCCMSTemplatePathDelegate(SCTE35Trigger trigger);

        private Dictionary<string, string> _ccmsTemplateCache = new Dictionary<string, string>();

        public Expert Expert {get;}

        public List<IList<Expert.ProfileToChannel>> InputProfileInfo {get;}

        public override SplicerConfig Config => Expert.SplicerConfig;

        public GetCCMSTemplatePathDelegate GetCCMSTemplatePath {get;set;}

        public RandomMultiSingleOutRunner(Expert expert, 
            List<IList<Expert.ProfileToChannel>> inputProfileInfo,
            GetCCMSTemplatePathDelegate getCCMSTemplatePath, bool killall = false) : base(killall)
        {
            Expert = expert;
            InputProfileInfo = inputProfileInfo;
            GetCCMSTemplatePath = getCCMSTemplatePath;
        }

        public override void GenerateConfig()
        {
            Expert.SplicerConfig.EnforceOwnerReferences();
            var gen = new Expert.RandomMultiSingleOutGenerator(Expert);
            gen.CompleteConfigWithGivenInputs(InputProfileInfo);
        }

        public override void WriteCCMSFiles()
        {
            CCMSFiles = Expert.GenerateSimultaneousCCMSFiles(trigger=>
            {
                var path = GetCCMSTemplatePath(trigger);
                if (!_ccmsTemplateCache.TryGetValue(path, out var content))
                {
                    using (var fs = new FileStream(path, FileMode.Open))
                    using (var sr = new StreamReader(fs))
                    {
                        content = sr.ReadToEnd();
                        _ccmsTemplateCache[path] = content;
                    }
                }
                return content;
            });
            foreach (var file in CCMSFiles)
            {
                var path = Path.Combine(CCMSTempDirectory, file.FileName);
                using (var fsOut = new FileStream(path, FileMode.Create))
                using (var swOut = new StreamWriter(fsOut))
                {
                    file.Write(swOut);
                }
            }
        }
    }
}
