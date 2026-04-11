namespace Js2IL.Tests.CommonJS;

public class NodeModulesGeneratorTests
{
    [Fact]
    public void CommonJS_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry_EmitsManifest()
    {
        using var project = NodeModulesTestProjectSupport.CreateDualModeExportsImportsProject();
        var compiled = NodeModulesTestProjectSupport.Compile(project);

        Assert.True(compiled.Success, compiled.Logger.Errors);
        Assert.False(string.IsNullOrWhiteSpace(compiled.AssemblyPath));

        var moduleIds = NodeModulesTestProjectSupport.ReadCompiledModuleIdsFromManifest(compiled.AssemblyPath!);

        Assert.Contains("pkg/esm/index", moduleIds);
        Assert.Contains("pkg/esm/feature", moduleIds);
        Assert.Contains("pkg/esm/shared", moduleIds);
        Assert.Contains("pkg/cjs/index", moduleIds);
        Assert.Contains("pkg/cjs/feature", moduleIds);
        Assert.Contains("pkg/cjs/shared", moduleIds);
    }

    [Fact]
    public void CommonJS_NodeModules_PackageImports_BarePackageAlias_EmitsManifest()
    {
        using var project = NodeModulesTestProjectSupport.CreateBarePackageImportsAliasProject();
        var compiled = NodeModulesTestProjectSupport.Compile(project);

        Assert.True(compiled.Success, compiled.Logger.Errors);
        Assert.False(string.IsNullOrWhiteSpace(compiled.AssemblyPath));

        var moduleIds = NodeModulesTestProjectSupport.ReadCompiledModuleIdsFromManifest(compiled.AssemblyPath!);

        Assert.Contains("dep/esm/index", moduleIds);
        Assert.Contains("dep/esm/feature", moduleIds);
        Assert.Contains("dep/cjs/index", moduleIds);
        Assert.Contains("dep/cjs/feature", moduleIds);
    }
}
