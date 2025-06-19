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


namespace Js2IL.Tests
{
    public class GeneratorTests
    {
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;

        public GeneratorTests()
        {
            _parser = new JavaScriptParser();
            _validator = new JavaScriptAstValidator();
        }

        [Fact]
        public Task Generate_AdditionNumberNumber()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, "TestAssembly", "output");

            var il = Utilities.AssemblyToText.ConvertToText(@"c:\\git\test.dll");
            return Verify(il);
        }

        [Fact]
        public Task Generate_AdditionStringString()
        {
            // Arrange
            var testName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, "TestAssembly", "output");

            var il = Utilities.AssemblyToText.ConvertToText(@"c:\\git\test.dll");
            return Verify(il);
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
