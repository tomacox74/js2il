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
            source += "\nfunction Sampler() { var self = this; return self; }";
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
    public void NonEscapingReceiverAliasesPreserveEarlyBoundMath(bool assumeUnmodifiedHostGlobals)
    {
        const string source = """
            function Oscillator(size) {
                this.bufferSize = size;
                var self = this;
                this.generate = function(x) { return Math.round(x / self.bufferSize); };
                this.resize = function(size) { self["bufferSize"] = size; };
            }
            var oscillator = new Oscillator(2);
            console.log(oscillator.generate(3));
            oscillator.resize(4);
            console.log(oscillator.generate(3));
            """;
        var artifact = Compile(source, assumeUnmodifiedHostGlobals);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Equal(assumeUnmodifiedHostGlobals, il.Contains("Math::round(float64)", StringComparison.Ordinal));
        Assert.Equal("2\n1", Execute(artifact).Output.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void ReceiverAliasWritesInvalidateOnlyTheAffectedGlobal()
    {
        var artifact = Compile(
            """
            function change() {
                const self = (this);
                self.Number = function() { return 42; };
            }
            change();
            console.log(Number(1), Math.round(1.5));
            """,
            assumeUnmodifiedHostGlobals: true);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("Math::round(float64)", il);
        Assert.Contains("IsOriginalGlobalBinding", il);
        Assert.Equal("42 2", Execute(artifact).Output.Trim());
    }

    [Theory]
    [InlineData("member-write")]
    [InlineData("binding-write")]
    [InlineData("computed-key")]
    [InlineData("alias-chain")]
    [InlineData("shorthand-escape")]
    [InlineData("closure-return")]
    [InlineData("call-escape")]
    [InlineData("reassignment")]
    public void ReceiverAliasMutationAndEscapesKeepMathDynamic(string scenario)
    {
        var mutation = scenario switch
        {
            "member-write" => "self.Math.round = replacement;",
            "binding-write" => "self['Math'] = { round: replacement };",
            "computed-key" => "var key = 'Math'; self[key].round = replacement;",
            "alias-chain" => "var other = self; other.Math.round = replacement;",
            "shorthand-escape" => "var holder = { self }; holder.self.Math.round = replacement;",
            "closure-return" => "var leak = () => self; leak().Math.round = replacement;",
            "call-escape" => "Object.defineProperty(self, 'Math', { value: { round: replacement } });",
            "reassignment" => "self = this; self.Math.round = replacement;",
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        var artifact = Compile(
            $$"""
            function change() {
                var self = this;
                var replacement = function() { return 17; };
                {{mutation}}
            }
            change();
            console.log(Math.round(1.5));
            """,
            assumeUnmodifiedHostGlobals: true);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.DoesNotContain("Math::round(float64)", il);
        Assert.Contains("CallableOperations::Call1", il);
        Assert.Equal("17", Execute(artifact).Output.Trim());
    }

    [Theory]
    [InlineData("method-call")]
    [InlineData("computed-read")]
    [InlineData("redeclaration")]
    [InlineData("shadowed-parameter")]
    [InlineData("with")]
    public void ReceiverAliasUnknownUsesRemainConservative(string scenario)
    {
        var use = scenario switch
        {
            "method-call" => "self.read = function() { return 1; }; self.read();",
            "computed-read" => "var key = 'value'; self[key];",
            "redeclaration" => "var self;",
            "shadowed-parameter" => "function nested(self) { return self.value; }",
            "with" => "with ({}) { self.value; }",
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        var artifact = Compile(
            $$"""
            function Receiver() {
                var self = this;
                {{use}}
            }
            console.log(Math.round(1.5));
            """,
            assumeUnmodifiedHostGlobals: true);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.DoesNotContain("Math::round(float64)", il);
        Assert.Equal("2", Execute(artifact).Output.Trim());
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
            console.log("spread", Number.isNaN(Math.round(...[])), Math.round(...[1.5, 1, 2, 3, 4, 5]));
            console.log(Math.round(1.5, 1, 2), Math.round(1.5, 1, 2, 3),
                Math.round(1.5, 1, 2, 3, 4), Math.round(1.5, 1, 2, 3, 4, 5));
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
        Assert.Contains("spread true 2", output);
        Assert.Contains("2 2 2 2", output);
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CrossModuleMutationOrExposureDisablesDirectCalls(bool mutatesMath)
    {
        var directory = Path.Combine(Path.GetTempPath(), "DynamicMathModules");
        var main = Path.Combine(directory, "main.js");
        var mutation = Path.Combine(directory, "mutation.js");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(main, "require('./mutation'); console.log(Math.round(1.5));"),
                new(mutation, mutatesMath
                    ? "var alias = Math; alias['round'] = function() { return 17; };"
                    : "function Sampler() { var self = this; return self; }")
            ])
        {
            AssemblyName = "DynamicMathModules",
            AssumeUnmodifiedHostGlobals = true
        });
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("CallableOperations::Call1", il);
        Assert.DoesNotContain("Math::round(float64)", il);
        var result = InMemoryTestCompiler.ExecuteArtifact(artifact, main, "DynamicMathModules");
        Assert.Contains(mutatesMath ? "17" : "2", result.Output);
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
