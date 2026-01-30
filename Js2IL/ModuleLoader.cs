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
    private readonly JavaScriptAstValidator _validator = new JavaScriptAstValidator();
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    private readonly bool _verbose;

    public ModuleLoader(CompilerOptions options, IFileSystem fileSystem, ILogger logger)
    {
        _verbose = options.Verbose;
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public Modules? LoadModules(string modulePath)
    {
        var rootModulePath = Path.GetFullPath(modulePath);

        var diagnostics = new ModuleLoadDiagnostics(rootModulePath);
        var moduleCache = new Dictionary<string, ModuleDefinition>();
        var hadAnyErrors = false;
        ModuleDefinition? rootModule = null;

        void LoadRecursive(string currentPath)
        {
            if (moduleCache.ContainsKey(currentPath))
            {
                return;
            }

            var loadOk = TryLoadAndParseModule(currentPath, rootModulePath, diagnostics, out var module);
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
                // is local module?
                if (!dep.StartsWith(".") && !dep.StartsWith("/"))
                {
                    // skip loading non-local modules
                    continue;
                }

                var resolvedDepPath = ResolveModulePath(module.Path, dep);
                LoadRecursive(resolvedDepPath);
            }
        }

        LoadRecursive(rootModulePath);

        diagnostics.Flush(_logger);

        if (rootModule is null)
        {
            return null;
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
            ast = _parser.ParseJavaScript(jsSource, modulePath);
        }
        catch (Exception ex)
        {
            module = null;
            diagnostics.AddParseError(modulePath, ex.Message);
            return false;
        }

        var moduleName = JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdFromPath(modulePath, rootModulePath);

        module = new ModuleDefinition
        {
            Path = modulePath,
            Name = moduleName,
            Ast = ast
        };

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
            diagnostics.AddValidationErrors(moduleName, modulePath, validationResult.Errors);
            return false;
        }

        if (validationResult.Warnings.Any())
        {
            diagnostics.AddWarnings(moduleName, modulePath, validationResult.Warnings);
        }

        return true;
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

    private string ResolveModulePath(string basePath, string moduleSpecifier)
    {
        var baseDirectory = Path.GetDirectoryName(basePath) ?? ".";
        var combinedPath = Path.Combine(baseDirectory, moduleSpecifier);

        // Normalize the path
        var fullPath = Path.GetFullPath(combinedPath);

        return fullPath.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            ? fullPath
            : fullPath + ".js";
    }
}