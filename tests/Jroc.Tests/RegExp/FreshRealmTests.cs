using System.Reflection;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Jroc.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Tests.RegExp;

public sealed class FreshRealmTests
{
    [Fact]
    public void IntrinsicConstructorAccessPreservesRealmIdentityAndUserChanges()
    {
        BuiltinDelegateFunctionAdapter? previous = null;
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var lifecycle = CreateLifecycle();
            using var scope = lifecycle.EnterAsRoot();
            Assert.False(RuntimeIntrinsics.Current.IsBootstrapped);

            var constructor = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp);
            Assert.True(constructor.IsConstructor);
            Assert.NotSame(previous, constructor);
            Assert.Same(JavaScriptRuntime.RegExp.Prototype, ObjectRuntime.GetItem(constructor, "prototype"));
            Assert.Same(constructor, ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "constructor"));

            ObjectRuntime.SetItem(JavaScriptRuntime.RegExp.Prototype, "constructor", "changed");
            Assert.Same(constructor, BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp));
            Assert.Equal("changed", ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "constructor"));
            previous = constructor;
        }
    }

    [Theory]
    [InlineData("(?:)", "abc", new[] { "a", "b", "c" })]
    [InlineData(",", "a,b,c", new[] { "a", "b", "c" })]
    public void SplitInitializesItsDefaultConstructorInEachFreshRealm(
        string pattern,
        string input,
        string[] expected)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var lifecycle = CreateLifecycle();
            using var scope = lifecycle.EnterAsRoot();
            Assert.False(RuntimeIntrinsics.Current.IsBootstrapped);

            var regExp = JavaScriptRuntime.RegExp.CreateLiteral(pattern, "");
            Assert.False(RuntimeIntrinsics.Current.IsBootstrapped);

            var result = JavaScriptRuntime.String.SplitWithRegExp(input, regExp, null);

            Assert.Equal(expected, Enumerable.Range(0, expected.Length)
                .Select(index => Assert.IsType<string>(result[index])));
            Assert.Equal(expected.Length, result.Count);
            Assert.True(BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp).IsConstructor);
        }
    }

    [Fact]
    public void MatchAllInitializesItsDefaultConstructorInEachFreshRealm()
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var lifecycle = CreateLifecycle();
            using var scope = lifecycle.EnterAsRoot();
            Assert.False(RuntimeIntrinsics.Current.IsBootstrapped);

            var regExp = JavaScriptRuntime.RegExp.CreateLiteral("a", "g");
            Assert.False(RuntimeIntrinsics.Current.IsBootstrapped);

            var iterator = Assert.IsAssignableFrom<IJavaScriptIterator>(regExp.matchAllSymbol("aba"));
            var first = iterator.Next();
            var second = iterator.Next();
            Assert.False(first.done);
            Assert.False(second.done);
            Assert.Equal("a", Assert.IsType<JavaScriptRuntime.Array>(first.value)[0]);
            Assert.Equal("a", Assert.IsType<JavaScriptRuntime.Array>(second.value)[0]);
            Assert.True(iterator.Next().done);
        }
    }

    [Theory]
    [InlineData("fresh-regexp", null)]
    [InlineData("dromaeo-object-regexp", "Compile_Performance_Dromaeo_Object_Regexp")]
    [InlineData("dromaeo-object-regexp-modern", "Compile_Performance_Dromaeo_Object_Regexp_Modern")]
    public void HostedCompiledModulesExecuteWithUnmodifiedHostGlobals(
        string moduleName,
        string? resourceName)
    {
        var source = """
            var empty = "abc".split(/(?:)/);
            var nonempty = "a,b,c".split(/,/);
            var matches = "aba".matchAll(/a/g);
            if (empty.length !== 3 || empty[0] !== "a" || empty[1] !== "b" || empty[2] !== "c" ||
                nonempty.length !== 3 || nonempty[0] !== "a" || nonempty[1] !== "b" || nonempty[2] !== "c" ||
                matches.next().value[0] !== "a" || matches.next().value[0] !== "a" || !matches.next().done) {
                throw new Error("Unexpected RegExp results");
            }
            """;
        if (resourceName is not null)
        {
            using var stream = typeof(FreshRealmTests).Assembly.GetManifestResourceStream(
                $"Jroc.Tests.Integration.JavaScript.{resourceName}.js");
            Assert.NotNull(stream);
            using var reader = new StreamReader(stream);
            source = reader.ReadToEnd();
        }

        var directory = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "RegExpBootstrap", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var loadContext = new AssemblyLoadContext($"RegExpBootstrap-{Guid.NewGuid():N}", isCollectible: true);
        try
        {
            var scriptPath = Path.Combine(directory, moduleName + ".js");
            File.WriteAllText(scriptPath, source);
            var options = new CompilerOptions
            {
                OutputDirectory = directory,
                AssumeUnmodifiedHostGlobals = true
            };
            using var services = CompilerServices.BuildServiceProvider(options);
            Assert.True(services.GetRequiredService<Compiler>().Compile(scriptPath, moduleName));

            var assembly = loadContext.LoadFromAssemblyPath(Path.Combine(directory, moduleName + ".dll"));
            for (var attempt = 0; attempt < 2; attempt++)
            {
                using var exports = JsEngine.LoadModule(assembly, JsEngine.GetEntryModuleId(assembly));
            }
        }
        finally
        {
            loadContext.Unload();
            Directory.Delete(directory, recursive: true);
        }
    }

    private static RuntimeLifecycle CreateLifecycle()
        => RuntimeLifecycle.Create(Assembly.GetExecutingAssembly(), isHostedExecution: true);
}
