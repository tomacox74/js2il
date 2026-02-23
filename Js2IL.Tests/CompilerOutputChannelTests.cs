using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Tests;

public class CompilerOutputChannelTests
{
    [Fact]
    public void Compile_VerboseDiagnostics_DoNotPolluteUxStream()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var inputPath = Path.Combine(tempRoot, "input.js");
            var outputPath = Path.Combine(tempRoot, "out");

            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(inputPath, "\"use strict\";\nconsole.log('hello');\n");

            var logger = new TestLogger();
            var options = new CompilerOptions
            {
                OutputDirectory = outputPath,
                Verbose = true
            };

            var services = CompilerServices.BuildServiceProvider(options, fileSystem, logger);
            var compiler = services.GetRequiredService<Compiler>();

            var success = compiler.Compile(inputPath);

            Assert.True(success);
            Assert.Contains("Compilation succeeded", logger.Output, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Build the symbol tables", logger.Output, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("[TwoPhase]", logger.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }
}
