using System;
using System.Collections.Generic;
using System.IO;

namespace HumanDateParser
{
    internal class Parser
    {
        private readonly Dictionary<string, int> _months = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _days = new Dictionary<string, int>();

        private readonly TokenBuffer _tokens;

        public Parser(Tokeniser tokeniser)
        {
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

            _tokens = new TokenBuffer(tokeniser);
        }

        public void ReadImpliedRelativeTimeSpan(ref DateTime baseTime, Token numberValueToken, Token specifierTypeToken)
        {
            var peekToken = _tokens.PeekNext();
            var number = int.Parse(numberValueToken.Text);
            if (peekToken != null && peekToken.Kind == TokenKind.Ago) number *= -1;
            switch (specifierTypeToken.Kind)
            {
                case TokenKind.DaySpecifier:
                    baseTime = baseTime.AddDays(number);
                    break;
                case TokenKind.MonthSpecifier:
                    baseTime = baseTime.AddMonths(number);
                    break;
                case TokenKind.WeekSpecifier:
                    baseTime = baseTime.AddDays(7 * number);
                    break;
                case TokenKind.YearSpecifier:
                    baseTime = baseTime.AddYears(number);
                    break;
                default:
                    throw new ParseException($"Invalid unit following '{numberValueToken.Text}'.");
            };
        }

        public DateTime Parse()
        {
            var date = DateTime.Now;

            var currentToken = _tokens.CurrentToken;
            while (currentToken.Kind != TokenKind.BufferReadEnd)
            {
                switch (currentToken.Kind)
                {
                    case TokenKind.Number:
                        if (!_tokens.MoveNext()) throw new ParseException($"Cannot have number without units following.");
                        Console.WriteLine($"Value token: " + currentToken.Kind.ToString() + $" Specifier token: " + _tokens.CurrentToken.Kind.ToString());
                        ReadImpliedRelativeTimeSpan(ref date, currentToken, _tokens.CurrentToken);
                        break;
                }
                _tokens.MoveNext();
                currentToken = _tokens.CurrentToken;
            }
            return date;
        }
        /*
        private DateTime EvaluateDateExpression()
        {
            var load = true;
            while (load)
            {
                var tokenCanMove = _tokens.MoveNext();
                if (!tokenCanMove)
                {
                    load = false;
                    break;
                }
                var token = _tokens.CurrentToken;
                switch (token.Kind)
                {
                    case TokenKind.Today:
                        return DateTime.Now.Date;
                    case TokenKind.Tomorrow:
                        return DateTime.Now.Date.AddDays(1);
                    case TokenKind.Yesterday:
                        return DateTime.Now.Date.AddDays(-1);
                    case TokenKind.DayAbsolute:
                        return DateTime.Now.Date.AddDays(ParseRelativeDay(token.Text));
                    case TokenKind.Number:
                        if (!_tokens.MoveNext()) throw new ParseException($"Unknown unit type for number '{token.Text}'");
                        var number = int.Parse(token.Text);
                        var unitType = _tokens.CurrentToken;
                        if (_tokens.PeekNext()!.Kind == TokenKind.Ago) number *= -1;
                        return unitType.Kind switch
                        {
                            TokenKind.DaySpecifier => DateTime.Now.AddDays(number),
                            TokenKind.MonthSpecifier => DateTime.Now.AddMonths(number),
                            TokenKind.WeekSpecifier => DateTime.Now.AddDays(7 * number),
                            TokenKind.YearSpecifier => DateTime.Now.AddYears(number),
                            // month relative somewhere here
                            _ => throw new ParseException($"Unknown unit type for number '{token.Text}'"),
                        };
                    case TokenKind.Next:
                        if (!_tokens.MoveNext()) throw new ParseException($"Cannot have 'next' without a following specifier.");
                        return _tokens.CurrentToken.Kind switch
                        {
                            TokenKind.WeekSpecifier => DateTime.Now.AddDays(7),
                            TokenKind.MonthSpecifier => DateTime.Now.AddMonths(1),
                            TokenKind.YearSpecifier => DateTime.Now.AddYears(1)
                        };
                    case TokenKind.Last:
                        if (!_tokens.MoveNext()) throw new ParseException($"Cannot have 'last' without a following specifier.");
                        return _tokens.CurrentToken.Kind switch
                        {
                            TokenKind.WeekSpecifier => DateTime.Now.AddDays(-7),
                            TokenKind.MonthSpecifier => DateTime.Now.AddMonths(-1),
                            TokenKind.YearSpecifier => DateTime.Now.AddYears(-1)
                        };
                }
            }

            //Get Or Time Year
            switch (Peek(1).Kind)
            {
                case TokenKind.Number:
                    Year();
                    Load();
                    if (Peek(1).Kind == TokenKind.At)
                        Time();
                    break;
                case TokenKind.At:
                    Time();
                    break;
            }

            Load();
        }

        private void Time()
        {
            Load();
            if (Peek(1).Kind == TokenKind.Number)
            {
                var hourPart = int.Parse(Peek(1).Text);
                var minPart = 0;
                Load();

                if (Peek(1).Kind == TokenKind.Colon)
                {
                    Load();
                    if(Peek(1).Kind == TokenKind.Number)
                    {
                        minPart = int.Parse(Peek(1).Text);
                        Load();
                    }
                    else
                    {
                        Errors.Add("Minute time part required after ':' keyword");
                        return; 
                    }
                }

                if (Peek(1).Kind == TokenKind.TimeRelative && Peek(1).Text == "PM" && hourPart <= 12) hourPart = hourPart + 12;


                _dateRange.CurrentDate = _dateRange.CurrentDate.SetTime(hourPart, minPart);
            }
            else
            {
                Errors.Add("Time required after 'At' keyword");
                return;
            }
        }

        private void Year()
        {
            _dateRange.CurrentDate = (new DateTime(int.Parse(Peek(1).Text), _dateRange.CurrentDate.Month, _dateRange.CurrentDate.Day));
        }

        private void MonthDateIdent(int num)
        {
            if (Peek(1).Kind == TokenKind.MonthAbsolute)
            {
                _dateRange.AddDate(new DateTime(DateTime.Now.Year, _months[Peek(1).Text], num));
                Load();
            }
            else
                _dateRange.AddDate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, num));
        }

        private static int ParseRelativeDay(string dayString)
        {
            try
            {
                var dayOfWeek = Enum.Parse<DayOfWeek>(dayString, true);

                for (var i = 0; i < 7; i++)
                {
                    if (Today.AddDays(i).DayOfWeek == dayOfWeek)
                    {
                        return i;
                    }
                }
                throw new ParseException($"Unable to parse day '{dayString}'.");
            } catch (ArgumentException)
            {
                throw new ParseException($"Unable to parse day '{dayString}'.");
            }
        }

        private void NumQuickDateIdent(int num)
        {
            if (Peek(2).Kind == TokenKind.Ago) num = num * -1;
            switch (Peek(1).Kind)
            {
                case TokenKind.DaySpecifier:
                    _dateRange.AddDate(Today.AddDays(num));
                    Load();
                    break;
                case TokenKind.WeekSpecifier:
                    _dateRange.AddDate(Today.AddDays(num * 7));
                    Load();
                    break;
                case TokenKind.MonthSpecifier:
                    _dateRange.AddDate(Today.AddMonths(num));
                    Load();
                    break;
                case TokenKind.YearSpecifier:
                    _dateRange.AddDate(Today.AddYears(num));
                    Load();
                    break;
            }
        }*/
    }
}