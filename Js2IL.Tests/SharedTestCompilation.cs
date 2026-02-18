using Js2IL.Services;
using Js2IL.IR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Js2IL.Tests
{
    /// <summary>
    /// Provides a shared compilation cache for test assemblies to avoid compiling
    /// the same JavaScript twice for ExecutionTests and GeneratorTests.
    /// </summary>
    internal static class SharedTestCompilation
    {
        private static readonly ConcurrentDictionary<CompilationKey, Lazy<CompilationResult>> _cache = new();
        private static readonly string _sharedOutputRoot;

        static SharedTestCompilation()
        {
            // Use a shared directory for all cached compilations in this test run
            var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            var runId = Guid.NewGuid().ToString("N");
            _sharedOutputRoot = Path.Combine(root, $"Shared", runId);
            Directory.CreateDirectory(_sharedOutputRoot);
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
            var key = new CompilationKey(testCategory, testName, additionalScripts);

            // Use Lazy<T> to ensure only one thread compiles, even with concurrent access
            var lazyResult = _cache.GetOrAdd(key, _ => new Lazy<CompilationResult>(() =>
            {
                try
                {
                    var categoryOutputPath = GetSharedOutputPath(testCategory);
                    var compiled = compileFunc(categoryOutputPath);
                    return new CompilationResult(compiled);
                }
                catch (Exception ex)
                {
                    return new CompilationResult(ex);
                }
            }, LazyThreadSafetyMode.ExecutionAndPublication));

            var result = lazyResult.Value;

            if (result.Exception != null)
            {
                throw new InvalidOperationException(
                    $"Compilation failed for test {testName}",
                    result.Exception);
            }

            return result.CompiledAssembly!;
        }

        private static string GetSharedOutputPath(string testCategory)
        {
            var path = Path.Combine(_sharedOutputRoot, testCategory);
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
    }

    /// <summary>
    /// Represents a compiled test assembly with metadata.
    /// </summary>
    internal class CompiledAssembly
    {
        public string AssemblyPath { get; }
        public string PdbPath { get; }
        public string TestFilePath { get; }
        public string OutputDirectory { get; }

        public CompiledAssembly(string assemblyPath, string pdbPath, string testFilePath, string outputDirectory)
        {
            AssemblyPath = assemblyPath;
            PdbPath = pdbPath;
            TestFilePath = testFilePath;
            OutputDirectory = outputDirectory;
        }
    }
}
