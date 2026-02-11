using System.Text.Json;
using System.Linq;

namespace Js2IL.Services;

/// <summary>
/// Resolves CommonJS module specifiers to concrete .js files at compile time.
///
/// This mirrors Node.js module resolution rules as closely as practical for JS2IL,
/// but enforces that resolved targets must be <c>.js</c> files.
/// </summary>
public sealed class NodeModuleResolver
{
    private readonly IFileSystem _fileSystem;

    public NodeModuleResolver(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool TryResolve(string specifier, string baseDirectory, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        if (string.IsNullOrWhiteSpace(specifier))
        {
            error = "Module specifier must be a non-empty string.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            error = "Base directory must be provided for module resolution.";
            return false;
        }

        var normalizedSpecifier = NormalizeSpecifier(specifier);

        // Relative/absolute filesystem-like requests.
        if (IsPathLikeSpecifier(normalizedSpecifier))
        {
            var requestPath = ToAbsolutePath(normalizedSpecifier, baseDirectory);
            return TryResolveAsFileOrDirectory(requestPath, out resolvedPath, out error);
        }

        // Bare specifier: treat as npm package id (potentially with subpath).
        if (!TryResolveBarePackageSpecifier(normalizedSpecifier, baseDirectory, out resolvedPath, out error))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Encodes an arbitrary module id into a valid CLR identifier using a reversible scheme.
    /// Output contains only [A-Za-z0-9_] and never starts with a digit.
    /// </summary>
    public static string EncodeModuleIdToClrIdentifier(string moduleId)
    {
        if (string.IsNullOrWhiteSpace(moduleId))
        {
            return "_";
        }

        // Hex-encode UTF-8 bytes for full reversibility.
        // Prefix with "m_" to ensure it never starts with a digit.
        var bytes = System.Text.Encoding.UTF8.GetBytes(moduleId);
        var chars = new char[2 + bytes.Length * 2];
        chars[0] = 'm';
        chars[1] = '_';
        var i = 2;
        foreach (var b in bytes)
        {
            var hi = (b >> 4) & 0xF;
            var lo = b & 0xF;
            chars[i++] = (char)(hi < 10 ? ('0' + hi) : ('a' + (hi - 10)));
            chars[i++] = (char)(lo < 10 ? ('0' + lo) : ('a' + (lo - 10)));
        }

        return new string(chars);
    }

    private bool TryResolveBarePackageSpecifier(string specifier, string baseDirectory, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        ParseBareSpecifier(specifier, out var packageName, out var packageSubpath);
        if (string.IsNullOrWhiteSpace(packageName))
        {
            error = $"Invalid package specifier '{specifier}'.";
            return false;
        }

        foreach (var dir in WalkUpDirectories(baseDirectory))
        {
            var packageRoot = Path.Combine(dir, "node_modules", packageName);

            // Determine whether the package exists at this level by probing for common markers.
            var packageJsonPath = Path.Combine(packageRoot, "package.json");
            var packageIndexJs = Path.Combine(packageRoot, "index.js");

            if (!_fileSystem.FileExists(packageJsonPath) && !_fileSystem.FileExists(packageIndexJs))
            {
                continue;
            }

            // If subpath is present, resolve within the package.
            if (!string.IsNullOrEmpty(packageSubpath))
            {
                // First try package.json exports (if present).
                if (_fileSystem.FileExists(packageJsonPath)
                    && TryResolvePackageExports(packageRoot, packageJsonPath, packageSubpath, out resolvedPath, out error))
                {
                    return true;
                }

                // Fallback: treat subpath as a filesystem path within package root.
                var subpathPath = Path.Combine(packageRoot, packageSubpath.Replace('/', Path.DirectorySeparatorChar));
                if (TryResolveAsFileOrDirectory(subpathPath, out resolvedPath, out error))
                {
                    return true;
                }

                // Stop searching up: Node resolves the nearest matching node_modules package directory.
                return false;
            }

            // Package root entry resolution.
            if (_fileSystem.FileExists(packageJsonPath)
                && TryResolvePackageEntryFromPackageJson(packageRoot, packageJsonPath, out resolvedPath, out error))
            {
                return true;
            }

            // Fallback: <packageRoot>/index.js
            if (_fileSystem.FileExists(packageIndexJs))
            {
                resolvedPath = Path.GetFullPath(packageIndexJs);
                return true;
            }

            error = $"Could not resolve entry for package '{packageName}'.";
            return false;
        }

        error = $"Cannot find module '{specifier}' (searched node_modules up from '{baseDirectory}').";
        return false;
    }

    private bool TryResolveAsFileOrDirectory(string requestPath, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        // 1) Exact file
        if (TryAcceptResolvedJsFile(requestPath, out resolvedPath, out error))
        {
            return true;
        }

        // 2) Append .js
        if (!Path.HasExtension(requestPath))
        {
            var withJs = requestPath + ".js";
            if (TryAcceptResolvedJsFile(withJs, out resolvedPath, out error))
            {
                return true;
            }
        }

        // 3) Directory: package.json -> entry, then index.js
        var packageJson = Path.Combine(requestPath, "package.json");
        if (_fileSystem.FileExists(packageJson)
            && TryResolvePackageEntryFromPackageJson(requestPath, packageJson, out resolvedPath, out error))
        {
            return true;
        }

        var indexJs = Path.Combine(requestPath, "index.js");
        if (_fileSystem.FileExists(indexJs))
        {
            resolvedPath = Path.GetFullPath(indexJs);
            return true;
        }

        error = $"Cannot resolve module path '{requestPath}' to a .js file.";
        return false;
    }

    private bool TryAcceptResolvedJsFile(string path, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        if (!_fileSystem.FileExists(path))
        {
            return false;
        }

        var full = Path.GetFullPath(path);
        if (!full.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
        {
            error = $"Resolved module target '{full}' is not a .js file (only .js is supported).";
            return false;
        }

        resolvedPath = full;
        return true;
    }

    private bool TryResolvePackageEntryFromPackageJson(string packageRoot, string packageJsonPath, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        try
        {
            var json = _fileSystem.ReadAllText(packageJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Prefer exports if present.
            if (root.TryGetProperty("exports", out var exportsElement)
                && TryResolveExportsTarget(exportsElement, subpathKey: ".", out var targetRel)
                && TryResolveExportsTargetPath(packageRoot, targetRel, out resolvedPath, out error))
            {
                return true;
            }

            if (root.TryGetProperty("main", out var mainElement) && mainElement.ValueKind == JsonValueKind.String)
            {
                var main = mainElement.GetString();
                if (!string.IsNullOrWhiteSpace(main))
                {
                    var mainPath = Path.Combine(packageRoot, main.Replace('/', Path.DirectorySeparatorChar));
                    if (TryResolveAsFileOrDirectory(mainPath, out resolvedPath, out error))
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            error = $"Failed to read/parse package.json '{packageJsonPath}': {ex.Message}";
            return false;
        }

        // No usable entry in package.json.
        return false;
    }

    private bool TryResolvePackageExports(string packageRoot, string packageJsonPath, string packageSubpath, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        // Node uses keys like "." and "./subpath".
        var exportsKey = "./" + packageSubpath.TrimStart('/');

        try
        {
            var json = _fileSystem.ReadAllText(packageJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("exports", out var exportsElement))
            {
                return false;
            }

            if (TryResolveExportsTarget(exportsElement, exportsKey, out var targetRel))
            {
                return TryResolveExportsTargetPath(packageRoot, targetRel, out resolvedPath, out error);
            }
        }
        catch (Exception ex)
        {
            error = $"Failed to read/parse package.json '{packageJsonPath}': {ex.Message}";
            return false;
        }

        return false;
    }

    private bool TryResolveExportsTarget(JsonElement exportsElement, string subpathKey, out string target)
    {
        target = string.Empty;

        // exports can be:
        // - string: shorthand for '.'
        // - object: either subpath map (keys starting with '.') or conditional map
        // - array: try entries in order
        switch (exportsElement.ValueKind)
        {
            case JsonValueKind.String:
                if (subpathKey == ".")
                {
                    target = exportsElement.GetString() ?? string.Empty;
                    return !string.IsNullOrWhiteSpace(target);
                }
                return false;

            case JsonValueKind.Array:
                foreach (var entry in exportsElement.EnumerateArray())
                {
                    if (TryResolveExportsTarget(entry, subpathKey, out target))
                    {
                        return true;
                    }
                }
                return false;

            case JsonValueKind.Object:
                // If there is an explicit subpath entry, prefer that.
                if (LooksLikeSubpathMap(exportsElement))
                {
                    if (exportsElement.TryGetProperty(subpathKey, out var subpathEntry))
                    {
                        return TryResolveConditionalExports(subpathEntry, out target);
                    }

                    // Very small subset of pattern support: "./*".
                    // If present, substitute the single '*' with the requested subpath remainder.
                    if (TryResolveSubpathPattern(exportsElement, subpathKey, out var patternEntry, out var wildcardValue)
                        && TryResolveConditionalExports(patternEntry, out var patternTarget))
                    {
                        target = patternTarget.Replace("*", wildcardValue, StringComparison.Ordinal);
                        return !string.IsNullOrWhiteSpace(target);
                    }

                    return false;
                }

                // Conditional exports for '.'
                if (subpathKey == ".")
                {
                    return TryResolveConditionalExports(exportsElement, out target);
                }

                return false;

            default:
                return false;
        }
    }

    private static bool LooksLikeSubpathMap(JsonElement obj)
    {
        return obj.EnumerateObject().Any(p => p.Name.StartsWith(".", StringComparison.Ordinal));
    }

    private static bool TryResolveSubpathPattern(JsonElement exportsObj, string subpathKey, out JsonElement patternEntry, out string wildcardValue)
    {
        patternEntry = default;
        wildcardValue = string.Empty;

        // Only support single '*' patterns like "./*".
        foreach (var prop in exportsObj.EnumerateObject().Where(p => p.Name.Contains('*')))
        {
            var key = prop.Name;
            var starIndex = key.IndexOf('*', StringComparison.Ordinal);
            if (starIndex < 0)
            {
                continue;
            }

            var prefix = key.Substring(0, starIndex);
            var suffix = key.Substring(starIndex + 1);

            if (!subpathKey.StartsWith(prefix, StringComparison.Ordinal) || !subpathKey.EndsWith(suffix, StringComparison.Ordinal))
            {
                continue;
            }

            wildcardValue = subpathKey.Substring(prefix.Length, subpathKey.Length - prefix.Length - suffix.Length);
            patternEntry = prop.Value;
            return true;
        }

        return false;
    }

    private static bool TryResolveConditionalExports(JsonElement entry, out string target)
    {
        target = string.Empty;

        // In CommonJS require() mode, Node evaluates conditions.
        // Implement minimal priority: require -> node -> default.
        if (entry.ValueKind == JsonValueKind.String)
        {
            target = entry.GetString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(target);
        }

        if (entry.ValueKind == JsonValueKind.Array)
        {
            foreach (var e in entry.EnumerateArray())
            {
                if (TryResolveConditionalExports(e, out target))
                {
                    return true;
                }
            }
            return false;
        }

        if (entry.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        var priorities = new[] { "require", "node", "default" };
        foreach (var p in priorities)
        {
            if (entry.TryGetProperty(p, out var v) && TryResolveConditionalExports(v, out target))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryResolveExportsTargetPath(string packageRoot, string targetRel, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        if (string.IsNullOrWhiteSpace(targetRel))
        {
            return false;
        }

        // Node requires targets be relative ("./...") for package exports.
        var trimmed = targetRel.Trim();
        if (!trimmed.StartsWith("./", StringComparison.Ordinal))
        {
            error = $"Unsupported package.json exports target '{targetRel}' (expected relative path starting with './').";
            return false;
        }

        var rel = trimmed.Substring(2);
        var combined = Path.Combine(packageRoot, rel.Replace('/', Path.DirectorySeparatorChar));

        return TryResolveAsFileOrDirectory(combined, out resolvedPath, out error);
    }

    private static string NormalizeSpecifier(string specifier)
    {
        var s = specifier.Trim();
        // Preserve node: prefix for core-module detection elsewhere, but normalize slashes.
        return s.Replace('\\', '/');
    }

    private static bool IsPathLikeSpecifier(string specifier)
    {
        return specifier.StartsWith("./", StringComparison.Ordinal)
               || specifier.StartsWith("../", StringComparison.Ordinal)
               || specifier.StartsWith("/", StringComparison.Ordinal)
               || Path.IsPathRooted(specifier);
    }

    private static string ToAbsolutePath(string specifier, string baseDirectory)
    {
        // If rooted, Path.Combine returns the rooted path.
        var combined = Path.Combine(baseDirectory, specifier.Replace('/', Path.DirectorySeparatorChar));
        return Path.GetFullPath(combined);
    }

    private static IEnumerable<string> WalkUpDirectories(string startDirectory)
    {
        var current = Path.GetFullPath(startDirectory);

        while (!string.IsNullOrEmpty(current))
        {
            yield return current;

            var parent = Path.GetDirectoryName(current);
            if (string.IsNullOrEmpty(parent) || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            current = parent;
        }
    }

    private static void ParseBareSpecifier(string specifier, out string packageName, out string packageSubpath)
    {
        packageName = string.Empty;
        packageSubpath = string.Empty;

        var s = specifier;
        if (s.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
        {
            // Caller should treat node: as a core module; for compile-time npm resolution we keep it invalid.
            s = s.Substring("node:".Length);
        }

        if (s.StartsWith("@", StringComparison.Ordinal))
        {
            // Scoped package: @scope/name[/subpath]
            var firstSlash = s.IndexOf('/', 1);
            if (firstSlash < 0)
            {
                packageName = s;
                return;
            }

            var secondSlash = s.IndexOf('/', firstSlash + 1);
            if (secondSlash < 0)
            {
                packageName = s;
                return;
            }

            packageName = s.Substring(0, secondSlash);
            packageSubpath = s.Substring(secondSlash + 1);
            return;
        }

        // Unscoped: name[/subpath]
        var slash = s.IndexOf('/', StringComparison.Ordinal);
        if (slash < 0)
        {
            packageName = s;
            return;
        }

        packageName = s.Substring(0, slash);
        packageSubpath = s.Substring(slash + 1);
    }
}
