using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class EventsModuleContractTests
{
    [Fact]
    public void IEventsModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IEventsModule);

        Assert.Equal(
            "events",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IEventsModule_MapsOptionalRestAndAsyncTypes()
    {
        Assert.Contains(GetMethods("once"), method => method.GetParameters().Length == 2);
        Assert.Contains(GetMethods("once"), method => method.GetParameters().Length == 3);
        Assert.All(
            GetMethods("once"),
            method => Assert.Equal(typeof(IJavaScriptPromise), method.ReturnType));
        Assert.All(
            GetMethods("on"),
            method => Assert.Equal(typeof(IJavaScriptAsyncIterator), method.ReturnType));

        var setMaxListeners = Assert.Single(GetMethods("setMaxListeners"));
        Assert.NotNull(
            setMaxListeners.GetParameters()[1].GetCustomAttribute<ParamArrayAttribute>());
        Assert.Equal(
            typeof(Delegate),
            Assert.Single(GetMethods("addAbortListener")).GetParameters()[1].ParameterType);
        Assert.True(typeof(IEventsModule).GetProperty("defaultMaxListeners")?.CanWrite);
        Assert.True(typeof(IEventsModule).GetProperty("captureRejections")?.CanWrite);
    }

    [Fact]
    public void IEventsModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IEventsModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Equal(
            7,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(6, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicEventsModule_DelegatesAvailableMembers()
    {
        IEventsModule module = new Events();
        var emitter = new EventEmitter();

        Assert.Equal(typeof(EventEmitter), module.EventEmitter);
        Assert.IsType<Symbol>(module.errorMonitor);
        var iterator = module.on(emitter, "tick");
        Assert.True(iterator.HasReturn);
        Assert.NotNull(iterator.Return());
        Assert.IsAssignableFrom<IJavaScriptPromise>(module.once(emitter, "ready"));
        emitter.emit("ready", "value");
        Assert.Null(
            typeof(Events).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicEventsModule_ThrowsForUnavailableMembers()
    {
        IEventsModule module = new Events();
        var emitter = new EventEmitter();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.getMaxListeners(emitter));
        var propertyException = Assert.Throws<NotImplementedException>(
            () => _ = module.EventEmitterAsyncResource);
        var setterException = Assert.Throws<NotImplementedException>(
            () => module.defaultMaxListeners = 20d);

        Assert.Equal(
            "The intrinsic node:events module does not implement 'events.getMaxListeners'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:events module does not implement 'events.EventEmitterAsyncResource'.",
            propertyException.Message);
        Assert.Equal(
            "The intrinsic node:events module does not implement 'events.defaultMaxListeners'.",
            setterException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IEventsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
