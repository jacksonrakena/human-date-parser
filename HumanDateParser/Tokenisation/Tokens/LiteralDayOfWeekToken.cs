using System;

namespace HumanDateParser.Tokenisation.Tokens
{
    internal class LiteralDayOfWeekToken : IParseToken
    {
        public DayOfWeek DayOfWeek { get; }

        internal LiteralDayOfWeekToken(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
        }

        public static DayOfWeek? ParseDayOfWeek(string text)
        {
            if (!Enum.TryParse<DayOfWeek>(text, true, out var day)) return null;
            return day;
        }
    }
}
