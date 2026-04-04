using Js2IL.Services;
using JavaScriptRuntime;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.Loader;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class JavaScriptErrorStackTraceTests
{
    [Fact]
    public void ThrownError_HasSourceMappedStackTrace()
    {
        var baseName = "stacktrace_" + Guid.NewGuid().ToString("N");
        var jsFileName = baseName + ".js";
        var js = "\"use strict\";\n" +
                 "function inner() {\n" +
                 "  throw new Error(\"boom\");\n" +
                 "}\n" +
                 "function outer() { inner(); }\n" +
                 "try { outer(); } catch (e) { console.log(e.stack); }\n";

        var output = CompileAndRunWithEmitPdb(
            "JavaScriptErrorStackTrace",
            jsFileName,
            new Dictionary<string, string>
            {
                [jsFileName] = js
            });

        // The throw is on line 3 in stacktrace.js.
        Assert.Contains($"{jsFileName}:line 3", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ThrownError_InRewrittenModuleTopLevelCode_HasOriginalSourceMappedStackTrace()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var rootFileName = $"root_{suffix}.mjs";
        var depFileName = $"dep_{suffix}.mjs";
        var rootJs = "\"use strict\";\n" +
                     $"import defaultValue, {{ namedValue as renamed }} from \"./{depFileName}\";\n" +
                     "export { renamed as exportedRenamed };\n" +
                     "try {\n" +
                     "  throw new Error(String(defaultValue + renamed));\n" +
                     "} catch (e) {\n" +
                     "  console.log(e.stack);\n" +
                     "}\n";
        var depJs = "\"use strict\";\n" +
                    "export default 40;\n" +
                    "export const namedValue = 2;\n";

        var output = CompileAndRunWithEmitPdb(
            "JavaScriptModuleTopLevelStackTrace",
            rootFileName,
            new Dictionary<string, string>
            {
                [rootFileName] = rootJs,
                [depFileName] = depJs
            });

        Assert.Contains($"{rootFileName}:line 4", output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("rewritten.js", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ThrownError_InRewrittenModuleNestedCallable_HasOriginalSourceMappedStackTrace()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var rootFileName = $"root_{suffix}.mjs";
        var depFileName = $"dep_{suffix}.mjs";
        var rootJs = "\"use strict\";\n" +
                     $"import defaultValue, {{ namedValue as renamed }} from \"./{depFileName}\";\n" +
                     "export function explode() {\n" +
                     "  throw new Error(String(defaultValue + renamed));\n" +
                     "}\n" +
                     "try {\n" +
                     "  explode();\n" +
                     "} catch (e) {\n" +
                     "  console.log(e.stack);\n" +
                     "}\n";
        var depJs = "\"use strict\";\n" +
                    "export default 40;\n" +
                    "export const namedValue = 2;\n";

        var output = CompileAndRunWithEmitPdb(
            "JavaScriptModuleNestedStackTrace",
            rootFileName,
            new Dictionary<string, string>
            {
                [rootFileName] = rootJs,
                [depFileName] = depJs
            });

        Assert.Contains($"{rootFileName}:line 4", output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("rewritten.js", output, StringComparison.OrdinalIgnoreCase);
    }

    private static string CompileAndRunWithEmitPdb(
        string scenarioName,
        string entryFileName,
        IReadOnlyDictionary<string, string> sources)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", scenarioName, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var mockFs = new MockFileSystem();
        foreach (var (relativePath, content) in sources)
        {
            var absolutePath = Path.Combine(outputPath, relativePath);
            File.WriteAllText(absolutePath, content);
            mockFs.AddFile(absolutePath, content);
        }

        var entryPath = Path.Combine(outputPath, entryFileName);
        var dllPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(entryFileName) + ".dll");

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath,
            EmitPdb = true
        };

        var logger = new TestLogger();
        using var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(entryPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");

        return NormalizeDebugOutput(RunAssemblyAndCaptureOutput(dllPath));
    }

    private static string RunAssemblyAndCaptureOutput(string dllPath)
    {
        // Run in-proc while capturing console output (similar to ExecutionTestsBase).
        var modDir = Path.GetDirectoryName(dllPath) ?? string.Empty;
        var file = dllPath;

        var captured = new CapturingConsoleOutput();
        var capturedEnvironment = new JavaScriptRuntime.CapturingEnvironment();

        JavaScriptRuntime.CommonJS.ModuleContext.SetModuleContext(modDir, file);
        JavaScriptRuntime.EnvironmentProvider.SuppressExit = true;
        var runtimeServices = JavaScriptRuntime.RuntimeServices.BuildServiceProvider();
        runtimeServices.RegisterInstance(new ConsoleOutputSinks
        {
            Output = captured,
            ErrorOutput = captured
        });
        runtimeServices.RegisterInstance<IEnvironment>(capturedEnvironment);
        JavaScriptRuntime.Engine._serviceProviderOverride.Value = runtimeServices;

        try
        {
            // Load from path so the runtime can find the adjacent PDB for source-mapped stack traces.
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
            var entryPoint = assembly.EntryPoint ?? throw new InvalidOperationException("No entry point found in the generated assembly.");
            ((Action)Delegate.CreateDelegate(typeof(Action), entryPoint))();
        }
        finally
        {
            JavaScriptRuntime.Engine._serviceProviderOverride.Value = null;
        }

        return captured.GetOutput();
    }

    private static string NormalizeDebugOutput(string output)
    {
        output = output.Replace('\\', '/');
        var temp = Path.GetTempPath().Replace('\\', '/');
        return output.Replace(temp, "{TempPath}");
    }

    private sealed class CapturingConsoleOutput : IConsoleOutput
    {
        private readonly List<string> _lines = new();

        public void WriteLine(string line) => _lines.Add(line ?? string.Empty);

        public string GetOutput()
        {
            if (_lines.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\n", _lines) + "\n";
        }
    }
}
