using System;

namespace HumanDateParser
{
    internal class DateToken : IParseToken
    {
        public DateTime Value { get; }

        public DateToken(DateTime date)
        {
            Value = date;
        }
    }
}
