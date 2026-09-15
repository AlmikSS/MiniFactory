using System;
using System.Collections.Generic;
using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Core.LifeCycle.Core.Requests;
using KofeyekToolkit.Core.TickSystem;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KofeyekToolkit.Core.LifeCycle.Core
{
    /// <summary>
    /// Централизованная система управления спавном и деспавном игровых объектов.
    /// Обеспечивает поддержку объектных пулов для оптимизации производительности,
    /// автоматическую регистрацию тиков в TickService и уведомление компонентов
    /// через жизненные интерфейсы (IConstructable, ISpawnable, IDespawnable, IDestroyable).
    /// </summary>
    /// <remarks>
    /// Сервис работает через очередь запросов, что гарантирует применение всех операций
    /// спавна и деспавна в безопасном контексте (в методе Tick).
    /// </remarks>
    public sealed class SpawnService : ISystemTickable
    {
        private readonly Dictionary<EntityId, ObjectPool> _pools = new();
        private readonly Queue<ISpawnRequest> _spawnQueue = new();
        private readonly Queue<GameObject> _despawnQueue = new();
        private readonly TickService _tickService;
        private readonly DIContainer _diContainer;

        /// <summary>
        /// Определяет, выводит ли сервис диагностические сообщения.
        /// </summary>
        public bool IsLoggingEnabled { get; private set; } = true;

        /// <summary>
        /// Предоставляет API-член <c>SpawnService</c>.
        /// </summary>
        public SpawnService(TickService tickService, DIContainer diContainer)
        {
            _tickService = tickService;
            _diContainer = diContainer;

            InitializePools();
            Message($"Initialized with {_pools.Count} object pools.");
        }

        /// <summary>
        /// Включает или выключает диагностическое логирование этого сервиса.
        /// </summary>
        public void EnableLogging(bool enable)
        {
            IsLoggingEnabled = enable;
        }

        internal void DespawnInSceneObjects()
        {
            var allObjects = Object.FindObjectsByType<SceneObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            Message($"Despawning {allObjects.Length} scene objects.");

            foreach (var obj in allObjects)
            {
                var instance = obj.gameObject;
                
                NotifyComponents<IDespawnable>(instance, component => component.OnDespawn());
                NotifyComponents<IDestroyable>(instance, component => component.OnDestroyed());
                UnregisterAllTickables(instance);
            }
        }
        
        internal void SpawnInSceneObjects()
        {
            var allObjects = Object.FindObjectsByType<SceneObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            Message($"Initializing {allObjects.Length} scene objects.");

            foreach (var obj in allObjects)
            {
                var instance = obj.gameObject;
                
                InjectAllComponents(instance);
                NotifyComponents<IConstructable>(instance, component => component.OnConstruct());
                NotifyComponents<ISpawnable>(instance, component => component.OnSpawn());
                RegisterAllTickables(instance);
            }
        }

        /// <summary>
        /// Помещает запрос на спавн GameObject в очередь на выполнение.
        /// </summary>
        /// <param name="prefab">Префаб для инстанцирования.</param>
        /// <param name="position">Позиция спавна.</param>
        /// <param name="rotation">Поворот спавна.</param>
        /// <param name="action">Опциональный колбэк, вызываемый после спавна.</param>
        /// <param name="parent">Опциональный родительский трансформ.</param>
        /// <summary>
        /// Добавляет запрос на создание объекта в очередь.
        /// </summary>
        public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Action<GameObject> action, Transform parent = null)
        {
            if (prefab == null)
            {
                Error("Cannot queue a spawn request for a null GameObject prefab.");
                return;
            }

            var request = new SpawnGameObjectRequest(prefab, position, rotation, action, parent);
            _spawnQueue.Enqueue(request);
            Message($"Queued spawn request for {prefab.name}.");
        }

        /// <summary>
        /// Помещает запрос на спавн компонента в очередь на выполнение.
        /// </summary>
        /// <typeparam name="T">Тип компонента, наследующего MonoBehaviour.</typeparam>
        /// <param name="prefab">Компонент префаба для инстанцирования.</param>
        /// <param name="position">Позиция спавна.</param>
        /// <param name="rotation">Поворот спавна.</param>
        /// <param name="action">Опциональный колбэк, вызываемый после спавна.</param>
        /// <param name="parent">Опциональный родительский трансформ.</param>
        /// <summary>
        /// Добавляет запрос на создание объекта в очередь.
        /// </summary>
        public void Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Action<T> action, Transform parent = null) where T : Component
        {
            if (prefab == null)
            {
                Error($"Cannot queue a spawn request for a null {typeof(T).Name} prefab.");
                return;
            }

            var request = new SpawnRequest<T>(prefab, position, rotation, action, parent);
            _spawnQueue.Enqueue(request);
            Message($"Queued spawn request for {prefab.name}.");
        }
        
        /// <summary>
        /// Помещает запрос на деспавн игрового объекта в очередь на выполнение.
        /// </summary>
        /// <param name="instance">Экземпляр игрового объекта для деспавна.</param>
        /// <summary>
        /// Добавляет запрос на возврат объекта в пул или уничтожение.
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null)
            {
                Warning("Skipped despawn request for a null GameObject.");
                return;
            }
            
            _despawnQueue.Enqueue(instance);
            Message($"Queued despawn request for {instance.name}.");
        }

        /// <summary>
        /// Обрабатывает накопленные запросы спавна и деспавна.
        /// Вызывается автоматически системой тиков.
        /// </summary>
        /// <param name="deltaTime">Дельта времени.</param>
        /// <summary>
        /// Выполняет обновление объекта с переданной дельтой времени.
        /// </summary>
        public void Tick(float deltaTime)
        {
            ApplySpawnQueue();
            ApplyDespawnQueue();
        }
        
        /// <summary>
        /// Инициализирует пулы объектов из конфигурационного файла SpawnPoolsConfig.
        /// Загружается через Resources.Load.
        /// </summary>
        private void InitializePools()
        {
            var poolsConfig = Resources.Load<SpawnPoolsConfig>("SpawnPoolsConfig");

            if (poolsConfig == null)
            {
                Warning("SpawnPoolsConfig was not found. Objects will be instantiated without pools.");
                return;
            }
            
            foreach (var config in poolsConfig.Pools)
            {
                var id = config.Prefab.gameObject.GetEntityId();
                var pool = new ObjectPool(config.Prefab, config.StartPoolSize, this);
                _pools.Add(id, pool);
            }

            Message($"Initialized {_pools.Count} object pools from SpawnPoolsConfig.");
        }
        
        private void ApplySpawnQueue()
        {
            while (_spawnQueue.Count > 0)
            {
                var request = _spawnQueue.Dequeue();
                if (request != null)
                    request.Execute(this);
            }
        }

        private void ApplyDespawnQueue()
        {
            while (_despawnQueue.Count > 0)
            {
                var instance = _despawnQueue.Dequeue();
                var id = instance.GetEntityId();
                if (_pools.TryGetValue(id, out var pool))
                {
                    pool.Return(instance);
                    continue;
                }

                NotifyComponents<IDespawnable>(instance, component => component.OnDespawn());
                NotifyComponents<IDestroyable>(instance, component => component.OnDestroyed());
                UnregisterAllTickables(instance);
                Object.Destroy(instance);
            }
        }
        
        /// <summary>
        /// Уведомляет все компоненты целевого объекта, реализующие указанный интерфейс,
        /// вызывая переданное действие.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
        /// <param name="target">Целевой игровой объект.</param>
        /// <param name="action">Действие, вызываемое для каждого компонента.</param>
        internal void NotifyComponents<TInterface>(GameObject target, Action<TInterface> action) where TInterface : class
        {
            var components = target.GetComponents<TInterface>();

            foreach (var component in components)
            {
                action(component);
            }
        }

        /// <summary>
        /// Выполняет физический спавн GameObject с учетом пулов.
        /// Если объект присутствует в пуле — извлекается из него, иначе инстанцируется.
        /// </summary>
        /// <param name="prefab">Префаб.</param>
        /// <param name="position">Позиция.</param>
        /// <param name="rotation">Поворот.</param>
        /// <param name="parent">Родитель.</param>
        /// <returns>Созданный экземпляр GameObject.</returns>
        internal GameObject ExecutePhysicalSpawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (prefab == null)
            {
                Error("Cannot spawn a null GameObject prefab.");
                return null;
            }
            
            var id = prefab.GetEntityId();
            GameObject instance;

            if (_pools.TryGetValue(id, out var pool))
            {
                instance = pool.Get(position, rotation, parent);
            }
            else
            {
                instance = Object.Instantiate(prefab, position, rotation, parent);
                InjectAllComponents(instance);
                NotifyComponents<IConstructable>(instance, component => component.OnConstruct());
                NotifyComponents<ISpawnable>(instance, component => component.OnSpawn());
                RegisterAllTickables(instance);
            }

            return instance;
        }

        /// <summary>
        /// Выполняет физический спавн компонента с учетом пулов.
        /// </summary>
        /// <typeparam name="T">Тип компонента.</typeparam>
        /// <param name="prefab">Компонент префаба.</param>
        /// <param name="position">Позиция.</param>
        /// <param name="rotation">Поворот.</param>
        /// <param name="parent">Родитель.</param>
        /// <returns>Компонент созданного экземпляра.</returns>
        internal T ExecutePhysicalSpawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            if (prefab == null)
            {
                Error($"Cannot spawn a null {typeof(T).Name} prefab.");
                return null;
            }
            
            var id = prefab.gameObject.GetEntityId();
            GameObject instance = null;

            if (_pools.TryGetValue(id, out var pool))
            {
                instance = pool.Get(position, rotation, parent);
            }
            else
            {
                instance = Object.Instantiate(prefab.gameObject, position, rotation, parent);
                InjectAllComponents(instance);
                NotifyComponents<IConstructable>(instance, component => component.OnConstruct());
                NotifyComponents<ISpawnable>(instance, component => component.OnSpawn());
                RegisterAllTickables(instance);
            }

            return instance == null ? null : instance.GetComponent<T>();
        }

        /// <summary>
        /// Регистрирует массив тиков в TickService.
        /// </summary>
        /// <param name="tickables">Массив объектов, реализующих ITickable.</param>
        internal void RegisterAllTickables(ITickable[] tickables)
        {
            foreach (var tickable in tickables)
            {
                _tickService.Register(tickable);
            }
        }

        /// <summary>
        /// Отменяет регистрацию массива тиков в TickService.
        /// </summary>
        /// <param name="tickables">Массив объектов, реализующих ITickable.</param>
        internal void UnregisterAllTickables(ITickable[] tickables)
        {
            foreach (var tickable in tickables)
            {
                _tickService.Unregister(tickable);
            }
        }
        
        private void RegisterAllTickables(GameObject gameObject)
        {
            var tickables = gameObject.GetComponentsInChildren<ITickable>();
            RegisterAllTickables(tickables);
        }

        private void UnregisterAllTickables(GameObject gameObject)
        {
            var tickables = gameObject.GetComponentsInChildren<ITickable>();
            UnregisterAllTickables(tickables);
        }
        
        internal void InjectAllComponents(GameObject root)
        {
            var components = root.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var component in components)
            {
                var metadata = TypeMetadata.Get(component.GetType());
                if (metadata.HasInjectMembers)
                {
                    _diContainer.Inject(component);
                }
            }
        }

        private void Message(string message)
        {
            if (IsLoggingEnabled)
                Log.Message(message);
        }

        private void Warning(string message)
        {
            if (IsLoggingEnabled)
                Log.Warning(message);
        }

        private void Error(string message)
        {
            if (IsLoggingEnabled)
                Log.Error(message);
        }
    }
}
