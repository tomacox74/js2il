using Js2IL.Services;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using System.IO;


namespace Js2IL.Tests
{
    public class GeneratorTests
    {
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        public GeneratorTests()
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

            _outputPath = Path.Combine(_outputPath, "GeneratorTests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        [Fact]
        public Task BinaryOperator_AddNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_AddStringString()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_AddStringNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_SubNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_MulNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_DivNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_ModNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_LessThan()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task Generate_ForLoopCountToFive()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        private Task GenerateTest(string testName)
        {
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, testName, _outputPath);

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            var il = Utilities.AssemblyToText.ConvertToText(expectedPath);
            return Verify(il, _verifySettings);
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
