using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Membus.WpfTwitterClient.Frame.UI;
using System.Linq;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    public class AttentionViewModel : IDisposable
    {
        private readonly IDisposable streamDispose;

        public AttentionViewModel(IObservable<RequestForAttention> attentionStream)
        {
            CurrentAttentions = new ObservableCollection<object>();
            streamDispose = attentionStream.ObserveOnDispatcher().Subscribe(onAttentionRequested);
        }

        public ObservableCollection<object> CurrentAttentions { get; private set; }

        private void onAttentionRequested(RequestForAttention attention)
        {
            CurrentAttentions.Add(attention.ViewModel);
        }

        public void Dispose()
        {
            streamDispose.Dispose();
        }
    }
}