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
    }
}