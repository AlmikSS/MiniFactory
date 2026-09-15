using UnityEngine;

namespace MiniFactory.Gameplay.Economy
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "Configs/EconomyConfig")]
    public sealed class EconomyConfig : ScriptableObject
    {
        [SerializeField] private int _startBalance;

        public int StartBalance => _startBalance;
    }
}