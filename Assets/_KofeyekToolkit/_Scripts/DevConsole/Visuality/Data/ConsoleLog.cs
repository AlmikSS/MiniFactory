using UnityEngine;

namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Представляет одно сообщение, отображаемое в журнале разработческой консоли.
    /// </summary>
    public class ConsoleLog
    {
        /// <summary>
        /// Текст сообщения журнала.
        /// </summary>
        public readonly string Message;
        /// <summary>
        /// Тип сообщения журнала.
        /// </summary>
        public readonly LogType Type;

        /// <summary>
        /// Предоставляет API-член <c>ConsoleLog</c>.
        /// </summary>
        public ConsoleLog(string message, LogType type)
        {
            Message = message;
            Type = type;
        }
    }
}