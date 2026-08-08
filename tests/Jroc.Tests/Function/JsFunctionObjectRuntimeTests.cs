using JavaScriptRuntime;

namespace Jroc.Tests.Function;

public sealed class JsFunctionObjectRuntimeTests
{
    [Fact]
    public void FunctionObjects_HaveOrdinaryObjectSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            PrototypeChain.Enable();

            var function = new RecordingFunction();
            var otherFunction = new RecordingFunction();
            var symbol = new Symbol("function-state");

            Assert.NotSame(function, otherFunction);
            Assert.Equal("function", TypeUtilities.Typeof(function));
            Assert.Same(JavaScriptRuntime.Function.Prototype, JavaScriptRuntime.Object.getPrototypeOf(function));

            ObjectRuntime.SetProperty(function, "custom", 42d);
            ObjectRuntime.SetItem(function, symbol, "symbol-value");
            Assert.Equal(42d, ObjectRuntime.GetProperty(function, "custom"));
            Assert.Equal("symbol-value", ObjectRuntime.GetItem(function, symbol));

            var descriptor = new JsObject
            {
                ["value"] = "fixed",
                ["writable"] = false,
                ["enumerable"] = false,
                ["configurable"] = true
            };
            JavaScriptRuntime.Object.defineProperty(function, "fixed", descriptor);

            var actualDescriptor = Assert.IsType<JsObject>(
                JavaScriptRuntime.Object.getOwnPropertyDescriptor(function, "fixed"));
            Assert.Equal("fixed", actualDescriptor["value"]);
            Assert.False(Assert.IsType<bool>(actualDescriptor["writable"]));
            Assert.False(Assert.IsType<bool>(actualDescriptor["enumerable"]));
            Assert.True(Assert.IsType<bool>(actualDescriptor["configurable"]));

            object? setterThis = null;
            object? setterValue = null;
            var getter = new LambdaFunction((thisArgument, _) =>
            {
                Assert.Same(function, thisArgument);
                return "accessor-value";
            });
            var setter = new LambdaFunction((thisArgument, arguments) =>
            {
                setterThis = thisArgument;
                setterValue = arguments.GetArgument(0);
                return null;
            });
            JavaScriptRuntime.Object.defineProperty(function, "accessor", new JsObject
            {
                ["get"] = getter,
                ["set"] = setter,
                ["enumerable"] = true,
                ["configurable"] = true
            });

            Assert.Equal("accessor-value", ObjectRuntime.GetProperty(function, "accessor"));
            ObjectRuntime.SetProperty(function, "accessor", "updated");
            Assert.Same(function, setterThis);
            Assert.Equal("updated", setterValue);

            Assert.True(ObjectRuntime.DeleteProperty(function, "custom"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(function, "custom"));

            JavaScriptRuntime.Object.preventExtensions(function);
            Assert.False(JavaScriptRuntime.Object.isExtensible(function));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(function, "newProperty", 1d));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void CallableOperations_CallRestoresNestedInvocationContext()
    {
        var function = new ReentrantFunction();
        var previousThis = RuntimeServices.SetCurrentThis("original-this");
        var previousArguments = RuntimeServices.SetCurrentArguments(new object?[] { "original-argument" });
        var previousCallee = RuntimeServices.SetCurrentCallee("original-callee");
        var previousNewTarget = RuntimeServices.SetCurrentNewTarget("original-new-target");
        try
        {
            var result = Assert.IsType<NestedCallSnapshot>(
                CallableOperations.Call(function, "outer-this", new object?[] { "outer" }));
            var functionPrototypeResult = Assert.IsType<CallSnapshot>(
                JavaScriptRuntime.Function.Call(
                    function,
                    "prototype-call-this",
                    new object?[] { "prototype-call" }));

            Assert.Equal("inner-this", result.Inner.ThisArgument);
            Assert.Equal("inner", result.Inner.Argument);
            Assert.Same(function, result.Inner.Callee);
            Assert.Null(result.Inner.NewTarget);

            Assert.Equal("outer-this", result.OuterAfterInner.ThisArgument);
            Assert.Equal("outer", result.OuterAfterInner.Argument);
            Assert.Same(function, result.OuterAfterInner.Callee);
            Assert.Null(result.OuterAfterInner.NewTarget);

            Assert.Equal("prototype-call-this", functionPrototypeResult.ThisArgument);
            Assert.Equal("prototype-call", functionPrototypeResult.Argument);

            Assert.Equal("original-this", RuntimeServices.GetCurrentThis());
            Assert.Equal("original-argument", Assert.Single(RuntimeServices.GetCurrentArguments()!));
            Assert.Equal("original-callee", RuntimeServices.GetCurrentCallee());
            Assert.Equal("original-new-target", RuntimeServices.GetCurrentNewTarget());
        }
        finally
        {
            RuntimeServices.SetCurrentNewTarget(previousNewTarget);
            RuntimeServices.SetCurrentCallee(previousCallee);
            RuntimeServices.SetCurrentArguments(previousArguments);
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    [Fact]
    public async Task CallableOperations_ConcurrentCallsKeepReceiverAndArgumentsIsolated()
    {
        using var barrier = new Barrier(2);
        var function = new ConcurrentFunction(barrier);

        var firstTask = Task.Run(() =>
            Assert.IsType<CallSnapshot>(
                CallableOperations.Call(function, "first-this", new object?[] { "first" })));
        var secondTask = Task.Run(() =>
            Assert.IsType<CallSnapshot>(
                CallableOperations.Call(function, "second-this", new object?[] { "second" })));

        var results = await Task.WhenAll(firstTask, secondTask);

        Assert.Contains(results, result =>
            Equals(result.ThisArgument, "first-this") && Equals(result.Argument, "first"));
        Assert.Contains(results, result =>
            Equals(result.ThisArgument, "second-this") && Equals(result.Argument, "second"));
        Assert.All(results, result => Assert.Same(function, result.Callee));
    }

    [Fact]
    public void CallableOperations_ConstructsOnlyConstructableFunctionObjects()
    {
        var constructor = new ConstructableFunction();
        var arrow = new RecordingFunction();
        var explicitNewTarget = new JsObject();

        Assert.True(CallableOperations.IsCallable(constructor));
        Assert.True(CallableOperations.IsConstructor(constructor));
        Assert.False(CallableOperations.IsConstructor(arrow));

        var defaultResult = Assert.IsType<ConstructSnapshot>(
            CallableOperations.Construct(constructor, new object?[] { 1d }));
        Assert.Equal(1d, defaultResult.Argument);
        Assert.Same(constructor, defaultResult.NewTarget);
        Assert.Same(constructor, defaultResult.Callee);

        var explicitResult = Assert.IsType<ConstructSnapshot>(
            CallableOperations.Construct(constructor, new object?[] { 2d }, explicitNewTarget));
        Assert.Equal(2d, explicitResult.Argument);
        Assert.Same(explicitNewTarget, explicitResult.NewTarget);

        var error = Assert.Throws<TypeError>(
            () => CallableOperations.Construct(arrow, System.Array.Empty<object?>()));
        Assert.Equal("Value is not a constructor", error.Message);
    }

    [Fact]
    public void BoundFunctionObjects_UnifyCallsConstructionMetadataAndArguments()
    {
        var target = new ArgumentTransportFunction();
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            target,
            4d,
            "target",
            requiresInvocationContext: true);
        var bound = Assert.IsType<BoundFunctionObject>(
            JavaScriptRuntime.Function.Bind(
                target,
                "bound-this",
                new object?[] { "bound-0", "bound-1" }));

        var call = Assert.IsType<BoundCallSnapshot>(
            CallableOperations.Call(
                bound,
                "ignored-this",
                new object?[] { "call-0", "call-1" }));
        Assert.Equal("bound-this", call.ThisArgument);
        Assert.Equal(
            new[] { "bound-0", "bound-1", "call-0", "call-1" },
            call.Arguments);
        Assert.False(call.UsesArrayStorage);
        Assert.Equal(2d, ObjectRuntime.GetProperty(bound, "length"));
        Assert.Equal("bound target", ObjectRuntime.GetProperty(bound, "name"));
        Assert.False(JavaScriptRuntime.Object.hasOwn(bound, "prototype"));

        var chained = Assert.IsType<BoundFunctionObject>(
            JavaScriptRuntime.Function.Bind(
                bound,
                "ignored-again",
                new object?[] { "chained" }));
        var chainedCall = Assert.IsType<BoundCallSnapshot>(
            CallableOperations.Call(chained, null, new object?[] { "runtime" }));
        Assert.Equal("bound-this", chainedCall.ThisArgument);
        Assert.Equal(
            new[] { "bound-0", "bound-1", "chained", "runtime" },
            chainedCall.Arguments);
        Assert.Equal("bound bound target", ObjectRuntime.GetProperty(chained, "name"));
        Assert.Equal(1d, ObjectRuntime.GetProperty(chained, "length"));

        var constructor = new ConstructableFunction();
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            constructor,
            2d,
            "Constructor",
            requiresInvocationContext: true);
        var boundConstructor = Assert.IsType<BoundFunctionObject>(
            JavaScriptRuntime.Function.Bind(
                constructor,
                "ignored-constructor-this",
                new object?[] { "bound-constructor-argument" }));
        var constructed = Assert.IsType<ConstructSnapshot>(
            CallableOperations.Construct(
                boundConstructor,
                new object?[] { "runtime-constructor-argument" }));
        Assert.Equal("bound-constructor-argument", constructed.Argument);
        Assert.Same(constructor, constructed.NewTarget);
        Assert.True(boundConstructor.IsConstructor);
    }

    [Fact]
    public void BindingLegacyDelegatesProducesExplicitBoundFunctionObjects()
    {
        Func<object[], object?[]?, object?> legacy = static (_, args) =>
            new BoundCallSnapshot(
                RuntimeServices.GetCurrentThis(),
                args ?? System.Array.Empty<object?>(),
                UsesArrayStorage: true);
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            legacy,
            1d,
            "legacy",
            requiresInvocationContext: true);

        var bound = Assert.IsType<BoundFunctionObject>(
            JavaScriptRuntime.Function.Bind(
                legacy,
                "legacy-this",
                new object?[] { "bound" }));
        var result = Assert.IsType<BoundCallSnapshot>(
            CallableOperations.Call(bound, null, new object?[] { "runtime" }));

        Assert.Equal("legacy-this", result.ThisArgument);
        Assert.Equal(new[] { "bound", "runtime" }, result.Arguments);
        Assert.Equal("bound legacy", ObjectRuntime.GetProperty(bound, "name"));
    }

    [Fact]
    public void FixedArityLegacyInvocationPadsMissingHostArguments()
    {
        object?[]? received = null;
        Action<object?, object?, object?> host = (first, second, third) =>
            received = [first, second, third];
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            host,
            3d,
            "host",
            requiresInvocationContext: false);

        Closure.InvokeWithArgs2(
            host,
            RuntimeServices.EmptyScopes,
            "first",
            "second");

        Assert.Equal(
            new object?[] { "first", "second", null },
            received);
    }

    [Fact]
    public void CallableOperations_RejectsNonCallableValuesConsistently()
    {
        var value = new JsObject();

        Assert.False(CallableOperations.IsCallable(value));
        Assert.False(CallableOperations.IsConstructor(value));

        var callError = Assert.Throws<TypeError>(
            () => CallableOperations.Call(value, null, System.Array.Empty<object?>()));
        var constructError = Assert.Throws<TypeError>(
            () => CallableOperations.Construct(value, System.Array.Empty<object?>()));

        Assert.Equal("Value is not callable", callError.Message);
        Assert.Equal("Value is not a constructor", constructError.Message);
    }

    [Fact]
    public void LegacyDelegateAdapterSupportsIncrementalMigration()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            Func<object[], object?[]?, object?> legacyCall = static (_, args) =>
                new CallSnapshot(
                    RuntimeServices.GetCurrentThis(),
                    args![0],
                    RuntimeServices.GetCurrentCallee(),
                    RuntimeServices.GetCurrentNewTarget());
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                legacyCall,
                1d,
                "legacyCall",
                requiresInvocationContext: true);

            var adapter = new LegacyDelegateFunctionAdapter(legacyCall);

            Assert.True(CallableOperations.IsCallable(legacyCall));
            Assert.True(CallableOperations.IsCallable(adapter));
            Assert.Equal("function", TypeUtilities.Typeof(adapter));

            var rawResult = Assert.IsType<CallSnapshot>(
                CallableOperations.Call(legacyCall, "raw-this", new object?[] { "raw" }));
            var adapterResult = Assert.IsType<CallSnapshot>(
                CallableOperations.Call(adapter, "adapter-this", new object?[] { "adapter" }));

            Assert.Equal("raw-this", rawResult.ThisArgument);
            Assert.Equal("raw", rawResult.Argument);
            Assert.Same(legacyCall, rawResult.Callee);

            Assert.Equal("adapter-this", adapterResult.ThisArgument);
            Assert.Equal("adapter", adapterResult.Argument);
            Assert.Same(legacyCall, adapterResult.Callee);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void CallableProxyDispatchesToFunctionObject()
    {
        var target = new RecordingFunction();
        var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());

        Assert.True(CallableOperations.IsCallable(proxy));

        var result = Assert.IsType<CallSnapshot>(
            CallableOperations.Call(proxy, "proxy-this", new object?[] { "proxy-argument" }));

        Assert.Equal("proxy-this", result.ThisArgument);
        Assert.Equal("proxy-argument", result.Argument);
        Assert.Same(target, result.Callee);
    }

    [Fact]
    public void CallableProxyApplyAndConstructTrapsUseCentralizedOperations()
    {
        var target = new ConstructableFunction();
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            target,
            1d,
            "target",
            requiresInvocationContext: true);
        var handler = new JsObject();
        object? applyThis = null;
        object? applyTarget = null;
        handler["apply"] = new LambdaFunction((thisArgument, arguments) =>
        {
            applyThis = thisArgument;
            applyTarget = arguments.GetArgument(0);
            var argumentList = Assert.IsType<JavaScriptRuntime.Array>(
                arguments.GetArgument(2));
            return new CallSnapshot(
                arguments.GetArgument(1),
                argumentList[0],
                RuntimeServices.GetCurrentCallee(),
                RuntimeServices.GetCurrentNewTarget());
        });
        var proxy = new JavaScriptRuntime.Proxy(target, handler);

        var applyResult = Assert.IsType<CallSnapshot>(
            CallableOperations.Call(proxy, "proxy-this", new object?[] { "proxy-argument" }));
        Assert.Same(handler, applyThis);
        Assert.Same(target, applyTarget);
        Assert.Equal("proxy-this", applyResult.ThisArgument);
        Assert.Equal("proxy-argument", applyResult.Argument);

        object? constructThis = null;
        object? constructTarget = null;
        object? constructNewTarget = null;
        var constructed = new JsObject();
        handler["construct"] = new LambdaFunction((thisArgument, arguments) =>
        {
            constructThis = thisArgument;
            constructTarget = arguments.GetArgument(0);
            constructNewTarget = arguments.GetArgument(2);
            var argumentList = Assert.IsType<JavaScriptRuntime.Array>(
                arguments.GetArgument(1));
            constructed["argument"] = argumentList[0];
            return constructed;
        });

        Assert.Same(
            constructed,
            CallableOperations.Construct(proxy, new object?[] { 42d }));
        Assert.Same(handler, constructThis);
        Assert.Same(target, constructTarget);
        Assert.Same(proxy, constructNewTarget);
        Assert.Equal(42d, constructed["argument"]);

        handler["construct"] = new LambdaFunction((_, _) => 1d);
        Assert.Throws<TypeError>(
            () => CallableOperations.Construct(proxy, System.Array.Empty<object?>()));
        handler["apply"] = 1d;
        Assert.Throws<TypeError>(
            () => CallableOperations.Call(proxy, null, System.Array.Empty<object?>()));
    }

    [Fact]
    public void CallableClassificationAndIdentityUseCentralizedOperations()
    {
        var function = new RecordingFunction();
        var callableProxy = new JavaScriptRuntime.Proxy(function, new JsObject());
        var ordinaryProxy = new JavaScriptRuntime.Proxy(new JsObject(), new JsObject());

        Assert.Equal("function", TypeUtilities.Typeof(function));
        Assert.Equal("function", TypeUtilities.Typeof(callableProxy));
        Assert.Equal("object", TypeUtilities.Typeof(ordinaryProxy));
        Assert.True(CallableOperations.IsCallable(function));
        Assert.True(CallableOperations.IsCallable(callableProxy));
        Assert.False(CallableOperations.IsCallable(ordinaryProxy));
        Assert.True(Operators.StrictEqual(function, function));
        Assert.False(Operators.StrictEqual(function, new RecordingFunction()));
    }

    private sealed class RecordingFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => Capture(arguments);
    }

    private sealed class LambdaFunction(Func<object?, JsCallArguments, object?> implementation) : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => implementation(thisArgument, arguments);
    }

    private sealed class ReentrantFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            if (Equals(arguments.GetArgument(0), "outer"))
            {
                var inner = Assert.IsType<CallSnapshot>(
                    CallableOperations.Call(this, "inner-this", new object?[] { "inner" }));
                return new NestedCallSnapshot(inner, Capture(arguments));
            }

            return Capture(arguments);
        }
    }

    private sealed class ConcurrentFunction(Barrier barrier) : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            barrier.SignalAndWait(TimeSpan.FromSeconds(10));
            return Capture(arguments);
        }
    }

    private sealed class ConstructableFunction : JsFunctionObject
    {
        public override bool IsConstructor => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => Capture(arguments);

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
            => new ConstructSnapshot(
                arguments.GetArgument(0),
                newTarget,
                RuntimeServices.GetCurrentCallee());
    }

    private sealed class ArgumentTransportFunction : JsFunctionObject
    {
        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => new BoundCallSnapshot(
                RuntimeServices.GetCurrentThis(),
                arguments.ToArray(),
                arguments.UsesArrayStorage);
    }

    private static CallSnapshot Capture(in JsCallArguments arguments)
        => new(
            RuntimeServices.GetCurrentThis(),
            arguments.GetArgument(0),
            RuntimeServices.GetCurrentCallee(),
            RuntimeServices.GetCurrentNewTarget());

    private sealed record CallSnapshot(
        object? ThisArgument,
        object? Argument,
        object? Callee,
        object? NewTarget);

    private sealed record NestedCallSnapshot(
        CallSnapshot Inner,
        CallSnapshot OuterAfterInner);

    private sealed record ConstructSnapshot(
        object? Argument,
        object? NewTarget,
        object? Callee);

    private sealed record BoundCallSnapshot(
        object? ThisArgument,
        object?[] Arguments,
        bool UsesArrayStorage);
}
