using JavaScriptRuntime;
using Jroc.Services;
using Jroc.SymbolTables;
using Jroc.Tests.Utilities;

namespace Jroc.Tests.Math;

public sealed class DynamicDispatchTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void ReceiverAccessAndExposureControlMathBinding(
        bool assumeUnmodifiedHostGlobals,
        bool exposesThis)
    {
        var source = """
            function Oscillator(size) { this.bufferSize = size; }
            Oscillator.prototype.generate = function(x) {
                return Math.round(x / this.bufferSize);
            };
            console.log(new Oscillator(2).generate(3));
            """;
        if (exposesThis)
        {
            source += "\nfunction Sampler() { var self = this; }";
        }
        var module = new ModuleDefinition
        {
            Ast = new JavaScriptParser().ParseJavaScript(source, "oscillator.js"),
            Path = "oscillator.js",
            Name = "oscillator",
            ModuleId = "oscillator"
        };
        new SymbolTableBuilder().Build(module);
        Assert.Equal(exposesThis, module.SymbolTable!.Root.MayMutateGlobalCallableBindings);

        var artifact = Compile(source, assumeUnmodifiedHostGlobals);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        if (!assumeUnmodifiedHostGlobals || exposesThis)
        {
            Assert.Contains("GetGlobalBindingValue(string)", il);
            Assert.Contains("CallableOperations::Call1", il);
            Assert.DoesNotContain("Math::round(float64)", il);
        }
        else
        {
            Assert.Contains("Math::round(float64)", il);
        }
        Assert.Contains("2", Execute(artifact).Output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DynamicCallsPreserveReplacementDeletionAccessorsAndCoercion(bool assumeUnmodifiedHostGlobals)
    {
        const string source = """
            var originalMath = Math;
            var originalRound = Math.round;
            var alias = Math;
            alias["round"] = function(x) { return this === alias ? x + 10 : -1; };
            console.log(Math.round(1));
            delete Math.round;
            try { Math.round(1); } catch (e) { console.log(e.name); }
            var reads = 0;
            Object.defineProperty(Math, "round", {
                configurable: true,
                get: function() { reads++; return originalRound; }
            });
            console.log(Math.round(1.5), reads);
            Object.defineProperty(Math, "round", { configurable: true, writable: true, value: originalRound });
            var value = { valueOf: function() {
                Math.round = function() { return 42; };
                return -0.5;
            }};
            console.log(1 / Math.round(value), Math.round(1));
            function replace() { this.Math = { round: function() { return 77; } }; }
            replace();
            console.log(Math.round(1));
            globalThis.Math = originalMath;
            originalMath.round = originalRound;
            console.log(Number.isNaN(Math.round()), Math.round(1.5, "ignored"));
            console.log(Math.round(Infinity), Math.round(-Infinity), Number.isNaN(Math.round(NaN)));
            console.log(1 / Math.round(-0), Math.round(-1.5), Math.round(0.49999999999999994));
            var order = "";
            var coerced = { valueOf: function() { order += "c"; return 1.5; } };
            function extra() { order += "e"; return 0; }
            console.log(Math.round(coerced, extra()), order);
            try { Math.round({ valueOf: function() { throw new Error("coercion"); } }); }
            catch (e) { console.log(e.message); }
            """;
        var artifact = Compile(source, assumeUnmodifiedHostGlobals);
        var output = Execute(artifact).Output;
        Assert.Contains("11", output);
        Assert.Contains("TypeError", output);
        Assert.Contains("2 1", output);
        Assert.Contains("-Infinity 42", output);
        Assert.Contains("77", output);
        Assert.Contains("true 2", output);
        Assert.Contains("Infinity -Infinity true", output);
        Assert.Contains("-Infinity -1 0", output);
        Assert.Contains("2 ec", output);
        Assert.Contains("coercion", output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectsFunctionBeforeEvaluatingArguments(bool assumeUnmodifiedHostGlobals)
    {
        var artifact = Compile(
            """
            var original = Math.round;
            console.log(Math.round((Math.round = function() { return 99; }, 1.5)));
            console.log(Math.round(1.5));
            Object.defineProperty(Math, "round", {
                configurable: true,
                get: function() { console.log("get"); return original; }
            });
            function argument() { console.log("arg"); return 1.5; }
            console.log(Math.round(argument()));
            """,
            assumeUnmodifiedHostGlobals);
        var output = Execute(artifact).Output.Replace("\r\n", "\n");
        Assert.Contains("2\n99\nget\narg\n2", output);
    }

    [Fact]
    public void CrossModuleMutationDisablesDirectCallsAndRemainsObservable()
    {
        var directory = Path.Combine(Path.GetTempPath(), "DynamicMathModules");
        var main = Path.Combine(directory, "main.js");
        var mutation = Path.Combine(directory, "mutation.js");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(main, "require('./mutation'); console.log(Math.round(1.5));"),
                new(mutation, "var alias = Math; alias['round'] = function() { return 17; };")
            ])
        {
            AssemblyName = "DynamicMathModules",
            AssumeUnmodifiedHostGlobals = true
        });
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("CallableOperations::Call1", il);
        Assert.DoesNotContain("Math::round(float64)", il);
        var result = InMemoryTestCompiler.ExecuteArtifact(artifact, main, "DynamicMathModules");
        Assert.Contains("17", result.Output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReentrantHostCallbackCanMutateDynamicallyBoundMath(bool assumeUnmodifiedHostGlobals)
    {
        var request = new JrocInMemoryCompileRequest(Path.Combine(Path.GetTempPath(), "ReentrantMath.js"))
        {
            SourceText = """
                function expose() { return this; }
                exports.read = function() { return Math.round(1.5); };
                exports.run = function(callback) {
                    var before = Math.round(1.5);
                    var nested = callback();
                    return before + ":" + nested + ":" + Math.round(1.5);
                };
                """,
            AssumeUnmodifiedHostGlobals = assumeUnmodifiedHostGlobals
        };
        using var module = JrocInMemoryCompiler.CompileAndLoadModule(request);
        dynamic exports = module.Exports;
        Func<double> callback = () =>
        {
            ObjectRuntime.SetItem(GlobalThis.Math, "round", (BuiltinFunction1)((_, _) => 99d));
            return (double)exports.read();
        };
        Assert.Equal("2:99:99", (string)exports.run(callback));
    }

    private static JrocCompiledAssemblyArtifact Compile(string source, bool assumeUnmodifiedHostGlobals)
        => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), "DynamicMath.js"))
        {
            SourceText = source,
            AssemblyName = "DynamicMath",
            AssumeUnmodifiedHostGlobals = assumeUnmodifiedHostGlobals
        });

    private static InMemoryTestExecutionResult Execute(JrocCompiledAssemblyArtifact artifact)
        => InMemoryTestCompiler.ExecuteArtifact(
            artifact,
            Path.Combine(Path.GetTempPath(), "DynamicMath.js"),
            "DynamicMath");
}
