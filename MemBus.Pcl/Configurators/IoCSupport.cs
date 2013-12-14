using System;
using System.Reflection;
using MemBus.Setup;
using MemBus.Support;


namespace MemBus.Configurators
{
    /// <summary>
    /// Add ServiceLocator Support
    /// </summary>
    public class IoCSupport : ISetup<IConfigurableBus>
    {
        private IocAdapter _adapter;
        private Type _handlerType;
        private Func<Type, Type> _messageTypeResolver = msgT => msgT;

        /// <summary>
        /// Add an IoCadapter that will be used to resolve subscriptions. subscriptions will be resolved based on the interface you provide
        /// </summary>
        public IoCSupport SetAdapter(IocAdapter adapter)
        {
            _adapter = adapter;
            return this;
        }

        /// <summary>
        /// Set the interface type. It needs to be a generic interface with one type argument.
        /// (Don't specify the type, e.g. typeof(IHandles&lt;&gt;)
        /// It must define only one method that is void and accepts one argument 
        /// (typically of the message type)
        /// </summary>
        public IoCSupport SetHandlerInterface(Type openGenericHandlerType)
        {
            _handlerType = openGenericHandlerType;
            return this;
        }

        /// <summary>
        /// Set the function that will be used to resolve the type of message that will be used when resolving handler type.
        /// It may be useful if the generic type of the open generic handler interface is not exactly matching the type of the messages passed.
        /// </summary>
        public IoCSupport SetMessageTypeResolver(Func<Type, Type> messageTypeResolver)
        {
            _messageTypeResolver = messageTypeResolver;
            return this;
        }

        void ISetup<IConfigurableBus>.Accept(IConfigurableBus setup)
        {
            ThrowIfBadPreconditions();
            setup.AddService(_adapter);
            setup.AddResolver(new IoCBasedResolver(_adapter, _handlerType, _messageTypeResolver));
        }

        private void ThrowIfBadPreconditions()
        {
            if (_adapter == null)
                throw new ArgumentException("The IocAdapter has not been specified on the Ioc support.");
            if (_handlerType == null)
                throw new ArgumentException("No handler type has been specified.");
            if (!_handlerType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("An open generic should be specified as handler type");
            if (_handlerType.GetTypeInfo().GenericTypeParameters.Length != 1)
                throw new ArgumentException("An open generic should be specified that has only one type argument");
            if (!_handlerType.InterfaceIsSuitableAsHandlerType())
                throw new ArgumentException("Type should contain a single method with one argument and void return");
        }
    }
}