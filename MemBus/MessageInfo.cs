using System;

namespace MemBus
{
    public class MessageInfo
    {
        private readonly object message;

        public MessageInfo(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            this.message = message;
        }

        public bool IsType<T>()
        {
            return message.GetType().Equals(typeof (T));
        }

        public bool IsType<T>(Func<T,bool> additionalMatch)
        {
            if (additionalMatch == null) throw new ArgumentNullException("additionalMatch");
            if (IsType<T>())
                return additionalMatch((T) message);
            return false;
        }

        public string Name { get { return message.GetType().Name; } }

    }
}