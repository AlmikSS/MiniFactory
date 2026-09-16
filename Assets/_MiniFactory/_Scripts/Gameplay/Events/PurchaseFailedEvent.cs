using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct PurchaseFailedEvent : IGameEvent
    {
        public string ProductId;
        public string Reason;
    }
}