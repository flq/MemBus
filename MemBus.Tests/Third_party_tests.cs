using System.IO;
using NUnit.Framework;
using System.Linq;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{

    [TestFixture]
    public class Third_party_tests
    {

        [Test]
        public void multiple_enumerations_of_method_infos_result_in_same_sequence()
        {
            var l1 = typeof (FileInfo).GetMethods();
            var l2 = typeof(FileInfo).GetMethods();
            l1.SequenceEqual(l2).ShouldBeTrue("The two method info arrays are not comparable");
        }
        
    }

}