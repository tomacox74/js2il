using Js2IL.IR;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Diagnostics;
using Xunit;

namespace Js2IL.Tests;

[Collection("IRPipelineMetrics")]
public class IRPipelineParentScopeTests
{
    [Fact]
    public void ClosureCapturedVariable_ReadWrite_Works_WithIRPipelineEnabled()
    {
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();

        try
        {
            var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "IRPipelineParentScope");
            Directory.CreateDirectory(outputPath);

            var js = """
                function makeCounter() {
                    var x = 0;
                    function inc() {
                        x = x + 1;
                        return x;
                    }
                    console.log(inc());
                    console.log(inc());
                }
                makeCounter();
                """;

            var testFilePath = Path.Combine(outputPath, "parent-scope.js");
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(testFilePath, js);

            var options = new CompilerOptions
            {
                OutputDirectory = outputPath
            };

            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, new TestLogger());
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            Assert.True(compiler.Compile(testFilePath));

            // Execute out-of-proc to validate closure semantics (captured var read/write across calls).
            var dllPath = Path.Combine(outputPath, "parent-scope.dll");
            Assert.True(File.Exists(dllPath), $"Expected output assembly at {dllPath}");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = dllPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            Assert.NotNull(proc);
            Assert.True(proc!.WaitForExit(30000));
            var stdOut = proc.StandardOutput.ReadToEnd();
            var stdErr = proc.StandardError.ReadToEnd();
            Assert.True(proc.ExitCode == 0, $"dotnet failed: {stdErr}\nSTDOUT:\n{stdOut}");

            var normalized = stdOut.Replace("\r\n", "\n").Trim();
            Assert.Equal("1\n2", normalized);
        }
        finally
        {
            IRPipelineMetrics.Enabled = false;
        }
    }

    [Fact]
    public void ClosureCapturedVariable_UpdateExpressions_Work_WithIRPipelineEnabled()
    {
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();

        try
        {
            var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "IRPipelineParentScope");
            Directory.CreateDirectory(outputPath);

            var js = """
                function makeCounter() {
                    var x = 0;
                    function f() {
                        console.log(x++);
                        console.log(++x);
                        console.log(x--);
                        console.log(--x);
                    }
                    f();
                    f();
                }
                makeCounter();
                """;

            var testFilePath = Path.Combine(outputPath, "parent-scope-updateexpr.js");
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(testFilePath, js);

            var options = new CompilerOptions
            {
                OutputDirectory = outputPath
            };

            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, new TestLogger());
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            Assert.True(compiler.Compile(testFilePath));

            var dllPath = Path.Combine(outputPath, "parent-scope-updateexpr.dll");
            Assert.True(File.Exists(dllPath), $"Expected output assembly at {dllPath}");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = dllPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            Assert.NotNull(proc);
            Assert.True(proc!.WaitForExit(30000));
            var stdOut = proc.StandardOutput.ReadToEnd();
            var stdErr = proc.StandardError.ReadToEnd();
            Assert.True(proc.ExitCode == 0, $"dotnet failed: {stdErr}\nSTDOUT:\n{stdOut}");

            var normalized = stdOut.Replace("\r\n", "\n").Trim();
            Assert.Equal("0\n2\n2\n0\n0\n2\n2\n0", normalized);
        }
        finally
        {
            IRPipelineMetrics.Enabled = false;
        }
    }
}
