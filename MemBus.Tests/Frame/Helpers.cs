using System;
using MemBus.Support;
using Moq;
using Moq.Protected;

namespace MemBus.Tests.Frame
{
    public class Helpers
    {
        public static Mock<ISubscription> MockSubscriptionThatHandles<T>()
        {
            var mock = new Mock<ISubscription>();
            mock.Setup(m => m.Handles).Returns(typeof (T));
            return mock;
        }
    }
}