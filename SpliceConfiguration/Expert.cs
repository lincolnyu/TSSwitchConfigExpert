using System;
using System.Collections.Generic;
using System.Linq;

namespace SpliceConfiguration
{
    class Expert
    {
        public SplicerConfig SplicerConfig {get; private set; }
        public List<CCMSFile> CCMSFiles {get;} = new List<CCMSFile>();


        private List<Input> inputs_ = new List<Input>();

        public Input AddInput(string name, IEnumerable<Tuple<string, string, int>> programNameIdNumbers, string uri, string apiId)
        {
            var input = new Input
            {
                Name = name,
                Uri = uri,
                ApiId = apiId,
            };
            foreach (var tuple in programNameIdNumbers)
            {
                var ip = new InputProgram
                {
                    Name = tuple.Item1,
                    ProgramId = tuple.Item2,
                    ProgramNumber = tuple.Item3
                };
            }
            return input;
        }
            
        public Output AddOuptut()
        {
            throw new System.NotImplementedException();
        }

        public void SetChannels(InputProgram primary, ICollection<InputProgram> additional, Output output)
        {

        }


        public void GenerateSplicerConfig()
        {

        }

        public void LoadTemplateCCMSPlaylist()
        {

        }

        public void GenerateCCMSPlaylist()
        {

        }

        public static SplicerConfig TestGenerateConfig()
        {
            var inputSD = new Input
            {
                Name="HBOAdrSD_Src",
                Uri = "udp://127.0.0.1:5000",
                ApiId = "3",
                InputPrograms = 
                {
                    new InputProgram
                    {
                        Name = "HBOAdriaSD",
                        ProgramId = "3",
                        ProgramNumber = 59
                    }
                }
            };
            var inputHD = new Input
            {
                Name="HBOAdrHD_Src",
                Uri = "udp://127.0.0.1:5100",
                ApiId = "4",
                InputPrograms = 
                {
                    new InputProgram
                    {
                        Name = "HBOAdriaHD",
                        ProgramId = "4",
                        ProgramNumber = 309
                    }
                }
            };

            const int txRateSD = 10000000;
            const int txRateHD = 13000000;

            const int maxGopLenSD = 24;
            const int maxGopLenHD = 40;

            const string placeholderSD = "stream_003";
            const string placeholderHD = "stream_003";

            var librarySd = Library.CreateTSLibrary("HBO_SD", "/Data/video_files/multitriggers/SD_Assets/");
            var libraryHd = Library.CreateTSLibrary("HBO_HD", "/Data/video_files/multitriggers/HD_Assets/");

            var profileHBOAdrSDCro = new Profile
            {
                OutputMuxRate = txRateSD,
                PCRPid = 811,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5923,
                        ProgramNumber = 1222,
                        ServiceName = "HBO Slovenia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };
            var profileHBOAdrHDCro = new Profile
            {
                OutputMuxRate = txRateHD,
                PCRPid = 3321,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5932,
                        ProgramNumber = 1221,
                        ServiceName = "HBO Croatia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrSDSlo = new Profile
            {
                OutputMuxRate = txRateSD,
                PCRPid = 821,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5923,
                        ProgramNumber = 1222,
                        ServiceName = "HBO Slovenia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrHDSlo  = new Profile
            {
                OutputMuxRate = txRateHD,
                PCRPid = 3322,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5933,
                        ProgramNumber = 1305,
                        ServiceName = "HBO Slovenia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrSDSrb  = new Profile
            {
                OutputMuxRate = txRateSD,
                PCRPid = 813,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5924,
                        ProgramNumber = 1224,
                        ServiceName = "HBO Serbia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrHDSrb  = new Profile
            {
                OutputMuxRate = txRateHD,
                PCRPid = 3323,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5934,
                        ProgramNumber = 1306,
                        ServiceName = "HBO Serbia HD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrSDBul  = new Profile
            {
                OutputMuxRate = txRateSD,
                PCRPid = 814,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5925,
                        ProgramNumber = 1225,
                        ServiceName = "HBO Bulgaria SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrHDBul  = new Profile
            {
                OutputMuxRate = txRateHD,
                PCRPid = 3324,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5935,
                        ProgramNumber = 1307,
                        ServiceName = "HBO Bulgaria HD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrSDMac  = new Profile
            {
                OutputMuxRate = txRateSD,
                PCRPid = 815,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5926,
                        ProgramNumber = 1226,
                        ServiceName = "HBO Macedonia SD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var profileHBOAdrHDMac  = new Profile
            {
                OutputMuxRate = txRateHD,
                PCRPid = 3325,
                OutputPrograms = 
                {
                    new OutputProgram
                    {
                        PmtPid = 5936,
                        ProgramNumber = 1308,
                        ServiceName = "HBO Macedonia HD Splicer1",
                        ServiceProviderName = "HBO Europe"
                    }
                }
            };

            var scte35_HBOAdrSD_Cro = new SCTE35Trigger
            {
                NetworkId = "2",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrHD_Cro = new SCTE35Trigger
            {
                NetworkId = "2",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrSD_Slo = new SCTE35Trigger
            {
                NetworkId = "3",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Srb = new SCTE35Trigger
            {
                NetworkId = "4",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Bul = new SCTE35Trigger
            {
                NetworkId = "5",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Mac = new SCTE35Trigger
            {
                NetworkId = "6",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrHD_Slo = new SCTE35Trigger
            {
                NetworkId = "3",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Srb = new SCTE35Trigger
            {
                NetworkId = "4",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Bul = new SCTE35Trigger
            {
                NetworkId = "5",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Mac = new SCTE35Trigger
            {
                NetworkId = "6",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var channel_HBOAdrSD_Cro = new Channel
            {
                Name = "HBOAdrSD_Cro",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrHDCro,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Cro
            };

            var channel_HBOAdrHD_Cro = new Channel
            {
                Name = "HBOAdrHD_Cro",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrHDCro,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Cro
            };

            var channel_HBOAdrSD_Slo = new Channel
            {
                Name = "HBOAdrSD_Slo",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrSDSlo,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Slo
            };

            var channel_HBOAdrSD_Srb = new Channel
            {
                Name = "HBOAdrSD_Srb",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrSDSrb,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Srb
            };

            var channel_HBOAdrSD_Bul = new Channel
            {
                Name = "HBOAdrSD_Bul",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrSDBul,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Bul
            };

            var channel_HBOAdrSD_Mac = new Channel
            {
                Name = "HBOAdrSD_Mac",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profileHBOAdrSDMac,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Mac
            };

            var channel_HBOAdrHD_Slo = new Channel
            {
                Name = "HBOAdrHD_Slo",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profileHBOAdrHDSlo,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Slo
            };

            var channel_HBOAdrHD_Srb = new Channel
            {
                Name = "HBOAdrHD_Srb",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profileHBOAdrHDSrb,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Srb
            };

            var channel_HBOAdrHD_Bul = new Channel
            {
                Name = "HBOAdrHD_Bul",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profileHBOAdrHDBul,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Bul
            };

            var channel_HBOAdrHD_Mac = new Channel
            {
                Name = "HBOAdrHD_Mac",
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profileHBOAdrHDMac,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Mac
            };

            var output_HBOAdrSD_Cro = new Output
            {
                Name = "HBOAdrSD_Cro",
                TxRate = txRateSD,
                Uri = "udp://127.0.0.1:9000",
                ApiId = "11",
                Channels = 
                {
                    channel_HBOAdrSD_Cro
                }
            };

            var output_HBOAdrSD_Slo = new Output
            {
                Name = "HBOAdrSD_Slo",
                TxRate = txRateSD,
                Uri = "udp://127.0.0.1:9010",
                ApiId = "12",
                Channels = 
                {
                    channel_HBOAdrSD_Slo
                }
            };

            var output_HBOAdrSD_Srb = new Output
            {
                Name = "HBOAdrSD_Srb",
                TxRate = txRateSD,
                Uri = "udp://127.0.0.1:9020",
                ApiId = "13",
                Channels = 
                {
                    channel_HBOAdrSD_Srb
                }
            };

            var output_HBOAdrSD_Bul = new Output
            {
                Name = "HBOAdrSD_Bul",
                TxRate = txRateSD,
                Uri = "udp://127.0.0.1:9030",
                ApiId = "14",
                Channels = 
                {
                    channel_HBOAdrSD_Bul
                }
            };

            var output_HBOAdrSD_Mac = new Output
            {
                Name = "HBOAdrSD_Mac",
                TxRate = txRateSD,
                Uri = "udp://127.0.0.1:9040",
                ApiId = "15",
                Channels =
                {
                    channel_HBOAdrSD_Mac
                }
            };

            var output_HBOAdrHD_Cro = new Output
            {
                Name = "HBOAdrHD_Cro",
                TxRate = txRateHD,
                Uri = "udp://127.0.0.1:9050",
                ApiId = "16",
                Channels = 
                {
                    channel_HBOAdrHD_Cro
                }
            };

            var output_HBOAdrHD_Slo = new Output
            {
                Name = "HBOAdrHD_Slo",
                TxRate = txRateHD,
                Uri = "udp://127.0.0.1:9060",
                ApiId = "17",
                Channels = 
                {
                    channel_HBOAdrHD_Slo
                }
            };

            var output_HBOAdrHD_Srb = new Output
            {
                Name = "HBOAdrHD_Srb",
                TxRate = txRateHD,
                Uri = "udp://127.0.0.1:9070",
                ApiId = "18",
                Channels = 
                {
                    channel_HBOAdrHD_Srb
                }
            };

            var output_HBOAdrHD_Bul = new Output
            {
                Name = "HBOAdrHD_Bul",
                TxRate = txRateHD,
                Uri = "udp://127.0.0.1:9080",
                ApiId = "19",
                Channels = 
                {
                    channel_HBOAdrHD_Bul
                }
            };

            var output_HBOAdrHD_Mac = new Output
            {
                Name = "HBOAdrHD_Mac",
                TxRate = txRateHD,
                Uri = "udp://127.0.0.1:9090",
                ApiId = "20",
                Channels = 
                {
                    channel_HBOAdrHD_Mac
                }
            };

            var config = new SplicerConfig
            {
                Inputs = 
                {
                    inputSD,
                    inputHD
                },
                Profiles = 
                {
                    profileHBOAdrSDCro,
                    profileHBOAdrHDCro,
                    profileHBOAdrSDSlo,
                    profileHBOAdrHDSlo,
                    profileHBOAdrSDSrb,
                    profileHBOAdrHDSrb,
                    profileHBOAdrSDBul,
                    profileHBOAdrHDBul,
                    profileHBOAdrSDMac,
                    profileHBOAdrHDMac
                },
                Libraries = 
                {
                    librarySd,
                    libraryHd
                },
                Triggers = 
                {
                    scte35_HBOAdrSD_Cro,
                    scte35_HBOAdrHD_Cro,
                    scte35_HBOAdrSD_Slo,
                    scte35_HBOAdrSD_Srb,
                    scte35_HBOAdrSD_Bul,
                    scte35_HBOAdrSD_Mac,
                    scte35_HBOAdrHD_Slo,
                    scte35_HBOAdrHD_Srb,
                    scte35_HBOAdrHD_Bul,
                    scte35_HBOAdrHD_Mac
                },
                Channels = 
                {
                    channel_HBOAdrSD_Cro,
                    channel_HBOAdrHD_Bul,
                    channel_HBOAdrSD_Slo,
                    channel_HBOAdrSD_Srb,
                    channel_HBOAdrSD_Bul,
                    channel_HBOAdrSD_Mac,
                    channel_HBOAdrHD_Slo,
                    channel_HBOAdrHD_Srb,
                    channel_HBOAdrHD_Bul,
                    channel_HBOAdrHD_Mac
                },
                Outputs = 
                {
                    output_HBOAdrSD_Cro,
                    output_HBOAdrSD_Slo,
                    output_HBOAdrSD_Srb,
                    output_HBOAdrSD_Bul,
                    output_HBOAdrSD_Mac,
                    output_HBOAdrHD_Cro,
                    output_HBOAdrHD_Slo,
                    output_HBOAdrHD_Srb,
                    output_HBOAdrHD_Bul,
                    output_HBOAdrHD_Mac
                }
            };

            return config;
        }
    }
}
