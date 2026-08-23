using Acornima.Ast;
using Jroc.Services;
using Jroc.SymbolTables;
using Xunit;

namespace Jroc.Tests;

public sealed class ConstructorShapeAnalysisTests
{
    private static SymbolTable Build(string source)
    {
        var parser = new JavaScriptParser();
        var module = new ModuleDefinition
        {
            Ast = parser.ParseJavaScript(source, "test.js"),
            Path = "test.js",
            Name = "test",
            ModuleId = "test"
        };
        new SymbolTableBuilder().Build(module);
        return module.SymbolTable!;
    }

    [Fact]
    public void GraphNodePrefix_IsEligibleAndPropagatesToNewBinding()
    {
        var symbols = Build(
            """
            function GraphNode(x, y, wall) {
                this.x = x;
                this.y = y;
                this.wall = wall;
                this.pos = { x: x, y: y };
            }
            var node = new GraphNode(1, 2, false);
            console.log(node.pos);
            """);

        var constructor = symbols.GetBindingInfo("GraphNode")!;
        var node = symbols.GetBindingInfo("node")!;
        Assert.NotNull(constructor.ConstructorShape);
        Assert.True(
            constructor.ConstructorShape!.IsEligible,
            constructor.ConstructorShape.DisqualifyReason);
        Assert.Equal(
            new[] { "x", "y", "wall", "pos" },
            constructor.ConstructorShape.Members.Select(
                static member => member.Name));
        Assert.Same(constructor.ConstructorShape, node.ConstructedShape);
    }

    [Theory]
    [InlineData(
        "function C(v) { if (v) this.x = v; }",
        "conditionally")]
    [InlineData(
        "function C(k, v) { this[k] = v; }",
        "computed")]
    [InlineData(
        "function C(v) { this.x = v; delete this.x; }",
        "delete")]
    [InlineData(
        "function C(v) { this.x = v; Object.defineProperty(this, 'x', { value: v }); }",
        "defineProperty")]
    [InlineData(
        "function C(v) { consume(this); this.x = v; }",
        "after the unconditional")]
    [InlineData(
        "function C(v) { this.x = v; var later = () => { this.y = v; }; }",
        "outside the top-level")]
    [InlineData(
        "function C(v) { this.x = v; return {}; }",
        "return value")]
    public void UnsupportedConstructor_IsDisqualified(
        string source,
        string reason)
    {
        var shape = Build(source).GetBindingInfo("C")!.ConstructorShape;
        Assert.NotNull(shape);
        Assert.False(shape!.IsEligible);
        Assert.Contains(
            reason,
            shape.DisqualifyReason,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ParameterPropertyUse_RecordsGuardedShapeCandidate()
    {
        var symbols = Build(
            """
            function GraphNode(pos) {
                this.pos = pos;
            }
            function read(obj) {
                return obj.pos;
            }
            read(new GraphNode("x"));
            """);
        var readScope = Enumerate(symbols.Root)
            .Single(scope => scope.Name == "read");
        var candidate = Assert.Single(
            readScope.Bindings["obj"].ConstructorShapeCandidates);
        Assert.Same(
            symbols.GetBindingInfo("GraphNode")!.ConstructorShape,
            candidate);
    }

    private static IEnumerable<Scope> Enumerate(Scope scope)
    {
        yield return scope;
        foreach (var child in scope.Children)
        {
            foreach (var descendant in Enumerate(child))
            {
                yield return descendant;
            }
        }
    }
}
