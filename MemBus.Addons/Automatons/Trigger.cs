using System;

namespace MemBus.Addons.Automatons
{
    public abstract class Trigger
    {
        public event EventHandler Fire;

        /// <summary>
        /// kick off the <see cref="Fire"/> event. If you pass an object into the optional parameter sender
        /// it will be used as sender for the event
        /// </summary>
        protected void fire(object sender = null)
        {
            if (Fire != null)
                Fire(sender ?? this, EventArgs.Empty);
        }
    }
}