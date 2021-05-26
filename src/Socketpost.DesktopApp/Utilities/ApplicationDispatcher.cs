using Avalonia.Threading;
using System;

namespace Socketpost.DesktopApp.Utilities
{
    /// <inheritdoc/>
    internal class ApplicationDispatcher : Socketpost.Utilities.IDispatcher
    {
        /// <inheritdoc/>
        public void Dispatch(Action method, params object[] args)
        {
            Dispatcher.UIThread.InvokeAsync(method);
        }
    }
}
