using System;

namespace Socketpost.Utilities
{
    /// <summary>
    /// Provides services for managing the queue of work items for a thread
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Executes the specified action synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="method">A delegate to invoke through the dispatcher</param>
        /// <param name="args">The data to provide</param>
        void Dispatch(Action method, params object[] args);
    }
}
