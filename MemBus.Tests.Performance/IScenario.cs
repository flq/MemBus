using System.IO;

namespace MemBus.Tests.Performance
{
    public interface IScenario
    {
        void Run(IBus bus, TextWriter writer);
        void Reset();
    }
}