using PowerArgs;
using Js2IL.Services;
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
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var parsed = Args.Parse<Js2ILArgs>(args);
            Console.WriteLine($"Converting {parsed.InputFile} to IL...");

            // Step 1: Parse JavaScript to AST
            Console.WriteLine("Step 1: Parsing JavaScript...");
            var jsSource = File.ReadAllText(parsed.InputFile);
            var parser = new JavaScriptParser();
            var ast = parser.ParseJavaScript(jsSource);

            if (parsed.Verbose)
            {
                Console.WriteLine("AST Structure:");
                parser.VisitAst(ast, node => Console.WriteLine($"Node Type: {node.Type}"));
            }

            // Step 2: Validate AST
            Console.WriteLine("\nStep 2: Validating AST...");
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

            // Step 3: Generate IL
            // NOTE: some checkes such as the existance of the the file parsed.InputFile are done by powerargs for us
            Console.WriteLine("\nStep 3: Generating dotnet assembly...");
            var assemblyGenerator = new AssemblyGenerator();
            var outputPath = parsed.OutputPath;
            if (parsed.OutputPath == null)
            {
                outputPath = Path.GetDirectoryName(Path.GetFullPath(parsed.InputFile));
            }
            
            var assemblyName = Path.GetFileNameWithoutExtension(parsed.InputFile);

            assemblyGenerator.Generate(ast, assemblyName, outputPath!);

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
}
