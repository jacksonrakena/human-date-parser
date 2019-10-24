using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal enum RelativeType
    {
        Last,
        Next,
        Ago
    }

    internal class RelativeToken : IParseToken
    {
        public RelativeType RelativeType { get; }

        public RelativeToken(RelativeType type)
        {
            RelativeType = type;
        }
    }
}
