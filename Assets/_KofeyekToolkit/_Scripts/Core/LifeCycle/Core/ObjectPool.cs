using System;
using System.Collections.Generic;
using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KofeyekToolkit.Core.LifeCycle.Core
{
    /// <summary>
    /// Повторно использует экземпляры префаба, уменьшая количество операций создания и уничтожения объектов.
    /// </summary>
    public sealed class ObjectPool : IDisposable
    {
        private readonly GameObject _objectPrefab;
        private readonly Queue<(GameObject Instance, ITickable[] Tickables)> _poolQueue = new();
        private readonly SpawnService _spawnService;
        private readonly Transform _root;
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;
        
        /// <summary>
        /// Создаёт пул для указанного префаба и заполняет его начальными экземплярами.
        /// </summary>
        public ObjectPool(GameObject objectPrefab, int capacity, SpawnService spawnService, Transform parent = null)
        {
            _objectPrefab = objectPrefab;
            _spawnService = spawnService;
            
            _root = new GameObject($"{_objectPrefab.name}_Pool").transform;
            _root.SetParent(parent);

            for (var i = 0; i < capacity; i++)
            {
                SpawnInstance();
            }

            Message($"Created with {capacity} preallocated instances.");
        }
        
        /// <summary>
        /// Освобождает ресурсы и уничтожает объекты пула.
        /// </summary>
        public void Dispose()
        {
            while (_poolQueue.Count > 0)
            {
                var instance = _poolQueue.Dequeue();
                _spawnService.NotifyComponents<IDestroyable>(instance.Instance, component => component.OnDestroyed());
            }
            
            Object.Destroy(_root);
            Message("Disposed.");
        }

        /// <summary>
        /// Извлекает объект из пула и подготавливает его к использованию.
        /// </summary>
        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_poolQueue.Count <= 0)
            {
                Warning("Pool was empty; creating an additional instance.");
                SpawnInstance();
            }
            
            var instance = _poolQueue.Dequeue();
            
            instance.Instance.transform.SetPositionAndRotation(position, rotation);
            instance.Instance.transform.SetParent(parent);
            instance.Instance.SetActive(true);
            _spawnService.InjectAllComponents(instance.Instance);
            _spawnService.NotifyComponents<ISpawnable>(instance.Instance, component => component.OnSpawn());
            _spawnService.RegisterAllTickables(instance.Tickables);
            return instance.Instance;
        }

        /// <summary>
        /// Возвращает объект в пул.
        /// </summary>
        public void Return(GameObject instance)
        {
            var tickables = instance.GetComponentsInChildren<ITickable>();
            instance.transform.SetParent(_root);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.SetActive(false);
            _spawnService.NotifyComponents<IDespawnable>(instance, component => component.OnDespawn());
            _spawnService.UnregisterAllTickables(tickables);
            _poolQueue.Enqueue((instance, tickables));
            Message($"Returned {instance.name} to the pool.");
        }
        
        private void SpawnInstance()
        {
            var instance = Object.Instantiate(_objectPrefab, _root);
            var tickables = instance.GetComponentsInChildren<ITickable>();
            _poolQueue.Enqueue((instance, tickables));
            instance.SetActive(false);
            _spawnService.NotifyComponents<IConstructable>(instance, component => component.OnConstruct());
        }

        private void Message(string message)
        {
            if (_isLoggingEnabled)
                Log.Message(message);
        }

        private void Warning(string message)
        {
            if (_isLoggingEnabled)
                Log.Warning(message);
        }

        private void Error(string message)
        {
            if (_isLoggingEnabled)
                Log.Error(message);
        }

    }
}
