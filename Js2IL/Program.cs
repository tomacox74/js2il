using PowerArgs;
using Js2IL.Services;
using Js2IL.SymbolTables;
using Acornima.Ast;
using System.IO;

namespace Js2IL;

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class Js2ILArgs
{
    [ArgRequired]
    [ArgPosition(0)]
    [ArgDescription("The JavaScript file to convert")]
    [ArgExistingFile]
    public required string InputFile { get; set; }

    [ArgPosition(1)]
    [ArgDescription("The output path for the generated IL")]
    [ArgExistingDirectory]
    public string? OutputPath { get; set; }

    [ArgDescription("Enable verbose output")]
    public bool Verbose { get; set; }

    [ArgDescription("Analyze and report unused properties and methods")]
    public bool AnalyzeUnused { get; set; }
}

public static class TestClass
{
}

class Program
{
    static void Main(string[] args)
    {
        JavaScriptRuntime.Console.Log("1", "2");


        try
        {
            var parsed = Args.Parse<Js2ILArgs>(args);
            Console.WriteLine($"Converting {parsed.InputFile} to IL...");

            // Step 1: Parse JavaScript to AST
            Console.WriteLine("Parsing JavaScript...");
            var jsSource = File.ReadAllText(parsed.InputFile);
            var parser = new JavaScriptParser();
            var ast = parser.ParseJavaScript(jsSource);

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
                Console.WriteLine("\nValidation Errors:");
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"Error: {error}");
                }
                return;
            }

            if (validationResult.Warnings.Any())
            {
                Console.WriteLine("\nValidation Warnings:");
                foreach (var warning in validationResult.Warnings)
                {
                    Console.WriteLine($"Warning: {warning}");
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
                        Console.WriteLine($"Warning: {warning}");
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
            var outputPath = parsed.OutputPath;
            if (parsed.OutputPath == null)
            {
                outputPath = Path.GetDirectoryName(Path.GetFullPath(parsed.InputFile));
            }

            var assemblyName = Path.GetFileNameWithoutExtension(parsed.InputFile);

            assemblyGenerator.Generate(ast, symbolTable, assemblyName, outputPath!);

            Console.WriteLine($"\nConversion complete. Output written to {outputPath}");
        }
        catch (ArgException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<Js2ILArgs>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner error: {ex.InnerException.Message}");
            }
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
}
