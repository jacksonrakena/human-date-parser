using System;

namespace HumanDateParser
{
    /// <summary>
    ///     A set of configurable options for a <see cref="HumanDateParser"/>.
    /// </summary>
    public class ParserOptions
    {
        /// <summary>
        ///     Describes how the parser should react when passed a year, for example,
        ///     if the only input is '2019'. The default behaviour is <see cref="PassedYearOptions.SetToJanuaryFirstOfYear"/>,
        ///     which will, in that case, set the current date to January 1st, 2019, at 12am.
        ///     The pre-3.0 behaviour was <see cref="PassedYearOptions.ThrowException"/>, which will
        ///     throw an exception.
        /// </summary>
        public PassedYearOptions PassedYearBehaviour = PassedYearOptions.SetToJanuaryFirstOfYear;

        /// <summary>
        ///     If set, the parser will throw a <see cref="ParseException"/> if the parsed date
        ///     is further back in time than this setting. Defaults to null.
        /// </summary>
        public DateTimeOffset? OldestTimeBound = null;

        /// <summary>
        ///     If set, the parser will throw a <see cref="ParseException"/> if the parsed date
        ///     is too far in the future than this setting. Defaults to null.
        /// </summary>
        public DateTimeOffset? NewestTimeBound = null;

        /// <summary>
        ///     If set to <c>false</c>, the parser will throw a <see cref="ParseException"/> if tokens
        ///     relative to the current date are passed. For example, if 'in 3 minutes' is passed and this is
        ///     <c>false</c>, the parser will throw. Defaults to <c>true</c>.
        /// </summary>
        public bool AllowRelativeTokens = true;
        
        /// <summary>
        ///     The date that the parser will work relatively to - for example, if 'in 3 minutes' is passed,
        ///     the calculated date will be this value + 3 minutes. Defaults to <c>null</c>, which means
        ///     any parse will be relative to the time at which the Parse method is called.
        /// </summary>
        public DateTimeOffset? RelativeTo = null;
    }

    /// <summary>
    ///     A set of options describing how to behave when a year is passed to the parser.
    /// </summary>
    public enum PassedYearOptions
    {
        /// <summary>
        ///     Sets the parsers' date to January 1st of the year that was passed.
        /// </summary>
        SetToJanuaryFirstOfYear,
        
        /// <summary>
        ///     Sets the parsers' date to the current date, but with the year switched to the passed date.
        /// </summary>
        SetToCurrentDateInYear,
        
        /// <summary>
        ///     Throws an UnitExpected exception. This was the original behaviour.
        /// </summary>
        ThrowException
    }
}