using Js2IL.IR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Js2IL.Tests;

[Collection("IRPipelineMetrics")]
public class IRNewExpressionUserDefinedClassTests
{
    [Fact]
    public void IR_NewExpression_UserDefinedClass_IsCompiledByIR()
    {
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();

        var js = @"
class Foo {
  constructor(a = 123) {
    // keep ctor non-trivial so it is emitted and declared
    if (a !== 123) { throw new Error('bad'); }
  }
}
new Foo();
";

        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "IRNewExpressionUserDefinedClass");
        Directory.CreateDirectory(outputPath);

        var testFilePath = Path.Combine(outputPath, "test.js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(testFilePath, js);

        var options = new CompilerOptions { OutputDirectory = outputPath };
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, new TestLogger());
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        var ok = compiler.Compile(testFilePath);
        Assert.True(ok);

        var stats = IRPipelineMetrics.GetStats();
        Assert.Equal(1, stats.MainMethodAttempts);
        Assert.Equal(1, stats.MainMethodSuccesses);
        Assert.Null(IRPipelineMetrics.GetLastFailure());
    }

      [Fact]
      public void IR_NewExpression_UserDefinedClass_ConstructorArgCountMismatch_ThrowsNotSupported()
      {
        var js = @"
    class Foo {
      constructor(a, b = 2) {
      }
    }
    new Foo();
    ";

        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "IRNewExpressionUserDefinedClass_ArgCountMismatch");
        Directory.CreateDirectory(outputPath);

        var testFilePath = Path.Combine(outputPath, "test.js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(testFilePath, js);

        var options = new CompilerOptions { OutputDirectory = outputPath };
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, new TestLogger());
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        var ex = Assert.Throws<NotSupportedException>(() => compiler.Compile(testFilePath));
        Assert.Contains("Constructor for class 'Foo' expects 1-2 argument(s) but call site has 0.", ex.Message);
      }
}
