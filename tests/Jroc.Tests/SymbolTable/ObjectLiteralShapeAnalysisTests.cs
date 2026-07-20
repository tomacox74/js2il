using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Services;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Jroc.Tests;

/// <summary>
/// Tests for the object literal shape eligibility analysis (issue #1429).
/// The analysis must be strictly conservative: only provably-safe literals qualify.
/// </summary>
public class ObjectLiteralShapeAnalysisTests
{
    private readonly JavaScriptParser _parser = new();
    private readonly SymbolTableBuilder _scopeBuilder = new();

    private SymbolTable BuildSymbolTable(string code)
    {
        var ast = _parser.ParseJavaScript(code, "test.js");
        var module = new ModuleDefinition
        {
            Ast = ast,
            Path = "test.js",
            Name = "test",
            ModuleId = "test"
        };
        _scopeBuilder.Build(module);
        return module.SymbolTable!;
    }

    private ObjectLiteralShapeInfo GetShape(string code, string bindingName)
    {
        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo(bindingName);
        Assert.NotNull(binding);
        Assert.NotNull(binding!.ObjectLiteralShape);
        return binding.ObjectLiteralShape!;
    }

    // ---------------------------------------------------------------------
    // Eligible cases
    // ---------------------------------------------------------------------

    [Fact]
    public void Eligible_ConstLiteral_StaticReadsOnly()
    {
        var shape = GetShape(
            @"const a = { b: 'hello', n: 42, f: true };
              console.log(a.b);
              console.log(a.n);
              console.log(a.f);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Equal(new[] { "b", "n", "f" }, shape.Members.Select(m => m.Name));
        Assert.Equal(typeof(string), shape.Members[0].ClrType);
        Assert.Equal(typeof(double), shape.Members[1].ClrType);
        Assert.Equal(typeof(bool), shape.Members[2].ClrType);
    }

    [Fact]
    public void Eligible_StaticMemberWrites_KeepStableType()
    {
        var shape = GetShape(
            @"const a = { n: 1 };
              a.n = 2;
              a.n = 3;
              console.log(a.n);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Equal(typeof(double), shape.Members[0].ClrType);
    }

    [Fact]
    public void Eligible_MixedTypeWrite_DemotesMemberToObject()
    {
        var shape = GetShape(
            @"const a = { n: 1 };
              a.n = 'now a string';
              console.log(a.n);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Null(shape.Members[0].ClrType);
    }

    [Fact]
    public void Eligible_UpdateExpression_KeepsNumericType()
    {
        var shape = GetShape(
            @"const a = { n: 1 };
              a.n++;
              console.log(a.n);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Equal(typeof(double), shape.Members[0].ClrType);
    }

    [Fact]
    public void Eligible_VarBinding_NeverReassigned()
    {
        var shape = GetShape(
            @"var a = { b: 1 };
              console.log(a.b);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
    }

    [Fact]
    public void Eligible_FunctionMember_CalledWithoutThis()
    {
        var shape = GetShape(
            @"const a = { go: function (x) { return x + 1; } };
              console.log(a.go(1));",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.True(shape.Members[0].IsFunction);
    }

    [Fact]
    public void Eligible_ReadInsideClosure()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              function reader() { return a.b; }
              console.log(reader());",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
    }

    [Fact]
    public void Eligible_TypeofUse()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              console.log(typeof a);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
    }

    [Fact]
    public void Eligible_ShadowedNameDoesNotDisqualify()
    {
        // The inner `a` is a different binding; passing it to a call must not
        // disqualify the outer literal.
        var shape = GetShape(
            @"const a = { b: 1 };
              function inner(a) { console.log(a); }
              inner(42);
              console.log(a.b);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
    }

    // ---------------------------------------------------------------------
    // Disqualified: unsafe uses of the binding
    // ---------------------------------------------------------------------

    [Theory]
    [InlineData("callUnknown(a);", "object passed to a call")]
    [InlineData("new Holder(a);", "object passed to a call")]
    [InlineData("function f() { return a; } f();", "object returned from a function")]
    [InlineData("const other = { nested: a };", "object stored into another object or array")]
    [InlineData("const arr = [a];", "object stored into another object or array")]
    [InlineData("let alias = a;", "object aliased to another binding")]
    [InlineData("target.prop = a;", "object stored through an assignment")]
    [InlineData("const copy = { ...a };", "object used in a spread")]
    [InlineData("for (const k in a) { console.log(k); }", "object enumerated by for-in/for-of")]
    [InlineData("console.log('b' in a);", "object used with the 'in' operator")]
    public void Disqualified_UnsafeBindingUse(string usage, string expectedReasonFragment)
    {
        var code = $@"const a = {{ b: 1 }};
              function callUnknown(x) {{ console.log(x); }}
              function Holder(x) {{ this.x = x; }}
              const target = {{ prop: 0 }};
              {usage}
              console.log(a.b);";

        var shape = GetShape(code, "a");
        Assert.False(shape.IsEligible);
        Assert.Contains(expectedReasonFragment, shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_BindingReassigned()
    {
        var shape = GetShape(
            @"let a = { b: 1 };
              a = { b: 2 };
              console.log(a.b);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("reassigned", shape.DisqualifyReason);
    }

    // ---------------------------------------------------------------------
    // Disqualified: unsafe member operations
    // ---------------------------------------------------------------------

    [Fact]
    public void Disqualified_DeleteMember()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              delete a.b;
              console.log(a.b);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("delete", shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_ComputedMemberAccess()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              const k = 'b';
              console.log(a[k]);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("computed member access", shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_UndeclaredMemberAccess()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              a.c = 2;
              console.log(a.b);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("undeclared member 'c'", shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_ObjectFreeze()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              Object.freeze(a);
              console.log(a.b);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("object passed to a call", shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_ObjectDefineProperty()
    {
        var shape = GetShape(
            @"const a = { b: 1 };
              Object.defineProperty(a, 'b', { value: 2 });
              console.log(a.b);",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("object passed to a call", shape.DisqualifyReason);
    }

    [Fact]
    public void Disqualified_MethodCallWhoseBodyUsesThis()
    {
        var shape = GetShape(
            @"const a = { n: 1, go: function () { return this.n; } };
              console.log(a.go());",
            "a");

        Assert.False(shape.IsEligible);
        Assert.Contains("uses 'this'", shape.DisqualifyReason);
    }

    // ---------------------------------------------------------------------
    // Disqualified: unsafe literal structure
    // ---------------------------------------------------------------------

    [Theory]
    [InlineData("{}", "no members")]
    [InlineData("{ ...src }", "spread element")]
    [InlineData("{ get b() { return 1; } }", "getter/setter")]
    [InlineData("{ set b(v) { } }", "getter/setter")]
    [InlineData("{ ['b']: 1 }", "computed key")]
    [InlineData("{ 'b': 1 }", "non-identifier key")]
    [InlineData("{ 1: 'one' }", "non-identifier key")]
    [InlineData("{ __proto__: null }", "__proto__")]
    [InlineData("{ b: 1, b: 2 }", "duplicate key 'b'")]
    public void Disqualified_UnsafeLiteralStructure(string literal, string expectedReasonFragment)
    {
        var code = $@"const src = {{ x: 1 }};
              const a = {literal};
              console.log(a);";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo("a");
        Assert.NotNull(binding);
        var shape = binding!.ObjectLiteralShape;
        Assert.NotNull(shape);
        Assert.False(shape!.IsEligible);
        Assert.Contains(expectedReasonFragment, shape.DisqualifyReason);
    }

    // ---------------------------------------------------------------------
    // Interprocedural propagation (issue #1434, phase 6)
    // ---------------------------------------------------------------------

    private static System.Collections.Generic.IEnumerable<Scope> EnumerateScopes(Scope scope)
    {
        yield return scope;
        foreach (var child in scope.Children)
        {
            foreach (var descendant in EnumerateScopes(child))
            {
                yield return descendant;
            }
        }
    }

    private ObjectLiteralShapeInfo? GetParameterShape(SymbolTable symbolTable, string functionName, string parameterName)
    {
        var functionScope = EnumerateScopes(symbolTable.Root)
            .First(s => s.Kind == ScopeKind.Function && string.Equals(s.Name, functionName, StringComparison.Ordinal));
        return functionScope.Bindings[parameterName].ObjectLiteralShape;
    }

    private BindingInfo GetParameterBinding(SymbolTable symbolTable, string functionName, string parameterName)
    {
        var functionScopes = EnumerateScopes(symbolTable.Root)
            .Where(s => s.Kind == ScopeKind.Function && s.Bindings.ContainsKey(parameterName))
            .ToArray();
        var functionScope = functionScopes.FirstOrDefault(
                s => string.Equals(s.Name, functionName, StringComparison.Ordinal))
            ?? Assert.Single(functionScopes);
        return functionScope.Bindings[parameterName];
    }

    [Fact]
    public void Interprocedural_ObjectDestructuring_KeepsLiteralEligibleAndInfersMemberTypes()
    {
        var symbolTable = BuildSymbolTable(
            @"let config = {
                  sieveSize: 1000000,
                  timeLimitSeconds: 5,
                  verbose: false,
                  runtime: ''
              };
              config.runtime = process.argv[0];
              config.verbose = process.argv.includes('verbose');
              const main = ({ sieveSize, timeLimitSeconds, verbose, runtime }) => {
                  consume(sieveSize, timeLimitSeconds);
                  console.log(sieveSize, timeLimitSeconds, verbose, runtime);
              };
              function consume(size, seconds) { console.log(size + seconds); }
              main(config);");

        var shape = symbolTable.GetBindingInfo("config")!.ObjectLiteralShape;
        Assert.NotNull(shape);
        Assert.True(shape!.IsEligible, shape.DisqualifyReason);

        var sieveSize = GetParameterBinding(symbolTable, "main", "sieveSize");
        var timeLimitSeconds = GetParameterBinding(symbolTable, "main", "timeLimitSeconds");
        Assert.True(sieveSize.IsStableType);
        Assert.Equal(typeof(double), sieveSize.ClrType);
        Assert.True(timeLimitSeconds.IsStableType);
        Assert.Equal(typeof(double), timeLimitSeconds.ClrType);

        Assert.False(GetParameterBinding(symbolTable, "main", "verbose").IsStableType);
        Assert.False(GetParameterBinding(symbolTable, "main", "runtime").IsStableType);

        Assert.Equal(typeof(double), GetParameterBinding(symbolTable, "consume", "size").ClrType);
        Assert.Equal(typeof(double), GetParameterBinding(symbolTable, "consume", "seconds").ClrType);
    }

    [Fact]
    public void Interprocedural_ObjectDestructuring_ConflictingCallSiteKeepsBindingGeneric()
    {
        var symbolTable = BuildSymbolTable(
            @"const numeric = { value: 1 };
              const textual = { value: 'one' };
              function read({ value }) { console.log(value); }
              read(numeric);
              read(textual);");

        Assert.True(symbolTable.GetBindingInfo("numeric")!.ObjectLiteralShape!.IsEligible);
        Assert.True(symbolTable.GetBindingInfo("textual")!.ObjectLiteralShape!.IsEligible);
        Assert.False(GetParameterBinding(symbolTable, "read", "value").IsStableType);
    }

    [Fact]
    public void Interprocedural_DirectPropagation_TypesParameter()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              function c(o) { console.log(o.b); }
              c(a);");

        Assert.True(symbolTable.GetBindingInfo("a")!.ObjectLiteralShape!.IsEligible);

        var paramShape = GetParameterShape(symbolTable, "c", "o");
        Assert.NotNull(paramShape);
        Assert.True(paramShape!.IsEligible);
        // The parameter shares the literal's shape so codegen uses one generated CLR type.
        Assert.Same(symbolTable.GetBindingInfo("a")!.ObjectLiteralShape, paramShape);
    }

    [Fact]
    public void Interprocedural_IncompatibleCallSites_ParameterStaysGeneric()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              const d = { z: 1 };
              function c(o) { console.log(o.b); }
              c(a);
              c(d);");

        // Both literals only flow into a safe callee use, so they remain eligible for their own
        // early binding, but the parameter cannot join two different shapes.
        Assert.True(symbolTable.GetBindingInfo("a")!.ObjectLiteralShape!.IsEligible);
        Assert.True(symbolTable.GetBindingInfo("d")!.ObjectLiteralShape!.IsEligible);
        Assert.Null(GetParameterShape(symbolTable, "c", "o"));
    }

    [Fact]
    public void Interprocedural_SpreadArgument_ParameterStaysGeneric()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              function c(o) { console.log(o.b); }
              function forward(rest) { c(...rest); }
              c(a);
              forward([a]);");

        // The spread call site cannot supply positional evidence, so the parameter is generic.
        Assert.Null(GetParameterShape(symbolTable, "c", "o"));
    }

    [Fact]
    public void Interprocedural_SpreadShiftsPositionalLiteral_FallsBackSafely()
    {
        // A spread before a positional literal argument shifts `a`'s parameter slot by an unknown
        // amount: `a` cannot be assumed to map to parameter index 1. The literal obligation must
        // not be accepted as stable, so the literal falls back and the parameter stays generic.
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              const rest = [1, 2];
              function c(x, o) { console.log(o.b); }
              c(...rest, a);");

        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.NotNull(literalShape);
        Assert.False(literalShape!.IsEligible);
        Assert.Null(GetParameterShape(symbolTable, "c", "o"));
        Assert.Null(GetParameterShape(symbolTable, "c", "x"));
    }

    [Fact]
    public void Interprocedural_SpreadShiftsForwardedParameter_FallsBackSafely()
    {
        // A parameter forwarded after a spread cannot be proved to line up with a callee slot,
        // so the forwarded evidence must not be accepted and both slots stay generic.
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              function d(o) { console.log(o.b); }
              function c(rest, o) { d(...rest, o); }
              c([1], a);");

        Assert.Null(GetParameterShape(symbolTable, "d", "o"));
    }

    [Fact]
    public void Interprocedural_SafeParameterMemberWrite_TypesParameter()
    {
        // Same-type member writes/updates through the parameter are safe: they lower through the
        // generated setter, so the parameter is early-bound to the literal's shape (issue #1434
        // safe-use parity).
        var symbolTable = BuildSymbolTable(
            @"const a = { n: 1 };
              function bump(o) { o.n = 5; o.n++; console.log(o.n); }
              bump(a);");

        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.NotNull(literalShape);
        Assert.True(literalShape!.IsEligible, literalShape.DisqualifyReason);

        var paramShape = GetParameterShape(symbolTable, "bump", "o");
        Assert.NotNull(paramShape);
        Assert.True(paramShape!.IsEligible);
        Assert.Same(literalShape, paramShape);
    }

    [Fact]
    public void Interprocedural_ConflictingParameterMemberWrite_FallsBackSafely()
    {
        // Writing a string into a member the literal declares as a number cannot be lowered through
        // the numeric setter, so the write demotes the slot and disqualifies the feeding literal.
        var symbolTable = BuildSymbolTable(
            @"const a = { n: 1 };
              function clobber(o) { o.n = 'text'; console.log(o.n); }
              clobber(a);");

        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.NotNull(literalShape);
        Assert.False(literalShape!.IsEligible);
        Assert.Null(GetParameterShape(symbolTable, "clobber", "o"));
    }

    [Fact]
    public void Interprocedural_ParameterMethodCall_FallsBackSafely()
    {
        // Interprocedural method-call parity is conservatively out of scope: calling a member
        // through the parameter passes the object as `this` into a callee this pass does not model,
        // so it is treated as an escaping use.
        var symbolTable = BuildSymbolTable(
            @"const a = { greet: function () { return 'hi'; } };
              function run(o) { console.log(o.greet()); }
              run(a);");

        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.NotNull(literalShape);
        Assert.False(literalShape!.IsEligible);
        Assert.Null(GetParameterShape(symbolTable, "run", "o"));
    }

    [Fact]
    public void Interprocedural_UnsafeCalleeUse_DisqualifiesLiteralAndParameter()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              function c(o) { return o; }
              c(a);");

        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.NotNull(literalShape);
        Assert.False(literalShape!.IsEligible);
        Assert.Contains("unsafe", literalShape.DisqualifyReason);
        Assert.Null(GetParameterShape(symbolTable, "c", "o"));
    }

    [Fact]
    public void Interprocedural_MultiHopChain_PropagatesBeyondFourHops()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'deep' };
              function h6(o) { console.log(o.b); }
              function h5(o) { h6(o); }
              function h4(o) { h5(o); }
              function h3(o) { h4(o); }
              function h2(o) { h3(o); }
              function h1(o) { h2(o); }
              h1(a);");

        Assert.True(symbolTable.GetBindingInfo("a")!.ObjectLiteralShape!.IsEligible);
        foreach (var name in new[] { "h1", "h2", "h3", "h4", "h5", "h6" })
        {
            var paramShape = GetParameterShape(symbolTable, name, "o");
            Assert.True(paramShape is { IsEligible: true }, $"{name}.o should carry the inferred shape");
        }
    }

    [Fact]
    public void Interprocedural_MutualRecursion_ResolvesDeterministically()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hi' };
              function f(o) { if (o.b) { g(o); } console.log(o.b); }
              function g(o) { if (o.b) { f(o); } console.log(o.b); }
              f(a);");

        Assert.True(symbolTable.GetBindingInfo("a")!.ObjectLiteralShape!.IsEligible);
        Assert.True(GetParameterShape(symbolTable, "f", "o") is { IsEligible: true });
        Assert.True(GetParameterShape(symbolTable, "g", "o") is { IsEligible: true });
    }

    [Fact]
    public void Interprocedural_StructurallyIdenticalLiterals_ShareShapeAtParameter()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { x: 1 };
              const b = { x: 2 };
              function f(o) { return o.x; }
              f(a);
              f(b);");

        var shapeA = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        var shapeB = symbolTable.GetBindingInfo("b")!.ObjectLiteralShape;
        Assert.True(shapeA!.IsEligible);
        Assert.True(shapeB!.IsEligible);
        Assert.Equal(shapeA.GetStructuralSignatureKey(), shapeB.GetStructuralSignatureKey());

        var paramShape = GetParameterShape(symbolTable, "f", "o");
        Assert.NotNull(paramShape);
        Assert.True(paramShape!.IsEligible);
        // The join canonicalizes onto one representative shape (the first, deterministically).
        Assert.Same(shapeA, paramShape);
    }

    [Fact]
    public void Interprocedural_ReorderedSameShapeLiterals_ShareShapeAtParameter()
    {
        // Members declared in a different source order must produce the same structural signature
        // (canonicalized by member name) so the two literals share one generated CLR type and can
        // join at the parameter (issue #1434 phase 6 canonicalization).
        var symbolTable = BuildSymbolTable(
            @"const a = { x: 1, y: 'left' };
              const b = { y: 'right', x: 2 };
              function f(o) { return o.x + o.y; }
              f(a);
              f(b);");

        var shapeA = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        var shapeB = symbolTable.GetBindingInfo("b")!.ObjectLiteralShape;
        Assert.True(shapeA!.IsEligible);
        Assert.True(shapeB!.IsEligible);

        // Same names/types in a different order canonicalize to one signature.
        Assert.Equal(shapeA.GetStructuralSignatureKey(), shapeB.GetStructuralSignatureKey());
        // Each literal keeps its own construction/enumeration source order.
        Assert.Equal(new[] { "x", "y" }, shapeA.Members.Select(m => m.Name));
        Assert.Equal(new[] { "y", "x" }, shapeB.Members.Select(m => m.Name));

        var paramShape = GetParameterShape(symbolTable, "f", "o");
        Assert.NotNull(paramShape);
        Assert.True(paramShape!.IsEligible);
        Assert.Same(shapeA, paramShape);
    }

    [Fact]
    public void StructuralSignatureKey_IsOrderInsensitiveByMemberName()
    {
        // Direct unit check on the canonicalization: reordered members share a key, but a differing
        // member type or a different member set does not.
        var symbolTable = BuildSymbolTable(
            @"const a = { x: 1, y: 'left' };
              const b = { y: 'right', x: 2 };
              const c = { x: 'one', y: 'two' };
              console.log(a.x, a.y, b.x, b.y, c.x, c.y);");

        var keyA = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape!.GetStructuralSignatureKey();
        var keyB = symbolTable.GetBindingInfo("b")!.ObjectLiteralShape!.GetStructuralSignatureKey();
        var keyC = symbolTable.GetBindingInfo("c")!.ObjectLiteralShape!.GetStructuralSignatureKey();

        Assert.Equal(keyA, keyB);
        Assert.NotEqual(keyA, keyC);
    }

    [Fact]
    public void Interprocedural_ConflictingMemberTypes_ParameterStaysGeneric()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { x: 1 };
              const b = { x: 'text' };
              function f(o) { return o.x; }
              f(a);
              f(b);");

        // Same member name but different member CLR types => distinct signatures => no join.
        Assert.Null(GetParameterShape(symbolTable, "f", "o"));
    }

    [Fact]
    public void Interprocedural_EscapingCallable_ParameterStaysGeneric()
    {
        var symbolTable = BuildSymbolTable(
            @"const a = { b: 'hello' };
              function c(o) { console.log(o.b); }
              const alias = c;
              c(a);");

        // `c` is aliased to another binding, so it is not a closed-world callable and passing the
        // literal into it must fall back to the generic path.
        var literalShape = symbolTable.GetBindingInfo("a")!.ObjectLiteralShape;
        Assert.False(literalShape!.IsEligible);
        Assert.Null(GetParameterShape(symbolTable, "c", "o"));
    }

    // ---------------------------------------------------------------------
    // Member metadata
    // ---------------------------------------------------------------------

    [Fact]
    public void FunctionMember_DemotedToDataOnWrite()
    {
        var shape = GetShape(
            @"const a = { go: function () { return 1; } };
              a.go = 5;
              console.log(a.go);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.False(shape.Members[0].IsFunction);
        Assert.Null(shape.Members[0].ClrType);
    }

    [Fact]
    public void NegativeNumericLiteral_InfersDouble()
    {
        var shape = GetShape(
            @"const a = { n: -5 };
              console.log(a.n);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Equal(typeof(double), shape.Members[0].ClrType);
    }

    [Fact]
    public void NonLiteralInitializer_MemberIsObjectTyped()
    {
        var shape = GetShape(
            @"const seed = 10;
              const a = { n: seed };
              console.log(a.n);",
            "a");

        Assert.True(shape.IsEligible, shape.DisqualifyReason);
        Assert.Null(shape.Members[0].ClrType);
    }

    [Fact]
    public void NonLiteralBinding_HasNoShape()
    {
        var symbolTable = BuildSymbolTable("const a = 42; console.log(a);");
        var binding = symbolTable.GetBindingInfo("a");
        Assert.NotNull(binding);
        Assert.Null(binding!.ObjectLiteralShape);
    }
}
