using JavaScriptRuntime;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Modules.Shared;

namespace Jroc.Tests.CommonJS;

public sealed class RequireAdapterLazinessTests
{
    [Fact]
    public void ModuleConstruction_DoesNotMaterializeRequireAdapter()
    {
        var requireService = new Require(new LocalModulesAssembly());
        var requireTarget = new RequireFunctionTarget(requireService, "entry");

        _ = new Module(
            "entry",
            "/workspace/entry.js",
            parent: null,
            requireDelegate: requireTarget.Require);

        Assert.False(requireTarget.IsFunctionValueMaterializedForTests);
        Assert.False(
            BuiltinDelegateFunctionAdapter.HasStableAdapterForTests(
                requireTarget.Require));
    }

    [Fact]
    public void FirstRequireValueRead_CreatesOneStableAdapterWithMainMetadata()
    {
        var requireService = new Require(new LocalModulesAssembly());
        var requireTarget = new RequireFunctionTarget(requireService, "entry");
        var module = new Module(
            "entry",
            "/workspace/entry.js",
            parent: null,
            requireDelegate: requireTarget.Require);
        requireTarget.SetMainModule(module);

        var first = RequireRuntime.GetFunctionValue(requireTarget.Require);
        var second = RequireRuntime.GetFunctionValue(requireTarget.Require);

        Assert.True(requireTarget.IsFunctionValueMaterializedForTests);
        Assert.True(
            BuiltinDelegateFunctionAdapter.HasStableAdapterForTests(
                requireTarget.Require));
        Assert.Same(first, second);
        Assert.Same(first, module.require);
        Assert.IsType<BuiltinDelegateFunctionAdapter>(module.require);
        Assert.False(module.require is Delegate);
        Assert.Equal("require", ObjectRuntime.GetProperty(first, "name"));
        Assert.Equal(1d, ObjectRuntime.GetProperty(first, "length"));
        Assert.Same(module, ObjectRuntime.GetProperty(first, "main"));
    }

    [Fact]
    public void CompiledModuleWithoutRequireValueRead_HasNoMaterializationCall()
    {
        var il = CompileToIl(
            """
            "use strict";

            const arrow0 = () => 0;
            function increment(value) {
                return value + 1;
            }

            function run(iterations) {
                let result = 0;
                for (let index = 0; index < iterations; index++) {
                    result = increment(result);
                }

                return result + arrow0();
            }

            module.exports = { run };
            """,
            "require-adapter-lazy-module.js");

        Assert.DoesNotContain(
            "RequireRuntime::GetFunctionValue",
            il,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "BuiltinDelegateFunctionAdapter::FromDelegate",
            il,
            StringComparison.Ordinal);
    }

    [Fact]
    public void CompiledRequireValueRead_MaterializesThroughRequireRuntime()
    {
        var il = CompileToIl(
            """
            "use strict";
            module.exports = require;
            """,
            "require-adapter-value-read.js");

        Assert.Contains(
            "RequireRuntime::GetFunctionValue",
            il,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "BuiltinDelegateFunctionAdapter::FromDelegate",
            il,
            StringComparison.Ordinal);
    }

    private static string CompileToIl(string source, string fileName)
    {
        var artifact = JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest(
                Path.Combine(Environment.CurrentDirectory, fileName))
            {
                SourceText = source
            });
        return Utilities.AssemblyToText.ConvertToText(
            artifact.PeBytes,
            artifact.AssemblyName);
    }
}
