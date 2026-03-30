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
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "JavaScriptErrorStackTrace", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var baseName = "stacktrace_" + Guid.NewGuid().ToString("N");
        var jsFileName = baseName + ".js";
        var dllFileName = baseName + ".dll";

        var jsPath = Path.Combine(outputPath, jsFileName);
        var dllPath = Path.Combine(outputPath, dllFileName);

        var js = "\"use strict\";\n" +
                 "function inner() {\n" +
                 "  throw new Error(\"boom\");\n" +
                 "}\n" +
                 "function outer() { inner(); }\n" +
                 "try { outer(); } catch (e) { console.log(e.stack); }\n";

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath,
            EmitPdb = true
        };

        var logger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(jsPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");

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

        var output = captured.GetOutput();
        // Normalize for Windows paths.
        output = output.Replace('\\', '/');
        var temp = Path.GetTempPath().Replace('\\', '/');
        output = output.Replace(temp, "{TempPath}");

        // The throw is on line 3 in stacktrace.js.
        Assert.Contains($"{jsFileName}:line 3", output, StringComparison.OrdinalIgnoreCase);
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
