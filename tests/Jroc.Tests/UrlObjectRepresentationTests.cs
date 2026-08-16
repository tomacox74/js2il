using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class UrlObjectRepresentationTests
{
    [Fact]
    public void UrlWrappersAndIterators_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var url = new JavaScriptRuntime.Node.URL(
            "https://user:password@example.test:8443/path?one=1#initial");
        var searchParams = url.searchParams;
        var iterator = searchParams.entries();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(url);
        Assert.IsAssignableFrom<JsObject>(searchParams);
        Assert.IsAssignableFrom<JsObject>(iterator);
        Assert.Same(JavaScriptRuntime.Node.URL.Prototype, JsObjectConstructor.getPrototypeOf(url));
        Assert.Same(JavaScriptRuntime.Node.URLSearchParams.Prototype, JsObjectConstructor.getPrototypeOf(searchParams));
        Assert.Same(JavaScriptRuntime.Iterator.Prototype, JsObjectConstructor.getPrototypeOf(iterator));

        Assert.Equal(url.href, ObjectRuntime.GetProperty(url, "href"));
        Assert.Equal(url.origin, ObjectRuntime.GetProperty(url, "origin"));
        Assert.Equal(url.protocol, ObjectRuntime.GetProperty(url, "protocol"));
        Assert.Equal(url.username, ObjectRuntime.GetProperty(url, "username"));
        Assert.Equal(url.password, ObjectRuntime.GetProperty(url, "password"));
        Assert.Equal(url.host, ObjectRuntime.GetProperty(url, "host"));
        Assert.Equal(url.hostname, ObjectRuntime.GetProperty(url, "hostname"));
        Assert.Equal(url.port, ObjectRuntime.GetProperty(url, "port"));
        Assert.Equal(url.pathname, ObjectRuntime.GetProperty(url, "pathname"));
        Assert.Equal(url.search, ObjectRuntime.GetProperty(url, "search"));
        Assert.Equal(url.hash, ObjectRuntime.GetProperty(url, "hash"));
        Assert.Same(searchParams, ObjectRuntime.GetProperty(url, "searchParams"));
        Assert.Equal(searchParams.size, ObjectRuntime.GetProperty(searchParams, "size"));

        var hrefDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(JavaScriptRuntime.Node.URL.Prototype, "href"));
        var hrefGetter = ObjectRuntime.GetProperty(hrefDescriptor, "get");
        Assert.Throws<TypeError>(() => CallableOperations.Call0(hrefGetter, new JsObject()));

        ObjectRuntime.SetProperty(url, "search", "?two=2");
        ObjectRuntime.SetProperty(url, "hash", "changed");
        searchParams.append("live", "value");
        Assert.Same(searchParams, url.searchParams);
        Assert.Equal("?two=2&live=value", url.search);
        Assert.Equal("#changed", url.hash);

        ObjectRuntime.SetProperty(url, "custom", 42d);
        ObjectRuntime.SetProperty(searchParams, "custom", "searchParams");
        ObjectRuntime.SetProperty(iterator, "custom", "iterator");
        Assert.Equal(42d, ObjectRuntime.GetProperty(url, "custom"));
        Assert.Equal("searchParams", ObjectRuntime.GetProperty(searchParams, "custom"));
        Assert.Equal("iterator", ObjectRuntime.GetProperty(iterator, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(url, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(searchParams, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(iterator, "custom"));

        JsObjectConstructor.setPrototypeOf(url, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(url));
        JsObjectConstructor.setPrototypeOf(url, JavaScriptRuntime.Node.URL.Prototype);

        JsObjectConstructor.freeze(url);
        JsObjectConstructor.freeze(searchParams);
        JsObjectConstructor.freeze(iterator);
        Assert.True(JsObjectConstructor.isFrozen(url));
        Assert.True(JsObjectConstructor.isFrozen(searchParams));
        Assert.True(JsObjectConstructor.isFrozen(iterator));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(url, "custom", 0d));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(searchParams, "custom", "changed"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(iterator, "custom", "changed"));

        ObjectRuntime.SetProperty(url, "hash", "after-freeze");
        searchParams.append("mutable", "internal-slot");
        Assert.Equal("#after-freeze", url.hash);
        Assert.Equal(
            "?two=2&live=value&mutable=internal-slot",
            url.search);

        var firstEntry = iterator.Next();
        Assert.False(firstEntry.done);
        iterator.Return();
        Assert.True(iterator.Next().done);

        var exhaustedIterator = searchParams.values();
        Assert.False(exhaustedIterator.Next().done);
        Assert.False(exhaustedIterator.Next().done);
        Assert.False(exhaustedIterator.Next().done);
        Assert.True(exhaustedIterator.Next().done);
        Assert.True(exhaustedIterator.Next().done);
    }

    [Fact]
    public void UrlPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-url-prototype-descriptors",
            "Url.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-url-prototype-descriptors",
            "Url.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal(
            $"url{Environment.NewLine}search-params{Environment.NewLine}",
            mutationResult.Output);
        Assert.Equal(
            $"true{Environment.NewLine}true{Environment.NewLine}",
            readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-url-prototype-descriptors" => ("""
                Object.defineProperty(URL.prototype, "descriptorLeakCheck", {
                  value: "url",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(URLSearchParams.prototype, "descriptorLeakCheck", {
                  value: "search-params",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new URL("https://example.test").descriptorLeakCheck);
                console.log(new URLSearchParams().descriptorLeakCheck);
                """, null),
            "read-url-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  URL.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  URLSearchParams.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
