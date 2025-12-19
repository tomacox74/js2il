using Js2IL.Services;
using Js2IL.SymbolTables;
namespace Js2IL;

public class Compiler
{
    private readonly bool analyzeUnused;
    private readonly string? outputDirectory;
    private readonly bool verbose;

    private readonly SymbolTableBuilder _symbolTableBuilder = new SymbolTableBuilder();

    private readonly ModuleLoader _moduleLoader;

    public Compiler(CompilerOptions options, ModuleLoader moduleLoader)
    {
        this.outputDirectory = options.OutputDirectory;
        this.verbose = options.Verbose;
        this.analyzeUnused = options.AnalyzeUnused;
        this._moduleLoader = moduleLoader;
    }   

    public bool Compile(string inputFile)
    {
        var modules = this._moduleLoader.LoadModules(inputFile);
        if (modules == null)
        {
            return false;
        }

        var module = modules.rootModule;

        // Step 3: Analyze unused code (if requested)
        if (this.analyzeUnused)
        {
            Console.WriteLine("\nAnalyzing unused code...");
            var unusedCodeAnalyzer = new UnusedCodeAnalyzer();
            var unusedCodeResult = unusedCodeAnalyzer.Analyze(module.Ast);

            if (unusedCodeResult.UnusedFunctions.Any())
            {
                Console.WriteLine("\nUnused Functions:");
                foreach (var unusedFunc in unusedCodeResult.UnusedFunctions)
                {
                    Console.WriteLine($"  - {unusedFunc}");
                }
            }

            if (unusedCodeResult.UnusedProperties.Any())
            {
                Console.WriteLine("\nUnused Properties:");
                foreach (var unusedProp in unusedCodeResult.UnusedProperties)
                {
                    Console.WriteLine($"  - {unusedProp}");
                }
            }

            if (unusedCodeResult.UnusedVariables.Any())
            {
                Console.WriteLine("\nUnused Variables:");
                foreach (var unusedVar in unusedCodeResult.UnusedVariables)
                {
                    Console.WriteLine($"  - {unusedVar}");
                }
            }

            if (unusedCodeResult.Warnings.Any())
            {
                Console.WriteLine("\nUnused Code Analysis Warnings:");
                foreach (var warning in unusedCodeResult.Warnings)
                {
                    Logger.WriteLineWarning($"Warning: {warning}");
                }
            }
        }

        // Step 4: Build scope tree
        Console.WriteLine("\nBuilding scope tree...");
        var symbolTable = _symbolTableBuilder.Build(module);

        if (this.verbose)
        {
            Console.WriteLine("\nScope Tree Structure:");
            PrintScopeTree(symbolTable.Root, 0);
        }

        // Step 5: Generate IL
        // NOTE: some checkes such as the existance of the the file parsed.InputFile are done by powerargs for us
        Console.WriteLine("\nGenerating dotnet assembly...");
        var assemblyGenerator = new AssemblyGenerator();

        // Resolve and validate output directory; create if missing
        string outputPath;
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

        var assemblyName = Path.GetFileNameWithoutExtension(inputFile);

        assemblyGenerator.Generate(module.Ast, symbolTable, assemblyName, outputPath);

        Console.WriteLine($"\nConversion complete. Output written to {outputPath}");

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

}