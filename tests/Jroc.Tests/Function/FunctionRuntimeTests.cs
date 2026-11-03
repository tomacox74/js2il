using JavaScriptRuntime;

namespace Jroc.Tests.Function;

public sealed class FunctionRuntimeTests
{
    [Fact]
    public void DynamicFunctionCallPreservesReceiverEvaluationAndSourceBoundaries()
    {
        const string source = """
            var receiverCalls = 0;
            function receiver() { receiverCalls++; return { ignored: true }; }
            const subtract = Function.call(receiver(), "a", "b", "return a - b;");
            assert.sameValue(receiverCalls, 1);
            assert.sameValue(subtract(9, 4), 5);
            assert.sameValue(subtract.length, 2);
            assert.sameValue(subtract.name, "anonymous");
            assert.sameValue(Function.call()(), undefined);
            assert.throws(SyntaxError, () => Function.call(null, "if ("));
            assert.throws(TypeError, () => new Function.call(null, ""));
            const Dynamic = Function;
            assert.sameValue(Dynamic.call(null, "return 7;")(), 7);
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(DynamicFunctionCallPreservesReceiverEvaluationAndSourceBoundaries),
            "FunctionRuntime", _ => (source, "dynamic_function_call.js"));
        Assert.Empty(result.Output);
    }

    [Fact]
    public void DynamicFunctionCallHonorsReplacedCallAndConstructor()
    {
        const string source = """
            const original = Function;
            const marker = {};
            original.call = function(receiver, body) {
              assert.sameValue(this, original);
              assert.sameValue(receiver, marker);
              return body;
            };
            assert.sameValue(Function.call(marker, "return 23;"), "return 23;");
            Function = function(parameter, body) {
              assert.sameValue(this, marker);
              assert.sameValue(parameter, "a");
              assert.sameValue(body, "return a;");
              return 42;
            };
            assert.sameValue(Function.call(marker, "a", "return a;"), 42);
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(DynamicFunctionCallHonorsReplacedCallAndConstructor),
            "FunctionRuntime", _ => (source, "dynamic_function_replaced_call.js"));
        Assert.Empty(result.Output);
    }

    [Fact]
    public void DynamicFunctionConstructorsCoerceArgumentsOnceBeforeRejectingHashbang()
    {
        const string source = """
            const constructors = [
                Function,
                (async function () {}).constructor,
                (function* () {}).constructor,
                (async function* () {}).constructor
            ];
            for (const ctor of constructors) {
                const order = [];
                const parameter = { toString() { order.push("parameter"); return "#!\n_"; } };
                const body = { toString() { order.push("body"); return ""; } };
                assert.throws(SyntaxError, () => ctor(parameter, body));
                assert.sameValue(order.join(","), "parameter,body");
                assert.throws(SyntaxError, () => new ctor("a", "#!\n_"));
            }
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "dynamic_function_coercion", "FunctionRuntime",
            _ => (source, "dynamic_function_coercion.js"));
        Assert.Empty(result.Output);
    }

    [Theory]
    [InlineData("'#!';")]
    [InlineData("\"#!\";")]
    [InlineData("`#!`;")]
    [InlineData("// #!\n")]
    [InlineData("/* #! */")]
    public void DynamicFunctionSourcePreservesHashbangTextInLiteralsAndComments(string body)
    {
        var source = JavaScriptRuntime.Function.PrepareDynamicFunctionSource(new object?[] { "a", body });
        Assert.Equal(new[] { "a" }, source.ParameterNames);
        Assert.Equal(body, source.Body);
    }

    [Fact]
    public void CollectRestArguments_UsesTypedStartIndexAndReturnsArray()
    {
        var previousArguments = RuntimeServices.SetCurrentArguments(new object?[] { "first", 2d, true });
        try
        {
            var rest = RuntimeServices.CollectRestArguments(1d);

            Assert.Equal(2, rest.Count);
            Assert.Equal(2d, rest[0]);
            Assert.True(Assert.IsType<bool>(rest[1]));
        }
        finally
        {
            RuntimeServices.SetCurrentArguments(previousArguments);
        }
    }

    [Fact]
    public void Construct_UsesDescriptorFreeJsObjectStorage()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            Func<object[], object?[]?, object?> constructor = static (_, args) =>
            {
                ObjectRuntime.SetProperty(RuntimeServices.GetCurrentThis()!, "value", args![0]);
                return 7d;
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(constructor, 1d, "Receiver", requiresInvocationContext: true);
            JavaScriptRuntime.Function.MarkConstructible(constructor);
            var constructorValue =
                BuiltinDelegateFunctionAdapter.FromDelegate(constructor);

            var instance = Assert.IsType<JsObject>(
                CallableOperations.Construct(
                    constructorValue,
                    new object?[] { 42d }));
            Assert.Equal(42d, instance["value"]);
            Assert.Equal(42d, ObjectRuntime.GetProperty(instance, "value"));
            Assert.False(instance.HasNonDataDescriptors);

            var prototype = Assert.IsType<JsObject>(
                ObjectRuntime.GetProperty(constructorValue, "prototype"));
            Assert.Same(prototype, JavaScriptRuntime.Object.getPrototypeOf(instance));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void OrdinaryFunctionPrototypes_AreJsObjects()
    {
        _ = GlobalThis.Function;

        Assert.IsType<JsObject>(JavaScriptRuntime.Function.Prototype);
        Assert.IsType<JsObject>(JavaScriptRuntime.Function.RestrictedPropertiesPrototype);
        Assert.Same(
            JavaScriptRuntime.Function.Prototype,
            JavaScriptRuntime.Object.getPrototypeOf(JavaScriptRuntime.Function.RestrictedPropertiesPrototype));

        Assert.True(PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Function.Prototype, "caller", out var caller));
        Assert.True(PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Function.Prototype, "arguments", out var arguments));
        Assert.Same(caller.Get, caller.Set);
        Assert.Same(caller.Get, arguments.Get);
        Assert.Same(arguments.Get, arguments.Set);
    }
}
