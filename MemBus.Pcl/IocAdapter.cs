using System;
using System.Collections.Generic;

namespace MemBus
{
    /// <summary>
    /// Implement to be used in conjunction with <see cref="IoCSupport"/>. This allows you to inject your container of choice into
    /// the resolution of subscriptions
    /// </summary>
    public interface IocAdapter
    {
        /// <summary>
        /// Return all instances that implement the desired type
        /// </summary>
        IEnumerable<object> GetAllInstances(Type desiredType);
    }
}