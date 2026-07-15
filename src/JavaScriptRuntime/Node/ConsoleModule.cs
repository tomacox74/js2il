using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("console")]
    public sealed class ConsoleModule
    {
        public Type Console => typeof(JavaScriptRuntime.Console);
    }
}
