using System.Xml;

namespace SpliceConfiguration
{
    public class ElementaryStream
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

        /**
         Only applicable for videos
         */
        public int MaxGopLength { get; set; }

    }
}