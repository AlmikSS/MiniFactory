using KofeyekToolkit.Events;

namespace MiniFactory.GamePlay.Events
{
    public class AddMoneyEvent : IGameEvent
    {
        public readonly int Productivity;

        public AddMoneyEvent(int productivity)
        {
            Productivity = productivity;
        }
    }
}