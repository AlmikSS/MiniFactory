using System;
using System.Collections.Generic;
using KofeyekToolkit.Logging;

namespace KofeyekToolkit.Events
{
    /// <summary>
    /// Регистрирует обработчики типизированных игровых событий и безопасно уведомляет их о публикации.
    /// </summary>
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;

        /// <summary>
        /// Регистрирует обработчик типизированного игрового события.
        /// </summary>
        public void Register<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_eventHandlers.ContainsKey(type))
                _eventHandlers[type] = new List<Delegate>();

            if (_eventHandlers[type].Contains(handler))
            {
                Warning($"Handler is already registered for {type.Name}.");
                return;
            }
            
            _eventHandlers[type].Add(handler);
            Message($"Registered handler for {type.Name}.");
        }

        /// <summary>
        /// Удаляет обработчик из подписок на событие.
        /// </summary>
        public void Unregister<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
                return;
            
            var type = typeof(T);
            if (!_eventHandlers.TryGetValue(type, out var eventHandler)) 
                return;
            
            eventHandler.Remove(handler);
            if (eventHandler.Count <= 0)
                _eventHandlers.Remove(type);
            
            Message($"Unregistered handler for {type.Name}.");
        }

        /// <summary>
        /// Публикует игровое событие для зарегистрированных обработчиков.
        /// </summary>
        public void Invoke<T>(T gameEvent) where T : IGameEvent
        {
            if (gameEvent == null)
                return;
            
            var type = typeof(T);
            if (!_eventHandlers.TryGetValue(type, out var eventHandler))
                return;
            
            var snapshot = eventHandler.ToArray();
            foreach (var handler in snapshot)
            {
                try
                {
                    var action = handler as Action<T>;
                    action?.Invoke(gameEvent);
                }
                catch (Exception ex)
                {
                    Error($"Failed to invoke a handler for {type.Name}: {ex.Message}");
                }
            }
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
