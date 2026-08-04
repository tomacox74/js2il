using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class UtilTypesModuleContractTests
{
    [Fact]
    public void IUtilTypesModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IUtilTypesModule);

        Assert.Equal(
            "util/types",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IUtilTypesModule_MapsCompletePredicateSurface()
    {
        var methods = typeof(IUtilTypesModule).GetMethods();

        Assert.Equal(43, methods.Length);
        Assert.All(methods, method =>
        {
            Assert.Equal(typeof(bool), method.ReturnType);
            Assert.Equal(typeof(object), Assert.Single(method.GetParameters()).ParameterType);
            Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>());
        });
        Assert.Empty(typeof(IUtilTypesModule).GetProperties());
    }

    [Fact]
    public void IntrinsicUtilTypesModule_PreservesAliasIdentityAndDelegatesPredicates()
    {
        var util = new Util();
        var module = Assert.IsAssignableFrom<IUtilTypesModule>(util.types);
        var arrayBuffer = new ArrayBuffer(4d);
        var sharedArrayBuffer = new SharedArrayBuffer(4d);

        Assert.True(module.isArrayBuffer(arrayBuffer));
        Assert.False(module.isArrayBuffer(sharedArrayBuffer));
        Assert.True(module.isAnyArrayBuffer(sharedArrayBuffer));
        Assert.True(module.isArrayBufferView(new Uint8Array(4d)));
        Assert.True(module.isSharedArrayBuffer(sharedArrayBuffer));
        Assert.Null(
            typeof(UtilTypesModule).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicUtilTypesModule_ThrowsForUnavailablePredicates()
    {
        IUtilTypesModule module = new UtilTypesModule();

        var exception = Assert.Throws<NotImplementedException>(
            () => module.isBoxedPrimitive("value"));

        Assert.Equal(
            "The intrinsic node:util/types module does not implement 'util.types.isBoxedPrimitive'.",
            exception.Message);
    }
}
