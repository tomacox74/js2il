using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Js2IL.Tests
{
    public class CliTests
    {
        private static string GetJs2ILExecutablePath()
        {
            // Find the js2il executable in the build output
            var asmLocation = typeof(Js2IL.Services.AssemblyGenerator).Assembly.Location;
            var binDir = Path.GetDirectoryName(asmLocation)!;
            var js2ilExe = Path.Combine(binDir, "js2il.exe");
            var js2ilDll = Path.Combine(binDir, "js2il.dll");
            
            // On Windows use .exe, otherwise use dotnet js2il.dll
            if (File.Exists(js2ilExe))
                return js2ilExe;
            if (File.Exists(js2ilDll))
                return js2ilDll;
            
            throw new FileNotFoundException($"js2il executable not found in {binDir}");
        }

        private static (int ExitCode, string StdOut, string StdErr) RunOutOfProc(params string[] args)
        {
            var exePath = GetJs2ILExecutablePath();
            var useDotnet = exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
            
            var psi = new ProcessStartInfo
            {
                FileName = useDotnet ? "dotnet" : exePath,
                Arguments = useDotnet ? $"\"{exePath}\" {string.Join(" ", args.Select(a => $"\"{a}\""))}" : string.Join(" ", args.Select(a => $"\"{a}\"")),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            return (process.ExitCode, stdout, stderr);
        }

    [Fact]
        public void Version_Prints_and_ExitCode0()
        {
            var (code, stdout, stderr) = RunOutOfProc("--version");
            Assert.Equal(0, code);
            Assert.Contains("js2il ", (stdout + stderr), StringComparison.OrdinalIgnoreCase);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

    [Fact]
        public void Help_PrintsUsage_And_ExitCode0()
        {
            var (code, stdout, stderr) = RunOutOfProc("-h");
            Assert.Equal(0, code);
            // Accept either our custom usage or PowerArgs default (which uses the host process name)
            Assert.True(stdout.Contains("Usage: js2il <InputFile>") || stdout.Contains("Usage - "), stdout);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

    [Fact]
        public void NoArgs_ShowsError_And_NonZeroExit()
        {
            var (code, stdout, stderr) = RunOutOfProc();
            Assert.NotEqual(0, code);
            Assert.Contains("InputFile is required", stderr, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Usage:", stderr, StringComparison.OrdinalIgnoreCase);
        }

    [Fact]
        public void NonexistentInput_ShowsError_And_NonZeroExit()
        {
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n") + ".js");
            var (code, stdout, stderr) = RunOutOfProc(missing);
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
                var (code, stdout, stderr) = RunOutOfProc(jsFile, "-o", outDir);

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
