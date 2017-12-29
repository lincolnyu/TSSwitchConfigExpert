using System.Xml;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    class Profile: IXmlWritable
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

        public BasicProfile SourceProfile {get;set;}

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

        public void GenerateFromSourceProfile()
        {
            // TODO ...
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
