using System;
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
            => new Parser(new Tokeniser(new CharacterBuffer(dateString, 3)), relativeTo ?? DateTime.Now).Parse();
    }

}