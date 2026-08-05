using System.Reflection;
using System.Reflection.Emit;
using JavaScriptRuntime;

namespace Jroc.Tests.Function;

public sealed class JsCallArgumentsRuntimeTests
{
    [Fact]
    public void FixedArityCallsUseInlineArgumentStorage()
    {
        var function = new InspectArgumentsFunction();

        for (var arity = 0; arity <= 5; arity++)
        {
            var result = Assert.IsType<ArgumentsSnapshot>(arity switch
            {
                0 => CallableOperations.Call0(function, "receiver"),
                1 => CallableOperations.Call1(function, "receiver", "a0"),
                2 => CallableOperations.Call2(function, "receiver", "a0", "a1"),
                3 => CallableOperations.Call3(function, "receiver", "a0", "a1", "a2"),
                4 => CallableOperations.Call4(function, "receiver", "a0", "a1", "a2", "a3"),
                5 => CallableOperations.Call5(function, "receiver", "a0", "a1", "a2", "a3", "a4"),
                _ => throw new InvalidOperationException()
            });

            Assert.Equal(arity, result.Count);
            Assert.False(result.UsesArrayStorage);
            Assert.Equal("receiver", result.ThisArgument);
        }
    }

    [Fact]
    public void ContextFreeAdapterUsesOnlyExplicitInvocationState()
    {
        var function = new ContextFreeFunction();
        var previousThis = RuntimeServices.SetCurrentThis("ambient-this");
        var previousArguments = RuntimeServices.SetCurrentArguments(
            new object?[] { "ambient-argument" });
        try
        {
            var result = Assert.IsType<ContextFreeSnapshot>(
                CallableOperations.Call1(function, "explicit-this", "explicit-argument"));
            var genericBridgeResult = Assert.IsType<ContextFreeSnapshot>(
                Closure.InvokeWithArgs(
                    function,
                    RuntimeServices.EmptyScopes,
                    new object?[] { "generic-argument" }));

            Assert.Equal("explicit-this", result.ThisArgument);
            Assert.Equal("explicit-argument", result.Argument);
            Assert.Equal("ambient-this", genericBridgeResult.ThisArgument);
            Assert.Equal("generic-argument", genericBridgeResult.Argument);
            Assert.Equal("ambient-this", RuntimeServices.GetCurrentThis());
            Assert.Equal(
                "ambient-argument",
                Assert.Single(RuntimeServices.GetCurrentArguments()!));
        }
        finally
        {
            RuntimeServices.SetCurrentArguments(previousArguments);
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    [Fact]
    public void LegacyFixedDispatchersBridgeWithoutMaterializingArguments()
    {
        var function = new InspectArgumentsFunction();
        var previousThis = RuntimeServices.SetCurrentThis("legacy-receiver");
        try
        {
            var one = Assert.IsType<ArgumentsSnapshot>(
                Closure.InvokeWithArgs1(
                    function,
                    RuntimeServices.EmptyScopes,
                    "a0"));
            var three = Assert.IsType<ArgumentsSnapshot>(
                Closure.InvokeWithArgs3(
                    function,
                    RuntimeServices.EmptyScopes,
                    "a0",
                    "a1",
                    "a2"));
            var five = Assert.IsType<ArgumentsSnapshot>(
                Closure.InvokeWithArgs5(
                    function,
                    RuntimeServices.EmptyScopes,
                    "a0",
                    "a1",
                    "a2",
                    "a3",
                    "a4"));

            Assert.Equal(new[] { 1, 3, 5 }, new[] { one.Count, three.Count, five.Count });
            Assert.False(one.UsesArrayStorage);
            Assert.False(three.UsesArrayStorage);
            Assert.False(five.UsesArrayStorage);
            Assert.Equal("legacy-receiver", five.ThisArgument);
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    [Fact]
    public void ArbitraryAndSpreadArgumentsRetainArrayStorage()
    {
        var function = new InspectArgumentsFunction();
        object?[] spreadArguments = ["a0", null, 3d, true, "a4", "a5", "a6"];

        var result = Assert.IsType<ArgumentsSnapshot>(
            CallableOperations.Call(function, "receiver", spreadArguments));

        Assert.Equal(spreadArguments.Length, result.Count);
        Assert.True(result.UsesArrayStorage);
        Assert.Same(spreadArguments, result.MaterializedArguments);
        Assert.Null(result.MissingArgument);
    }

    [Fact]
    public void ArgumentsArrayMaterializesLazilyAndRemainsStableWithinCall()
    {
        var function = new MaterializeArgumentsFunction();

        var result = Assert.IsType<MaterializedArgumentsSnapshot>(
            CallableOperations.Call3(function, null, "first", null, "third"));

        Assert.False(result.UsedArrayStorageBeforeMaterialization);
        Assert.Same(result.FirstRead, result.SecondRead);
        Assert.Equal(new object?[] { "first", null, "third" }, result.FirstRead);
    }

    [Fact]
    public void MissingExtraDefaultAndRestSemanticsUseArgumentCount()
    {
        var function = new DefaultsAndRestFunction();

        var missing = Assert.IsType<DefaultsAndRestSnapshot>(
            CallableOperations.Call0(function, null));
        var extra = Assert.IsType<DefaultsAndRestSnapshot>(
            CallableOperations.Call4(function, null, "value", 1d, 2d, 3d));

        Assert.Equal("default", missing.First);
        Assert.Empty(missing.Rest);
        Assert.Equal("value", extra.First);
        Assert.Equal(new object?[] { 1d, 2d, 3d }, extra.Rest);
    }

    [Fact]
    public void FixedArityConstructionCarriesNewTargetWithoutArrayStorage()
    {
        var constructor = new InspectConstructorFunction();
        var newTarget = new JsObject();

        var result = Assert.IsType<ConstructArgumentsSnapshot>(
            CallableOperations.Construct2(constructor, newTarget, "first", "second"));

        Assert.Equal(2, result.Count);
        Assert.False(result.UsesArrayStorage);
        Assert.Same(newTarget, result.NewTarget);
        Assert.Same(constructor, result.Callee);
    }

    [Fact]
    public void AbruptCompletionRestoresOuterPackedArguments()
    {
        var function = new CatchingFunction();

        var result = Assert.IsType<object?[]>(
            CallableOperations.Call1(function, null, "outer"));

        Assert.Equal(new object?[] { "outer" }, result);
        Assert.Null(RuntimeServices.GetCurrentArguments());
    }

    [Fact]
    public void TypedImplementationRemainsTypedBehindGenericAdapter()
    {
        var function = new TypedAdapterFunction();
        var type = typeof(TypedAdapterFunction);
        var numberMethod = type.GetMethod(nameof(TypedAdapterFunction.InvokeNumber))!;
        var booleanMethod = type.GetMethod(nameof(TypedAdapterFunction.InvokeBoolean))!;
        var stringMethod = type.GetMethod(nameof(TypedAdapterFunction.InvokeString))!;
        var adapterMethod = type.GetMethod(
            "CallCore",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

        Assert.Equal(typeof(double), numberMethod.ReturnType);
        Assert.Equal(typeof(double), Assert.Single(numberMethod.GetParameters()).ParameterType);
        Assert.Equal(typeof(bool), booleanMethod.ReturnType);
        Assert.Equal(typeof(bool), Assert.Single(booleanMethod.GetParameters()).ParameterType);
        Assert.Equal(typeof(string), stringMethod.ReturnType);
        Assert.Equal(typeof(string), Assert.Single(stringMethod.GetParameters()).ParameterType);

        Assert.Equal(typeof(object), adapterMethod.ReturnType);
        var adapterArgumentsParameter = adapterMethod.GetParameters()[1];
        Assert.Equal(typeof(JsCallArguments).MakeByRefType(), adapterArgumentsParameter.ParameterType);
        Assert.True(adapterArgumentsParameter.IsIn);

        Assert.DoesNotContain(
            unchecked((byte)OpCodes.Box.Value),
            numberMethod.GetMethodBody()!.GetILAsByteArray()!);
        Assert.Contains(
            unchecked((byte)OpCodes.Box.Value),
            adapterMethod.GetMethodBody()!.GetILAsByteArray()!);

        Assert.Equal(3d, CallableOperations.Call1(function, null, 2d));
        Assert.Same(function, function.Identity);
        Assert.False(typeof(JsCallArguments).IsByRefLike);
    }

    private sealed class InspectArgumentsFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => new ArgumentsSnapshot(
                arguments.Count,
                arguments.UsesArrayStorage,
                thisArgument,
                arguments.ToArray(),
                arguments.GetArgument(arguments.Count + 1));
    }

    private sealed class ContextFreeFunction : JsFunctionObject
    {
        public override bool RequiresInvocationContext => false;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => new ContextFreeSnapshot(thisArgument, arguments.GetArgument(0));
    }

    private sealed class MaterializeArgumentsFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            var usedArrayStorage = arguments.UsesArrayStorage;
            var firstRead = RuntimeServices.GetCurrentArguments()!;
            var secondRead = RuntimeServices.GetCurrentArguments()!;
            return new MaterializedArgumentsSnapshot(usedArrayStorage, firstRead, secondRead);
        }
    }

    private sealed class DefaultsAndRestFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            var first = arguments.GetArgument(0) ?? "default";
            var rest = new object?[System.Math.Max(arguments.Count - 1, 0)];
            for (var index = 1; index < arguments.Count; index++)
            {
                rest[index - 1] = arguments.GetArgument(index);
            }

            return new DefaultsAndRestSnapshot(first, rest);
        }
    }

    private sealed class InspectConstructorFunction : JsFunctionObject
    {
        public override bool IsConstructor => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => null;

        protected override object? ConstructCore(
            in JsCallArguments arguments,
            object? newTarget)
            => new ConstructArgumentsSnapshot(
                arguments.Count,
                arguments.UsesArrayStorage,
                newTarget,
                RuntimeServices.GetCurrentCallee());
    }

    private sealed class CatchingFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            try
            {
                CallableOperations.Call0(new ThrowingFunction(), null);
            }
            catch (InvalidOperationException)
            {
            }

            return RuntimeServices.GetCurrentArguments();
        }
    }

    private sealed class ThrowingFunction : JsFunctionObject
    {
        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new InvalidOperationException("abrupt completion");
    }

    private sealed class TypedAdapterFunction : JsFunctionObject
    {
        public JsFunctionObject Identity => this;

        public double InvokeNumber(double value) => value + 1d;

        public bool InvokeBoolean(bool value) => !value;

        public string InvokeString(string value) => value + "!";

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => InvokeNumber(TypeUtilities.ToNumber(arguments.GetArgument(0)));
    }

    private sealed record ArgumentsSnapshot(
        int Count,
        bool UsesArrayStorage,
        object? ThisArgument,
        object?[] MaterializedArguments,
        object? MissingArgument);

    private sealed record MaterializedArgumentsSnapshot(
        bool UsedArrayStorageBeforeMaterialization,
        object?[] FirstRead,
        object?[] SecondRead);

    private sealed record ContextFreeSnapshot(object? ThisArgument, object? Argument);

    private sealed record DefaultsAndRestSnapshot(object? First, object?[] Rest);

    private sealed record ConstructArgumentsSnapshot(
        int Count,
        bool UsesArrayStorage,
        object? NewTarget,
        object? Callee);
}
