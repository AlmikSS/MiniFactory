using System;

namespace MiniFactory.Persistence.Data
{
    [Serializable]
    public sealed class MachineSaveData
    {
        public string Id;
        public bool IsLocked;
        public int Level;
        public int Productivity;
        public int NextLevelPrice;
    }
}