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

        private string verifierOverride;
        public string VerifierOverride
        {
            get { return verifierOverride; }
            set
            {
                verifierOverride = value;
                //NotifyOfPropertyChange(()=>VerifierOverride);
            }
        }
    }
}