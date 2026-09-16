using KofeyekToolkit.Events;

namespace MiniFactory.Gameplay.Events
{
    public struct MachineUnlockedEvent : IGameEvent
    {
        public string MachineId;
        public int Cost;
    }
}