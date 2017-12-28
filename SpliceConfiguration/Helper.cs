namespace SpliceConfiguration
{
    public static class Helper
    {
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
    }
}