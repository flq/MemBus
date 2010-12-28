using System;
using System.Collections.Generic;
using MemBus;
using StructureMap;
using System.Linq;

namespace Membus.WpfTwitterClient.Frame
{
    public class ServiceLocator : IocAdapter
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


        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            return container.GetAllInstances(desiredType).OfType<object>();
        }
    }
}