using Js2IL.Services;
using Js2IL.Validation;

namespace Js2IL;

/// <summary>
/// The module loader reads JavaScript files and produces ModuleDefinition instances.
/// </summary>
/// <remarks>
/// This class only parses and validates the javascript.
/// This includes also checking for unsupported javascript features
/// </remarks>
public class ModuleLoader
{
    private readonly JavaScriptParser _parser = new JavaScriptParser();
    private readonly JavaScriptAstValidator _validator;
    private readonly IFileSystem _fileSystem;
    private readonly NodeModuleResolver _moduleResolver;
    private readonly ILogger _logger;

    private readonly bool _verbose;

    public ModuleLoader(CompilerOptions options, IFileSystem fileSystem, NodeModuleResolver moduleResolver, ILogger logger)
    {
        _verbose = options.Verbose;
        _validator = new JavaScriptAstValidator(options.StrictMode);
        _fileSystem = fileSystem;
        _moduleResolver = moduleResolver;
        _logger = logger;
    }

    public Modules? LoadModules(string modulePath, string? rootModuleIdOverride = null)
    {
        var rootModulePath = Path.GetFullPath(modulePath);

        var diagnostics = new ModuleLoadDiagnostics(rootModulePath);
        var moduleCache = new Dictionary<string, ModuleDefinition>();
        var hadAnyErrors = false;
        ModuleDefinition? rootModule = null;

        void LoadRecursive(string currentPath, string? requestedAliasModuleId = null)
        {
            if (moduleCache.ContainsKey(currentPath))
            {
                if (!string.IsNullOrWhiteSpace(requestedAliasModuleId)
                    && moduleCache.TryGetValue(currentPath, out var cached))
                {
                    AddAliasIfMissing(cached, requestedAliasModuleId);
                }
                return;
            }

            var isRoot = string.Equals(currentPath, rootModulePath, StringComparison.OrdinalIgnoreCase);
            var loadOk = TryLoadAndParseModule(
                currentPath,
                rootModulePath,
                diagnostics,
                isRoot ? rootModuleIdOverride : null,
                requestedAliasModuleId,
                out var module);
            if (module is null)
            {
                hadAnyErrors = true;
                return;
            }

            if (string.Equals(currentPath, rootModulePath, StringComparison.OrdinalIgnoreCase))
            {
                rootModule = module;
            }

            // Preserve insertion order (and therefore emitted module order) matching the old loader.
            moduleCache[currentPath] = module;
            if (!loadOk)
            {
                hadAnyErrors = true;
            }

            // Continue walking the dependency graph even if this module failed validation.
            // Dependency extraction must be best-effort to avoid crashing when validation failed.
            foreach (var dep in GetModuleDependenciesBestEffort(module))
            {
                // Skip Node built-in modules that are provided by the runtime.
                // (We do not compile their sources.)
                if (!dep.StartsWith(".", StringComparison.Ordinal)
                    && !dep.StartsWith("/", StringComparison.Ordinal)
                    && JavaScriptRuntime.Node.NodeModuleRegistry.TryGetModuleType(dep, out _))
                {
                    continue;
                }

                var baseDir = Path.GetDirectoryName(module.Path) ?? ".";
                if (!_moduleResolver.TryResolve(dep, baseDir, out var resolvedDepPath, out var resolveError))
                {
                    hadAnyErrors = true;
                    diagnostics.AddParseError(
                        module.Path,
                        $"Failed to resolve require('{dep}') from '{module.Path}': {resolveError}");
                    continue;
                }

                // Track alias module ids for bare specifiers (e.g. require('pkg') -> canonical pkg/lib/index).
                var aliasId = TryNormalizeBareAlias(dep);
                LoadRecursive(resolvedDepPath, aliasId);
            }
        }

        LoadRecursive(rootModulePath, requestedAliasModuleId: TryNormalizeBareAlias(rootModuleIdOverride));

        diagnostics.Flush(_logger);

        if (rootModule is null)
        {
            return null;
        }

        // Ensure the root module can be loaded by a stable host-facing id when compilation
        // starts from an explicit module id (CLI --moduleid).
        // (This is intentionally redundant with the recursive load request aliasing.)
        var rootOverrideAlias = TryNormalizeBareAlias(rootModuleIdOverride);
        if (!string.IsNullOrWhiteSpace(rootOverrideAlias))
        {
            AddAliasIfMissing(rootModule, rootOverrideAlias);

            // If the override is a bare package id (e.g. "@scope/pkg"), ensure that exact package id
            // is also published as an alias to the resolved entry module.
            if (TryGetPackageIdentity(rootModule.Path, out var rootPackageName, out _)
                && !string.Equals(rootOverrideAlias, rootPackageName, StringComparison.OrdinalIgnoreCase))
            {
                AddAliasIfMissing(rootModule, rootPackageName);
            }
        }

        if (hadAnyErrors)
        {
            return null;
        }

        var modules = new Modules { rootModule = rootModule };
        foreach (var kvp in moduleCache)
        {
            modules._modules[kvp.Key] = kvp.Value;
        }

        return modules;
    }

    private bool TryLoadAndParseModule(
        string modulePath,
        string rootModulePath,
        ModuleLoadDiagnostics diagnostics,
        string? rootModuleIdOverride,
        string? requestedAliasModuleId,
        out ModuleDefinition? module)
    {
        string jsSource;
        try
        {
            jsSource = _fileSystem.ReadAllText(modulePath);
        }
        catch (Exception ex)
        {
            module = null;
            diagnostics.AddParseError(modulePath, ex.Message);
            return false;
        }

        Acornima.Ast.Program ast;
        try
        {
            var sourceFileForDebugging = modulePath;
            if (_fileSystem is ISourceFilePathResolver resolver
                && resolver.TryGetSourceFilePath(modulePath, out var resolved)
                && !string.IsNullOrWhiteSpace(resolved))
            {
                sourceFileForDebugging = resolved;
            }

            ast = _parser.ParseJavaScript(jsSource, sourceFileForDebugging);
        }
        catch (Exception ex)
        {
            module = null;
            diagnostics.AddParseError(modulePath, ex.Message);
            return false;
        }

        // Compute canonical logical module id (runtime-facing) and CLR type name.
        var canonicalModuleId = ComputeCanonicalModuleId(modulePath, rootModulePath);
        var manifestDefaultId = JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdForManifestFromPath(modulePath, rootModulePath);

        // Root module id override (used by CLI --moduleid) should become the alias id
        // that points at the root module's canonical id.
        var isPackageModule = IsUnderNodeModules(modulePath);

        // Internal unique key used by compilation registries (must be globally unique within an assembly).
        // For package modules, prefer the reversible encoding to avoid collisions.
        var internalKey = isPackageModule
            ? NodeModuleResolver.EncodeModuleIdToClrIdentifier(canonicalModuleId)
            : JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdFromPath(modulePath, rootModulePath);

        // Human-readable CLR names for the generated module root type.
        // For packages we group types by package name and use within-package path segments for type names.
        // This keeps ILSpy output readable while preserving a unique internal module key.
        string clrNamespace;
        string clrTypeName;
        if (isPackageModule && TryGetPackageIdentity(modulePath, out var packageName, out var withinPackageNoExt))
        {
            clrNamespace = "Packages." + SanitizeClrIdentifier(packageName);
            clrTypeName = SanitizeClrIdentifier(string.IsNullOrWhiteSpace(withinPackageNoExt) ? "index" : withinPackageNoExt);
        }
        else
        {
            clrNamespace = "Modules";
            clrTypeName = internalKey;
        }

        module = new ModuleDefinition
        {
            Path = modulePath,
            // Canonical logical id used for runtime mapping.
            ModuleId = canonicalModuleId,
            // Internal module key used by compilation registries.
            Name = internalKey,
            // CLR identity for the emitted module root type (readable in ILSpy).
            ClrNamespace = clrNamespace,
            ClrTypeName = clrTypeName,
            IsPackageModule = isPackageModule,
            Ast = ast
        };

        // For local modules, preserve existing manifest-like ids for host discovery.
        // (This is distinct from CLR name and distinct from package canonical ids.)
        // If the module is not under node_modules, alias the canonical id to the legacy manifest id
        // when they differ (they typically match).
        if (!isPackageModule && !string.Equals(canonicalModuleId, manifestDefaultId, StringComparison.OrdinalIgnoreCase))
        {
            AddAliasIfMissing(module, manifestDefaultId);
        }

        // Bare require('pkg') should remain a stable host-facing id.
        if (!string.IsNullOrWhiteSpace(requestedAliasModuleId))
        {
            AddAliasIfMissing(module, requestedAliasModuleId);
        }

        if (!string.IsNullOrWhiteSpace(rootModuleIdOverride))
        {
            var rootAlias = TryNormalizeBareAlias(rootModuleIdOverride);
            if (!string.IsNullOrWhiteSpace(rootAlias))
            {
                AddAliasIfMissing(module, rootAlias);
            }
        }

        if (this._verbose)
        {
            _logger.WriteLine("AST Structure:");
            _parser.VisitAst(ast, node =>
            {
                var message = $"Node Type: {node.Type}";
                if (node is Acornima.Ast.NumericLiteral num)
                    message += $", Value: {num.Value}";
                if (node is Acornima.Ast.UnaryExpression unary)
                    message += $", Operator: {unary.Operator}";
                _logger.WriteLine(message);
            });
        }

        if (this._verbose)
        {
            _logger.WriteLine($"\nValidating module: {modulePath}");
        }
        var validationResult = _validator.Validate(ast);
        if (!validationResult.IsValid)
        {
            diagnostics.AddValidationErrors(module.ModuleId, modulePath, validationResult.Errors);
            return false;
        }

        if (validationResult.Warnings.Any())
        {
            diagnostics.AddWarnings(module.ModuleId, modulePath, validationResult.Warnings);
        }

        return true;
    }

    private static void AddAliasIfMissing(ModuleDefinition module, string alias)
    {
        var normalized = alias.Replace('\\', '/').Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        // Avoid aliasing to the canonical id.
        if (string.Equals(normalized, module.ModuleId, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!module.AliasModuleIds.Contains(normalized, StringComparer.OrdinalIgnoreCase))
        {
            module.AliasModuleIds.Add(normalized);
        }
    }

    private static string? TryNormalizeBareAlias(string? specifier)
    {
        if (string.IsNullOrWhiteSpace(specifier))
        {
            return null;
        }

        var s = specifier.Trim().Replace('\\', '/');
        if (s.StartsWith("./", StringComparison.Ordinal) || s.StartsWith("../", StringComparison.Ordinal) || s.StartsWith("/", StringComparison.Ordinal))
        {
            return null;
        }

        if (s.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (s.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
        {
            s = s.Substring(0, s.Length - 3);
        }

        return s;
    }

    private static string ComputeCanonicalModuleId(string modulePath, string rootModulePath)
    {
        // Package modules: canonical id is <packageName>/<relativePathWithinPackageNoExt>
        if (TryGetPackageIdentity(modulePath, out var packageName, out var withinPackageNoExt))
        {
            return withinPackageNoExt.Length == 0
                ? packageName
                : packageName + "/" + withinPackageNoExt;
        }

        // Local modules: use the existing manifest id (path-like relative to root directory, no .js).
        return JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdForManifestFromPath(modulePath, rootModulePath);
    }

    private static bool IsUnderNodeModules(string modulePath)
    {
        var full = Path.GetFullPath(modulePath);
        var parts = full.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return parts.Any(p => string.Equals(p, "node_modules", StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryGetPackageIdentity(string modulePath, out string packageName, out string withinPackageNoExt)
    {
        packageName = string.Empty;
        withinPackageNoExt = string.Empty;

        var full = Path.GetFullPath(modulePath);
        var segments = full.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var nodeModulesIndex = Array.FindLastIndex(segments, s => string.Equals(s, "node_modules", StringComparison.OrdinalIgnoreCase));
        if (nodeModulesIndex < 0 || nodeModulesIndex + 1 >= segments.Length)
        {
            return false;
        }

        var pkgStart = nodeModulesIndex + 1;
        if (segments[pkgStart].StartsWith("@", StringComparison.Ordinal) && pkgStart + 1 < segments.Length)
        {
            packageName = segments[pkgStart] + "/" + segments[pkgStart + 1];
            pkgStart += 2;
        }
        else
        {
            packageName = segments[pkgStart];
            pkgStart += 1;
        }

        // Remaining segments represent the path within the package to the file.
        var withinSegments = segments.Skip(pkgStart).ToArray();
        if (withinSegments.Length == 0)
        {
            withinPackageNoExt = string.Empty;
            return true;
        }

        // Remove extension from the last segment.
        var last = withinSegments[withinSegments.Length - 1];
        last = last.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            ? last.Substring(0, last.Length - 3)
            : Path.ChangeExtension(last, null) ?? last;
        withinSegments[withinSegments.Length - 1] = last;

        withinPackageNoExt = string.Join("/", withinSegments);
        return true;
    }

    private static string SanitizeClrIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "_";
        }

        var s = value.Trim().Replace('\\', '/');

        var sb = new System.Text.StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                sb.Append(c);
            }
            else
            {
                sb.Append('_');
            }
        }

        if (sb.Length == 0)
        {
            return "_";
        }

        if (char.IsDigit(sb[0]))
        {
            sb.Insert(0, '_');
        }

        return sb.ToString();
    }

    private sealed class ModuleLoadDiagnostics
    {
        private readonly string _rootModulePath;
        private readonly List<(string ModuleName, string ModulePath, string Message)> _parseErrors = new();
        private readonly List<(string ModuleName, string ModulePath, string Message)> _validationErrors = new();
        private readonly List<(string ModuleName, string ModulePath, string Message)> _warnings = new();

        public ModuleLoadDiagnostics(string rootModulePath)
        {
            _rootModulePath = rootModulePath;
        }

        public void AddParseError(string modulePath, string message)
        {
            var moduleName = JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdFromPath(modulePath, _rootModulePath);
            _parseErrors.Add((moduleName, modulePath, message));
        }

        public void AddValidationErrors(string moduleName, string modulePath, IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                _validationErrors.Add((moduleName, modulePath, error));
            }
        }

        public void AddWarnings(string moduleName, string modulePath, IEnumerable<string> warnings)
        {
            foreach (var warning in warnings)
            {
                _warnings.Add((moduleName, modulePath, warning));
            }
        }

        public void Flush(ILogger logger)
        {
            if (_parseErrors.Count > 0)
            {
                logger.WriteLineError("\nParse Errors:");
                foreach (var e in _parseErrors)
                {
                    logger.WriteLineError($"Error: [{e.ModuleName}] {e.ModulePath}: {e.Message}");
                }
            }

            if (_validationErrors.Count > 0)
            {
                logger.WriteLineError("\nValidation Errors:");
                foreach (var e in _validationErrors)
                {
                    logger.WriteLineError($"Error: [{e.ModuleName}] {e.ModulePath}: {e.Message}");
                }
            }

            if (_warnings.Count > 0)
            {
                logger.WriteLine("\nValidation Warnings:");
                foreach (var w in _warnings)
                {
                    logger.WriteLineWarning($"Warning: [{w.ModuleName}] {w.ModulePath}: {w.Message}");
                }
            }
        }
    }

    private IEnumerable<string> GetModuleDependencies(ModuleDefinition module)
    {
        var dependencies = new List<string>();

        _parser.VisitAst(module.Ast, node =>
        {
            if (node is Acornima.Ast.CallExpression callExpr)
            {
                if (callExpr.Callee is Acornima.Ast.Identifier identifier)
                {
                    // validation should have verfied that only string literals are being passed
                    // we cannot do static analysis on dynamic requires
                    if (identifier.Name == "require" && callExpr.Arguments.Count == 1)
                    {
                        var arg = callExpr.Arguments[0];
                        if (arg is Acornima.Ast.StringLiteral strLiteral)
                        {
                            dependencies.Add(strLiteral.Value);
                        }
                        else
                        {
                            // This should not happen due to prior validation
                            // consider this a internal consistency error
                            throw new Exception("Invalid require() argument type detected during dependency extraction.");
                        }
                    }
                }
            }
        });

        return dependencies;
    }

    private IEnumerable<string> GetModuleDependenciesBestEffort(ModuleDefinition module)
    {
        var dependencies = new List<string>();

        try
        {
            _parser.VisitAst(module.Ast, node =>
            {
                if (node is Acornima.Ast.CallExpression callExpr)
                {
                    if (callExpr.Callee is Acornima.Ast.Identifier identifier)
                    {
                        if (identifier.Name == "require" && callExpr.Arguments.Count == 1)
                        {
                            if (callExpr.Arguments[0] is Acornima.Ast.StringLiteral strLiteral)
                            {
                                dependencies.Add(strLiteral.Value);
                            }
                        }
                    }
                }
            });
        }
        catch
        {
            // Best-effort only. If the AST contains unexpected shapes (likely due to validation failures),
            // skip dependency extraction rather than crashing so we can still aggregate diagnostics.
        }

        return dependencies;
    }

    // NOTE: module resolution is handled by NodeModuleResolver to support npm packages.
}