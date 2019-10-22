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
    }
}