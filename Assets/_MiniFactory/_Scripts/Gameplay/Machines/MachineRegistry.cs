using System.Collections.Generic;
using UnityEngine;

namespace MiniFactory.Gameplay.Machines
{
    public sealed class MachineRegistry : MonoBehaviour
    {
        [SerializeField] private List<Machine> _machines = new();

        public IReadOnlyList<Machine> Machines => _machines;
    }
}