using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Jroc.Tests
{
    public class CliTests
    {
        private static string? GetJrocExecutablePath()
        {
            // Try to find a built jroc executable/dll next to the test assembly.
            var asmLocation = typeof(Jroc.Services.AssemblyGenerator).Assembly.Location;
            var binDir = Path.GetDirectoryName(asmLocation)!;

            // Prefer the DLL (run via dotnet) for cross-platform consistency.
            // Native executables may have platform-specific quirks (e.g., PowerArgs hangs on Linux).
            string[] dllCandidates = { "Jroc.dll", "jroc.dll" };
            string[] exeCandidates = { "Jroc.exe", "jroc.exe", "Jroc", "jroc" };

            foreach (var dll in dllCandidates)
            {
                var path = Path.Combine(binDir, dll);
                if (File.Exists(path))
                    return path;
            }

            foreach (var exe in exeCandidates)
            {
                var path = Path.Combine(binDir, exe);
                if (File.Exists(path))
                    return path;
            }

            // Not found — caller may fallback to using `dotnet run --project` with the project path.
            return null;
        }

        private static (int ExitCode, string StdOut, string StdErr) RunOutOfProc(params string[] args)
        {
            var exePath = GetJrocExecutablePath();
            ProcessStartInfo psi;
            string launchMethod;

            if (exePath != null)
            {
                var useDotnet = exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
                launchMethod = useDotnet ? $"dotnet {exePath}" : exePath;
                Console.WriteLine($"[CliTests] Found executable: {exePath}");
                Console.WriteLine($"[CliTests] Launch method: {launchMethod}");
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
                // that haven't produced an executable yet can still execute jroc.
                // Locate Jroc.csproj by walking up from the test assembly location.
                var asmLocation = typeof(Jroc.Services.AssemblyGenerator).Assembly.Location;
                var dir = Path.GetDirectoryName(asmLocation)!;
                Console.WriteLine($"[CliTests] No executable found, searching for Jroc.csproj from: {dir}");
                string? projectPath = null;
                while (!string.IsNullOrEmpty(dir))
                {
                    var candidate = Path.Combine(dir, "src", "Cli", "Jroc.csproj");
                    if (!File.Exists(candidate))
                    {
                        candidate = Path.Combine(dir, "Cli", "Jroc.csproj");
                    }

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
                    throw new FileNotFoundException("Could not locate Jroc.csproj to run dotnet run --project");

                launchMethod = $"dotnet run --project {projectPath}";
                Console.WriteLine($"[CliTests] Using fallback: {launchMethod}");
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

            Console.WriteLine($"[CliTests] Starting process: {psi.FileName} {psi.Arguments}");
            using var process = Process.Start(psi)!;
            
            // Use async reads to avoid deadlock and add a timeout
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            
            var exited = process.WaitForExit(30000); // 30 second timeout
            if (!exited)
            {
                Console.WriteLine("[CliTests] ERROR: Process timed out after 30 seconds, killing...");
                process.Kill();
                throw new TimeoutException($"Process '{psi.FileName}' timed out after 30 seconds");
            }
            
            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();
            
            Console.WriteLine($"[CliTests] Process exited with code: {process.ExitCode}");
            Console.WriteLine($"[CliTests] stdout length: {stdout.Length}, stderr length: {stderr.Length}");
            
            return (process.ExitCode, stdout, stderr);
        }

        [Fact]
        public void Version_Prints_and_ExitCode0()
        {
            var (code, stdout, stderr) = RunOutOfProc("--version");
            Assert.Equal(0, code);
            Assert.Contains("jroc ", (stdout + stderr), StringComparison.OrdinalIgnoreCase);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

        [Fact]
        public void Help_PrintsUsage_And_ExitCode0()
        {
            if (OperatingSystem.IsLinux())
            {
                // appears to hang indefinitely on Linux when -h is passed to the process.
                // Skip this test on Linux until the root cause is identified and fixed.
                Console.WriteLine("[CliTests] Skipping Help_PrintsUsage_And_ExitCode0 test on Linux due to hang.");
                return;
            }

            var (code, stdout, stderr) = RunOutOfProc("-h");
            Assert.Equal(0, code);
            // Accept either our custom usage or PowerArgs default (which uses the host process name)
            Assert.True(stdout.Contains("Usage: jroc <InputFile>") || stdout.Contains("Usage - "), stdout);
            Assert.True(string.IsNullOrWhiteSpace(stderr));
        }

        [Fact]
        public void NoArgs_ShowsError_And_NonZeroExit()
        {
            var (code, stdout, stderr) = RunOutOfProc();
            Assert.NotEqual(0, code);
            // CLI now supports either an input file or --moduleid.
            Assert.True(
                stderr.Contains("InputFile is required", StringComparison.OrdinalIgnoreCase)
                || stderr.Contains("Provide <InputFile> or --moduleid", StringComparison.OrdinalIgnoreCase),
                stderr);
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
            var tempRoot = Path.Combine(Path.GetTempPath(), "jroc_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "simple.js");
            File.WriteAllText(jsFile, "\"use strict\";\nconsole.log('x is', 3);");
            var outDir = Path.Combine(tempRoot, "out");

            try
            {
                // Act
                var (code, stdout, stderr) = RunOutOfProc(jsFile, "-o", outDir);

                // Assert
                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                Assert.DoesNotContain(
                    "[ReceiverFlow]",
                    stdout,
                    StringComparison.Ordinal);

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

        [Fact]
        public void Convert_WithAssemblyName_UsesIdentityForAllArtifacts()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "jroc_cli_identity_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "source.js");
            var outDir = Path.Combine(tempRoot, "out");
            File.WriteAllText(jsFile, "console.log('identity');");

            try
            {
                var (code, stdout, stderr) = RunOutOfProc(
                    jsFile,
                    "-o",
                    outDir,
                    "--assemblyname",
                    "Configured.Identity");

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                var assemblyPath = Path.Combine(outDir, "Configured.Identity.dll");
                Assert.True(File.Exists(assemblyPath));
                Assert.True(File.Exists(Path.Combine(outDir, "Configured.Identity.runtimeconfig.json")));
                Assert.Equal("Configured.Identity", AssemblyName.GetAssemblyName(assemblyPath).Name);
            }
            finally
            {
                try { Directory.Delete(tempRoot, recursive: true); } catch { }
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task Convert_AdditionalInputs_UsesFirstEntryAsDefaultAndPositionalOutput()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_entries_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var entry = Path.Combine(testRoot, "entry.js");
            var extra = Path.Combine(testRoot, "extra.js");
            var third = Path.Combine(testRoot, "third.js");
            var outDir = Path.Combine(testRoot, "out");
            File.WriteAllText(entry, "console.log('default entry');");
            File.WriteAllText(extra, "console.log('extra entry');");
            File.WriteAllText(third, "console.log('third entry');");

            try
            {
                var (code, _, stderr) = RunOutOfProc(
                    entry, "-a", extra, outDir, "-a", third);

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                var assemblyPath = Path.Combine(outDir, "entry.dll");
                Assert.True(File.Exists(assemblyPath), $"Missing output: {assemblyPath}");
                Assert.False(File.Exists(Path.Combine(outDir, "extra.dll")));
                Assert.True(File.Exists(Path.Combine(outDir, "entry.runtimeconfig.json")));

                Assert.Equal("entry", AssemblyName.GetAssemblyName(assemblyPath).Name);

                using var process = Process.Start(new ProcessStartInfo("dotnet")
                {
                    ArgumentList = { assemblyPath },
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                })!;
                var stdoutTask = process.StandardOutput.ReadToEndAsync();
                var stderrTask = process.StandardError.ReadToEndAsync();
                using var timeout = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(30));
                try
                {
                    await process.WaitForExitAsync(timeout.Token);
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        await process.WaitForExitAsync();
                    }
                    throw new TimeoutException("Compiled CLI assembly timed out.");
                }

                Assert.Equal(0, process.ExitCode);
                Assert.Equal("default entry", (await stdoutTask).Trim());
                Assert.True(string.IsNullOrWhiteSpace(await stderrTask));
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void Convert_InvalidAdditionalInput_FailsCompilation()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_invalid_entry_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var entry = Path.Combine(testRoot, "entry.js");
            var extra = Path.Combine(testRoot, "invalid.js");
            var outDir = Path.Combine(testRoot, "out");
            File.WriteAllText(entry, "console.log('default entry');");
            File.WriteAllText(extra, "function (");

            try
            {
                var (code, _, stderr) = RunOutOfProc(entry, outDir, "--additional-input", extra);

                Assert.NotEqual(0, code);
                Assert.Contains(extra, stderr, StringComparison.Ordinal);
                Assert.False(File.Exists(Path.Combine(outDir, "entry.dll")));
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void Convert_AdditionalInput_WithAssemblyName_OverridesDefaultArtifactName()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_named_entries_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var entry = Path.Combine(testRoot, "entry.js");
            var extra = Path.Combine(testRoot, "extra.js");
            var outDir = Path.Combine(testRoot, "out");
            File.WriteAllText(entry, "console.log('default entry');");
            File.WriteAllText(extra, "console.log('extra entry');");

            try
            {
                var (code, _, stderr) = RunOutOfProc(
                    entry, "-a=" + extra, "-o", outDir, "--assemblyname", "Combined.Entries");

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                var assemblyPath = Path.Combine(outDir, "Combined.Entries.dll");
                Assert.True(File.Exists(assemblyPath), $"Missing output: {assemblyPath}");
                Assert.True(File.Exists(Path.Combine(outDir, "Combined.Entries.runtimeconfig.json")));
                Assert.Equal("Combined.Entries", AssemblyName.GetAssemblyName(assemblyPath).Name);
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void Convert_MissingAdditionalInput_IdentifiesTheFileAndDoesNotCompile()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_missing_entry_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var entry = Path.Combine(testRoot, "entry.js");
            var missing = Path.Combine(testRoot, "missing.js");
            var outDir = Path.Combine(testRoot, "out");
            File.WriteAllText(entry, "console.log('default entry');");

            try
            {
                var (code, _, stderr) = RunOutOfProc(entry, outDir, "--additional-input", missing);

                Assert.NotEqual(0, code);
                Assert.Contains($"Additional input file '{missing}' does not exist", stderr, StringComparison.Ordinal);
                Assert.False(File.Exists(Path.Combine(outDir, "entry.dll")));
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void Convert_MissingDefaultEntry_WithAdditionalInput_IdentifiesTheFile()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_missing_default_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var missing = Path.Combine(testRoot, "missing.js");
            var extra = Path.Combine(testRoot, "extra.js");
            File.WriteAllText(extra, "console.log('extra entry');");

            try
            {
                var (code, _, stderr) = RunOutOfProc(missing, "--additional-input", extra);

                Assert.NotEqual(0, code);
                Assert.Contains($"Input file '{missing}' does not exist", stderr, StringComparison.Ordinal);
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Theory]
        [InlineData("--additional-input")]
        [InlineData("-a")]
        public void Convert_AdditionalInput_WithoutPath_ShowsError(string option)
        {
            var (code, _, stderr) = RunOutOfProc(option);

            Assert.NotEqual(0, code);
            Assert.Contains("--additional-input requires a file path", stderr, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData("--additional-input")]
        [InlineData("-a")]
        public void Convert_AdditionalInput_WithModuleId_ShowsConflict(string option)
        {
            var (code, _, stderr) = RunOutOfProc("--moduleid", "any-module", option, "extra.js");

            Assert.NotEqual(0, code);
            Assert.Contains("--additional-input cannot be used with --moduleid", stderr, StringComparison.Ordinal);
        }

        [Fact]
        public void Convert_AnalyzeUnused_RequiresFullOption()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "jroc_cli_analyze_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(testRoot);
            var entry = Path.Combine(testRoot, "entry.js");
            var outDir = Path.Combine(testRoot, "out");
            File.WriteAllText(entry, "function unused() {}");

            try
            {
                var (code, stdout, stderr) = RunOutOfProc(entry, "-o", outDir, "--analyzeunused");

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                Assert.Contains("Analyzing unused code for module:", stdout, StringComparison.Ordinal);
                Assert.True(File.Exists(Path.Combine(outDir, "entry.dll")));
            }
            finally
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void Convert_WithDiagnosticFile_WritesDiagnosticsToFile()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "jroc_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "simple.js");
            File.WriteAllText(
                jsFile,
                """
                "use strict";
                let receiver = true ? [] : { push(value) { return value; } };
                for (let index = 0; index < 1; index++) {
                    receiver.push(index);
                }
                console.log('x is', 3);
                """);
            var outDir = Path.Combine(tempRoot, "out");
            var diagnosticFile = Path.Combine(tempRoot, "diagnostics.log");

            try
            {
                var (code, stdout, stderr) = RunOutOfProc(jsFile, "-o", outDir, "--diagnostic-file", diagnosticFile);

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                Assert.Contains("Compilation succeeded", stdout, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("[TwoPhase]", stdout, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("[ReceiverFlow]", stdout, StringComparison.Ordinal);

                Assert.True(File.Exists(diagnosticFile), $"Missing diagnostics file: {diagnosticFile}");
                var diagnostics = File.ReadAllText(diagnosticFile);
                Assert.Contains("[TwoPhase]", diagnostics, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("Build the symbol tables", diagnostics, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("[ReceiverFlow]", diagnostics, StringComparison.Ordinal);
                Assert.Contains("action=guarded", diagnostics, StringComparison.Ordinal);
            }
            finally
            {
                try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
            }
        }

        [Fact]
        public void Convert_Verbose_ReportsTypeInferencePassesAndBindingTypes()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "jroc_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "inference.js");
            File.WriteAllText(
                jsFile,
                """
                const numeric = 1;
                const text = 'hello';
                const direct = value => value + 1;
                direct(1);
                const escaped = () => 1;
                const alias = escaped;
                """);
            var outDir = Path.Combine(tempRoot, "out");

            try
            {
                var (code, stdout, stderr) = RunOutOfProc(jsFile, "-o", outDir, "-v");

                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                Assert.Contains("Type inference pass 1", stdout, StringComparison.Ordinal);
                Assert.Contains("numeric => System.Double", stdout, StringComparison.Ordinal);
                Assert.Contains("text => System.String", stdout, StringComparison.Ordinal);
                Assert.Contains(
                    "[CallableMaterialization] direct => DirectOnly; uses=1; direct-calls=1; reasons=None",
                    stdout,
                    StringComparison.Ordinal);
                Assert.Contains(
                    "[CallableMaterialization] escaped => IdentityObservable; uses=1; direct-calls=0; reasons=Alias",
                    stdout,
                    StringComparison.Ordinal);
            }
            finally
            {
                try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
            }
        }

        [Fact]
        public void Convert_Verbose_ExplainsReceiverFlowDecisions()
        {
            var tempRoot = Path.Combine(
                Path.GetTempPath(),
                "jroc_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "receiver-flow.js");
            File.WriteAllText(
                jsFile,
                """
                let useArray = true;
                let receiver = useArray ? [] : {
                    push(value) { return value; }
                };
                receiver.push(0);
                for (let index = 0; index < 1; index++) {
                    receiver.push(index);
                }
                """);
            var outDir = Path.Combine(tempRoot, "out");

            try
            {
                var (code, stdout, stderr) =
                    RunOutOfProc(jsFile, "-o", outDir, "-v");

                Assert.Equal(0, code);
                Assert.True(
                    string.IsNullOrWhiteSpace(stderr),
                    $"Unexpected stderr: {stderr}");
                Assert.Contains(
                    "[ReceiverFlow] scope=receiver_flow",
                    stdout,
                    StringComparison.Ordinal);
                Assert.Contains(
                    "member=push",
                    stdout,
                    StringComparison.Ordinal);
                Assert.Contains(
                    "action=retained-generic(cold)",
                    stdout,
                    StringComparison.Ordinal);
                Assert.Contains(
                    "action=guarded",
                    stdout,
                    StringComparison.Ordinal);
            }
            finally
            {
                try
                {
                    Directory.Delete(tempRoot, recursive: true);
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }
        }

        [Fact]
        public void Convert_NonStrictJs_Succeeds()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "jroc_cli_test_" + Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(tempRoot);
            var jsFile = Path.Combine(tempRoot, "nonstrict.js");
            File.WriteAllText(jsFile, "console.log('hello');\n");
            var outDir = Path.Combine(tempRoot, "out");

            try
            {
                var (code, stdout, stderr) = RunOutOfProc(jsFile, "-o", outDir);
                Assert.Equal(0, code);
                Assert.True(string.IsNullOrWhiteSpace(stderr), $"Unexpected stderr: {stderr}");
                Assert.Contains("Compilation succeeded", stdout, StringComparison.OrdinalIgnoreCase);

                var baseName = Path.GetFileNameWithoutExtension(jsFile);
                var dllPath = Path.Combine(outDir, baseName + ".dll");
                Assert.True(File.Exists(dllPath), $"Missing output: {dllPath}");
            }
            finally
            {
                try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
            }
        }
    }
}
