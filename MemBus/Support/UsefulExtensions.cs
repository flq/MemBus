using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemBus.Messages;
using MemBus.Subscribing;

namespace MemBus.Support
{
    /// <summary>
    /// Extensions used internally by Membus
    /// </summary>
    public static class UsefulExtensions
    {
        /// <summary>
        /// Sends off <see cref="ExceptionOccurred"/> messages on a bus based on a list of faulted tasks
        /// </summary>
        public static void PublishExceptionMessages(this Task[] tasks, IBus bus)
        {
            foreach (var t in tasks.Where(t => t.IsFaulted))
                bus.Publish(new ExceptionOccurred(t.Exception));
        }

        /// <summary>
        /// Raise an event
        /// </summary>
        public static void Raise(this EventHandler @event, object sender)
        {
            if (@event != null)
                @event(sender, EventArgs.Empty);
        }

        /// <summary>
        /// A single item as Enumerable
        /// </summary>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            return new[] {item};
        }

        /// <summary>
        /// Perform an action with every item of an enumerable
        /// </summary>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var i in items)
                action(i);
        }

        /// <summary>
        /// string.Format as extension method
        /// </summary>
        public static string Fmt(this string @string, params object[] args)
        {
            if (@string == null) throw new ArgumentNullException("string");
            return string.Format(@string, args);
        }

        /// <summary>
        /// Return the value accessesd by selector or the default(O) if the input is null
        /// </summary>
        public static O IfNotNull<I,O>(this I input, Func<I,O> selector) where I : class
        {
            return input != null ? selector(input) : default(O);
        }

        internal static bool CheckDenyOrAllIsGood(this object obj)
        {
            return obj is IDenyShaper && ((IDenyShaper)obj).Deny;
        }

        internal static IDisposable TryReturnDisposerOfSubscription(this ISubscription sub)
        {
            return sub is IDisposableSubscription ? ((IDisposableSubscription)sub).GetDisposer() : null;
        }

        public static object ExtractTarget(this Delegate action)
        {
            if (action.Target == null)
                return null;
            // Disgusting fact: PClstuff uses the same Closure type, but we cannot reach it...
            if (action.Target.GetType().Name == "Closure")
            {
                dynamic z = action.Target;
                return z.Constants[0];
            }
            return action.Target;
        }



    }
}