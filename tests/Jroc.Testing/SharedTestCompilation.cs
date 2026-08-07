using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Jroc.Tests
{
    /// <summary>
    /// Provides a shared compilation cache for test assemblies to avoid compiling
    /// the same JavaScript twice for ExecutionTests and GeneratorTests.
    /// </summary>
    internal static class SharedTestCompilation
    {
        private const int ConsumersPerCompilation = 2;
        private static readonly ConcurrentDictionary<CompilationKey, CacheEntry> _cache = new();
        private static readonly string _sharedOutputRoot;

        static SharedTestCompilation()
        {
            // Keep the historical path shape as a logical source/module root. The directory is
            // created only when JROC_WRITE_TEST_ARTIFACTS=1 materializes an artifact.
            _sharedOutputRoot = Path.Combine(Path.GetTempPath(), "Jroc.Tests");
        }

        /// <summary>
        /// Compiles a test or returns a cached compilation result.
        /// </summary>
        public static CompiledAssembly GetOrCompile(
            string testCategory,
            string testName,
            string[]? additionalScripts,
            Func<string, CompiledAssembly> compileFunc)
        {
            var keyScripts = additionalScripts?.ToArray();
            var key = new CompilationKey(testCategory, testName, keyScripts);

            // Use Lazy<T> to ensure only one thread compiles, even with concurrent access.
            var cacheEntry = _cache.GetOrAdd(key, _ => new CacheEntry(() =>
            {
                try
                {
                    // Keep a unique logical output subdirectory per test. This also becomes the
                    // materialization directory when artifact output is explicitly requested.
                    var testOutputPath = GetTestOutputPath(testCategory, testName);
                    var compiled = compileFunc(testOutputPath);
                    return new CompilationResult(compiled);
                }
                catch (Exception ex)
                {
                    return new CompilationResult(ex);
                }
            }));

            var result = cacheEntry.Result.Value;
            if (cacheEntry.RegisterConsumer() == ConsumersPerCompilation)
            {
                ((ICollection<KeyValuePair<CompilationKey, CacheEntry>>)_cache)
                    .Remove(new KeyValuePair<CompilationKey, CacheEntry>(key, cacheEntry));
            }

            if (result.Exception != null)
            {
                throw new InvalidOperationException(
                    $"Compilation failed for test {testName}",
                    result.Exception);
            }

            return result.CompiledAssembly!;
        }

        private static string GetTestOutputPath(string testCategory, string testName)
        {
            // Keep per-compilation isolation for relative filesystem behavior. Assemblies remain
            // in memory unless artifact materialization is explicitly requested.
            var runId = Guid.NewGuid().ToString("N");
            var path = Path.Combine(_sharedOutputRoot, $"{testCategory}.ExecutionTests", runId);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Clears the compilation cache. Useful for testing.
        /// </summary>
        internal static void ClearCache()
        {
            _cache.Clear();
        }

        internal static bool IsCached(
            string testCategory,
            string testName,
            string[]? additionalScripts)
            => _cache.ContainsKey(
                new CompilationKey(testCategory, testName, additionalScripts?.ToArray()));

        private record CompilationKey(string Category, string TestName, string[]? AdditionalScripts)
        {
            // Override equality to properly compare string arrays
            public virtual bool Equals(CompilationKey? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                if (Category != other.Category || TestName != other.TestName)
                    return false;

                if (AdditionalScripts == null && other.AdditionalScripts == null)
                    return true;

                if (AdditionalScripts == null || other.AdditionalScripts == null)
                    return false;

                if (AdditionalScripts.Length != other.AdditionalScripts.Length)
                    return false;

                for (int i = 0; i < AdditionalScripts.Length; i++)
                {
                    if (AdditionalScripts[i] != other.AdditionalScripts[i])
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(Category, TestName);
                if (AdditionalScripts != null)
                {
                    foreach (var script in AdditionalScripts)
                    {
                        hash = HashCode.Combine(hash, script);
                    }
                }
                return hash;
            }
        }

        private class CompilationResult
        {
            public CompiledAssembly? CompiledAssembly { get; }
            public Exception? Exception { get; }

            public CompilationResult(CompiledAssembly assembly)
            {
                CompiledAssembly = assembly;
            }

            public CompilationResult(Exception exception)
            {
                Exception = exception;
            }
        }

        private sealed class CacheEntry
        {
            private int _consumerCount;

            public CacheEntry(Func<CompilationResult> createResult)
            {
                Result = new Lazy<CompilationResult>(
                    createResult,
                    LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public Lazy<CompilationResult> Result { get; }

            public int RegisterConsumer()
                => Interlocked.Increment(ref _consumerCount);
        }
    }

    /// <summary>
    /// Represents a compiled test assembly with metadata.
    /// </summary>
    public class CompiledAssembly
    {
        public JrocCompiledAssemblyArtifact Artifact { get; }
        public JrocMaterializedAssembly? MaterializedArtifact { get; }
        public string? AssemblyPath => MaterializedArtifact?.AssemblyPath;
        public string? PdbPath => MaterializedArtifact?.PdbPath;
        public string TestFilePath { get; }
        public IReadOnlyList<string> AdditionalScriptPaths { get; }
        public string OutputDirectory { get; }

        public CompiledAssembly(
            JrocCompiledAssemblyArtifact artifact,
            string testFilePath,
            IReadOnlyList<string> additionalScriptPaths,
            string outputDirectory,
            JrocMaterializedAssembly? materializedArtifact = null)
        {
            Artifact = artifact;
            MaterializedArtifact = materializedArtifact;
            TestFilePath = testFilePath;
            AdditionalScriptPaths = additionalScriptPaths;
            OutputDirectory = outputDirectory;
        }
    }

    internal static class TestArtifactOutput
    {
        internal const string WriteArtifactsEnvironmentVariable = "JROC_WRITE_TEST_ARTIFACTS";

        internal static bool IsEnabled
            => string.Equals(
                Environment.GetEnvironmentVariable(WriteArtifactsEnvironmentVariable),
                "1",
                StringComparison.Ordinal);
    }
}
