using System.Xml;

namespace SpliceConfiguration
{
    class OutputProgram : IXmlWritable
    {
        public int PmtPid {get;set;}
        public int ProgramNumber{get;set;}
        public string ServiceName {get;set;}
        public string ServiceProviderName {get;set;}

        public void WriteToXml(XmlWriter xw)
        {
            xw.WriteStartElement("OutputProgram");
            xw.WriteAttributeString("pmtPid", PmtPid.ToString());
            xw.WriteAttributeString("programNumber", ProgramNumber.ToString());
            if (ServiceName != null)
            {
                xw.WriteAttributeString("serviceName", ServiceName);
            }
            if (ServiceProviderName != null)
            {
                xw.WriteAttributeString("serviceProviderName", ServiceProviderName);
            }
            xw.WriteEndElement();
        }
    }
}