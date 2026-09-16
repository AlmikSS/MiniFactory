using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct BoostFinishedEvent : IGameEvent
    {
        public float Multiplier;
    }
}