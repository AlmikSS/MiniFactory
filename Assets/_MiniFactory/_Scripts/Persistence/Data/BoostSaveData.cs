using System;

namespace MiniFactory.Persistence.Data
{
    [Serializable]
    public sealed class BoostSaveData
    {
        public bool IsActive;
        public long StartUnixSeconds;
        public long EndUnixSeconds;
        public float Multiplier;
    }
}