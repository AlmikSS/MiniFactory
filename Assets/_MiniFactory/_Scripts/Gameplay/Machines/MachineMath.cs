using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public static class MachineMath
    {
        public static int CalculateProductivity(int startProductivity, float growth, int level)
        {
            var value = startProductivity * Mathf.Pow(growth, level - 1);
            return Mathf.Max(1, Mathf.RoundToInt(value));
        }

        public static int CalculateUpgradeCost(int baseCost, float growth, int level)
        {
            var value = baseCost * Mathf.Pow(growth, level - 1);
            return Mathf.Max(1, Mathf.CeilToInt(value));
        }
    }
}