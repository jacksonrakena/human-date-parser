using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal class TimeUnitToken : ParseToken
    {
        public TimeUnit Unit { get; }

        internal TimeUnitToken(TimeUnit unit) : base(TokenKind.Unit, string.Empty)
        {
            Unit = unit;
        }
    }

    internal enum TimeUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year
    }
}
