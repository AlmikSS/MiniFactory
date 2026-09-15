using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Logging;
using UnityEngine;

namespace KofeyekToolkit.Core.TickSystem
{
    /// <summary>
    /// Передаёт Unity-обновления в <see cref="TickService"/>.
    /// </summary>
    public sealed class TickAdapter : MonoBehaviour
    {
        private TickService _tickService;
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;
        
        [Inject]
        private void Initialize(TickService tickService)
        {
            DontDestroyOnLoad(gameObject);
            _tickService = tickService;
            Message("Initialized.");
        }

        private void Update()
        {
            _tickService?.Update(Time.unscaledDeltaTime);
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
