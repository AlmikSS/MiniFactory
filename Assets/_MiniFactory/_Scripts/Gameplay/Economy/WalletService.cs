using System;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Logging;

namespace MiniFactory.Gameplay.Economy
{
    [Register]
    public sealed class WalletService
    {
        public event Action<int> MoneyChangedEvent;

        public int Balance { get; private set; }

        public WalletService(EconomyConfig config)
        {
            Balance = config.StartBalance;
        }
        
        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
            MoneyChangedEvent?.Invoke(Balance);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
                return true;

            if (Balance < amount)
            {
                Log.Warning($"Not enough money. Required: {amount}, balance: {Balance}.");
                return false;
            }

            Balance -= amount;
            MoneyChangedEvent?.Invoke(Balance);
            return true;
        }
    }
}