using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
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
