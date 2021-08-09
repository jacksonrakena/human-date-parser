namespace HumanDateParser.Tokenisation.Tokens
{
    internal enum LiteralMonth
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    internal class LiteralMonthToken : IParseToken
    {
        public LiteralMonth Month { get; }

        internal LiteralMonthToken(LiteralMonth literalMonth)
        {
            Month = literalMonth;
        }

        internal static LiteralMonth? ParseMonth(string monthString)
        {
            switch (monthString.ToLower())
            {
                case "jan":
                case "january":
                    return LiteralMonth.January;
                case "febuary":
                case "february":
                case "feb":
                    return LiteralMonth.February;
                case "march":
                case "mar":
                    return LiteralMonth.March;
                case "apr":
                case "april":
                    return LiteralMonth.April;
                case "may":
                    return LiteralMonth.May;
                case "june":
                case "jun":
                    return LiteralMonth.June;
                case "july":
                case "jul":
                    return LiteralMonth.July;
                case "august":
                case "aug":
                    return LiteralMonth.August;
                case "september":
                case "sep":
                    return LiteralMonth.September;
                case "oct":
                case "october":
                    return LiteralMonth.October;
                case "nov":
                case "november":
                    return LiteralMonth.November;
                case "dec":
                case "december":
                    return LiteralMonth.December;
                default:
                    return null;
            }
        }
    }
}
