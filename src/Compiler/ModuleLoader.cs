using Acornima.Ast;
using Js2IL.DebugSymbols;
using Js2IL.Services;
using Js2IL.Utilities;
using Js2IL.Validation;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
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
    private readonly ICompilerOutput _ux;
    private readonly Microsoft.Extensions.Logging.ILogger<ModuleLoader> _diagnosticLogger;

    private readonly bool _diagnosticsEnabled;

    public ModuleLoader(
        CompilerOptions options,
        IFileSystem fileSystem,
        NodeModuleResolver moduleResolver,
        ICompilerOutput ux,
        Microsoft.Extensions.Logging.ILogger<ModuleLoader>? diagnosticLogger = null)
    {
        _diagnosticsEnabled = options.DiagnosticsEnabled;
        _validator = new JavaScriptAstValidator(options.StrictMode);
        _fileSystem = fileSystem;
        _moduleResolver = moduleResolver;
        _ux = ux;
        _diagnosticLogger = diagnosticLogger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ModuleLoader>.Instance;
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
            // Module dependencies are resolved before validation so we can still load the rest
            // of the graph and report adjacent errors in one compiler run.
            foreach (var dep in module.Dependencies)
            {
                LoadRecursive(dep.ResolvedPath, dep.RequestedAliasModuleId);
            }
        }

        LoadRecursive(rootModulePath, requestedAliasModuleId: TryNormalizeBareAlias(rootModuleIdOverride));

        diagnostics.Flush(_ux);

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

    public bool LinkModules(Modules modules, ICompilerOutput? logger = null)
    {
        ArgumentNullException.ThrowIfNull(modules);

        foreach (var module in modules._modules.Values)
        {
            var record = module.ModuleRecord ??= new ModuleRecord();
            record.LinkPhase = ModuleLinkPhase.Unlinked;
            record.LinkErrors.Clear();
            record.ResolvedExports.Clear();
            record.EvaluationPhase = ModuleEvaluationPhase.Unevaluated;
            record.EvaluationOrder = -1;
            record.EvaluationComponent = -1;
        }

        var linkedModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var hadErrors = false;
        foreach (var module in modules._modules.Values)
        {
            if (!LinkModule(module, modules, linkedModules))
            {
                hadErrors = true;
            }
        }

        if (hadErrors)
        {
            FlushModuleLinkErrors(modules, logger ?? _ux);
            return false;
        }

        return true;
    }

    public bool PlanModuleEvaluation(Modules modules, ICompilerOutput? logger = null)
    {
        ArgumentNullException.ThrowIfNull(modules);

        var localLogger = logger ?? _ux;
        foreach (var module in modules._modules.Values)
        {
            var record = module.ModuleRecord ??= new ModuleRecord();
            if (record.LinkPhase != ModuleLinkPhase.Linked)
            {
                localLogger.WriteLineError($"Error: [{module.ModuleId}] {module.Path}: module evaluation planning requires successful module linking.");
                record.EvaluationPhase = ModuleEvaluationPhase.EvaluationError;
                return false;
            }

            record.EvaluationPhase = ModuleEvaluationPhase.Unevaluated;
            record.EvaluationOrder = -1;
            record.EvaluationComponent = -1;
        }

        var components = ComputeModuleEvaluationComponents(modules);
        var orderedComponents = TopologicallyOrderComponents(components, modules);
        for (var order = 0; order < orderedComponents.Count; order++)
        {
            foreach (var module in orderedComponents[order])
            {
                var record = module.ModuleRecord!;
                record.EvaluationComponent = components[module.Path];
                record.EvaluationOrder = order;
                record.EvaluationPhase = ModuleEvaluationPhase.Planned;
            }
        }

        return true;
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

        var requestResolutionOk = TryResolveAndRewriteModuleRequests(jsSource, ast, modulePath, rootModulePath, out var moduleDependencies, out var requestRewrittenSource, out var requestRewriteErrors);
        if (!requestResolutionOk)
        {
            foreach (var requestRewriteError in requestRewriteErrors)
            {
                diagnostics.AddParseError(modulePath, requestRewriteError);
            }
        }

        if (!string.Equals(jsSource, requestRewrittenSource, StringComparison.Ordinal))
        {
            jsSource = requestRewrittenSource;
            try
            {
                ast = _parser.ParseJavaScript(jsSource, sourceFileForDebugging);
            }
            catch (Exception ex)
            {
                module = null;
                diagnostics.AddParseError(modulePath, $"Failed to parse module-request-rewritten source: {ex.Message}");
                return false;
            }
        }

        var moduleRecord = BuildModuleRecord(ast, moduleDependencies);

        if (!TryRewriteStaticModuleSyntax(
            jsSource,
            ast,
            sourceFileForDebugging,
            out var rewrittenSource,
            out var topLevelDebugSpanOverrides,
            out var rewriteError))
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

        Dictionary<Node, SourceSpan>? debugSequencePointOverrides = null;
        if (topLevelDebugSpanOverrides.Count > 0)
        {
            if (!TryCreateTopLevelDebugSequencePointOverrides(
                ast,
                topLevelDebugSpanOverrides,
                out debugSequencePointOverrides,
                out var debugOverrideError))
            {
                module = null;
                diagnostics.AddParseError(modulePath, debugOverrideError ?? "Failed to align rewritten debug sequence points.");
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
            Ast = ast,
            ModuleRecord = moduleRecord
        };

        if (debugSequencePointOverrides != null)
        {
            foreach (var (node, span) in debugSequencePointOverrides)
            {
                module.DebugSequencePointOverrides[node] = span;
            }
        }

        module.Dependencies.AddRange(moduleDependencies);

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

        if (this._diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("AST Structure:");
            _parser.VisitAst(ast, node =>
            {
                var message = $"Node Type: {node.Type}";
                if (node is Acornima.Ast.NumericLiteral num)
                    message += $", Value: {num.Value}";
                if (node is Acornima.Ast.UnaryExpression unary)
                    message += $", Operator: {unary.Operator}";
                _diagnosticLogger.LogInformation("{AstNodeMessage}", message);
            });
        }

        if (this._diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("Validating module: {ModulePath}", modulePath);
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

        return requestResolutionOk;
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
function __js2il_esm_get(mod, name) {
    return mod[name];
}
function __js2il_esm_namespace(mod) {
    if (mod != null && mod.__esModule === true) {
        return mod;
    }

    if (mod == null || (typeof mod !== ""object"" && typeof mod !== ""function"")) {
        return { default: mod, ""module.exports"": mod };
    }

    if (Object.prototype.hasOwnProperty.call(mod, ""__js2il_esm_namespace"")) {
        return mod.__js2il_esm_namespace;
    }

    var ns = {};
    Object.defineProperty(ns, ""default"", { enumerable: true, configurable: true, get: function () { return mod; } });
    Object.defineProperty(ns, ""module.exports"", { enumerable: true, configurable: true, get: function () { return mod; } });

    for (var key in mod) {
        if (!Object.prototype.hasOwnProperty.call(mod, key)) {
            continue;
        }
        if (key === ""default"" || key === ""module.exports"" || key === ""__esModule"" || key === ""__js2il_esm_namespace"") {
            continue;
        }
        (function (k) {
            Object.defineProperty(ns, k, { enumerable: true, configurable: true, get: function () { return mod[k]; } });
        })(key);
    }

    try {
        Object.defineProperty(mod, ""__js2il_esm_namespace"", { value: ns, enumerable: false, configurable: false, writable: false });
    } catch (e) {
        // ignore caching failures (non-extensible exports)
    }

    return ns;
}
function __js2il_esm_export(name, getter) {
    __js2il_esm_mark();
    Object.defineProperty(exports, name, { enumerable: true, configurable: true, get: getter });
}";

    private readonly struct TextEdit
    {
        public int Start { get; }
        public int End { get; }
        public string Replacement { get; }

        public TextEdit(int start, int end, string replacement)
        {
            Start = start;
            End = end;
            Replacement = replacement;
        }
    }

    private readonly record struct TopLevelDebugSpanOverride(SourceSpan Span, bool HideGeneratedCallables);

    private static string ApplyTextEdits(string source, List<TextEdit> edits)
    {
        if (edits.Count == 0)
        {
            return source;
        }

        edits.Sort(static (a, b) =>
        {
            var c = b.Start.CompareTo(a.Start);
            if (c != 0) return c;
            return b.End.CompareTo(a.End);
        });

        var sb = new StringBuilder(source);
        foreach (var edit in edits)
        {
            sb.Remove(edit.Start, edit.End - edit.Start);
            sb.Insert(edit.Start, edit.Replacement);
        }
        return sb.ToString();
    }

    private bool TryResolveAndRewriteModuleRequests(
        string source,
        Acornima.Ast.Program ast,
        string modulePath,
        string rootModulePath,
        out List<ModuleDependency> dependencies,
        out string rewrittenSource,
        out List<string> errors)
    {
        var resolvedDependencies = new List<ModuleDependency>();
        rewrittenSource = source;
        var localErrors = new List<string>();

        var baseDirectory = Path.GetDirectoryName(modulePath) ?? ".";
        var edits = new List<TextEdit>();

        void HandleRequest(StringLiteral literal, ModuleResolutionMode resolutionMode, string operationDescription)
        {
            var specifier = literal.Value;
            if (string.IsNullOrWhiteSpace(specifier) || IsBuiltInModuleSpecifier(specifier))
            {
                return;
            }

            if (!_moduleResolver.TryResolve(specifier, baseDirectory, resolutionMode, out var resolvedPath, out var resolveError))
            {
                localErrors.Add($"Failed to resolve {operationDescription} from '{modulePath}': {resolveError}");
                return;
            }

            string? requestedAliasModuleId = null;
            var runtimeSpecifier = specifier;
            if (specifier.StartsWith("#", StringComparison.Ordinal)
                || (resolutionMode == ModuleResolutionMode.Import && IsBarePackageSpecifier(specifier)))
            {
                runtimeSpecifier = ComputeCanonicalModuleId(resolvedPath, rootModulePath);
                if (!string.Equals(runtimeSpecifier, specifier, StringComparison.Ordinal))
                {
                    edits.Add(new TextEdit(literal.Start, literal.End, CreateJsStringLiteral(runtimeSpecifier)));
                }
            }
            else if (resolutionMode == ModuleResolutionMode.Require && IsBarePackageSpecifier(specifier))
            {
                requestedAliasModuleId = TryNormalizeBareAlias(specifier);
            }

            resolvedDependencies.Add(new ModuleDependency
            {
                Request = runtimeSpecifier,
                ResolvedPath = resolvedPath,
                RequestedAliasModuleId = requestedAliasModuleId
            });
        }

        _parser.VisitAst(ast, node =>
        {
            switch (node)
            {
                case ImportDeclaration { Source: StringLiteral sourceLiteral }:
                    HandleRequest(sourceLiteral, ModuleResolutionMode.Import, $"import '{sourceLiteral.Value}'");
                    break;

                case ExportNamedDeclaration { Source: StringLiteral sourceLiteral }:
                    HandleRequest(sourceLiteral, ModuleResolutionMode.Import, $"export-from '{sourceLiteral.Value}'");
                    break;

                case ExportAllDeclaration { Source: StringLiteral sourceLiteral }:
                    HandleRequest(sourceLiteral, ModuleResolutionMode.Import, $"export-all-from '{sourceLiteral.Value}'");
                    break;

                case ImportExpression importExpression when importExpression.Source is StringLiteral sourceLiteral:
                    HandleRequest(sourceLiteral, ModuleResolutionMode.Import, $"import('{sourceLiteral.Value}')");
                    break;

                case CallExpression callExpression
                    when callExpression.Callee is Identifier { Name: "require" }
                         && callExpression.Arguments.Count == 1
                         && callExpression.Arguments[0] is StringLiteral sourceLiteral:
                    HandleRequest(sourceLiteral, ModuleResolutionMode.Require, $"require('{sourceLiteral.Value}')");
                    break;
            }
        });

        dependencies = resolvedDependencies;
        rewrittenSource = ApplyTextEdits(source, edits);
        errors = localErrors;
        return localErrors.Count == 0;
    }

    private static string CreateExportGetterLine(string exportName, string valueExpression)
    {
        return $"__js2il_esm_export(\"{EscapeJsString(exportName)}\", function() {{ return {valueExpression}; }});";
    }

    private static void AppendImportPrelude(
        ImportDeclaration importDeclaration,
        string source,
        ref int tempCounter,
        SourceSpan debugSpan,
        List<(string Text, SourceSpan Span)> importPrelude,
        Dictionary<string, string> importedBindings)
    {
        if (importDeclaration.Attributes.Count > 0)
        {
            throw new NotSupportedException("Import attributes are not yet supported");
        }

        var importSourceLiteral = GetNodeSource(source, importDeclaration.Source);
        if (importDeclaration.Specifiers.Count == 0)
        {
            importPrelude.Add(($"require({importSourceLiteral});", debugSpan));
            return;
        }

        var moduleTemp = $"__js2il_esm_mod_{tempCounter++}";
        importPrelude.Add(($"var {moduleTemp} = require({importSourceLiteral});", debugSpan));

        foreach (var specifier in importDeclaration.Specifiers)
        {
            switch (specifier)
            {
                case ImportDefaultSpecifier defaultSpecifier:
                    if (defaultSpecifier.Local is not Identifier defaultLocal)
                    {
                        throw new NotSupportedException("Default import local binding must be an identifier");
                    }
                    importedBindings[defaultLocal.Name] = $"__js2il_esm_default({moduleTemp})";
                    break;

                case ImportNamespaceSpecifier namespaceSpecifier:
                    if (namespaceSpecifier.Local is not Identifier namespaceLocal)
                    {
                        throw new NotSupportedException("Namespace import local binding must be an identifier");
                    }
                    importPrelude.Add(($"var {namespaceLocal.Name} = __js2il_esm_namespace({moduleTemp});", debugSpan));
                    break;

                case ImportSpecifier importSpecifier:
                    if (importSpecifier.Local is not Identifier importLocal)
                    {
                        throw new NotSupportedException("Named import local binding must be an identifier");
                    }
                    var importedName = GetExpressionName(importSpecifier.Imported);
                    importedBindings[importLocal.Name] = $"__js2il_esm_get({moduleTemp}, \"{EscapeJsString(importedName)}\")";
                    break;

                default:
                    throw new NotSupportedException($"Unsupported import specifier form '{specifier.TypeText}'");
            }
        }
    }

    private static bool TryRewriteImportedBindingReferences(
        string source,
        Acornima.Ast.Program ast,
        IReadOnlyDictionary<string, string> importedBindings,
        out string rewritten,
        out string? error)
    {
        rewritten = source;
        error = null;

        if (importedBindings.Count == 0)
        {
            return true;
        }

        var edits = new List<TextEdit>();

        var scopes = new ScopeStack();
        scopes.PushFunctionScope();

        bool ShouldRewrite(Identifier id)
        {
            if (!importedBindings.ContainsKey(id.Name))
            {
                return false;
            }
            return !scopes.IsDeclared(id.Name);
        }

        void RewriteIdentifier(Identifier id)
        {
            if (!importedBindings.TryGetValue(id.Name, out var replacement))
            {
                return;
            }
            if (scopes.IsDeclared(id.Name))
            {
                return;
            }
            edits.Add(new TextEdit(id.Start, id.End, replacement));
        }

        void ErrorIfImportWrite(Identifier id)
        {
            if (!importedBindings.ContainsKey(id.Name) || scopes.IsDeclared(id.Name))
            {
                return;
            }

            throw new NotSupportedException($"Cannot assign to import binding '{id.Name}'");
        }

        void ErrorIfImportWritePattern(Node pattern)
        {
            var names = new List<string>();
            CollectBindingNames(pattern, names);
            foreach (var name in names)
            {
                if (importedBindings.ContainsKey(name) && !scopes.IsDeclared(name))
                {
                    throw new NotSupportedException($"Cannot assign to import binding '{name}'");
                }
            }
        }

        void DeclarePattern(Node pattern, bool isVarKind)
        {
            var names = new List<string>();
            CollectBindingNames(pattern, names);
            if (names.Count == 0)
            {
                return;
            }

            if (isVarKind)
            {
                scopes.DeclareVar(names);
            }
            else
            {
                scopes.DeclareLexical(names);
            }
        }

        void VisitPattern(Node? pattern)
        {
            switch (pattern)
            {
                case null:
                    return;

                case AssignmentPattern ap:
                    VisitPattern(ap.Left);
                    VisitExpression(ap.Right);
                    return;

                case RestElement re:
                    VisitPattern(re.Argument);
                    return;

                case ArrayPattern arr:
                    foreach (var el in arr.Elements)
                    {
                        VisitPattern(el);
                    }
                    return;

                case ObjectPattern obj:
                    foreach (var prop in obj.Properties)
                    {
                        switch (prop)
                        {
                            case Property p:
                                if (p.Computed)
                                {
                                    VisitExpression(p.Key);
                                }
                                VisitPattern(p.Value);
                                break;
                            case RestElement r:
                                VisitPattern(r.Argument);
                                break;
                        }
                    }
                    return;
            }
        }

        void VisitStatement(Statement? statement)
        {
            switch (statement)
            {
                case null:
                    return;

                case BlockStatement block:
                    scopes.PushBlockScope();
                    foreach (var s in block.Body)
                    {
                        VisitStatement(s);
                    }
                    scopes.Pop();
                    return;

                case ExpressionStatement es:
                    VisitExpression(es.Expression);
                    return;

                case VariableDeclaration vd:
                    {
                        var isVarKind = vd.Kind == VariableDeclarationKind.Var;

                        // Declare names first (shadowing affects defaults/initializers).
                        foreach (var decl in vd.Declarations)
                        {
                            DeclarePattern(decl.Id, isVarKind);
                        }

                        foreach (var decl in vd.Declarations)
                        {
                            VisitPattern(decl.Id);
                            VisitExpression(decl.Init);
                        }
                        return;
                    }

                case FunctionDeclaration fd:
                    if (fd.Id != null)
                    {
                        scopes.DeclareLexical([fd.Id.Name]);
                    }
                    scopes.PushFunctionScope();
                    foreach (var p in fd.Params)
                    {
                        DeclarePattern(p, isVarKind: false);
                    }
                    foreach (var p in fd.Params)
                    {
                        VisitPattern(p);
                    }
                    VisitStatement(fd.Body);
                    scopes.Pop();
                    return;

                case ReturnStatement rs:
                    VisitExpression(rs.Argument);
                    return;

                case IfStatement ifs:
                    VisitExpression(ifs.Test);
                    VisitStatement(ifs.Consequent);
                    VisitStatement(ifs.Alternate);
                    return;

                case WhileStatement ws:
                    VisitExpression(ws.Test);
                    VisitStatement(ws.Body);
                    return;

                case DoWhileStatement dws:
                    VisitStatement(dws.Body);
                    VisitExpression(dws.Test);
                    return;

                case ForStatement fs:
                    if (fs.Init is VariableDeclaration vdecl)
                    {
                        VisitStatement(vdecl);
                    }
                    else
                    {
                        VisitExpression(fs.Init);
                    }
                    VisitExpression(fs.Test);
                    VisitExpression(fs.Update);
                    VisitStatement(fs.Body);
                    return;

                case ForInStatement fis:
                    if (fis.Left is VariableDeclaration lvd)
                    {
                        VisitStatement(lvd);
                    }
                    else if (fis.Left is Identifier lid)
                    {
                        ErrorIfImportWrite(lid);
                    }
                    else if (fis.Left is ArrayPattern or ObjectPattern or AssignmentPattern or RestElement)
                    {
                        ErrorIfImportWritePattern(fis.Left);
                        VisitPattern(fis.Left);
                    }
                    else
                    {
                        VisitExpression(fis.Left);
                    }
                    VisitExpression(fis.Right);
                    VisitStatement(fis.Body);
                    return;

                case ForOfStatement fos:
                    if (fos.Left is VariableDeclaration lvd2)
                    {
                        VisitStatement(lvd2);
                    }
                    else if (fos.Left is Identifier lid2)
                    {
                        ErrorIfImportWrite(lid2);
                    }
                    else if (fos.Left is ArrayPattern or ObjectPattern or AssignmentPattern or RestElement)
                    {
                        ErrorIfImportWritePattern(fos.Left);
                        VisitPattern(fos.Left);
                    }
                    else
                    {
                        VisitExpression(fos.Left);
                    }
                    VisitExpression(fos.Right);
                    VisitStatement(fos.Body);
                    return;

                case SwitchStatement ss:
                    VisitExpression(ss.Discriminant);
                    scopes.PushBlockScope();
                    foreach (var c in ss.Cases)
                    {
                        VisitExpression(c.Test);
                        foreach (var cs in c.Consequent)
                        {
                            VisitStatement(cs);
                        }
                    }
                    scopes.Pop();
                    return;

                case TryStatement ts:
                    VisitStatement(ts.Block);
                    if (ts.Handler != null)
                    {
                        scopes.PushBlockScope();
                        if (ts.Handler.Param != null)
                        {
                            DeclarePattern(ts.Handler.Param, isVarKind: false);
                            VisitPattern(ts.Handler.Param);
                        }
                        VisitStatement(ts.Handler.Body);
                        scopes.Pop();
                    }
                    VisitStatement(ts.Finalizer);
                    return;

                case ThrowStatement th:
                    VisitExpression(th.Argument);
                    return;

                case LabeledStatement ls:
                    VisitStatement(ls.Body);
                    return;

                case BreakStatement:
                case ContinueStatement:
                case EmptyStatement:
                case DebuggerStatement:
                    return;

                case ClassDeclaration cd:
                    if (cd.Id != null)
                    {
                        scopes.DeclareLexical([cd.Id.Name]);
                    }
                    VisitExpression(cd.SuperClass);
                    VisitClassBody(cd.Body);
                    return;

                default:
                    // Best-effort: still walk any expressions we know about.
                    return;
            }
        }

        void VisitClassBody(ClassBody body)
        {
            foreach (var element in body.Body)
            {
                switch (element)
                {
                    case MethodDefinition md:
                        if (md.Computed)
                        {
                            VisitExpression(md.Key);
                        }
                        VisitExpression(md.Value);
                        break;

                    case PropertyDefinition pd:
                        if (pd.Computed)
                        {
                            VisitExpression(pd.Key);
                        }
                        VisitExpression(pd.Value);
                        break;
                }
            }
        }

        void VisitExpression(Node? expression)
        {
            switch (expression)
            {
                case null:
                    return;

                case Identifier id:
                    RewriteIdentifier(id);
                    return;

                case Literal:
                case ThisExpression:
                case Super:
                    return;

                case MemberExpression me:
                    VisitExpression(me.Object);
                    if (me.Computed)
                    {
                        VisitExpression(me.Property);
                    }
                    return;

                case CallExpression ce:
                    VisitExpression(ce.Callee);
                    foreach (var arg in ce.Arguments)
                    {
                        VisitExpression(arg);
                    }
                    return;

                case NewExpression ne:
                    VisitExpression(ne.Callee);
                    foreach (var arg in ne.Arguments)
                    {
                        VisitExpression(arg);
                    }
                    return;

                case UpdateExpression up:
                    if (up.Argument is Identifier uid)
                    {
                        ErrorIfImportWrite(uid);
                    }
                    VisitExpression(up.Argument);
                    return;

                case UnaryExpression ue:
                    if (string.Equals(ue.Operator.ToString(), "delete", StringComparison.OrdinalIgnoreCase)
                        && ue.Argument is Identifier did
                        && ShouldRewrite(did))
                    {
                        throw new NotSupportedException($"Cannot delete import binding '{did.Name}'");
                    }
                    VisitExpression(ue.Argument);
                    return;

                case BinaryExpression be:
                    VisitExpression(be.Left);
                    VisitExpression(be.Right);
                    return;

                case AssignmentExpression ae:
                    if (ae.Left is Identifier aid)
                    {
                        ErrorIfImportWrite(aid);
                        VisitExpression(ae.Left);
                    }
                    else if (ae.Left is ArrayPattern or ObjectPattern or AssignmentPattern or RestElement)
                    {
                        ErrorIfImportWritePattern(ae.Left);
                        VisitPattern(ae.Left);
                    }
                    else
                    {
                        VisitExpression(ae.Left);
                    }
                    VisitExpression(ae.Right);
                    return;

                case ConditionalExpression ce2:
                    VisitExpression(ce2.Test);
                    VisitExpression(ce2.Consequent);
                    VisitExpression(ce2.Alternate);
                    return;

                case SequenceExpression se:
                    foreach (var ex in se.Expressions)
                    {
                        VisitExpression(ex);
                    }
                    return;

                case ArrayExpression arr:
                    foreach (var el in arr.Elements)
                    {
                        VisitExpression(el);
                    }
                    return;

                case ObjectExpression obj:
                    foreach (var prop in obj.Properties)
                    {
                        switch (prop)
                        {
                            case Property p when p.Shorthand && p.Key is Identifier sid && p.Value is Identifier vid && sid.Name == vid.Name:
                                if (ShouldRewrite(vid))
                                {
                                    edits.Add(new TextEdit(sid.Start, sid.End, $"{sid.Name}: {importedBindings[sid.Name]}"));
                                }
                                break;

                            case Property p:
                                if (p.Computed)
                                {
                                    VisitExpression(p.Key);
                                }
                                VisitExpression(p.Value);
                                break;

                            default:
                                VisitExpression(prop);
                                break;
                        }
                    }
                    return;

                case FunctionExpression fe:
                    scopes.PushFunctionScope();
                    if (fe.Id != null)
                    {
                        scopes.DeclareLexical([fe.Id.Name]);
                    }
                    foreach (var p in fe.Params)
                    {
                        DeclarePattern(p, isVarKind: false);
                    }
                    foreach (var p in fe.Params)
                    {
                        VisitPattern(p);
                    }
                    VisitStatement(fe.Body);
                    scopes.Pop();
                    return;

                case ArrowFunctionExpression afe:
                    scopes.PushFunctionScope();
                    foreach (var p in afe.Params)
                    {
                        DeclarePattern(p, isVarKind: false);
                    }
                    foreach (var p in afe.Params)
                    {
                        VisitPattern(p);
                    }
                    if (afe.Body is BlockStatement bs)
                    {
                        VisitStatement(bs);
                    }
                    else
                    {
                        VisitExpression(afe.Body);
                    }
                    scopes.Pop();
                    return;

                case ClassExpression ce3:
                    scopes.PushBlockScope();
                    if (ce3.Id != null)
                    {
                        scopes.DeclareLexical([ce3.Id.Name]);
                    }
                    VisitExpression(ce3.SuperClass);
                    VisitClassBody(ce3.Body);
                    scopes.Pop();
                    return;
            }
        }

        try
        {
            foreach (var statement in ast.Body)
            {
                VisitStatement(statement);
            }
        }
        catch (NotSupportedException ex)
        {
            error = $"{ex.Message} (line {ast.Location.Start.Line}).";
            return false;
        }

        rewritten = ApplyTextEdits(source, edits);
        return true;
    }

    private sealed class ScopeStack
    {
        private sealed class ScopeFrame
        {
            public bool IsFunctionBoundary { get; }
            public HashSet<string> Names { get; } = new(StringComparer.Ordinal);

            public ScopeFrame(bool isFunctionBoundary)
            {
                IsFunctionBoundary = isFunctionBoundary;
            }
        }

        private readonly Stack<ScopeFrame> _stack = new();

        public void PushFunctionScope() => _stack.Push(new ScopeFrame(isFunctionBoundary: true));
        public void PushBlockScope() => _stack.Push(new ScopeFrame(isFunctionBoundary: false));
        public void Pop() => _stack.Pop();

        public void DeclareLexical(IEnumerable<string> names)
        {
            var frame = _stack.Peek();
            foreach (var name in names)
            {
                frame.Names.Add(name);
            }
        }

        public void DeclareVar(IEnumerable<string> names)
        {
            var functionFrame = _stack.FirstOrDefault(f => f.IsFunctionBoundary) ?? _stack.Peek();
            foreach (var name in names)
            {
                functionFrame.Names.Add(name);
            }
        }

        public bool IsDeclared(string name)
        {
            foreach (var frame in _stack)
            {
                if (frame.Names.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private bool TryRewriteStaticModuleSyntax(
        string source,
        Acornima.Ast.Program ast,
        string debugDocumentId,
        out string rewrittenSource,
        out List<TopLevelDebugSpanOverride> topLevelDebugSpanOverrides,
        out string? error)
    {
        rewrittenSource = source;
        topLevelDebugSpanOverrides = new List<TopLevelDebugSpanOverride>();
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

        var insertPos = 0;
        foreach (var statement in ast.Body)
        {
            if (IsDirectivePrologueStatement(statement))
            {
                insertPos = statement.End;
                continue;
            }
            break;
        }

        var exportPreludeDeclarations = new List<string>();
        var exportPrelude = new List<(string Text, SourceSpan Span)>();
        var importPrelude = new List<(string Text, SourceSpan Span)>();
        var importedBindings = new Dictionary<string, string>(StringComparer.Ordinal);
        var edits = new List<TextEdit>();
        var tempCounter = 0;
        var rewrittenAny = false;

        foreach (var statement in ast.Body)
        {
            try
            {
                switch (statement)
                {
                    case ImportDeclaration importDeclaration:
                        rewrittenAny = true;
                        AppendImportPrelude(
                            importDeclaration,
                            source,
                            ref tempCounter,
                            SourceSpan.FromNode(statement, debugDocumentId),
                            importPrelude,
                            importedBindings);
                        edits.Add(new TextEdit(statement.Start, statement.End, string.Empty));
                        break;

                    case ExportNamedDeclaration exportNamedDeclaration:
                        rewrittenAny = true;
                        var exportSpan = SourceSpan.FromNode(statement, debugDocumentId);
                        if (exportNamedDeclaration.Attributes.Count > 0)
                        {
                            throw new NotSupportedException("Export attributes are not yet supported");
                        }

                        if (exportNamedDeclaration.Declaration != null)
                        {
                            if (exportNamedDeclaration.Declaration is not Node declarationNode)
                            {
                                throw new NotSupportedException("Unsupported export declaration node");
                            }

                            switch (exportNamedDeclaration.Declaration)
                            {
                                case VariableDeclaration variableDeclaration:
                                    edits.Add(new TextEdit(statement.Start, statement.End, GetNodeSource(source, declarationNode)));

                                    var bindingNames = new List<string>();
                                    foreach (var variableDeclarator in variableDeclaration.Declarations)
                                    {
                                        CollectBindingNames(variableDeclarator.Id, bindingNames);
                                    }

                                    foreach (var name in bindingNames.Distinct(StringComparer.Ordinal))
                                    {
                                        exportPrelude.Add((CreateExportGetterLine(name, name), exportSpan));
                                    }
                                    break;

                                case FunctionDeclaration functionDeclaration when functionDeclaration.Id != null:
                                    edits.Add(new TextEdit(statement.Start, statement.End, GetNodeSource(source, declarationNode)));
                                    exportPrelude.Add((CreateExportGetterLine(functionDeclaration.Id.Name, functionDeclaration.Id.Name), exportSpan));
                                    break;

                                case ClassDeclaration:
                                    // Class declarations are lexical and can trip validation if referenced before declaration; keep inline.
                                    edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter)));
                                    break;

                                default:
                                    throw new NotSupportedException("Unsupported export declaration form");
                            }
                        }
                        else if (exportNamedDeclaration.Source == null)
                        {
                            // export { local as exported } (keep inline to preserve ordering vs declarations)
                            edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter)));
                        }
                        else
                        {
                            // Re-export from another module. Keep as a statement rewrite for now.
                            edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter)));
                        }
                        break;

                    case ExportDefaultDeclaration exportDefaultDeclaration:
                        rewrittenAny = true;
                        var exportDefaultSpan = SourceSpan.FromNode(statement, debugDocumentId);
                        switch (exportDefaultDeclaration.Declaration)
                        {
                            case FunctionDeclaration functionDeclaration when functionDeclaration.Id != null:
                                edits.Add(new TextEdit(statement.Start, statement.End, GetNodeSource(source, functionDeclaration)));
                                exportPrelude.Add((CreateExportGetterLine("default", functionDeclaration.Id.Name), exportDefaultSpan));
                                break;

                            case ClassDeclaration:
                                // Keep inline to avoid referencing lexical class bindings before declaration.
                                edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportDefaultDeclaration(exportDefaultDeclaration, source, ref tempCounter)));
                                break;

                            default:
                                edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportDefaultDeclaration(exportDefaultDeclaration, source, ref tempCounter)));
                                break;
                        }
                        break;

                    case ExportAllDeclaration exportAllDeclaration:
                        rewrittenAny = true;
                        edits.Add(new TextEdit(statement.Start, statement.End, RewriteExportAllDeclaration(exportAllDeclaration, source, ref tempCounter)));
                        break;
                }
            }
            catch (NotSupportedException ex)
            {
                error = $"{ex.Message} (line {statement.Location.Start.Line}).";
                return false;
            }
        }

        if (!rewrittenAny)
        {
            return true;
        }

        foreach (var statement in ast.Body)
        {
            if (!IsDirectivePrologueStatement(statement))
            {
                break;
            }

            topLevelDebugSpanOverrides.Add(new TopLevelDebugSpanOverride(SourceSpan.FromNode(statement, debugDocumentId), false));
        }

        foreach (var exportPreludeDeclaration in exportPreludeDeclarations)
        {
            if (!AppendHiddenSnippetDebugSpans(topLevelDebugSpanOverrides, exportPreludeDeclaration, debugDocumentId, true, out error))
            {
                return false;
            }
        }

        if (!AppendHiddenSnippetDebugSpans(topLevelDebugSpanOverrides, "__js2il_esm_mark();", debugDocumentId, false, out error))
        {
            return false;
        }

        foreach (var (line, span) in exportPrelude)
        {
            if (!AppendRepeatedSnippetDebugSpans(topLevelDebugSpanOverrides, line, _ => new TopLevelDebugSpanOverride(span, true), out error))
            {
                return false;
            }
        }

        foreach (var (line, span) in importPrelude)
        {
            if (!AppendRepeatedSnippetDebugSpans(topLevelDebugSpanOverrides, line, _ => new TopLevelDebugSpanOverride(span, false), out error))
            {
                return false;
            }
        }

        foreach (var statement in ast.Body)
        {
            if (IsDirectivePrologueStatement(statement))
            {
                continue;
            }

            var originalSpan = SourceSpan.FromNode(statement, debugDocumentId);
            switch (statement)
            {
                case ImportDeclaration:
                    break;

                case ExportNamedDeclaration exportNamedDeclaration when exportNamedDeclaration.Declaration is VariableDeclaration:
                    topLevelDebugSpanOverrides.Add(new TopLevelDebugSpanOverride(originalSpan, false));
                    break;

                case ExportNamedDeclaration exportNamedDeclaration when exportNamedDeclaration.Declaration is FunctionDeclaration { Id: not null }:
                    topLevelDebugSpanOverrides.Add(new TopLevelDebugSpanOverride(SourceSpan.Hidden(debugDocumentId), false));
                    break;

                case ExportNamedDeclaration exportNamedDeclaration when exportNamedDeclaration.Declaration is ClassDeclaration:
                    if (!AppendRepeatedSnippetDebugSpans(
                        topLevelDebugSpanOverrides,
                        RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter),
                        index => new TopLevelDebugSpanOverride(
                            index == 0 ? SourceSpan.Hidden(debugDocumentId) : originalSpan,
                            index != 0),
                        out error))
                    {
                        return false;
                    }
                    break;

                case ExportNamedDeclaration exportNamedDeclaration:
                    if (!AppendRepeatedSnippetDebugSpans(
                        topLevelDebugSpanOverrides,
                        RewriteExportNamedDeclaration(exportNamedDeclaration, source, ref tempCounter),
                        _ => new TopLevelDebugSpanOverride(originalSpan, true),
                        out error))
                    {
                        return false;
                    }
                    break;

                case ExportDefaultDeclaration exportDefaultDeclaration when exportDefaultDeclaration.Declaration is FunctionDeclaration { Id: not null }:
                    topLevelDebugSpanOverrides.Add(new TopLevelDebugSpanOverride(SourceSpan.Hidden(debugDocumentId), false));
                    break;

                case ExportDefaultDeclaration exportDefaultDeclaration when exportDefaultDeclaration.Declaration is ClassDeclaration:
                    if (!AppendRepeatedSnippetDebugSpans(
                        topLevelDebugSpanOverrides,
                        RewriteExportDefaultDeclaration(exportDefaultDeclaration, source, ref tempCounter),
                        index => new TopLevelDebugSpanOverride(
                            index == 0 ? SourceSpan.Hidden(debugDocumentId) : originalSpan,
                            index != 0),
                        out error))
                    {
                        return false;
                    }
                    break;

                case ExportDefaultDeclaration exportDefaultDeclaration:
                    if (!AppendRepeatedSnippetDebugSpans(
                        topLevelDebugSpanOverrides,
                        RewriteExportDefaultDeclaration(exportDefaultDeclaration, source, ref tempCounter),
                        index => new TopLevelDebugSpanOverride(originalSpan, index != 0),
                        out error))
                    {
                        return false;
                    }
                    break;

                case ExportAllDeclaration exportAllDeclaration:
                    if (!AppendRepeatedSnippetDebugSpans(
                        topLevelDebugSpanOverrides,
                        RewriteExportAllDeclaration(exportAllDeclaration, source, ref tempCounter),
                        _ => new TopLevelDebugSpanOverride(originalSpan, true),
                        out error))
                    {
                        return false;
                    }
                    break;

                default:
                    topLevelDebugSpanOverrides.Add(new TopLevelDebugSpanOverride(originalSpan, false));
                    break;
            }
        }

        var preludeBuilder = new StringBuilder();
        preludeBuilder.AppendLine();
        foreach (var line in exportPreludeDeclarations)
        {
            preludeBuilder.AppendLine(line);
        }
        preludeBuilder.AppendLine("__js2il_esm_mark();");
        foreach (var (line, _) in exportPrelude)
        {
            preludeBuilder.AppendLine(line);
        }
        foreach (var (line, _) in importPrelude)
        {
            preludeBuilder.AppendLine(line);
        }
        preludeBuilder.AppendLine();
        edits.Add(new TextEdit(insertPos, insertPos, preludeBuilder.ToString()));

        var rewrittenPhase1 = ApplyTextEdits(source, edits);

        Acornima.Ast.Program rewrittenAst;
        try
        {
            rewrittenAst = _parser.ParseJavaScript(rewrittenPhase1, "rewritten.js");
        }
        catch (Exception ex)
        {
            error = $"Failed to parse rewritten ES module syntax: {ex.Message}";
            return false;
        }

        if (!TryRewriteImportedBindingReferences(rewrittenPhase1, rewrittenAst, importedBindings, out var rewrittenPhase2, out var bindError))
        {
            error = bindError;
            return false;
        }

        var builder = new StringBuilder(rewrittenPhase2.Length + 512);
        builder.Append(rewrittenPhase2);
        builder.AppendLine();
        builder.AppendLine(EsModuleInteropPrelude);
        rewrittenSource = builder.ToString();

        if (!AppendHiddenSnippetDebugSpans(topLevelDebugSpanOverrides, EsModuleInteropPrelude, debugDocumentId, true, out error))
        {
            return false;
        }

        return true;
    }

    private bool TryCreateTopLevelDebugSequencePointOverrides(
        Acornima.Ast.Program ast,
        IReadOnlyList<TopLevelDebugSpanOverride> topLevelDebugSpanOverrides,
        out Dictionary<Node, SourceSpan>? overrides,
        out string? error)
    {
        overrides = null;
        error = null;

        if (topLevelDebugSpanOverrides.Count == 0)
        {
            overrides = new Dictionary<Node, SourceSpan>(ReferenceEqualityComparer.Instance);
            return true;
        }

        if (ast.Body.Count != topLevelDebugSpanOverrides.Count)
        {
            error = $"Internal debug sequence point mapping mismatch: expected {ast.Body.Count} top-level statements after rewrite, but built {topLevelDebugSpanOverrides.Count} debug spans.";
            return false;
        }

        overrides = new Dictionary<Node, SourceSpan>(ReferenceEqualityComparer.Instance);
        var overrideMap = overrides;
        for (var i = 0; i < ast.Body.Count; i++)
        {
            var statement = ast.Body[i];
            var topLevelOverride = topLevelDebugSpanOverrides[i];
            overrideMap[statement] = topLevelOverride.Span;

            if (!topLevelOverride.HideGeneratedCallables)
            {
                continue;
            }

            var documentId = topLevelOverride.Span.Document;
            var hiddenSpan = SourceSpan.Hidden(documentId);
            var hideEntireStatementSubtree = topLevelOverride.Span.IsHidden;
            var walker = new AstWalker();
            walker.Visit(statement, node =>
            {
                if (ReferenceEquals(node, statement))
                {
                    if (hideEntireStatementSubtree)
                    {
                        AddHiddenStatementOverrides(node, hiddenSpan, overrideMap);
                    }

                    return;
                }

                if (hideEntireStatementSubtree)
                {
                    return;
                }

                if (IsGeneratedCallableNode(node))
                {
                    AddHiddenStatementOverrides(node, hiddenSpan, overrideMap);
                }
            });
        }

        return true;
    }

    private bool AppendRepeatedSnippetDebugSpans(
        List<TopLevelDebugSpanOverride> spans,
        string snippet,
        Func<int, TopLevelDebugSpanOverride> spanFactory,
        out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(snippet))
        {
            return true;
        }

        Acornima.Ast.Program snippetAst;
        try
        {
            snippetAst = _parser.ParseJavaScript(snippet, "rewritten.js");
        }
        catch (Exception ex)
        {
            error = $"Failed to parse rewritten module debug mapping snippet: {ex.Message}";
            return false;
        }

        for (var i = 0; i < snippetAst.Body.Count; i++)
        {
            spans.Add(spanFactory(i));
        }

        return true;
    }

    private bool AppendHiddenSnippetDebugSpans(
        List<TopLevelDebugSpanOverride> spans,
        string snippet,
        string debugDocumentId,
        bool hideGeneratedCallables,
        out string? error)
    {
        return AppendRepeatedSnippetDebugSpans(
            spans,
            snippet,
            _ => new TopLevelDebugSpanOverride(SourceSpan.Hidden(debugDocumentId), hideGeneratedCallables),
            out error);
    }

    private static bool IsGeneratedCallableNode(Node node)
    {
        return node is FunctionDeclaration
            or FunctionExpression
            or ArrowFunctionExpression;
    }

    private static void AddHiddenStatementOverrides(Node node, SourceSpan hiddenSpan, Dictionary<Node, SourceSpan> overrides)
    {
        var walker = new AstWalker();
        walker.Visit(node, child =>
        {
            if (child is Statement statement)
            {
                overrides[statement] = hiddenSpan;
            }
        });
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

    private static string CreateJsStringLiteral(string value)
    {
        return $"\"{EscapeJsString(value)}\"";
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

    private static bool IsBuiltInModuleSpecifier(string specifier)
    {
        if (string.IsNullOrWhiteSpace(specifier))
        {
            return false;
        }

        return JavaScriptRuntime.Node.NodeModuleRegistry.TryGetModuleType(specifier, out _);
    }

    private static bool IsBarePackageSpecifier(string specifier)
    {
        if (string.IsNullOrWhiteSpace(specifier))
        {
            return false;
        }

        var normalized = specifier.Trim().Replace('\\', '/');
        if (normalized.StartsWith("#", StringComparison.Ordinal))
        {
            return false;
        }

        if (normalized.StartsWith("./", StringComparison.Ordinal)
            || normalized.StartsWith("../", StringComparison.Ordinal)
            || normalized.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        if (Path.IsPathRooted(normalized))
        {
            return false;
        }

        return !normalized.StartsWith("node:", StringComparison.OrdinalIgnoreCase);
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

    private static ModuleRecord BuildModuleRecord(Acornima.Ast.Program ast, IReadOnlyList<ModuleDependency> moduleDependencies)
    {
        var record = new ModuleRecord();

        void AddRequestedModule(string moduleRequest)
        {
            var dependency = moduleDependencies.FirstOrDefault(existing =>
                string.Equals(existing.Request, moduleRequest, StringComparison.Ordinal));
            if (dependency == null)
            {
                return;
            }

            if (record.RequestedModules.Any(existing =>
                    string.Equals(existing.Specifier, dependency.Request, StringComparison.Ordinal)
                    && string.Equals(existing.ResolvedPath, dependency.ResolvedPath, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            record.RequestedModules.Add(new ModuleRequestRecord
            {
                Specifier = dependency.Request,
                ResolvedPath = dependency.ResolvedPath
            });
        }

        foreach (var statement in ast.Body)
        {
            switch (statement)
            {
                case ImportDeclaration { Source: StringLiteral sourceLiteral } importDeclaration:
                    record.HasStaticModuleSyntax = true;
                    AddRequestedModule(sourceLiteral.Value);
                    if (importDeclaration.Specifiers.Count == 0)
                    {
                        record.ImportEntries.Add(new ModuleImportEntry
                        {
                            Kind = ModuleImportKind.SideEffect,
                            ModuleRequest = sourceLiteral.Value
                        });
                        break;
                    }

                    foreach (var specifier in importDeclaration.Specifiers)
                    {
                        switch (specifier)
                        {
                            case ImportDefaultSpecifier defaultSpecifier when defaultSpecifier.Local is Identifier defaultLocal:
                                record.ImportEntries.Add(new ModuleImportEntry
                                {
                                    Kind = ModuleImportKind.Default,
                                    ModuleRequest = sourceLiteral.Value,
                                    LocalName = defaultLocal.Name,
                                    ImportName = "default"
                                });
                                break;

                            case ImportNamespaceSpecifier namespaceSpecifier when namespaceSpecifier.Local is Identifier namespaceLocal:
                                record.ImportEntries.Add(new ModuleImportEntry
                                {
                                    Kind = ModuleImportKind.Namespace,
                                    ModuleRequest = sourceLiteral.Value,
                                    LocalName = namespaceLocal.Name
                                });
                                break;

                            case ImportSpecifier importSpecifier when importSpecifier.Local is Identifier importLocal:
                                record.ImportEntries.Add(new ModuleImportEntry
                                {
                                    Kind = ModuleImportKind.Named,
                                    ModuleRequest = sourceLiteral.Value,
                                    LocalName = importLocal.Name,
                                    ImportName = GetExpressionName(importSpecifier.Imported)
                                });
                                break;
                        }
                    }

                    break;

                case ExportNamedDeclaration { Declaration: not null } exportNamedDeclaration:
                    record.HasStaticModuleSyntax = true;
                    AddLocalDeclarationExports(record, exportNamedDeclaration.Declaration);
                    break;

                case ExportNamedDeclaration { Source: StringLiteral sourceLiteral } exportNamedFrom:
                    record.HasStaticModuleSyntax = true;
                    AddRequestedModule(sourceLiteral.Value);
                    foreach (var specifier in exportNamedFrom.Specifiers)
                    {
                        record.IndirectExportEntries.Add(new ModuleExportEntry
                        {
                            Kind = ModuleExportKind.Indirect,
                            ExportName = GetExpressionName(specifier.Exported),
                            LocalName = GetExpressionName(specifier.Local),
                            ModuleRequest = sourceLiteral.Value
                        });
                    }

                    break;

                case ExportNamedDeclaration exportNamedDeclaration:
                    record.HasStaticModuleSyntax = true;
                    foreach (var specifier in exportNamedDeclaration.Specifiers)
                    {
                        record.LocalExportEntries.Add(new ModuleExportEntry
                        {
                            Kind = ModuleExportKind.Local,
                            ExportName = GetExpressionName(specifier.Exported),
                            LocalName = GetExpressionName(specifier.Local)
                        });
                    }

                    break;

                case ExportDefaultDeclaration exportDefaultDeclaration:
                    record.HasStaticModuleSyntax = true;
                    var defaultBindingName = exportDefaultDeclaration.Declaration switch
                    {
                        FunctionDeclaration { Id: not null } functionDeclaration => functionDeclaration.Id!.Name,
                        ClassDeclaration { Id: not null } classDeclaration => classDeclaration.Id!.Name,
                        _ => "default"
                    };

                    record.LocalExportEntries.Add(new ModuleExportEntry
                    {
                        Kind = ModuleExportKind.Default,
                        ExportName = "default",
                        LocalName = defaultBindingName
                    });
                    break;

                case ExportAllDeclaration { Source: StringLiteral sourceLiteral } exportAllDeclaration when exportAllDeclaration.Exported != null:
                    record.HasStaticModuleSyntax = true;
                    AddRequestedModule(sourceLiteral.Value);
                    record.IndirectExportEntries.Add(new ModuleExportEntry
                    {
                        Kind = ModuleExportKind.Namespace,
                        ExportName = GetExpressionName(exportAllDeclaration.Exported),
                        LocalName = "*namespace*",
                        ModuleRequest = sourceLiteral.Value
                    });
                    break;

                case ExportAllDeclaration { Source: StringLiteral sourceLiteral }:
                    record.HasStaticModuleSyntax = true;
                    AddRequestedModule(sourceLiteral.Value);
                    record.StarExportEntries.Add(new ModuleExportEntry
                    {
                        Kind = ModuleExportKind.Star,
                        ExportName = "*",
                        ModuleRequest = sourceLiteral.Value
                    });
                    break;
            }
        }

        return record;
    }

    private static void AddLocalDeclarationExports(ModuleRecord record, INode declaration)
    {
        switch (declaration)
        {
            case VariableDeclaration variableDeclaration:
                var bindingNames = new List<string>();
                foreach (var declarator in variableDeclaration.Declarations)
                {
                    CollectBindingNames(declarator.Id, bindingNames);
                }

                foreach (var bindingName in bindingNames.Distinct(StringComparer.Ordinal))
                {
                    record.LocalExportEntries.Add(new ModuleExportEntry
                    {
                        Kind = ModuleExportKind.Local,
                        ExportName = bindingName,
                        LocalName = bindingName
                    });
                }
                break;

            case FunctionDeclaration functionDeclaration when functionDeclaration.Id != null:
                record.LocalExportEntries.Add(new ModuleExportEntry
                {
                    Kind = ModuleExportKind.Local,
                    ExportName = functionDeclaration.Id.Name,
                    LocalName = functionDeclaration.Id.Name
                });
                break;

            case ClassDeclaration classDeclaration when classDeclaration.Id != null:
                record.LocalExportEntries.Add(new ModuleExportEntry
                {
                    Kind = ModuleExportKind.Local,
                    ExportName = classDeclaration.Id.Name,
                    LocalName = classDeclaration.Id.Name
                });
                break;
        }
    }

    private bool LinkModule(ModuleDefinition module, Modules modules, HashSet<string> linkedModules)
    {
        var record = module.ModuleRecord ??= new ModuleRecord();
        if (record.LinkPhase == ModuleLinkPhase.Linked)
        {
            return true;
        }

        if (record.LinkPhase == ModuleLinkPhase.Linking)
        {
            return true;
        }

        if (record.LinkPhase == ModuleLinkPhase.LinkError)
        {
            return false;
        }

        record.LinkPhase = ModuleLinkPhase.Linking;

        var hadErrors = false;
        foreach (var request in record.RequestedModules)
        {
            if (!modules._modules.TryGetValue(request.ResolvedPath, out var dependencyModule))
            {
                record.LinkErrors.Add($"Module request '{request.Specifier}' resolved to '{request.ResolvedPath}', but that module was not loaded.");
                hadErrors = true;
                continue;
            }

            if (!linkedModules.Contains(dependencyModule.Path) && !LinkModule(dependencyModule, modules, linkedModules))
            {
                hadErrors = true;
            }
        }

        var directExportNames = new HashSet<string>(
            record.LocalExportEntries.Select(entry => entry.ExportName)
                .Concat(record.IndirectExportEntries.Select(entry => entry.ExportName)),
            StringComparer.Ordinal);

        var exportNames = new HashSet<string>(GetExportedNames(module, modules, new HashSet<string>(StringComparer.OrdinalIgnoreCase)), StringComparer.Ordinal);
        foreach (var exportName in exportNames)
        {
            var resolution = ResolveExport(module, exportName, modules, new HashSet<string>(StringComparer.Ordinal));
            switch (resolution.Status)
            {
                case ModuleExportResolutionStatus.Resolved:
                    record.ResolvedExports[exportName] = resolution.Export!;
                    break;
                case ModuleExportResolutionStatus.Ambiguous:
                    record.LinkErrors.Add($"Ambiguous export '{exportName}' in module '{module.ModuleId}'.");
                    hadErrors = true;
                    break;
                case ModuleExportResolutionStatus.NotFound when directExportNames.Contains(exportName):
                    record.LinkErrors.Add($"Export '{exportName}' in module '{module.ModuleId}' could not be resolved during module linking.");
                    hadErrors = true;
                    break;
            }
        }

        foreach (var importEntry in record.ImportEntries)
        {
            if (importEntry.Kind is ModuleImportKind.SideEffect or ModuleImportKind.Namespace)
            {
                continue;
            }

            if (!TryGetDependencyModule(module, importEntry.ModuleRequest, modules, out var dependencyModule))
            {
                record.LinkErrors.Add($"Import request '{importEntry.ModuleRequest}' in module '{module.ModuleId}' could not be resolved during module linking.");
                hadErrors = true;
                continue;
            }

            var importedName = importEntry.ImportName ?? "default";
            var resolution = ResolveExport(dependencyModule, importedName, modules, new HashSet<string>(StringComparer.Ordinal));
            switch (resolution.Status)
            {
                case ModuleExportResolutionStatus.NotFound:
                    record.LinkErrors.Add($"Module '{dependencyModule.ModuleId}' does not export '{importedName}' required by '{module.ModuleId}'.");
                    hadErrors = true;
                    break;
                case ModuleExportResolutionStatus.Ambiguous:
                    record.LinkErrors.Add($"Module '{dependencyModule.ModuleId}' exports '{importedName}' ambiguously, so '{module.ModuleId}' cannot import it.");
                    hadErrors = true;
                    break;
            }
        }

        record.LinkPhase = hadErrors || record.LinkErrors.Count > 0
            ? ModuleLinkPhase.LinkError
            : ModuleLinkPhase.Linked;

        if (record.LinkPhase == ModuleLinkPhase.Linked)
        {
            linkedModules.Add(module.Path);
            return true;
        }

        return false;
    }

    private IEnumerable<string> GetExportedNames(ModuleDefinition module, Modules modules, HashSet<string> exportStarSet)
    {
        if (!exportStarSet.Add(module.Path))
        {
            return Enumerable.Empty<string>();
        }

        var record = module.ModuleRecord;
        if (record == null)
        {
            return Enumerable.Empty<string>();
        }

        var names = new HashSet<string>(
            record.LocalExportEntries.Select(entry => entry.ExportName)
                .Concat(record.IndirectExportEntries.Select(entry => entry.ExportName)),
            StringComparer.Ordinal);

        foreach (var starExport in record.StarExportEntries)
        {
            if (!TryGetDependencyModule(module, starExport.ModuleRequest, modules, out var dependencyModule))
            {
                continue;
            }

            foreach (var exportName in GetExportedNames(dependencyModule, modules, exportStarSet))
            {
                if (!string.Equals(exportName, "default", StringComparison.Ordinal))
                {
                    names.Add(exportName);
                }
            }
        }

        exportStarSet.Remove(module.Path);
        return names;
    }

    private ModuleExportResolution ResolveExport(ModuleDefinition module, string exportName, Modules modules, HashSet<string> resolveSet)
    {
        var record = module.ModuleRecord;
        if (record == null)
        {
            return ModuleExportResolution.NotFound();
        }

        // CommonJS-shaped modules expose properties dynamically through the existing runtime interop,
        // so the linker treats any requested name as potentially available instead of rejecting it
        // with the stricter static ESM ResolveExport rules.
        if (IsDynamicExportSurface(module))
        {
            return ModuleExportResolution.Resolved(new ModuleResolvedExport
            {
                ExportName = exportName,
                TargetModule = module,
                BindingName = exportName,
                Kind = string.Equals(exportName, "default", StringComparison.Ordinal)
                    ? ModuleExportKind.Default
                    : ModuleExportKind.Local
            });
        }

        if (record.ResolvedExports.TryGetValue(exportName, out var cached))
        {
            return ModuleExportResolution.Resolved(cached);
        }

        var resolveKey = module.Path + "\n" + exportName;
        if (!resolveSet.Add(resolveKey))
        {
            return ModuleExportResolution.NotFound();
        }

        ModuleResolvedExport? resolved = null;

        foreach (var localExport in record.LocalExportEntries.Where(entry => string.Equals(entry.ExportName, exportName, StringComparison.Ordinal)))
        {
            var candidate = new ModuleResolvedExport
            {
                ExportName = exportName,
                TargetModule = module,
                BindingName = localExport.LocalName ?? exportName,
                Kind = localExport.Kind
            };

            if (!TryMergeResolvedExport(ref resolved, candidate))
            {
                return ModuleExportResolution.Ambiguous();
            }
        }

        foreach (var indirectExport in record.IndirectExportEntries.Where(entry => string.Equals(entry.ExportName, exportName, StringComparison.Ordinal)))
        {
            if (!TryGetDependencyModule(module, indirectExport.ModuleRequest, modules, out var dependencyModule))
            {
                continue;
            }

            ModuleExportResolution candidateResolution;
            if (indirectExport.Kind == ModuleExportKind.Namespace)
            {
                candidateResolution = ModuleExportResolution.Resolved(new ModuleResolvedExport
                {
                    ExportName = exportName,
                    TargetModule = dependencyModule,
                    BindingName = "*namespace*",
                    Kind = ModuleExportKind.Namespace
                });
            }
            else
            {
                candidateResolution = ResolveExport(dependencyModule, indirectExport.LocalName ?? exportName, modules, resolveSet);
            }

            if (candidateResolution.Status == ModuleExportResolutionStatus.Ambiguous)
            {
                return candidateResolution;
            }

            if (candidateResolution.Status == ModuleExportResolutionStatus.Resolved
                && !TryMergeResolvedExport(ref resolved, candidateResolution.Export!))
            {
                return ModuleExportResolution.Ambiguous();
            }
        }

        if (resolved != null)
        {
            return ModuleExportResolution.Resolved(resolved);
        }

        if (string.Equals(exportName, "default", StringComparison.Ordinal))
        {
            return ModuleExportResolution.NotFound();
        }

        foreach (var starExport in record.StarExportEntries)
        {
            if (!TryGetDependencyModule(module, starExport.ModuleRequest, modules, out var dependencyModule))
            {
                continue;
            }

            var candidateResolution = ResolveExport(dependencyModule, exportName, modules, resolveSet);
            if (candidateResolution.Status == ModuleExportResolutionStatus.NotFound)
            {
                continue;
            }

            if (candidateResolution.Status == ModuleExportResolutionStatus.Ambiguous)
            {
                return candidateResolution;
            }

            if (!TryMergeResolvedExport(ref resolved, candidateResolution.Export!))
            {
                return ModuleExportResolution.Ambiguous();
            }
        }

        return resolved != null
            ? ModuleExportResolution.Resolved(resolved)
            : ModuleExportResolution.NotFound();
    }

    private static bool TryMergeResolvedExport(ref ModuleResolvedExport? current, ModuleResolvedExport candidate)
    {
        if (current == null)
        {
            current = candidate;
            return true;
        }

        return ReferenceEquals(current.TargetModule, candidate.TargetModule)
            && string.Equals(current.BindingName, candidate.BindingName, StringComparison.Ordinal)
            && current.Kind == candidate.Kind;
    }

    private static bool TryGetDependencyModule(ModuleDefinition module, string? moduleRequest, Modules modules, out ModuleDefinition dependencyModule)
    {
        dependencyModule = null!;
        if (string.IsNullOrWhiteSpace(moduleRequest))
        {
            return false;
        }

        var resolvedPath = module.ModuleRecord?.RequestedModules
            .FirstOrDefault(request => string.Equals(request.Specifier, moduleRequest, StringComparison.Ordinal))
            ?.ResolvedPath
            ?? module.Dependencies
                .FirstOrDefault(dependency => string.Equals(dependency.Request, moduleRequest, StringComparison.Ordinal))
                ?.ResolvedPath;

        if (resolvedPath != null && modules._modules.TryGetValue(resolvedPath, out var resolvedDependencyModule))
        {
            dependencyModule = resolvedDependencyModule;
            return true;
        }

        return false;
    }

    private static bool IsDynamicExportSurface(ModuleDefinition module)
    {
        if (string.Equals(Path.GetExtension(module.Path), ".cjs", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var record = module.ModuleRecord;
        if (record == null || string.Equals(Path.GetExtension(module.Path), ".mjs", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (record.HasStaticModuleSyntax)
        {
            return false;
        }

        return record.LocalExportEntries.Count == 0
            && record.IndirectExportEntries.Count == 0
            && record.StarExportEntries.Count == 0;
    }

    private static Dictionary<string, int> ComputeModuleEvaluationComponents(Modules modules)
    {
        var index = 0;
        var componentIndex = 0;
        var stack = new Stack<ModuleDefinition>();
        var onStack = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var indices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var lowLinks = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var components = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        void StrongConnect(ModuleDefinition module)
        {
            indices[module.Path] = index;
            lowLinks[module.Path] = index;
            index++;
            stack.Push(module);
            onStack.Add(module.Path);

            foreach (var dependency in GetStaticModuleGraphDependencies(module, modules))
            {
                if (!indices.ContainsKey(dependency.Path))
                {
                    StrongConnect(dependency);
                    lowLinks[module.Path] = Math.Min(lowLinks[module.Path], lowLinks[dependency.Path]);
                }
                else if (onStack.Contains(dependency.Path))
                {
                    lowLinks[module.Path] = Math.Min(lowLinks[module.Path], indices[dependency.Path]);
                }
            }

            // If low-link does not point back to this node's DFS index, this module is not the
            // root of a strongly connected component yet, so leave it on the stack for its root.
            if (lowLinks[module.Path] != indices[module.Path])
            {
                return;
            }

            while (stack.Count > 0)
            {
                var connected = stack.Pop();
                onStack.Remove(connected.Path);
                components[connected.Path] = componentIndex;
                if (string.Equals(connected.Path, module.Path, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            componentIndex++;
        }

        foreach (var module in modules._modules.Values)
        {
            if (!indices.ContainsKey(module.Path))
            {
                StrongConnect(module);
            }
        }

        return components;
    }

    private static List<List<ModuleDefinition>> TopologicallyOrderComponents(Dictionary<string, int> components, Modules modules)
    {
        var grouped = modules._modules.Values
            .GroupBy(module => components[module.Path])
            .ToDictionary(group => group.Key, group => group.ToList());
        var ordered = new List<List<ModuleDefinition>>();
        var visited = new HashSet<int>();

        void Visit(int componentId)
        {
            if (!visited.Add(componentId))
            {
                return;
            }

            foreach (var module in grouped[componentId])
            {
                foreach (var dependency in GetStaticModuleGraphDependencies(module, modules))
                {
                    if (!components.TryGetValue(dependency.Path, out var dependencyComponent)
                        || dependencyComponent == componentId)
                    {
                        continue;
                    }

                    Visit(dependencyComponent);
                }
            }

            ordered.Add(grouped[componentId]);
        }

        foreach (var componentId in grouped.Keys.OrderBy(id => id))
        {
            Visit(componentId);
        }

        return ordered;
    }

    private static IEnumerable<ModuleDefinition> GetStaticModuleGraphDependencies(ModuleDefinition module, Modules modules)
    {
        var record = module.ModuleRecord;
        if (record == null)
        {
            yield break;
        }

        var yielded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var request in record.RequestedModules)
        {
            if (string.IsNullOrWhiteSpace(request.ResolvedPath)
                || !yielded.Add(request.ResolvedPath)
                || !modules._modules.TryGetValue(request.ResolvedPath, out var dependencyModule))
            {
                continue;
            }

            yield return dependencyModule;
        }
    }

    private static void FlushModuleLinkErrors(Modules modules, ICompilerOutput logger)
    {
        var errors = modules._modules.Values
            .Select(module => (Module: module, Record: module.ModuleRecord))
            .Where(item => item.Record != null && item.Record.LinkErrors.Count > 0)
            .ToList();
        if (errors.Count == 0)
        {
            return;
        }

        logger.WriteLineError("\nModule Link Errors:");
        foreach (var (module, record) in errors)
        {
            foreach (var error in record!.LinkErrors)
            {
                logger.WriteLineError($"Error: [{module.ModuleId}] {module.Path}: {error}");
            }
        }
    }

    private enum ModuleExportResolutionStatus
    {
        NotFound,
        Resolved,
        Ambiguous
    }

    private readonly struct ModuleExportResolution
    {
        public ModuleExportResolutionStatus Status { get; }

        public ModuleResolvedExport? Export { get; }

        private ModuleExportResolution(ModuleExportResolutionStatus status, ModuleResolvedExport? export)
        {
            Status = status;
            Export = export;
        }

        public static ModuleExportResolution NotFound() => new(ModuleExportResolutionStatus.NotFound, null);

        public static ModuleExportResolution Resolved(ModuleResolvedExport export) => new(ModuleExportResolutionStatus.Resolved, export);

        public static ModuleExportResolution Ambiguous() => new(ModuleExportResolutionStatus.Ambiguous, null);
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

        public void Flush(ICompilerOutput logger)
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
