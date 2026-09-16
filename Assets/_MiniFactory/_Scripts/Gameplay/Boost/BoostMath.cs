using System;

namespace MiniFactory.Gameplay.Boost
{
    public static class BoostMath
    {
        public static long GetBoostedSecondsInRange(long boostStart, long boostEnd, long rangeStart, long rangeEnd)
        {
            if (boostEnd <= rangeStart || boostStart >= rangeEnd)
                return 0;

            var overlapStart = Math.Max(boostStart, rangeStart);
            var overlapEnd = Math.Min(boostEnd, rangeEnd);
            return Math.Max(0, overlapEnd - overlapStart);
        }
    }
}