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
            if (_level > 0)
                return;
            
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
            _productivity = MachineMath.CalculateProductivity(_config.StartProductivity, _config.ProductivityGrowth, _level);
            _nextLevelPrice = MachineMath.CalculateUpgradeCost(_config.BaseUpgradeCost, _config.UpgradeCostGrowth, _level);
        }

        public void Unlock()
        {
            if (!_locked) return;
            
            _locked = false;
        }
        
        public void RestoreState(bool isLocked, int level, int productivity, int nextLevelPrice)
        {
            _locked = isLocked;
            _level = level;
            _productivity = productivity;
            _nextLevelPrice = nextLevelPrice;
        }
    }
}