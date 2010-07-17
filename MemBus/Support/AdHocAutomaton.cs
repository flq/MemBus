using System;

namespace MemBus.Support
{
    public class AdHocAutomaton<M> : AbstractAutomaton<M>
    {
        private readonly Func<Trigger> triggerCreator;
        private readonly Func<M> messageCreator;

        public AdHocAutomaton(Func<Trigger> triggerCreator, Func<M> messageCreator) : base(triggerCreator)
        {
            this.messageCreator = messageCreator;
        }

        protected override M constructMessage(object sender)
        {
            return messageCreator();
        }
    }
}