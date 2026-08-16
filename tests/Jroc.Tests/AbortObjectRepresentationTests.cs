using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class AbortObjectRepresentationTests
{
    [Fact]
    public void AbortWrappers_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        _ = GlobalThis.globalThis;

        var controller = Assert.IsType<AbortController>(
            ObjectRuntime.ConstructValue(
                GlobalThis.AbortController,
                System.Array.Empty<object>()));
        var signal = controller.signal;
        var customControllerPrototype = new JsObject();
        var customSignalPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(controller);
        Assert.IsAssignableFrom<JsObject>(signal);
        Assert.Same(AbortController.Prototype, JsObjectConstructor.getPrototypeOf(controller));
        Assert.Same(AbortSignal.Prototype, JsObjectConstructor.getPrototypeOf(signal));
        Assert.Same(
            AbortController.Prototype,
            ObjectRuntime.GetProperty(GlobalThis.AbortController, "prototype"));
        Assert.Same(
            AbortSignal.Prototype,
            ObjectRuntime.GetProperty(GlobalThis.AbortSignal, "prototype"));
        Assert.Same(
            GlobalThis.AbortController,
            ObjectRuntime.GetProperty(AbortController.Prototype, "constructor"));
        Assert.Same(
            GlobalThis.AbortSignal,
            ObjectRuntime.GetProperty(AbortSignal.Prototype, "constructor"));

        var abort = ObjectRuntime.GetProperty(controller, "abort");
        var addEventListener = ObjectRuntime.GetProperty(signal, "addEventListener");
        Assert.Same(
            ObjectRuntime.GetProperty(AbortController.Prototype, "abort"),
            abort);
        Assert.Same(
            ObjectRuntime.GetProperty(AbortSignal.Prototype, "addEventListener"),
            addEventListener);
        Assert.Same(signal, ObjectRuntime.GetProperty(controller, "signal"));
        Assert.False(JsObjectConstructor.hasOwn(controller, "signal"));
        Assert.False(JsObjectConstructor.hasOwn(signal, "aborted"));
        Assert.False(JsObjectConstructor.hasOwn(signal, "reason"));

        var signalDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(
                AbortController.Prototype,
                "signal"));
        var signalGetter = ObjectRuntime.GetProperty(signalDescriptor, "get");
        Assert.Throws<TypeError>(
            () => CallableOperations.Call0(signalGetter, new JsObject()));
        Assert.Throws<TypeError>(
            () => CallableOperations.Call0(addEventListener, new JsObject()));

        ObjectRuntime.SetProperty(controller, "custom", 42d);
        ObjectRuntime.SetProperty(signal, "custom", "signal");
        var controllerCustomDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(controller, "custom"));
        var signalCustomDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(signal, "custom"));
        Assert.Equal(42d, ObjectRuntime.GetProperty(controllerCustomDescriptor, "value"));
        Assert.Equal("signal", ObjectRuntime.GetProperty(signalCustomDescriptor, "value"));
        Assert.True(JsObjectConstructor.hasOwn(controller, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(signal, "custom"));

        JsObjectConstructor.setPrototypeOf(controller, customControllerPrototype);
        JsObjectConstructor.setPrototypeOf(signal, customSignalPrototype);
        Assert.Same(customControllerPrototype, JsObjectConstructor.getPrototypeOf(controller));
        Assert.Same(customSignalPrototype, JsObjectConstructor.getPrototypeOf(signal));
        JsObjectConstructor.setPrototypeOf(controller, AbortController.Prototype);
        JsObjectConstructor.setPrototypeOf(signal, AbortSignal.Prototype);

        JsObjectConstructor.freeze(controller);
        JsObjectConstructor.freeze(signal);
        Assert.True(JsObjectConstructor.isFrozen(controller));
        Assert.True(JsObjectConstructor.isFrozen(signal));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(controller, "custom", 0d));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(signal, "custom", "changed"));

        var calls = 0;
        Func<object[], object?[]?, object?> listener = (_, _) =>
        {
            calls++;
            return null;
        };
        var abortReason = new JsObject();
        CallableOperations.Call2(
            addEventListener,
            signal,
            "abort",
            BuiltinDelegateFunctionAdapter.FromDelegate(listener));
        CallableOperations.Call1(abort, controller, abortReason);

        Assert.Equal(1, calls);
        Assert.True(signal.aborted);
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(signal, "aborted")));
        Assert.Same(abortReason, signal.reason);
        Assert.Same(abortReason, ObjectRuntime.GetProperty(signal, "reason"));
    }

    [Fact]
    public void AbortPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-abort-prototype-descriptors",
            "Abort.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-abort-prototype-descriptors",
            "Abort.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal(
            $"controller{Environment.NewLine}signal{Environment.NewLine}",
            mutationResult.Output);
        Assert.Equal(
            $"true{Environment.NewLine}true{Environment.NewLine}",
            readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-abort-prototype-descriptors" => ("""
                const controller = new AbortController();
                const signal = new AbortSignal();
                Object.defineProperty(AbortController.prototype, "descriptorLeakCheck", {
                  value: "controller",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(AbortSignal.prototype, "descriptorLeakCheck", {
                  value: "signal",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(controller.descriptorLeakCheck);
                console.log(signal.descriptorLeakCheck);
                """, null),
            "read-abort-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  AbortController.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  AbortSignal.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(
                nameof(testName),
                testName,
                "Unknown descriptor isolation script.")
        };
}
