using System;
using MemBus.Messages;

namespace MemBus.Support
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