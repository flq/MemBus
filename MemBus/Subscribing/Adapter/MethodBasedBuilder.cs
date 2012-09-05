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
        #if!WINRT
        private readonly BindingFlags _methodBindingFlags;
        #endif

        #if!WINRT
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
        #else
        public MethodScanner(Func<MethodInfo, bool> methodSelector, Func<Type, bool> returnTypeSpecifier)
        {
            _methodSelector = methodSelector;
            _returnTypeSpecifier = returnTypeSpecifier;
        }

        public MethodScanner(string methodName, Func<Type, bool> returnTypeSpecifier)
            : this(mi => mi.Name == methodName, returnTypeSpecifier)
        {
        }
        #endif


        public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");
            #if !WINRT
            var candidates = targetToAdapt.GetType().MethodCandidatesForSubscriptionBuilders(_methodSelector, _returnTypeSpecifier, _methodBindingFlags).ToList();
            #else
            var candidates = targetToAdapt.GetType().MethodCandidatesForSubscriptionBuilders(_methodSelector, _returnTypeSpecifier).ToList();
            #endif
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