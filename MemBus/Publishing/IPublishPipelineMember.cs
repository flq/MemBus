using System.Threading.Tasks;

namespace MemBus.Publishing
{
    /// <summary>
    /// Defines a single member of a publishing pipeline
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