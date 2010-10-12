using System;
using System.Windows;
using System.Windows.Threading;

namespace Membus.WpfTwitterClient.Frame
{
    /// <summary>
    /// Method when you need an action to be invoked on the current dispatcher. Use it where an Action{T} is requested.
    /// </summary>
    public class ActionOnDispatcher<T>
    {
        private readonly Action<T> lambda;

        public ActionOnDispatcher(Action<T> lambda)
        {
            this.lambda = lambda;
        }

        public static implicit operator Action<T>(ActionOnDispatcher<T> actionOnDispatcher)
        {
            return t => Application.Current.Dispatcher.Invoke(actionOnDispatcher.lambda, t);
        }
    }
}