using MiniFactory.Gameplay.Offline;
using NUnit.Framework;

namespace MiniFactory.Tests
{
    [TestFixture]
    public sealed class OfflineMathTests
    {
        [Test]
        public void CalculateIncome_NoBoost_ReturnsLinearIncome()
        {
            var result = OfflineMath.CalculateIncome(10, 60, 0, 2f);
            Assert.AreEqual(600, result);
        }

        [Test]
        public void CalculateIncome_FullBoost_AppliesMultiplier()
        {
            var result = OfflineMath.CalculateIncome(10, 0, 60, 2f);
            Assert.AreEqual(1200, result);
        }

        [Test]
        public void CalculateIncome_MixedBoost_SplitsCorrectly()
        {
            var result = OfflineMath.CalculateIncome(10, 30, 30, 2f);
            Assert.AreEqual(900, result);
        }

        [Test]
        public void CalculateIncome_ZeroElapsed_ReturnsZero()
        {
            var result = OfflineMath.CalculateIncome(10, 0, 0, 2f);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void CalculateIncome_NegativeSeconds_ClampsToZero()
        {
            var result = OfflineMath.CalculateIncome(10, -5, -5, 2f);
            Assert.AreEqual(0, result);
        }
    }
}