using System;
using System.Collections.Generic;

namespace MiniFactory.Persistence.Data
{
    [Serializable]
    public sealed class SaveData
    {
        public int Version = 1;
        public int Balance;
        public long LastExitUnixSeconds;
        public List<MachineSaveData> Machines = new();
        public BoostSaveData Boost = new();
    }
}