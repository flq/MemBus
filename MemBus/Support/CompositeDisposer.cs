using System;
using System.Collections.Generic;

namespace MemBus.Support
{
    public class CompositeDisposer : IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private bool disposed;

        public CompositeDisposer(IEnumerable<IDisposable> disposables)
        {
            this.disposables.AddRange(disposables);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposables.ForEach(d=>d.Dispose());
            disposed = true;
        }
    }
}