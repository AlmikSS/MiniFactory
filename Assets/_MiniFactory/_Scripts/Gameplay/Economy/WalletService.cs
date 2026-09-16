using System;
using KofeyekToolkit.Logging;
using MiniFactory.Persistence.Data;
using MiniFactory.Persistence.Interfaces;
using MiniFactory.Persistence.Logic;

namespace MiniFactory.Gameplay.Economy
{
    public sealed class WalletService : ISaveable
    {
        private readonly EconomyConfig _config;
        private readonly SaveService _saveService;
        private int _balance;

        public event Action<int> MoneyChangedEvent;

        public int Balance => _balance;

        public WalletService(EconomyConfig config, SaveService saveService)
        {
            _config = config;
            _saveService = saveService;
            _balance = _config.StartBalance;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            _balance += amount;
            MoneyChangedEvent?.Invoke(_balance);
            _saveService?.MarkDirty();
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
                return true;

            if (_balance < amount)
            {
                Log.Warning($"Not enough money. Required: {amount}, balance: {_balance}.");
                return false;
            }

            _balance -= amount;
            MoneyChangedEvent?.Invoke(_balance);
            _saveService?.MarkDirty();
            return true;
        }

        public void Capture(SaveData data)
        {
            data.Balance = _balance;
        }

        public void Restore(SaveData data)
        {
            _balance = data.Balance > 0 ? data.Balance : _config.StartBalance;
            MoneyChangedEvent?.Invoke(_balance);
        }
    }
}