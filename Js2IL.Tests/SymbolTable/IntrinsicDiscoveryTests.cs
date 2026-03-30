using System.Reflection;
using Js2IL.SymbolTables;
using Xunit;

namespace Js2IL.Tests.SymbolTableTests;

public class IntrinsicDiscoveryTests
{
    private static bool IsKnownGlobalIntrinsic(string name)
    {
        var method = typeof(SymbolTableBuilder).GetMethod(
            "IsKnownGlobalIntrinsic",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        return (bool)method!.Invoke(null, new object[] { name })!;
    }

    [Theory]
    [InlineData("process")]
    [InlineData("console")]
    [InlineData("Infinity")]
    [InlineData("NaN")]
    [InlineData("setTimeout")]
    [InlineData("clearTimeout")]
    [InlineData("setInterval")]
    [InlineData("clearInterval")]
    [InlineData("setImmediate")]
    [InlineData("clearImmediate")]
    [InlineData("Int32Array")]
    [InlineData("Promise")]
    [InlineData("Error")]
    [InlineData("TypeError")]
    [InlineData("Boolean")]
    [InlineData("Buffer")]
    public void IsKnownGlobalIntrinsic_ReturnsTrue_ForRuntimeBackedIntrinsics(string name)
    {
        Assert.True(IsKnownGlobalIntrinsic(name));
    }

    [Theory]
    [InlineData("NotARealIntrinsic")]
    public void IsKnownGlobalIntrinsic_ReturnsFalse_ForUnsupportedOrUnknownNames(string name)
    {
        Assert.False(IsKnownGlobalIntrinsic(name));
    }
}
