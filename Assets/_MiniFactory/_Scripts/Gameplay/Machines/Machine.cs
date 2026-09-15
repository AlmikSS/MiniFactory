using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Logging;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class Machine : MonoBehaviour, IGameplayTickable
    {
        [SerializeField] private bool _locked = true;
        
        private MachinesConfig _config;
        private uint _level = 1;
        private float _productivity;
        private float _costToUnlock;
        private float _nextLevelPrice;
        
        [Inject]
        private void Initialize(MachinesConfig config)
        {
            _config = config;
            _productivity = _config.StartProductivity;
            _costToUnlock = _config.StartCostToUnlock;
            _nextLevelPrice = _config.StartNextLevelPrice;
            Log.Message("Machine initialized correctly.");
        }

        public void Tick(float deltaTime)
        {
            
        }
    }
}