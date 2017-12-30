using System.Xml;

namespace SpliceConfiguration
{
    class ElementaryStream
    {
        public enum Types
        {
            Video,
            Audio,
            Any
        }

        public int Pid {get;set;}

        public Types Type {get;set;}

        public int BitRate {get;set;}
    }
}