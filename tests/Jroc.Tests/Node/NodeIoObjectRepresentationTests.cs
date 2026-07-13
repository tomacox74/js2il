using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Xunit;
using NodeChildProcess = JavaScriptRuntime.Node.ChildProcess;
using NodeProcess = JavaScriptRuntime.Node.Process;
using NodeUtil = JavaScriptRuntime.Node.Util;

namespace Jroc.Tests.Node;

public class NodeIoObjectRepresentationTests
{
    [Fact]
    public void ProcessVersionsAndEnvironment_AreJsObjects_WithoutInterningEnvironmentKeys()
    {
        var environmentKey = $"JROC_TEST_{Guid.NewGuid():N}";
        var previousValue = Environment.GetEnvironmentVariable(environmentKey);

        Assert.Null(string.IsInterned(environmentKey));
        Environment.SetEnvironmentVariable(environmentKey, "value");

        try
        {
            var process = new NodeProcess(new NonTerminatingEnvironment());

            Assert.IsType<JsObject>(process.versions);
            var environment = Assert.IsType<JsObject>(process.env);
            Assert.Equal("value", ObjectRuntime.GetProperty(environment, environmentKey));
            environment[environmentKey] = "updated";
            Assert.Equal("updated", ObjectRuntime.GetProperty(environment, environmentKey));
            Assert.Contains(environmentKey, environment.GetOwnPropertyNames());
            Assert.Null(string.IsInterned(environmentKey));
        }
        finally
        {
            Environment.SetEnvironmentVariable(environmentKey, previousValue);
        }
    }

    [Fact]
    public void ChildProcessSpawnSync_ReturnsJsObject()
    {
        var childProcess = new NodeChildProcess();
        var command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh";
        var args = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new object[] { "/c", "echo jroc" }
            : new object[] { "-c", "printf jroc" };

        var result = Assert.IsType<JsObject>(childProcess.spawnSync(command, args));

        Assert.Equal(0d, ObjectRuntime.GetProperty(result, "status"));
        Assert.Contains("jroc", ObjectRuntime.GetProperty(result, "stdout")?.ToString());
    }

    [Fact]
    public void ChildProcessSpawnSyncFailure_ReturnsJsObject()
    {
        var result = Assert.IsType<JsObject>(
            new NodeChildProcess().spawnSync($"jroc-missing-command-{Guid.NewGuid():N}"));

        Assert.Equal(-1d, ObjectRuntime.GetProperty(result, "status"));
        Assert.NotNull(ObjectRuntime.GetProperty(result, "error"));
    }

    [Fact]
    public void ChildProcessForkOptions_RecognizeNonEnumerableHostDictionaryProperties()
    {
        var options = new Dictionary<string, object?> { ["silent"] = true };
        PropertyDescriptorStore.DefineOrUpdate(options, "silent", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = true,
            Writable = true,
            Enumerable = false,
            Configurable = true,
        });
        var tryHasOwnOption = typeof(NodeChildProcess).GetMethod(
            "TryHasOwnOption",
            BindingFlags.Static | BindingFlags.NonPublic)!;
        var arguments = new object?[] { options, "silent", null };

        var found = Assert.IsType<bool>(tryHasOwnOption.Invoke(null, arguments));

        Assert.True(found);
        Assert.True(Assert.IsType<bool>(arguments[2]));
    }

    [Fact]
    public void NetAddressRecord_IsJsObject()
    {
        var result = Assert.IsType<JsObject>(NodeNetworkingCommon.CreateAddressRecord("127.0.0.1", 8123));

        Assert.Equal("127.0.0.1", ObjectRuntime.GetProperty(result, "address"));
        Assert.Equal("IPv4", ObjectRuntime.GetProperty(result, "family"));
        Assert.Equal(8123d, ObjectRuntime.GetProperty(result, "port"));
        Assert.Equal(
            "{\"address\":\"127.0.0.1\",\"family\":\"IPv4\",\"port\":8123}",
            JavaScriptRuntime.JSON.Stringify(result));
        Assert.Contains("address", new NodeUtil().inspect(result));
    }
}
