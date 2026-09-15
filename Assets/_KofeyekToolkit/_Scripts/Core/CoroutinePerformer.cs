using UnityEngine;

namespace KofeyekToolkit.Core
{
    /// <summary>
    /// Предоставляет сохраняемый между сценами компонент для запуска корутин из обычных классов.
    /// </summary>
    public sealed class CoroutinePerformer : MonoBehaviour
    {
        /// <summary>
        /// Единственный экземпляр компонента для запуска корутин.
        /// </summary>
        public static CoroutinePerformer Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}