using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.GamePlay.Events;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class Machine : MonoBehaviour, IGameplayTickable
    {
        [SerializeField] private bool _locked = true;
        [SerializeField] private MachineConfig _config;
        
        private EventBus _eventBus;
        private uint _level = 1;
        private int _productivity;
        private int _costToUnlock;
        private int _nextLevelPrice;
        private float _addMoneyDelay;
        private float _addMoneyTimer;
        
        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
            _productivity = _config.StartProductivity;
            _costToUnlock = _config.BaseUnlockCost;
            _nextLevelPrice = _config.BaseUpgradeCost;
            Log.Message("Machine initialized correctly.");
        }

        public void Tick(float deltaTime)
        {
            if (_locked) return;
            
            _addMoneyTimer += deltaTime;
            if (!(_addMoneyTimer >= _addMoneyDelay))
                return;
            
            _eventBus.Invoke(new AddMoneyEvent(_productivity));
            _addMoneyTimer = 0f;
        }

        public void LevelUp()
        {
            if (_locked) return;
            
            CalculateNewLevelStats();
        }

        public void Unlock()
        {
            if (!_locked) return;
            
            _locked = true;
        }

        private void CalculateNewLevelStats()
        {
            _level++;
            //TODO Some formulas
        }
    }
}