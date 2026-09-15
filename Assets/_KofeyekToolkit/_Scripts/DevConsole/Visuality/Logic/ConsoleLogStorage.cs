using System;
using System.Collections.Generic;

namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Хранит сообщения журнала, полученные консолью, и уведомляет подписчиков о новых записях.
    /// </summary>
    public static class ConsoleLogStorage
    {
        private static readonly List<ConsoleLog> _logs = new();
        
        /// <summary>
        /// Сохранённые записи журнала консоли.
        /// </summary>
        public static IEnumerable<ConsoleLog> Logs => _logs;
        public static event Action<ConsoleLog> LogAddedEvent;
        
        /// <summary>
        /// Добавляет запись в журнал консоли.
        /// </summary>
        public static void AddLog(string message, UnityEngine.LogType type)
        {
            var consoleLog = new ConsoleLog(message, type);
            _logs.Add(consoleLog);
            LogAddedEvent?.Invoke(consoleLog);
        }
    }
}