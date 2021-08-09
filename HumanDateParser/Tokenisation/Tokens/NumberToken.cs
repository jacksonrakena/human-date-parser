namespace HumanDateParser.Tokenisation.Tokens
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
