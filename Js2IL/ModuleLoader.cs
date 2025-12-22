using Js2IL.Services;

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

    private readonly bool _verbose;

    public ModuleLoader(CompilerOptions options, IFileSystem fileSystem)
    {
        _verbose = options.Verbose;
        _fileSystem = fileSystem;
    }

    public Modules? LoadModules(string modulePath)
    {
        var rootModulePath = Path.GetFullPath(modulePath);

        if (TryLoadAndParseModule(rootModulePath, rootModulePath, out ModuleDefinition? module))
        {
            var modules = new Modules { rootModule = module! };
            modules._modules[rootModulePath] = module!;

            if (!LoadDepedencies(module!, modules))
            {
                return null;
            }

            return modules;
        }

        return null;
    }

    private bool TryLoadAndParseModule(string modulePath, string rootModulePath, out ModuleDefinition? module)
    {
        var jsSource = _fileSystem.ReadAllText(modulePath);
        var ast = _parser.ParseJavaScript(jsSource, modulePath);

        var moduleName = JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdFromPath(modulePath, rootModulePath);

        module = new ModuleDefinition
        {
            Path = modulePath,
            Name = moduleName,
            Ast = ast
        };

        if (this._verbose)
        {
            Console.WriteLine("AST Structure:");
            _parser.VisitAst(ast, node =>
            {
                Console.Write($"Node Type: {node.Type}");
                if (node is Acornima.Ast.NumericLiteral num)
                    Console.Write($", Value: {num.Value}");
                if (node is Acornima.Ast.UnaryExpression unary)
                    Console.Write($", Operator: {unary.Operator}");
                Console.WriteLine();
            });
        }

        if (this._verbose)
        {
            Console.WriteLine($"\nValidating module: {modulePath}");
        }
        var validationResult = _validator.Validate(ast);
        if (!validationResult.IsValid)
        {
            Logger.WriteLineError("\nValidation Errors:");
            foreach (var error in validationResult.Errors)
            {
                Logger.WriteLineError($"Error: {error}");
            }
            return false;
        }

        if (validationResult.Warnings.Any())
        {
            Console.WriteLine("\nValidation Warnings:");
            foreach (var warning in validationResult.Warnings)
            {
                Logger.WriteLineWarning($"Warning: {warning}");
            }
        }
        

        return true;
    }

    private bool LoadDepedencies(ModuleDefinition module, Modules modules)
    {
        var dependencies = GetModuleDependencies(module);
        foreach (var dep in dependencies)
        {
            // is local module?
            if (!dep.StartsWith(".") && !dep.StartsWith("/"))
            {
                // skip loading non-local modules
                continue;
            }

            var resolvedDepPath = ResolveModulePath(module.Path, dep);
            if (!modules._modules.TryGetValue(resolvedDepPath, out var depModule))
            {
                if (TryLoadAndParseModule(resolvedDepPath, modules.rootModule.Path, out depModule) && depModule != null)
                {
                    modules._modules[resolvedDepPath] = depModule;
                }
                else
                {
                    return false;
                }
            }

            // Recursively load nested dependencies (guarded by the module cache above).
            if (!LoadDepedencies(depModule!, modules))
            {
                return false;
            }
        }

        return true;
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