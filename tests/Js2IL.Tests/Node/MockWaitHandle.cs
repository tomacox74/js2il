using System;
using JavaScriptRuntime.EngineCore;

namespace Js2IL.Tests.Node
{
    public class MockWaitHandle : IWaitHandle
    {
        private readonly Action _onSet;
        private readonly Action<int> _onWaitOne;

        public MockWaitHandle(Action onSet, Action<int> onWaitOne)
        {
            _onSet = onSet;
            _onWaitOne = onWaitOne;
        }

        public void Set()
        {
            _onSet?.Invoke();
        }


        public void WaitOne(int millisecondsTimeout)
        {
            _onWaitOne?.Invoke(millisecondsTimeout);
        }
    }
}
