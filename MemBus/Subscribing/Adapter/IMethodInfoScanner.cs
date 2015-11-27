using System;
using System.Collections.Generic;
using System.Reflection;
using MemBus.Support;

namespace MemBus.Subscribing
{

    internal interface IMethodInfoScanner
    {
        IEnumerable<ClassifiedMethodInfo> GetMethodInfos(object targetToAdapt);
    }

    /// <summary>
    /// Wraps a method info and contains the logic in what way said method is usable to MemBus.
    /// </summary>
    public class ClassifiedMethodInfo
    {
        public ClassifiedMethodInfo(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Classifier = DeriveClassifier();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassifiedMethodInfo) obj);
        }

        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }

        public MethodInfo MethodInfo { get; private set; }

        public MethodInfoClassifier Classifier { get; private set; }

        public static ClassifiedMethodInfo New(MethodInfo mi)
        {
            return new ClassifiedMethodInfo(mi);
        }

        protected bool Equals(ClassifiedMethodInfo other)
        {
            return MethodInfo.Equals(other.MethodInfo);
        }

        private MethodInfoClassifier DeriveClassifier()
        {
            var classifier = MethodInfoClassifier.Unusable;
            if (MethodHasParameter)
            {
                if (ParameterIsObservable)
                {
                    if (ReturnTypeIsVoid)
                        classifier = MethodInfoClassifier.ObservableSink;
                    if (MethodReturnsObservable)
                        classifier = MethodInfoClassifier.ObservableMap;
                }
                else
                {
                    if (ReturnTypeIsVoid)
                        classifier = MethodInfoClassifier.MessageSink;
                    else
                        classifier = MethodInfoClassifier.MessageMap;
                }
            }
            else if (MethodHasNoParameter && MethodReturnsObservable)
            {
                classifier = MethodInfoClassifier.ObservableSource;
            }
            return classifier;
        }

        private bool MethodReturnsObservable
        {
            get { return MethodInfo.ReturnType.IsConcreteObservable(); }
        }

        private bool ReturnTypeIsVoid
        {
            get { return MethodInfo.ReturnType == typeof (void); }
        }

        private bool ParameterIsObservable
        {
            get { return MethodInfo.GetParameters()[0].ParameterType.IsConcreteObservable(); }
        }

        private bool MethodHasParameter
        {
            get { return MethodInfo.GetParameters().Length == 1; }
        }

        private bool MethodHasNoParameter
        {
            get { return MethodInfo.GetParameters().Length == 0; }
        }
    }

    /// <summary>
    /// Further classifies a method info as to how it will be used by MemBus.
    /// Please note that where Observables are in use, the parameter type or return type *must* be of
    /// the interface type <see cref="IObservable{T}"/>, not some derived type.
    /// </summary>
    public enum MethodInfoClassifier
    {
        /// <summary>
        /// Method is unusable by MemBus
        /// </summary>
        Unusable = 0,
        /// <summary>
        /// The method accepts messages and also returns something which should be interpreted as message.
        /// Note that if the return value implements IEnumerable, the return value will be enumerated and each element will be published as message.
        /// This is not recursive.
        /// </summary>
        MessageMap,
        /// <summary>
        /// The method accepts messages but returns nothing in response.
        /// </summary>
        MessageSink,
        /// <summary>
        /// The method accepts an observable and returns nothing.
        /// </summary>
        ObservableSink,
        /// <summary>
        /// The method accepts an observable and returns an observable.
        /// </summary>
        ObservableMap,
        /// <summary>
        /// The method accepts no parameter but returns an observable.
        /// </summary>
        ObservableSource
    }
}