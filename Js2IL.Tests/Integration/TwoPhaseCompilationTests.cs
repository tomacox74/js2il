using System;
using System.IO;
using Js2IL.Services;
using Js2IL.Services.TwoPhaseCompilation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Js2IL.Tests.Integration
{
    /// <summary>
    /// Tests for the Two-Phase Compilation Pipeline.
    /// These tests verify that the two-phase compilation pipeline works correctly.
    /// </summary>
    public class TwoPhaseCompilationTests
    {
        [Fact]
        public void TwoPhaseCompilation_ArrowFunction_CompilesSuccessfully()
        {
            // Simple arrow function test
            var js = @"
const add = (a, b) => a + b;
console.log(add(1, 2));
";
            AssertCompilesWithTwoPhase(js);
        }

        [Fact]
        public void TwoPhaseCompilation_FunctionExpression_CompilesSuccessfully()
        {
            // Function expression test
            var js = @"
const multiply = function(a, b) {
    return a * b;
};
console.log(multiply(3, 4));
";
            AssertCompilesWithTwoPhase(js);
        }

        [Fact]
        public void TwoPhaseCompilation_NestedArrows_CompilesSuccessfully()
        {
            // Nested arrow functions test
            var js = @"
const outer = (x) => {
    const inner = (y) => x + y;
    return inner(10);
};
console.log(outer(5));
";
            AssertCompilesWithTwoPhase(js);
        }

        [Fact]
        public void TwoPhaseCompilation_ArrayMapWithArrow_CompilesSuccessfully()
        {
            // Array.map with arrow callback
            var js = @"
const arr = [1, 2, 3];
const doubled = arr.map(x => x * 2);
console.log(doubled.join(','));
";
            AssertCompilesWithTwoPhase(js);
        }

        [Fact]
        public void TwoPhaseCompilation_MixedCallables_CompilesSuccessfully()
        {
            // Mix of function declarations, expressions, and arrows
            var js = @"
function greet(name) {
    return 'Hello, ' + name;
}

const transform = function(s) {
    return s.toUpperCase();
};

const process = (items) => items.map(item => transform(greet(item)));

console.log(process(['world']).join(', '));
";
            AssertCompilesWithTwoPhase(js);
        }

        [Fact]
        public void TwoPhaseCompilation_ClassWithMethods_CompilesSuccessfully()
        {
            // Note: Arrow functions with 'this' inside class constructors not yet supported (see #244)
            // Using a simple class pattern without arrows in constructor
            var js = @"
class Calculator {
    constructor(initial) {
        this.value = initial;
    }
    
    add(n) {
        this.value = this.value + n;
        return this;
    }
    
    getValue() {
        return this.value;
    }
}

const calc = new Calculator(10);
calc.add(5);
console.log(calc.getValue());
";
            AssertCompilesWithTwoPhase(js);
        }

        /// <summary>
        /// Verifies the invariant: strict mode enforces lookup-only behavior.
        /// When strict mode is enabled, CallableRegistry.GetDeclaredToken throws if a callable is not found.
        /// </summary>
        [Fact]
        public void TwoPhaseCompilation_StrictMode_ThrowsWhenCallableNotDeclared()
        {
            // Create a CallableRegistry and enable strict mode
            var registry = new CallableRegistry();
            registry.StrictMode = true;

            // Create a dummy CallableId that was never declared
            var undeclaredCallable = new CallableId
            {
                Kind = CallableKind.Arrow,
                DeclaringScopeName = "TestModule",
                Name = "undeclared",
                JsParamCount = 0,
                Location = new SourceLocation(1, 1),
                AstNode = null
            };

            // Attempting to get a declared token for an undeclared callable should throw
            var ex = Assert.Throws<InvalidOperationException>(() => registry.GetDeclaredToken(undeclaredCallable));
            Assert.Contains("missing callable token", ex.Message.ToLowerInvariant());
        }

        private void AssertCompilesWithTwoPhase(string jsCode)
        {
            var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "TwoPhase", Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputDir);
            
            var inputFile = Path.Combine(outputDir, "test.js");
            File.WriteAllText(inputFile, jsCode);

            try
            {
                var options = new CompilerOptions
                {
                    OutputDirectory = outputDir,
                    Verbose = true
                };

                var serviceProvider = CompilerServices.BuildServiceProvider(options, fileSystem: null, new TestLogger());
                var compiler = serviceProvider.GetRequiredService<Compiler>();
                
                var result = compiler.Compile(inputFile);
                
                Assert.True(result, $"Compilation failed with TwoPhaseCompilation enabled.\nCode:\n{jsCode}");

                var outputDll = Path.Combine(outputDir, "test.dll");
                Assert.True(File.Exists(outputDll), $"Output assembly missing: {outputDll}");
                Assert.True(new FileInfo(outputDll).Length > 0, "Output assembly is empty");
            }
            finally
            {
                try { Directory.Delete(outputDir, true); } catch { /* ignore cleanup errors */ }
            }
        }
    }
}
