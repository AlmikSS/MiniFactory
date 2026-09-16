using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct MachineUpgradedEvent : IGameEvent
    {
        public string MachineId;
        public int NewLevel;
        public int Cost;
    }
}