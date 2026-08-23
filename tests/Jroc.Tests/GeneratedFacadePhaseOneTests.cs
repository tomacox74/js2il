using System.Reflection;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseOneTests
{
    [Fact]
    public void FacadeTypes_ArePublicNestedStaticAndUseOnlyBclSignatures()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./api.js'); require('./api/css.js');",
            "FacadeShape",
            new Dictionary<string, string>
            {
                ["api.js"] = "module.exports = 1;",
                ["api/css.js"] = "module.exports = 2;"
            },
            entryFileName: "hello.js");
        using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);

        var root = loaded.Assembly.GetType("FacadeShape", throwOnError: true)!;
        var scripts = root.GetNestedType("Scripts", BindingFlags.Public)!;
        var hello = scripts.GetNestedType("hello", BindingFlags.Public)!;
        var api = scripts.GetNestedType("api", BindingFlags.Public)!;
        var css = api.GetNestedType("css", BindingFlags.Public)!;

        foreach (var type in new[] { root, scripts, hello, api, css })
        {
            Assert.True(type.IsAbstract && type.IsSealed);
            Assert.Equal(typeof(object), type.BaseType);
        }

        Assert.True(root.IsPublic);
        Assert.All(new[] { scripts, hello, api, css }, type => Assert.True(type.IsNestedPublic));

        foreach (var type in new[] { root, hello })
        {
            AssertRunSignature(Assert.Single(
                type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)));
        }

        foreach (var type in new[] { api, css })
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            AssertRunSignature(Assert.Single(methods, method => method.Name == "Run"));
            Assert.NotNull(Assert.Single(methods, method => method.Name == "Import").ReturnType);
        }

        Assert.Empty(
            scripts.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly));
    }

    [Fact]
    public void FacadeTypes_AreDeterministicAcrossCompilations()
    {
        static string[] GetFacadeTypes()
        {
            using var harness = new GeneratedAssemblyConsumerHarness(
                "require('./api.js'); require('./api/css.js');",
                "DeterministicFacade",
                new Dictionary<string, string>
                {
                    ["api.js"] = "module.exports = 1;",
                    ["api/css.js"] = "module.exports = 2;"
                });
            using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);
            return loaded.Assembly
                .GetTypes()
                .Where(type =>
                    type.FullName?.StartsWith("DeterministicFacade", StringComparison.Ordinal) == true)
                .Select(type => type.FullName!)
                .Order(StringComparer.Ordinal)
                .ToArray();
        }

        Assert.Equal(GetFacadeTypes(), GetFacadeTypes());
    }

    [Fact]
    public void CSharpConsumer_CallsEntryAndExplicitScriptRun()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "console.log('hello');",
            "HelloAssembly",
            entryFileName: "hello.js");

        var result = harness.Build(
            """
            HelloAssembly.Run();
            HelloAssembly.Scripts.hello.Run();
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["hello", "hello"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void DeepPathsAndModuleDirectoryDuality_CompileAndRun()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./api.js'); require('./api/css.js');",
            "DeepAssembly",
            new Dictionary<string, string>
            {
                ["api.js"] = "if (require.main === module) console.log('api');",
                ["api/css.js"] = "if (require.main === module) console.log('css');"
            });

        var result = harness.Build(
            """
            DeepAssembly.Scripts.api.Run();
            DeepAssembly.Scripts.api.css.Run();
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["api", "css"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void RepeatedModulePathSegments_CompileAndRunFromCSharp()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./api/api.js');",
            "RepeatedPathAssembly",
            new Dictionary<string, string>
            {
                ["api/api.js"] =
                    "if (require.main === module) console.log('repeated');"
            });

        var result = harness.Build(
            "RepeatedPathAssembly.Scripts.api.api.Run();",
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["repeated"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ScopedPackageFacade_UsesPackageRelativeNestedPaths()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./api/css.js'); if (require.main === module) console.log('package');",
            "scope.pkg",
            new Dictionary<string, string>
            {
                ["node_modules/@scope/pkg/api/css.js"] =
                    "if (require.main === module) console.log('css');"
            },
            entryFileName: "node_modules/@scope/pkg/index.js");

        var result = harness.Build(
            """
            scope_pkg.Run();
            scope_pkg.Scripts.index.Run();
            scope_pkg.Scripts.api.css.Run();
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["package", "package", "css"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void RunArguments_MapExactlyToProcessArgvAndSelectedModule()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            const dependency = require('./dependency.js');
            console.log(JSON.stringify(dependency.argv));
            console.log(JSON.stringify(process.argv));
            """,
            "ArgsAssembly",
            new Dictionary<string, string>
            {
                ["dependency.js"] =
                    """
                    module.exports.argv = process.argv;
                    if (require.main === module) console.log(JSON.stringify(process.argv));
                    """
            });

        var result = harness.Build(
            """
            ArgsAssembly.Run("--mode", "test", "", "hello world", "Ω", "--flag");
            ArgsAssembly.Scripts.dependency.Run("duplicate", "duplicate");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                """["jroc","entry","--mode","test","","hello world","Ω","--flag"]""",
                """["jroc","entry","--mode","test","","hello world","Ω","--flag"]""",
                """["jroc","dependency","duplicate","duplicate"]"""
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ProcessArgvMutation_DoesNotLeakAcrossIsolatedRuns()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            process.argv.push("mutated");
            console.log(JSON.stringify(process.argv));
            """,
            "ArgvIsolationAssembly");

        var result = harness.Build(
            """
            ArgvIsolationAssembly.Run("value");
            ArgvIsolationAssembly.Run("value");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                """["jroc","entry","value","mutated"]""",
                """["jroc","entry","value","mutated"]"""
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ProgramMain_ForwardsArgumentsThroughRootRun()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "console.log(JSON.stringify(process.argv));",
            "ProgramMainAssembly");

        var result = harness.Build(
            """
            using System.Reflection;

            ProgramMainAssembly.Run("one", "");
            typeof(ProgramMainAssembly).Assembly.EntryPoint!.Invoke(
                null,
                new object?[] { new[] { "one", "" } });
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                """["jroc","entry","one",""]""",
                """["jroc","entry","one",""]"""
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ProgramNamedFacade_DoesNotCollideWithGeneratedEntryPointType()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "console.log('program facade');",
            "Program");

        var result = harness.Build(
            """
            internal static class HostEntryPoint
            {
                public static void Main()
                {
                    global::Program.Run();
                }
            }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["program facade"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void Run_RemainsAvailableWhenExportsExistAndIgnoresExports()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "module.exports = { answer: 42 }; console.log('ran exports module');",
            "ExportsAssembly");

        var result = harness.Build("ExportsAssembly.Run();", run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["ran exports module"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void Run_RejectsNullArraysAndElementsBeforeJavaScriptExecution()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "console.log('ran');",
            "ArgumentValidationAssembly");

        var result = harness.Build(
            """
            try { ArgumentValidationAssembly.Run(null!); }
            catch (ArgumentNullException) { Console.WriteLine("null-array"); }

            try { ArgumentValidationAssembly.Run(new string[] { "ok", null! }); }
            catch (ArgumentException) { Console.WriteLine("null-element"); }

            ArgumentValidationAssembly.Run("valid");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["null-array", "null-element", "ran"],
            OutputLines(result.RunStandardOutput));
    }

    [Theory]
    [InlineData("throw new Error('boom');", "boom")]
    [InlineData("throw 42;", "non-error")]
    [InlineData("setTimeout(() => { throw new Error('timer boom'); }, 0);", "timer boom")]
    [InlineData("Promise.reject(new Error('unhandled rejection'));", "unhandled rejection")]
    [InlineData("await Promise.reject(new Error('top-level await boom'));", "top-level await boom")]
    public void Run_TranslatesJavaScriptAndEventLoopFailures(
        string javaScript,
        string expectedMessage)
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            javaScript,
            "FailureAssembly");

        var result = harness.Build(
            """
            try
            {
                FailureAssembly.Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.InnerException?.GetType().Name);
                Console.WriteLine(exception.InnerException?.Message);
            }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        var lines = OutputLines(result.RunStandardOutput);
        Assert.Equal("JsScriptRunException", lines[0]);
        Assert.Equal("JsErrorException", lines[1]);
        Assert.Contains(expectedMessage, lines[2], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Run_TranslatesDependencyEvaluationFailure()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./dependency.js');",
            "DependencyFailureAssembly",
            new Dictionary<string, string>
            {
                ["dependency.js"] = "throw new Error('dependency boom');"
            });

        var result = harness.Build(
            """
            try { DependencyFailureAssembly.Run(); }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException?.Message);
                Console.WriteLine(
                    exception.InnerException?.GetType().GetProperty("JsStack")
                        ?.GetValue(exception.InnerException));
            }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Contains("JsScriptRunException", result.RunStandardOutput);
        Assert.Contains("entry", result.RunStandardOutput);
        Assert.Contains("dependency boom", result.RunStandardOutput);
        Assert.Contains("dependency.js", result.RunStandardOutput);
    }

    [Fact]
    public void Run_DrainsSuccessfulAsyncWorkAndHandledRejections()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            Promise.reject(new Error("handled")).catch(() => console.log("handled"));
            Promise.resolve().then(() => console.log("microtask"));
            setImmediate(() => console.log("immediate"));
            setTimeout(() => console.log("timer"), 0);
            """,
            "AsyncDrainAssembly");

        var result = harness.Build("AsyncDrainAssembly.Run();", run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["handled", "microtask", "immediate", "timer"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ProcessExitZero_StopsExecutionWithoutTerminatingHost()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            console.log("before");
            setTimeout(() => console.log("leaked-timer"), 25);
            process.exit(0);
            console.log("after");
            """,
            "ExitZeroAssembly");

        var result = harness.Build(
            """
            ExitZeroAssembly.Run();
            Thread.Sleep(100);
            Console.WriteLine("host-alive");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["before", "host-alive"], OutputLines(result.RunStandardOutput));
    }

    [Theory]
    [InlineData("process.exit(7);", "7")]
    [InlineData("process.exitCode = 9;", "9")]
    public void NonzeroProcessExit_BecomesHostFailure(
        string javaScript,
        string expectedExitCode)
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            javaScript,
            "ExitFailureAssembly");

        var result = harness.Build(
            """
            try { ExitFailureAssembly.Run(); }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.GetType().GetProperty("ExitCode")?.GetValue(exception));
            }
            Console.WriteLine("host-alive");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["JsScriptRunException", expectedExitCode, "host-alive"],
            OutputLines(result.RunStandardOutput));
    }

    [Theory]
    [InlineData(
        """
        try { process.exit(0); }
        catch { console.log("caught"); }
        console.log("after");
        """)]
    [InlineData(
        """
        (async () => {
            try {
                await Promise.resolve();
                process.exit(0);
            } catch {
                console.log("caught");
            }
        })();
        setTimeout(() => console.log("after"), 25);
        """)]
    [InlineData(
        """
        setTimeout(() => process.exit(0), 0);
        setTimeout(() => console.log("after"), 25);
        """)]
    [InlineData(
        """
        try { process.exit(0); }
        finally { console.log("finally"); }
        """)]
    [InlineData(
        """
        (async () => {
            try {
                await Promise.resolve();
                process.exit(0);
            } finally {
                console.log("finally");
            }
        })();
        setTimeout(() => console.log("after"), 25);
        """)]
    [InlineData(
        """
        require("assert").throws(() => process.exit(0));
        console.log("after");
        """)]
    public void ProcessExit_CannotBeCaughtByJavaScript(string javaScript)
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            javaScript,
            "UncatchableExitAssembly");

        var result = harness.Build(
            """
            UncatchableExitAssembly.Run();
            Console.WriteLine("host-alive");
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["host-alive"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void RepeatedAndConcurrentRuns_UseIsolatedRuntimeState()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            globalThis.runCount = (globalThis.runCount || 0) + 1;
            console.log(runCount);
            """,
            "IsolationAssembly");

        var result = harness.Build(
            """
            IsolationAssembly.Run();
            IsolationAssembly.Run();
            Parallel.For(0, 4, _ => IsolationAssembly.Run());
            """,
            run: true);

        AssertConsumerSucceeded(result);
        var lines = OutputLines(result.RunStandardOutput);
        Assert.Equal(6, lines.Length);
        Assert.All(lines, line => Assert.Equal("1", line));
    }

    [Fact]
    public void NamingPlanner_RejectsRunAndFutureImportMemberCollisions()
    {
        foreach (var reservedName in new[] { "Run", "Import" })
        {
            var exception = Assert.Throws<JrocFacadeNameCollisionException>(
                () => JrocFacadeNamePlanner.Create(
                    "ReservedMemberAssembly",
                    "api.js",
                    ["api.js", $"api/{reservedName}.js"]));

            Assert.Contains(reservedName, exception.ProposedClrPath);
        }
    }

    [Theory]
    [InlineData("RunAssembly", "Run.js", "RunAssembly.Scripts.Run")]
    [InlineData("ImportAssembly", "Import.js", "ImportAssembly.Scripts.Import")]
    [InlineData("ScriptsAssembly", "Scripts.js", "ScriptsAssembly.Scripts.Scripts")]
    public void NamingPlanner_RejectsCSharpTypeMemberNameCollisions(
        string assemblyName,
        string moduleId,
        string expectedPath)
    {
        var exception = Assert.Throws<JrocFacadeNameCollisionException>(
            () => JrocFacadeNamePlanner.Create(
                assemblyName,
                moduleId,
                [moduleId]));

        Assert.Equal(expectedPath, exception.ProposedClrPath);
    }

    [Theory]
    [InlineData("Run")]
    [InlineData("Import")]
    [InlineData("Scripts")]
    public void NamingPlanner_RejectsAssemblyFacadeMemberNameCollisions(
        string assemblyName)
    {
        var exception = Assert.Throws<JrocFacadeNameCollisionException>(
            () => JrocFacadeNamePlanner.Create(
                assemblyName,
                "entry",
                ["entry"]));

        Assert.Equal(assemblyName, exception.ProposedClrPath);
    }

    private static void AssertConsumerSucceeded(GeneratedAssemblyConsumerResult result)
    {
        Assert.True(
            result.BuildExitCode == 0,
            $"Consumer build failed.{Environment.NewLine}{result.BuildDiagnostics}");
        Assert.True(
            result.RunExitCode == 0,
            $"Consumer run failed.{Environment.NewLine}" +
            $"{result.RunStandardOutput}{Environment.NewLine}{result.RunStandardError}");
    }

    private static string[] OutputLines(string output) =>
        output.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

    private static void AssertRunSignature(MethodInfo run)
    {
        Assert.Equal("Run", run.Name);
        Assert.Equal(typeof(void), run.ReturnType);
        var parameter = Assert.Single(run.GetParameters());
        Assert.Equal(typeof(string[]), parameter.ParameterType);
        Assert.NotNull(parameter.GetCustomAttribute<ParamArrayAttribute>());
    }
}
