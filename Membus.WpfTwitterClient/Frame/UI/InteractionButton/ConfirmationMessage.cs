namespace Membus.WpfTwitterClient.Frame.UI
{
    public class ConfirmationMessage : AbstractInteractionButtonViewModel
    {
        public string Message { get; private set; }

        public ConfirmationMessage(string message)
        {
            Message = message;
        }

        public override void MainPush()
        {
            Stop();
        }
    }
}