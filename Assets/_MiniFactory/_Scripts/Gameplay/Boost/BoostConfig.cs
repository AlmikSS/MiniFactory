using UnityEngine;

namespace MiniFactory.Gameplay.Boost
{
    [CreateAssetMenu(fileName = "BoostConfig", menuName = "Configs/BoostConfig")]
    public sealed class BoostConfig : ScriptableObject
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private float _durationSeconds;
        [SerializeField] private float _multiplier;
        [SerializeField] private int _price;

        public bool Enabled => _enabled;
        public float DurationSeconds => _durationSeconds;
        public float Multiplier => _multiplier;
        public int Price => _price;
    }
}