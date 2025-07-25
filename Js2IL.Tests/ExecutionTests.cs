﻿using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public class ExecutionTests
    {
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        public ExecutionTests()
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

            _outputPath = Path.Combine(_outputPath, "ExecutionTests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }


        // Sorted test methods
        [Fact]
        public Task ArrayLiteral() { var testName = nameof(ArrayLiteral); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddNumberNumber() { var testName = nameof(BinaryOperator_AddNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringNumber() { var testName = nameof(BinaryOperator_AddStringNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringString() { var testName = nameof(BinaryOperator_AddStringString); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber() { var testName = nameof(BinaryOperator_BitwiseAndNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber() { var testName = nameof(BinaryOperator_BitwiseOrNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber() { var testName = nameof(BinaryOperator_BitwiseXorNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_DivNumberNumber() { var testName = nameof(BinaryOperator_DivNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_Equal() { var testName = nameof(BinaryOperator_Equal); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber() { var testName = nameof(BinaryOperator_ExpNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThan() { var testName = nameof(BinaryOperator_GreaterThan); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThanOrEqual() { var testName = nameof(BinaryOperator_GreaterThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber() { var testName = nameof(BinaryOperator_LeftShiftNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThan() { var testName = nameof(BinaryOperator_LessThan); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThanOrEqual() { var testName = nameof(BinaryOperator_LessThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_ModNumberNumber() { var testName = nameof(BinaryOperator_ModNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_MulNumberNumber() { var testName = nameof(BinaryOperator_MulNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber() { var testName = nameof(BinaryOperator_RightShiftNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_SubNumberNumber() { var testName = nameof(BinaryOperator_SubNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber() { var testName = nameof(BinaryOperator_UnsignedRightShiftNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountDownFromFive() { var testName = nameof(ControlFlow_ForLoop_CountDownFromFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountToFive() { var testName = nameof(ControlFlow_ForLoop_CountToFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_GreaterThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_LessThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return ExecutionTest(testName); }

        [Fact(Skip = "process/argv not yet supported")]
        public Task Environment_EnumerateProcessArgV() { var testName = nameof(Environment_EnumerateProcessArgV); return ExecutionTest(testName); }

        [Fact]
        public Task Function_HelloWorld() { var testName = nameof(Function_HelloWorld); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectLiteral() { var testName = nameof(ObjectLiteral); return ExecutionTest(testName); }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() { var testName = nameof(UnaryOperator_MinusMinusPostfix); return ExecutionTest(testName); }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() { var testName = nameof(UnaryOperator_PlusPlusPostfix); return ExecutionTest(testName); }

        private Task ExecutionTest(string testName)
        {
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, testName, _outputPath);

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            var il = ExecuteGeneratedAssembly(expectedPath);
            //ExecuteGeneratedAssemblyInProc(expectedPath);
            return Verify(il, _verifySettings);
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
