using System;
using System.Collections.Generic;

namespace KofeyekToolkit.DI.Core
{
    /// <summary>
    /// Кэширует метаданные типов, используемые контейнером зависимостей.
    /// </summary>
    public static class TypeMetadata
    {
        private static readonly Dictionary<Type, TypeInfo> Cache = new();

        /// <summary>
        /// Извлекает объект из пула и подготавливает его к использованию.
        /// </summary>
        public static TypeInfo Get(Type type)
        {
            if (!Cache.TryGetValue(type, out var info))
            {
                info = new  TypeInfo(type);
                Cache[type] = info;
            }
            
            return info;
        }
    }
}