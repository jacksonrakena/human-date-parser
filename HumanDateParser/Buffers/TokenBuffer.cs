using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HumanDateParser
{
    internal class TokenBuffer : Buffer<Token>
    {
        public bool ContainsKind(TokenKind kind) => Any(t => t.Kind == kind);

        public TokenBuffer(Tokeniser tokeniser)
        {
            _list = tokeniser.Tokenise();
            Debug.WriteLine($"Tokeniser finished: " + string.Join(", ", _list.Select(c => c.Kind.ToString())));
        }
    }
}
