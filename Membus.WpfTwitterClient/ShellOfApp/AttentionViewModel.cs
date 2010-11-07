using System;
using System.Collections.ObjectModel;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    public class AttentionViewModel : IDisposable
    {
        private readonly IDisposable streamDispose;

        public AttentionViewModel(StreamOfAttentions attentionStream)
        {
            CurrentAttentions = new ObservableCollection<object>();
            streamDispose = attentionStream.Subscribe(onAttentionRequested);
        }

        public ObservableCollection<object> CurrentAttentions { get; private set; }

        private void onAttentionRequested(RequestForAttention attention)
        {
            var viewModel = attention.ViewModel;
            viewModel.CloseRequested += (s, e) => CurrentAttentions.Remove(viewModel);
            CurrentAttentions.Add(viewModel);
        }

        public void Dispose()
        {
            streamDispose.Dispose();
        }
    }
}