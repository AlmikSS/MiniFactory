using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KofeyekToolkit.DI.Attributes;

namespace KofeyekToolkit.DI.Core
{
    /// <summary>
    /// Содержит подготовленные отражением сведения о конструкторе и членах, помеченных <see cref="InjectAttribute"/>.
    /// </summary>
    public sealed class TypeInfo
    {
        /// <summary>
        /// Выбранный конструктор типа.
        /// </summary>
        public ConstructorInfo Constructor { get; }
        /// <summary>
        /// Скомпилированная фабрика вызова конструктора.
        /// </summary>
        public Func<object[], object> ConstructorFactory { get; }
        /// <summary>
        /// Поля, в которые требуется внедрить зависимости.
        /// </summary>
        public List<FieldInfo> InjectFields { get; } = new();
        /// <summary>
        /// Свойства, в которые требуется внедрить зависимости.
        /// </summary>
        public List<PropertyInfo> InjectProperties { get; } = new();
        /// <summary>
        /// Методы, вызываемые после внедрения зависимостей.
        /// </summary>
        public List<MethodInfo> InjectMethods { get; } = new();
        /// <summary>
        /// Скомпилированные операции установки полей.
        /// </summary>
        public List<Action<object, object>> FieldSetters { get; } = new();
        /// <summary>
        /// Скомпилированные операции установки свойств.
        /// </summary>
        public List<Action<object, object>> PropertySetters { get; } = new();
        /// <summary>
        /// Скомпилированные операции вызова методов.
        /// </summary>
        public List<Action<object, object[]>> MethodInvokers { get; } = new();
        /// <summary>
        /// Признак наличия членов, помеченных для внедрения.
        /// </summary>
        public bool HasInjectMembers => InjectFields.Count > 0 || InjectProperties.Count > 0 || InjectMethods.Count > 0;

        /// <summary>
        /// Предоставляет API-член <c>TypeInfo</c>.
        /// </summary>
        public TypeInfo(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            Constructor = constructors.FirstOrDefault(c => c.GetCustomAttributes<InjectAttribute>() != null) ?? constructors.FirstOrDefault();
            
            if (Constructor != null)
                ConstructorFactory = CompileConstructorFactory(Constructor);
            
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in type.GetFields(flags))
            {
                if (field.GetCustomAttribute<InjectAttribute>() == null)
                    continue;
                
                InjectFields.Add(field);
                FieldSetters.Add(CompileFieldSetter(field));
            }

            foreach (var prop in type.GetProperties(flags))
            {
                if (prop.GetCustomAttribute<InjectAttribute>() == null || !prop.CanWrite)
                    continue;
                
                InjectProperties.Add(prop);
                PropertySetters.Add(CompilePropertySetter(prop));
            }
            
            foreach (var method in type.GetMethods(flags))
            {
                if (method.GetCustomAttribute<InjectAttribute>() == null) 
                    continue;
                
                InjectMethods.Add(method);
                MethodInvokers.Add(CompileMethodInvoker(method));
            }
        }

        private Func<object[], object> CompileConstructorFactory(ConstructorInfo ctor)
        {
            var argsParam = Expression.Parameter(typeof(object[]), "args");
            var parameters = ctor.GetParameters();
            var argExpr = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var access = Expression.ArrayIndex(argsParam, index);
                argExpr[i] = Expression.Convert(access, parameters[i].ParameterType);
            }

            var newExpr = Expression.New(ctor, argExpr);
            var lambda = Expression.Lambda<Func<object[], object>>(
                Expression.Convert(newExpr, typeof(object)), argsParam);
            return lambda.Compile();
        }
        
        private Action<object, object> CompileFieldSetter(FieldInfo field)
        {
            var instanceParam = Expression.Parameter(typeof(object[]), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var castInstance = Expression.Convert(instanceParam, field.DeclaringType);
            var fieldAccess = Expression.Field(castInstance, field);
            var castValue = Expression.Convert(valueParam, field.FieldType);
            var assign = Expression.Assign(fieldAccess, castValue);
            return Expression.Lambda<Action<object, object>>(assign, instanceParam, valueParam).Compile();
        }
        
        private Action<object, object> CompilePropertySetter(PropertyInfo prop)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var castInstance = Expression.Convert(instanceParam, prop.DeclaringType);
            var propertyAccess = Expression.Property(castInstance, prop);
            var castValue = Expression.Convert(valueParam, prop.PropertyType);
            var assign = Expression.Assign(propertyAccess, castValue);
            return Expression.Lambda<Action<object, object>>(assign, instanceParam, valueParam).Compile();
        }
        
        private Action<object, object[]> CompileMethodInvoker(MethodInfo method)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var argsParam = Expression.Parameter(typeof(object[]), "args");
            var castInstance = Expression.Convert(instanceParam, method.DeclaringType);
            var parameters = method.GetParameters();
            var argExpr = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var access = Expression.ArrayIndex(argsParam, index);
                argExpr[i] = Expression.Convert(access, parameters[i].ParameterType);
            }
            
            var callExpr = Expression.Call(castInstance, method, argExpr);
            return Expression.Lambda<Action<object, object[]>>(callExpr, instanceParam, argsParam).Compile();
        }
    }
}