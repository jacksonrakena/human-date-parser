using System;
using System.Collections.Generic;

namespace HumanDateParser
{
    internal class Parser
    {
        private readonly Dictionary<string, int> _months = new Dictionary<string, int>();

        private readonly DateTime _baseTime;
        private readonly Tokeniser _tokeniser;

        public Parser(string text, DateTime relativeTo)
        {
            _baseTime = relativeTo;
            _months.Add("JAN", 1);
            _months.Add("FEB", 2);
            _months.Add("MAR", 3);
            _months.Add("APR", 4);
            _months.Add("MAY", 5);
            _months.Add("JUN", 6);
            _months.Add("JUL", 7);
            _months.Add("AUG", 8);
            _months.Add("SEPT", 9);
            _months.Add("OCT", 10);
            _months.Add("NOV", 11);
            _months.Add("DEC", 12);

            _months.Add("JANUARY", 1);
            _months.Add("FEBUARY", 2);
            _months.Add("MARCH", 3);
            _months.Add("APRIL", 4);
            _months.Add("JUNE", 6);
            _months.Add("JULY", 7);
            _months.Add("AUGUST", 8);
            _months.Add("SEPTEMBER", 9);
            _months.Add("OCTOBER", 10);
            _months.Add("NOVEMBER", 11);
            _months.Add("DECEMBER", 12);

            _tokeniser = new Tokeniser(text);
        }

        public void ReadImpliedRelativeTimeSpan(ref DateTime baseTime, ParseToken numberValueToken, ParseToken specifierTypeToken)
        {
            var number = int.Parse(numberValueToken.Text);
            if (_tokeniser.ContainsKind(TokenKind.Ago)) number *= -1;
            baseTime = specifierTypeToken.Kind switch
            {
                TokenKind.Day => baseTime.AddDays(number),
                TokenKind.Month => baseTime.AddMonths(number),
                TokenKind.Week => baseTime.AddDays(7 * number),
                TokenKind.Year => baseTime.AddYears(number),
                TokenKind.Minute => baseTime.AddMinutes(number),
                TokenKind.Second => baseTime.AddSeconds(number),
                TokenKind.Hour => baseTime.AddHours(number),
                _ => throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit following '{numberValueToken.Text}'.")
            };
        }

        public void ReadRelativeDateUnit(bool isFuture, ref DateTime baseTime, ParseToken specifierOrDowUnitToken)
        {
            switch (specifierOrDowUnitToken.Kind)
            {
                case TokenKind.Year:
                    baseTime = baseTime.AddYears(isFuture ? 1 : -1);
                    break;
                case TokenKind.Month:
                    baseTime = baseTime.AddMonths(isFuture ? 1 : -1);
                    break;
                case TokenKind.Week:
                    baseTime = baseTime.AddDays(isFuture ? 7 : -7);
                    break;
                case TokenKind.Day:
                    // this is literally 'last day/next day'
                    baseTime = baseTime.AddDays(isFuture ? 1 : -1);
                    break;
                case TokenKind.AbsoluteMonth:
                    var curMonth = baseTime.Month;
                    var newMonth = _months[specifierOrDowUnitToken.Text.ToUpper()];
                    if (isFuture)
                    {
                        if (curMonth == newMonth) baseTime = baseTime.AddYears(1);
                        else if (curMonth < newMonth) baseTime = baseTime.AddMonths(newMonth - curMonth);
                        else baseTime = baseTime.AddMonths(12 + newMonth - curMonth);
                    }
                    else
                    {
                        if (curMonth == newMonth) baseTime = baseTime.AddYears(-1);
                        else if (curMonth < newMonth) baseTime = baseTime.AddMonths(-12 + (newMonth - curMonth));
                        else baseTime = baseTime.AddMonths(-12 + newMonth - curMonth);
                    }
                    break;
                case TokenKind.AbsoluteDayOfWeek:
                    if (!Enum.TryParse<DayOfWeek>(specifierOrDowUnitToken.Text, true, out var day))
                        throw new ParseException(ParseFailReason.InvalidDayOfWeek, $"{specifierOrDowUnitToken.Text} is not a valid day of the week.");

                    var origBaseTimeDays = baseTime.Date.DayOfYear;
                    if (day == baseTime.DayOfWeek) baseTime = baseTime.AddDays(isFuture ? 7 : -7);
                    else
                    {
                        baseTime = baseTime.AddDays(isFuture ? 7 : -7);

                        if (day > baseTime.DayOfWeek)
                        {
                            baseTime = baseTime.AddDays(day - baseTime.DayOfWeek);
                        } else
                        {
                            baseTime = baseTime.AddDays(-1 * (baseTime.DayOfWeek - day));
                            if (isFuture && (baseTime.Date.DayOfYear - origBaseTimeDays) < 7) baseTime = baseTime.AddDays(7);
                        }
                    }
                    break;

                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"'Last {specifierOrDowUnitToken.Text}' is not a valid relative date.");
            }
        }

        public void ReadRelativeDayTime(ref DateTime date, ParseToken valueToken, ParseToken specifierToken)
        {
            var hours = int.Parse(valueToken.Text);
            var minutes = date.Minute;
            var seconds = date.Second;
            switch (specifierToken.Kind)
            {
                case TokenKind.Am:
                    if (hours == 12) hours = 0; // 12 AM == 0000 hours
                    break;
                case TokenKind.Pm:
                    if (hours != 12) hours += 12; // 12 PM = 1200 hours 
                    break;
                case TokenKind.Colon:
                    if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.NumberExpected, $"Expected a minute specifier to follow after a colon.");
                    if (!(_tokeniser.Current is NumberToken num)) throw new ParseException(ParseFailReason.InvalidUnit, $"Expected a number to follow the colon.");
                    minutes = num.Value;

                    if (_tokeniser.PeekNext() != null && _tokeniser.PeekNext().Kind == TokenKind.Colon)
                    {
                        _tokeniser.MoveNext();
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.NumberExpected, $"Expected a second specifier to follow the colon.");
                        if (!(_tokeniser.Current is NumberToken msNum)) throw new ParseException(ParseFailReason.InvalidUnit, "Expected a number to follow the colon.");
                        seconds = msNum.Value;
                    }

                    if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, "Expected an AM/PM.");
                    switch (_tokeniser.Current.Kind)
                    {
                        case TokenKind.Am:
                            if (hours == 12) hours = 0; // 12 AM == 0000 hours
                            break;
                        case TokenKind.Pm:
                            if (hours != 12) hours += 12; // 12 PM = 1200 hours 
                            break;
                        default:
                            throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit {_tokeniser.Current.Text}, expected AM or PM.");
                    }
                    break;
                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit {specifierToken.Text}, expected AM or PM.");
            }
            date = new DateTime(date.Year, date.Month, date.Day, hours, minutes, seconds);
        }

        public DetailedParseResult ParseDetailed()
        {
            var result = Parse();
            return new DetailedParseResult(result, _tokeniser.All());
        }

        public DateTime Parse()
        {
            var date = _baseTime;

            while (_tokeniser.MoveNext())
            {
                var currentToken = _tokeniser.Current;
                switch (currentToken.Kind)
                {
                    case TokenKind.In:
                        if (!_tokeniser.MoveNext() || _tokeniser.Current.Kind != TokenKind.Number) throw new ParseException(ParseFailReason.NumberExpected, "Expected a number to come after 'in'.");
                        var numToken = _tokeniser.Current;
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have number without units following.");
                        ReadImpliedRelativeTimeSpan(ref date, numToken, _tokeniser.Current);
                        break;
                    case TokenKind.Number:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have number without units following.");
                        switch (_tokeniser.Current.Kind)
                        {
                            case TokenKind.Day:
                            case TokenKind.Hour:
                            case TokenKind.Minute:
                            case TokenKind.Second:
                            case TokenKind.Week:
                            case TokenKind.Year:
                            case TokenKind.Month:
                                ReadImpliedRelativeTimeSpan(ref date, currentToken, _tokeniser.Current);
                                break;
                            case TokenKind.Am:
                            case TokenKind.Pm:
                            case TokenKind.Colon:
                                ReadRelativeDayTime(ref date, currentToken, _tokeniser.Current);
                                break;

                        }
                        break;
                    case TokenKind.Last:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'last' without day of week, or specifier unit following.");
                        ReadRelativeDateUnit(false, ref date, _tokeniser.Current);
                        break;
                    case TokenKind.Today:
                        break;
                    case TokenKind.Tomorrow:
                        date = date.AddDays(1);
                        break;
                    case TokenKind.Yesterday:
                        date = date.AddDays(-1);
                        break;
                    case TokenKind.Next:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'next' without day of week, or specifier unit following.");
                        ReadRelativeDateUnit(true, ref date, _tokeniser.Current);
                        break;
                    case TokenKind.At:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'at' without a time following.");
                        var numericalValue = _tokeniser.Current;
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'at {numericalValue.Text}' without a time unit following.");
                        ReadRelativeDayTime(ref date, numericalValue, _tokeniser.Current);
                        break;
                }
            }
            return date;
        }
    }
}