using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HumanDateParser
{
    internal class TokenBuffer : Enumerator<Token>
    {
        public TokenBuffer(Tokeniser tokeniser)
        {
            _list = tokeniser.Tokenise();
            Debug.WriteLine($"Tokeniser finished: " + string.Join(", ", _list.Select(c => c.Kind.ToString())));
        }
    }
}
