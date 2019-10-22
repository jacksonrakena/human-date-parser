using System;
using System.Text;
using HumanDateParser;

namespace HumanDateParser
{
    internal class Tokeniser
    {
        public CharacterBuffer _buffer;

        public Tokeniser(CharacterBuffer charBuffer)
        {
            _buffer = charBuffer;
        }

        internal Token NextToken()
        {
            while (true)
            {
                switch (_buffer.Peek(1))
                {
                    case ' ':
                        _buffer.Next();
                        break;
                    case -1:
                        return new Token(TokenKind.BufferReadEnd, "end");
                    case '-':
                        _buffer.Next();
                        return new Token(TokenKind.To, "to");
                    case ':':
                        _buffer.Next();
                        return new Token(TokenKind.Colon, "<:>");
                    default:
                        // words like 'tomorrow'
                        if (char.IsLetter((char)_buffer.Peek(1))) return ParseNextIdentifier();
                        // number formats
                        else if (char.IsNumber((char)_buffer.Peek(1)))
                        {
                            // date format like 21/10/2019 or 21-10-2019
                            var bufPeek2 = _buffer.Peek(2);
                            var bufPeek3 = _buffer.Peek(3);
                            if (bufPeek2 == '-' || bufPeek2 == '/' || bufPeek3 == '-' || bufPeek3 == '/')
                                return new Token(TokenKind.DateAbsolute, ReadIdentifierUntilEnd());
           
                            return ReadNumber();
                        }
                        else
                        {
                            _buffer.Next();
                        }
                        break;
                }
            }
        }

        private Token ParseNextIdentifier()
        {
            var identifier = ReadIdentifierUntilEnd();
            switch (identifier.ToUpper())
            {
                case "TODAY":
                    return new Token(TokenKind.Today, "<today>");
                case "TOMMOROW":
                    return new Token(TokenKind.Tomorrow, "<tommorow>");
                case "YESTERDAY":
                    return new Token(TokenKind.Yesterday, "<yesterday>");
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
                    return new Token(TokenKind.LiteralMonth, identifier.ToUpper());
                case "MONDAY":
                case "TUESDAY":
                case "WEDNESDAY":
                case "THURSDAY":
                case "FRIDAY":
                case "SATURDAY":
                case "SUNDAY":
                    return new Token(TokenKind.LiteralDay, identifier.ToUpper());
                case "YEAR":
                case "YEARS":
                    return new Token(TokenKind.YearSpecifier, "<year>");
                case "MONTH":
                case "MONTHS":
                    return new Token(TokenKind.MonthSpecifier, "<month>");
                case "WEEK":
                case "WEEKS":
                    return new Token(TokenKind.WeekSpecifier, "<week>");
                case "DAY":
                case "DAYS":
                    return new Token(TokenKind.DaySpecifier, "<day>");
                case "NEXT":
                    return new Token(TokenKind.Next, "<next>");
                case "LAST":
                    return new Token(TokenKind.Last, "<previous>");
                case "AT":
                    return new Token(TokenKind.At, "<at>");
                case "TO":
                    return new Token(TokenKind.To, "<to>");
                case "AGO":
                    return new Token(TokenKind.Ago, "<ago>");
                case "IN":
                    return new Token(TokenKind.In, "<in>");
                case "TH":
                case "RD":
                case "ND":
                case "ST":
                    return new Token(TokenKind.MonthRelative, "<month_modifier>");
                case "AM":
                case "PM":
                    return new Token(TokenKind.TimeRelative, identifier.ToUpper());
                case "END":
                    return new Token(TokenKind.BufferReadEnd, "<eof>");
                default:
                    throw new ParseException(ParseFailReason.InvalidUnit, $"Unknown token '{identifier}'.");
            }
        }

        private string ReadIdentifierUntilEnd()
        {
            var s = new StringBuilder();
            while (char.IsLetter((char)_buffer.Peek(1)) || char.IsNumber((char)_buffer.Peek(1)) || (char)_buffer.Peek(1) == '_' || (char)_buffer.Peek(1) == '/' || (char)_buffer.Peek(1) == '-' || (char)_buffer.Peek(1) == '.')
            {
                s.Append((char)_buffer.Peek(1));
                _buffer.Next();
            }
            return s.ToString();
        }

        private Token ReadNumber()
        {
            var s = new StringBuilder();
            var c = (char)_buffer.Peek(1);
            while (char.IsNumber(c) )
            {
                s.Append(c);
                _buffer.Next();
                c = (char)_buffer.Peek(1);
            }
            var stemp = s.ToString();
            return new Token(TokenKind.Number, stemp);
        }
    }
}