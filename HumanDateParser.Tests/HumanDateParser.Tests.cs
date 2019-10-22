using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HumanDateParser.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        [DataRow(1), DataRow(2), DataRow(3)]
        public void TestImpliedRelativeFutureTimes(int months)
        {
            var actual = DateTime.Now.AddMonths(months);
            var parsed = HumanDateParser.Parse($"{months} months");

            Assert.IsNotNull(parsed);
            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(1), DataRow(2), DataRow(3)]
        public void TestImpliedRelativeFutureTimes_Negative(int monthsAgo)
        {
            var actual = DateTime.Now.AddMonths(monthsAgo * -1);
            var parsed = HumanDateParser.Parse($"{monthsAgo} months ago");

            Assert.IsNotNull(parsed);
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
            var now = new DateTime(2019, 05, currentDay);

            var actual = now.AddDays(lastDaysActual);
            var parsed = HumanDateParser.Parse($"last {lastDay.ToString()}", now);

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        [DataRow(10, -16, "June")]
        [DataRow(01, -12, "January")]
        public void TestImpliedLastTimes_Month(int currentMonth, int lastMonthActual, string lastMonth)
        {
            var now = new DateTime(2019, currentMonth, 01);

            var actual = now.AddMonths(lastMonthActual);
            var parsed = HumanDateParser.Parse($"last {lastMonth}", now);

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestImpliedLastTimes_MonthLiteral()
        {
            Assert.AreEqual(DateTime.Now.AddMonths(-1).ToString(), HumanDateParser.Parse("Last month").ToString());
        }

        [TestMethod]
        [DataRow(2019), DataRow(1607)]
        public void TestImpliedLastTimes_YearLiteral(int currentYear)
        {
            var now = new DateTime(currentYear, 01, 01);

            var actual = now.AddYears(-1);
            var parsed = HumanDateParser.Parse($"last year", now);

            Assert.AreEqual(actual.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void TestImpliedLastTimes_WeekLiteral()
        {
            Assert.AreEqual(DateTime.Now.AddDays(-7).ToString(), HumanDateParser.Parse("Last week").ToString());
        }
    }
}