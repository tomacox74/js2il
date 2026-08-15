using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class DateObjectRepresentationTests
{
    [Fact]
    public void DateWrappers_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var date = new JavaScriptRuntime.Date(0d);
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(date);
        Assert.Same(GlobalThis.DatePrototypeValue, JsObjectConstructor.getPrototypeOf(date));
        Assert.Equal(0d, date.getTime());

        ObjectRuntime.SetProperty(date, "custom", 42d);
        Assert.Equal(42d, ObjectRuntime.GetProperty(date, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(date, "custom"));

        JsObjectConstructor.setPrototypeOf(date, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(date));

        JsObjectConstructor.freeze(date);
        Assert.True(JsObjectConstructor.isFrozen(date));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(date, "custom", 0d));
    }

    [Fact]
    public void DateWrappers_DoNotAllocatePrototypeFallbackStorage()
    {
        const int objectCount = 10_000;
        var prototype = new JsObject();

        _ = MeasureJsObjectAllocations(1, prototype);
        _ = MeasureDateWrapperAllocations(1);

        var jsObjectAllocations = MeasureJsObjectAllocations(objectCount, prototype);
        var dateWrapperAllocations = MeasureDateWrapperAllocations(objectCount);

        Assert.InRange(dateWrapperAllocations, 0, jsObjectAllocations + objectCount * 32L);
    }

    [Fact]
    public void DatePrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-date-prototype-descriptors",
            "Date.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-date-prototype-descriptors",
            "Date.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"runtime-one{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}", readResult.Output);
    }

    private static long MeasureJsObjectAllocations(int count, object prototype)
    {
        var objects = new JsObject[count];
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < objects.Length; index++)
        {
            var value = new JsObject();
            PrototypeChain.InitializePrototype(value, prototype);
            objects[index] = value;
        }

        GC.KeepAlive(objects);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static long MeasureDateWrapperAllocations(int count)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        _ = new JavaScriptRuntime.Date(0d);
        var wrappers = new JavaScriptRuntime.Date[count];
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < wrappers.Length; index++)
        {
            wrappers[index] = new JavaScriptRuntime.Date(0d);
        }

        GC.KeepAlive(wrappers);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-date-prototype-descriptors" => ("""
                Object.defineProperty(Date.prototype, "descriptorLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new Date(0).descriptorLeakCheck);
                """, null),
            "read-date-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  Date.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
