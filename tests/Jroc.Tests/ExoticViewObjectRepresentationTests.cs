using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class ExoticViewObjectRepresentationTests
{
    [Fact]
    public void ExoticViews_UseInlineJsObjectStorageWithoutMovingInternalSlots()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var typedArray = new Uint8Array(new object?[] { 1d, 2d });
        var emptyTypedArray = new Uint8Array();
        var arguments = new ArgumentsObject(new object?[] { "argument" }, null, null, null);
        var buffer = new JavaScriptRuntime.Node.Buffer(new byte[] { 1, 2 });
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(typedArray);
        Assert.IsAssignableFrom<JsObject>(arguments);
        Assert.IsAssignableFrom<JsObject>(buffer);
        Assert.Same(Uint8Array.Prototype, JsObjectConstructor.getPrototypeOf(typedArray));
        Assert.Same(GlobalThis.ObjectPrototypeValue, JsObjectConstructor.getPrototypeOf(arguments));

        ObjectRuntime.SetProperty(emptyTypedArray, "custom", "typed-array");
        ObjectRuntime.SetProperty(arguments, "custom", "arguments");
        ObjectRuntime.SetProperty(buffer, "custom", "buffer");
        Assert.Equal("typed-array", ObjectRuntime.GetProperty(emptyTypedArray, "custom"));
        Assert.Equal("arguments", ObjectRuntime.GetProperty(arguments, "custom"));
        Assert.Equal("buffer", ObjectRuntime.GetProperty(buffer, "custom"));

        JsObjectConstructor.setPrototypeOf(buffer, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(buffer));

        Assert.Throws<TypeError>(() => JsObjectConstructor.freeze(typedArray));
        JsObjectConstructor.freeze(emptyTypedArray);
        JsObjectConstructor.freeze(arguments);
        JsObjectConstructor.freeze(buffer);
        Assert.True(JsObjectConstructor.isFrozen(emptyTypedArray));
        Assert.True(JsObjectConstructor.isFrozen(arguments));
        Assert.True(JsObjectConstructor.isFrozen(buffer));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(emptyTypedArray, "custom", "changed"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(arguments, "custom", "changed"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(buffer, "custom", "changed"));

        Assert.Equal(1d, typedArray[0d]);
        Assert.Equal("argument", ObjectRuntime.GetItem(arguments, 0d));
        buffer.writeUInt8(255d, 0d);
        Assert.Equal(255d, buffer[0d]);
    }

    [Fact]
    public void TypedArrayAndArgumentsPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-exotic-view-prototype-descriptors",
            "ExoticView.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-exotic-view-prototype-descriptors",
            "ExoticView.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"typed-array{Environment.NewLine}arguments{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}true{Environment.NewLine}", readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-exotic-view-prototype-descriptors" => ("""
                Object.defineProperty(Uint8Array.prototype, "descriptorLeakCheck", {
                  value: "typed-array",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(Object.prototype, "argumentsDescriptorLeakCheck", {
                  value: "arguments",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new Uint8Array().descriptorLeakCheck);
                console.log((function () {
                  return arguments.argumentsDescriptorLeakCheck;
                })());
                """, null),
            "read-exotic-view-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  Uint8Array.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  Object.prototype,
                  "argumentsDescriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
