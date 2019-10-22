namespace HumanDateParser
{
    internal enum TokenKind
    {
        YearSpecifier,
        MonthSpecifier,
        WeekSpecifier,
        DaySpecifier,
        MonthAbsolute,
        MonthRelative,
        TimeRelative,
        DayAbsolute,
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
        BufferReadEnd
    }
}