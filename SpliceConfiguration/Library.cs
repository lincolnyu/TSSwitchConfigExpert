using System.Xml;

namespace SpliceConfiguration
{
    class Library : IXmlWritable
    {
        public string Extension {get;set;}

        public string LibraryType {get;set;}

        public string Location {get;set;}

        public string Name {get;set;}

        public void WriteToXml(XmlWriter xw)
        {
            xw.WriteStartElement("Library");
            xw.WriteAttributeString("extension", Extension);
            xw.WriteAttributeString("libraryType", LibraryType);
            xw.WriteAttributeString("location", Location);
            xw.WriteAttributeString("name", Name);
            xw.WriteEndElement();
        }

        public static Library CreateTSLibrary(string name, string location)
            => new Library 
            {
                Extension = "ts",
                LibraryType = "SPLICE_ASSET",
                Name = name,
                Location = location
            };
    }
}
