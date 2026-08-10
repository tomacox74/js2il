using System.Reflection;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class DiagnosticsChannelModuleContractTests
{
    [Fact]
    public void IDiagnosticsChannelModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(
            typeof(IDiagnosticsChannelModule),
            "diagnostics_channel",
            5,
            5,
            1);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(
            typeof(IDiagnosticsChannelModule));

        var channelType = typeof(IChannel);
        Assert.Equal(
            "Channel",
            channelType.GetCustomAttribute<NodeModuleTypeAttribute>()?.TypeName);
        Assert.True(channelType.GetMethods().Count(method => !method.IsSpecialName) == 8);
        Assert.Single(channelType.GetProperties());

        var tracingChannelType = typeof(ITracingChannel);
        Assert.Equal(
            "TracingChannel",
            tracingChannelType.GetCustomAttribute<NodeModuleTypeAttribute>()?.TypeName);
        Assert.True(tracingChannelType.GetMethods().Count(method => !method.IsSpecialName) == 8);
        Assert.Single(tracingChannelType.GetProperties());
    }

    [Fact]
    public void IntrinsicDiagnosticsChannel_DelegatesAndThrowsExplicitly()
    {
        IDiagnosticsChannelModule module = new DiagnosticsChannel();
        var channel = Assert.IsType<ChannelObject>(module.channel("contract:test"));
        Delegate listener = (Action<object?, object?>)((_, _) => { });

        Assert.False(module.hasSubscribers("contract:test"));
        module.subscribe("contract:test", listener);
        Assert.True(channel.hasSubscribers);
        Assert.True(module.unsubscribe("contract:test", listener));
        Assert.False(channel.hasSubscribers);

        var exception = Assert.Throws<NotImplementedException>(
            () => module.tracingChannel("contract:test"));
        Assert.Equal(
            "The intrinsic node:diagnostics_channel module does not implement 'diagnostics_channel.tracingChannel'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(DiagnosticsChannel));
    }
}
