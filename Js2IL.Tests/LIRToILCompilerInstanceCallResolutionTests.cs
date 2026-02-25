using Js2IL.IL;
using System.Collections.Generic;
using Xunit;

namespace Js2IL.Tests;

public class LIRToILCompilerInstanceCallResolutionTests
{
    private sealed class ResolutionFixture
    {
        public void call() { }
        public void call(object arg) { }
        public void call(object[] args) { }
    }

    private sealed class InheritedConflictFixture : List<object>
    {
        public double indexOf(object[]? args) => 42;
    }

    private sealed class PascalCaseOnlyFixture
    {
        public object Log(object[] args) => args.Length;
    }

    private sealed class MixedCaseFixture
    {
        public object log(object[] args) => args.Length;
        public object Log(object[] args) => args.Length;
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_PrefersExactObjectOverload_ForArrayPushArity1()
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(JavaScriptRuntime.Array), "push", argCount: 1);

        Assert.NotNull(chosen);
        var parameters = chosen!.GetParameters();
        Assert.Single(parameters);
        Assert.Equal(typeof(object), parameters[0].ParameterType);
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_UsesObjectArrayFallback_WhenNoExactMatchExists()
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(JavaScriptRuntime.Array), "push", argCount: 2);

        Assert.NotNull(chosen);
        var parameters = chosen!.GetParameters();
        Assert.Single(parameters);
        Assert.Equal(typeof(object[]), parameters[0].ParameterType);
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_UsesDeterministicRanking_ForExactAndVariadicCandidates()
    {
        var arity0 = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(ResolutionFixture), "call", argCount: 0);
        var arity1 = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(ResolutionFixture), "call", argCount: 1);
        var arity2 = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(ResolutionFixture), "call", argCount: 2);

        Assert.Empty(arity0!.GetParameters());
        Assert.Equal(typeof(object), arity1!.GetParameters()[0].ParameterType);
        Assert.Equal(typeof(object[]), arity2!.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_PrefersReceiverDeclaredMethods_OverInheritedNameMatches()
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(InheritedConflictFixture), "indexOf", argCount: 1);

        Assert.NotNull(chosen);
        Assert.Equal(typeof(InheritedConflictFixture), chosen!.DeclaringType);
        Assert.Equal(typeof(object[]), chosen.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_UsesCaseInsensitiveFallback_WhenExactCaseMissing()
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(PascalCaseOnlyFixture), "log", argCount: 4);

        Assert.NotNull(chosen);
        Assert.Equal("Log", chosen!.Name);
        Assert.Equal(typeof(object[]), chosen.GetParameters()[0].ParameterType);
    }

    [Fact]
    public void ResolveTypedInstanceMethodOverload_PrefersExactCase_OverCaseInsensitiveFallback()
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(typeof(MixedCaseFixture), "log", argCount: 4);

        Assert.NotNull(chosen);
        Assert.Equal("log", chosen!.Name);
    }
}
