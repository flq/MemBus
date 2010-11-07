using System.Collections.Generic;
using Twitterizer;

namespace Membus.WpfTwitterClient.Timeline
{
    public class NewTwitterStatuses
    {
        public IEnumerable<TwitterStatus> Statuses { get; private set; }

        public NewTwitterStatuses(IEnumerable<TwitterStatus> statuses)
        {
            Statuses = statuses;
        }
    }
}