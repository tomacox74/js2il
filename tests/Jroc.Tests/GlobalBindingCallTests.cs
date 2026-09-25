using JavaScriptRuntime;
using Jroc.Tests.Utilities;

namespace Jroc.Tests;

public class GlobalBindingCallTests
{
    [Fact]
    public void ReassignedGlobalCallsUseCurrentBinding()
    {
        const string source = """
            globalThis.Object = function () { return "fake-object"; };
            globalThis.Array = function () { return "fake-array"; };
            globalThis.parseInt = function () { return "fake-parseInt"; };
            console.log(Object(), Array(), parseInt("123"));
            """;

        var artifact = Compile("ReassignedGlobalCalls", source);
        var result = Execute(artifact, "ReassignedGlobalCalls");
        Assert.Contains("fake-object fake-array fake-parseInt", result.Output);
    }

    [Fact]
    public void DiscardedCallsObserveReplacement()
    {
        const string source = """
            globalThis.Number = function () { console.log("number called"); };
            globalThis.String = function () { console.log("string called"); };
            globalThis.Symbol = function () { console.log("symbol called"); };
            Number();
            String();
            Symbol();
            """;

        var result = Execute(Compile("DiscardedGlobalCalls", source), "DiscardedGlobalCalls");
        Assert.Contains("number called", result.Output);
        Assert.Contains("string called", result.Output);
        Assert.Contains("symbol called", result.Output);
    }

    [Fact]
    public void CalleeIsSelectedBeforeArgumentsChangeBinding()
    {
        const string source = """
            console.log(typeof Object((globalThis.Object = function () { return "new"; }, 5)));
            console.log(Object());
            """;

        var result = Execute(Compile("GlobalCallArgumentOrder", source), "GlobalCallArgumentOrder");
        Assert.Contains("object", result.Output);
        Assert.Contains("new", result.Output);
    }

    [Fact]
    public void ReplacementAccessorIsEvaluatedOnEveryCall()
    {
        const string source = """
            var original = Object;
            Object.defineProperty(globalThis, "Object", {
                configurable: true,
                get: function () { console.log("getter"); return original; }
            });
            Object();
            Object();
            """;

        var result = Execute(Compile("GlobalCallAccessor", source), "GlobalCallAccessor");
        Assert.Equal(2, result.Output.Split("getter", StringSplitOptions.None).Length - 1);
    }

    [Fact]
    public void HostOverrideIsObservedEvenWithoutSourceMutation()
    {
        var host = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue(
                "parseInt",
                (Func<object?, object?, string>)((_, _) => "host"),
                overwritePolicy: RuntimeGlobalOverwritePolicy.ReplaceExisting)
            .Build();
        var artifact = Compile(
            "GlobalCallHostOverride",
            "console.log(parseInt('123'));",
            host: host);

        Assert.Contains("host", Execute(artifact, "GlobalCallHostOverride", host).Output);
    }

    [Fact]
    public void ReassignedGlobalConstructorUsesCurrentBinding()
    {
        const string source = """
            globalThis.Array = function () { this.marker = "replacement"; };
            console.log(new Array().marker);
            """;

        var result = Execute(Compile("ReassignedGlobalConstructor", source), "ReassignedGlobalConstructor");
        Assert.Contains("replacement", result.Output);
    }

    [Fact]
    public void ConstructorCalleeIsSelectedBeforeArgumentsChangeBinding()
    {
        const string source = """
            var original = new Array((globalThis.Array = function () { this.marker = "replacement"; }, 2));
            console.log(original.length, new Array().marker);
            """;

        var result = Execute(Compile("GlobalConstructorArgumentOrder", source), "GlobalConstructorArgumentOrder");
        Assert.Contains("2 replacement", result.Output);
    }

    [Fact]
    public void ReassignedGlobalStaticMethodUsesCurrentReceiver()
    {
        const string source = """
            globalThis.Array = { isArray: function () { return "replacement"; } };
            console.log(Array.isArray([]));
            """;

        var result = Execute(Compile("ReassignedGlobalStaticMethod", source), "ReassignedGlobalStaticMethod");
        Assert.Contains("replacement", result.Output);
    }

    [Fact]
    public void ReassignedGlobalToNonCallableThrows()
    {
        const string source = """
            globalThis.Number = 3;
            try { Number(1); } catch (error) { console.log(error.name); }
            """;

        var result = Execute(Compile("NonCallableGlobalBinding", source), "NonCallableGlobalBinding");
        Assert.Contains("TypeError", result.Output);
    }

    [Fact]
    public void ComputedGlobalWritesAndCallbackMutationsAreObserved()
    {
        const string source = """
            function change() { globalThis["Number"] = function () { return "changed"; }; }
            change();
            console.log(Number(1));
            """;

        var result = Execute(Compile("ComputedGlobalWrite", source), "ComputedGlobalWrite");
        Assert.Contains("changed", result.Output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BigIntLiteralsIgnoreReplacedGlobalCallable(bool assumeUnmodifiedHostGlobals)
    {
        const string source = """
            globalThis.BigInt = function () { return 9; };
            console.log(2n + 3n, BigInt(1));
            """;

        var result = Execute(
            Compile("BigIntLiteralAndReplacedGlobal", source, assumeUnmodifiedHostGlobals),
            "BigIntLiteralAndReplacedGlobal");
        Assert.Contains("5 9", result.Output);
    }

    [Fact]
    public void WithObjectCallShadowsGlobalIntrinsic()
    {
        const string source = """
            with ({ Array: function () { return "shadow"; } }) {
                console.log(Array());
            }
            """;

        var result = Execute(Compile("WithGlobalCall", source), "WithGlobalCall");
        Assert.Contains("shadow", result.Output);
    }

    [Fact]
    public void OptInEmitsGuardFreeCallsOnlyWhenSourceIsProvenSafe()
    {
        var clean = Compile(
            "GlobalCallProvenSafe",
            "globalThis.unrelated = 1; console.log(Number('123'));",
            assumeUnmodifiedHostGlobals: true);
        Assert.DoesNotContain(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(clean.PeBytes, clean.AssemblyName));
        Assert.Contains("123", Execute(clean, "GlobalCallProvenSafe").Output);

        var mutable = Compile(
            "GlobalCallProvenMutable",
            "globalThis.Number = function () { return 'changed'; }; console.log(Number('123'));",
            assumeUnmodifiedHostGlobals: true);
        Assert.Contains(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(mutable.PeBytes, mutable.AssemblyName));
        Assert.Contains("changed", Execute(mutable, "GlobalCallProvenMutable").Output);

        var computed = Compile(
            "GlobalCallComputedWrite",
            "globalThis['Number'] = function () { return 'computed'; }; console.log(Number(1));",
            assumeUnmodifiedHostGlobals: true);
        Assert.Contains(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(computed.PeBytes, computed.AssemblyName));
        Assert.Contains("computed", Execute(computed, "GlobalCallComputedWrite").Output);
    }

    [Fact]
    public void OptInKeepsGuardsForGlobalObjectAliases()
    {
        var alias = Compile(
            "GlobalCallAliasedGlobal",
            "var alias = globalThis; alias.Number = function () { return 'aliased'; }; console.log(Number('123'));",
            assumeUnmodifiedHostGlobals: true);
        Assert.Contains(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(alias.PeBytes, alias.AssemblyName));
        Assert.Contains("aliased", Execute(alias, "GlobalCallAliasedGlobal").Output);
    }

    [Fact]
    public void OptInConsidersAllModulesAndHostOverrides()
    {
        var host = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue(
                "parseInt",
                (Func<object?, object?, string>)((_, _) => "host"),
                overwritePolicy: RuntimeGlobalOverwritePolicy.ReplaceExisting)
            .Build();
        var hostArtifact = Compile(
            "GlobalCallHostOverrideOptIn",
            "console.log(parseInt('123'));",
            assumeUnmodifiedHostGlobals: true,
            host: host);
        Assert.Contains("host", Execute(hostArtifact, "GlobalCallHostOverrideOptIn", host).Output);
        Assert.Contains(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(hostArtifact.PeBytes, hostArtifact.AssemblyName));

        var directory = Path.Combine(Path.GetTempPath(), "GlobalCallMultiEntry");
        var first = Path.Combine(directory, "first.js");
        var second = Path.Combine(directory, "second.js");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new JrocInMemoryEntrySource(first, "globalThis.Number = function () { return 'changed'; };"),
                new JrocInMemoryEntrySource(second, "console.log(Number('123'));")
            ])
        {
            AssemblyName = "GlobalCallMultiEntry",
            AssumeUnmodifiedHostGlobals = true
        });
        Assert.Contains(
            "IsOriginalGlobalBinding",
            AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
    }

    private static JrocCompiledAssemblyArtifact Compile(
        string name,
        string source,
        bool assumeUnmodifiedHostGlobals = false,
        HostRuntimeIntrinsicDescriptors? host = null)
        => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), name + ".js"))
        {
            AssemblyName = name,
            SourceText = source,
            AssumeUnmodifiedHostGlobals = assumeUnmodifiedHostGlobals,
            HostRuntimeIntrinsics = host ?? HostRuntimeIntrinsicDescriptors.Empty
        });

    private static InMemoryTestExecutionResult Execute(
        JrocCompiledAssemblyArtifact artifact,
        string name,
        HostRuntimeIntrinsicDescriptors? host = null)
        => InMemoryTestCompiler.ExecuteArtifact(
            artifact,
            Path.Combine(Path.GetTempPath(), name + ".js"),
            name,
            hostRuntimeIntrinsics: host);
}
