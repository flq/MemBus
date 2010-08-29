using System;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Support
{
    /// <summary>
    /// Helps to dispose several instances at once. You can pass in any object, the container will seed
    /// out automatically what can be disposed and what not.
    /// </summary>
    public class DisposeContainer : IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private bool disposed;

        public DisposeContainer(params object[] disposables) : this(disposables.AsEnumerable())
        {    
        }

        public DisposeContainer(IEnumerable<object> disposables)
        {
            this.disposables.AddRange(from d in disposables let realD = d as IDisposable where realD != null select realD);
        }

        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposables.ForEach(d=>d.Dispose());
            disposables.Clear();
            disposed = true;
        }
    }
}