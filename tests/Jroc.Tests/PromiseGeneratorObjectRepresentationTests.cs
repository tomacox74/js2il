using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class PromiseGeneratorObjectRepresentationTests
{
    [Fact]
    public void PromiseAndResolverObjects_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var promise = (JavaScriptRuntime.Promise)JavaScriptRuntime.Promise.resolve("value")!;
        var resolvers = JavaScriptRuntime.Promise.withResolvers();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(promise);
        Assert.IsAssignableFrom<JsObject>(resolvers);
        Assert.Same(JavaScriptRuntime.Promise.Prototype, JsObjectConstructor.getPrototypeOf(promise));
        Assert.Same(GlobalThis.ObjectPrototypeValue, JsObjectConstructor.getPrototypeOf(resolvers));
        Assert.Same(resolvers.promise, ObjectRuntime.GetProperty(resolvers, "promise"));
        Assert.Same(resolvers.resolve, ObjectRuntime.GetProperty(resolvers, "resolve"));
        Assert.Same(resolvers.reject, ObjectRuntime.GetProperty(resolvers, "reject"));

        ObjectRuntime.SetProperty(promise, "custom", 42d);
        ObjectRuntime.SetProperty(resolvers, "custom", "resolvers");
        Assert.Equal(42d, ObjectRuntime.GetProperty(promise, "custom"));
        Assert.Equal("resolvers", ObjectRuntime.GetProperty(resolvers, "custom"));

        JsObjectConstructor.setPrototypeOf(promise, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(promise));

        JsObjectConstructor.freeze(promise);
        JsObjectConstructor.freeze(resolvers);
        Assert.True(JsObjectConstructor.isFrozen(promise));
        Assert.True(JsObjectConstructor.isFrozen(resolvers));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(promise, "custom", 0d));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(resolvers, "custom", "changed"));

        Assert.IsType<JavaScriptRuntime.Promise>(promise.then());
        CallableOperations.Call1(resolvers.resolve, null, "resolved");
    }

    [Fact]
    public void GeneratorObjects_UseInlineJsObjectStorage()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "generator-object-inline-storage",
            "Generator.ObjectRepresentation",
            GetInlineStorageScript);

        Assert.Equal(
            $"true{Environment.NewLine}generator{Environment.NewLine}true{Environment.NewLine}1{Environment.NewLine}" +
            $"true{Environment.NewLine}async-generator{Environment.NewLine}true{Environment.NewLine}2{Environment.NewLine}",
            result.Output);
    }

    [Fact]
    public void PromiseAndGeneratorPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-promise-generator-prototype-descriptors",
            "PromiseGenerator.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-promise-generator-prototype-descriptors",
            "PromiseGenerator.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"promise{Environment.NewLine}generator{Environment.NewLine}async-generator{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}true{Environment.NewLine}true{Environment.NewLine}", readResult.Output);
    }

    private static (string Script, string? SourcePath) GetInlineStorageScript(string testName)
    {
        Assert.Equal("generator-object-inline-storage", testName);
        return ("""
            "use strict";

            function* generator() {
              yield 1;
            }

            async function* asyncGenerator() {
              yield 2;
            }

            const iterator = generator();
            console.log(Object.getPrototypeOf(iterator) === Object.getPrototypeOf(generator()));
            iterator.custom = "generator";
            console.log(iterator.custom);
            Object.freeze(iterator);
            console.log(Object.isFrozen(iterator));
            console.log(iterator.next().value);

            (async () => {
              const asyncIterator = asyncGenerator();
              console.log(Object.getPrototypeOf(asyncIterator) === Object.getPrototypeOf(asyncGenerator()));
              asyncIterator.custom = "async-generator";
              console.log(asyncIterator.custom);
              Object.freeze(asyncIterator);
              console.log(Object.isFrozen(asyncIterator));
              console.log((await asyncIterator.next()).value);
            })();
            """, null);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-promise-generator-prototype-descriptors" => ("""
                function* generator() {
                  yield 1;
                }

                async function* asyncGenerator() {
                  yield 2;
                }

                Object.defineProperty(Promise.prototype, "descriptorLeakCheck", {
                  value: "promise",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(Object.getPrototypeOf(generator()), "descriptorLeakCheck", {
                  value: "generator",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(Object.getPrototypeOf(asyncGenerator()), "descriptorLeakCheck", {
                  value: "async-generator",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(Promise.resolve().descriptorLeakCheck);
                console.log(generator().descriptorLeakCheck);
                console.log(asyncGenerator().descriptorLeakCheck);
                """, null),
            "read-promise-generator-prototype-descriptors" => ("""
                function* generator() {
                  yield 1;
                }

                async function* asyncGenerator() {
                  yield 2;
                }

                console.log(Object.getOwnPropertyDescriptor(
                  Promise.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  Object.getPrototypeOf(generator()),
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  Object.getPrototypeOf(asyncGenerator()),
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
