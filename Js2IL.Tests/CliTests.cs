using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Js2IL.Tests
{
    public class CliTests
    {
        private static (int ExitCode, string StdOut, string StdErr) RunInProc(params string[] args)
        {
            // Capture console output
            var prevOut = Console.Out;
            var prevErr = Console.Error;
            var prevExit = Environment.ExitCode;

            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            try
            {
                Console.SetOut(outWriter);
                Console.SetError(errWriter);
                Environment.ExitCode = 0;

                // Reflectively invoke Js2IL.Program.Main (non-public)
                var asm = typeof(Js2IL.Services.AssemblyGenerator).Assembly;
                var progType = asm.GetType("Js2IL.Program", throwOnError: true)!;
                var main = progType.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                Assert.NotNull(main);
                main!.Invoke(null, new object[] { args });
            }
            finally
            {
                Console.SetOut(prevOut);
                Console.SetError(prevErr);
            }

            return (Environment.ExitCode, outWriter.ToString(), errWriter.ToString());
        }

    [Fact]
        public void Version_Prints_and_ExitCode0()
        {
            var (code, stdout, stderr) = RunInProc("--version");
            Assert.Equal(0, code);
            Assert.Contains("js2il ", (stdout + stderr), StringComparison.OrdinalIgnoreCase);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

    [Fact]
        public void Help_PrintsUsage_And_ExitCode0()
        {
            var (code, stdout, stderr) = RunInProc("-h");
            Assert.Equal(0, code);
            // Accept either our custom usage or PowerArgs default (which uses the host process name)
            Assert.True(stdout.Contains("Usage: js2il <InputFile>") || stdout.Contains("Usage - "), stdout);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

    [Fact]
        public void NoArgs_ShowsError_And_NonZeroExit()
        {
            var (code, stdout, stderr) = RunInProc();
            Assert.NotEqual(0, code);
            Assert.Contains("InputFile is required", stderr, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Usage:", stderr, StringComparison.OrdinalIgnoreCase);
        }

    [Fact]
        public void NonexistentInput_ShowsError_And_NonZeroExit()
        {
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n") + ".js");
            var (code, stdout, stderr) = RunInProc(missing);
            Assert.NotEqual(0, code);
            Assert.Contains("does not exist", stderr, StringComparison.OrdinalIgnoreCase);
            Assert.True(string.IsNullOrWhiteSpace(stdout));
        }

    [Fact]
        public void Convert_SimpleJs_ProducesOutputs()
        {
            // Arrange: create simple JS file and output directory
            var tempRoot = Path.Combine(Path.GetTempPath(), "js2il_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "simple.js");
            File.WriteAllText(jsFile, "console.log('x is ', 3);");
            var outDir = Path.Combine(tempRoot, "out");

            try
            {
                // Act
                var (code, stdout, stderr) = RunInProc(jsFile, "-o", outDir);

                // Assert
                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");

                var baseName = Path.GetFileNameWithoutExtension(jsFile);
                var dllPath = Path.Combine(outDir, baseName + ".dll");
                var runtimeConfig = Path.Combine(outDir, baseName + ".runtimeconfig.json");
                var jsRuntime = Path.Combine(outDir, "JavaScriptRuntime.dll");

                Assert.True(File.Exists(dllPath), $"Missing output: {dllPath}");
                Assert.True(File.Exists(runtimeConfig), $"Missing output: {runtimeConfig}");
                Assert.True(File.Exists(jsRuntime), $"Missing runtime: {jsRuntime}");
            }
            finally
            {
                try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
            }
        }
    }
}
