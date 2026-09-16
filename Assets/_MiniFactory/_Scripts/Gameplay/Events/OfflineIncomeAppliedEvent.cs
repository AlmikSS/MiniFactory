using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct OfflineIncomeAppliedEvent : IGameEvent
    {
        public int Amount;
        public long ElapsedSeconds;
        public long BoostedSeconds;
    }
}