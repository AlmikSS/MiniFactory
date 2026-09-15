using System.Reflection;

namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Содержит метаданные команды разработческой консоли: имя, описание, целевой метод и его параметры.
    /// </summary>
    public sealed class ConsoleCommand
    {
        /// <summary>
        /// Имя сущности.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Текстовое описание сущности.
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// Метод, реализующий команду.
        /// </summary>
        public readonly MethodInfo Method;
        /// <summary>
        /// Экземпляр, на котором вызывается метод команды.
        /// </summary>
        public readonly object Target;
        /// <summary>
        /// Параметры метода команды.
        /// </summary>
        public readonly ParameterInfo[] Parameters;

        /// <summary>
        /// Предоставляет API-член <c>ConsoleCommand</c>.
        /// </summary>
        public ConsoleCommand(string name, string description, MethodInfo method, object target, ParameterInfo[] parameters)
        {
            Name = name;
            Description = description;
            Method = method;
            Target = target;
            Parameters = parameters;
        }
    }
}