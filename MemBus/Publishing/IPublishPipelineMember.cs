using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Publishing
{
    /// <summary>
    /// Defines a single member of a publishing pipeline.
    /// If you introduce a new publish pipeline member,
    /// you can additionally implement <see cref="IRequireBus" />
    /// to get access to the <see cref="IBus" /> instance.
    /// </summary>
    public interface IPublishPipelineMember
    {
        void LookAt(PublishToken token);
    }

    /// <summary>
    /// Defines a member of a publishing pipeline
    /// that can be awaited
    /// </summary>
    public interface IAsyncPublishPipelineMember
    {
        /// <summary>
        /// Inspect the publish token
        /// </summary>
        Task LookAtAsync(PublishToken token);
    }

}