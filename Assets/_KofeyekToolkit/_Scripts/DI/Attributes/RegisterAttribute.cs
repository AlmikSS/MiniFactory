using System;

namespace KofeyekToolkit.DI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    /// <summary>
    /// Задаёт контракт и время жизни типа для автоматической регистрации в <see cref="DIContainer"/>.
    /// </summary>
    public sealed class RegisterAttribute : Attribute
    {
        /// <summary>
        /// Контракт зарегистрированного сервиса.
        /// </summary>
        public Type ContractType { get; }
        /// <summary>
        /// Время жизни зарегистрированного сервиса.
        /// </summary>
        public RegisterType RegisterType { get; set; } 
        
        /// <summary>
        /// Предоставляет API-член <c>RegisterAttribute</c>.
        /// </summary>
        public RegisterAttribute(Type contractType = null, RegisterType registerType = RegisterType.Singleton)
        {
            ContractType = contractType;
            RegisterType = registerType;
        }
    }

    /// <summary>
    /// Определяет время жизни зарегистрированного сервиса.
    /// </summary>
    public enum RegisterType
    {
        Singleton,
        Transient,
    }
}