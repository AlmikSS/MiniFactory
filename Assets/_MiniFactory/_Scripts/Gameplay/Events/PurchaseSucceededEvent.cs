using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct PurchaseSucceededEvent : IGameEvent
    {
        public string ProductId;
        public int RewardAmount;
    }
}