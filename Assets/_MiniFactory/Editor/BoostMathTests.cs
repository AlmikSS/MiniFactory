using MiniFactory.Gameplay.Boost;
using NUnit.Framework;

namespace MiniFactory.Tests
{
    [TestFixture]
    public sealed class BoostMathTests
    {
        [Test]
        public void GetBoostedSecondsInRange_NoOverlap_ReturnsZero()
        {
            var result = BoostMath.GetBoostedSecondsInRange(100, 200, 300, 400);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetBoostedSecondsInRange_BoostInsideRange_ReturnsBoostDuration()
        {
            var result = BoostMath.GetBoostedSecondsInRange(150, 250, 100, 300);
            Assert.AreEqual(100, result);
        }

        [Test]
        public void GetBoostedSecondsInRange_RangeInsideBoost_ReturnsRangeDuration()
        {
            var result = BoostMath.GetBoostedSecondsInRange(50, 500, 100, 200);
            Assert.AreEqual(100, result);
        }

        [Test]
        public void GetBoostedSecondsInRange_PartialOverlapOnRight_ReturnsOverlap()
        {
            // boost [100, 200], range [150, 300] -> overlap [150, 200] = 50
            var result = BoostMath.GetBoostedSecondsInRange(100, 200, 150, 300);
            Assert.AreEqual(50, result);
        }

        [Test]
        public void GetBoostedSecondsInRange_PartialOverlapOnLeft_ReturnsOverlap()
        {
            // boost [150, 300], range [100, 200] -> overlap [150, 200] = 50
            var result = BoostMath.GetBoostedSecondsInRange(150, 300, 100, 200);
            Assert.AreEqual(50, result);
        }

        [Test]
        public void GetBoostedSecondsInRange_TouchingEdges_ReturnsZero()
        {
            var result = BoostMath.GetBoostedSecondsInRange(100, 200, 200, 300);
            Assert.AreEqual(0, result);
        }
    }
}