using System;
using MemBus.Addons.Messages;

namespace MemBus.Addons.Automatons
{
    public class PeriodAutomaton : AbstractAutomaton<PeriodPassed>
    {
        private readonly string id;

        public PeriodAutomaton(string id, TimeSpan wait, TimeSpan period)
            : base(() => new PeriodicTrigger(wait, period))
        {
            this.id = id;
        }

        protected override PeriodPassed constructMessage(object sender)
        {
            return new PeriodPassed(id);
        }
    }
}