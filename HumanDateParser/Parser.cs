using System;
using System.Collections.Generic;

namespace HumanDateParser
{
    internal class Parser
    {
        private readonly DateTime _baseTime;
        private readonly Tokeniser _tokeniser;

        public Parser(string text, DateTime relativeTo)
        {
            _baseTime = relativeTo;
            _tokeniser = new Tokeniser(text);
        }

        public void ReadImpliedRelativeTimeSpan(ref DateTime baseTime, NumberToken num, TimeUnitToken units)
        {
            var number = num.Value;
            if (_tokeniser.ContainsToken<RelativeToken>(c => c.RelativeType == RelativeType.Ago)) number *= -1;
            baseTime = units.Unit switch
            {
                TimeUnit.Day => baseTime.AddDays(number),
                TimeUnit.Month => baseTime.AddMonths(number),
                TimeUnit.Week => baseTime.AddDays(7 * number),
                TimeUnit.Year => baseTime.AddYears(number),
                TimeUnit.Minute => baseTime.AddMinutes(number),
                TimeUnit.Second => baseTime.AddSeconds(number),
                TimeUnit.Hour => baseTime.AddHours(number),
                TimeUnit.Millisecond => baseTime.AddMilliseconds(number),
                _ => throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit following '{num.Value}'.")
            };
        }

        public void ReadRelativeDateUnit(bool isFuture, ref DateTime baseTime, IParseToken specifierOrDowUnitToken)
        {
            switch (specifierOrDowUnitToken)
            {
                case TimeUnitToken timeUnitToken when timeUnitToken.Unit == TimeUnit.Year:
                    baseTime = baseTime.AddYears(isFuture ? 1 : -1);
                    break;

                case TimeUnitToken timeUnitToken when timeUnitToken.Unit == TimeUnit.Month:
                    baseTime = baseTime.AddMonths(isFuture ? 1 : -1);
                    break;

                case TimeUnitToken timeUnitToken when timeUnitToken.Unit == TimeUnit.Week:
                    baseTime = baseTime.AddDays(isFuture ? 7 : -7);
                    break;

                case TimeUnitToken timeUnitToken when timeUnitToken.Unit == TimeUnit.Day:
                    baseTime = baseTime.AddDays(isFuture ? 1 : -1);
                    break;

                case LiteralMonthToken literalMonthToken:
                    var curMonth = baseTime.Month;
                    var newMonth = (int) literalMonthToken.Month;
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

                case LiteralDayOfWeekToken literalDayToken:
                    var day = literalDayToken.DayOfWeek;

                    var origBaseTimeDays = baseTime.Date.DayOfYear;
                    if (day == baseTime.DayOfWeek) baseTime = baseTime.AddDays(isFuture ? 7 : -7);
                    else
                    {
                        baseTime = baseTime.AddDays(isFuture ? 7 : -7);

                        if (day > baseTime.DayOfWeek)
                        {
                            baseTime = baseTime.AddDays(day - baseTime.DayOfWeek);
                        }
                        else
                        {
                            baseTime = baseTime.AddDays(-1 * (baseTime.DayOfWeek - day));
                            if (isFuture && (baseTime.Date.DayOfYear - origBaseTimeDays) < 7) baseTime = baseTime.AddDays(7);
                        }
                    }
                    break;
                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"{specifierOrDowUnitToken.GetType().Name} is not a valid unit type.");
            }
        }

        public void ReadRelativeDayTime(ref DateTime date, NumberToken firstNumberToken, TriviaToken specifierToken)
        {
            var hours = firstNumberToken.Value;
            var minutes = date.Minute;
            var seconds = date.Second;
            switch (specifierToken.TriviaType)
            {
                case TriviaType.Am:
                    if (hours == 12) hours = 0; // 12 AM == 0000 hours
                    break;
                case TriviaType.Pm:
                    if (hours != 12) hours += 12; // 12 PM = 1200 hours 
                    break;
                case TriviaType.Colon:
                    if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.NumberExpected, $"Expected a minute specifier to follow after a colon.");
                    if (!(_tokeniser.Current is NumberToken num)) throw new ParseException(ParseFailReason.InvalidUnit, $"Expected a number to follow the colon.");
                    minutes = num.Value;

                    if (_tokeniser.PeekNext() == null || !(_tokeniser.PeekNext() is TriviaToken t) || t.TriviaType != TriviaType.Colon)
                    {
                        _tokeniser.MoveNext();
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.NumberExpected, $"Expected a second specifier to follow the colon.");
                        if (!(_tokeniser.Current is NumberToken msNum)) throw new ParseException(ParseFailReason.InvalidUnit, "Expected a number to follow the colon.");
                        seconds = msNum.Value;
                    }

                    if (!_tokeniser.MoveNext() || !(_tokeniser.Current is TriviaToken t0) )
                        throw new ParseException(ParseFailReason.UnitExpected, "Expected an AM or PM specifier.");
                    switch (t0.TriviaType)
                    {
                        case TriviaType.Am:
                            if (hours == 12) hours = 0; // 12 AM == 0000 hours
                            break;
                        case TriviaType.Pm:
                            if (hours != 12) hours += 12; // 12 PM = 1200 hours 
                            break;
                        default:
                            throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit '{t0.TriviaType.ToString()}', expected AM or PM.");
                    }
                    break;
                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"Invalid unit '{specifierToken.TriviaType.ToString()}', expected AM or PM.");
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
                switch (currentToken)
                {
                    case TriviaToken trivia when trivia.TriviaType == TriviaType.In:
                        if (!_tokeniser.MoveNext() || !(_tokeniser.Current is NumberToken n)) throw new ParseException(ParseFailReason.NumberExpected, "Expected a number to come after 'in'.");
                        if (!_tokeniser.MoveNext() || !(_tokeniser.Current is TimeUnitToken tu)) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have number without units following.");
                        ReadImpliedRelativeTimeSpan(ref date, n, tu);
                        break;
                    case TriviaToken trivia when trivia.TriviaType == TriviaType.At:
                        if (!_tokeniser.MoveNext() || !(_tokeniser.Current is NumberToken atTriviaNumber)) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'at' without a time following.");
                        if (!_tokeniser.MoveNext() || !(_tokeniser.Current is TriviaToken triviaToken)) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'at {atTriviaNumber.Value}' without an AM, PM, or colon following.");
                        ReadRelativeDayTime(ref date, atTriviaNumber, triviaToken);
                        break;
                    case NumberToken numberToken:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have number without units following.");
                        switch (_tokeniser.Current)
                        {
                            case TimeUnitToken timeUnitToken:
                                ReadImpliedRelativeTimeSpan(ref date, numberToken, timeUnitToken);
                                break;
                            case TriviaToken trivia when trivia.TriviaType == TriviaType.Am || trivia.TriviaType == TriviaType.Pm || trivia.TriviaType == TriviaType.Colon:
                                ReadRelativeDayTime(ref date, numberToken, trivia);
                                break;
                            default:
                                throw new ParseException(ParseFailReason.InvalidUnit, $"Expected AM, PM, a colon, or a time unit. Received {_tokeniser.Current.GetType().Name}.");
                        }
                        break;
                    case RelativeToken relativeToken when relativeToken.RelativeType == RelativeType.Last:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'last' without day of week, or specifier unit following.");
                        ReadRelativeDateUnit(false, ref date, _tokeniser.Current);
                        break;
                    case TodayToken _:
                        break;
                    case TomorrowToken _:
                        date = date.AddDays(1);
                        break;
                    case YesterdayToken _:
                        date = date.AddDays(-1);
                        break;
                    case RelativeToken relativeToken when relativeToken.RelativeType == RelativeType.Next:
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'next' without day of week, or specifier unit following.");
                        ReadRelativeDateUnit(true, ref date, _tokeniser.Current);
                        break;

                }
            }
            return date;
        }
    }
}