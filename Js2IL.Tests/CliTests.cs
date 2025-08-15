using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Js2IL.Tests
{
    public class CliTests : IClassFixture<CliRunFixture>
    {
        private readonly CliRunFixture _fx;
        public CliTests(CliRunFixture fx) => _fx = fx;

        [Fact]
        public void Version_Prints_and_ExitCode0()
        {
            var r = _fx.RunCli("--version");
            Assert.Equal(0, r.ExitCode);
            Assert.Contains("js2il ", r.Stdout + r.Stderr);
        }

        [Fact]
        public void Help_PrintsUsage_And_ExitCode0()
        {
            var r = _fx.RunCli("-h");
            Assert.Equal(0, r.ExitCode);
            // Accept either our custom usage or PowerArgs default usage
            Assert.True(r.Stdout.Contains("Usage: js2il <InputFile>") || r.Stdout.Contains("Usage - Js2IL"), r.Stdout);
            Assert.Equal(string.Empty, r.Stderr.Trim());
        }

        [Fact]
        public void NoArgs_ShowsError_And_NonZeroExit()
        {
            var r = _fx.RunCli("");
            Assert.NotEqual(0, r.ExitCode);
            Assert.Contains("InputFile is required", r.Stderr);
        }

        [Fact]
        public void NonexistentInput_ShowsError_And_NonZeroExit()
        {
            var r = _fx.RunCli("-i does-not-exist-12345.js");
            Assert.NotEqual(0, r.ExitCode);
            Assert.Contains("does not exist", r.Stderr);
        }

        [Fact]
        public void Convert_SimpleJs_ProducesOutputs()
        {
            var root = _fx.RepoRoot;
            var input = Path.Combine(root, "tests", "simple.js");
            var outDir = Path.Combine(Path.GetTempPath(), "js2il_cli_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);

            var r = _fx.RunCli($"\"{input}\" -o \"{outDir}\"");
            try
            {
                Assert.True(r.ExitCode == 0, r.Stdout + r.Stderr);
                var asm = Path.Combine(outDir, "simple.dll");
                var rconfig = Path.Combine(outDir, "simple.runtimeconfig.json");
                var rt = Path.Combine(outDir, "JavaScriptRuntime.dll");
                Assert.True(File.Exists(asm), "Expected output assembly not found");
                Assert.True(File.Exists(rconfig), "Expected runtimeconfig not found");
                Assert.True(File.Exists(rt), "Expected runtime DLL not found");
            }
            finally
            {
                try { Directory.Delete(outDir, recursive: true); } catch { /* ignore */ }
            }
        }
    }

    public class CliRunFixture : IDisposable
    {
        public string RepoRoot { get; }
        public string Js2IlDllPath { get; }

        public CliRunFixture()
        {
            RepoRoot = FindRepoRoot() ?? throw new InvalidOperationException("Could not find repo root");
            Js2IlDllPath = GetCliDllPath();
        }

        private static string? FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "js2il.sln"))) return dir.FullName;
                dir = dir.Parent;
            }
            return null;
        }

        private string GetCliDllPath()
        {
            // Use a public type from the Js2IL assembly to locate the built DLL
            var asm = typeof(Js2IL.Services.AssemblyGenerator).Assembly;
            var path = asm.Location;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new InvalidOperationException("Could not locate Js2IL assembly path from referenced type.");
            }
            return path;
        }

        public (int ExitCode, string Stdout, string Stderr) RunCli(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = ($"\"{Js2IlDllPath}\"") + (string.IsNullOrWhiteSpace(args) ? string.Empty : " " + args),
                WorkingDirectory = RepoRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            using var p = Process.Start(psi)!;
            var stdout = p.StandardOutput.ReadToEnd();
            var stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return (p.ExitCode, stdout, stderr);
        }

        public void Dispose() { }
    }
}
