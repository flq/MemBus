using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using System.Linq;

namespace Membus.WpfTwitterClient.Frame
{
    public class ServiceLocator : ServiceLocatorImplBase
    {
        private readonly Func<IContainer> containerLookup;

        private IContainer container
        {
            get { return containerLookup(); }
        }

        public ServiceLocator(Func<IContainer> containerLookup)
        {
            this.containerLookup = containerLookup;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return string.IsNullOrEmpty(key) ? container.GetInstance(serviceType) : container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return container.GetAllInstances(serviceType).OfType<object>();
        }
    }
}