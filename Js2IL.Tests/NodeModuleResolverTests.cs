using System;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

public class NodeModuleResolverTests
{
    [Fact]
    public void Resolve_BarePackage_UsesPackageJsonMain()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var nodeModules = Path.Combine(projectDir, "node_modules");
        var pkgRoot = Path.Combine(nodeModules, "pkg");
        var pkgJson = Path.Combine(pkgRoot, "package.json");
        var entry = Path.Combine(pkgRoot, "lib", "index.js");

        fs.AddFile(pkgJson, "{\"name\":\"pkg\",\"main\":\"lib/index.js\"}");
        fs.AddFile(entry, "\"use strict\"; module.exports = 123;");

        var ok = resolver.TryResolve("pkg", baseDirectory: projectDir, out var resolved, out var error);

        Assert.True(ok, error);
        Assert.Equal(Path.GetFullPath(entry), resolved);
    }

    [Fact]
    public void Resolve_BarePackage_UsesExportsForRootAndSubpath()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var pkgRoot = Path.Combine(projectDir, "node_modules", "pkg");
        var pkgJson = Path.Combine(pkgRoot, "package.json");
        var rootEntry = Path.Combine(pkgRoot, "dist", "index.js");
        var featureEntry = Path.Combine(pkgRoot, "dist", "feature.js");

        fs.AddFile(
            pkgJson,
            "{\"name\":\"pkg\",\"exports\":{\".\":\"./dist/index.js\",\"./feature\":\"./dist/feature.js\"}}"
        );
        fs.AddFile(rootEntry, "\"use strict\"; module.exports = 'root';");
        fs.AddFile(featureEntry, "\"use strict\"; module.exports = 'feature';");

        Assert.True(resolver.TryResolve("pkg", projectDir, out var resolvedRoot, out var errorRoot), errorRoot);
        Assert.Equal(Path.GetFullPath(rootEntry), resolvedRoot);

        Assert.True(resolver.TryResolve("pkg/feature", projectDir, out var resolvedFeature, out var errorFeature), errorFeature);
        Assert.Equal(Path.GetFullPath(featureEntry), resolvedFeature);
    }

    [Fact]
    public void Resolve_ScopedPackage_Main()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var pkgRoot = Path.Combine(projectDir, "node_modules", "@scope", "pkg");
        var pkgJson = Path.Combine(pkgRoot, "package.json");
        var entry = Path.Combine(pkgRoot, "lib", "index.js");

        fs.AddFile(pkgJson, "{\"name\":\"@scope/pkg\",\"main\":\"lib/index.js\"}");
        fs.AddFile(entry, "\"use strict\"; module.exports = 1;");

        Assert.True(resolver.TryResolve("@scope/pkg", projectDir, out var resolved, out var error), error);
        Assert.Equal(Path.GetFullPath(entry), resolved);
    }
}
