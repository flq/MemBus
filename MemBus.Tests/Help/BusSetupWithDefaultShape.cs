using System.Text;
using MemBus.Setup;

namespace MemBus.Tests.Help
{
    public class BusSetupWithDefaultShape : ISetup<IConfigurableBus>
    {
        private readonly StringBuilder sb;

        public BusSetupWithDefaultShape(StringBuilder sb)
        {
            this.sb = sb;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigureSubscribing(
                s =>
                    {
                        s.DefaultShapeOutwards(new TestShaper("Bar", () => sb.Append("Bar")));
                        s.MessageMatch(mi => mi.IsType<MessageB>(),
                                       sc => sc.ShapeOutwards(new TestShaper("Foo", () => sb.Append("Foo"))));
                    });
        }
    }
}