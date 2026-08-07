using Jroc.IR;

namespace Jroc.Tests;

/// <summary>
/// Shared compilation logic for test assemblies.
/// </summary>
public static class TestCompiler
{
    /// <summary>
    /// Compiles JavaScript test code to an in-memory assembly artifact.
    /// </summary>
    /// <param name="writeArtifacts">
    /// Overrides <c>JROC_WRITE_TEST_ARTIFACTS</c> when specified. Materialization is intended
    /// only for explicit disk-output coverage and external inspection tools.
    /// </param>
    public static CompiledAssembly Compile(
        string testName,
        string testCategory,
        string outputDirectory,
        Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath,
        string[]? additionalScripts,
        bool enableIRMetrics = false,
        bool? writeArtifacts = null)
    {
        var (script, sourcePath) = getJavaScriptAndSourcePath(testName);
        var logicalTestPath = Path.Combine(outputDirectory, $"{testName}.js");
        var entryPath = sourcePath ?? logicalTestPath;

        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, script, sourcePath);
        var additionalScriptPaths = new List<string>();

        if (additionalScripts is not null)
        {
            foreach (var scriptName in additionalScripts)
            {
                var (additionalScript, additionalSourcePath) = getJavaScriptAndSourcePath(scriptName);
                var additionalPath = additionalSourcePath
                    ?? Path.Combine(outputDirectory, $"{scriptName}.js");

                fileSystem.AddFile(additionalPath, additionalScript, additionalSourcePath);
                additionalScriptPaths.Add(Path.Combine(outputDirectory, $"{scriptName}.js"));
            }
        }

        var testLogger = new TestLogger();
        var previousMetricsEnabled = false;
        if (enableIRMetrics)
        {
            previousMetricsEnabled = IRPipelineMetrics.Enabled;
            IRPipelineMetrics.Enabled = true;
            IRPipelineMetrics.Reset();
        }

        try
        {
            var artifact = JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    FileSystem = fileSystem,
                    EmitPdb = true
                },
                testLogger);

            var materializedArtifact = (writeArtifacts ?? TestArtifactOutput.IsEnabled)
                ? artifact.Materialize(outputDirectory)
                : null;

            return new CompiledAssembly(
                artifact,
                logicalTestPath,
                additionalScriptPaths,
                outputDirectory,
                materializedArtifact);
        }
        catch (InvalidOperationException ex)
        {
            var details = string.IsNullOrWhiteSpace(testLogger.Errors)
                ? string.Empty
                : $"\nErrors:\n{testLogger.Errors}";
            var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                ? string.Empty
                : $"\nWarnings:\n{testLogger.Warnings}";
            var lastFailure = enableIRMetrics ? IRPipelineMetrics.GetLastFailure() : null;
            var failureDetails = string.IsNullOrWhiteSpace(lastFailure)
                ? string.Empty
                : $"\nIR failure: {lastFailure}";

            throw new InvalidOperationException(
                $"Compilation failed for test {testName}.{failureDetails}{details}{warnings}",
                ex);
        }
        finally
        {
            if (enableIRMetrics)
            {
                IRPipelineMetrics.Enabled = previousMetricsEnabled;
            }
        }
    }
}
