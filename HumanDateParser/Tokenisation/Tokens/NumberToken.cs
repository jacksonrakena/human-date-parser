using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal class NumberToken : IParseToken
    {
        public int Value { get; }

        public NumberToken(int value)
        {
            Value = value;
        }
    }
}
