using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HumanDateParser.Buffers;
using HumanDateParser.Tokenisation.Tokens;

namespace HumanDateParser.Tokenisation
{
    internal class Tokeniser : BufferStream<IParseToken>, IDisposable
    {
        public CharacterBufferStream _buffer;

        public bool ContainsToken<TTokenType>() where TTokenType : IParseToken => _list.Any(c => c is TTokenType);
        public bool ContainsToken<TTokenType>(Predicate<TTokenType> predicate) where TTokenType : IParseToken => _list.Any(c => c is TTokenType t && predicate(t));

        public Tokeniser(CharacterBufferStream text)
        {
            _buffer = text;

            while (_buffer.MoveNext())
            {
                var current = _buffer.Current;

                switch (current)
                {
                    case ' ':
                        break;
                    case -1:
                        _list.Add(new EndToken());
                        return;
                    case '-':
                        _list.Add(new TriviaToken(TriviaType.Dash));
                        break;
                    case ':':
                        _list.Add(new TriviaToken(TriviaType.Colon));
                        break;
                    default:
                        // words
                        if (char.IsLetter((char)current))
                        {
                            _list.Add(TokeniseNextWord());
                            break;
                        }
                        // date formats like 21/10/2019 or 21-10-2019
                        else if (char.IsNumber((char)current))
                        {
                            var bufPeek2 = _buffer.Peek(2);
                            var bufPeek3 = _buffer.Peek(3);
                            if (bufPeek2 == '-' || bufPeek2 == '/' || bufPeek3 == '-' || bufPeek3 == '/')
                            {
                                var str = ReadString();
                                if (!DateTime.TryParse(str, out var dt)) throw new ParseException(ParseFailReason.InvalidUnit, $"{str} is not a valid date.");
                                _list.Add(new DateToken(dt));
                            } else
                            {
                                _list.Add(ReadNumber());
                            }
                        }
                        break;
                }
            }

            return;
        }

        private IParseToken TokeniseNextWord()
        {
            var identifier = ReadString().ToUpper();

            var dayOfWeek = LiteralDayOfWeekToken.ParseDayOfWeek(identifier);
            if (dayOfWeek != null) return new LiteralDayOfWeekToken(dayOfWeek.Value);

            var literalMonth = LiteralMonthToken.ParseMonth(identifier);
            if (literalMonth != null) return new LiteralMonthToken(literalMonth.Value);

            switch (identifier)
            {
                case "TODAY":
                    return new TodayToken();
                case "NOW":
                    return new TodayToken();
                case "TOMORROW":
                    return new TomorrowToken();
                case "YESTERDAY":
                    return new YesterdayToken();
                case "YEAR":
                case "YEARS":
                case "Y":
                    return new TimeUnitToken(TimeUnit.Year);
                case "MONTH":
                case "MONTHS":
                case "MO":
                    return new TimeUnitToken(TimeUnit.Month);
                case "WEEK":
                case "WEEKS":
                case "W":
                    return new TimeUnitToken(TimeUnit.Week);
                case "DAY":
                case "DAYS":
                case "D":
                    return new TimeUnitToken(TimeUnit.Day);
                case "S":
                case "SECONDS":
                case "SECOND":
                case "SEC":
                    return new TimeUnitToken(TimeUnit.Second);
                case "M":
                case "MINUTES":
                case "MINUTE":
                case "MIN":
                    return new TimeUnitToken(TimeUnit.Minute);
                case "H":
                case "HOURS":
                case "HOUR":
                    return new TimeUnitToken(TimeUnit.Hour);
                case "MS":
                case "MSEC":
                case "MILLIS":
                case "MILLISEC":
                case "MILLISECONDS":
                    return new TimeUnitToken(TimeUnit.Millisecond);
                case "NEXT":
                    return new RelativeToken(RelativeType.Next);
                case "LAST":
                    return new RelativeToken(RelativeType.Last);
                case "AGO":
                    return new RelativeToken(RelativeType.Ago);
                case "AT":
                    return new TriviaToken(TriviaType.At);
                case "TO":
                    return new TriviaToken(TriviaType.To);
                case "IN":
                    return new TriviaToken(TriviaType.In);
                case "AM":
                    return new TriviaToken(TriviaType.Am);
                case "PM":
                    return new TriviaToken(TriviaType.Pm);
                case "END":
                    return new EndToken();
                case "A":
                    return new NumberToken(1);
                case "FEW":
                    return new NumberToken(TokeniserConstants.FEW);
                case "SOME":
                    return new NumberToken(TokeniserConstants.SOME);
                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"Unknown token '{identifier}'.");
            }
        }

        private string ReadString()
        {
            var isLetter = char.IsLetter((char)_buffer.Current);
            var s = new StringBuilder().Append((char) _buffer.Current);
            while (_buffer.MoveNext())
            {
                if (IsValidProceduralRead((char)_buffer.Current, isLetter))
                    s.Append((char)_buffer.Current);
                else
                {
                    _buffer.MoveBack();
                    break;
                }
            }
            return s.ToString();
        }

        private static bool IsValidProceduralRead(char current, bool expectedLetter)
        {
            bool c;
            if (expectedLetter) c = char.IsLetter(current);
            else c = char.IsNumber(current);
            return c || current == '_' || current == '/' || current == '-' || current == '.';
        }

        private NumberToken ReadNumber()
        {
            var s = new StringBuilder().Append((char)_buffer.Current);
            while (_buffer.MoveNext())
            {
                if (char.IsNumber((char)_buffer.Current))
                    s.Append((char)_buffer.Current);
                else
                {
                    _buffer.MoveBack();
                    break;
                }
            }
            if (!int.TryParse(s.ToString(), out var i)) throw new ParseException(ParseFailReason.InvalidUnit, $"The provided number was not a valid integer.");
            return new NumberToken(i);
        }
    }
}