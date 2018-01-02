using System.Xml;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    public class SplicerConfig : IXmlWritable
    {
        public List<Input> Inputs {get;} = new List<Input>();

        public List<Output> Outputs {get;} = new List<Output>();

        public List<Profile> Profiles {get;} = new List<Profile>();

        public List<Channel> Channels {get;} = new List<Channel>();

        public List<Library> Libraries {get;} = new List<Library>();

        public List<SCTE35Trigger> Triggers {get;} = new List<SCTE35Trigger>();

        public void WriteToXml(XmlWriter xw)
        {
            // TODO make default / hardcoded sections customizable

            xw.WriteStartElement("Splicer2ServerConfig");

            xw.WriteStartElement("Inputs");
                foreach (var input in Inputs)
                {
                    input.WriteToXml(xw);
                }
            xw.WriteEndElement();

            xw.WriteStartElement("ACEProfiles");
            xw.WriteEndElement();

            xw.WriteStartElement("InfuzeMappings");
                xw.WriteStartElement("KeyerMappings");
                xw.WriteEndElement();
                xw.WriteStartElement("TextCrawlMappings");
                xw.WriteEndElement();
                xw.WriteStartElement("FontMappings");
                xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("InfuzeProfiles");
            xw.WriteEndElement();

            xw.WriteStartElement("Profiles");
                foreach (var profile in Profiles)
                {
                    profile.WriteToXml(xw);
                }
            xw.WriteEndElement();

            xw.WriteStartElement("Channels");
                foreach (var channel in Channels)
                {
                    channel.WriteToXml(xw);
                }
            xw.WriteEndElement();

            xw.WriteStartElement("Outputs");
                foreach (var output in Outputs)
                {
                    output.WriteToXml(xw);
                }
            xw.WriteEndElement();

            xw.WriteStartElement("Libraries");
                foreach (var library in Libraries)
                {
                    library.WriteToXml(xw);
                }
            xw.WriteEndElement();

            xw.WriteStartElement("Triggers");
                xw.WriteStartElement("CCMSGlobals");
                xw.WriteAttributeString("databasePath", "mysql://mediaware:mediaware@localhost/ccms");
                xw.WriteAttributeString("verFileOutputDirectory", "/opt/mediaware/asrun/");
                xw.WriteEndElement();

                xw.WriteStartElement("TriggeringMethods");
                    foreach (var trigger in Triggers)
                    {
                        trigger.WriteToXml(xw);
                    }
                xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("Monitoring");
                xw.WriteStartElement("AsRun");
                    xw.WriteStartElement("FilenameTemplate");
                        xw.WriteString("/opt/mediaware/asrun/AsRun.log");
                    xw.WriteEndElement();
                    xw.WriteStartElement("DateType");
                        xw.WriteString("UTC");
                    xw.WriteEndElement();
                    xw.WriteStartElement("MaxDaysToKeep");
                        xw.WriteString("14");
                    xw.WriteEndElement();
                xw.WriteEndElement();
            xw.WriteEndElement();
            
            xw.WriteStartElement("Advanced");
                xw.WriteStartElement("StartupSpliceDelay_ms");
                    xw.WriteString("3000");
                xw.WriteEndElement();
                xw.WriteStartElement("TimeCodeSpliceAdjust");
                    xw.WriteString("00:00:00:00");
                xw.WriteEndElement();
                xw.WriteStartElement("EnableTeletextWeaving");
                    xw.WriteString("false");
                xw.WriteEndElement();
                xw.WriteStartElement("UDPPacketSize");
                    xw.WriteString("1316");
                xw.WriteEndElement();

                xw.WriteStartElement("BlankColor");
                    xw.WriteString("#000000");
                xw.WriteEndElement();
                xw.WriteStartElement("FileInfoDatabasePath");
                    xw.WriteString("mysql://mediaware:mediaware@localhost/fileinfo");
                xw.WriteEndElement();
                xw.WriteStartElement("BurninDatabasePath");
                    xw.WriteString("mysql://mediaware:mediaware@localhost/burnin");
                xw.WriteEndElement();
                xw.WriteStartElement("TimeZoneDatabasePath");
                    xw.WriteString("/opt/mediaware/instream2/config/date_time_zonespec.csv");
                xw.WriteEndElement();
                xw.WriteStartElement("InputMonitoring");
                xw.WriteAttributeString("ccErrorPeriod", "2");
                xw.WriteAttributeString("lowAURateThresholdPercentage", "60");
                xw.WriteAttributeString("maxCCErrorsInPeriod", "2");
                xw.WriteAttributeString("pidErrorPeriod", "2");
                xw.WriteAttributeString("timeoutSyncErrorPeriod", "5");
                xw.WriteEndElement();

                xw.WriteStartElement("Restarts");
                    xw.WriteStartElement("LongSpliceRequestSeconds");
                    xw.WriteAttributeString("timeout", "30.0");
                    xw.WriteEndElement();
                    xw.WriteStartElement("EmptyStreamSeconds");
                    xw.WriteAttributeString("timeout", "30.0");
                    xw.WriteEndElement();
                    xw.WriteStartElement("DroppedAUSeconds");
                    xw.WriteAttributeString("timeout", "1.0");
                    xw.WriteEndElement();
                    xw.WriteStartElement("LowAURateLeavingMuxer");
                    xw.WriteAttributeString("maxConsecutiveSecondsUnderThreshold", "5");
                    xw.WriteAttributeString("thresholdPercentage", "60");
                    xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteStartElement("Resumption");
                    xw.WriteAttributeString("databasePath", "mysql://mediaware:mediaware@localhost/resumption");
                xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteEndElement();
        }
    }
}
