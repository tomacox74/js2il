using System;
using System.Linq;
using System.Text.Json;

namespace Js2IL.Services;

public enum ModuleResolutionMode
{
    Require,
    Import
}

/// <summary>
/// Resolves CommonJS and ESM module specifiers to concrete JavaScript module files at compile time.
///
/// This mirrors Node.js module resolution rules as closely as practical for JS2IL,
/// and supports <c>.js</c>, <c>.mjs</c>, and <c>.cjs</c> targets.
/// </summary>
public sealed class NodeModuleResolver
{
    private readonly IFileSystem _fileSystem;
    private static readonly string[] KnownScriptExtensions = [".js", ".mjs", ".cjs"];
    private static readonly string[] SupportedConditionNames = ["import", "require", "node", "default"];

    public NodeModuleResolver(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool TryResolve(string specifier, string baseDirectory, out string resolvedPath, out string? error)
        => TryResolve(specifier, baseDirectory, ModuleResolutionMode.Require, out resolvedPath, out error);

    public bool TryResolve(string specifier, string baseDirectory, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
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

        if (normalizedSpecifier.StartsWith("#", StringComparison.Ordinal))
        {
            return TryResolvePackageImports(normalizedSpecifier, baseDirectory, resolutionMode, out resolvedPath, out error);
        }

        // Relative/absolute filesystem-like requests.
        if (IsPathLikeSpecifier(normalizedSpecifier))
        {
            var requestPath = ToAbsolutePath(normalizedSpecifier, baseDirectory);
            return TryResolveAsFileOrDirectory(requestPath, resolutionMode, out resolvedPath, out error);
        }

        // Bare specifier: treat as npm package id (potentially with subpath).
        if (!TryResolveBarePackageSpecifier(normalizedSpecifier, baseDirectory, resolutionMode, out resolvedPath, out error))
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

    private bool TryResolveBarePackageSpecifier(string specifier, string baseDirectory, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
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
            var hasIndexCandidate = KnownScriptExtensions
                .Select(ext => Path.Combine(packageRoot, "index" + ext))
                .Any(_fileSystem.FileExists);

            if (!_fileSystem.FileExists(packageJsonPath) && !hasIndexCandidate)
            {
                continue;
            }

            // If subpath is present, resolve within the package.
            if (!string.IsNullOrEmpty(packageSubpath))
            {
                // First try package.json exports (if present).
                if (_fileSystem.FileExists(packageJsonPath))
                {
                    if (TryResolvePackageExports(packageRoot, packageJsonPath, packageSubpath, resolutionMode, out resolvedPath, out error))
                    {
                        return true;
                    }

                    if (error != null)
                    {
                        return false;
                    }
                }

                // Fallback: treat subpath as a filesystem path within package root.
                var subpathPath = Path.Combine(packageRoot, packageSubpath.Replace('/', Path.DirectorySeparatorChar));
                if (TryResolveAsFileOrDirectory(subpathPath, resolutionMode, out resolvedPath, out error))
                {
                    return true;
                }

                // Stop searching up: Node resolves the nearest matching node_modules package directory.
                return false;
            }

            // Package root entry resolution.
            if (_fileSystem.FileExists(packageJsonPath))
            {
                if (TryResolvePackageEntryFromPackageJson(packageRoot, packageJsonPath, resolutionMode, out resolvedPath, out error))
                {
                    return true;
                }

                if (error != null)
                {
                    return false;
                }
            }

            // Fallback: <packageRoot>/index.(js|mjs|cjs)
            foreach (var ext in KnownScriptExtensions)
            {
                var packageIndexCandidate = Path.Combine(packageRoot, "index" + ext);
                if (_fileSystem.FileExists(packageIndexCandidate))
                {
                    resolvedPath = Path.GetFullPath(packageIndexCandidate);
                    return true;
                }
            }

            error = $"Could not resolve entry for package '{packageName}'.";
            return false;
        }

        error = $"Cannot find module '{specifier}' (searched node_modules up from '{baseDirectory}').";
        return false;
    }

    private bool TryResolveAsFileOrDirectory(string requestPath, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        // 1) Exact file
        if (TryAcceptResolvedScriptFile(requestPath, out resolvedPath, out error))
        {
            return true;
        }

        // 2) Append common script extensions
        if (!Path.HasExtension(requestPath))
        {
            foreach (var ext in KnownScriptExtensions)
            {
                var withExtension = requestPath + ext;
                if (TryAcceptResolvedScriptFile(withExtension, out resolvedPath, out error))
                {
                    return true;
                }
            }
        }

        // 3) Directory: package.json -> entry, then index.(js|mjs|cjs)
        var packageJson = Path.Combine(requestPath, "package.json");
        if (_fileSystem.FileExists(packageJson)
            && TryResolvePackageEntryFromPackageJson(requestPath, packageJson, resolutionMode, out resolvedPath, out error))
        {
            return true;
        }

        foreach (var ext in KnownScriptExtensions)
        {
            var indexCandidate = Path.Combine(requestPath, "index" + ext);
            if (_fileSystem.FileExists(indexCandidate))
            {
                resolvedPath = Path.GetFullPath(indexCandidate);
                return true;
            }
        }

        error = $"Cannot resolve module path '{requestPath}' to a supported script file (.js, .mjs, .cjs).";
        return false;
    }

    private bool TryAcceptResolvedScriptFile(string path, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        if (!_fileSystem.FileExists(path))
        {
            return false;
        }

        var full = Path.GetFullPath(path);
        if (!KnownScriptExtensions.Any(ext => full.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
        {
            error = $"Resolved module target '{full}' is not a supported script file (.js, .mjs, .cjs).";
            return false;
        }

        resolvedPath = full;
        return true;
    }

    private bool TryResolvePackageEntryFromPackageJson(string packageRoot, string packageJsonPath, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        try
        {
            var json = _fileSystem.ReadAllText(packageJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Prefer exports if present.
            if (root.TryGetProperty("exports", out var exportsElement))
            {
                if (TryResolvePackageMapTarget(exportsElement, requestKey: ".", mapName: "exports", resolutionMode, out var targetRel, out error)
                    && TryResolveExportsTargetPath(packageRoot, targetRel, resolutionMode, out resolvedPath, out error))
                {
                    return true;
                }

                error ??= $"Package root export '.' is not defined in '{packageJsonPath}'.";
                return false;
            }

            if (root.TryGetProperty("main", out var mainElement) && mainElement.ValueKind == JsonValueKind.String)
            {
                var main = mainElement.GetString();
                if (!string.IsNullOrWhiteSpace(main))
                {
                    var mainPath = Path.Combine(packageRoot, main.Replace('/', Path.DirectorySeparatorChar));
                    if (TryResolveAsFileOrDirectory(mainPath, resolutionMode, out resolvedPath, out error))
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

    private bool TryResolvePackageExports(string packageRoot, string packageJsonPath, string packageSubpath, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
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

            if (TryResolvePackageMapTarget(exportsElement, exportsKey, "exports", resolutionMode, out var targetRel, out error))
            {
                return TryResolveExportsTargetPath(packageRoot, targetRel, resolutionMode, out resolvedPath, out error);
            }

            error ??= $"Package export '{exportsKey}' is not defined in '{packageJsonPath}'.";
            return false;
        }
        catch (Exception ex)
        {
            error = $"Failed to read/parse package.json '{packageJsonPath}': {ex.Message}";
            return false;
        }
    }

    private bool TryResolvePackageImports(string specifier, string baseDirectory, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        foreach (var dir in WalkUpDirectories(baseDirectory))
        {
            var packageJsonPath = Path.Combine(dir, "package.json");
            if (!_fileSystem.FileExists(packageJsonPath))
            {
                continue;
            }

            try
            {
                var json = _fileSystem.ReadAllText(packageJsonPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("imports", out var importsElement))
                {
                    error = $"Cannot resolve package imports specifier '{specifier}' from '{baseDirectory}': nearest package.json '{packageJsonPath}' does not define \"imports\".";
                    return false;
                }

                if (TryResolvePackageMapTarget(importsElement, specifier, "imports", resolutionMode, out var targetRel, out error))
                {
                    return TryResolveImportsTargetPath(dir, targetRel, resolutionMode, out resolvedPath, out error);
                }

                if (error == null)
                {
                    error = $"Package imports specifier '{specifier}' is not defined in '{packageJsonPath}'.";
                }

                return false;
            }
            catch (Exception ex)
            {
                error = $"Failed to read/parse package.json '{packageJsonPath}': {ex.Message}";
                return false;
            }
        }

        error = $"Cannot resolve package imports specifier '{specifier}' from '{baseDirectory}': no containing package.json found.";
        return false;
    }

    private static bool TryResolvePackageMapTarget(JsonElement mapElement, string requestKey, string mapName, ModuleResolutionMode resolutionMode, out string target, out string? error)
    {
        target = string.Empty;
        error = null;

        switch (mapElement.ValueKind)
        {
            case JsonValueKind.String:
                if (string.Equals(mapName, "exports", StringComparison.Ordinal) && requestKey == ".")
                {
                    target = mapElement.GetString() ?? string.Empty;
                    return !string.IsNullOrWhiteSpace(target);
                }

                return false;

            case JsonValueKind.Array:
                foreach (var entry in mapElement.EnumerateArray())
                {
                    if (TryResolvePackageMapTarget(entry, requestKey, mapName, resolutionMode, out target, out error))
                    {
                        return true;
                    }
                }

                return false;

            case JsonValueKind.Object:
                if (LooksLikePackageRequestMap(mapElement, mapName))
                {
                    if (mapElement.TryGetProperty(requestKey, out var exactEntry))
                    {
                        return TryResolvePackageMapEntry(exactEntry, requestKey, mapName, resolutionMode, out target, out error);
                    }

                    if (TryResolveSpecifierPattern(mapElement, requestKey, out var patternEntry, out var wildcardValue)
                        && TryResolvePackageMapEntry(patternEntry, requestKey, mapName, resolutionMode, out var patternTarget, out error))
                    {
                        target = patternTarget.Replace("*", wildcardValue, StringComparison.Ordinal);
                        return !string.IsNullOrWhiteSpace(target);
                    }

                    return false;
                }

                if (string.Equals(mapName, "exports", StringComparison.Ordinal) && requestKey == ".")
                {
                    return TryResolvePackageMapEntry(mapElement, requestKey, mapName, resolutionMode, out target, out error);
                }

                return false;

            default:
                return false;
        }
    }

    private static bool TryResolvePackageMapEntry(JsonElement entry, string requestKey, string mapName, ModuleResolutionMode resolutionMode, out string target, out string? error)
    {
        target = string.Empty;
        error = null;

        switch (entry.ValueKind)
        {
            case JsonValueKind.String:
                target = entry.GetString() ?? string.Empty;
                return !string.IsNullOrWhiteSpace(target);

            case JsonValueKind.Array:
                foreach (var element in entry.EnumerateArray())
                {
                    if (TryResolvePackageMapEntry(element, requestKey, mapName, resolutionMode, out target, out error))
                    {
                        return true;
                    }
                }

                return false;

            case JsonValueKind.Object:
                return TryResolveConditionalPackageTarget(entry, requestKey, mapName, resolutionMode, out target, out error);

            default:
                error = $"Unsupported package.json {mapName} entry shape for '{requestKey}'.";
                return false;
        }
    }

    private static bool LooksLikePackageRequestMap(JsonElement obj, string mapName)
    {
        var prefix = string.Equals(mapName, "imports", StringComparison.Ordinal) ? "#" : ".";
        return obj.EnumerateObject().Any(p => p.Name.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static bool TryResolveSpecifierPattern(JsonElement mapObject, string requestKey, out JsonElement patternEntry, out string wildcardValue)
    {
        patternEntry = default;
        wildcardValue = string.Empty;

        // Only support single '*' patterns.
        foreach (var prop in mapObject.EnumerateObject().Where(p => p.Name.Contains('*')))
        {
            var key = prop.Name;
            var starIndex = key.IndexOf('*', StringComparison.Ordinal);
            if (starIndex < 0)
            {
                continue;
            }

            var prefix = key.Substring(0, starIndex);
            var suffix = key.Substring(starIndex + 1);

            if (!requestKey.StartsWith(prefix, StringComparison.Ordinal) || !requestKey.EndsWith(suffix, StringComparison.Ordinal))
            {
                continue;
            }

            wildcardValue = requestKey.Substring(prefix.Length, requestKey.Length - prefix.Length - suffix.Length);
            patternEntry = prop.Value;
            return true;
        }

        return false;
    }

    private static bool TryResolveConditionalPackageTarget(JsonElement entry, string requestKey, string mapName, ModuleResolutionMode resolutionMode, out string target, out string? error)
    {
        target = string.Empty;
        error = null;

        var priorities = resolutionMode == ModuleResolutionMode.Import
            ? new[] { "import", "node", "default" }
            : new[] { "require", "node", "default" };

        foreach (var priority in priorities)
        {
            if (!entry.TryGetProperty(priority, out var value))
            {
                continue;
            }

            if (TryResolvePackageMapEntry(value, requestKey, mapName, resolutionMode, out target, out error))
            {
                return true;
            }

            if (error != null)
            {
                return false;
            }
        }

        var conditionKeys = entry.EnumerateObject()
            .Select(p => p.Name)
            .Where(name => !name.StartsWith(".", StringComparison.Ordinal) && !name.StartsWith("#", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (conditionKeys.Length == 0)
        {
            return false;
        }

        var supportedConditions = conditionKeys
            .Where(name => SupportedConditionNames.Contains(name, StringComparer.Ordinal))
            .ToArray();

        if (supportedConditions.Length > 0)
        {
            var ignoredUnsupportedConditions = conditionKeys
                .Where(name => !SupportedConditionNames.Contains(name, StringComparer.Ordinal))
                .ToArray();

            error = $"No matching package.json {mapName} conditions for '{requestKey}' in {DescribeResolutionMode(resolutionMode)} mode. Available supported conditions: {string.Join(", ", supportedConditions)}.";
            if (ignoredUnsupportedConditions.Length > 0)
            {
                error += $" Ignored unsupported conditions: {string.Join(", ", ignoredUnsupportedConditions)}.";
            }

            return false;
        }

        error = $"Unsupported package.json {mapName} conditions for '{requestKey}': {string.Join(", ", conditionKeys)}. Supported conditions: import, require, node, default.";
        return false;
    }

    private bool TryResolveExportsTargetPath(string packageRoot, string targetRel, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
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

        return TryResolveAsFileOrDirectory(combined, resolutionMode, out resolvedPath, out error);
    }

    private bool TryResolveImportsTargetPath(string packageRoot, string targetRel, ModuleResolutionMode resolutionMode, out string resolvedPath, out string? error)
    {
        resolvedPath = string.Empty;
        error = null;

        if (string.IsNullOrWhiteSpace(targetRel))
        {
            return false;
        }

        // Keep the first closure slice deterministic: resolve only package-local targets.
        var trimmed = targetRel.Trim();
        if (!trimmed.StartsWith("./", StringComparison.Ordinal))
        {
            error = $"Unsupported package.json imports target '{targetRel}' (expected relative path starting with './').";
            return false;
        }

        var rel = trimmed.Substring(2);
        var combined = Path.Combine(packageRoot, rel.Replace('/', Path.DirectorySeparatorChar));

        return TryResolveAsFileOrDirectory(combined, resolutionMode, out resolvedPath, out error);
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

    private static string DescribeResolutionMode(ModuleResolutionMode resolutionMode)
        => resolutionMode == ModuleResolutionMode.Import ? "import" : "require";
}
