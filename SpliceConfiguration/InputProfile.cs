using System.Collections.Generic;
using System.Xml;

namespace SpliceConfiguration
{
    class InputProfile
    {
        public int MuxRate {get ;set;}

        public List<ElementaryStream> ElementaryStreams {get;} = new List<ElementaryStream>();


        /**
         The input program that owns this profile. Doesn't have to be set if not needed.
         */
        public InputProgram Owner {get;set;}

        public int MaxGopLength { get; set; }
    }
}