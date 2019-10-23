using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanDateParser
{
    /// <summary>
    ///     A static class that can parse human-readable dates.
    /// </summary>
    public static class HumanDateParser
    {
        /// <summary>
        ///     Parses a human-readable date into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateString">A human-readable date.</param>
        /// <param name="relativeTo">The time to parse relative to, for relative dates. Defaults to <see cref="DateTime.Now"/>.</param>
        /// <returns>The provided date, in <see cref="DateTime"/> format.</returns>
        /// <exception cref="ParseException">An exception arises during parsing.</exception>
        public static DateTime Parse(string dateString, DateTime? relativeTo = null)
            => new Parser(dateString, relativeTo ?? DateTime.Now).Parse();

        /// <summary>
        ///     Parses a human-readable date into a <see cref="DateTime"/>, as well as including
        ///     tools, debug information, and temporary data stored during parsing. This should
        ///     not be used by most consumers.
        /// </summary>
        /// <param name="dateString">A human-readable date.</param>
        /// <param name="relativeTo">The time to parse relative to, for relative dates. Defaults to <see cref="DateTime.Now"/>.</param>
        /// <returns>The provided date, in <see cref="DateTime"/> format.</returns>
        /// <exception cref="ParseException">An exception arises during parsing.</exception>
        public static DetailedParseResult ParseDetailed(string dateString, DateTime? relativeTo = null)
            => new Parser(dateString, relativeTo ?? DateTime.Now).ParseDetailed();
    }

    /// <summary>
    ///     A detailed parse result.
    /// </summary>
    public class DetailedParseResult
    {
        /// <summary>
        ///     The result.
        /// </summary>
        public DateTime Result { get; }

        /// <summary>
        ///     The tokens used to construct this result.
        /// </summary>
        public List<ParseToken> Tokens { get; }

        internal DetailedParseResult(DateTime result, List<ParseToken> tokens)
        {
            Result = result;
            Tokens = tokens;
        }
    }
}