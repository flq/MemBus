using Caliburn.Micro;
using MemBus;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class VerifyPin : AbstractInteractionButtonViewModel
    {
        private readonly string possibleVerifier;
        private readonly IBus bus;

        public VerifyPin(string possibleVerifier, IBus bus)
        {
            this.possibleVerifier = possibleVerifier;
            this.bus = bus;
        }

        public string PossibleVerifier
        {
            get { return possibleVerifier; }
        }

        public string VerifierOverride { get; set; }

        public void CaptureIsGood()
        {
            bus.Publish(new RequestToGetAccessToken(PossibleVerifier));
            Stop();
        }

        public void UseUserPin()
        {
            bus.Publish(new RequestToGetAccessToken(VerifierOverride));
            Stop();
        }
    }
}