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
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_AddStringNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_AddStringString()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_DivNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_LessThan()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_ModNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_MulNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_SubNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task Generate_ForLoopCountToFive()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task Generate_ForLoopCountDownFromFive()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task ObjectLiteral()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_LessThanOrEqual()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_GreaterThanOrEqual()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            return GenerateTest(testName);
        }

        [Fact]
        public Task BinaryOperator_Equal()
        {
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
