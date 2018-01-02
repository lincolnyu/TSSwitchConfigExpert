using System.Xml;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    public class Channel : IXmlWritable
    {
        public class JamPrevention
        {
            public double NoticeTimeSeconds {get;set;}
            public double NoticeAccuracySeconds {get;set;}
        }

        public string AccuracyMode {get;set;}
        public bool EnableRateTracking{get;set;}
        public bool EnableResumption{get;set;}

        public double InputBufferSeconds {get;set;} = 5.0;

        public Input Input {get;set;}

        public int MaxGopLength {get;set;}

        public double SpliceActivationAdjustment {get;set;} = 10.0;

        public bool TolerantTimeCodes {get;set;}

        public string Name {get;set;}

        public InputProgram PrimaryProgram {get;set;}

        public Profile Profile {get;set;}

        public SCTE35Trigger SCTE35Config {get;set;}

        public JamPrevention JamPrev {get;set;}

        public List<InputProgram> Additional = new List<InputProgram>();
        
        public void WriteToXml(XmlWriter xw)
        {
            xw.WriteStartElement("Channel");
            xw.WriteAttributeString("accuracyMode", AccuracyMode);
            xw.WriteAttributeString("enableRateTracking", EnableRateTracking.BoolToStr());
            xw.WriteAttributeString("enableResumption", EnableResumption.BoolToStr());
            xw.WriteAttributeString("inputBufferSeconds", InputBufferSeconds.RoundToStr(1));
            xw.WriteAttributeString("inputName", Input.Name);
            xw.WriteAttributeString("maxGopLength", MaxGopLength.ToString());
            xw.WriteAttributeString("name", Name);
            xw.WriteAttributeString("primaryProgramName", PrimaryProgram.Name);
            xw.WriteAttributeString("profile", Profile.Name);
            if (SCTE35Config != null)
            {
                xw.WriteAttributeString("scte35Config", SCTE35Config.Name);
            }
            
            xw.WriteAttributeString("spliceActivationAdjustment", SpliceActivationAdjustment.RoundToStr(1));
            xw.WriteAttributeString("tolerantTimeCodes", TolerantTimeCodes.BoolToStr());
            
                if (JamPrev != null)
                {
                    xw.WriteStartElement("JamPrevention");
                        xw.WriteStartElement("noticeTimeSeconds");
                            xw.WriteString(JamPrev.NoticeTimeSeconds.RoundToStr(1));
                        xw.WriteEndElement();
                        xw.WriteStartElement("noticeAccuracySeconds");
                            xw.WriteString(JamPrev.NoticeAccuracySeconds.RoundToStr(1));
                        xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                xw.WriteStartElement("AdditionalCrosspoints");
                xw.WriteEndElement();

            xw.WriteEndElement();
        }
    }
}