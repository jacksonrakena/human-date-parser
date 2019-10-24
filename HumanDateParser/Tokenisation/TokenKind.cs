namespace HumanDateParser
{
    /// <summary>
    ///     Represents a token type.
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        ///     A <see cref="TimeUnit"/>, like month, year, or day.
        /// </summary>
        Unit,

        /// <summary>
        ///     An absolute month, like January or February.
        /// </summary>
        AbsoluteMonth,

        /// <summary>
        ///     "AM".
        /// </summary>
        Am,

        /// <summary>
        ///     "PM".
        /// </summary>
        Pm,

        /// <summary>
        ///     An absolute day of the week, like Monday or Tuesday.
        /// </summary>
        AbsoluteDayOfWeek,

        /// <summary>
        ///     Today.
        /// </summary>
        Today,

        /// <summary>
        ///     Tomorrow.
        /// </summary>
        Tomorrow,

        /// <summary>
        ///     Yesterday.
        /// </summary>
        Yesterday,

        /// <summary>
        ///     An absolute date, like 21-10-2019 or 21/10/2019.
        /// </summary>
        AbsoluteDate,

        /// <summary>
        ///     A number.
        /// </summary>
        Number,

        /// <summary>
        ///     "Next".
        /// </summary>
        Next,

        /// <summary>
        ///     "Last".
        /// </summary>
        Last,

        /// <summary>
        ///     A dash (-).
        /// </summary>
        Dash,
        
        /// <summary>
        ///     "At".
        /// </summary>
        At,

        /// <summary>
        ///     "Ago".
        /// </summary>
        Ago,

        /// <summary>
        ///     A colon (:).
        /// </summary>
        Colon,

        /// <summary>
        ///     The end of the sequence.
        /// </summary>
        End,

        /// <summary>
        ///     "In".
        /// </summary>
        In
    }
}