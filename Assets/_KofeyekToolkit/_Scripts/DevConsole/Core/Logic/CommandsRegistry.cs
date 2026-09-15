using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KofeyekToolkit.Logging;

namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Находит методы с атрибутом <see cref="CommandAttribute"/> и хранит их метаданные для выполнения из консоли.
    /// </summary>
    public static class CommandsRegistry
    {
        private static readonly Dictionary<string, ConsoleCommand> _commands = new(); 
        private static bool _isLoggingEnabled = true;
        /// <summary>
        /// Зарегистрированные команды, доступные для выполнения.
        /// </summary>
        public static IReadOnlyDictionary<string, ConsoleCommand> Commands => _commands;
        public static bool IsLoggingEnabled => _isLoggingEnabled;

        public static void EnableLogging(bool enable) => _isLoggingEnabled = enable;

        /// <summary>
        /// Находит и регистрирует все методы, отмеченные атрибутом команды.
        /// </summary>
        public static void RegisterAllCommands()
        {
            _commands.Clear();
            Message("Searching for command methods.");

            var methods = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(a => a.GetTypes()).
                SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                
                if (attribute == null)
                    continue;

                object target = null;

                if (!method.IsStatic)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(method.DeclaringType))
                        target = UnityEngine.Object.FindAnyObjectByType(method.DeclaringType);
                    else if (method.DeclaringType != null)
                        target = Activator.CreateInstance(method.DeclaringType);
                }

                var command = new ConsoleCommand(
                    attribute.Name,
                    attribute.Description,
                    method,
                    target,
                    method.GetParameters());
                
                _commands.Add(command.Name, command);
            }

            Message($"Registered {_commands.Count} commands.");
        }

        /// <summary>
        /// Ищет команду по имени.
        /// </summary>
        public static bool TryGetCommand(string commandName, out ConsoleCommand command)
        {
            return _commands.TryGetValue(commandName.ToLower(), out command);
        }

        private static void Message(string message)
        {
            if (_isLoggingEnabled)
                Log.Message(message);
        }

        private static void Warning(string message)
        {
            if (_isLoggingEnabled)
                Log.Warning(message);
        }

        private static void Error(string message)
        {
            if (_isLoggingEnabled)
                Log.Error(message);
        }

    }
}
