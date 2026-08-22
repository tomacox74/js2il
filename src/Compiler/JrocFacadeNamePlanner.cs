using System.Collections.ObjectModel;
using System.Text;

namespace Jroc;

public sealed record JrocFacadeModuleName(string ModuleId, IReadOnlyList<string> TypePath)
{
    public string ClrPath(string rootTypeName) =>
        string.Join(".", new[] { rootTypeName, "Scripts" }.Concat(TypePath));
}

public sealed record JrocFacadeNamePlan(
    string RootTypeName,
    string EntryModuleId,
    IReadOnlyList<JrocFacadeModuleName> Modules);

public sealed class JrocFacadeNameCollisionException : InvalidOperationException
{
    internal JrocFacadeNameCollisionException(
        string firstModuleId,
        string secondModuleId,
        string proposedClrPath)
        : base(
            $"Module ids '{firstModuleId}' and '{secondModuleId}' collide at generated CLR path " +
            $"'{proposedClrPath}'. Rename one module or choose module ids that remain distinct after C# identifier normalization.")
    {
        FirstModuleId = firstModuleId;
        SecondModuleId = secondModuleId;
        ProposedClrPath = proposedClrPath;
    }

    public string FirstModuleId { get; }

    public string SecondModuleId { get; }

    public string ProposedClrPath { get; }
}

public static class JrocFacadeNamePlanner
{
    private static readonly HashSet<string> CSharpKeywords =
    [
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
        "void", "volatile", "while"
    ];

    public static JrocFacadeNamePlan Create(
        string assemblyName,
        string entryModuleId,
        IEnumerable<string> canonicalModuleIds,
        string? moduleIdPrefix = null)
    {
        JrocAssemblyIdentity.Validate(assemblyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(entryModuleId);
        ArgumentNullException.ThrowIfNull(canonicalModuleIds);

        var rootTypeName = NormalizeIdentifier(assemblyName, stripLeadingAtSign: true);
        var modules = new List<JrocFacadeModuleName>();
        var root = new NameNode(sourceSegment: null, sourceModuleId: null);

        foreach (var moduleId in canonicalModuleIds
                     .Distinct(StringComparer.Ordinal)
                     .Order(StringComparer.Ordinal))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
            var relativeModuleId = RemovePrefix(moduleId, moduleIdPrefix);
            var sourceSegments = NormalizeModuleId(relativeModuleId);
            var typePath = sourceSegments.Select(segment => NormalizeIdentifier(segment)).ToArray();

            Insert(root, sourceSegments, typePath, moduleId, rootTypeName);
            modules.Add(new JrocFacadeModuleName(moduleId, new ReadOnlyCollection<string>(typePath)));
        }

        ValidateFacadeMemberCollisions(
            root,
            rootTypeName,
            entryModuleId,
            parentTypeName: "Scripts",
            parentPath: []);

        if (!modules.Any(module => string.Equals(module.ModuleId, entryModuleId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(
                $"Entry module '{entryModuleId}' is not present in the facade naming plan.");
        }

        return new JrocFacadeNamePlan(
            rootTypeName,
            entryModuleId,
            new ReadOnlyCollection<JrocFacadeModuleName>(
                modules.OrderBy(module => module.ModuleId, StringComparer.Ordinal).ToArray()));
    }

    public static string NormalizeIdentifier(string value, bool stripLeadingAtSign = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var trimmed = value.Trim();
        if (stripLeadingAtSign)
        {
            trimmed = trimmed.TrimStart('@');
        }

        var builder = new StringBuilder(trimmed.Length);
        var previousWasReplacement = false;
        foreach (var character in trimmed)
        {
            if (character == '_' || char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                previousWasReplacement = false;
            }
            else if (!previousWasReplacement)
            {
                builder.Append('_');
                previousWasReplacement = true;
            }
        }

        var identifier = builder.ToString().Trim('_');
        if (identifier.Length == 0)
        {
            identifier = "_";
        }

        if (char.IsDigit(identifier[0]) || CSharpKeywords.Contains(identifier))
        {
            identifier = "_" + identifier;
        }

        return identifier;
    }

    private static string RemovePrefix(string moduleId, string? moduleIdPrefix)
    {
        var normalized = moduleId.Trim().Replace('\\', '/').TrimStart('/');
        var normalizedPrefix = moduleIdPrefix?.Trim().Replace('\\', '/').Trim('/');

        if (!string.IsNullOrWhiteSpace(normalizedPrefix)
            && normalized.StartsWith(normalizedPrefix + "/", StringComparison.Ordinal))
        {
            return normalized[(normalizedPrefix.Length + 1)..];
        }

        return normalized;
    }

    private static string[] NormalizeModuleId(string moduleId)
    {
        var normalized = moduleId.Trim().Replace('\\', '/').Trim('/');
        if (normalized.StartsWith("./", StringComparison.Ordinal))
        {
            normalized = normalized[2..];
        }

        if (normalized.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || normalized.EndsWith(".mjs", StringComparison.OrdinalIgnoreCase)
            || normalized.EndsWith(".cjs", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[..normalized.LastIndexOf('.')];
        }

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length == 0 ? ["index"] : segments;
    }

    private static void Insert(
        NameNode root,
        IReadOnlyList<string> sourceSegments,
        IReadOnlyList<string> typePath,
        string moduleId,
        string rootTypeName)
    {
        var current = root;
        for (var index = 0; index < typePath.Count; index++)
        {
            var typeSegment = typePath[index];
            var sourceSegment = sourceSegments[index];
            if (current.Children.TryGetValue(typeSegment, out var existing))
            {
                if (!string.Equals(existing.SourceSegment, sourceSegment, StringComparison.Ordinal))
                {
                    var proposedPath = string.Join(
                        ".",
                        new[] { rootTypeName, "Scripts" }.Concat(typePath.Take(index + 1)));
                    throw new JrocFacadeNameCollisionException(
                        existing.SourceModuleId ?? moduleId,
                        moduleId,
                        proposedPath);
                }

                current = existing;
                continue;
            }

            var caseInsensitiveMatch = current.Children.Values.FirstOrDefault(
                child => string.Equals(child.TypeSegment, typeSegment, StringComparison.OrdinalIgnoreCase));
            if (caseInsensitiveMatch is not null)
            {
                var proposedPath = string.Join(
                    ".",
                    new[] { rootTypeName, "Scripts" }.Concat(typePath.Take(index + 1)));
                throw new JrocFacadeNameCollisionException(
                    caseInsensitiveMatch.SourceModuleId ?? moduleId,
                    moduleId,
                    proposedPath);
            }

            var child = new NameNode(sourceSegment, moduleId) { TypeSegment = typeSegment };
            current.Children.Add(typeSegment, child);
            current = child;
        }

        if (current.ModuleId is not null
            && !string.Equals(current.ModuleId, moduleId, StringComparison.Ordinal))
        {
            throw new JrocFacadeNameCollisionException(
                current.ModuleId,
                moduleId,
                string.Join(".", new[] { rootTypeName, "Scripts" }.Concat(typePath)));
        }

        current.ModuleId = moduleId;
    }

    private static void ValidateFacadeMemberCollisions(
        NameNode node,
        string rootTypeName,
        string entryModuleId,
        string parentTypeName,
        IReadOnlyList<string> parentPath)
    {
        if (parentPath.Count == 0
            && IsReservedFacadeMemberName(rootTypeName, includeScripts: true))
        {
            throw new JrocFacadeNameCollisionException(
                entryModuleId,
                entryModuleId,
                rootTypeName);
        }

        foreach (var child in node.Children.Values)
        {
            var childPath = parentPath.Concat([child.TypeSegment]).ToArray();
            if (string.Equals(child.TypeSegment, parentTypeName, StringComparison.OrdinalIgnoreCase)
                || (child.ModuleId is not null
                    && IsReservedFacadeMemberName(child.TypeSegment, includeScripts: false))
                || (node.ModuleId is not null
                    && IsReservedFacadeMemberName(child.TypeSegment, includeScripts: false)))
            {
                throw new JrocFacadeNameCollisionException(
                    node.ModuleId ?? child.ModuleId ?? entryModuleId,
                    child.SourceModuleId ?? node.ModuleId ?? entryModuleId,
                    string.Join(
                        ".",
                        new[] { rootTypeName, "Scripts" }.Concat(childPath)));
            }

            ValidateFacadeMemberCollisions(
                child,
                rootTypeName,
                entryModuleId,
                child.TypeSegment,
                childPath);
        }
    }

    private static bool IsReservedFacadeMemberName(
        string name,
        bool includeScripts) =>
        string.Equals(name, "Run", StringComparison.OrdinalIgnoreCase)
        || string.Equals(name, "Import", StringComparison.OrdinalIgnoreCase)
        || (includeScripts
            && string.Equals(name, "Scripts", StringComparison.OrdinalIgnoreCase));

    private sealed class NameNode(string? sourceSegment, string? sourceModuleId)
    {
        public string? SourceSegment { get; } = sourceSegment;

        public string? SourceModuleId { get; } = sourceModuleId;

        public string TypeSegment { get; init; } = string.Empty;

        public Dictionary<string, NameNode> Children { get; } = new(StringComparer.Ordinal);

        public string? ModuleId { get; set; }
    }
}
