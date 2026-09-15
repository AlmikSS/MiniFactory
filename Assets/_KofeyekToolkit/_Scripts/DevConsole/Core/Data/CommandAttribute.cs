using System;

namespace KofeyekToolkit.DevConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    /// <summary>
    /// Помечает метод как команду разработческой консоли и задаёт её имя и описание.
    /// </summary>
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// Имя сущности.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Текстовое описание сущности.
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// Предоставляет API-член <c>CommandAttribute</c>.
        /// </summary>
        public CommandAttribute(string name, string description)
        {
            Name = name.ToLower();
            Description = description;
        }
    }
}