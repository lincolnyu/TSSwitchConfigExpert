using System.Xml;

namespace SpliceConfiguration
{
    class InputProgram : IXmlWritable
    {
        /**
         'name' attribute
         */
        public string Name { get; set; }

        /**
         'number' attribute that uniquly identifies the input program in the config
         */
        public string ProgramId {get; set;}

        /**
         'programNumber' attribute, which is the program number in the TS
         */
        public int ProgramNumber {get; set;}

        public BasicProfile Profile {get;set;}

        public void WriteToXml(XmlWriter xw)
        {
            // TODO make more fields customizable
            xw.WriteStartElement("Program");
            xw.WriteAttributeString("fallback", "true");
            xw.WriteAttributeString("name", Name);
            xw.WriteAttributeString("number", ProgramId);
            xw.WriteAttributeString("programNumber", ProgramNumber.ToString());
            xw.WriteAttributeString("programType", "STANDARD");
            xw.WriteEndElement();
        }
    }
}
