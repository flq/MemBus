using System;

namespace MemBus.Support
{
    /// <summary>
    /// An automaton is something that should usually publish messages inside the bus' infrastructure 
    /// </summary>
    /// <typeparam name="M">The type of the message that will be sent by this automaton</typeparam>
    public abstract class AbstractAutomaton<M>
    {
        private IBus bus;
        private readonly Trigger trigger;

        protected AbstractAutomaton(Func<Trigger> triggerCreator)
        {
            trigger = triggerCreator();
            if (trigger == null)
                throw new InvalidAutomatonException(string.Format("No trigger was defined for {0} automaton", typeof (M).Name));
            trigger.Fire += onTriggerFire;
        }

        private void onTriggerFire(object sender, EventArgs e)
        {
            M m = constructMessage(sender);
            bus.Publish(m);
        }

        protected abstract M constructMessage(object sender);

        public void AcceptBus(IBus bus)
        {
            this.bus = bus;
        }
    }
}