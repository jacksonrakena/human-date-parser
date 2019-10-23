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

        internal List<Token> Tokenise()
        {
            var list = new List<Token>();

            while (_buffer.MoveNext())
            {
                var current = _buffer.Current;

                switch (current)
                {
                    case ' ':
                        break;
                    case -1:
                        list.Add(new Token(TokenKind.End, string.Empty));
                        return list;
                    case '-':
                        list.Add(new Token(TokenKind.Dash, string.Empty));
                        break;
                    case ':':
                        list.Add(new Token(TokenKind.Colon, string.Empty));
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
                                list.Add(new Token(TokenKind.DateAbsolute, ReadString()));
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

        private Token TokeniseNextWord()
        {
            var identifier = ReadString().ToUpper();
            switch (identifier)
            {
                case "TODAY":
                    return new Token(TokenKind.Today, string.Empty);
                case "TOMMOROW":
                    return new Token(TokenKind.Tomorrow, string.Empty);
                case "YESTERDAY":
                    return new Token(TokenKind.Yesterday, string.Empty);
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
                    return new Token(TokenKind.LiteralMonth, identifier);
                case "MONDAY":
                case "TUESDAY":
                case "WEDNESDAY":
                case "THURSDAY":
                case "FRIDAY":
                case "SATURDAY":
                case "SUNDAY":
                    return new Token(TokenKind.LiteralDay, identifier);
                case "YEAR":
                case "YEARS":
                    return new Token(TokenKind.YearSpecifier, string.Empty);
                case "MONTH":
                case "MONTHS":
                case "MO":
                    return new Token(TokenKind.MonthSpecifier, string.Empty);
                case "WEEK":
                case "WEEKS":
                case "W":
                    return new Token(TokenKind.WeekSpecifier, string.Empty);
                case "DAY":
                case "DAYS":
                case "D":
                    return new Token(TokenKind.DaySpecifier, string.Empty);
                case "NEXT":
                    return new Token(TokenKind.Next, string.Empty);
                case "LAST":
                    return new Token(TokenKind.Last, string.Empty);
                case "AT":
                    return new Token(TokenKind.At, string.Empty);
                case "TO":
                    return new Token(TokenKind.Dash, string.Empty);
                case "AGO":
                    return new Token(TokenKind.Ago, string.Empty);
                case "IN":
                    return new Token(TokenKind.In, string.Empty);
                case "TH":
                case "RD":
                case "ND":
                case "ST":
                    return new Token(TokenKind.MonthRelative, string.Empty);
                case "AM":
                    return new Token(TokenKind.Am, string.Empty);
                case "PM":
                    return new Token(TokenKind.Pm, string.Empty);
                case "END":
                    return new Token(TokenKind.End, string.Empty);
                case "S":
                case "SECONDS":
                case "SECOND":
                case "SEC":
                    return new Token(TokenKind.SecondSpecifier, string.Empty);
                case "M":
                case "MINUTES":
                case "MINUTE":
                case "MIN":
                    return new Token(TokenKind.MinuteSpecifier, string.Empty);
                case "H":
                case "HOURS":
                case "HOUR":
                    return new Token(TokenKind.HourSpecifier, string.Empty);
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