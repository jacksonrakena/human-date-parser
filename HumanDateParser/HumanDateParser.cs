using System;
using System.Collections.Generic;
using System.Linq;
using HumanDateParser.Tokenisation.Tokens;

namespace HumanDateParser
{
    /// <summary>
    ///     A class that can parse human-readable dates.
    /// </summary>
    public class HumanDateParser
    {
        private readonly ParserOptions _options;
        
        /// <summary>
        ///     Creates a new <see cref="HumanDateParser"/> using the provided options.
        ///     Providing <c>null</c> will use the default options.
        /// </summary>
        /// <param name="options">
        ///     The options to use. Defaults to the default options.
        /// </param>
        public HumanDateParser(ParserOptions? options = null)
        {
            _options = options ?? new ParserOptions();
        }
        /// <summary>
        ///     Parses a human-readable date into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateString">A human-readable date.</param>
        /// <returns>The provided date, in <see cref="DateTime"/> format.</returns>
        /// <exception cref="ParseException">An exception arises during parsing.</exception>
        public DateTimeOffset Parse(string dateString)
            => new Parser(dateString, _options).Parse();

        /// <summary>
        ///     Parses a human-readable date into a <see cref="DateTime"/>, as well as including
        ///     tools, debug information, and temporary data stored during parsing. This should
        ///     not be used by most consumers.
        /// </summary>
        /// <param name="dateString">A human-readable date.</param>
        /// <returns>The provided date, in <see cref="DateTime"/> format.</returns>
        /// <exception cref="ParseException">An exception arises during parsing.</exception>
        public DetailedParseResult DetailedParse(string dateString)
            => new Parser(dateString, _options).ParseDetailed();
    }

    /// <summary>
    ///     A detailed parse result.
    /// </summary>
    public class DetailedParseResult
    {
        /// <summary>
        ///     The result.
        /// </summary>
        public DateTimeOffset Result { get; }

        /// <summary>
        ///     The tokens used to construct this result.
        /// </summary>
        public List<IParseToken> Tokens { get; }

        internal DetailedParseResult(DateTimeOffset result, List<IParseToken> tokens)
        {
            Result = result;
            Tokens = tokens;
        }
    }
}