using System.Collections.Generic;
using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;
using MiniFactory.Persistence.Data;
using MiniFactory.Persistence.Interfaces;
using MiniFactory.Persistence.Logic;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class MachineController : ISaveable
    {
        private readonly List<Machine> _machines;
        private readonly WalletService _walletService;
        private readonly EventBus _eventBus;
        private readonly SaveService _saveService;

        public IReadOnlyList<Machine> Machines => _machines;
        
        public MachineController(IEnumerable<Machine> machines, WalletService walletService, EventBus eventBus, SaveService saveService)
        {
            _machines = new List<Machine>(machines);
            _walletService = walletService;
            _eventBus = eventBus;
            _saveService = saveService;
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
            
            _saveService?.MarkDirty();
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
            
            _saveService?.MarkDirty();
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
        
        public void Capture(SaveData data)
        {
            data.Machines.Clear();
            foreach (var m in _machines)
            {
                if (m == null) continue;

                data.Machines.Add(new MachineSaveData
                {
                    Id = m.Id,
                    IsLocked = m.IsLocked,
                    Level = m.Level,
                    Productivity = m.Productivity,
                    NextLevelPrice = m.NextLevelPrice
                });
            }
        }

        public void Restore(SaveData data)
        {
            if (data.Machines == null || data.Machines.Count == 0)
                return;

            foreach (var machine in _machines)
            {
                if (machine == null) continue;

                foreach (var saved in data.Machines)
                {
                    if (saved.Id != machine.Id)
                        continue;

                    machine.RestoreState(saved.IsLocked, saved.Level, saved.Productivity, saved.NextLevelPrice);
                    break;
                }
            }
        }
    }
}