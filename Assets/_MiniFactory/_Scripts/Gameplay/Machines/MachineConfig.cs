using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    [CreateAssetMenu(fileName = "MachineConfig", menuName = "Machines/MachineConfig")]
    public sealed class MachineConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private int _startProductivity;
        [SerializeField] private int _baseUnlockCost;
        [SerializeField] private int _baseUpgradeCost;
        [SerializeField] private float _upgradeCostGrowth;
        [SerializeField] private float _productivityGrowth;
        [SerializeField] private float _addMoneyDelay;

        public string Id => _id;
        public string DisplayName => _displayName;
        public int StartProductivity => _startProductivity;
        public int BaseUnlockCost => _baseUnlockCost;
        public int BaseUpgradeCost => _baseUpgradeCost;
        public float UpgradeCostGrowth => _upgradeCostGrowth;
        public float ProductivityGrowth => _productivityGrowth;
        public float AddMoneyDelay => _addMoneyDelay;
    }
}