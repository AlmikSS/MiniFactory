using MiniFactory.Gameplay.Machines;
using NUnit.Framework;

namespace MiniFactory.Tests
{
    [TestFixture]
    public sealed class MachineMathTests
    {
        [Test]
        public void CalculateProductivity_Level1_ReturnsStartValue()
        {
            var result = MachineMath.CalculateProductivity(10, 1.2f, 1);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void CalculateProductivity_Level5_AppliesGrowth()
        {
            var result = MachineMath.CalculateProductivity(10, 1.2f, 5);
            Assert.AreEqual(21, result);
        }

        [Test]
        public void CalculateProductivity_ExtremeLowGrowth_ClampsTo1()
        {
            var result = MachineMath.CalculateProductivity(1, 0.5f, 10);
            Assert.GreaterOrEqual(result, 1);
        }

        [Test]
        public void CalculateUpgradeCost_Level1_ReturnsBaseCost()
        {
            var result = MachineMath.CalculateUpgradeCost(50, 1.5f, 1);
            Assert.AreEqual(50, result);
        }

        [Test]
        public void CalculateUpgradeCost_Level3_AppliesGrowth()
        {
            var result = MachineMath.CalculateUpgradeCost(50, 1.5f, 3);
            Assert.AreEqual(113, result);
        }

        [Test]
        public void CalculateUpgradeCost_GrowthAlwaysIncreases()
        {
            var cost1 = MachineMath.CalculateUpgradeCost(50, 1.5f, 1);
            var cost2 = MachineMath.CalculateUpgradeCost(50, 1.5f, 2);
            var cost3 = MachineMath.CalculateUpgradeCost(50, 1.5f, 3);

            Assert.Less(cost1, cost2);
            Assert.Less(cost2, cost3);
        }
    }
}