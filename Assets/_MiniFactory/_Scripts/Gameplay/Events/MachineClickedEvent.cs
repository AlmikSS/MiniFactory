using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Machines;

namespace MiniFactory.Gameplay.Events
{
    public struct MachineClickedEvent : IGameEvent
    {
        public Machine Machine;
    }
}