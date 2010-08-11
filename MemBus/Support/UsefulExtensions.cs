using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemBus.Messages;
using MemBus.Subscribing;

namespace MemBus.Support
{
    public static class UsefulExtensions
    {
        public static IEnumerable<ExceptionOccurred> ConvertToExceptionMessages(this IEnumerable<Task> tasks)
        {
            return tasks
                .Where(t => t.Exception != null)
                .Select(t => new ExceptionOccurred(t.Exception));
        }

        /// <summary>
        /// Perform an action with every item of an enumerable
        /// </summary>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var i in items)
                action(i);
        }

        public static bool CheckDenyOrAllIsGood(this object obj)
        {
            return obj is IDenyShaper ? ((IDenyShaper)obj).Deny : false;
        }

    }
}