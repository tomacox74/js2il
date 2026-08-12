using Jroc.IR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Jroc.Tests;

[Collection("IRPipelineMetrics")]
public class IRNewExpressionUserDefinedClassTests
{
    [Fact]
    public void IR_NewExpression_UserDefinedClass_IsCompiledByIR()
    {
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();

        var js = @"
      ""use strict"";

class Foo {
  constructor(a = 123) {
    // keep ctor non-trivial so it is emitted and declared
    if (a !== 123) { throw new Error('bad'); }
  }
}
new Foo();
";

        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "IRNewExpressionUserDefinedClass");
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
      public void IR_NewExpression_UserDefinedClass_MissingArguments_CompileAsUndefined()
      {
        var js = @"
      ""use strict"";

    class Foo {
      constructor(a, b = 2) {
      }
    }
    new Foo();
    ";

        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "IRNewExpressionUserDefinedClass_ArgCountMismatch");
        Directory.CreateDirectory(outputPath);

        var testFilePath = Path.Combine(outputPath, "test.js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(testFilePath, js);

        var options = new CompilerOptions { OutputDirectory = outputPath };
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, new TestLogger());
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(testFilePath));
      }
}
