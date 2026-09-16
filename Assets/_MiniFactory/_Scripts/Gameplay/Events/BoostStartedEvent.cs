using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct BoostStartedEvent : IGameEvent
    {
        public int Cost;
        public float DurationSeconds;
        public float Multiplier;
    }
}