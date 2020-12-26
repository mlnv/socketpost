using Socketpost.Utilities;
using System;

namespace Socketpost.WinApp.Tests
{
    internal class MockDispatcher : IDispatcher
    {
        public void Dispatch(Action method, params object[] args)
        {
            method.DynamicInvoke(args);
        }
    }
}
