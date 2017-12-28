using System.Xml;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    class Input : IXmlWritable
    {
        /**
         'name' attribute
         */
        public string Name { get; set;}

        /**
          uri attribute
         */
        public string Uri {get; set;}

        /**
         'api_id' atribute
         */
        public string ApiId{get; set;}

        public List<InputProgram> InputPrograms { get; } = new List<InputProgram>();

        public void WriteToXml(XmlWriter xw)
        {
            // TODO make more fields customizable
            xw.WriteStartElement("Input");
            xw.WriteAttributeString("autoReturnToMainTimeout", "60");
            xw.WriteAttributeString("name", Name);
            xw.WriteAttributeString("switchMode", "AUTO");
            xw.WriteAttributeString("uri", Uri);
            xw.WriteAttributeString("api_id", ApiId);
                foreach (var ip in InputPrograms)
                {
                    ip.WriteToXml(xw);
                }               
            xw.WriteEndElement();
        }
    }
}
