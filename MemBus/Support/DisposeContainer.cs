using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Support
{
    /// <summary>
    /// Helps to dispose several instances at once. You can pass in any object, the container will seed
    /// out automatically what can be disposed and what not.
    /// </summary>
    public class DisposeContainer : IDisposable, IEnumerable<IDisposable>
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private readonly object disposeLock = new object();
        private volatile bool disposed;

        public DisposeContainer(params object[] disposables) : this(disposables.AsEnumerable())
        {    
        }

        public DisposeContainer(IEnumerable<object> disposables)
        {
            this.disposables.AddRange(disposables.OfType<IDisposable>());
        }

        [Api]
        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        [Api]
        public void Add(params IDisposable[] disposables)
        {
            this.disposables.AddRange(disposables);
        }

        public void Dispose()
        {
            if (disposed) return;
            lock (disposeLock)
            {
                if (disposed) return;
                disposables.Each(d => d.Dispose());
                disposables.Clear();
                disposed = true;
            }
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}