using Caliburn.Micro;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class VerifyPinViewModel : PropertyChangedBase
    {
        private readonly string possibleVerifier;

        public VerifyPinViewModel(string possibleVerifier)
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
                NotifyOfPropertyChange(()=>VerifierOverride);
            }
        }
    }
}