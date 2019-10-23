using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal class NumberToken : Token
    {
        public int Value { get; }

        public NumberToken(int value) : base(TokenKind.Number, value.ToString())
        {
            Value = value;
        }
    }
}
