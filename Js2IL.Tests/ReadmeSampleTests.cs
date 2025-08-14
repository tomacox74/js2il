using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests
{
    public class ReadmeSampleTests
    {
        [Fact]
        public void TryIt_Sample_Works_As_Documented()
        {
            // Find repository root by locating the solution file
            var start = new DirectoryInfo(Path.GetDirectoryName(typeof(ReadmeSampleTests).Assembly.Location)!);
            DirectoryInfo? dir = start;
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "js2il.sln")))
            {
                dir = dir.Parent;
            }
            Assert.NotNull(dir);
            var repoRoot = dir!.FullName;

            // Paths used in the README
            var projectPath = Path.Combine(repoRoot, "Js2IL");
            var jsPath = Path.Combine(repoRoot, "tests", "simple.js");
            var outDir = Path.Combine(repoRoot, "out");

            // Ensure clean output folder to mirror README `.\\out`
            if (Directory.Exists(outDir))
            {
                try { Directory.Delete(outDir, recursive: true); } catch { /* ignore */ }
            }
            Directory.CreateDirectory(outDir);

            // Step 1: Compile using the exact flow shown in README
            // dotnet run --project .\\Js2IL -- .\\tests\\simple.js .\\out
            var runResult = RunProcess(
                fileName: "dotnet",
                arguments: $"run --project \"{projectPath}\" -- \"{jsPath}\" \"{outDir}\"",
                workingDirectory: repoRoot);
            Assert.True(runResult.exitCode == 0, $"dotnet run failed:\nSTDOUT:\n{runResult.stdOut}\nSTDERR:\n{runResult.stdErr}");

            var dllPath = Path.Combine(outDir, "simple.dll");
            Assert.True(File.Exists(dllPath), $"Expected generated assembly not found: {dllPath}\nSTDOUT:\n{runResult.stdOut}\nSTDERR:\n{runResult.stdErr}");

            // Step 2: Execute the generated assembly as shown in README
            // dotnet .\\out\\simple.dll
            var execResult = RunProcess(
                fileName: "dotnet",
                arguments: $"\"{dllPath}\"",
                workingDirectory: repoRoot);
            Assert.True(execResult.exitCode == 0, $"dotnet execution failed:\nSTDOUT:\n{execResult.stdOut}\nSTDERR:\n{execResult.stdErr}");

            // Expected output from tests/simple.js in README
            // "x is  3" (note the two spaces from console.log concatenation)
            var actual = (execResult.stdOut ?? string.Empty).Trim();
            Assert.Equal("x is  3", actual);

            // Cleanup to avoid polluting the workspace
            try { Directory.Delete(outDir, recursive: true); } catch { /* ignore */ }
        }

        private static (int exitCode, string stdOut, string stdErr) RunProcess(string fileName, string arguments, string workingDirectory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi)!;
            proc.WaitForExit();
            var stdOut = proc.StandardOutput.ReadToEnd();
            var stdErr = proc.StandardError.ReadToEnd();
            return (proc.ExitCode, stdOut, stdErr);
        }
    }
}
