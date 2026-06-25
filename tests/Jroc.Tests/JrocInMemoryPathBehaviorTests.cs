using Jroc.Runtime;

namespace Jroc.Tests;

public sealed class JrocInMemoryPathBehaviorTests
{
    [Fact]
    public void CompileAndLoadModule_DoesNotMaterializeAssemblyToDiskImplicitly()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "InMemoryPathBehavior", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var entryPath = Path.Combine(tempRoot, "inmemory-location-check.js");

            using var module = JrocInMemoryCompiler.CompileAndLoadModule(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    SourceText = "\"use strict\";\nexports.value = 123;\n"
                });

            Assert.Equal(string.Empty, module.Assembly.Location);
            Assert.Empty(Directory.EnumerateFileSystemEntries(tempRoot));
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { }
        }
    }

    [Fact]
    public void CompileAndLoadModule_WhenHostedForkNeedsCompiledAssemblyPath_ThrowsExplicitConfigurationError()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "inmemory-fork.js");

        using var module = JrocInMemoryCompiler.CompileAndLoadModule<IHostedForkExports>(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = """
                    "use strict";
                    const child_process = require("child_process");
                    exports.attemptFork = function () {
                        child_process.fork("./child.js");
                    };
                    """
            });

        var ex = Assert.Throws<JsInvocationException>(() => module.Exports.attemptFork());
        Assert.Equal("inmemory-fork", ex.ModuleId);
        Assert.Equal("attemptFork", ex.MemberName);

        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        Assert.Equal("Error", jsError.JsName);
        Assert.Contains(
            "child_process.fork requires a compiled assembly path when running under JsEngine hosting",
            jsError.JsMessage ?? jsError.Message,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "Pass JsModuleLoadOptions.CompiledAssemblyPath",
            jsError.JsMessage ?? jsError.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    public interface IHostedForkExports : IDisposable
    {
        void attemptFork();
    }
}
