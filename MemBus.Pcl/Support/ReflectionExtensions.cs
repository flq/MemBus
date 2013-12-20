using System;
using System.Reflection;
using System.Linq;

namespace MemBus.Support
{
    /// <summary>
    /// A number of reflection Extension Methods to help you
    /// in working with attributes and other reflection mechanisms
    /// </summary>
    public static class ReflectionExtensions
    {

        /// <summary>
        /// Determine whether a certain custom attribute is specified on this element
        /// </summary>
        /// <typeparam name="T">The type of the attribute you are looking for</typeparam>
        /// <param name="provider">Target</param>
        /// <returns>True if this attribute is defined on this target</returns>
        [Api]
        public static bool HasAttribute<T>(this MemberInfo provider) where T : Attribute
        {
            return provider.GetCustomAttributes<T>().Any();
        }

        /// <summary>
        /// Normalization method
        /// </summary>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static InterfaceMapping GetRuntimeInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
        }

        public static bool IsConcreteObservable(this Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof (IObservable<>);
        }

        /// <summary>
        /// Get a specific attribute from your target
        /// </summary>
        /// <typeparam name="T">The type of the attribute you are looking for</typeparam>
        /// <param name="provider">Target</param>
        /// <returns>The first attribute on the target that is of the desired type</returns>
        [Api]
        public static T GetAttribute<T>(this MemberInfo provider) where T : Attribute
        {
            return provider.GetCustomAttributes<T>().FirstOrDefault();
        }

        /// <summary>
        /// Ask some class whether it implements a certain interface
        /// </summary>
        /// <typeparam name="T">The interface you are looking for</typeparam>
        /// <param name="type">the inspected type</param>
        /// <returns>true if the interface is imaplemented</returns>
        public static bool ImplementsInterface<T>(this Type type) where T : class
        {
            return type.GetTypeInfo().ImplementedInterfaces.Any(t => t == typeof(T));
        }
        
        public static bool CanBeCastTo<T>(this Type type)
        {
            return typeof (T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public static bool CanBeCastTo(this Type type, Type otherType)
        {
            return otherType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

    }
}