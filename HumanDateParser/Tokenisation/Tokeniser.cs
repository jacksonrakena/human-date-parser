using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HumanDateParser;

namespace HumanDateParser
{
    internal class Tokeniser
    {
        public CharacterBuffer _buffer;

        public Tokeniser(string text)
        {
            _buffer = new CharacterBuffer(text);
        }

        internal List<ParseToken> Tokenise()
        {
            var list = new List<ParseToken>();

            while (_buffer.MoveNext())
            {
                var current = _buffer.Current;

                switch (current)
                {
                    case ' ':
                        break;
                    case -1:
                        list.Add(new ParseToken(TokenKind.End, string.Empty));
                        return list;
                    case '-':
                        list.Add(new ParseToken(TokenKind.Dash, string.Empty));
                        break;
                    case ':':
                        list.Add(new ParseToken(TokenKind.Colon, string.Empty));
                        break;
                    default:
                        // words
                        if (char.IsLetter((char)current))
                        {
                            list.Add(TokeniseNextWord());
                            break;
                        }
                        // date formats like 21/10/2019 or 21-10-2019
                        else if (char.IsNumber((char)current))
                        {
                            var bufPeek2 = _buffer.Peek(2);
                            var bufPeek3 = _buffer.Peek(3);
                            if (bufPeek2 == '-' || bufPeek2 == '/' || bufPeek3 == '-' || bufPeek3 == '/')
                            {
                                list.Add(new ParseToken(TokenKind.AbsoluteDate, ReadString()));
                            } else
                            {
                                list.Add(ReadNumber());
                            }
                        }
                        break;
                }
            }

            return list;
        }

        private ParseToken TokeniseNextWord()
        {
            var identifier = ReadString().ToUpper();
            switch (identifier)
            {
                case "TODAY":
                    return new ParseToken(TokenKind.Today, string.Empty);
                case "TOMMOROW":
                    return new ParseToken(TokenKind.Tomorrow, string.Empty);
                case "YESTERDAY":
                    return new ParseToken(TokenKind.Yesterday, string.Empty);
                case "JAN":
                case "JANUARY":
                case "FEB":
                case "FEBUARY":
                case "MAR":
                case "MARCH":
                case "APR":
                case "APRIL":
                case "MAY":
                case "JUN":
                case "JUNE":
                case "JUL":
                case "JULY":
                case "AUG":
                case "AUGUST":
                case "SEPT":
                case "SEP":
                case "SEPTEMBER":
                case "OCT":
                case "OCTOBER":
                case "NOV":
                case "NOVEMBER":
                case "DEC":
                case "DECEMBER":
                    return new ParseToken(TokenKind.AbsoluteMonth, identifier);
                case "MONDAY":
                case "TUESDAY":
                case "WEDNESDAY":
                case "THURSDAY":
                case "FRIDAY":
                case "SATURDAY":
                case "SUNDAY":
                    return new ParseToken(TokenKind.AbsoluteDayOfWeek, identifier);
                case "YEAR":
                case "YEARS":
                case "Y":
                    return new ParseToken(TokenKind.Year, string.Empty);
                case "MONTH":
                case "MONTHS":
                case "MO":
                    return new ParseToken(TokenKind.Month, string.Empty);
                case "WEEK":
                case "WEEKS":
                case "W":
                    return new ParseToken(TokenKind.Week, string.Empty);
                case "DAY":
                case "DAYS":
                case "D":
                    return new ParseToken(TokenKind.Day, string.Empty);
                case "NEXT":
                    return new ParseToken(TokenKind.Next, string.Empty);
                case "LAST":
                    return new ParseToken(TokenKind.Last, string.Empty);
                case "AT":
                    return new ParseToken(TokenKind.At, string.Empty);
                case "TO":
                    return new ParseToken(TokenKind.Dash, string.Empty);
                case "AGO":
                    return new ParseToken(TokenKind.Ago, string.Empty);
                case "IN":
                    return new ParseToken(TokenKind.In, string.Empty);
                case "AM":
                    return new ParseToken(TokenKind.Am, string.Empty);
                case "PM":
                    return new ParseToken(TokenKind.Pm, string.Empty);
                case "END":
                    return new ParseToken(TokenKind.End, string.Empty);
                case "S":
                case "SECONDS":
                case "SECOND":
                case "SEC":
                    return new ParseToken(TokenKind.Second, string.Empty);
                case "M":
                case "MINUTES":
                case "MINUTE":
                case "MIN":
                    return new ParseToken(TokenKind.Minute, string.Empty);
                case "H":
                case "HOURS":
                case "HOUR":
                    return new ParseToken(TokenKind.Hour, string.Empty);
                case "A":
                    return new NumberToken(1);
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
            return new NumberToken(int.Parse(s.ToString()));
        }
    }
}