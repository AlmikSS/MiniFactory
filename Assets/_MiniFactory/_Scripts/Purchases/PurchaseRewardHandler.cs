using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;

namespace MiniFactory.Purchases
{
    public sealed class PurchaseRewardHandler
    {
        private readonly WalletService _walletService;

        public PurchaseRewardHandler(EventBus eventBus, WalletService walletService)
        {
            _walletService = walletService;
            eventBus.Register<PurchaseSucceededEvent>(OnPurchaseSucceeded);
        }

        private void OnPurchaseSucceeded(PurchaseSucceededEvent evt)
        {
            if (evt.RewardAmount <= 0)
                return;

            _walletService.Add(evt.RewardAmount);
        }
    }
}