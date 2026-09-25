using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Jroc;
using Jroc.Tests;

namespace Jroc.Test262.Tests;

internal sealed class Test262FolderAssemblyCache : IDisposable
{
    private static readonly Regex RunnableFact = new(
        """\[Fact(?:\((?<options>[^\]]*)\))?\]\s*public\s+(?:async\s+)?Task\s+@?\w+\s*\([^)]*\)\s*=>\s*(?:this\.)?(?:ExecutionTest|ExecutionTestFromFile)\s*\(\s*@?"(?<name>[^"]+)""",
        RegexOptions.Compiled);

    private readonly ConcurrentDictionary<string, Lazy<FolderAssembly>> _folders =
        new(StringComparer.Ordinal);

    private int _disposed;
    private int _compilationCount;
    private int _loadCount;

    internal static Test262FolderAssemblyCache? Active { get; set; }

    internal int CompilationCount => Volatile.Read(ref _compilationCount);
    internal int LoadCount => Volatile.Read(ref _loadCount);

    internal FolderAssembly Get(string sourcePath)
    {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        var directory = new FileInfo(sourcePath).Directory
            ?? throw new ArgumentException("A fixture source directory is required.", nameof(sourcePath));
        while (!string.Equals(directory.Name, "JavaScript", StringComparison.Ordinal))
        {
            directory = directory.Parent
                ?? throw new InvalidOperationException($"Fixture '{sourcePath}' is not beneath a JavaScript folder.");
        }
        return _folders.GetOrAdd(directory.FullName, path =>
            new Lazy<FolderAssembly>(() => Build(path), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }

    private FolderAssembly Build(string folder)
    {
        var sourceDirectory = Directory.GetParent(folder)?.FullName
            ?? throw new InvalidOperationException($"No test source directory for '{folder}'.");
        var names = Directory.EnumerateFiles(sourceDirectory, "*.cs", SearchOption.TopDirectoryOnly)
            .SelectMany(file => RunnableFact.Matches(File.ReadAllText(file))
                .Where(match => !Regex.IsMatch(match.Groups["options"].Value, @"\bSkip\s*="))
                .Select(match => match.Groups["name"].Value))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        if (names.Length == 0)
        {
            throw new InvalidOperationException($"No registered runnable test262 fixtures in '{folder}'.");
        }

        var fixtures = new Dictionary<string, string>(StringComparer.Ordinal);
        var entries = new List<JrocInMemoryEntrySource>(names.Length);
        var fileSystem = new MockFileSystem();
        var includes = new HashSet<string>(StringComparer.Ordinal);
        var identifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var aliasedPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var directoryNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            var segments = name.Replace('\\', '/').Split('/');
            for (var length = 1; length < segments.Length; length++)
            {
                var sourceName = string.Join('/', segments.Take(length));
                var identifier = string.Join('/', segments.Take(length).Select(segment => JrocFacadeNamePlanner.NormalizeIdentifier(segment)));
                if (directoryNames.TryGetValue(identifier, out var existing)
                    && !string.Equals(existing, sourceName, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Fixture directories '{existing}' and '{sourceName}' in '{folder}' have colliding CLR names.");
                }
                directoryNames[identifier] = sourceName;
            }
        }

        foreach (var name in names)
        {
            var path = Path.GetFullPath(Path.Combine(
                folder, name.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js"));
            if (!Path.GetDirectoryName(path)!.StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                && !string.Equals(Path.GetDirectoryName(path), folder, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Fixture '{name}' escapes '{folder}'.");
            }
            var script = File.ReadAllText(path);
            fixtures.Add(path, script);
            var segments = name.Replace('\\', '/').Split('/');
            var normalized = segments.Select(segment => JrocFacadeNamePlanner.NormalizeIdentifier(segment)).ToArray();
            var identifier = string.Join('/', normalized);
            var reservedName = normalized[^1].Equals("Run", StringComparison.OrdinalIgnoreCase)
                || normalized[^1].Equals("Import", StringComparison.OrdinalIgnoreCase)
                || (normalized.Length == 1
                    && normalized[0].Equals("Scripts", StringComparison.OrdinalIgnoreCase));
            var collidesWithDirectory = directoryNames.TryGetValue(identifier, out var directoryName)
                && !string.Equals(directoryName, name.Replace('\\', '/'), StringComparison.Ordinal);
            var logicalPath = !reservedName && !collidesWithDirectory && identifiers.Add(identifier)
                ? path
                : Path.Combine(Path.GetDirectoryName(path)!, $"__test262_entry_{entries.Count:D5}.js");
            if (logicalPath != path && File.Exists(logicalPath))
            {
                throw new InvalidOperationException($"Logical test262 entry path '{logicalPath}' already exists.");
            }
            if (logicalPath != path)
            {
                aliasedPaths.Add(logicalPath, path);
            }
            fileSystem.AddFile(logicalPath, Test262SharedAssertHarness.PrepareEntryScript(script), path);
            entries.Add(new JrocInMemoryEntrySource(
                logicalPath));
            includes.UnionWith(Test262SharedAssertHarness.GetHarnessIncludes(script));
        }

        try
        {
            var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(entries)
            {
                EmitPdb = true,
                FileSystem = fileSystem,
                HostRuntimeIntrinsics = Test262HostRuntimeIntrinsics.Create(includes)
            });
            Interlocked.Increment(ref _compilationCount);
            var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
            Interlocked.Increment(ref _loadCount);
            var moduleIds = fixtures.Keys
                .Zip(artifact.EntryModules, (path, entry) => (path, entry.ModuleId))
                .ToDictionary(pair => pair.path, pair => pair.ModuleId, StringComparer.Ordinal);
            return new FolderAssembly(artifact, loaded, fixtures, moduleIds);
        }
        catch (Exception ex)
        {
            var detail = ex.Message;
            foreach (var (logicalPath, originalPath) in aliasedPaths)
            {
                detail = detail.Replace(logicalPath, originalPath, StringComparison.Ordinal);
            }
            throw new InvalidOperationException(
                $"Failed to compile/load test262 folder '{folder}' ({entries.Count} registered runnable fixtures): {detail}", ex);
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }
        foreach (var lazy in _folders.Values)
        {
            if (lazy.IsValueCreated)
            {
                lazy.Value.Dispose();
            }
        }
        _folders.Clear();
    }

    internal sealed class FolderAssembly(
        JrocCompiledAssemblyArtifact artifact,
        JrocLoadedAssembly loaded,
        IReadOnlyDictionary<string, string> fixtures,
        IReadOnlyDictionary<string, string> moduleIds) : IDisposable
    {
        private readonly IReadOnlyDictionary<string, string> _fixtures = fixtures;
        private readonly IReadOnlyDictionary<string, string> _moduleIds = moduleIds;

        internal JrocCompiledAssemblyArtifact Artifact { get; } = artifact;
        internal JrocLoadedAssembly Loaded { get; } = loaded;

        internal (string Script, string ModuleId) GetEntry(string path)
        {
            path = Path.GetFullPath(path);
            if (!_fixtures.TryGetValue(path, out var script) || !_moduleIds.TryGetValue(path, out var moduleId))
            {
                throw new InvalidOperationException($"Fixture '{path}' is not registered as a runnable entry in this folder.");
            }
            return (script, moduleId);
        }

        public void Dispose() => Loaded.Dispose();
    }
}
