using MiniFactory.Persistence.Data;

namespace MiniFactory.Persistence.Interfaces
{
    public interface ISaveable
    {
        void Capture(SaveData data);
        void Restore(SaveData data);
    }
}