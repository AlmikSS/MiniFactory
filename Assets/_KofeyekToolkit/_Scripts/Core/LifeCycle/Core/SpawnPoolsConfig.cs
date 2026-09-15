using System;
using System.Collections.Generic;
using UnityEngine;

namespace KofeyekToolkit.Core.LifeCycle.Core
{
    [CreateAssetMenu(fileName = "SpawnPoolsConfig", menuName = "KofeyekToolkit/SpawnPoolsConfig")]
    /// <summary>
    /// Хранит настраиваемые в Unity параметры пулов префабов для <see cref="SpawnService"/>.
    /// </summary>
    public sealed class SpawnPoolsConfig : ScriptableObject
    {
        [SerializeField] private List<PoolConfig> _pools = new();
        
        /// <summary>
        /// Настройки доступных пулов объектов.
        /// </summary>
        public IReadOnlyList<PoolConfig> Pools => _pools;
    }

    [Serializable]
    /// <summary>
    /// Описывает префаб и начальный размер одного пула объектов.
    /// </summary>
    public class PoolConfig
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _startPoolSize;

        /// <summary>
        /// Префаб, для которого создаётся пул.
        /// </summary>
        public GameObject Prefab => _prefab;
        /// <summary>
        /// Количество экземпляров, создаваемых при инициализации пула.
        /// </summary>
        public int StartPoolSize => _startPoolSize;
    }
}