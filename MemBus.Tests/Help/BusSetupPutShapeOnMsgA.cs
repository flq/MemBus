using System.Text;
using MemBus.Setup;

namespace MemBus.Tests.Help
{
    public class BusSetupPutShapeOnMsgA : ISetup<IConfigurableBus>
    {
        private readonly TestShaper shaper;


        public BusSetupPutShapeOnMsgA(TestShaper shaper)
        {
            this.shaper = shaper;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.AddService(new StringBuilder());
            setup.ConfigureSubscribing(
                s => s.MessageMatch(mi => mi.IsType<MessageA>(), c => c.ShapeOutwards(shaper)));
        }
    }
}