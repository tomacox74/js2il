using System.Collections;
using System.Reflection;
using Acornima;
using Acornima.Ast;
using Jroc.Utilities;
using Jroc.Validation;
using AstNode = Acornima.Ast.Node;

namespace Jroc.Tests;

public class ValidatorTraversalTests
{
    private static readonly string[] TraversalSources =
    [
        """
        "use strict";
        ;
        debugger;
        let [a, , ...rest] = [1, , ...items];
        let {x: b = 1, c, ...other} = object;
        a = (b += 2);
        const values = [null, true, false, 1n, /x/g, this, "text"];
        const result = a ? b + c : !a;
        a && b || (c ?? a);
        a++; --b;
        (a, b);
        object?.method?.(...rest);
        new object.Constructor(a);
        tag`a${a}b${b}c`;
        const properties = { a, [b]: c, ...other, method() {}, get x() {}, set x(v) {} };
        function f({x = 1}, ...args) { return new.target; }
        const g = function named() {};
        const arrow = (a = 1) => a;
        if (a) { throw b; } else { f(); }
        label: for (let i = 0; i < 2; i++) {
          if (a) continue label;
          break;
        }
        for (a in object) {}
        for (const x of values) {}
        while (a) { break; }
        do {} while (a);
        switch (a) { case 1: break; default: f(); }
        try {} catch ({message}) {} finally {}
        """,
        """
        async function f() {
          await a;
          for await (const x of values) {}
        }
        const g = async function() { await a; };
        const h = async () => await a;
        function* generator() { yield; yield* values; }
        const k = function* () { yield a; };
        class C extends Base {
          #x = 1;
          field = 2;
          static { this.x = 1; }
          constructor() { super(); }
          async method() { await a; }
          *generator() { yield a; }
          accessor value = 3;
        }
        const D = class extends C {};
        """,
        """
        import defaultValue, {remote as local, same} from "module" with {type: "json"};
        import * as namespace from "other";
        export {local as exported, same};
        export const a = 1;
        export * as all from "module";
        export default function() {};
        import.meta;
        import("module", {with: {type: "json"}});
        """,
        """
        @decorate
        class C {
          @decorate method() {}
          @decorate field;
          @decorate accessor value;
        }
        """,
        "with (object) { value; }"
    ];

    [Fact]
    public void TypedTraversal_CoversEveryConcreteParserNodeAndChildProperty()
    {
        var parser = new Parser(new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            PreserveParens = true,
            ExperimentalESFeatures = ExperimentalESFeatures.Decorators
        });
        var nodes = new HashSet<AstNode>(ReferenceEqualityComparer.Instance);
        foreach (var source in TraversalSources)
        {
            var root = source.Contains("import ", StringComparison.Ordinal)
                ? (Acornima.Ast.Program)parser.ParseModule(source)
                : parser.ParseScript(source);
            Collect(root);
        }

        var concreteTypes = typeof(AstNode).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(AstNode).IsAssignableFrom(type))
            .OrderBy(type => type.FullName);
        Assert.Equal(concreteTypes, nodes.Select(node => node.GetType()).Distinct().OrderBy(type => type.FullName));

        foreach (var node in nodes)
        {
            var expected = new List<AstNode>();
            foreach (var property in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.Name is nameof(AstNode.ChildNodes) or nameof(AstNode.UserData)
                    || !property.CanRead || property.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                switch (property.GetValue(node))
                {
                    case AstNode child:
                        expected.Add(child);
                        break;
                    case IEnumerable collection:
                        foreach (var item in collection)
                        {
                            if (item is AstNode element)
                            {
                                expected.Add(element);
                            }
                        }
                        break;
                }
            }

            var actual = new List<AstNode>();
            AstChildVisitor.Visit(node, child =>
            {
                if (child is not null) actual.Add(child);
            });
            Assert.Equal(expected.Distinct(), actual.Distinct());
        }

        void Collect(AstNode node)
        {
            if (!nodes.Add(node)) return;
            foreach (var child in node.ChildNodes)
            {
                Collect(child);
            }
        }
    }

    [Theory]
    [InlineData("async function f() { try { await a; } catch (e) { await b; } finally { await c; } }")]
    [InlineData("async function f() { for (await a; await b; await c) { await d; } for await (const x of a) { await x; } }")]
    [InlineData("async function f() { const a = [await b, , await c]; const o = {[await d]: await e}; g(await a, ...[await b]); }")]
    [InlineData("async function f() { async function g() { await a; } const h = async () => await b; await c; }")]
    [InlineData("class C { async f() { await a; } async *g() { yield await b; } }")]
    [InlineData("function* f() { try { yield a; } catch (e) { yield b; } finally { yield c; } }")]
    [InlineData("function* f() { const a = [yield b, , yield c]; const o = {[yield d]: yield e}; g(yield a, ...[yield b]); }")]
    [InlineData("function* f() { function* g() { yield a; } const h = function* () { yield b; }; yield; yield* a; }")]
    [InlineData("""
        async function f() {
          class C extends (await baseClass) {
            async [await methodKey]() { await methodBody; }
            [await fieldKey] = async () => await fieldValue;
          }
          const D = class extends (await otherBase) {
            async [await otherKey]() { await otherBody; }
          };
        }
        """)]
    public void Validate_SuspensionChildrenAndInvalidPlacements_PreserveDiagnostics(string source)
    {
        var ast = new Parser().ParseScript(source, "traversal.js");
        var validator = new JavaScriptAstValidator();
        Assert.Empty(validator.Validate(ast).Errors);

        var invalid = (Acornima.Ast.Program)new RemoveSuspensionFlags().Visit(ast)!;
        var result = validator.Validate(invalid);
        var expected = LegacySuspensionErrors(invalid);
        Assert.NotEmpty(expected);
        Assert.False(result.IsValid);
        Assert.Equal(expected, result.Errors);
    }

    [Fact]
    public void Validate_SharedNodesAndNullOptionalChildren_ReportEachSuspensionOnce()
    {
        var awaitNode = new AwaitExpression(new Identifier("value"));
        var yieldNode = new YieldExpression(null, false);
        var elements = NodeList.From<Expression?>([awaitNode, null, yieldNode, awaitNode, yieldNode]);
        var statements = NodeList.From<Statement>([new NonSpecialExpressionStatement(new ArrayExpression(elements))]);
        var body = new FunctionBody(statements, false);
        var function = new FunctionDeclaration(new Identifier("f"), NodeList.Empty<AstNode>(), body, false, false);
        var ast = new Script(NodeList.From<Statement>([function]), false);

        var result = new JavaScriptAstValidator().Validate(ast);

        Assert.Equal(
        [
            "The 'await' keyword is only valid inside async functions (line 0)",
            "The 'yield' keyword is only valid inside generator functions (line 0)"
        ], result.Errors);
    }

    [Fact]
    public void Validate_NestedFunctionDepthAndFinallyState_PreserveExistingBehavior()
    {
        var ast = new Parser().ParseScript("""
            async function* outer() {
              try {} finally {
                function nested() {}
              }
              async function sibling() { await a; }
            }
            """);
        var rewritten = (Acornima.Ast.Program)new AddNestedSuspension().Visit(ast)!;

        var result = new JavaScriptAstValidator().Validate(rewritten);

        Assert.Empty(result.Errors);
        Assert.Equal(LegacySuspensionErrors(rewritten), result.Errors);
    }

    [Fact]
    public void Validate_SiblingFunctionBoundaries_DoNotLeakSuspensionDepth()
    {
        var ast = new Parser().ParseScript("""
            async function allowedAsync() { try {} finally { await value; } }
            function* allowedGenerator() { try {} finally { yield value; } }
            function nested() {}
            """);
        var rewritten = (Acornima.Ast.Program)new AddNestedSuspension().Visit(ast)!;

        var result = new JavaScriptAstValidator().Validate(rewritten);

        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(LegacySuspensionErrors(rewritten), result.Errors);
    }

    [Theory]
    [InlineData("await value;")]
    [InlineData("for await (const value of values) {}")]
    public void Validate_TopLevelAwait_PreservesAsyncRootSupport(string source)
    {
        var ast = new Parser(new ParserOptions { AllowAwaitOutsideFunction = true }).ParseScript(source);

        var result = new JavaScriptAstValidator().Validate(ast);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void TypedTraversal_NullRequiredChildren_DoNotHideLaterChildren()
    {
        var child = new YieldExpression(null, false);
        var malformed = new NonLogicalBinaryExpression("+", null!, child);
        var children = new List<AstNode?>();

        AstChildVisitor.Visit(malformed, children.Add);

        Assert.Equal([null, child], children);
    }

    [Fact]
    public void Validate_MetadataCyclesAndCollections_PreserveVisitedProtection()
    {
        var child = new YieldExpression(null, false);
        var ast = new Script(NodeList.Empty<Statement>(), false);
        ast.UserData = new object[] { ast, child, child };
        child.UserData = ast;

        var result = new JavaScriptAstValidator().Validate(ast);

        Assert.Equal(["The 'yield' keyword is only valid inside generator functions (line 0)"], result.Errors);
    }

    private static List<string> LegacySuspensionErrors(AstNode root)
    {
        var errors = new List<string>();
        for (var pass = 0; pass < 2; pass++)
        {
            var visited = new HashSet<AstNode>(ReferenceEqualityComparer.Instance);
            Walk(root, 0, 0, 0);

            void Walk(AstNode node, int functionDepth, int asyncDepth, int generatorDepth)
            {
                if (!visited.Add(node)) return;
                if (node is IFunction function)
                {
                    functionDepth++;
                    if (function.Async) asyncDepth++;
                    if (function.Generator) generatorDepth++;
                }

                if (pass == 0 && functionDepth > 0 && asyncDepth == 0)
                {
                    if (node is AwaitExpression)
                        errors.Add($"The 'await' keyword is only valid inside async functions (line {node.Location.Start.Line})");
                    if (node is ForOfStatement { Await: true })
                        errors.Add($"The 'for await...of' statement is only valid inside async functions (line {node.Location.Start.Line})");
                }
                if (pass == 1 && node is YieldExpression && generatorDepth == 0)
                    errors.Add($"The 'yield' keyword is only valid inside generator functions (line {node.Location.Start.Line})");

                foreach (var property in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!property.CanRead || property.GetIndexParameters().Length != 0) continue;
                    switch (property.GetValue(node))
                    {
                        case AstNode child:
                            Walk(child, functionDepth, asyncDepth, generatorDepth);
                            break;
                        case IEnumerable collection:
                            foreach (var item in collection)
                                if (item is AstNode element)
                                    Walk(element, functionDepth, asyncDepth, generatorDepth);
                            break;
                    }
                }
            }
        }
        return errors;
    }

    private sealed class RemoveSuspensionFlags : AstRewriter
    {
        protected override object VisitFunctionDeclaration(FunctionDeclaration node)
        {
            var visited = (FunctionDeclaration)base.VisitFunctionDeclaration(node)!;
            return new FunctionDeclaration(visited.Id, visited.Params, visited.Body, false, false)
                { Location = node.Location, Range = node.Range };
        }

        protected override object VisitFunctionExpression(FunctionExpression node)
        {
            var visited = (FunctionExpression)base.VisitFunctionExpression(node)!;
            return new FunctionExpression(visited.Id, visited.Params, visited.Body, false, false)
                { Location = node.Location, Range = node.Range };
        }

        protected override object VisitArrowFunctionExpression(ArrowFunctionExpression node)
        {
            var visited = (ArrowFunctionExpression)base.VisitArrowFunctionExpression(node)!;
            return new ArrowFunctionExpression(visited.Params, visited.Body, visited.Body is Expression, false)
                { Location = node.Location, Range = node.Range };
        }
    }

    private sealed class AddNestedSuspension : AstRewriter
    {
        protected override object VisitFunctionDeclaration(FunctionDeclaration node)
        {
            if (node.Id?.Name != "nested") return base.VisitFunctionDeclaration(node)!;
            var statements = NodeList.From<Statement>(
            [
                new NonSpecialExpressionStatement(new AwaitExpression(new Identifier("value"))),
                new NonSpecialExpressionStatement(new YieldExpression(null, false))
            ]);
            return node.UpdateWith(node.Id, node.Params, new FunctionBody(statements, false));
        }
    }
}
