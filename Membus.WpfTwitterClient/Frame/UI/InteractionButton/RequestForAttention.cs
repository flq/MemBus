namespace Membus.WpfTwitterClient.Frame.UI
{
    public class RequestForAttention
    {
        private readonly AbstractInteractionButtonViewModel viewModel;

        public RequestForAttention(AbstractInteractionButtonViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public AbstractInteractionButtonViewModel ViewModel
        {
            get { return viewModel; }
        }
    }
}