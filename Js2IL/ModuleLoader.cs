using Acornima.Ast;
using Js2IL.Services;
using Js2IL.Validation;
using System.Globalization;
using System.Text;

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
        var sourceFileForDebugging = modulePath;
        if (_fileSystem is ISourceFilePathResolver resolver
            && resolver.TryGetSourceFilePath(modulePath, out var resolved)
            && !string.IsNullOrWhiteSpace(resolved))
        {
            sourceFileForDebugging = resolved;
        }

        try
        {
            ast = _parser.ParseJavaScript(jsSource, sourceFileForDebugging);
        }
        catch (Exception ex)
        {
            module = null;
            diagnostics.AddParseError(modulePath, ex.Message);
            return false;
        }

        if (!TryRewriteStaticModuleSyntax(jsSource, ast, out var rewrittenSource, out var rewriteError))
        {
            module = null;
            diagnostics.AddParseError(modulePath, rewriteError ?? "Failed to rewrite ES module syntax.");
            return false;
        }

        if (!string.Equals(jsSource, rewrittenSource, StringComparison.Ordinal))
        {
            jsSource = rewrittenSource;
            try
            {
                ast = _parser.ParseJavaScript(jsSource, sourceFileForDebugging);
            }
            catch (Exception ex)
            {
                module = null;
                diagnostics.AddParseError(modulePath, $"Failed to parse rewritten ES module syntax: {ex.Message}");
                return false;
            }
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

    private static readonly string EsModuleInteropPrelude =
@"function __js2il_esm_mark() {
    if (!Object.prototype.hasOwnProperty.call(exports, ""__esModule"")) {
        Object.defineProperty(exports, ""__esModule"", { value: true, enumerable: false, configurable: true });
    }
}
function __js2il_esm_default(mod) {
    return (mod != null && Object.prototype.hasOwnProperty.call(mod, ""default"")) ? mod.default : mod;
}
function __js2il_esm_namespace(mod) {
    if (mod != null && mod.__esModule === true) {
        return mod;
    }
    var ns = { default: mod };
    ns[""module.exports""] = mod;
    if (mod != null && (typeof mod === ""object"" || typeof mod === ""function"")) {
        for (var key in mod) {
            if (!Object.prototype.hasOwnProperty.call(mod, key)) {
                continue;
            }
            if (key !== ""default"" && key !== ""module.exports"" && key !== ""__esModule"") {
                ns[key] = mod[key];
            }
        }
    }
    return ns;
}
function __js2il_esm_export(name, getter) {
    __js2il_esm_mark();
    Object.defineProperty(exports, name, { enumerable: true, configurable: true, get: getter });
}";

    private bool TryRewriteStaticModuleSyntax(string source, Acornima.Ast.Program ast, out string rewrittenSource, out string? error)
    {
        rewrittenSource = source;
        error = null;

        var topLevelModuleStatements = new HashSet<Node>(ReferenceEqualityComparer.Instance);
        foreach (var statement in ast.Body)
        {
            if (IsStaticModuleDeclaration(statement))
            {
                topLevelModuleStatements.Add(statement);
            }
        }

        if (topLevelModuleStatements.Count == 0)
        {
            return true;
        }

        Node? nestedStaticModuleNode = null;
        _parser.VisitAst(ast, node =>
        {
            if (nestedStaticModuleNode != null)
            {
                return;
            }

            if (IsStaticModuleDeclaration(node) && !topLevelModuleStatements.Contains(node))
            {
                nestedStaticModuleNode = node;
            }
        });

        if (nestedStaticModuleNode != null)
        {
            error = $"Static import/export declarations are only supported at top level (line {nestedStaticModuleNode.Location.Start.Line}).";
            return false;
        }

        var seenNonDirectiveNonModuleStatement = false;
        foreach (var statement in ast.Body)
        {
            if (IsStaticModuleDeclaration(statement))
            {
                if (seenNonDirectiveNonModuleStatement)
                {
                    error = $"Static import/export declarations must appear before non-directive top-level statements in this MVP implementation (line {statement.Location.Start.Line}).";
                    return false;
                }

                continue;
            }

            if (!IsDirectivePrologueStatement(statement))
            {
                seenNonDirectiveNonModuleStatement = true;
            }
        }

        var builder = new StringBuilder(source.Length + 512);
        var cursor = 0;
        var tempCounter = 0;
        var rewrittenAny = false;

        foreach (var statement in ast.Body)
        {
            if (!TryRewriteTopLevelModuleStatement(statement, source, ref tempCounter, out var rewrittenStatement, out var rewriteError))
            {
                if (!string.IsNullOrWhiteSpace(rewriteError))
                {
                    error = rewriteError;
                    return false;
                }
                continue;
            }

            rewrittenAny = true;
            builder.Append(source, cursor, statement.Start - cursor);

            builder.AppendLine(rewrittenStatement);
            cursor = statement.End;
        }

        if (!rewrittenAny)
        {
            return true;
        }

        builder.Append(source, cursor, source.Length - cursor);
        builder.AppendLine();
        builder.AppendLine(EsModuleInteropPrelude);
        rewrittenSource = builder.ToString();
        return true;
    }

    private static bool TryRewriteTopLevelModuleStatement(
        Statement statement,
        string source,
        ref int tempCounter,
        out string rewrittenStatement,
        out string? error)
    {
        rewrittenStatement = string.Empty;
        error = null;

        try
        {
            switch (statement)
            {
                case ImportDeclaration importDeclaration:
                    rewrittenStatement = RewriteImportDeclaration(importDeclaration, source, ref tempCounter);
                    return true;

                case ExportNamedDeclaration exportNamedDeclaration:
                    rewrittenStatement = RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter);
                    return true;

                case ExportDefaultDeclaration exportDefaultDeclaration:
                    rewrittenStatement = RewriteExportDefaultDeclaration(exportDefaultDeclaration, source, ref tempCounter);
                    return true;

                case ExportAllDeclaration exportAllDeclaration:
                    rewrittenStatement = RewriteExportAllDeclaration(exportAllDeclaration, source, ref tempCounter);
                    return true;

                default:
                    return false;
            }
        }
        catch (NotSupportedException ex)
        {
            error = $"{ex.Message} (line {statement.Location.Start.Line}).";
            return false;
        }
    }

    private static string RewriteImportDeclaration(ImportDeclaration importDeclaration, string source, ref int tempCounter)
    {
        if (importDeclaration.Attributes.Count > 0)
        {
            throw new NotSupportedException("Import attributes are not yet supported");
        }

        var importSourceLiteral = GetNodeSource(source, importDeclaration.Source);
        if (importDeclaration.Specifiers.Count == 0)
        {
            return $"require({importSourceLiteral});";
        }

        var moduleTemp = $"__js2il_esm_mod_{tempCounter++}";
        var builder = new StringBuilder();
        builder.Append("var ").Append(moduleTemp).Append(" = require(").Append(importSourceLiteral).AppendLine(");");

        foreach (var specifier in importDeclaration.Specifiers)
        {
            switch (specifier)
            {
                case ImportDefaultSpecifier defaultSpecifier:
                    if (defaultSpecifier.Local is not Identifier defaultLocal)
                    {
                        throw new NotSupportedException("Default import local binding must be an identifier");
                    }
                    builder.Append("var ").Append(defaultLocal.Name).Append(" = __js2il_esm_default(").Append(moduleTemp).AppendLine(");");
                    break;

                case ImportNamespaceSpecifier namespaceSpecifier:
                    if (namespaceSpecifier.Local is not Identifier namespaceLocal)
                    {
                        throw new NotSupportedException("Namespace import local binding must be an identifier");
                    }
                    builder.Append("var ").Append(namespaceLocal.Name).Append(" = __js2il_esm_namespace(").Append(moduleTemp).AppendLine(");");
                    break;

                case ImportSpecifier importSpecifier:
                    if (importSpecifier.Local is not Identifier importLocal)
                    {
                        throw new NotSupportedException("Named import local binding must be an identifier");
                    }
                    var importedName = GetExpressionName(importSpecifier.Imported);
                    builder.Append("var ").Append(importLocal.Name).Append(" = ")
                        .Append(moduleTemp).Append("[\"").Append(EscapeJsString(importedName)).AppendLine("\"];");
                    break;

                default:
                    throw new NotSupportedException($"Unsupported import specifier form '{specifier.TypeText}'");
            }
        }

        return builder.ToString();
    }

    private static string RewriteExportNamedDeclaration(ExportNamedDeclaration exportNamedDeclaration, string source, ref int tempCounter)
    {
        if (exportNamedDeclaration.Attributes.Count > 0)
        {
            throw new NotSupportedException("Export attributes are not yet supported");
        }

        var builder = new StringBuilder();

        if (exportNamedDeclaration.Declaration != null)
        {
            if (exportNamedDeclaration.Declaration is not Node declarationNode)
            {
                throw new NotSupportedException("Unsupported export declaration node");
            }

            builder.AppendLine(GetNodeSource(source, declarationNode));

            switch (exportNamedDeclaration.Declaration)
            {
                case VariableDeclaration variableDeclaration:
                    var bindingNames = new List<string>();
                    foreach (var variableDeclarator in variableDeclaration.Declarations)
                    {
                        CollectBindingNames(variableDeclarator.Id, bindingNames);
                    }

                    foreach (var name in bindingNames.Distinct(StringComparer.Ordinal))
                    {
                        AppendExportGetter(builder, name, name);
                    }
                    break;

                case FunctionDeclaration functionDeclaration when functionDeclaration.Id != null:
                    AppendExportGetter(builder, functionDeclaration.Id.Name, functionDeclaration.Id.Name);
                    break;

                case ClassDeclaration classDeclaration when classDeclaration.Id != null:
                    AppendExportGetter(builder, classDeclaration.Id.Name, classDeclaration.Id.Name);
                    break;

                default:
                    throw new NotSupportedException("Unsupported export declaration form");
            }

            return builder.ToString();
        }

        if (exportNamedDeclaration.Source != null)
        {
            var moduleTemp = $"__js2il_esm_mod_{tempCounter++}";
            var exportSourceLiteral = GetNodeSource(source, exportNamedDeclaration.Source);
            builder.Append("var ").Append(moduleTemp).Append(" = require(").Append(exportSourceLiteral).AppendLine(");");

            foreach (var specifier in exportNamedDeclaration.Specifiers)
            {
                var localName = GetExpressionName(specifier.Local);
                var exportName = GetExpressionName(specifier.Exported);
                AppendExportGetter(builder, exportName, $"{moduleTemp}[\"{EscapeJsString(localName)}\"]");
            }

            return builder.ToString();
        }

        foreach (var specifier in exportNamedDeclaration.Specifiers)
        {
            var localName = GetExpressionName(specifier.Local);
            var exportName = GetExpressionName(specifier.Exported);
            AppendExportGetter(builder, exportName, localName);
        }

        return builder.ToString();
    }

    private static string RewriteExportDefaultDeclaration(ExportDefaultDeclaration exportDefaultDeclaration, string source, ref int tempCounter)
    {
        var builder = new StringBuilder();

        switch (exportDefaultDeclaration.Declaration)
        {
            case FunctionDeclaration functionDeclaration when functionDeclaration.Id != null:
                builder.AppendLine(GetNodeSource(source, functionDeclaration));
                AppendExportGetter(builder, "default", functionDeclaration.Id.Name);
                return builder.ToString();

            case ClassDeclaration classDeclaration when classDeclaration.Id != null:
                builder.AppendLine(GetNodeSource(source, classDeclaration));
                AppendExportGetter(builder, "default", classDeclaration.Id.Name);
                return builder.ToString();
        }

        if (exportDefaultDeclaration.Declaration is not Node declarationNode)
        {
            throw new NotSupportedException("Unsupported export default declaration node");
        }

        var defaultTemp = $"__js2il_esm_default_{tempCounter++}";
        builder.Append("var ").Append(defaultTemp).Append(" = ").Append(GetNodeSource(source, declarationNode)).AppendLine(";");
        AppendExportGetter(builder, "default", defaultTemp);
        return builder.ToString();
    }

    private static string RewriteExportAllDeclaration(ExportAllDeclaration exportAllDeclaration, string source, ref int tempCounter)
    {
        if (exportAllDeclaration.Attributes.Count > 0)
        {
            throw new NotSupportedException("Export attributes are not yet supported");
        }

        var moduleTemp = $"__js2il_esm_mod_{tempCounter++}";
        var exportSourceLiteral = GetNodeSource(source, exportAllDeclaration.Source);
        var builder = new StringBuilder();
        builder.Append("var ").Append(moduleTemp).Append(" = require(").Append(exportSourceLiteral).AppendLine(");");

        if (exportAllDeclaration.Exported != null)
        {
            var exportName = GetExpressionName(exportAllDeclaration.Exported);
            AppendExportGetter(builder, exportName, $"__js2il_esm_namespace({moduleTemp})");
            return builder.ToString();
        }

        var keyName = $"__js2il_esm_key_{tempCounter++}";
        builder.Append("for (var ").Append(keyName).Append(" in ").Append(moduleTemp).AppendLine(") {");
        builder.Append("  if (!Object.prototype.hasOwnProperty.call(")
            .Append(moduleTemp)
            .Append(", ")
            .Append(keyName)
            .AppendLine(")) { continue; }");
        builder.Append("  if (")
            .Append(keyName)
            .AppendLine(" === \"default\" || " + keyName + " === \"__esModule\" || " + keyName + " === \"module.exports\") { continue; }");
        builder.Append("  (function(__k) { __js2il_esm_export(__k, function() { return ")
            .Append(moduleTemp)
            .Append("[__k]; }); })(")
            .Append(keyName)
            .AppendLine(");");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void AppendExportGetter(StringBuilder builder, string exportName, string valueExpression)
    {
        builder.Append("__js2il_esm_export(\"")
            .Append(EscapeJsString(exportName))
            .Append("\", function() { return ")
            .Append(valueExpression)
            .AppendLine("; });");
    }

    private static string GetNodeSource(string source, Node node)
    {
        return source.Substring(node.Start, node.End - node.Start);
    }

    private static string GetExpressionName(Expression expression)
    {
        return expression switch
        {
            Identifier identifier => identifier.Name,
            Literal literal when literal.Value is string s => s,
            Literal literal when literal.Value != null => Convert.ToString(literal.Value, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => throw new NotSupportedException("Only identifier and string-literal names are supported in import/export specifiers")
        };
    }

    private static void CollectBindingNames(Node pattern, List<string> names)
    {
        switch (pattern)
        {
            case Identifier identifier:
                names.Add(identifier.Name);
                break;

            case AssignmentPattern assignmentPattern:
                CollectBindingNames(assignmentPattern.Left, names);
                break;

            case RestElement restElement:
                CollectBindingNames(restElement.Argument, names);
                break;

            case ArrayPattern arrayPattern:
                foreach (var element in arrayPattern.Elements)
                {
                    if (element != null)
                    {
                        CollectBindingNames(element, names);
                    }
                }
                break;

            case ObjectPattern objectPattern:
                foreach (var property in objectPattern.Properties)
                {
                    switch (property)
                    {
                        case Property objectProperty when objectProperty.Value != null:
                            CollectBindingNames(objectProperty.Value, names);
                            break;

                        case Property objectProperty:
                            CollectBindingNames(objectProperty.Key, names);
                            break;

                        case RestElement objectRestElement:
                            CollectBindingNames(objectRestElement.Argument, names);
                            break;
                    }
                }
                break;

            default:
                throw new NotSupportedException($"Unsupported export binding pattern '{pattern.TypeText}'");
        }
    }

    private static string EscapeJsString(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private static bool IsDirectivePrologueStatement(Statement statement)
    {
        return statement is ExpressionStatement { Expression: Literal literal } && literal.Value is string;
    }

    private static bool IsStaticModuleDeclaration(Node node)
    {
        return node is ImportDeclaration
            or ExportNamedDeclaration
            or ExportDefaultDeclaration
            or ExportAllDeclaration;
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

        s = TrimKnownModuleExtension(s);

        return s;
    }

    private static string TrimKnownModuleExtension(string value)
    {
        if (value.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".mjs", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".cjs", StringComparison.OrdinalIgnoreCase))
        {
            return Path.ChangeExtension(value, null) ?? value;
        }

        return value;
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
            // Handle require() calls
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
            
            // Handle import() expressions
            if (node is Acornima.Ast.ImportExpression importExpr)
            {
                // Validation ensures Source is a StringLiteral
                if (importExpr.Source is Acornima.Ast.Literal lit && lit.Value is string specifier)
                {
                    dependencies.Add(specifier);
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
                // Handle require() calls
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
                
                // Handle import() expressions
                if (node is Acornima.Ast.ImportExpression importExpr)
                {
                    if (importExpr.Source is Acornima.Ast.Literal lit && lit.Value is string specifier)
                    {
                        dependencies.Add(specifier);
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
