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

    [Fact]
    public void Resolve_BarePackage_UsesImportAndRequireConditions()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var pkgRoot = Path.Combine(projectDir, "node_modules", "pkg");
        var pkgJson = Path.Combine(pkgRoot, "package.json");
        var importEntry = Path.Combine(pkgRoot, "esm", "index.js");
        var requireEntry = Path.Combine(pkgRoot, "cjs", "index.cjs");
        var importFeatureEntry = Path.Combine(pkgRoot, "esm", "feature.js");
        var requireFeatureEntry = Path.Combine(pkgRoot, "cjs", "feature.cjs");

        fs.AddFile(
            pkgJson,
            "{"
            + "\"name\":\"pkg\","
            + "\"type\":\"module\","
            + "\"exports\":{"
            + "\".\":{\"import\":\"./esm/index.js\",\"require\":\"./cjs/index.cjs\"},"
            + "\"./feature\":{\"import\":\"./esm/feature.js\",\"require\":\"./cjs/feature.cjs\"}"
            + "}"
            + "}"
        );
        fs.AddFile(importEntry, "export default 'esm';");
        fs.AddFile(requireEntry, "\"use strict\"; module.exports = 'cjs';");
        fs.AddFile(importFeatureEntry, "export default 'esm-feature';");
        fs.AddFile(requireFeatureEntry, "\"use strict\"; module.exports = 'cjs-feature';");

        Assert.True(resolver.TryResolve("pkg", projectDir, ModuleResolutionMode.Import, out var importResolved, out var importError), importError);
        Assert.Equal(Path.GetFullPath(importEntry), importResolved);

        Assert.True(resolver.TryResolve("pkg", projectDir, ModuleResolutionMode.Require, out var requireResolved, out var requireError), requireError);
        Assert.Equal(Path.GetFullPath(requireEntry), requireResolved);

        Assert.True(resolver.TryResolve("pkg/feature", projectDir, ModuleResolutionMode.Import, out var importFeatureResolved, out var importFeatureError), importFeatureError);
        Assert.Equal(Path.GetFullPath(importFeatureEntry), importFeatureResolved);

        Assert.True(resolver.TryResolve("pkg/feature", projectDir, ModuleResolutionMode.Require, out var requireFeatureResolved, out var requireFeatureError), requireFeatureError);
        Assert.Equal(Path.GetFullPath(requireFeatureEntry), requireFeatureResolved);
    }

    [Fact]
    public void Resolve_PackageImports_UsesNearestPackageJsonAndConditions()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var packageRoot = Path.Combine(projectDir, "packages", "app");
        var packageJson = Path.Combine(packageRoot, "package.json");
        var importShared = Path.Combine(packageRoot, "esm", "shared.js");
        var requireShared = Path.Combine(packageRoot, "cjs", "shared.cjs");
        var importFeature = Path.Combine(packageRoot, "esm", "tool.js");
        var requireFeature = Path.Combine(packageRoot, "cjs", "tool.cjs");
        var baseDirectory = Path.Combine(packageRoot, "src", "nested");

        fs.AddFile(
            packageJson,
            "{"
            + "\"name\":\"app\","
            + "\"imports\":{"
            + "\"#shared\":{\"import\":\"./esm/shared.js\",\"require\":\"./cjs/shared.cjs\"},"
            + "\"#feature/*\":{\"import\":\"./esm/*.js\",\"require\":\"./cjs/*.cjs\"}"
            + "}"
            + "}"
        );
        fs.AddFile(importShared, "export default 'esm-shared';");
        fs.AddFile(requireShared, "\"use strict\"; module.exports = 'cjs-shared';");
        fs.AddFile(importFeature, "export default 'esm-tool';");
        fs.AddFile(requireFeature, "\"use strict\"; module.exports = 'cjs-tool';");

        Assert.True(resolver.TryResolve("#shared", baseDirectory, ModuleResolutionMode.Import, out var importResolved, out var importError), importError);
        Assert.Equal(Path.GetFullPath(importShared), importResolved);

        Assert.True(resolver.TryResolve("#shared", baseDirectory, ModuleResolutionMode.Require, out var requireResolved, out var requireError), requireError);
        Assert.Equal(Path.GetFullPath(requireShared), requireResolved);

        Assert.True(resolver.TryResolve("#feature/tool", baseDirectory, ModuleResolutionMode.Import, out var importFeatureResolved, out var importFeatureError), importFeatureError);
        Assert.Equal(Path.GetFullPath(importFeature), importFeatureResolved);

        Assert.True(resolver.TryResolve("#feature/tool", baseDirectory, ModuleResolutionMode.Require, out var requireFeatureResolved, out var requireFeatureError), requireFeatureError);
        Assert.Equal(Path.GetFullPath(requireFeature), requireFeatureResolved);
    }

    [Fact]
    public void Resolve_PackageImports_ReportsUnsupportedConditions()
    {
        var fs = new MockFileSystem();
        var resolver = new NodeModuleResolver(fs);

        var projectDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "js2il-tests", Guid.NewGuid().ToString("N"), "proj"));
        var packageRoot = Path.Combine(projectDir, "packages", "app");
        var packageJson = Path.Combine(packageRoot, "package.json");
        var baseDirectory = Path.Combine(packageRoot, "src");

        fs.AddFile(
            packageJson,
            "{"
            + "\"name\":\"app\","
            + "\"imports\":{"
            + "\"#shared\":{\"browser\":\"./browser.js\"}"
            + "}"
            + "}"
        );

        var ok = resolver.TryResolve("#shared", baseDirectory, ModuleResolutionMode.Import, out _, out var error);

        Assert.False(ok);
        Assert.Contains("Unsupported package.json imports conditions", error);
        Assert.Contains("Supported conditions: import, require, node, default", error);
    }
}
