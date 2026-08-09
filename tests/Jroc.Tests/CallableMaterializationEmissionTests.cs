using System.Text.RegularExpressions;

namespace Jroc.Tests;

public sealed class CallableMaterializationEmissionTests
{
    [Fact]
    public void DirectOnlyArrowKeepsBodyButEliminatesObjectInitializationAndLocal()
    {
        const string source = """
            const direct = value => value + 1;
            console.log(direct(2));
            """;
        var il = CompileToIl(source, "direct-only-materialization.js");
        var owner = GetFirstArrowOwner(il);
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains(
            ".class nested public auto ansi sealed beforefieldinit FunctionObject",
            il,
            StringComparison.Ordinal);
        Assert.Contains($"/{owner}::__js_call__", moduleInit, StringComparison.Ordinal);
        Assert.DoesNotContain(
            $"/{owner}/FunctionObject::.ctor",
            moduleInit,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            $"/{owner}/FunctionObject",
            moduleInit,
            StringComparison.Ordinal);
        Assert.DoesNotContain("SetFunctionInferredName", moduleInit, StringComparison.Ordinal);
    }

    [Fact]
    public void EscapingArrowMaterializesOnceWhileKnownCallRemainsDirect()
    {
        const string source = """
            let shared = 0;
            const escaped = value => shared += value;
            const alias = escaped;
            console.log(alias === escaped, alias(1), escaped(2), shared);
            """;
        var il = CompileToIl(source, "escaping-materialization.js");
        var owner = GetFirstArrowOwner(il);
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains(
            $"/{owner}/FunctionObject::.ctor",
            moduleInit,
            StringComparison.Ordinal);
        Assert.Contains($"/{owner}::__js_call__", moduleInit, StringComparison.Ordinal);
        Assert.Contains("InvokeFunctionCallWithArgs", moduleInit, StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveWithCallMaterializesAndRetainsDynamicBindingProbe()
    {
        const string source = """
            const target = () => "lexical";
            with ({ target: () => "with" }) {
                console.log(target());
            }
            """;
        var il = CompileToIl(source, "with-call-materialization.js");
        var owner = GetFirstArrowOwner(il);
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains(
            $"/{owner}/FunctionObject::.ctor",
            moduleInit,
            StringComparison.Ordinal);
        Assert.Contains("ObjectRuntime::HasPropertyIn", moduleInit, StringComparison.Ordinal);
        Assert.Contains("InvokeFunctionCallWithArgs", moduleInit, StringComparison.Ordinal);
    }

    [Fact]
    public void DirectOnlyArrowCalledFromNestedBlockEliminatesMaterialization()
    {
        const string source = """
            const target = () => 1;
            {
                console.log(target());
            }
            """;
        var il = CompileToIl(source, "nested-block-direct-only.js");
        var owner = GetFirstArrowOwner(il);
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains($"/{owner}::__js_call__", moduleInit, StringComparison.Ordinal);
        Assert.DoesNotContain(
            $"/{owner}/FunctionObject::.ctor",
            moduleInit,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ArrowReadFromNestedCallableRemainsMaterialized()
    {
        const string source = """
            const target = () => 1;
            const invoke = () => target();
            console.log(invoke());
            """;
        var il = CompileToIl(source, "nested-callable-materialization.js");
        var owner = GetFirstArrowOwner(il);
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains(
            $"/{owner}/FunctionObject::.ctor",
            moduleInit,
            StringComparison.Ordinal);
    }

    [Fact]
    public void LoopHeadAndBodyCallableUsesDoNotViolateDirectOnlyInvariant()
    {
        const string source = """
            for (const target = () => false; target();) {
                target();
            }
            """;

        _ = CompileToIl(source, "loop-callable-materialization.js");
    }

    [Fact]
    public void PrimeMainArrowHasNoMaterializationOrDeadFunctionObjectLocal()
    {
        var assembly = typeof(CallableMaterializationEmissionTests).Assembly;
        using var stream = assembly.GetManifestResourceStream(
            "Jroc.Tests.Integration.JavaScript.Compile_Performance_PrimeJavaScript.js");
        Assert.NotNull(stream);
        using var reader = new StreamReader(stream);
        var il = CompileToIl(reader.ReadToEnd(), "prime-materialization-guardrail.js");
        var moduleInit = ExtractModuleInitializer(il);

        Assert.Contains(
            "/ArrowFunction_L229C14/FunctionObject",
            il,
            StringComparison.Ordinal);
        Assert.Contains(
            "/ArrowFunction_L229C14::__js_call__",
            moduleInit,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "/ArrowFunction_L229C14/FunctionObject",
            moduleInit,
            StringComparison.Ordinal);
        Assert.DoesNotContain("ldstr \"main\"", moduleInit, StringComparison.Ordinal);
    }

    private static string CompileToIl(string source, string fileName)
    {
        var entryPath = Path.Combine(Environment.CurrentDirectory, fileName);
        var artifact = JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = source
            });
        return Utilities.AssemblyToText.ConvertToText(
            artifact.PeBytes,
            artifact.AssemblyName);
    }

    private static string GetFirstArrowOwner(string il)
    {
        var match = Regex.Match(
            il,
            @"\.class nested public auto ansi abstract sealed beforefieldinit (?<owner>ArrowFunction_L\d+C\d+)");
        Assert.True(match.Success);
        return match.Groups["owner"].Value;
    }

    private static string ExtractModuleInitializer(string il)
    {
        var methodName = il.IndexOf("void __js_module_init__", StringComparison.Ordinal);
        Assert.True(methodName >= 0);
        var methodStart = il.LastIndexOf(".method", methodName, StringComparison.Ordinal);
        var methodEnd = il.IndexOf(
            "} // end of method",
            methodName,
            StringComparison.Ordinal);
        Assert.True(methodStart >= 0 && methodEnd > methodStart);
        return il[methodStart..methodEnd];
    }
}
