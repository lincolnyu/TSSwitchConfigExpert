using System.Xml;

namespace SpliceConfiguration
{
    class SCTE35Trigger : IXmlWritable
    {
        public string Name {get;set;}

        public bool AlwaysAutoReturn {get;set;} = true;

        public Library Library {get;set;}

        public string NetworkId {get;set;}

        public bool SequantialPlaylist {get;set;} = true;

        public string ZoneId {get;set;}

        public string MissingAssetPlaceholder {get;set;}

        public bool SkipMissingAssets {get;set;} = false;

        public bool UseRealFileDurations {get;set;} = false;

        public void WriteToXml(XmlWriter xw)
        {
            xw.WriteStartElement("SCTE35");
            xw.WriteAttributeString("name", Name);
                xw.WriteStartElement("ccms");
                xw.WriteAttributeString("alwaysAutoReturn", AlwaysAutoReturn.BoolToStr());
                xw.WriteAttributeString("library", Library.Name);
                xw.WriteAttributeString("networkID", NetworkId);
                xw.WriteAttributeString("zoneID", ZoneId);
                xw.WriteAttributeString("sequentialPlaylist", SequantialPlaylist.BoolToStr());
                    xw.WriteStartElement("AssetConfig");
                    xw.WriteAttributeString("missingAssetPlaceholder", MissingAssetPlaceholder);
                    xw.WriteAttributeString("skipMissingAssets", SkipMissingAssets.BoolToStr());
                    xw.WriteAttributeString("useRealFileDurations", UseRealFileDurations.BoolToStr());
                    xw.WriteEndElement();
                xw.WriteEndElement();
            xw.WriteEndElement();
        }
    }
}
