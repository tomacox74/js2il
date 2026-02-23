using Js2IL.Services;
using Js2IL.SymbolTables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Js2IL;

public class Compiler
{
    private readonly bool analyzeUnused;
    private readonly string? outputDirectory;
    private readonly bool diagnosticsEnabled;

    private readonly SymbolTableBuilder _symbolTableBuilder = new SymbolTableBuilder();

    private readonly ModuleLoader _moduleLoader;

    private readonly IServiceProvider _serviceProvider;
    private readonly ICompilerOutput _ux;
    private readonly Microsoft.Extensions.Logging.ILogger<Compiler> _diagnosticLogger;

    public Compiler(
        IServiceProvider serviceProvider,
        CompilerOptions options,
        ModuleLoader moduleLoader,
        ICompilerOutput ux,
        Microsoft.Extensions.Logging.ILogger<Compiler> diagnosticLogger)
    {
        this.outputDirectory = options.OutputDirectory;
        this.diagnosticsEnabled = options.DiagnosticsEnabled;
        this.analyzeUnused = options.AnalyzeUnused;
        this._moduleLoader = moduleLoader;
        this._serviceProvider = serviceProvider;
        this._ux = ux;
        this._diagnosticLogger = diagnosticLogger;
    }   

    public bool Compile(string inputFile, string? rootModuleIdOverride = null)
    {
        // When diagnostics are enabled, capture IR pipeline failure reasons to aid debugging.
        if (this.diagnosticsEnabled)
        {
            IR.IRPipelineMetrics.Enabled = true;
            IR.IRPipelineMetrics.Reset();
        }

        var modules = this._moduleLoader.LoadModules(inputFile, rootModuleIdOverride);
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
        if (this.diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("Build the symbol tables");
        }
        foreach (var mod in modules._modules.Values)
        {
            _symbolTableBuilder.Build(mod);
        }

        if (this.diagnosticsEnabled)
        {
             
            foreach (var mod in modules._modules.Values)
            {
                _diagnosticLogger.LogInformation(string.Empty);
                _diagnosticLogger.LogInformation("Scope Tree Structure:");
                _diagnosticLogger.LogInformation("Module: {ModulePath}", mod.Path);
                var symbolTable = mod.SymbolTable!;
                PrintScopeTree(symbolTable.Root, 0);
            }
        }

        // Generate IL assembly
        if (this.diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("Generating dotnet assembly...");
        }
        var assemblyGenerator = _serviceProvider.GetRequiredService<AssemblyGenerator>();

        // Resolve and validate output directory; create if missing
        if (!EnsureOutputPathExists(inputFile, this.outputDirectory, out var outputPath))
        {
            return false;
        }

        var assemblyName = Path.GetFileNameWithoutExtension(inputFile);

        assemblyGenerator.Generate(modules, assemblyName, outputPath);

        _ux.WriteLine($"Compilation succeeded. Output written to {outputPath}");

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
            var ext = Path.GetExtension(outputDirectory);
            if (string.Equals(ext, ".dll", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase))
            {
                _ux.WriteLineWarning(
                    $"Warning: Output path '{outputDirectory}' is treated as a directory. Did you intend it to be an output directory name?");
            }

            outputPath = Path.GetFullPath(outputDirectory);
        }

        // If a file exists at the output path, fail with a clear message
        if (File.Exists(outputPath))
        {
            _ux.WriteLineError($"Error: Output path '{outputPath}' is a file. Provide a directory path.");
            return false;
        }

        // Ensure the directory exists (treat common IO issues as known failures)
        try
        {
            Directory.CreateDirectory(outputPath);
            if (this.diagnosticsEnabled)
            {
                _diagnosticLogger.LogInformation("Ensured output directory exists: {OutputPath}", outputPath);
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException
                                    || ex is ArgumentException
                                    || ex is PathTooLongException
                                    || ex is NotSupportedException
                                    || ex is DriveNotFoundException
                                    || ex is IOException)
        {
            _ux.WriteLineError($"Error creating output directory '{outputPath}': {ex.Message}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method to print the scope tree structure for verbose output.
    /// </summary>
    private void PrintScopeTree(Scope scope, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        _diagnosticLogger.LogInformation("{ScopeLine}", $"{indent}{scope.Name} ({scope.Kind})");
        
        if (scope.Bindings.Any())
        {
            _diagnosticLogger.LogInformation("{ScopeLine}", $"{indent}  Variables:");
            foreach (var binding in scope.Bindings.Values)
            {
                var capturedSuffix = binding.IsCaptured ? ", Captured" : string.Empty;
                var stableSuffix = binding.IsStableType ? ", Stable" : string.Empty;
                _diagnosticLogger.LogInformation("{ScopeLine}", $"{indent}    - {binding.Name} ({binding.Kind}{capturedSuffix}{stableSuffix})");
            }
        }

        foreach (var child in scope.Children)
        {
            PrintScopeTree(child, indentLevel + 1);
        }
    }

    private void AnalyzeUnusedForModules(Modules modules)
    {
        foreach (var module in modules._modules.Values)
        {
            AnalyzeUnusedForModule(module);
        }
    }

    private void AnalyzeUnusedForModule(ModuleDefinition module)
    {
            _ux.WriteLine();
            _ux.WriteLine($"Analyzing unused code for module: {module.Path}");
            var unusedCodeAnalyzer = new UnusedCodeAnalyzer();
            var unusedCodeResult = unusedCodeAnalyzer.Analyze(module.Ast);

            if (unusedCodeResult.UnusedFunctions.Any())
            {
                _ux.WriteLine();
                _ux.WriteLine("Unused Functions:");
                foreach (var unusedFunc in unusedCodeResult.UnusedFunctions)
                {
                    _ux.WriteLine($"  - {unusedFunc}");
                }
            }

            if (unusedCodeResult.UnusedProperties.Any())
            {
                _ux.WriteLine();
                _ux.WriteLine("Unused Properties:");
                foreach (var unusedProp in unusedCodeResult.UnusedProperties)
                {
                    _ux.WriteLine($"  - {unusedProp}");
                }
            }

            if (unusedCodeResult.UnusedVariables.Any())
            {
                _ux.WriteLine();
                _ux.WriteLine("Unused Variables:");
                foreach (var unusedVar in unusedCodeResult.UnusedVariables)
                {
                    _ux.WriteLine($"  - {unusedVar}");
                }
            }

            if (unusedCodeResult.Warnings.Any())
            {
                _ux.WriteLine();
                _ux.WriteLine("Unused Code Analysis Warnings:");
                foreach (var warning in unusedCodeResult.Warnings)
                {
                    _ux.WriteLineWarning($"Warning: {warning}");
                }
            }
        
    }

}
