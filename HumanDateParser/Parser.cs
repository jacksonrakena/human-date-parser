using System;
using System.Collections.Generic;
using HumanDateParser.Buffers;
using HumanDateParser.Tokenisation;
using HumanDateParser.Tokenisation.Tokens;

namespace HumanDateParser
{
    internal class Parser
    {
        private readonly DateTimeOffset _baseTime;
        private readonly ParserOptions _options;
        private readonly Tokeniser _tokeniser;

        public Parser(string text, ParserOptions options)
        {
            _options = options;
            _baseTime = options.RelativeTo ?? DateTimeOffset.Now;
            _tokeniser = new Tokeniser(new CharacterBufferStream(text));
        }

        public void ReadImpliedRelativeTimeSpan(ref DateTimeOffset baseTime, NumberToken num, TimeUnitToken units)
        {
            var number = num.Value;
            if (_tokeniser.ContainsToken<RelativeToken>() && !_options.AllowRelativeTokens) 
                throw new ParseException(ParseFailReason.RelativeTokensNotAllowed,
                    "Relative tokens are not allowed.");
            if (_tokeniser.ContainsToken<RelativeToken>(c => c.RelativeType == RelativeType.Ago)) number *= -1;
            try
            {
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
            } catch (ArgumentOutOfRangeException)
            {
                throw new ParseException(ParseFailReason.InvalidUnit, "Cannot add that much time to the current date.");
            }
        }

        public void ReadRelativeDateUnit(bool isFuture, ref DateTimeOffset baseTime, IParseToken specifierOrDowUnitToken)
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

        public void ReadRelativeDayTime(ref DateTimeOffset date, NumberToken firstNumberToken, TriviaToken specifierToken)
        {
            var hours = firstNumberToken.Value;
            var minutes = 0;
            var seconds = 0;
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

                    if (_tokeniser.PeekNext() != null && _tokeniser.PeekNext() is TriviaToken t && t.TriviaType == TriviaType.Colon)
                    {
                        _tokeniser.MoveNext();
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.NumberExpected, $"Expected a second specifier to follow the colon.");
                        if (!(_tokeniser.Current is NumberToken msNum)) throw new ParseException(ParseFailReason.InvalidUnit, "Expected a number to follow the colon.");
                        seconds = msNum.Value;
                    }

                    if (!_tokeniser.MoveNext() || !(_tokeniser.Current is TriviaToken t0))
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
            if (hours >= 24 || minutes >= 60 || seconds >= 60) throw new ParseException(ParseFailReason.InvalidUnit, $"Not a valid time."); 
            date = new DateTimeOffset(date.Year, date.Month, date.Day, hours, minutes, seconds, date.Offset);
        }

        public DetailedParseResult ParseDetailed()
        {
            var result = Parse();
            return new DetailedParseResult(result, _tokeniser.All());
        }

        public DateTimeOffset Parse()
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
                        // Could possibly be a year..?
                        if (!_tokeniser.MoveNext())
                        {
                            date = _options.PassedYearBehaviour switch
                            {
                                PassedYearOptions.SetToJanuaryFirstOfYear => new DateTimeOffset(numberToken.Value, 1, 1,
                                    0, 0, 0, date.Offset),
                                PassedYearOptions.SetToCurrentDateInYear => new DateTimeOffset(numberToken.Value,
                                    date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Offset),
                                PassedYearOptions.ThrowException => throw new ParseException(ParseFailReason.UnitExpected, "Expected a unit after a number."),
                                _ => throw new ParseException(ParseFailReason.Internal,
                                    "Switch cases for passed year are out-of-date.")
                            };
                            break;
                        }
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
                        if (!_options.AllowRelativeTokens)
                            throw new ParseException(ParseFailReason.RelativeTokensNotAllowed,
                                "Relative tokens are not allowed.");
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
                        if (!_options.AllowRelativeTokens)
                            throw new ParseException(ParseFailReason.RelativeTokensNotAllowed,
                                "Relative tokens are not allowed.");
                        if (!_tokeniser.MoveNext()) throw new ParseException(ParseFailReason.UnitExpected, $"Cannot have 'next' without day of week, or specifier unit following.");
                        ReadRelativeDateUnit(true, ref date, _tokeniser.Current);
                        break;
                }
            }

            if (_options.NewestTimeBound != null && date > _options.NewestTimeBound)
            {
                throw new ParseException(ParseFailReason.TooFar, "Parsed date is too far in the future.");
            }

            if (_options.OldestTimeBound != null && date > _options.OldestTimeBound)
            {
                throw new ParseException(ParseFailReason.TooOld, "Parsed date is too far in the past.");
            }
            
            return date;
        }
    }
}