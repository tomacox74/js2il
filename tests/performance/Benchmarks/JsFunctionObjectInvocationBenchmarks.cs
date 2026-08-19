using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using JavaScriptRuntime;

namespace Benchmarks;

/// <summary>
/// Compares the compiled function-object ABI with the intentional built-in
/// delegate adapter boundary.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class JsFunctionObjectInvocationBenchmarks
{
    private static readonly object Argument0 = new();
    private static readonly object Argument1 = new();
    private static readonly object Argument2 = new();
    private static readonly object?[] ArbitraryArguments = [Argument0, Argument1, Argument2];
    private static readonly object[] LexicalSuperScopes = [new object()];
    private static readonly JsCallArguments PackedArguments =
        JsCallArguments.From(Argument0, Argument1, Argument2);

    private readonly BenchmarkFunction _functionObject = new();
    private readonly ContextualBenchmarkFunction _contextualFunctionObject = new();
    private readonly JsFuncNoScopes3 _builtinDelegate =
        static (_, _, _, argument2) => argument2;
    private readonly BuiltinFunction3 _receiverAwareBuiltin =
        static (_, _, _, argument2) => argument2;
    private BuiltinDelegateFunctionAdapter _builtinAdapter = null!;
    private BuiltinDelegateFunctionAdapter _receiverAwareBuiltinAdapter = null!;

    [GlobalSetup]
    public void Setup()
    {
        Function.InitializeFunctionInstance(
            _builtinDelegate,
            3d,
            "builtin",
            requiresInvocationContext: false);
        _builtinAdapter =
            BuiltinDelegateFunctionAdapter.FromDelegate(_builtinDelegate);
        _receiverAwareBuiltinAdapter =
            BuiltinDelegateFunctionAdapter.FromDelegate(_receiverAwareBuiltin);
    }

    [Benchmark(Description = "Receiver-aware built-in adapter fixed arity 3")]
    public object ReceiverAwareBuiltinAdapterFixed3()
        => CallableOperations.Call3(
            _receiverAwareBuiltinAdapter,
            Argument0,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Baseline = true, Description = "Built-in delegate adapter fixed arity 3")]
    public object BuiltinDelegateAdapterFixed3()
        => CallableOperations.Call3(
            _builtinAdapter,
            Argument0,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Description = "JsFunctionObject fixed arity 3")]
    public object FunctionObjectFixed3()
        => CallableOperations.Call3(
            _functionObject,
            null,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Description = "JsFunctionObject pre-materialized arbitrary args")]
    public object FunctionObjectArbitrary()
        => CallableOperations.Call(_functionObject, null, ArbitraryArguments)!;

    [Benchmark(Description = "JsFunctionObject ambient-context fixed arity 3")]
    public object FunctionObjectAmbientContextFixed3()
        => CallableOperations.Call3(
            _contextualFunctionObject,
            null,
            Argument0,
            Argument1,
            Argument2)!;

    [Benchmark(Description = "Thread-static compatibility frame prototype")]
    public object ThreadStaticCompatibilityFramePrototype()
    {
        var previousDepth =
            ThreadStaticInvocationFramePrototype.Push(
                Argument0,
                Argument1,
                LexicalSuperScopes,
                currentArguments: null,
                currentCallArguments: PackedArguments,
                callee: _contextualFunctionObject,
                newTarget: null);
        try
        {
            return ThreadStaticInvocationFramePrototype.CurrentCallee!;
        }
        finally
        {
            ThreadStaticInvocationFramePrototype.Pop(previousDepth);
        }
    }

    [Benchmark(Description = "JsFunctionObject spread materialization")]
    public object FunctionObjectSpreadMaterialization()
        => CallableOperations.Call(
            _functionObject,
            null,
            new object?[] { Argument0, Argument1, Argument2 })!;

    private sealed class BenchmarkFunction : JsFunctionObject
    {
        public override bool RequiresInvocationContext => false;

        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => arguments.GetArgument(2);
    }

    private sealed class ContextualBenchmarkFunction : JsFunctionObject
    {
        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => arguments.GetArgument(2);
    }

    private static class ThreadStaticInvocationFramePrototype
    {
        [ThreadStatic]
        private static FrameStack? _stack;

        internal static object? CurrentCallee
            => _stack?.Current.Callee;

        internal static int Push(
            object? currentThis,
            object? lexicalSuperReceiver,
            object[] lexicalSuperScopes,
            object?[]? currentArguments,
            in JsCallArguments currentCallArguments,
            object? callee,
            object? newTarget)
            => (_stack ??= new FrameStack()).Push(
                currentThis,
                lexicalSuperReceiver,
                lexicalSuperScopes,
                currentArguments,
                currentCallArguments,
                callee,
                newTarget);

        internal static void Pop(int previousDepth)
            => _stack!.Pop(previousDepth);

        private sealed class FrameStack
        {
            private PrototypeFrame[] _frames = new PrototypeFrame[8];
            private int _depth;

            internal ref readonly PrototypeFrame Current
                => ref _frames[_depth];

            internal int Push(
                object? currentThis,
                object? lexicalSuperReceiver,
                object[] lexicalSuperScopes,
                object?[]? currentArguments,
                in JsCallArguments currentCallArguments,
                object? callee,
                object? newTarget)
            {
                var previousDepth = _depth;
                var nextDepth = previousDepth + 1;
                if (nextDepth == _frames.Length)
                {
                    System.Array.Resize(
                        ref _frames,
                        _frames.Length * 2);
                }

                _frames[nextDepth] = new PrototypeFrame(
                    currentThis,
                    lexicalSuperReceiver,
                    lexicalSuperScopes,
                    currentArguments,
                    currentCallArguments,
                    callee,
                    newTarget);
                _depth = nextDepth;
                return previousDepth;
            }

            internal void Pop(int previousDepth)
            {
                _frames[_depth] = default;
                _depth = previousDepth;
            }
        }

        private readonly record struct PrototypeFrame(
            object? CurrentThis,
            object? LexicalSuperReceiver,
            object[]? LexicalSuperScopes,
            object?[]? CurrentArguments,
            JsCallArguments? CurrentCallArguments,
            object? Callee,
            object? NewTarget);
    }
}
