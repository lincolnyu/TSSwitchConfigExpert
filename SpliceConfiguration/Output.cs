using System.Collections.Generic;
using System.Xml;

namespace SpliceConfiguration
{
    public class Output : IXmlWritable
    {
        public int Latency{get;set;} = 15;
        public string Name {get;set;}
        public int TxRate {get;set;}
        public string TxRateDisplayUnits {get;set;} = "megabits";
        public string Uri {get;set;}

        public string ApiId {get;set;}

        public List<Channel> Channels = new List<Channel>();

        public void WriteToXml(XmlWriter xw)
        {
            // TODO hardcoded stuff..
            xw.WriteStartElement("Output");
            xw.WriteAttributeString("latency", Latency.ToString());
            xw.WriteAttributeString("multiplexType", "SPTS");
            xw.WriteAttributeString("name", Name);
            xw.WriteAttributeString("serviceMode", "DVB");
            xw.WriteAttributeString("txRate", TxRate.ToString());
            xw.WriteAttributeString("txRateDisplayUnits", "megabits");
            xw.WriteAttributeString("uri", Uri);
            xw.WriteAttributeString("api_id", ApiId);
                xw.WriteStartElement("Channels");
                    foreach (var channel in Channels)
                    {
                        xw.WriteStartElement("Channel");
                        xw.WriteAttributeString("name", channel.Name);
                        xw.WriteEndElement();
                    }
                xw.WriteEndElement();
            xw.WriteEndElement();
        }
    }
}