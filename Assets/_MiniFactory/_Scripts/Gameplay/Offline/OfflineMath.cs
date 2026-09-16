using System;

namespace MiniFactory.Gameplay.Offline
{
    public static class OfflineMath
    {
        public static int CalculateIncome( double baseIncomePerSecond, long normalSeconds, long boostedSeconds, float boostMultiplier)
        {
            if (normalSeconds < 0) normalSeconds = 0;
            if (boostedSeconds < 0) boostedSeconds = 0;

            var income = baseIncomePerSecond * normalSeconds + baseIncomePerSecond * boostMultiplier * boostedSeconds;

            if (income <= 0)
                return 0;

            return (int)Math.Round(income);
        }
    }
}