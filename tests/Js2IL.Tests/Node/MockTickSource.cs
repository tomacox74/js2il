using System;
using JavaScriptRuntime.EngineCore;

namespace Js2IL.Tests.Node
{
    public class MockTickSource : ITickSource
    {
        private long _ticks;

        public MockTickSource()
        {
            _ticks = DateTime.UtcNow.Ticks;
        }

        public long GetTicks()
        {
            return _ticks;
        }

        public void Increment(TimeSpan span)
        {
            _ticks += span.Ticks;
        }
    }
}
