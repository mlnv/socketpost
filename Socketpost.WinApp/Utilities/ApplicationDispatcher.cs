using Socketpost.Utilities;
using System;
using System.Windows.Threading;

namespace Socketpost.WinApp.Utilities
{
    /// <inheritdoc/>
    internal class ApplicationDispatcher : IDispatcher
    {
        private Dispatcher UnderlyingDispatcher
        {
            get
            {
                if (App.Current == null)
                {
                    throw new InvalidOperationException("You must call this method from within a running WPF application!");
                }

                if (App.Current.Dispatcher == null)
                {
                    throw new InvalidOperationException("You must call this method from within a running WPF application with an active dispatcher!");
                }

                return App.Current.Dispatcher;
            }
        }

        /// <inheritdoc/>
        public void Dispatch(Action method, params object[] args)
        {
            UnderlyingDispatcher.BeginInvoke(method, args);
        }
    }
}
