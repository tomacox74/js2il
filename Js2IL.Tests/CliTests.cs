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
        private static string? GetJs2ILExecutablePath()
        {
            // Try to find a built js2il executable/dll next to the test assembly.
            var asmLocation = typeof(Js2IL.Services.AssemblyGenerator).Assembly.Location;
            var binDir = Path.GetDirectoryName(asmLocation)!;

            // Check various naming conventions (case-sensitive on Linux):
            // - Windows: Js2IL.exe / Js2IL.dll
            // - Linux: Js2IL (no extension) / Js2IL.dll
            string[] exeCandidates = { "Js2IL.exe", "js2il.exe", "Js2IL", "js2il" };
            string[] dllCandidates = { "Js2IL.dll", "js2il.dll" };

            foreach (var exe in exeCandidates)
            {
                var path = Path.Combine(binDir, exe);
                if (File.Exists(path))
                    return path;
            }

            foreach (var dll in dllCandidates)
            {
                var path = Path.Combine(binDir, dll);
                if (File.Exists(path))
                    return path;
            }

            // Not found — caller may fallback to using `dotnet run --project` with the project path.
            return null;
        }

        private static (int ExitCode, string StdOut, string StdErr) RunOutOfProc(params string[] args)
        {
            var exePath = GetJs2ILExecutablePath();
            ProcessStartInfo psi;
            if (exePath != null)
            {
                var useDotnet = exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
                psi = new ProcessStartInfo
                {
                    FileName = useDotnet ? "dotnet" : exePath,
                    Arguments = useDotnet ? $"\"{exePath}\" {string.Join(" ", args.Select(a => $"\"{a}\""))}" : string.Join(" ", args.Select(a => $"\"{a}\"")),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else
            {
                // Fall back to running the project via `dotnet run --project <path>` so CI runners
                // that haven't produced an executable yet can still execute js2il.
                // Locate Js2IL.csproj by walking up from the test assembly location.
                var asmLocation = typeof(Js2IL.Services.AssemblyGenerator).Assembly.Location;
                var dir = Path.GetDirectoryName(asmLocation)!;
                string? projectPath = null;
                while (!string.IsNullOrEmpty(dir))
                {
                    var candidate = Path.Combine(dir, "Js2IL", "Js2IL.csproj");
                    if (File.Exists(candidate))
                    {
                        projectPath = candidate;
                        break;
                    }
                    var parent = Path.GetDirectoryName(dir);
                    if (string.IsNullOrEmpty(parent) || parent == dir) break;
                    dir = parent;
                }
                if (projectPath == null)
                    throw new FileNotFoundException("Could not locate Js2IL.csproj to run dotnet run --project");

                psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --no-build --project \"{projectPath}\" -- {string.Join(" ", args.Select(a => $"\"{a}\""))}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

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
