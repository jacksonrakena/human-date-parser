using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HumanDateParser.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestTimesWithMultipleColons()
        {
            var baset = new DateTimeOffset(2019, 10, 05, 16, 00, 00, DateTimeOffset.Now.Offset);
            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = baset
            });

            var actual = new DateTimeOffset(baset.Year, baset.Month, baset.Day, 02, 12, 24, DateTimeOffset.Now.Offset);
            var parsed = parser.Parse("at 2:12:24 AM");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestTimeWithOneColon()
        {
            var baset = new DateTimeOffset(2019, 10, 05, 16, 00, 00, DateTimeOffset.Now.Offset);
            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = baset
            });

            var actual = new DateTimeOffset(baset.Year, baset.Month, baset.Day, 02, 12, 00, DateTimeOffset.Now.Offset);
            var parsed = parser.Parse("at 2:12 AM");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(1), DataRow(2), DataRow(3)]
        public void TestImpliedRelativeFutureTimes(int months)
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddMonths(months);
            var parsed = parser.Parse($"{months} months");

            Assert.IsNotNull(parsed);
            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(1), DataRow(5), DataRow(20)]
        public void TestImpliedRelativeFutureTimes_Days(int days)
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddDays(days);
            var parsed = parser.Parse($"{days} days");

            Assert.IsNotNull(parsed);
            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(1), DataRow(2), DataRow(3)]
        public void TestImpliedRelativeFutureTimes_InKind(int months)
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddMonths(months);
            var parsed = parser.Parse($"in {months} months");

            Assert.IsNotNull(parsed);
            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(1), DataRow(2), DataRow(3)]
        public void TestImpliedRelativeFutureTimes_Negative(int monthsAgo)
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddMonths(monthsAgo * -1);
            var parsed = parser.Parse($"{monthsAgo} months ago");

            Assert.IsNotNull(parsed);
            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(5)]
        public void TestImpliedRelativeFutureTimes_NegativeMinutes(int minutesAgo)
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddMinutes(-1 * minutesAgo);
            var parsed = parser.Parse($"{minutesAgo}m ago");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestImpliedRelativeAgoTimes_Combined()
        {
            var parser = new HumanDateParser();
            var actual = DateTimeOffset.Now.AddHours(-2).AddMinutes(-5).AddSeconds(-10);
            var parsed = parser.Parse("2h5m10s ago");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        // may 16 = thursday
        // may 9 = thursday
        // may 8 = wednesday
        [DataRow(16, DayOfWeek.Thursday, -7)]
        [DataRow(16, DayOfWeek.Wednesday, -8)]
        public void TestImpliedLastTimes_Day(int currentDay, DayOfWeek lastDay, int lastDaysActual)
        {
            var now = new DateTimeOffset(2019, 05, currentDay, 0, 0, 0, DateTimeOffset.Now.Offset);
            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = now
            });

            var actual = now.AddDays(lastDaysActual);
            var parsed = parser.Parse($"last {lastDay.ToString()}");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(10, -16, "June")]
        [DataRow(01, -12, "January")]
        public void TestImpliedLastTimes_Month(int currentMonth, int lastMonthActual, string lastMonth)
        {
            var now = new DateTimeOffset(2019, currentMonth, 01, 0, 0, 0, DateTimeOffset.Now.Offset);
            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = now
            });

            var actual = now.AddMonths(lastMonthActual);
            var parsed = parser.Parse($"last {lastMonth}");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestImpliedLastTimes_MonthLiteral()
        {
            Assert.AreEqual(DateTimeOffset.Now.AddMonths(-1).Month, new HumanDateParser().Parse("Last month").Month);
        }

        [TestMethod]
        [DataRow(2019), DataRow(1607)]
        public void TestImpliedLastTimes_YearLiteral(int currentYear)
        {
            var now = new DateTimeOffset(currentYear, 01, 01, 0, 0, 0, DateTimeOffset.Now.Offset);
            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = now
            });

            var actual = now.AddYears(-1);
            var parsed = parser.Parse("last year");

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestImpliedLastTimes_WeekLiteral()
        {
            Assert.AreEqual(DateTimeOffset.Now.AddDays(-7).ToString(), new HumanDateParser().Parse("Last week").ToString());
        }

        [TestMethod]
        public void TestRelativeTime_PmAm()
        {
            var baseTime = new DateTimeOffset(2019, 10, 05, 06, 00, 00, DateTimeOffset.Now.Offset);

            var parser = new HumanDateParser(new ParserOptions
            {
                RelativeTo = baseTime
            });
            Assert.AreEqual(new DateTimeOffset(2019, 10, 05, 05, 00, 00, DateTimeOffset.Now.Offset).ToString(), parser.Parse("5 AM").ToString());
            Assert.AreEqual(new DateTimeOffset(2019, 10, 05, 16, 00, 00, DateTimeOffset.Now.Offset).ToString(), parser.Parse("4 PM").ToString());
            Assert.AreEqual(new DateTimeOffset(2019, 10, 05, 06, 00, 00, DateTimeOffset.Now.Offset).ToString(), parser.Parse("06 AM").ToString());
        }
    }
}