using Xunit;

namespace Js2IL.Tests;

public class ModuleLoaderTests
{
    [Fact]
    public void LoadModules_ParseError_ReturnsNullAndLogsErrors()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var loader = new ModuleLoader(options, fileSystem, logger);

        var modulePath = Path.GetFullPath("C:\\temp\\bad.js");
        fileSystem.AddFile(modulePath, "while (false) { notALoop: { continue notALoop; } }");

        var modules = loader.LoadModules(modulePath);

        Assert.Null(modules);
        Assert.Contains("Parse Errors", logger.Errors);
        Assert.Contains("does not denote an iteration statement", logger.Errors);
    }
}
