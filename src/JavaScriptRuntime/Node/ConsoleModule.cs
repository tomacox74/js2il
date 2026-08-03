using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("console")]
    public sealed partial class ConsoleModule
    {
        private readonly global::JavaScriptRuntime.Console _console;

        public ConsoleModule(ConsoleOutputSinks consoleOutputSinks)
        {
            _console = new global::JavaScriptRuntime.Console(consoleOutputSinks);
        }

        public ConsoleModule()
            : this(new ConsoleOutputSinks())
        {
        }

        public Type Console => typeof(JavaScriptRuntime.Console);

        public object? debug(params object?[] data) => _console.log(data);

        public object? error(params object?[] data) => _console.error(data);

        public object? info(params object?[] data) => _console.log(data);

        public object? log(params object?[] data) => _console.log(data);

        public object? table(object? tabularData) => _console.table(tabularData);

        public object? warn(params object?[] data) => _console.warn(data);

        private object? InvokeContractDebug(object? data, object?[] args)
            => _console.log(CombineContractArguments(data, args));

        private object? InvokeContractError() => _console.error();

        private object? InvokeContractError(object? data, object?[] args)
            => _console.error(CombineContractArguments(data, args));

        private object? InvokeContractInfo() => _console.log();

        private object? InvokeContractInfo(object? data, object?[] args)
            => _console.log(CombineContractArguments(data, args));

        private object? InvokeContractLog() => _console.log();

        private object? InvokeContractLog(object? data, object?[] args)
            => _console.log(CombineContractArguments(data, args));

        private object? InvokeContractWarn() => _console.warn();

        private object? InvokeContractWarn(object? data, object?[] args)
            => _console.warn(CombineContractArguments(data, args));

        private static object?[] CombineContractArguments(object? data, object?[] args)
        {
            var combined = new object?[args.Length + 1];
            combined[0] = data;
            global::System.Array.Copy(args, 0, combined, 1, args.Length);
            return combined;
        }
    }
}
