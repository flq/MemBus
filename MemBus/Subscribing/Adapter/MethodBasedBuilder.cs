using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Support;

namespace MemBus.Subscribing
{
    internal class MethodScanner : IMethodInfoScanner
    {
        private readonly Func<MethodInfo,bool> _methodSelector;
        private readonly Func<Type, bool> _returnTypeSpecifier;
        private readonly BindingFlags _methodBindingFlags;

        public MethodScanner(Func<MethodInfo, bool> methodSelector, Func<Type, bool> returnTypeSpecifier, BindingFlags methodBindingFlags)
        {
            _methodSelector = methodSelector;
            _returnTypeSpecifier = returnTypeSpecifier;
            _methodBindingFlags = methodBindingFlags;
        }

        public MethodScanner(string methodName, Func<Type, bool> returnTypeSpecifier)
            : this(mi => mi.Name == methodName, returnTypeSpecifier, BindingFlags.Public | BindingFlags.Instance)
        {
        }

        public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().MethodCandidatesForSubscriptionBuilders(_methodSelector, _returnTypeSpecifier, _methodBindingFlags).ToList();

            return candidates;
        }

        public static IMethodInfoScanner ForNonVoidMethods(string methodName)
        {
            return new MethodScanner(methodName, type => type != typeof(void));
        }

        public static IMethodInfoScanner ForVoidMethods(string methodName)
        {
            return new MethodScanner(methodName, type => type == typeof(void));
        }
    }
}