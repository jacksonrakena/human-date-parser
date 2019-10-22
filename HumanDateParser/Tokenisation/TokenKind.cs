namespace HumanDateParser
{
    internal enum TokenKind
    {
        YearSpecifier,
        MonthSpecifier,
        WeekSpecifier,
        DaySpecifier,
        LiteralMonth,
        MonthRelative,
        TimeRelative,
        LiteralDay,
        Today,
        Tomorrow,
        Yesterday,
        DateAbsolute,
        Number,
        Next,
        Last,
        To,
        At,
        Ago,
        Colon,
        BufferReadEnd,
        In
    }
}