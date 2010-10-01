using Caliburn.Micro;

namespace Membus.WpfTwitterClient
{
    public class ShellViewModel : PropertyChangedBase
    {
        private string test = "bla";

        public string Test
        {
            get
            {
                return test;
            }
            set
            {
                test = value;
                NotifyOfPropertyChange(() => Test);
            }
        }
    }
}