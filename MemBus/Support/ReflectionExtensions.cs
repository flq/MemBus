using System;
using System.Collections.Generic;
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
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T), true);
            return atts.Length > 0;
        }


        /// <summary>
        /// Get a specific attribute from your target
        /// </summary>
        /// <typeparam name="T">The type of the attribute you are looking for</typeparam>
        /// <param name="provider">Target</param>
        /// <returns>The first attribute on the target that is of the desired type</returns>
        [Api]
        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            var atts = provider.GetCustomAttributes(typeof(T), true);
            return atts.Length > 0 ? atts[0] as T : null;
        }

        /// <summary>
        /// Do some work for every attribute of a desaired type
        /// </summary>
        /// <typeparam name="T">The desired attribute type</typeparam>
        /// <param name="provider">Target</param>
        /// <param name="action">An action to perform on a found attribute</param>
        [Api]
        [Obsolete("This helper will be removed in the near future")]
        public static void ForAttributesOf<T>(this ICustomAttributeProvider provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetCustomAttributes(typeof(T), true))
                action(attribute);
        }

        /// <summary>
        /// Ask some class whether it implements a certain interface
        /// </summary>
        /// <typeparam name="T">The interface you are looking for</typeparam>
        /// <param name="type">the inspected type</param>
        /// <returns>true if the interface is imaplemented</returns>
        public static bool ImplementsInterface<T>(this Type type) where T : class
        {
            return type.GetInterfaces().Any(t => t == typeof(T));
        }

        /// <summary>
        /// Raise an event
        /// </summary>
        public static void Raise(this EventHandler @event, object sender)
        {
            if (@event != null)
                @event(sender, EventArgs.Empty);
        }

        public static bool InterfaceIsSuitableAsHandlerType(this Type interfaceType)
        {
            return interfaceType.MethodsSuitableForSubscription().Count() == 1;
        }

        public static IEnumerable<MethodInfo> MethodsSuitableForSubscription(this Type interfaceType)
        {
            return from mi in interfaceType.GetMethods()
                   where mi.GetParameters().Length == 1 &&
                         mi.ReturnType.Equals(typeof(void))
                   select mi;
        }

        public static IEnumerable<MethodInfo> VoidMethodCandidatesForSubscriptionBuilders(this Type reflectedType, string methodName)
        {
            return reflectedType.MethodCandidatesForSubscriptionBuilders(methodName, t => t.Equals(typeof(void)));
        }

        public static IEnumerable<MethodInfo> ReturningMethodCandidatesForSubscriptionBuilders(this Type reflectedType, string methodName)
        {
            return reflectedType.MethodCandidatesForSubscriptionBuilders(methodName, t => !t.Equals(typeof(void)));
        }

        private static IEnumerable<MethodInfo> MethodCandidatesForSubscriptionBuilders(this Type reflectedType, string methodName, Func<Type,bool> returntypePredicate)
        {
            var candidates =
                (from mi in reflectedType.GetMethods()
                 where
                   mi.Name == methodName &&
                   !mi.IsGenericMethod &&
                   mi.GetParameters().Length == 1 &&
                   returntypePredicate(mi.ReturnType)
                 select mi);
            return candidates;
        }
    }
}