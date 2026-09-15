using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Economy;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class Machine : MonoBehaviour, IGameplayTickable, IConstructable
    {
        [SerializeField] private MachineConfig _config;
        [SerializeField] private bool _startLocked = true;

        private WalletService _walletService;
        private bool _locked;
        private int _level;
        private int _productivity;
        private int _nextLevelPrice;
        private float _addMoneyTimer;

        public string Id => _config.Id;
        public string DisplayName => _config.DisplayName;
        public bool IsLocked => _locked;
        public int Level => _level;
        public int Productivity => _productivity;
        public int NextLevelPrice => _nextLevelPrice;
        public int UnlockCost => _config.BaseUnlockCost;
        public float AddMoneyDelay => _config.AddMoneyDelay;
        
        [Inject]
        private void Construct(WalletService walletService)
        {
            _walletService = walletService;
        }
        
        public void OnConstruct()
        {
            if (_config == null)
            {
                Log.Error($"{nameof(Machine)} on '{name}' has no {nameof(MachineConfig)} assigned.");
                enabled = false;
                return;
            }

            _locked = _startLocked;
            _level = 1;
            _productivity = _config.StartProductivity;
            _nextLevelPrice = _config.BaseUpgradeCost;
            _addMoneyTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (_locked || _walletService == null)
                return;

            _addMoneyTimer += deltaTime;
            if (_addMoneyTimer < _config.AddMoneyDelay)
                return;

            _addMoneyTimer -= _config.AddMoneyDelay;
            _walletService.Add(_productivity);
        }

        public void LevelUp()
        {
            if (_locked) return;
            
            _level++;
            _productivity = CalculateProductivity(_level);
            _nextLevelPrice = CalculateUpgradeCost(_level);
        }

        public void Unlock()
        {
            if (!_locked) return;
            
            _locked = false;
        }
        
        private int CalculateProductivity(int level)
        {
            var value = _config.StartProductivity * Mathf.Pow(_config.ProductivityGrowth, level - 1);
            return Mathf.RoundToInt(value);
        }

        private int CalculateUpgradeCost(int level)
        {
            var value = _config.BaseUpgradeCost * Mathf.Pow(_config.UpgradeCostGrowth, level - 1);
            return Mathf.CeilToInt(value);
        }
    }
}