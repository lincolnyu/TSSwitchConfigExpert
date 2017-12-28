using System.Xml;

namespace SpliceConfiguration
{
    interface IXmlWritable
    {
        void WriteToXml(XmlWriter xw);
    }
}