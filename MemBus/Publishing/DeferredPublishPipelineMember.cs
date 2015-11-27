using System;

namespace MemBus.Publishing
{
    /// <summary>
    /// Use this class if you cannot instantiate the actual pipeline member at the time you setup the pipeline
    /// Provide a factory method to a pipeline member that will be called and used once this member is called
    /// during publish processing
    /// </summary>
    /// <typeparam name="T">Any class implementing <see cref="IPublishPipelineMember"/></typeparam>
    public class DeferredPublishPipelineMember<T> : IPublishPipelineMember where T : IPublishPipelineMember
    {
        private readonly Func<T> _actualPipelineMember;

        /// <summary>
        /// ctor with the factory method to obtain the "real" <see cref="IPublishPipelineMember"/>
        /// </summary>
        /// <param name="actualPipelineMember"></param>
        public DeferredPublishPipelineMember(Func<T> actualPipelineMember)
        {
            _actualPipelineMember = actualPipelineMember;
        }

        void IPublishPipelineMember.LookAt(PublishToken token)
        {
            var member = _actualPipelineMember();
            member.LookAt(token);
        }
    }
}