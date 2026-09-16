using UnityEngine;

namespace MiniFactory.Gameplay.Economy
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "Configs/EconomyConfig")]
    public sealed class EconomyConfig : ScriptableObject
    {
        [SerializeField] private int _startBalance;
        [SerializeField] private int _maxOfflineSeconds = 8 * 60 * 60;

        public int StartBalance => _startBalance;
        public int MaxOfflineSeconds => _maxOfflineSeconds;
    }
}