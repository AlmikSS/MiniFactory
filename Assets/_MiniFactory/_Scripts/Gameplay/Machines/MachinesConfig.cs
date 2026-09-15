using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    [CreateAssetMenu(fileName = "MachinesConfig", menuName = "Machines/MachinesConfig")]
    public sealed class MachinesConfig : ScriptableObject
    {
        [SerializeField] private float _startProductivity;
        [SerializeField] private float _startCostToUnlock;
        [SerializeField] private float _startNextLevelPrice;

        public float StartProductivity => _startProductivity;
        public float StartCostToUnlock => _startCostToUnlock;
        public float StartNextLevelPrice => _startNextLevelPrice;
    }
}