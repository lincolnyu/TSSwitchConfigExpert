using System.Collections.Generic;
using System.IO;
using SpliceConfiguration;

namespace SpliceExecution
{
    public class RandomMultiSingleOutRunner : SpliceRunner
    {
        public Expert Expert {get;}

        public List<IList<Expert.ProfileToChannel>> InputProfileInfo {get;}

        public string CCMSTemplatePath {get;}

        public override SplicerConfig Config => Expert.SplicerConfig;

        public RandomMultiSingleOutRunner(Expert expert, 
            List<IList<Expert.ProfileToChannel>> inputProfileInfo,
            string ccmsTemplate, bool killall = false) : base(killall)
        {
            Expert = expert;
            InputProfileInfo = inputProfileInfo;
            CCMSTemplatePath = ccmsTemplate;
        }

        public override void GenerateConfig()
        {
            Expert.SplicerConfig.EnforceOwnerReferences();
            var gen = new Expert.RandomMultiSingleOutGenerator(Expert);
            gen.CompleteConfigWithGivenInputs(InputProfileInfo);
        }

        public override void WriteCCMSFiles()
        {
            using (var fsTemplate = new FileStream(CCMSTemplatePath, FileMode.Open))
            using (var srTemplate = new StreamReader(fsTemplate))
            {
                CCMSFiles = Expert.GenerateSimultaneousCCMSFiles(srTemplate);
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
}
