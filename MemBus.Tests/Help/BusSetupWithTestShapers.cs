using System.Text;
using MemBus.Setup;

namespace MemBus.Tests.Help
{
    public class BusSetupWithTestShapers : ISetup<IConfigurableBus>
    {
        private readonly StringBuilder sb;

        public BusSetupWithTestShapers(StringBuilder sb)
        {
            this.sb = sb;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigureSubscribing(
                s => s.MessageMatch(mi => mi.IsType<MessageB>(),
                                    sc => sc.ShapeOutwards(
                                        new TestShaper("B", () => sb.Append("B")),
                                        new TestShaper("A", () => sb.Append("A"))
                                              )));
        }
    }
}