using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MemBus.Subscribing
{
    internal class SubscriptionBuilder
    {
        private readonly List<IMethodInfoScanner> _scanner = new List<IMethodInfoScanner>();
        private IPublisher _publisher;

        public void AddScanner(IMethodInfoScanner scanner)
        {
            _scanner.Add(scanner);
        }

        public void SetPublisher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            var groups = _scanner
                .SelectMany(s => s.GetMethodInfos(targetToAdapt))
                .Distinct()
                .GroupBy(mi => mi.ReturnType == typeof (void) ? "void" : "non-void")
                .ToDictionary(group => group.Key, group => group.ToList());

            return SafeGroup(groups, "void")
                .ConstructSubscriptions(targetToAdapt)
                .Concat(
                  SafeGroup(groups, "non-void").ConstructPublishingSubscriptions(targetToAdapt, _publisher)
                );
        }

        private static IEnumerable<MethodInfo> SafeGroup(IDictionary<string, List<MethodInfo>> groups, string methodGroup)
        {
            return groups.ContainsKey(methodGroup) ? groups[methodGroup] : Enumerable.Empty<MethodInfo>();
        }
    }
}