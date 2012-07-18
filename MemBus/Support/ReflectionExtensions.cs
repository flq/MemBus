using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MemBus.Subscribing;

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

        public static IEnumerable<MethodInfo> MethodCandidatesForSubscriptionBuilders(
            this Type reflectedType, 
            Func<MethodInfo, bool> methodSelector, 
            Func<Type, bool> returntypePredicate, 
            BindingFlags methodBindingFlags)
        {
            var disposeTokenMethod = reflectedType.ImplementsInterface<IAcceptDisposeToken>()
                                         ? (mi =>
                                            mi.Name == "Accept" && mi.GetParameters().Length == 1 &&
                                            mi.GetParameters()[0].ParameterType == typeof (IDisposable))
                                         : new Func<MethodInfo, bool>(mi => false);

            var candidates =
                (from mi in reflectedType.GetMethods(methodBindingFlags)
                 where
                 !mi.IsGenericMethod &&
                   mi.GetParameters().Length == 1 &&
                   !disposeTokenMethod(mi) &&
                   returntypePredicate(mi.ReturnType) &&
                   methodSelector(mi)
                 select mi);
            return candidates;
        }
    }
}