namespace HumanDateParser
{
    /// <summary>
    ///     Represents a token type.
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        ///     "y", "year", or "years".
        /// </summary>
        Year,

        /// <summary>
        ///     "mo", "month", or "months".
        /// </summary>
        Month,

        /// <summary>
        ///     "w", "week", or "weeks".
        /// </summary>
        Week,

        /// <summary>
        ///     "d", "day", or "days".
        /// </summary>
        Day,

        /// <summary>
        ///     "h", "hour", or "hours".
        /// </summary>
        Hour,

        /// <summary>
        ///     "m", "min", "minute", or "minutes".
        /// </summary>
        Minute,

        /// <summary>
        ///     "s", "sec", "second", or "seconds".
        /// </summary>
        Second,

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