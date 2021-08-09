using System;

namespace HumanDateParser.Tokenisation.Tokens
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
