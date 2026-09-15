using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    [CreateAssetMenu(fileName = "MachinesConfig", menuName = "Machines/MachinesConfig")]
    public sealed class MachinesConfig : ScriptableObject
    {
        [SerializeField] private int _startProductivity;
        [SerializeField] private int _startCostToUnlock;
        [SerializeField] private int _startNextLevelPrice;
        [SerializeField] private float _addMoneyDelay;

        public int StartProductivity => _startProductivity;
        public int StartCostToUnlock => _startCostToUnlock;
        public int StartNextLevelPrice => _startNextLevelPrice;
        public float AddMoneyDelay => _addMoneyDelay;
    }
}