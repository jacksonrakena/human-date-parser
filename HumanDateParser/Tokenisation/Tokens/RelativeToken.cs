namespace HumanDateParser.Tokenisation.Tokens
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
