using Js2IL.IR;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Js2IL.Tests
{
    /// <summary>
    /// Shared compilation logic for test assemblies.
    /// </summary>
    internal static class TestCompiler
    {
        /// <summary>
        /// Compiles JavaScript test code to a .NET assembly.
        /// </summary>
        public static CompiledAssembly Compile(
            string testName,
            string testCategory,
            string outputDirectory,
            Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath,
            string[]? additionalScripts,
            bool enableIRMetrics = false)
        {
            var (js, jsSourcePath) = getJavaScriptAndSourcePath(testName);
            var testFilePath = Path.Combine(outputDirectory, $"{testName}.js");

            // Prefer a stable on-disk path (within the repo) as the logical module path.
            // This ensures the PDB "document" points at a file VS Code can open when clicking stack frames.
            var entryPathForCompilation = jsSourcePath ?? testFilePath;

            // If we couldn't resolve a stable on-disk source path, write the JS to the temp location
            // so debuggers can still open the file when the PDB points at it.
            if (entryPathForCompilation == testFilePath && !File.Exists(testFilePath))
            {
                var testDir = Path.GetDirectoryName(testFilePath);
                if (!string.IsNullOrWhiteSpace(testDir))
                {
                    Directory.CreateDirectory(testDir);
                }
                File.WriteAllText(testFilePath, js);
            }

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(entryPathForCompilation, js, jsSourcePath);

            // Add additional scripts to the mock file system
            if (additionalScripts != null)
            {
                foreach (var scriptName in additionalScripts)
                {
                    var (scriptContent, scriptSourcePath) = getJavaScriptAndSourcePath(scriptName);

                    // Same rule as the entry file: prefer the stable source path so module resolution + PDB
                    // documents stay rooted in the repo.
                    var scriptTempPath = Path.Combine(outputDirectory, $"{scriptName}.js");
                    var scriptPathForCompilation = scriptSourcePath ?? scriptTempPath;

                    if (scriptPathForCompilation == scriptTempPath && !File.Exists(scriptTempPath))
                    {
                        var scriptDir = Path.GetDirectoryName(scriptTempPath);
                        if (!string.IsNullOrWhiteSpace(scriptDir))
                        {
                            Directory.CreateDirectory(scriptDir);
                        }
                        File.WriteAllText(scriptTempPath, scriptContent);
                    }

                    mockFileSystem.AddFile(scriptPathForCompilation, scriptContent, scriptSourcePath);
                }
            }

            var options = new CompilerOptions
            {
                OutputDirectory = outputDirectory,
                EmitPdb = true
            };

            var testLogger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            bool prevMetricsEnabled = false;
            if (enableIRMetrics)
            {
                prevMetricsEnabled = IRPipelineMetrics.Enabled;
                IRPipelineMetrics.Enabled = true;
                IRPipelineMetrics.Reset();
            }

            try
            {
                if (!compiler.Compile(entryPathForCompilation))
                {
                    var details = string.IsNullOrWhiteSpace(testLogger.Errors)
                        ? string.Empty
                        : $"\nErrors:\n{testLogger.Errors}";
                    var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                        ? string.Empty
                        : $"\nWarnings:\n{testLogger.Warnings}";

                    string failureDetails = string.Empty;
                    if (enableIRMetrics)
                    {
                        var lastFailure = IRPipelineMetrics.GetLastFailure();
                        failureDetails = string.IsNullOrWhiteSpace(lastFailure)
                            ? string.Empty
                            : $"\nIR failure: {lastFailure}";
                    }

                    throw new InvalidOperationException($"Compilation failed for test {testName}.{failureDetails}{details}{warnings}");
                }
            }
            finally
            {
                if (enableIRMetrics)
                {
                    IRPipelineMetrics.Enabled = prevMetricsEnabled;
                }
            }

            // Compiler outputs <entryFileBasename>.dll into OutputDirectory.
            // For nested-path test names (e.g. "CommonJS_Require_X/a"), the DLL will be "a.dll".
            var assemblyName = Path.GetFileNameWithoutExtension(testFilePath);
            var assemblyPath = Path.Combine(outputDirectory, $"{assemblyName}.dll");
            var pdbPath = Path.Combine(outputDirectory, $"{assemblyName}.pdb");

            if (!File.Exists(assemblyPath))
            {
                throw new InvalidOperationException($"Expected assembly not found at '{assemblyPath}'.");
            }

            if (options.EmitPdb && !File.Exists(pdbPath))
            {
                throw new InvalidOperationException($"Expected PDB not found at '{pdbPath}'.");
            }

            return new CompiledAssembly(assemblyPath, pdbPath, testFilePath, outputDirectory);
        }
    }
}
