using System;
using System.IO;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    public class CCMSFile
    {
        // File name:  MDDNNHHH.XXX
        //  M:   1 - CS
        //  DD:  01 - 31
        //  NN:  network ID within the headend
        //  HHH: headend also known as Zone ID
        //  XXX: SCH Schedule; VER: Ver_List

        public class Record
        {
            public string EventType;
            public DateTime ScheduledDateTime;

            public DateTime WindowStartTime;
            public TimeSpan WindowDuration;

            public int BreakNumber;
            public int PositionNumber;
            public TimeSpan ScheduledLength;
            public DateTime ActualAiredTime;
            public TimeSpan ActualAiredLength;
            public int ActualAiredPosition;
            public string SpotId;
            public int StatusCode;
            public string Remaining; // TODO refine later

            public Record Clone()
            {
                var clone = new Record
                {
                    ScheduledDateTime = ScheduledDateTime,
                    WindowStartTime = WindowStartTime,
                    WindowDuration = WindowDuration,
                    BreakNumber = BreakNumber,
                    PositionNumber = PositionNumber,
                    ScheduledLength = ScheduledLength,
                    ActualAiredTime = ActualAiredTime,
                    ActualAiredLength = ActualAiredLength,
                    ActualAiredPosition = ActualAiredPosition,
                    SpotId = SpotId,
                    StatusCode = StatusCode,
                    Remaining = Remaining
                };
                return clone;
            }

            public static Record Parse(string line)
            {
                var r = new Record
                {
                    EventType = line.Substring(0,3),
                    ScheduledDateTime = Helper.CombineDateTime(line.Substring(4,4).MMDDToDate(), line.Substring(9,5).HHMMSSToTime()),
                    WindowStartTime = line.Substring(16,4).HHMMSSToTime(),
                    WindowDuration = line.Substring(21,4).HHMMToSpan(),
                    BreakNumber = int.Parse(line.Substring(26,3)),
                    PositionNumber = int.Parse(line.Substring(30,3)),
                    ScheduledLength = line.Substring(34,6).HHMMSSToSpan(),
                    ActualAiredTime = line.Substring(41,6).HHMMSSToTime(),
                    ActualAiredLength = line.Substring(48,8).HHMMSSCCToSpan(),
                    ActualAiredPosition = int.Parse(line.Substring(57,3)),
                    SpotId = line.Substring(61, 11),
                    StatusCode = int.Parse(line.Substring(73,4)),
                    Remaining = line.Substring(78)
                };
                return r;
            }

            public override string ToString()
            {
                return $"{EventType} {ScheduledDateTime.DateToMMDD()} {ScheduledDateTime.TimeToHHMMSS()} {WindowStartTime.TimeToHHMM()} {WindowDuration.SpanToHHMM()} "
                    + $"{BreakNumber:D3} {PositionNumber:D3} {ScheduledLength.SpanToHHMMSS()} {ActualAiredTime.TimeToHHMMSS()} {ActualAiredLength.SpanToHHMMSSCC()} "
                    + $"{ActualAiredPosition:D3} {SpotId.PadLeft(11, '0')} {StatusCode:D4} {Remaining}";
            }
        }

        public string FileName {get;set;}

        public DateTime Date {get;private set;}

        public int NetworkId {get; private set;}

        /**
         'zoneID' in splicer2server_config
         */
        public int Headend {get; private set;}

        public List<Record> Records {get;} = new List<Record>();

        public CCMSFile(DateTime date, int networkId, int headend)
        {
            Date =  date;
            NetworkId = networkId;
            Headend = headend;
            FileName = GetCCMSFileName(date, networkId, headend);
        }

        private CCMSFile()
        {
        }

        public void CopyRecordsFrom(CCMSFile other, bool enforceDates = false)
        {
            Records.Clear();
            foreach (var r in other.Records)
            {
                var rc = r.Clone();
                Records.Add(rc);
            }
            if (enforceDates)
            {
                EnforceDates();
            }
        }

        public static CCMSFile LoadTemplate(TextReader tr)
        {
            var c = new CCMSFile();
            while (true)
            {
                var line = tr.ReadLine();
                if (line == null) break;
                if (line.StartsWith("REM")) continue;
                var r = Record.Parse(line);
                c.Records.Add(r);
            }
            return c;
        }

        public void EnforceDates()
        {
            foreach (var r in Records)
            {
                r.ScheduledDateTime = Helper.CombineDateTime(Date, r.ScheduledDateTime);
            }
        }

        public void Write(TextWriter tw)
        {
            foreach (var r in Records)
            {
                tw.WriteLine(r.ToString());
            }
        }

        public static string GetCCMSFileName(DateTime date, int networkId, int headend)
        {
            char[] monthChars = {'1','2','3','4','5','6','7','8','9','a','b','c'};
            return $"{monthChars[date.Month]}{date.Day:D2}{networkId:D2}{headend:D3}";
        }

        public static void RandomizeSpotIds(IEnumerable<CCMSFile> files)
        {
            var rand = new Random();
            var used = new HashSet<string>();
            foreach (var file in files)
            {
                foreach (var r in file.Records)
                {
                    string id;
                    do
                    {
                        id = rand.Next(int.MaxValue).ToString();
                    } while (used.Contains(id));
                    used.Add(id);
                    r.SpotId = id;
                }
            }
        }
    }
}

/**
  Sample CCMS File:

REM ===============================> 12/11/2017 14:50:38 Channel: 08 Language: CRO Format: SD Schedule date: 1221 Schedule download Id 310522
REM date time   start dur brk pos length time   length   pos   media id  stat                                  Spot
REM MMDD HHMMSS HHMM HHMM ### ### HHMMSS HHMMSS HHMMSSCC ### ##SPOT_ID## #### Client Name..................... Description
LOI 0102 045107 0000 2359 001 001 000005 000000 00000000 000 13685575    0000 -------------------------------- TITLE GO OD
LOI 0102 045112 0000 2359 001 002 000011 000000 00000000 000 17461994    0000 -------------------------------- HOLIDAY JOY
LOI 0102 045123 0000 2359 001 003 000126 000000 00000000 000 18968999    0000 -------------------------------- HBO3 IMAGE NOVEM
LOI 0102 045249 0000 2359 001 004 000044 000000 00000000 000 19240929    0000 -------------------------------- LIFE 2017.12.31 
LOI 0102 045333 0000 2359 001 005 000053 000000 00000000 000 19408341    0000 -------------------------------- GUARDIANS OF THE
LOI 0102 045426 0000 2359 001 006 000147 000000 00000000 000 19273714    0000 -------------------------------- MONTHLY IMAGE DE
LOI 0102 045613 0000 2359 001 007 000114 000000 00000000 000 19416212    0000 -------------------------------- LOGAN 2018.01.14
LOI 0102 045727 0000 2359 001 008 000043 000000 00000000 000 19285292    0000 -------------------------------- BRITANNIA
LOI 0102 045810 0000 2359 001 009 000045 000000 00000000 000 18901177    0000 -------------------------------- VICTORIA II TRAI
LOI 0102 045855 0000 2359 001 010 000051 000000 00000000 000 18959912    0000 -------------------------------- PIRATES OF THE C
LOI 0102 045946 0000 2359 001 011 000005 000000 00000000 000 13686450    0000 -------------------------------- COPYRIGHT
LOI 0102 050000 0000 2359 002 001 000005 000000 00000000 000 13686450    0000 -------------------------------- COPYRIGHT
LOI 0102 050005 0000 2359 002 002 000039 000000 00000000 000 19063890    0000 -------------------------------- NORMAN THE MODER
LOI 0102 050044 0000 2359 002 003 000008 000000 00000000 000 15025847    0000 -------------------------------- OPEN FILM
LOI 0102 050052 0000 2359 002 004 000005 000000 00000000 000 13685575    0000 -------------------------------- TITLE GO OD
LOI 0102 050057 0000 2359 002 005 000005 000000 00000000 000 13686696    0000 -------------------------------- 12 DOLBY
 */
