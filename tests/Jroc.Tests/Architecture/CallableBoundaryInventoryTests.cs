using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Jroc.Tests.Architecture;

public sealed class CallableBoundaryInventoryTests
{
    private static readonly string[] RequiredRoles = ["producer", "consumer"];

    private static readonly string[] RequiredClassifications =
    [
        "direct-call",
        "materialization",
        "dynamic-call",
        "construction",
        "callback",
        "export-interop",
        "reflection",
        "continuation",
        "host-adapter",
        "bootstrap"
    ];

    [Fact]
    public void CallableBoundaryInventory_ClassifiesEveryDetectedSourceBoundary()
    {
        var repositoryRoot = FindRepositoryRoot();
        var inventory = LoadInventory(repositoryRoot);

        Assert.Equal(2, inventory.SchemaVersion);
        Assert.Equal(1707, inventory.UmbrellaIssue);
        Assert.NotEmpty(inventory.Boundaries);
        Assert.NotEmpty(inventory.IntentionalDelegateBoundaries);
        Assert.Equal(
            inventory.Boundaries.Count,
            inventory.Boundaries.Select(entry => entry.Id).Distinct(StringComparer.Ordinal).Count());

        foreach (var role in RequiredRoles)
        {
            Assert.Contains(inventory.Boundaries, entry => entry.Roles.Contains(role, StringComparer.Ordinal));
        }

        foreach (var classification in RequiredClassifications)
        {
            Assert.Contains(
                inventory.Boundaries,
                entry => entry.Classifications.Contains(classification, StringComparer.Ordinal));
        }

        var sourceFiles = Directory
            .EnumerateFiles(Path.Combine(repositoryRoot, "src"), "*.cs", SearchOption.AllDirectories)
            .Select(path => new SourceFile(
                Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'),
                File.ReadAllText(path)))
            .ToArray();

        foreach (var entry in inventory.Boundaries)
        {
            Assert.NotEmpty(entry.Roles);
            Assert.NotEmpty(entry.Classifications);
            Assert.NotEmpty(entry.TrackingIssues);
            Assert.NotEmpty(entry.SourceGlobs);
            Assert.False(string.IsNullOrWhiteSpace(entry.Rationale));
            Assert.All(entry.TrackingIssues, issue => Assert.True(issue > 0, $"{entry.Id} has an invalid tracking issue."));

            foreach (var sourceGlob in entry.SourceGlobs)
            {
                Assert.True(
                    sourceFiles.Any(file => MatchesGlob(file.RelativePath, sourceGlob)),
                    $"Inventory entry '{entry.Id}' glob '{sourceGlob}' does not match a source file.");
            }
        }

        foreach (var boundary in inventory.IntentionalDelegateBoundaries)
        {
            Assert.False(string.IsNullOrWhiteSpace(boundary.Id));
            Assert.NotEmpty(boundary.SourceGlobs);
            Assert.NotEmpty(boundary.Symbols);
            Assert.False(string.IsNullOrWhiteSpace(boundary.Rationale));
            Assert.All(
                boundary.SourceGlobs,
                sourceGlob => Assert.True(
                    sourceFiles.Any(file => MatchesGlob(file.RelativePath, sourceGlob)),
                    $"Intentional delegate boundary '{boundary.Id}' glob '{sourceGlob}' does not match a source file."));
            Assert.All(
                boundary.Symbols,
                symbol => Assert.Contains(
                    sourceFiles.Where(file => boundary.SourceGlobs.Any(
                        sourceGlob => MatchesGlob(file.RelativePath, sourceGlob))),
                    file => file.Content.Contains(symbol, StringComparison.Ordinal)));
        }
    }

    [Fact]
    public void RetiredCompiledFunctionDelegatePaths_DoNotReappear()
    {
        var repositoryRoot = FindRepositoryRoot();
        var sourceFiles = EnumerateSourceFiles(repositoryRoot);
        string[] retiredSymbols =
        [
            "CallableDelegateTypeResolver",
            "GetMaterializedDelegateType",
            "Closure.BindArrow",
            "Closure.BindMoveNext",
            "CreateBoundDelegate",
            "InvokeDirectWithArgs",
            "ClassConstructorValue"
        ];

        var violations = sourceFiles
            .SelectMany(file => retiredSymbols
                .Where(symbol => file.Content.Contains(symbol, StringComparison.Ordinal))
                .Select(symbol => $"{file.RelativePath}: {symbol}"))
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Retired compiled-function delegate APIs reappeared:"
            + Environment.NewLine
            + string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void CompilerDelegateCreationSites_AreExplicitlyAllowlisted()
    {
        var repositoryRoot = FindRepositoryRoot();
        var compilerFiles = EnumerateSourceFiles(repositoryRoot)
            .Where(file => file.RelativePath.StartsWith("src/Compiler/", StringComparison.Ordinal))
            .ToArray();
        var ldftnSites = compilerFiles
            .SelectMany(file => file.Content
                .Split('\n')
                .Select((line, index) => new
                {
                    file.RelativePath,
                    Line = index + 1,
                    Content = line
                })
                .Where(site => site.Content.Contains(
                    "ILOpCode.Ldftn",
                    StringComparison.Ordinal)))
            .ToArray();

        Assert.Equal(4, ldftnSites.Length);
        Assert.Equal(
            3,
            ldftnSites.Count(site => site.RelativePath ==
                "src/Compiler/IL/LIRToILCompiler.InstructionEmission.LeafScopeInstance.cs"));
        Assert.Single(
            ldftnSites,
            site => site.RelativePath ==
                "src/Compiler/Services/AssemblyGenerator.cs");

        var continuationEmitter = Assert.Single(
            compilerFiles,
            file => file.RelativePath ==
                "src/Compiler/IL/LIRToILCompiler.InstructionEmission.LeafScopeInstance.cs");
        Assert.Equal(
            3,
            CountOccurrences(
                continuationEmitter.Content,
                "GetContinuationDelegateCtorRef("));
        Assert.Equal(
            3,
            CountOccurrences(
                continuationEmitter.Content,
                "nameof(JavaScriptRuntime.CompiledContinuation.Create)"));

        var bootstrapEmitter = Assert.Single(
            compilerFiles,
            file => file.RelativePath ==
                "src/Compiler/Services/AssemblyGenerator.cs");
        Assert.Equal(
            1,
            CountOccurrences(
                bootstrapEmitter.Content,
                "_bclReferences.ModuleMainDelegate_Ctor_Ref"));
        Assert.DoesNotContain(
            compilerFiles,
            file => Regex.IsMatch(
                file.Content,
                @"new\s+ValueStorage\([^\r\n]*typeof\(Delegate\)",
                RegexOptions.CultureInvariant));
    }

    [Fact]
    public void JavaScriptVisibleDelegateChecks_AreCentralized()
    {
        var repositoryRoot = FindRepositoryRoot();
        var delegateCheck = new Regex(
            @"\bis\s+(?:not\s+)?Delegate\b|\bas\s+Delegate\b",
            RegexOptions.CultureInvariant);
        var matches = EnumerateSourceFiles(repositoryRoot)
            .Where(file => file.RelativePath.StartsWith(
                "src/JavaScriptRuntime/",
                StringComparison.Ordinal))
            .SelectMany(file => delegateCheck
                .Matches(file.Content)
                .Select(match => $"{file.RelativePath}:{match.Value}"))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "src/JavaScriptRuntime/BuiltinDelegateFunctionAdapter.cs:is Delegate",
                "src/JavaScriptRuntime/BuiltinDelegateFunctionAdapter.cs:is Delegate",
                "src/JavaScriptRuntime/CompiledContinuation.cs:is not Delegate"
            },
            matches);

        var callableOperations = File.ReadAllText(
            Path.Combine(
                repositoryRoot,
                "src",
                "JavaScriptRuntime",
                "CallableOperations.cs"));
        Assert.DoesNotMatch(delegateCheck, callableOperations);
    }

    [Fact]
    public void RuntimeDelegateFallbacks_AreExactInternalAdapterSites()
    {
        var repositoryRoot = FindRepositoryRoot();
        var sourceFiles = EnumerateSourceFiles(repositoryRoot).ToArray();
        var runtimeFiles = sourceFiles
            .Where(file => file.RelativePath.StartsWith(
                "src/JavaScriptRuntime/",
                StringComparison.Ordinal))
            .ToArray();

        var dynamicInvokeSites = runtimeFiles
            .Where(file => file.Content.Contains(
                ".DynamicInvoke(",
                StringComparison.Ordinal))
            .ToDictionary(
                file => file.RelativePath,
                file => CountOccurrences(file.Content, ".DynamicInvoke("),
                StringComparer.Ordinal);

        Assert.Equal(2, dynamicInvokeSites.Count);
        Assert.Equal(
            2,
            CountOccurrences(
                Assert.Single(
                    runtimeFiles,
                    file => file.RelativePath ==
                        "src/JavaScriptRuntime/Closure.cs").Content,
                "target.DynamicInvoke("));
        Assert.Equal(
            1,
            dynamicInvokeSites[
                "src/JavaScriptRuntime/Hosting/HostedFunctionObjects.cs"]);

        var delegateKeyedWeakTable = new Regex(
            @"ConditionalWeakTable\s*<\s*(?:(?:global::)?System\.)?Delegate\s*,",
            RegexOptions.CultureInvariant);
        var weakDelegateTables = sourceFiles
            .Where(file => delegateKeyedWeakTable.IsMatch(file.Content))
            .Select(file => file.RelativePath)
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Empty(weakDelegateTables);

        Assert.DoesNotContain(
            runtimeFiles,
            file => file.Content.Contains(
                "BuiltinDelegateMetadata",
                StringComparison.Ordinal));
    }

    [Fact]
    public void RuntimeDoesNotCompileDynamicAccessors()
    {
        var repositoryRoot = FindRepositoryRoot();
        var forbidden = new Regex(
            @"System\.Linq\.Expressions|Expression\.Lambda|Expression\.Compile|\bDynamicMethod\b",
            RegexOptions.CultureInvariant);
        var violations = EnumerateSourceFiles(repositoryRoot)
            .Where(file => forbidden.IsMatch(file.Content))
            .Select(file => file.RelativePath)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void LoweringAndIlEmission_DoNotDependOnAstCallableSemantics()
    {
        var repositoryRoot = FindRepositoryRoot();
        var forbiddenAstDependency = new Regex(
            @"using\s+Acornima\.Ast\s*;|Acornima\.Ast|\.AstNode\b",
            RegexOptions.CultureInvariant);
        var lirFiles = Directory.EnumerateFiles(
                Path.Combine(repositoryRoot, "src", "Compiler", "IR", "LIR"),
                "*.cs",
                SearchOption.AllDirectories)
            .Select(path => new SourceFile(
                Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'),
                File.ReadAllText(path)));
        var ilFiles = Directory.EnumerateFiles(
                Path.Combine(repositoryRoot, "src", "Compiler", "IL"),
                "*.cs",
                SearchOption.AllDirectories)
            .Select(path => new SourceFile(
                Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'),
                File.ReadAllText(path)));

        var violations = lirFiles.Concat(ilFiles)
            .Where(file => forbiddenAstDependency.IsMatch(file.Content))
            .Select(file => file.RelativePath)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void CallableBenchmark_CoversMaterializationRepeatedLoadAndSteadyState()
    {
        var repositoryRoot = FindRepositoryRoot();
        var benchmarkPath = Path.Combine(
            repositoryRoot,
            "tests",
            "performance",
            "Benchmarks",
            "CallableArchitectureBenchmarks.cs");
        var benchmark = File.ReadAllText(benchmarkPath);

        Assert.Contains(
            "Generated arrow object materialization",
            benchmark,
            StringComparison.Ordinal);
        Assert.Contains(
            "Repeated compiled module load",
            benchmark,
            StringComparison.Ordinal);
        Assert.Contains(
            "Loaded module direct-call loop",
            benchmark,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "BindArrow",
            benchmark,
            StringComparison.Ordinal);
    }

    private static CallableBoundaryInventory LoadInventory(string repositoryRoot)
    {
        var path = Path.Combine(repositoryRoot, "docs", "compiler", "CallableBoundaryInventory.json");
        var inventory = JsonSerializer.Deserialize<CallableBoundaryInventory>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return inventory ?? throw new InvalidOperationException($"Could not deserialize callable boundary inventory '{path}'.");
    }

    private static SourceFile[] EnumerateSourceFiles(string repositoryRoot)
        => Directory
            .EnumerateFiles(
                Path.Combine(repositoryRoot, "src"),
                "*.cs",
                SearchOption.AllDirectories)
            .Select(path => new SourceFile(
                Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'),
                File.ReadAllText(path)))
            .ToArray();

    private static bool MatchesGlob(string relativePath, string glob)
    {
        var regex = new StringBuilder("^");
        for (var index = 0; index < glob.Length; index++)
        {
            var character = glob[index];
            if (character == '*')
            {
                if (index + 1 < glob.Length && glob[index + 1] == '*')
                {
                    regex.Append(".*");
                    index++;
                }
                else
                {
                    regex.Append("[^/]*");
                }
            }
            else if (character == '?')
            {
                regex.Append("[^/]");
            }
            else
            {
                regex.Append(Regex.Escape(character.ToString()));
            }
        }

        regex.Append('$');
        return Regex.IsMatch(relativePath, regex.ToString(), RegexOptions.CultureInvariant);
    }

    private static int CountOccurrences(string content, string value)
    {
        var count = 0;
        var startIndex = 0;
        while ((startIndex = content.IndexOf(
                   value,
                   startIndex,
                   StringComparison.Ordinal)) >= 0)
        {
            count++;
            startIndex += value.Length;
        }

        return count;
    }

    private static string FindRepositoryRoot([CallerFilePath] string sourceFilePath = "")
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(sourceFilePath)!);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "jroc.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException($"Could not find the repository root from '{sourceFilePath}'.");
    }

    private sealed record SourceFile(string RelativePath, string Content);

    private sealed record CallableBoundaryInventory(
        int SchemaVersion,
        int UmbrellaIssue,
        IReadOnlyList<CallableBoundary> Boundaries,
        IReadOnlyList<IntentionalDelegateBoundary> IntentionalDelegateBoundaries);

    private sealed record CallableBoundary(
        string Id,
        IReadOnlyList<string> Roles,
        IReadOnlyList<string> Classifications,
        IReadOnlyList<int> TrackingIssues,
        IReadOnlyList<string> SourceGlobs,
        string Rationale);

    private sealed record IntentionalDelegateBoundary(
        string Id,
        IReadOnlyList<string> SourceGlobs,
        IReadOnlyList<string> Symbols,
        string Rationale);
}
