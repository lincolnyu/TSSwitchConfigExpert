using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SpliceConfiguration
{
    /**
      Reference: 
       https://en.wikipedia.org/wiki/MPEG_transport_stream
     */
    public class Expert
    {
        public class ProfileToChannel
        {
            public int? VideoPid; // When specified pick the video with specified PID

            public int? NonVideoPid; // When specified pick the non-video with specified PID

            public IList<string> Uris;

            public Channel.JamPrevention JamPrevention;

            #region Trigger related

            public Library Library;
            bool CreateTrigger => Library != null;
            public int NetworkId;
            public int ZoneId;
            public string MissingAssetPlaceholder;

            #endregion
        }

        public class RandomMultiSingleOutGenerator
        {
            public RandomMultiSingleOutGenerator(Expert expert)
            {
                Target = expert;
            }

            public Expert Target {get;}

            public bool EnableRateTracking {get; set;} = true;

            /**
             Pre: The config must already have at least one input with at least one input program each
             Generate config on the target with its existing inputs details and
             information specified in this class
             Inputs if having their api_ids the ids should be numeric

             Each channel uses one of the input programs in the config as primary 
             and doesn't have additional

            inputProgramToProfiles[i] -> i-th input program's parameters
            inputProgramToProfiles[i][j] -> j-th profile on the 1-th input program

             This populates Profiles, Channels and Outputs etc.
             */
            public void CompleteConfigWithGivenInputs(IList<IList<ProfileToChannel>> inputProgramToProfiles)
            {
                var i = 0;
                var config = Target.SplicerConfig;
                var usedOutputPids = new List<int>();

                var maxInputApiId = config.Inputs.Select(x=>int.Parse(x.ApiId)).Max();
                var apiId = maxInputApiId + 1;
                var outputProgramNumber = 1000; // This is ok?
                foreach (var input in config.Inputs)
                {
                    foreach (var inputProgram in input.InputPrograms)
                    {
                        var esPids = inputProgram.GetAllElementaryStreamPids();
                        Func<int> newOutputPid = ()=>
                        {
                            var pid = esPids.Concat(usedOutputPids).GenerateRandomPid();
                            usedOutputPids.Add(pid);
                            return pid;
                        };

                        var profileNamePrefix = $"profile_{inputProgram.Name}";
                        var j = 0;
                        foreach (var inputProgramProfile in inputProgramToProfiles[i])
                        {
                            var jamPrev = inputProgramProfile.JamPrevention;
                            var traitsList = new[]
                            {
                                new Profile.ElementaryStreamSelectionTraits
                                        {
                                            MatchType = Profile.OutputElementaryStream.MatchTypes.Video,
                                            Pid = inputProgramProfile.VideoPid,
                                            OutputPid = newOutputPid()
                                        },
                                new Profile.ElementaryStreamSelectionTraits
                                        {
                                            MatchType = Profile.OutputElementaryStream.MatchTypes.Pid,
                                            Pid = inputProgramProfile.NonVideoPid,
                                            OutputPid = newOutputPid()
                                        }
                            };

                            foreach (var t in traitsList)
                            {
                                t.RateTracking = EnableRateTracking;
                                t.SuggestTraits();
                            }
                            
                            var profileName = inputProgramToProfiles[i].Count > 1?
                                $"{profileNamePrefix}_{++j}" : profileNamePrefix;

                            var outputPmtPid = newOutputPid();
                            var outputProgram = new OutputProgram
                            {
                                PmtPid = outputPmtPid,
                                ProgramNumber = outputProgramNumber++
                            };

                            var profile = CreateProfile(profileName, inputProgram, 
                                traitsList,
                                new []{outputProgram});
                            Target.SplicerConfig.Profiles.Add(profile);

                            GenerateChannelsAndOutputsOnProfile(profile, inputProgramProfile, ref apiId);
                        }
                        if (i + 1 < inputProgramToProfiles.Count)
                        {
                            i++;
                        }
                    }
                }
            }

            private void GenerateChannelsAndOutputsOnProfile(Profile profile, ProfileToChannel inputProgramProfile, ref int apiId)
            {
                var jamPrev = inputProgramProfile.JamPrevention;
                var outputUris = inputProgramProfile.Uris;

                var suffix = profile.Name.Substring("profile".Length);
                var channelPrefix = $"channel{suffix}";
                var outputPrefix = $"output{suffix}";

                var triggerName =  $"scte35{suffix}";
                var trigger = Expert.CreateTrigger(triggerName, inputProgramProfile.Library, 
                    inputProgramProfile.NetworkId, inputProgramProfile.ZoneId, inputProgramProfile.MissingAssetPlaceholder);

                Target.SplicerConfig.Triggers.Add(trigger);

                var numChannels = outputUris.Count;
                for (var i = 0; i < numChannels; i++)
                {
                    var channelName = numChannels > 1? $"{channelPrefix}_{i+1}" : channelPrefix;
                    var channel = Expert.CreateChannel(channelName, profile, trigger, jamPrev);
                    Target.SplicerConfig.Channels.Add(channel);
                    var outputName = numChannels > 1? $"{outputPrefix}_{i+1}" : outputPrefix;
                    var output = Expert.CreateOutput(outputName, outputUris[i], new[]{channel}, apiId.ToString());
                    Target.SplicerConfig.Outputs.Add(output);
                    apiId++;
                }
            }
        }

        public SplicerConfig SplicerConfig {get; internal set; }

        public List<CCMSFile> CCMSFiles {get;} = new List<CCMSFile>();

        public static Input CreateInput(string name, IEnumerable<Tuple<string, string, int>> programNameIdNumbers, string uri, string apiId)
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
                    ProgramNumber = tuple.Item3,
                    Owner = input
                };
            }
            return input;
        }

        public static SCTE35Trigger CreateTrigger(string name, Library library, int networkId, int zoneId, string missingAssetPlaceHolder)
        {
            return new SCTE35Trigger
            {
                Name = name,
                Library = library,
                NetworkId = networkId.ToString(),
                ZoneId = zoneId.ToString(),
                MissingAssetPlaceholder = missingAssetPlaceHolder
            };
        }

        public static Profile CreateProfile(string name, InputProgram inputProgram, 
            IEnumerable<Profile.ElementaryStreamSelectionTraits> traitsQueue,
            IEnumerable<OutputProgram> outputPrograms)
        {
            var inputProfile = inputProgram.Profile;
            traitsQueue = Profile.SuggestTraits(traitsQueue);
            var firstVideo = traitsQueue.First(x=>x.MatchType == Profile.OutputElementaryStream.MatchTypes.Video);
            var outputProfile = new Profile
            {
                Name = name,
                SourceProfile = inputProfile,
                PCRPid = firstVideo.OutputPid
            };
            outputProfile.GenerateFromSource(traitsQueue);

            foreach (var outputProgram in outputPrograms)
            {
                outputProfile.OutputPrograms.Add(outputProgram);
            }

            return outputProfile;
        }

        public static Channel CreateChannel(string name, Profile profile, SCTE35Trigger trigger = null, Channel.JamPrevention jamPrev = null, bool? rateTracking = false)
        {
            var firstVideoInput = profile.SourceProfile.FirstVideo();
            var firstVideo = profile.FirstVideo();
            var channel = new Channel
            {
                Name = name,
                Profile = profile,
                AccuracyMode = "frameEncSubgop", // TODO smart choice or manual choice
                Input = profile.SourceProfile.Owner.Owner,
                PrimaryProgram = profile.SourceProfile.Owner,
                MaxGopLength = firstVideoInput.MaxGopLength,
                EnableRateTracking = rateTracking.HasValue? rateTracking.Value : firstVideo?.MinBitRate.HasValue?? false,
                SCTE35Config = trigger,
                JamPrev = jamPrev
            };

            // TODO additional crosspoints...
            
            return channel;
        }

        public static Output CreateOutput(string name, string uri, IEnumerable<Channel> channels, string apiId)
        {
            var output = new Output
            {
                Name = name,
                ApiId = apiId,
                Uri = uri
            };
            var tx = 0;
            foreach (var channel in channels)
            {
                output.Channels.Add(channel);
                tx += channel.Profile.OutputMuxRate;
            }
            output.TxRate = tx;
            return output;
        }
        
        public Library AddLibrary(string name, string location, string extension = "ts", string libraryType = "SPLICE_ASSET")
        {
            return new Library
            {
                Name = name,
                Location = location,
                Extension = extension,
                LibraryType = libraryType
            };
        }
        
        public SCTE35Trigger AddSCTE35Trigger(string name, Library library, Tuple<string, string> networkZoneIdPair)
        {
            var trigger = new SCTE35Trigger
            {
                Name = name,
                Library = library,
            };
            if (networkZoneIdPair != null)
            {
                trigger.NetworkId = networkZoneIdPair.Item1;
                trigger.ZoneId = networkZoneIdPair.Item2;
            }
            return trigger;
        }

        public List<CCMSFile> GenerateSimultaneousCCMSFiles(TextReader trTemplate)
        {
            var template = CCMSFile.LoadTemplate(trTemplate);
            var files = new List<CCMSFile>();
            var now = DateTime.UtcNow;
            foreach (var trigger in SplicerConfig.Triggers)
            {
                var networkId = int.Parse(trigger.NetworkId);
                var zoneId = int.Parse(trigger.ZoneId);
                var file = new CCMSFile(now, networkId, zoneId);
                file.CopyRecordsFrom(template, true);
                files.Add(file);
            }

            CCMSFile.RandomizeSpotIds(files);
            return files;
        }
    }
}
