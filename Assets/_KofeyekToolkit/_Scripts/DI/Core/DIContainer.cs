using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Logging;
using UnityEngine;

namespace KofeyekToolkit.DI.Core
{
    /// <summary>
    /// Регистрирует сервисы, создаёт их экземпляры и внедряет зависимости по атрибутам.
    /// </summary>
    public sealed class DIContainer : IDisposable
    {
        private readonly Dictionary<Type, Func<object>> _transientFactories = new();
        private readonly Dictionary<Type, object> _singletons = new();
        private readonly Dictionary<Type, Func<object[], object>> _singletonFactories = new();
        private readonly HashSet<Type> _resolving = new();
        private readonly Dictionary<Type, Type> _implementationTypes = new();
        private bool _disposed;
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;

        /// <summary>
        /// Инициализирует контейнер и подписывает его на завершение работы приложения.
        /// </summary>
        public DIContainer()
        {
            Application.quitting += OnApplicationQuitting;
            Message("Created.");
        }

        private void OnApplicationQuitting()
        {
            Dispose();
        }

        /// <summary>
        /// Освобождает ресурсы контейнера и отменяет подписку на завершение работы приложения.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            
            _disposed = true;

            Application.quitting -= OnApplicationQuitting;
            _transientFactories.Clear();
            _singletons.Clear();
            _singletonFactories.Clear();
            _resolving.Clear();
            _implementationTypes.Clear();
            Message("Disposed.");
        }
        
        /// <summary>
        /// Регистрирует сервис или обработчик в контейнере.
        /// </summary>
        public void Register<TImplementation>()
        {
            Register(typeof(TImplementation));
        }
        
        /// <summary>
        /// Регистрирует сервис или обработчик в контейнере.
        /// </summary>
        public void Register(Type implementationType)
        {
            var attrs = implementationType.GetCustomAttributes<RegisterAttribute>();
            foreach (var attr in attrs)
            {
                var contractType = attr.ContractType ?? implementationType;
                switch (attr.RegisterType)
                {
                    case RegisterType.Singleton:
                        RegisterSingleton(contractType, implementationType);
                        break;
                    case RegisterType.Transient:
                        RegisterTransient(contractType, implementationType);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Регистрирует сервис или обработчик в контейнере.
        /// </summary>
        public void Register<TService, TImplementation>(RegisterType registerType = RegisterType.Singleton) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), registerType);
        }

        /// <summary>
        /// Регистрирует сервис или обработчик в контейнере.
        /// </summary>
        public void Register(Type contractType, Type implementationType, RegisterType registerType)
        {
            if (registerType == RegisterType.Singleton)
                RegisterSingleton(contractType, implementationType);
            else
                RegisterTransient(contractType, implementationType);
        }
        
        /// <summary>
        /// Автоматически регистрирует типы с атрибутом регистрации из указанных сборок.
        /// </summary>
        public void RegisterServicesFromAssemblies(params Assembly[] assemblies)
        {
            foreach (var asm in assemblies)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.GetCustomAttributes<RegisterAttribute>().Any())
                        Register(type);
                }
            }
        }
        
        /// <summary>
        /// Регистрирует готовый экземпляр как singleton и внедряет его зависимости.
        /// </summary>
        public void RegisterInstance<TService>(TService instance)
        {
            _singletons[typeof(TService)] = instance;
            Message("Registered instance: " + instance.GetType().Name);
            Inject(instance);
        }
        
        /// <summary>
        /// Возвращает экземпляр зарегистрированного сервиса.
        /// </summary>
        public T Resolve<T>() => (T)Resolve(typeof(T));
        
        /// <summary>
        /// Возвращает экземпляр зарегистрированного сервиса.
        /// </summary>
        public object Resolve(Type serviceType)
        {
            if (_resolving.Contains(serviceType))
            {
                Error($"Circular dependency detected for {serviceType.Name}.");
                return null;
            }
            _resolving.Add(serviceType);
            try
            {
                if (_singletons.TryGetValue(serviceType, out var singleton))
                {
                    if (singleton == null)
                    {
                        var factory = _singletonFactories[serviceType];
                        var meta = TypeMetadata.Get(GetImplementationType(serviceType));
                        var args = ResolveParameters(meta.Constructor);
                        singleton = factory(args);
                        _singletons[serviceType] = singleton;
                        Inject(singleton);
                    }
                    return singleton;
                }

                if (_transientFactories.TryGetValue(serviceType, out var transientFactory))
                    return transientFactory();
                
                Error($"No registration for {serviceType.Name}.");
                return null;
            }
            finally
            {
                _resolving.Remove(serviceType);
            }
        }
        
        /// <summary>
        /// Внедряет зависимости в поля, свойства и методы целевого объекта.
        /// </summary>
        public void Inject(object target)
        {
            var meta = TypeMetadata.Get(target.GetType());
            
            for (var i = 0; i < meta.InjectFields.Count; i++)
            {
                var field = meta.InjectFields[i];
                var value = Resolve(field.FieldType);
                meta.FieldSetters[i](target, value);
            }
            
            for (var i = 0; i < meta.InjectProperties.Count; i++)
            {
                var prop = meta.InjectProperties[i];
                var value = Resolve(prop.PropertyType);
                meta.PropertySetters[i](target, value);
            }
            
            for (var i = 0; i < meta.InjectMethods.Count; i++)
            {
                var method = meta.InjectMethods[i];
                var parameters = method.GetParameters();
                var args = new object[parameters.Length];
                for (var p = 0; p < parameters.Length; p++)
                    args[p] = Resolve(parameters[p].ParameterType);
                meta.MethodInvokers[i](target, args);
            }
        }
        
        private void RegisterSingleton(Type contractType, Type implementationType)
        {
            var meta = TypeMetadata.Get(implementationType);
            _singletonFactories[contractType] = meta.ConstructorFactory;
            _singletons[contractType] = null;
            _implementationTypes[contractType] = implementationType;
            Message("Registered singleton: " + implementationType.Name);
        }

        private void RegisterTransient(Type contractType, Type implementationType)
        {
            var factory = CreateTransientFactory(implementationType);
            _transientFactories[contractType] = factory;
            _implementationTypes[contractType] = implementationType;
            Message("Registered transient: " + implementationType.Name);
        }

        private Func<object> CreateTransientFactory(Type implType)
        {
            var meta = TypeMetadata.Get(implType);
            return () =>
            {
                var args = ResolveParameters(meta.Constructor);
                var instance = meta.ConstructorFactory(args);
                Inject(instance);
                return instance;
            };
        }

        private object[] ResolveParameters(ConstructorInfo ctor)
        {
            if (ctor == null) return Array.Empty<object>();
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = Resolve(parameters[i].ParameterType);
            }
            return args;
        }

        private Type GetImplementationType(Type contractType)
        {
            return _implementationTypes.TryGetValue(contractType, out var impl) ? impl : contractType;
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
