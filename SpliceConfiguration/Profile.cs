using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace SpliceConfiguration
{
    public class Profile: IXmlWritable
    {
        public class OutputElementaryStream : IXmlWritable
        {
            public enum MatchTypes
            {
                Audio,
                Video,
                Pid
            }

            private int? maxBitRate_;

            public ElementaryStream SourceStream{get;set;}

            public string BitRateDisplayUnits {get;set;} = "bits";

            public bool CheckCCErrors{get;set;}

            public bool CheckPidErrors{get;set;}

            public bool ExcludeFromMuxerRestarts {get;set;}

            public int MaxBitRate 
            {
                get
                {
                    return maxBitRate_?? SourceStream.BitRate;
                }
                set
                {
                    maxBitRate_ = value;   
                }
            }

            public int? MinBitRate {get;set;}

            public int? OutputPid {get;set;}

            public MatchTypes MatchType {get;set;}

            public int ? MatchPid {get;set;}

            private string MatchTypeToStr()
            {
                switch (MatchType)
                {
                    case MatchTypes.Video: return "video";
                    case MatchTypes.Audio: return "audio";
                    case MatchTypes.Pid: return "any";
                    default: return "--";
                }
            }

            public void WriteToXml(XmlWriter xw)
            {
                xw.WriteStartElement("ElementaryStreamSpecification");
                xw.WriteAttributeString("bitRateDisplayUnits", BitRateDisplayUnits);
                xw.WriteAttributeString("checkCCErrors", CheckCCErrors.BoolToStr());
                xw.WriteAttributeString("checkPidErrors", CheckPidErrors.BoolToStr());
                xw.WriteAttributeString("excludeFromMuxerRestarts", ExcludeFromMuxerRestarts.BoolToStr());
                xw.WriteAttributeString("matchType", MatchTypeToStr());
                if (MatchType == MatchTypes.Pid)
                {
                    xw.WriteAttributeString("matchPid", MatchPid.ToString());
                }
                xw.WriteAttributeString("maxBitRate", MaxBitRate.ToString());
                if (MinBitRate.HasValue)
                {
                    xw.WriteAttributeString("minBitRate", MinBitRate.Value.ToString());
                }
                if (OutputPid.HasValue)
                {
                    xw.WriteAttributeString("outputPid", OutputPid.Value.ToString());
                }
                xw.WriteEndElement();
            }
        }

        private int? outputMuxRate_;

        public string Name {get;set;}

        public InputProfile SourceProfile {get;set;}

        public int OutputMuxRate 
        {
            get 
            {
                return outputMuxRate_?? SourceProfile.MuxRate;
            }
        
            set
            {
                outputMuxRate_ = value;
            }
        }

        public string OutputMuxRateDisplayUnits {get;set;}

        public int PCRPid {get;set;}

        /**
         'PCRRepetitionRate_ms'
         */
        public int PCRRepetitionRate {get;set;} = 25;

        public bool CompactingPacketizer {get;set;} = true;

        public double DVBSIEmitsPerSecond {get;set;} = 0.5;

        public double PSIEmitsPerSecond {get;set;} = 10;


        public bool IgnoreStreamErrors {get;set;} = false;

        public List<OutputElementaryStream> ElementaryStreams = new List<OutputElementaryStream>();

        public List<OutputProgram> OutputPrograms = new List<OutputProgram>();

        private OutputElementaryStream MatchElementaryStream(OutputElementaryStream.MatchTypes matchType, int? pid)
        {
            var oes = new OutputElementaryStream
            {
                MatchType = matchType
            };
            if (matchType == OutputElementaryStream.MatchTypes.Pid)
            {
                foreach (var es in SourceProfile.ElementaryStreams)
                {
                    if (es.Pid == pid.Value)
                    {
                        oes.MatchPid = es.Pid;
                        oes.SourceStream = es;
                        return oes;
                    }
                }
            }
            else
            {
                ElementaryStream.Types targetType;
                switch (matchType)
                {
                    case OutputElementaryStream.MatchTypes.Video:
                        targetType = ElementaryStream.Types.Video;
                        break;
                    case OutputElementaryStream.MatchTypes.Audio:
                        targetType = ElementaryStream.Types.Audio;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        return null;
                }
                foreach (var es in SourceProfile.ElementaryStreams)
                {
                    if (pid.HasValue)
                    {
                        if (es.Pid == pid.Value && es.Type == targetType)
                        {
                            oes.MatchPid = es.Pid;
                            oes.SourceStream = es;
                            return oes;
                        }
                    }
                    else
                    {
                        oes.SourceStream = es;
                        return oes;
                    }
                }
            }
            return null;
        }

        public IEnumerable<OutputElementaryStream> MatchElementaryStreams(IEnumerable<Tuple<OutputElementaryStream.MatchTypes, int?>> matches)
        {
            foreach (var match in matches)
            {
                var oes = MatchElementaryStream(match.Item1, match.Item2);
                if (oes != null)
                {
                    yield return oes;
                }
            }
        }

        public class ElementaryStreamSelectionTraits
        {
            public OutputElementaryStream.MatchTypes MatchType;
            public int? Pid;

            public bool RateTracking = false;

            public delegate void SetElementaryStreDelegate(OutputElementaryStream oes);

            public SetElementaryStreDelegate SetBitRates;
            public SetElementaryStreDelegate SelectDisplayBits;
            public bool CheckCCError;
            public bool CheckPidError;
            public bool ExcludeFromMuxerRestarts;
            public int OutputPid;

            public ElementaryStreamSelectionTraits()
            {
                SelectDisplayBits = DefaultSelectDisplayBits;
                SetBitRates = DefaultSetRates;
            }

            private void DefaultSelectDisplayBits(OutputElementaryStream oes)
            {
                var br = oes.MinBitRate.HasValue? oes.MinBitRate.Value : oes.MaxBitRate;
                if (br > 1000000)
                {
                    oes.BitRateDisplayUnits = "megabits";
                }
                else if (br > 1000)
                {
                    oes.BitRateDisplayUnits = "kilobits";
                }
                else
                {
                    oes.BitRateDisplayUnits = "bits";
                }
            }

            private void DefaultSetRates(OutputElementaryStream oes)
            {
                if (MatchType == OutputElementaryStream.MatchTypes.Video)
                {
                    oes.MaxBitRate = oes.SourceStream.BitRate;
                    if (RateTracking) oes.MinBitRate =  (int)Math.Round(0.8 * oes.SourceStream.BitRate);
                    else oes.MinBitRate = null;
                }
                else
                {
                    if (RateTracking) oes.MinBitRate =  (int)Math.Round(0.2 * oes.SourceStream.BitRate);
                    else oes.MinBitRate = null;
                }
            }

            public void SuggestTraits()
            {
                if (MatchType == OutputElementaryStream.MatchTypes.Video)
                {
                    CheckCCError = true;
                }
                else
                {
                    ExcludeFromMuxerRestarts = true;
                }
            }
        }

        public static IEnumerable<ElementaryStreamSelectionTraits> SuggestTraits(IEnumerable<ElementaryStreamSelectionTraits> traitsQueue)
        {
            foreach (var traits in traitsQueue)
            {
                traits.SuggestTraits();
                yield return traits;
            }
        }

        public void GenerateFromSource(IEnumerable<ElementaryStreamSelectionTraits> traitsQueue)
        {
            ElementaryStreams.Clear();
            foreach (var traits in traitsQueue)
            {
                var oes = MatchElementaryStream(traits.MatchType, traits.Pid);
                if (oes == null) continue;

                var es = oes.SourceStream;
                if (traits.SetBitRates != null)
                {
                    traits.SetBitRates(oes);
                }
                else
                {
                    oes.MaxBitRate = es.BitRate;
                    oes.MinBitRate = null;
                }
                oes.CheckCCErrors = traits.CheckCCError;
                oes.CheckPidErrors = traits.CheckPidError;
                oes.ExcludeFromMuxerRestarts = traits.ExcludeFromMuxerRestarts;
                oes.OutputPid = traits.OutputPid;
                if (traits.SelectDisplayBits != null)
                {
                    traits.SelectDisplayBits(oes);
                }
                else
                {
                    oes.BitRateDisplayUnits = "bits";
                }
                ElementaryStreams.Add(oes);
            }
            OutputMuxRate = SourceProfile.MuxRate;
        }

        public OutputElementaryStream FirstVideo()
        {
            return ElementaryStreams.FirstOrDefault(x=>x.MatchType == OutputElementaryStream.MatchTypes.Video);
        }
       
        public void WriteToXml(XmlWriter xw)
        {
            xw.WriteStartElement("Profile");
            xw.WriteAttributeString("name", Name);
                xw.WriteStartElement("OutputMuxRate");
                xw.WriteString(OutputMuxRate.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("OutputMuxRateDisplayUnits");
                xw.WriteString(OutputMuxRateDisplayUnits);
                xw.WriteEndElement();
                xw.WriteStartElement("PCRPid");
                xw.WriteString(PCRPid.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("PCRRepetitionRate_ms");
                xw.WriteString(PCRRepetitionRate.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("CompactingPacketizer");
                xw.WriteString(CompactingPacketizer.BoolToStr());
                xw.WriteEndElement();
                xw.WriteStartElement("DVBSIEmitsPerSecond");
                xw.WriteString(DVBSIEmitsPerSecond.RoundToSciNotation());
                xw.WriteEndElement();
                xw.WriteStartElement("PSIEmitsPerSecond");
                xw.WriteString(PSIEmitsPerSecond.RoundToSciNotation());
                xw.WriteEndElement();
                xw.WriteStartElement("ElementaryStreams");
                xw.WriteAttributeString("ignoreErrors", IgnoreStreamErrors.BoolToStr());
                    foreach (var es in ElementaryStreams)
                    {
                        es.WriteToXml(xw);
                    }
                xw.WriteEndElement();
                xw.WriteStartElement("OutputPrograms");
                    foreach (var op in OutputPrograms)
                    {
                        op.WriteToXml(xw);
                    }
                xw.WriteEndElement();
            xw.WriteEndElement();
        }
    }
}
