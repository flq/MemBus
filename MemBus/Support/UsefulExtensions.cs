using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemBus.Messages;

namespace MemBus.Support
{
    public static class UsefulExtensions
    {
        public static IEnumerable<ExceptionOccurred> ConvertToExceptionMessages(this IEnumerable<Task> tasks)
        {
            return tasks
                .Where(t => t.Exception != null)
                .Select(t => new ExceptionOccurred(t.Exception));
        }

    }
}