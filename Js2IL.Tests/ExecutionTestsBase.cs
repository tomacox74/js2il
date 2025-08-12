using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.Loader;

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

            // Run in-proc to avoid process startup overhead and capture output.
            var il = ExecuteGeneratedAssemblyInProc(expectedPath);
            
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
            // Capture System.Console output written by the generated program.
            var originalOut = System.Console.Out;
            var originalErr = System.Console.Error;
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            try
            {
                System.Console.SetOut(writer);
                System.Console.SetError(writer);

                // Use isolated ALC so dependencies (JavaScriptRuntime, System.Runtime ref) resolve from output folder
                var alc = new IsolatedLoadContext(Path.GetDirectoryName(assemblyPath)!);
                var assembly = alc.LoadFromAssemblyPath(assemblyPath);
                var entryPoint = assembly.EntryPoint;
                if (entryPoint == null)
                {
                    throw new InvalidOperationException("No entry point found in the generated assembly.");
                }

                // Generated Program.Main has no parameters and is static.
                var paramInfos = entryPoint.GetParameters();
                object?[]? args = paramInfos.Length == 0 ? null : new object?[] { Array.Empty<string>() };
                entryPoint.Invoke(null, args);

                writer.Flush();
                var output = sb.ToString();
                alc.Unload();
                return output;
            }
            finally
            {
                System.Console.SetOut(originalOut);
                System.Console.SetError(originalErr);
            }
        }

        private sealed class IsolatedLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public IsolatedLoadContext(string mainAssemblyDir) : base(isCollectible: true)
            {
                _resolver = new AssemblyDependencyResolver(mainAssemblyDir);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                string? path = _resolver.ResolveAssemblyToPath(assemblyName);
                if (path != null && File.Exists(path))
                {
                    return LoadFromAssemblyPath(path);
                }
                return null;
            }
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
