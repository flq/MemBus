using System.Collections.Generic;
using System.Reflection;

namespace MemBus.Subscribing
{
    internal interface IMethodInfoScanner
    {
        IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt);
    }
}