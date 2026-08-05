using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Jroc.Tests.Architecture;

public sealed class CallableBoundaryInventoryTests
{
    private static readonly CallableSourceMarker[] CallableSourceMarkers =
    [
        new("CallableId", [ "consumer" ], [ "direct-call" ]),
        new("CallableSignature", [ "consumer" ], [ "direct-call" ]),
        new("InvokeWithArgs", [ "consumer" ], [ "dynamic-call" ]),
        new("BindArrow", [ "producer" ], [ "materialization" ]),
        new("CreateBoundDelegate", [ "producer" ], [ "materialization" ]),
        new(" is Delegate", [ "consumer" ], [ "dynamic-call" ]),
        new(" is not Delegate", [ "consumer" ], [ "dynamic-call" ]),
        new("Delegate del", [ "consumer" ], [ "dynamic-call" ]),
        new("Delegate d", [ "consumer" ], [ "dynamic-call" ]),
        new("DynamicInvoke(", [ "consumer" ], [ "dynamic-call" ]),
        new("InvokeJsDelegate", [ "consumer" ], [ "export-interop" ])
    ];

    private static readonly string[] RequiredRoles = ["producer", "consumer"];

    private static readonly string[] RequiredClassifications =
    [
        "direct-call",
        "materialization",
        "dynamic-call",
        "construction",
        "callback",
        "export-interop",
        "reflection"
    ];

    [Fact]
    public void CallableBoundaryInventory_ClassifiesEveryDetectedSourceBoundary()
    {
        var repositoryRoot = FindRepositoryRoot();
        var inventory = LoadInventory(repositoryRoot);

        Assert.Equal(1, inventory.SchemaVersion);
        Assert.Equal(1707, inventory.UmbrellaIssue);
        Assert.NotEmpty(inventory.Boundaries);
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

        var detectedBoundaries = sourceFiles
            .Where(file => CallableSourceMarkers.Any(marker => file.Content.Contains(marker.Text, StringComparison.Ordinal)))
            .Select(file => file.RelativePath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(detectedBoundaries);

        var unclassified = detectedBoundaries
            .Where(path => !inventory.Boundaries.Any(entry =>
                entry.SourceGlobs.Any(sourceGlob => MatchesGlob(path, sourceGlob))))
            .ToArray();

        Assert.True(
            unclassified.Length == 0,
            "Callable source boundaries missing from docs/compiler/CallableBoundaryInventory.json:"
            + Environment.NewLine
            + string.Join(Environment.NewLine, unclassified.Select(path => $"  - {path}")));

        var mismatched = sourceFiles
            .Select(file => ValidateDetectedBoundary(file, inventory.Boundaries))
            .Where(message => message != null)
            .ToArray();

        Assert.True(
            mismatched.Length == 0,
            "Callable source boundaries have incomplete roles/classifications:"
            + Environment.NewLine
            + string.Join(Environment.NewLine, mismatched));
    }

    private static CallableBoundaryInventory LoadInventory(string repositoryRoot)
    {
        var path = Path.Combine(repositoryRoot, "docs", "compiler", "CallableBoundaryInventory.json");
        var inventory = JsonSerializer.Deserialize<CallableBoundaryInventory>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return inventory ?? throw new InvalidOperationException($"Could not deserialize callable boundary inventory '{path}'.");
    }

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

    private static string? ValidateDetectedBoundary(
        SourceFile file,
        IReadOnlyList<CallableBoundary> boundaries)
    {
        var detectedMarkers = CallableSourceMarkers
            .Where(marker => file.Content.Contains(marker.Text, StringComparison.Ordinal))
            .ToArray();
        if (detectedMarkers.Length == 0)
        {
            return null;
        }

        var matchingEntries = boundaries
            .Where(entry => entry.SourceGlobs.Any(sourceGlob => MatchesGlob(file.RelativePath, sourceGlob)))
            .ToArray();
        var classifiedRoles = matchingEntries
            .SelectMany(entry => entry.Roles)
            .ToHashSet(StringComparer.Ordinal);
        var classifiedOperations = matchingEntries
            .SelectMany(entry => entry.Classifications)
            .ToHashSet(StringComparer.Ordinal);
        var expectedRoles = detectedMarkers
            .SelectMany(marker => marker.RequiredRoles)
            .Distinct(StringComparer.Ordinal)
            .Where(role => !classifiedRoles.Contains(role))
            .ToArray();
        var expectedOperations = detectedMarkers
            .SelectMany(marker => marker.RequiredClassifications)
            .Distinct(StringComparer.Ordinal)
            .Where(classification => !classifiedOperations.Contains(classification))
            .ToArray();

        if (expectedRoles.Length == 0 && expectedOperations.Length == 0)
        {
            return null;
        }

        return $"  - {file.RelativePath}: missing roles [{string.Join(", ", expectedRoles)}], "
            + $"classifications [{string.Join(", ", expectedOperations)}]";
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

    private sealed record CallableSourceMarker(
        string Text,
        IReadOnlyList<string> RequiredRoles,
        IReadOnlyList<string> RequiredClassifications);

    private sealed record CallableBoundaryInventory(
        int SchemaVersion,
        int UmbrellaIssue,
        IReadOnlyList<CallableBoundary> Boundaries);

    private sealed record CallableBoundary(
        string Id,
        IReadOnlyList<string> Roles,
        IReadOnlyList<string> Classifications,
        IReadOnlyList<int> TrackingIssues,
        IReadOnlyList<string> SourceGlobs,
        string Rationale);
}
