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

            const string AccuracyMode = "frameEncSubgop";

            const int txRateSD = 10000000;
            const int txRateHD = 13000000;

            const int maxGopLenSD = 24;
            const int maxGopLenHD = 40;

            const string placeholderSD = "stream_003";
            const string placeholderHD = "stream_004";

            var librarySd = Library.CreateTSLibrary("HBO_SD", "/Data/video_files/multitriggers/SD_Assets/");
            var libraryHd = Library.CreateTSLibrary("HBO_HD", "/Data/video_files/multitriggers/HD_Assets/");

            var tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7301,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7331
                    };
            var profile_HBOAdrSD_Cro = new Profile
            {
                Name = "profile_HBOAdrSD_Cro",
                OutputMuxRate = txRateSD,
                PCRPid = 811,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "bits",
                        CheckCCErrors = true,
                        MaxBitRate = 8000000,
                        MinBitRate = 5000000,
                        OutputPid= 811
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7321,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7341
                    };
            var profile_HBOAdrHD_Cro = new Profile
            {
                Name = "profile_HBOAdrHD_Cro",
                OutputMuxRate = txRateHD,
                PCRPid = 3321,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 10000000,
                        MinBitRate = 8000000,
                        OutputPid= 3321
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7302,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7332
                    };
            var profile_HBOAdrSD_Slo = new Profile
            {
                Name = "profile_HBOAdrSD_Slo",
                OutputMuxRate = txRateSD,
                PCRPid = 812,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 8000000,
                        MinBitRate = 5000000,
                        OutputPid= 812
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

           tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7322,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7342
                    };
            var profile_HBOAdrHD_Slo  = new Profile
            {
                Name = "profile_HBOAdrHD_Slo",
                OutputMuxRate = txRateHD,
                PCRPid = 3322,
                ElementaryStreams = 
                {
                   new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 11000000,
                        MinBitRate = 8000000,
                        OutputPid= 3322
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

           tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7305,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7335
                    };
            var profile_HBOAdrSD_Srb  = new Profile
            {
                Name = "profile_HBOAdrSD_Srb",
                OutputMuxRate = txRateSD,
                PCRPid = 813,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 8000000,
                        MinBitRate = 5000000,
                        OutputPid= 813
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

           tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 3304,
                        BitRateDisplayUnits = "bits",
                        CheckCCErrors = true,
                        MaxBitRate = 300000,
                        MinBitRate = 192000,
                        OutputPid= 3316
                    };
            var profile_HBOAdrHD_Srb  = new Profile
            {
                Name = "profile_HBOAdrHD_Srb",
                OutputMuxRate = txRateHD,
                PCRPid = 3323,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 11000000,
                        MinBitRate = 8000000,
                        OutputPid= 3323
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7306,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7336
                    };
            var profile_HBOAdrSD_Bul  = new Profile
            {
                Name = "profile_HBOAdrSD_Bul",
                OutputMuxRate = txRateSD,
                PCRPid = 814,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 8000000,
                        MinBitRate = 5000000,
                        OutputPid= 814
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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


            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7326,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7346
                    };
            var profile_HBOAdrHD_Bul  = new Profile
            {
                Name = "profile_HBOAdrHD_Bul",
                OutputMuxRate = txRateHD,
                PCRPid = 3324,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 11000000,
                        MinBitRate = 8000000,
                        OutputPid= 3324
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7303,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7333
                    };
            var profile_HBOAdrSD_Mac  = new Profile
            {
                Name = "profile_HBOAdrSD_Mac",
                OutputMuxRate = txRateSD,
                PCRPid = 815,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 8000000,
                        MinBitRate = 5000000,
                        OutputPid= 815
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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

            tempOutES = new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                        MatchPid = 7323,
                        BitRateDisplayUnits = "bits",
                        ExcludeFromMuxerRestarts = true,
                        MaxBitRate = 200000,
                        MinBitRate = 4602,
                        OutputPid= 7343
                    };
            var profile_HBOAdrHD_Mac  = new Profile
            {
                Name = "profile_HBOAdrHD_Mac",
                OutputMuxRate = txRateHD,
                PCRPid = 3325,
                ElementaryStreams = 
                {
                    new Profile.OutputElementaryStream {
                        MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                        BitRateDisplayUnits = "megabits",
                        CheckCCErrors = true,
                        MaxBitRate = 11000000,
                        MinBitRate = 8000000,
                        OutputPid= 3325
                    },
                    tempOutES,
                    tempOutES,
                    tempOutES
                },
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
                Name = "scte35_HBOAdrSD_Cro",
                NetworkId = "2",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrHD_Cro = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrHD_Cro",
                NetworkId = "2",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrSD_Slo = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrSD_Slo",
                NetworkId = "3",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Srb = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrSD_Srb",
                NetworkId = "4",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Bul = new SCTE35Trigger
            {
                Name =  "scte35_HBOAdrSD_Bul",
                NetworkId = "5",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrSD_Mac = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrSD_Mac",
                NetworkId = "6",
                ZoneId = "108",
                MissingAssetPlaceholder = placeholderSD,
                Library = librarySd
            };

            var scte35_HBOAdrHD_Slo = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrHD_Slo",
                NetworkId = "3",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Srb = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrHD_Srb",
                NetworkId = "4",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Bul = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrHD_Bul",
                NetworkId = "5",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var scte35_HBOAdrHD_Mac = new SCTE35Trigger
            {
                Name = "scte35_HBOAdrHD_Mac",
                NetworkId = "6",
                ZoneId = "208",
                MissingAssetPlaceholder = placeholderHD,
                Library = libraryHd
            };

            var jamPrev = new Channel.JamPrevention
            {
            };

            var channel_HBOAdrSD_Cro = new Channel
            {
                Name = "HBOAdrSD_Cro",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrSD_Cro,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Cro,
                JamPrev = jamPrev
            };

            var channel_HBOAdrHD_Cro = new Channel
            {
                Name = "HBOAdrHD_Cro",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrHD_Cro,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Cro,
                JamPrev = jamPrev
            };

            var channel_HBOAdrSD_Slo = new Channel
            {
                Name = "HBOAdrSD_Slo",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrSD_Slo,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Slo,
                JamPrev = jamPrev
            };

            var channel_HBOAdrSD_Srb = new Channel
            {
                Name = "HBOAdrSD_Srb",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrSD_Srb,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Srb,
                JamPrev = jamPrev
            };

            var channel_HBOAdrSD_Bul = new Channel
            {
                Name = "HBOAdrSD_Bul",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrSD_Bul,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Bul,
                JamPrev = jamPrev
            };

            var channel_HBOAdrSD_Mac = new Channel
            {
                Name = "HBOAdrSD_Mac",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenSD,
                Profile = profile_HBOAdrSD_Mac,
                Input = inputSD,
                PrimaryProgram = inputSD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrSD_Mac,
                JamPrev = jamPrev
            };

            var channel_HBOAdrHD_Slo = new Channel
            {
                Name = "HBOAdrHD_Slo",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profile_HBOAdrHD_Slo,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Slo,
                JamPrev = jamPrev
            };

            var channel_HBOAdrHD_Srb = new Channel
            {
                Name = "HBOAdrHD_Srb",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profile_HBOAdrHD_Srb,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Srb,
                JamPrev = jamPrev
            };

            var channel_HBOAdrHD_Bul = new Channel
            {
                Name = "HBOAdrHD_Bul",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profile_HBOAdrHD_Bul,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Bul,
                JamPrev = jamPrev
            };

            var channel_HBOAdrHD_Mac = new Channel
            {
                Name = "HBOAdrHD_Mac",
                AccuracyMode = AccuracyMode,
                EnableRateTracking = true,
                MaxGopLength = maxGopLenHD,
                Profile = profile_HBOAdrHD_Mac,
                Input = inputHD,
                PrimaryProgram = inputHD.InputPrograms.FirstOrDefault(),
                SCTE35Config = scte35_HBOAdrHD_Mac,
                JamPrev = jamPrev
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
                    profile_HBOAdrSD_Cro,
                    profile_HBOAdrHD_Cro,
                    profile_HBOAdrSD_Slo,
                    profile_HBOAdrHD_Slo,
                    profile_HBOAdrSD_Srb,
                    profile_HBOAdrHD_Srb,
                    profile_HBOAdrSD_Bul,
                    profile_HBOAdrHD_Bul,
                    profile_HBOAdrSD_Mac,
                    profile_HBOAdrHD_Mac
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
                    channel_HBOAdrHD_Cro,
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
