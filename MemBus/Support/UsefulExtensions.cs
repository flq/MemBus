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
        public static void PublishExceptionMessages(this Task[] tasks, IBus bus)
        {
            for (int i = 0; i < tasks.Length; i++)
                if (tasks[i].IsFaulted)
                    bus.Publish(new ExceptionOccurred(tasks[i].Exception));
        }

        /// <summary>
        /// Perform an action with every item of an enumerable
        /// </summary>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var i in items)
                action(i);
        }

        public static string Fmt(this string @string, params object[] args)
        {
            if (@string == null) throw new ArgumentNullException("string");
            return string.Format(@string, args);
        }

        public static bool CheckDenyOrAllIsGood(this object obj)
        {
            return obj is IDenyShaper ? ((IDenyShaper)obj).Deny : false;
        }

        public static IDisposable TryReturnDisposerOfSubscription(this ISubscription sub)
        {
            return sub is IDisposableSubscription ? ((IDisposableSubscription)sub).GetDisposer() : null;
        }

    }
}