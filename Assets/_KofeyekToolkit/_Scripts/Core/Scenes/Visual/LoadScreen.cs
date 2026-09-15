using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Logging;
using UnityEngine;

namespace KofeyekToolkit.Core.Scenes.Visual
{
    /// <summary>
    /// Отображает или скрывает сохраняемый между сценами экран загрузки.
    /// </summary>
    public sealed class LoadScreen : MonoBehaviour, IConstructable
    {
        [SerializeField] private GameObject _root;
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;
        
        /// <summary>
        /// Инициализирует компонент после создания.
        /// </summary>
        public void OnConstruct()
        {
            DontDestroyOnLoad(gameObject);
            Hide();
            Message("Initialized.");
        }

        internal void Hide()
        {
            if (_root == null)
            {
                Error("Cannot hide because the root object is not assigned.");
                return;
            }

            _root.SetActive(false);
        }

        internal void Show()
        {
            if (_root == null)
            {
                Error("Cannot show because the root object is not assigned.");
                return;
            }

            _root.SetActive(true);
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
