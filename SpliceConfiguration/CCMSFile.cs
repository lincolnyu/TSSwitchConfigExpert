using System;
using System.IO;

namespace SpliceConfiguration
{
    class CCMSFile
    {
        public string FileName {get;set;}

        public DateTime Date {get;private set;}

        public int NetworkId {get; private set;}

        /**
         'zoneID' in splicer2server_config
         */
        public int HeadEnd {get; private set;}

        public CCMSFile(DateTime date, int netowrkId, int headend)
        {

        }

        public void Write(TextWriter tw)
        {
            
        }
    }
}
