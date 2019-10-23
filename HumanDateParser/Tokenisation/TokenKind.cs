﻿namespace HumanDateParser
{
    internal enum TokenKind
    {
        YearSpecifier,
        MonthSpecifier,
        WeekSpecifier,
        DaySpecifier,
        HourSpecifier,
        MinuteSpecifier,
        SecondSpecifier,
        LiteralMonth,
        MonthRelative,
        Am,
        Pm,
        LiteralDay,
        Today,
        Tomorrow,
        Yesterday,
        DateAbsolute,
        Number,
        Next,
        Last,
        Dash,
        At,
        Ago,
        Colon,
        End,
        In
    }
}