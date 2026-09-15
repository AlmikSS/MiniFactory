using System.Collections.Generic;
using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class MachineController
    {
        private readonly List<Machine> _machines;
        private readonly WalletService _walletService;
        private readonly EventBus _eventBus;

        public IReadOnlyList<Machine> Machines => _machines;
        
        public MachineController(IEnumerable<Machine> machines, WalletService walletService, EventBus eventBus)
        {
            _machines = new List<Machine>(machines);
            _walletService = walletService;
            _eventBus = eventBus;
        }
        
        public bool TryUnlock(Machine machine)
        {
            if (machine == null || !machine.IsLocked)
                return false;

            var cost = machine.UnlockCost;
            if (!_walletService.TrySpend(cost))
                return false;

            machine.Unlock();
            _eventBus.Invoke(new MachineUnlockedEvent
            {
                MachineId = machine.Id,
                Cost = cost
            });
            return true;
        }
        
        public bool TryUpgrade(Machine machine)
        {
            if (machine == null || machine.IsLocked)
                return false;

            var cost = machine.NextLevelPrice;
            if (!_walletService.TrySpend(cost))
                return false;

            machine.LevelUp();
            _eventBus.Invoke(new MachineUpgradedEvent
            {
                MachineId = machine.Id,
                NewLevel = machine.Level,
                Cost = cost
            });
            return true;
        }
        
        public int GetTotalIncomePerSecond()
        {
            var total = 0;
            foreach (var machine in _machines)
            {
                if (machine == null || machine.IsLocked)
                    continue;

                total += Mathf.RoundToInt(machine.Productivity / machine.AddMoneyDelay);
            }

            return total;
        }
    }
}