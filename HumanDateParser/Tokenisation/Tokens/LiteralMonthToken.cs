using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal class LiteralMonthToken : ParseToken
    {
        public int Month { get; }

        internal LiteralMonthToken(string month) : base(TokenKind.AbsoluteMonth, month)
        {
            Month = 
        }
    }
}
