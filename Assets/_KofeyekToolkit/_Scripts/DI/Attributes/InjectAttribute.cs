using System;

namespace KofeyekToolkit.DI.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    /// <summary>
    /// Помечает конструктор, поле, свойство или метод для внедрения зависимостей.
    /// </summary>
    public sealed class InjectAttribute : Attribute
    {
    }
}