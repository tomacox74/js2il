using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public abstract class ExecutionTestsBase
    {
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        protected ExecutionTestsBase(string testCategory)
        {
            _parser = new JavaScriptParser();
            _validator = new JavaScriptAstValidator();
            _verifySettings.DisableDiff();

            // create a temp directory for the generated assemblies
            _outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            _outputPath = Path.Combine(_outputPath, $"{testCategory}.ExecutionTests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        protected Task ExecutionTest(string testName, [CallerFilePath] string sourceFilePath = "")
        {
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, testName, _outputPath);

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            var il = ExecuteGeneratedAssembly(expectedPath);
            //ExecuteGeneratedAssemblyInProc(expectedPath);
            
            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                settings.UseDirectory(directory);
            }
            return Verify(il, settings);
        }

        private string ExecuteGeneratedAssembly(string assemblyPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{assemblyPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process!.WaitForExit();

            string stdOut = process.StandardOutput.ReadToEnd();
            string stdErr = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
            {
                throw new Exception($"dotnet execution failed:\n{stdErr}");
            }

            return stdOut;
        }

        private string ExecuteGeneratedAssemblyInProc(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var entryPoint = assembly.EntryPoint;
            if (entryPoint == null)
            {
                throw new InvalidOperationException("No entry point found in the generated assembly.");
            }
            var parameters = entryPoint.GetParameters().Select(p => p.ParameterType).ToArray();
            var instance = Activator.CreateInstance(assembly.GetType(entryPoint.DeclaringType!.FullName!)!);
            entryPoint.Invoke(instance, parameters.Length == 0 ? null : new object[] { });

            return "Execution completed successfully.";
        }

        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Js2IL.Tests.JavaScript.{testName}.js";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
