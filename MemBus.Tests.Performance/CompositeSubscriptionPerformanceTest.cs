using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MemBus.Support;

namespace MemBus.Tests.Performance
{
    public class CompositeSubscriptionPerformanceTest : IScenario
    {
        private static readonly Random R = new Random();
        private const int DisposalProbability = 7000;
        private const int NumberOfStartingSubscriptions = 1500;
        private const int NumberOfMessagesSent = 5000;

        private readonly Dictionary<int, Func<ISubscription>> _subBuilders = new Dictionary<int, Func<ISubscription>>
        {
            {1, () => new ASubOf<MessageA>()},
            {2, () => new ASubOf<MessageB>()},
            {3, () => new ASubOf<MessageC>()}
        };
        private readonly Dictionary<int, Func<object>> _messageBuilders = new Dictionary<int, Func<object>>
        {
            {0, () => new MessageA()},
            {1, () => new MessageB()},
            {2, () => new MessageC()}
        };

        private CompositeSubscription _cSub;
        private ISubscriptionResolver _cRes;
        private bool _alreadyRun;
        private static int _disposeCount;

        public void Reset()
        {
            _alreadyRun = true;
            _disposeCount = 0;
        }

        public void Run(IBus bus, TextWriter writer)
        {
            if(!_alreadyRun)
                writer.WriteLine("Disposalprob - 1 in {0}, Starting subs {1}, Messages sent: {2}", DisposalProbability, NumberOfStartingSubscriptions, NumberOfMessagesSent);
            SetupCompositeSubscription(writer);

            var threadCounter1 = 0;
            var threadCounter2 = 0;

            var t1 = new Timer(_ =>
            {
                threadCounter1++;
                AddSubscription();
            }, null, 0, 1);

            var t2 = new Timer(_ =>
            {
                threadCounter2++;
                AddSubscription();
            }, null, 0, 1);

            PumpMessages();

            if (!_alreadyRun)
                ListSubscriptions(writer, _cSub);
            t1.Dispose();
            t2.Dispose();
            writer.WriteLine("ThreadCounter1: " + threadCounter1 + ", ThreadCounter2: " + threadCounter2 + ", DisposeCounter: " + _disposeCount);
        }


        private void SetupCompositeSubscription(TextWriter writer)
        {
            var subs = Enumerable.Repeat(1, NumberOfStartingSubscriptions).Select(_ => NextSubBuilderIndex).Select(i => _subBuilders[i]()).ToList();

            if (!_alreadyRun)
                ListSubscriptions(writer, subs);

            _cSub = new CompositeSubscription(subs);
            _cRes = _cSub;
        }

        private void AddSubscription()
        {
            _cSub.Add(_subBuilders[NextSubBuilderIndex]());
        }

        private void PumpMessages()
        {
            for (var i = 0; i < NumberOfMessagesSent; i++)
            {
                var msg = _messageBuilders[i%3]();
                var subs = _cRes.GetSubscriptionsFor(msg);
                foreach(var s in subs)
                    s.Push(msg);
            }
        }

        private static void ListSubscriptions(TextWriter writer, IEnumerable<ISubscription> subs)
        {
            var groupCount = subs.GroupBy(sub => sub.GetType())
                .Select(grp => new {Type = grp.Key.GetGenericArguments()[0].Name, Count = grp.Count()})
                .ToList();
            foreach (var g in groupCount)
                writer.WriteLine("Got " + g.Count + " subscriptions for " + g.Type);
        }

        private static int NextSubBuilderIndex
        {
            get { return R.Next(1, 4); }
        }

        private static bool ShouldDispose()
        {
            return R.Next(DisposalProbability) == 1;
        }

        public class ASubOf<T> : IDisposableSubscription, IDisposable
        {
            public void Push(object message)
            {
                if (ShouldDispose())
                {
                    _disposeCount++;
                    Disposed.Raise(this);
                }
            }

            public bool Handles(Type messageType)
            {
                return typeof (T).IsAssignableFrom(messageType);
            }

            public IDisposable GetDisposer()
            {
                return this;
            }

            public bool IsDisposed { get; private set; }
            public event EventHandler Disposed;

            void IDisposable.Dispose()
            {
                IsDisposed = true;
            }
        }

        private class MessageA { }
        private class MessageB { }
        private class MessageC { }

    }
}