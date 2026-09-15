using System;
using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using UnityEngine;

namespace KofeyekToolkit.Core.LifeCycle.Core.Requests
{
    internal readonly struct SpawnGameObjectRequest : ISpawnRequest
    {
        private readonly GameObject _prefab;
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Transform _parent;
        private readonly Action<GameObject> _onSpawned;

        /// <summary>
        /// Предоставляет API-член <c>SpawnGameObjectRequest</c>.
        /// </summary>
        public SpawnGameObjectRequest(GameObject prefab, Vector3 position, Quaternion rotation, Action<GameObject> onSpawned, Transform parent)
        {
            _prefab = prefab;
            _position = position;
            _rotation = rotation;
            _parent = parent;
            _onSpawned = onSpawned;
        }

        /// <summary>
        /// Выполняет введённую консольную команду.
        /// </summary>
        public void Execute(SpawnService spawnService)
        {
            var instance = spawnService.ExecutePhysicalSpawnGameObject(_prefab, _position, _rotation, _parent);
            _onSpawned?.Invoke(instance);
        }
    }
}