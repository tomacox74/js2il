using Js2IL.Services;
using Js2IL.SymbolTables;
using Microsoft.Extensions.DependencyInjection;
namespace Js2IL;

public class Compiler
{
    private readonly bool analyzeUnused;
    private readonly string? outputDirectory;
    private readonly bool verbose;

    private readonly SymbolTableBuilder _symbolTableBuilder = new SymbolTableBuilder();

    private readonly ModuleLoader _moduleLoader;

    private readonly IServiceProvider _serviceProvider;

    public Compiler(IServiceProvider serviceProvider, CompilerOptions options, ModuleLoader moduleLoader)
    {
        this.outputDirectory = options.OutputDirectory;
        this.verbose = options.Verbose;
        this.analyzeUnused = options.AnalyzeUnused;
        this._moduleLoader = moduleLoader;
        this._serviceProvider = serviceProvider;
    }   

    public bool Compile(string inputFile)
    {
        var modules = this._moduleLoader.LoadModules(inputFile);
        if (modules == null)
        {
            return false;
        }

        // Analyze unused code (if requested)
        if (this.analyzeUnused)
        {
            AnalyzeUnusedForModules(modules);
        }

        // Build scope trees
        if (this.verbose)
        {
            Console.WriteLine("Build the symbol tables");
        }
        foreach (var mod in modules._modules.Values)
        {
            _symbolTableBuilder.Build(mod);
        }

        if (this.verbose)
        {
            
            foreach (var mod in modules._modules.Values)
            {
                Console.WriteLine();
                Console.WriteLine("Scope Tree Structure:");
                Console.WriteLine($"Module: {mod.Path}");
                var symbolTable = mod.SymbolTable!;
                PrintScopeTree(symbolTable.Root, 0);
            }
        }

        // Generate IL assembly
        if (this.verbose)
        {
            Console.WriteLine("Generating dotnet assembly...");
        }
        var assemblyGenerator = _serviceProvider.GetRequiredService<AssemblyGenerator>();

        // Resolve and validate output directory; create if missing
        if (!EnsureOutputPathExists(inputFile, this.outputDirectory, out var outputPath))
        {
            return false;
        }

        var assemblyName = Path.GetFileNameWithoutExtension(inputFile);

        assemblyGenerator.Generate(modules, assemblyName, outputPath);

        Console.WriteLine($"Compilation succeeded. Output written to {outputPath}");

        return true;
    }

    private bool EnsureOutputPathExists(string inputFile, string? outputDirectory, out string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            outputPath = Path.GetDirectoryName(Path.GetFullPath(inputFile))!;
        }
        else
        {
            outputPath = Path.GetFullPath(outputDirectory);
        }

        // If a file exists at the output path, fail with a clear message
        if (File.Exists(outputPath))
        {
            Logger.WriteLineError($"Error: Output path '{outputPath}' is a file. Provide a directory path.");
            return false;
        }

        // Ensure the directory exists (treat common IO issues as known failures)
        try
        {
            Directory.CreateDirectory(outputPath);
            if (this.verbose)
            {
                Console.WriteLine($"Ensured output directory exists: {outputPath}");
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException
                                    || ex is ArgumentException
                                    || ex is PathTooLongException
                                    || ex is NotSupportedException
                                    || ex is DriveNotFoundException
                                    || ex is IOException)
        {
            Logger.WriteLineError($"Error creating output directory '{outputPath}': {ex.Message}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method to print the scope tree structure for verbose output.
    /// </summary>
    private static void PrintScopeTree(Scope scope, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        Console.WriteLine($"{indent}{scope.Name} ({scope.Kind})");
        
        if (scope.Bindings.Any())
        {
            Console.WriteLine($"{indent}  Variables:");
            foreach (var binding in scope.Bindings.Values)
            {
                Console.WriteLine($"{indent}    - {binding.Name} ({binding.Kind})");
            }
        }

        foreach (var child in scope.Children)
        {
            PrintScopeTree(child, indentLevel + 1);
        }
    }

    private static void AnalyzeUnusedForModules(Modules modules)
    {
        foreach (var module in modules._modules.Values)
        {
            AnalyzeUnusedForModule(module);
        }
    }

    private static void AnalyzeUnusedForModule(ModuleDefinition module)
    {
            Console.WriteLine();
            Console.WriteLine($"Analyzing unused code for module: {module.Path}");
            var unusedCodeAnalyzer = new UnusedCodeAnalyzer();
            var unusedCodeResult = unusedCodeAnalyzer.Analyze(module.Ast);

            if (unusedCodeResult.UnusedFunctions.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Unused Functions:");
                foreach (var unusedFunc in unusedCodeResult.UnusedFunctions)
                {
                    Console.WriteLine($"  - {unusedFunc}");
                }
            }

            if (unusedCodeResult.UnusedProperties.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Unused Properties:");
                foreach (var unusedProp in unusedCodeResult.UnusedProperties)
                {
                    Console.WriteLine($"  - {unusedProp}");
                }
            }

            if (unusedCodeResult.UnusedVariables.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Unused Variables:");
                foreach (var unusedVar in unusedCodeResult.UnusedVariables)
                {
                    Console.WriteLine($"  - {unusedVar}");
                }
            }

            if (unusedCodeResult.Warnings.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Unused Code Analysis Warnings:");
                foreach (var warning in unusedCodeResult.Warnings)
                {
                    Logger.WriteLineWarning($"Warning: {warning}");
                }
            }
        
    }

}