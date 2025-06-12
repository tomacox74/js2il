using Js2IL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void Generate_ValidProgram_CreatesAssemblyMetadata()
        {
            // Arrange
            var js = @"var x = 1 + 2;
            console.log('X is',x);
        ";
            var ast = _parser.ParseJavaScript(js);
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, "TestAssembly", "output");

            Assert.NotNull(generator.metadataBuilder);

        }
    }
}
