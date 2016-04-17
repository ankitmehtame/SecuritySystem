namespace SecuritySystem.SecurityAlarm
{
    public static class StringExtensions
    {
        public static int? ToNullableInt(this string text)
        {
            if (text == null)
                return null;
            int val;
            return int.TryParse(text, out val) ? val : (int?)null;
        }
    }
}
