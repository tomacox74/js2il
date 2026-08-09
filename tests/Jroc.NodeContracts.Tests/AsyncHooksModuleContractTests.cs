using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class AsyncHooksModuleContractTests
{
    [Fact]
    public void IAsyncHooksModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(
            typeof(IAsyncHooksModule),
            "async_hooks",
            4,
            4,
            3);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(
            typeof(IAsyncHooksModule));

        AssertNestedContract(typeof(IAsyncHook), "AsyncHook", 2, 0);
        AssertNestedContract(
            typeof(IAsyncLocalStorage),
            "AsyncLocalStorage",
            5,
            1);
        AssertNestedContract(
            typeof(IAsyncResource),
            "AsyncResource",
            7,
            0);
    }

    [Fact]
    public void NestedContracts_PreserveRestAndCallbackMappings()
    {
        var resourceRun = NodeContractTestHelpers
            .GetMethods(typeof(IAsyncResource), "runInAsyncScope")
            .Single(method => method.GetParameters().Length == 3);
        Assert.Equal(
            typeof(Delegate),
            resourceRun.GetParameters()[0].ParameterType);
        Assert.NotNull(
            resourceRun.GetParameters()[2].GetCustomAttribute<ParamArrayAttribute>());

        var storageRun = Assert.Single(
            NodeContractTestHelpers.GetMethods(typeof(IAsyncLocalStorage), "run"));
        Assert.Equal(
            typeof(Delegate),
            storageRun.GetParameters()[1].ParameterType);
        Assert.NotNull(
            storageRun.GetParameters()[2].GetCustomAttribute<ParamArrayAttribute>());
    }

    [Fact]
    public void IntrinsicAsyncHooks_DelegatesImplementedMembersAndThrowsExplicitly()
    {
        IAsyncHooksModule module = new AsyncHooks();
        Assert.True(module.executionAsyncId() > 0);
        Assert.NotNull(module.executionAsyncResource());

        var resource = Assert.IsAssignableFrom<IAsyncResource>(
            CallableOperations.Construct1(
                module.AsyncResource,
                module.AsyncResource,
                "CONTRACT"));
        Assert.True(resource.asyncId() > 0);
        Assert.Equal(42d, resource.runInAsyncScope((Func<double>)(() => 42d)));

        var options = new JsObject
        {
            ["name"] = "contract"
        };
        var storage = Assert.IsAssignableFrom<IAsyncLocalStorage>(
            CallableOperations.Construct1(
                module.AsyncLocalStorage,
                module.AsyncLocalStorage,
                options));
        Assert.Equal("contract", storage.name);
        Assert.Equal(
            "value",
            storage.run("value", (Func<object?>)(() => storage.getStore())));

        Assert.IsAssignableFrom<IAsyncHook>(
            module.createHook(new JsObject()));
        var exception = Assert.Throws<NotImplementedException>(
            () => _ = module.asyncWrapProviders);
        Assert.Equal(
            "The intrinsic node:async_hooks module does not implement 'async_hooks.asyncWrapProviders'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(AsyncHooks));
    }

    private static void AssertNestedContract(
        Type contractType,
        string typeName,
        int methodCount,
        int propertyCount)
    {
        var identity = contractType.GetCustomAttribute<NodeModuleTypeAttribute>();
        Assert.NotNull(identity);
        Assert.Equal("async_hooks", identity.ModuleName);
        Assert.Equal(typeName, identity.TypeName);

        var generatedCode =
            contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);

        Assert.Equal(
            methodCount,
            contractType.GetMethods().Count(method => !method.IsSpecialName));
        Assert.Equal(propertyCount, contractType.GetProperties().Length);
        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(
                method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(
                property.GetCustomAttribute<NodeModuleMemberAttribute>()));
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(contractType);
    }
}
