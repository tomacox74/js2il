using PowerArgs;
using Js2IL.Services;
using Js2IL.SymbolTables;
using Acornima.Ast;
using System.IO;
using System.Reflection;

namespace Js2IL;

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class Js2ILArgs
{
    [ArgRequired]
    [ArgPosition(0)]
    [ArgDescription("The JavaScript file to convert")]
    [ArgExistingFile]
    [ArgShortcut("i")]
    public required string InputFile { get; set; }

    [ArgPosition(1)]
    [ArgDescription("The output path for the generated IL")]
    [ArgShortcut("o")]
    public string? OutputPath { get; set; }

    [ArgDescription("Enable verbose output")]
    [ArgShortcut("v")]
    public bool Verbose { get; set; }

    [ArgDescription("Analyze and report unused properties and methods")]
    [ArgShortcut("a")]
    public bool AnalyzeUnused { get; set; }

    [ArgDescription("Show version information and exit")]
    // Long-form only to avoid -v conflict with Verbose
    public bool Version { get; set; }

    [ArgDescription("Show help and exit")]
    [HelpHook]
    [ArgShortcut("?")]
    [ArgShortcut("h")]
    public bool Help { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var parsed = Args.Parse<Js2ILArgs>(args);
            if (parsed == null)
            {
                // PowerArgs already printed an error/usage; treat as failure
                Environment.ExitCode = 1;
                return;
            }

            // --help handling
            if (parsed.Help)
            {
                PrintUsage(Console.Out);
                return;
            }

            // --version handling
            if (parsed.Version)
            {
                var asm = Assembly.GetExecutingAssembly();
                var name = asm.GetName();
                var version = name.Version?.ToString() ?? "unknown";
                Console.WriteLine($"js2il {version}");
                return;
            }
            Console.WriteLine($"Converting {parsed.InputFile} to IL...");

            // Step 1: Parse JavaScript to AST
            Console.WriteLine("Parsing JavaScript...");
            var jsSource = File.ReadAllText(parsed.InputFile);
            var parser = new JavaScriptParser();
            var ast = parser.ParseJavaScript(jsSource, parsed.InputFile);

            if (parsed.Verbose)
            {
                Console.WriteLine("AST Structure:");
                parser.VisitAst(ast, node =>
                {
                    Console.Write($"Node Type: {node.Type}");
                    if (node is Acornima.Ast.NumericLiteral num)
                        Console.Write($", Value: {num.Value}");
                    if (node is Acornima.Ast.UnaryExpression unary)
                        Console.Write($", Operator: {unary.Operator}");
                    Console.WriteLine();
                });
            }

            // Step 2: Validate AST
            Console.WriteLine("\nValidating the Javascript is supported...");
            var validator = new JavaScriptAstValidator();
            var validationResult = validator.Validate(ast);

            if (!validationResult.IsValid)
            {
                WriteLineError("\nValidation Errors:");
                foreach (var error in validationResult.Errors)
                {
                    WriteLineError($"Error: {error}");
                }
                Environment.ExitCode = 1;
                return;
            }

            if (validationResult.Warnings.Any())
            {
                Console.WriteLine("\nValidation Warnings:");
                foreach (var warning in validationResult.Warnings)
                {
                    WriteLineWarning($"Warning: {warning}");
                }
            }

            // Step 3: Analyze unused code (if requested)
            if (parsed.AnalyzeUnused)
            {
                Console.WriteLine("\nAnalyzing unused code...");
                var unusedCodeAnalyzer = new UnusedCodeAnalyzer();
                var unusedCodeResult = unusedCodeAnalyzer.Analyze(ast);

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
                        WriteLineWarning($"Warning: {warning}");
                    }
                }
            }

            // Step 4: Build scope tree
            Console.WriteLine("\nBuilding scope tree...");
            var symbolTableBuilder = new SymbolTableBuilder();
            var symbolTable = symbolTableBuilder.Build(ast, parsed.InputFile);

            if (parsed.Verbose)
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
            if (string.IsNullOrWhiteSpace(parsed.OutputPath))
            {
                outputPath = Path.GetDirectoryName(Path.GetFullPath(parsed.InputFile))!;
            }
            else
            {
                outputPath = Path.GetFullPath(parsed.OutputPath);
            }

            // If a file exists at the output path, fail with a clear message
            if (File.Exists(outputPath))
            {
                WriteLineError($"Error: Output path '{outputPath}' is a file. Provide a directory path.");
                Environment.ExitCode = 1;
                return;
            }

            // Ensure the directory exists (treat common IO issues as known failures)
            try
            {
                Directory.CreateDirectory(outputPath);
                if (parsed.Verbose)
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
                WriteLineError($"Error creating output directory '{outputPath}': {ex.Message}");
                Environment.ExitCode = 1;
                return;
            }

            var assemblyName = Path.GetFileNameWithoutExtension(parsed.InputFile);

            assemblyGenerator.Generate(ast, symbolTable, assemblyName, outputPath);

            Console.WriteLine($"\nConversion complete. Output written to {outputPath}");
        }
        catch (ArgException ex)
        {
            WriteLineError(ex.Message);
            // Avoid potential template generation issues by printing a simple usage message
            PrintUsage(Console.Error);
            Environment.ExitCode = 1;
            return;
        }
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

    // Overload to print usage to a chosen writer (used for error cases to stderr)
    private static void PrintUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: js2il <InputFile> [<OutputPath>] [options]");
        writer.WriteLine();
        writer.WriteLine("Option               Description");
        writer.WriteLine("-i, --input          The JavaScript file to convert (positional supported)");
        writer.WriteLine("-o, --output         The output path for the generated IL (created if missing)");
        writer.WriteLine("-v, --verbose        Enable verbose output");
        writer.WriteLine("-a, --analyzeunused  Analyze and report unused properties and methods");
        writer.WriteLine("    --version        Show version information and exit");
    writer.WriteLine("-h, -?, --help       Show help and exit");
    }

    private static void WriteLineWarning(string message)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }

    private static void WriteLineError(string message)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }
}
