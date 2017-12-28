using System.Collections.Generic;
using System.Xml;

namespace SpliceConfiguration
{
    class BasicProfile
    {
        public int MuxRate {get ;set;}

        public List<ElementaryStream> ElementaryStreams {get;} = new List<ElementaryStream>();
    }
}