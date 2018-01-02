using System;
using System.Linq;
using System.Collections.Generic;

namespace SpliceConfiguration
{
    public static class Helper
    {
        static Random random_ = new Random();

        public static string RoundToStr(this double val, int precision = 1)
        {
            var fmt = $"{{0:F{precision}}}";
            return string.Format(fmt, val);
        }

        public static string RoundToSciNotation(this double val, int precision = 6, bool uppercase = false)
        {
            var fmt = uppercase? $"{{0:E{precision}}}" : $"{{0:e{precision}}}";
            return string.Format(fmt, val);
        }

        public static string BoolToStr(this bool val)
        {
            return val.ToString().ToLower();
        }

        public static IEnumerable<int> GetAllElementaryStreamPids(this InputProfile inputProfile)
        {
            return inputProfile.ElementaryStreams.Select(x=>x.Pid);    
        }

        public static IEnumerable<int> GetAllElementaryStreamPids(this InputProgram inputProgram)
        {
            return inputProgram.Profile.GetAllElementaryStreamPids();
        }

        public static void LoopInc(ref int i, int min, int max)
        {
            i++;
            if (i > max) i = min;
        }

        public static int GenerateRandomPid(this IEnumerable<int> excluded, int min = 32, int max = 8186)
        {
            return excluded.GenerateRandomPid(random_, min, max);
        }

        public static int GenerateRandomPid(this IEnumerable<int> excluded, Random rand, int min = 32, int max = 8186)
        {
            var set = new HashSet<int>();
            foreach (var e in excluded)
            {
                set.Add(e);
            }
            var initialV = rand.Next(min, max+1);
            var v = initialV;
            while (true)
            {
                if (!set.Contains(v)) return v;
                LoopInc(ref v, min, max);
                if (v == initialV) return -1;
            }
        }

        public static void EnforceOwnerReferences(this SplicerConfig config)
        {
            foreach (var input in config.Inputs)
            {
                foreach (var inputProgram in input.InputPrograms)
                {
                    inputProgram.Owner = input;
                    inputProgram.Profile.Owner = inputProgram;
                }
            }
        }

        public static DateTime MMDDToDate(this string mmdd)
        {
            
            var date = new DateTime(DateTime.UtcNow.Year, 
                int.Parse(mmdd.Substring(0,2)),
                int.Parse(mmdd.Substring(2,2)));
            return date;
        }

        public static string DateToMMDD(this DateTime date)
        {
            return $"{date.Month:D2}{date.Day:D2}";
        }


        public static DateTime HHMMSSToTime(this string hhmmss)
        {
            var now = DateTime.UtcNow;
            var time = new DateTime(now.Year, now.Month, now.Day,
                int.Parse(hhmmss.Substring(0,2)),
                int.Parse(hhmmss.Substring(2,2)),
                int.Parse(hhmmss.Substring(4,2)));
            return time;
        }

        public static DateTime CombineDateTime(DateTime date, DateTime time)
        {
            return new DateTime(date.Year, date.Month, date.Day, 
                time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public static string TimeToHHMMSS(this DateTime time)
        {
            return $"{time.Hour:D2}{time.Minute:D2}{time.Second:D2}";
        }

        public static DateTime HHMMToTime(this string hhmm)
        {
            var now = DateTime.UtcNow;
            var time = new DateTime(now.Year, now.Month, now.Day,
                int.Parse(hhmm.Substring(0,2)),
                int.Parse(hhmm.Substring(2,2)),
                0);
            return time;
        }

        public static string TimeToHHMM(this DateTime time)
        {
            return $"{time.Hour:D2}{time.Minute:D2}";
        }

        public static TimeSpan HHMMToSpan(this string hhmm)
        {
            var span = new TimeSpan(
                int.Parse(hhmm.Substring(0,2)),
                int.Parse(hhmm.Substring(2,2)),
                0);
            return span;
        }

        public static string SpanToHHMM(this TimeSpan span)
        {
            return $"{span.Hours:D2}{span.Minutes:D2}";
        }

        public static TimeSpan HHMMSSToSpan(this string hhmmss)
        {
            var span = new TimeSpan(
                int.Parse(hhmmss.Substring(0,2)),
                int.Parse(hhmmss.Substring(2,2)),
                int.Parse(hhmmss.Substring(4,2)));
            return span;
        }

        public static string SpanToHHMMSS(this TimeSpan span)
        {
            return $"{span.Hours:D2}{span.Minutes:D2}{span.Seconds:D2}";
        }

        public static TimeSpan HHMMSSCCToSpan(this string hhmmsscc)
        {
            var span = new TimeSpan(
                int.Parse(hhmmsscc.Substring(0,2)),
                int.Parse(hhmmsscc.Substring(2,2)),
                int.Parse(hhmmsscc.Substring(4,2)),
                int.Parse(hhmmsscc.Substring(6,2))*10);
            return span;
        }

        public static string SpanToHHMMSSCC(this TimeSpan span)
        {
            return $"{span.Hours:D2}{span.Minutes:D2}{span.Seconds:D2}{span.Milliseconds/10:D2}";
        }
    }
}