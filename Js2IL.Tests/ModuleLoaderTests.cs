using System;
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

    [Fact]
    public void LoadModules_DependencyMissingUseStrict_ReturnsNullAndLogsErrors()
    {
        var fileSystem = new MockFileSystem();
        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = false };
        var loader = new ModuleLoader(options, fileSystem, logger);

        var rootPath = Path.GetFullPath("C:\\temp\\root.js");
        var depPath = Path.GetFullPath("C:\\temp\\dep.js");

        fileSystem.AddFile(rootPath, "\"use strict\";\nconst d = require('./dep');\nconsole.log(d);\n");
        fileSystem.AddFile(depPath, "module.exports = 1;\n");

        var modules = loader.LoadModules(rootPath);

        Assert.Null(modules);
        Assert.Contains("Validation Errors", logger.Errors);
        Assert.Contains("requires strict mode", logger.Errors, StringComparison.OrdinalIgnoreCase);
    }
}
