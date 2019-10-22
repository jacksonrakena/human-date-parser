using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanDateParser
{
    internal class TokenBuffer
    {
        private int _position = 0;
        private readonly List<Token> _tokens = new List<Token>();
        private readonly Tokeniser _tokeniser;

        public Token CurrentToken => _tokens[_position];

        public TokenBuffer(Tokeniser tokeniser)
        {
            _tokeniser = tokeniser;
            var token = _tokeniser.NextToken();
            while (token.Kind != TokenKind.BufferReadEnd)
            {
                _tokens.Add(token);
                token = _tokeniser.NextToken();
            }
            _tokens.Add(token);

            Console.WriteLine($"Tokenizer: " + string.Join(", ", _tokens.Select(c => c.Kind.ToString())));
        }

        public Token? PeekNext() => Peek(1);

        public Token? Peek(int position)
        {
            var np = _position + position;
            if (np >= _tokens.Count) return null;
            return _tokens[np];
        }
        
        public bool MoveNext()
        {
            _position++;
            return _position <= _tokens.Count;
        }
    }
}
