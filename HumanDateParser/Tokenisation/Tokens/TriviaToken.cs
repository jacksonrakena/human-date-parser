using System;
using System.Collections.Generic;
using System.Text;

namespace HumanDateParser
{
    internal enum TriviaType
    {
        At,
        Dash,
        In,
        Am,
        Pm,
        To,
        Colon
    }

    internal class TriviaToken : IParseToken
    {
        public TriviaType TriviaType { get; }

        public TriviaToken(TriviaType type)
        {
            TriviaType = type;
        }
    }
}
