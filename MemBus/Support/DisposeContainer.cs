using System;
using System.Collections;
using System.Collections.Generic;

namespace MemBus.Support
{
    /// <summary>
    /// Helps to dispose several instances at once. You can pass in any object, the container will seed
    /// out automatically what can be disposed and what not.
    /// </summary>
    public class DisposeContainer : IDisposable, IEnumerable<IDisposable>
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly object _disposeLock = new object();
        private volatile bool _disposed;

        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (_disposed) return;
            lock (_disposeLock)
            {
                if (_disposed) return;
                foreach (var d in _disposables)
                    d.Dispose();
                _disposables.Clear();
                _disposed = true;
            }
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}