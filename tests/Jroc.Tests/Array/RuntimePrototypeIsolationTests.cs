using System.Runtime.ExceptionServices;
using JavaScriptRuntime;
using Xunit;

namespace Jroc.Tests.Array;

public class RuntimePrototypeIsolationTests
{
    [Fact]
    public void InMemoryScripts_DoNotShareArrayPrototypeDescriptorMutations()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-array-prototype-descriptors",
            "Array.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-array-prototype-descriptors",
            "Array.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "123",
                "true",
                string.Empty),
            mutationResult.Output);
        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "true",
                "false",
                "true",
                "true",
                string.Empty),
            readResult.Output);
    }

    [Fact]
    public void InMemoryScripts_DoNotShareObjectPrototypeDescriptorMutations()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-object-prototype-descriptors",
            "Runtime.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-object-prototype-descriptors",
            "Runtime.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "runtime-one",
                "runtime-one",
                "getter-runtime-one",
                "setter-runtime-one",
                "3",
                string.Empty),
            mutationResult.Output);
        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "true",
                "undefined",
                "true",
                "true",
                "0",
                string.Empty),
            readResult.Output);
    }

    [Fact]
    public void ArrayPrototype_UsesThreadLocalOverrides()
    {
        var results = new string?[2];
        var exceptions = new ExceptionDispatchInfo?[2];
        using var barrier = new Barrier(2);

        var left = CreatePrototypeMutationThread("left", 0);
        var right = CreatePrototypeMutationThread("right", 1);

        left.Start();
        right.Start();
        left.Join();
        right.Join();

        exceptions[0]?.Throw();
        exceptions[1]?.Throw();

        Assert.Equal("left", results[0]);
        Assert.Equal("right", results[1]);

        Thread CreatePrototypeMutationThread(string marker, int resultIndex)
            => new(() =>
            {
                try
                {
                    JavaScriptRuntime.Array.ResetPrototypeForTests();
                    var prototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                        ?? throw new InvalidOperationException("Array.prototype was not configured.");
                    ObjectRuntime.SetProperty(prototype, "threadMarker", marker);

                    var array = new JavaScriptRuntime.Array();
                    barrier.SignalAndWait();

                    results[resultIndex] = DotNet2JSConversions.ToString(ObjectRuntime.GetItem(array, "threadMarker"));
                }
                catch (Exception ex)
                {
                    exceptions[resultIndex] = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    JavaScriptRuntime.Array.ResetPrototypeForTests();
                }
            });
    }

    [Fact]
    public void ArrayPrototypeThreadOverride_RetainsIntrinsicDescriptorsAcrossSerialRuntimeStores()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var firstRuntime = RuntimeServices.BuildServiceProvider();
            var secondRuntime = RuntimeServices.BuildServiceProvider();

            GlobalThis.ServiceProvider = firstRuntime;
            var firstPrototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                ?? throw new InvalidOperationException("Array.prototype was not configured.");
            Assert.NotNull(ObjectRuntime.GetItem(firstPrototype, "push"));

            GlobalThis.ServiceProvider = null;
            GlobalThis.ServiceProvider = secondRuntime;

            var secondPrototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                ?? throw new InvalidOperationException("Array.prototype was not configured.");
            Assert.Same(firstPrototype, secondPrototype);
            Assert.NotNull(ObjectRuntime.GetItem(secondPrototype, "push"));
            Assert.NotNull(ObjectRuntime.GetItem(new JavaScriptRuntime.Array(), "push"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    [Fact]
    public void ObjectAssign_UsesDescriptorOverlayForIntrinsicDictionaryBackedPrototypes()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var firstRuntime = RuntimeServices.BuildServiceProvider();
            var secondRuntime = RuntimeServices.BuildServiceProvider();

            GlobalThis.ServiceProvider = firstRuntime;
            var arrayPrototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                ?? throw new InvalidOperationException("Array.prototype was not configured.");
            ObjectRuntime.SetProperty(arrayPrototype, "assignSourceLeak", "runtime-one");

            var copied = new JsObject();
            JavaScriptRuntime.Object.assign(copied, arrayPrototype);
            Assert.Equal("runtime-one", ObjectRuntime.GetItem(copied, "assignSourceLeak"));

            var source = new JsObject();
            ObjectRuntime.SetProperty(source, "assignTargetLeak", "runtime-one");
            JavaScriptRuntime.Object.assign(arrayPrototype, source);
            Assert.Equal("runtime-one", ObjectRuntime.GetItem(arrayPrototype, "assignTargetLeak"));

            GlobalThis.ServiceProvider = null;
            GlobalThis.ServiceProvider = secondRuntime;

            var secondPrototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                ?? throw new InvalidOperationException("Array.prototype was not configured.");
            Assert.Same(arrayPrototype, secondPrototype);
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(secondPrototype, "assignSourceLeak"));
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(secondPrototype, "assignTargetLeak"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-array-prototype-descriptors" => ("""
                Object.defineProperty(Array.prototype, "descriptorLeakCheck", {
                  value: 123,
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(Array.prototype, "push", {
                  value: Array.prototype.push,
                  enumerable: true,
                  configurable: false,
                  writable: false
                });

                console.log(Object.getOwnPropertyDescriptor(Array.prototype, "descriptorLeakCheck").value);
                console.log(Object.getOwnPropertyDescriptor(Array.prototype, "push").enumerable);
                """, null),
            "read-array-prototype-descriptors" => ("""
                var leakedDescriptor = Object.getOwnPropertyDescriptor(Array.prototype, "descriptorLeakCheck");
                var pushDescriptor = Object.getOwnPropertyDescriptor(Array.prototype, "push");

                console.log(leakedDescriptor === undefined);
                console.log(pushDescriptor.enumerable);
                console.log(pushDescriptor.configurable);
                console.log(pushDescriptor.writable);
                """, null),
            "mutate-object-prototype-descriptors" => ("""
                Object.defineProperty(Object.prototype, "prototypeLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });

                var child = Object.create(Object.prototype);
                console.log(child.prototypeLeakCheck);

                var shadow = Object.create(Object.prototype);
                Object.defineProperty(shadow, "prototypeLeakCheck", {
                  value: "own-shadow",
                  enumerable: false,
                  configurable: true,
                  writable: true
                });
                delete shadow.prototypeLeakCheck;
                console.log(shadow.prototypeLeakCheck);

                Object.prototype.__defineGetter__("getterLeakCheck", function() {
                  return "getter-runtime-one";
                });
                console.log(({}).getterLeakCheck);

                Object.prototype.__defineSetter__("setterLeakCheck", function(value) {
                  this.setterValue = value;
                });
                var setterChild = {};
                setterChild.setterLeakCheck = "setter-runtime-one";
                console.log(setterChild.setterValue);

                var count = 0;
                for (var key in JSON) {
                  count++;
                }
                console.log(count);
                """, null),
            "read-object-prototype-descriptors" => ("""
                var leakedDescriptor = Object.getOwnPropertyDescriptor(Object.prototype, "prototypeLeakCheck");
                var getterDescriptor = Object.getOwnPropertyDescriptor(Object.prototype, "getterLeakCheck");
                var setterDescriptor = Object.getOwnPropertyDescriptor(Object.prototype, "setterLeakCheck");
                var child = Object.create(Object.prototype);

                console.log(leakedDescriptor === undefined);
                console.log(typeof child.prototypeLeakCheck);
                console.log(getterDescriptor === undefined);
                console.log(setterDescriptor === undefined);

                var count = 0;
                for (var key in JSON) {
                  count++;
                }
                console.log(count);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
