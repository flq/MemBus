namespace Membus.WpfTwitterClient.Frame.UI
{
    public class RequestForAttention
    {
        private readonly object viewModel;

        public RequestForAttention(object viewModel)
        {
            this.viewModel = viewModel;
        }

        public object ViewModel
        {
            get { return viewModel; }
        }
    }
}