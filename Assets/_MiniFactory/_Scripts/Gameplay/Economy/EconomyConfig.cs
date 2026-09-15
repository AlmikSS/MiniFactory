using UnityEngine;

namespace MiniFactory.Gameplay.Economy
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "MiniFactory/EconomyConfig")]
    public sealed class EconomyConfig : ScriptableObject
    {
        [SerializeField] private int _startBalance;

        public int StartBalance => _startBalance;
    }
}