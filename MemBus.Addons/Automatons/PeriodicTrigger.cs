using System;
using System.Threading;

namespace MemBus.Addons.Automatons
{
    public class PeriodicTrigger : Trigger
    {
        private readonly Timer t;

        private void onTimer(object state)
        {
            fire();
        }

        public PeriodicTrigger(TimeSpan initialWait, TimeSpan period)
        {
            t = new Timer(onTimer);
            t.Change(initialWait, period);
        }
    }
}