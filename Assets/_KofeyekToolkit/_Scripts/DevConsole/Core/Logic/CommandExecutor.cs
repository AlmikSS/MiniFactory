using System;
using System.Linq;
using UnityEngine;
using KofeyekToolkit.Logging;

namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Разбирает введённую строку консоли, преобразует аргументы и вызывает зарегистрированную команду.
    /// </summary>
    public static class CommandExecutor
    {
        private static bool _isLoggingEnabled = true;

        public static bool IsLoggingEnabled => _isLoggingEnabled;

        public static void EnableLogging(bool enable) => _isLoggingEnabled = enable;

        /// <summary>
        /// Выполняет введённую консольную команду.
        /// </summary>
        public static void Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;
            
            var parts = input.Split();
            var commandName = parts[0];
            var args = parts.Skip(1).ToArray();

            if (!CommandsRegistry.TryGetCommand(commandName, out var command))
            {
                Warning($"Command '{commandName}' was not found.");
                return;
            }

            if (args.Length != command.Parameters.Length)
            {
                Warning($"Command '{commandName}' expects {command.Parameters.Length} arguments, received {args.Length}.");
                return;
            }

            try
            {
                var convertedArgs = new object[args.Length];
                
                for (var i = 0; i < args.Length; i++)
                {
                    convertedArgs[i] = ConvertArgument(args[i], command.Parameters[i].ParameterType);
                }
                
                command.Method.Invoke(command.Target, convertedArgs);
                Message($"Executed command '{commandName}'.");
            }
            catch (Exception e)
            {
                Error($"Command '{commandName}' failed: {e.Message}");
            }
        }
        
        private static object ConvertArgument(string arg, Type type)
        {
            if (type == typeof(string))
                return arg;
            
            if (type == typeof(int))
                return int.Parse(arg);
            
            if (type == typeof(float))
                return float.Parse(arg);

            if (type == typeof(bool))
                return bool.Parse(arg);

            if (type == typeof(Vector2))
                return ParseVector2(arg);

            if (type == typeof(Vector3))
                return ParseVector3(arg);
            
            if (type.IsEnum)
                return Enum.Parse(type, arg, true);
            
            Error($"Unsupported parameter type: {type.Name}.");
            return null;
        }
        
        private static Vector2 ParseVector2(string input)
        {
            var parts = input.Split(',');

            if (parts.Length != 2)
                throw new Exception("Vector2 must be in format: x,y");

            return new Vector2(
                float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture)
            );
        }

        private static Vector3 ParseVector3(string input)
        {
            var parts = input.Split(',');

            if (parts.Length != 3)
                throw new Exception("Vector3 must be in format: x,y,z");

            return new Vector3(
                float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)
            );
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
