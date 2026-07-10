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
