using Caliburn.Micro;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class VerifyPin : AbstractInteractionButtonViewModel
    {
        private readonly string possibleVerifier;

        public VerifyPin(string possibleVerifier)
        {
            this.possibleVerifier = possibleVerifier;
        }

        public string PossibleVerifier
        {
            get { return possibleVerifier; }
        }

        public string VerifierOverride { get; set; }

        public void CaptureIsGood()
        {
            
        }

        public void UseUserPin()
        {
            
        }
    }
}