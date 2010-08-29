using System;
using MemBus.Configurators;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests
{
    [TestFixture]
    public class Documentation_tests
    {
        [Test]
        public void the_simplest_possible()
        {
            var bus = BusSetup.StartWith<Conservative>().Construct();
            using (bus.Subscribe<MessageA>(Console.WriteLine))
              bus.Publish(new MessageA());
        }

        public void setup_for_a_client()
        {
            var bus = BusSetup.StartWith<AsyncRichClientFrontend, AdditionalSetup>().Construct();

        }

        class AdditionalSetup : ISetup<IConfigurableBus>
        {
            public void Accept(IConfigurableBus setup)
            {
                setup.ConfigurePublishing(
                    p =>
                        {
                            p.MessageMatch(
                                m => m.IsType<Transport>(),
                                l => l.PublishPipeline(new SequentialPublisher())
                                );
                            p.MessageMatch(
                                m => m.Name.EndsWith("Request"),
                                l => l.PublishPipeline(Publish.This(new Transport { On = true}), new ParallelNonBlockingPublisher())
                                );
                            p.MessageMatch(
                                m => m.Name.EndsWith("Response"),
                                l => l.PublishPipeline(new ParallelBlockingPublisher(), Publish.This(new Transport { On = false}))
                                );
                        });
                setup.ConfigureSubscribing(s => s.MessageMatch(
                    m => m.Name.EndsWith("Response") || m.IsType<Transport>(),
                    c => c.ShapeOutwards(new ShapeToUiDispatch(), new ShapeToDispose())));
            }
        }
    }
}