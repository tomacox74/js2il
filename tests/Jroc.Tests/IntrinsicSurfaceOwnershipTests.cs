using System.Reflection;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class IntrinsicSurfaceOwnershipTests
{
    [Theory]
    [InlineData("Number")]
    [InlineData("Boolean")]
    [InlineData("BigInt")]
    [InlineData("Symbol")]
    [InlineData("Math")]
    [InlineData("JSON")]
    [InlineData("Reflect")]
    [InlineData("Atomics")]
    [InlineData("Intl")]
    [InlineData("RegExp")]
    [InlineData("WeakRef")]
    [InlineData("FinalizationRegistry")]
    [InlineData("SharedArrayBuffer")]
    [InlineData("Error")]
    [InlineData("AggregateError")]
    [InlineData("SuppressedError")]
    [InlineData("Date")]
    [InlineData("DisposableStack")]
    [InlineData("AsyncDisposableStack")]
    public void IntrinsicImplementationOwnsItsSurfaceConfiguration(string name)
    {
        var type = typeof(GlobalThis).Assembly.GetType($"JavaScriptRuntime.{name}");
        Assert.NotNull(type);
        var method = type.GetMethod(
            "ConfigureIntrinsicSurface",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);
        Assert.Equal(typeof(void), method.ReturnType);
        Assert.Contains($"JavaScriptRuntime.{name}.ConfigureIntrinsicSurface(", ReadIntrinsicBootstrap());
    }

    [Fact]
    public void GlobalBootstrapOnlyOrchestratesIntrinsicSurfaceConfiguration()
    {
        var bootstrap = ReadIntrinsicBootstrap();
        Assert.DoesNotContain("PropertyDescriptorStore.DefineOrUpdate", bootstrap);
        Assert.DoesNotContain("DefineIntrinsicDataProperty", bootstrap);
        Assert.DoesNotContain("DefineIntrinsicConstantDataProperty", bootstrap);
        Assert.DoesNotContain("DefineIntrinsicToStringTagProperty", bootstrap);
        Assert.DoesNotContain("DefineBuiltinFunctionProperty", bootstrap);
        Assert.DoesNotContain("PrototypeChain.SetPrototype", bootstrap);
    }

    private static string ReadIntrinsicBootstrap()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "JavaScriptRuntime", "GlobalThis.cs"));
        var start = source.IndexOf("private void InitializeIntrinsicsCore()", StringComparison.Ordinal);
        Assert.True(start >= 0);
        var end = source.IndexOf("private static object ConstructTypedArray(", start, StringComparison.Ordinal);
        Assert.True(end > start);
        return source[start..end];
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
            directory is not null;
            directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "src", "JavaScriptRuntime", "GlobalThis.cs")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the runtime source directory.");
    }
}
