namespace HumanDateParser
{
    /// <summary>
    ///     A parse token.
    /// </summary>
    public class ParseToken
    {
        /// <summary>
        ///     The raw text of the token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     The kind of the token.
        /// </summary>
        public TokenKind Kind { get; set; }

        internal ParseToken(TokenKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }
    }
}