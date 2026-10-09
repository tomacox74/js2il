using Jroc.Services;
using Jroc.Validation;

namespace Jroc.Tests;

public sealed class ClassDecoratorTests
{
    private static void Execute(string name, string source)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            name, "ClassDecorators",
            _ => ("const assert = require('assert');\n" + source + "\nconsole.log('ok');", null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Theory]
    [InlineData("class C")]
    [InlineData("const C = class Named")]
    public void EvaluationAndApplicationOrder(string declaration)
    {
        var definition = declaration == "class C"
            ? "@factory('outer') @factory('inner') class C"
            : "const C = @factory('outer') @factory('inner') class Named";
        Execute("decorator_order", """
            const order = [];
            function factory(label) {
              order.push("evaluate:" + label);
              return function(value, context) {
                order.push("apply:" + label);
                assert.strictEqual(value.ready, undefined);
                assert.strictEqual(typeof value.prototype.method, "function");
                assert.strictEqual(context.kind, "class");
                assert.strictEqual(context.name, EXPECTED_NAME);
                assert.strictEqual(context.static, undefined);
                assert.strictEqual(context.private, undefined);
                assert.strictEqual(context.access, undefined);
                context.addInitializer(function() {
                  assert.strictEqual(this, value);
                  assert.strictEqual(value.ready, 42);
                  order.push("initialize:" + label);
                });
              };
            }
            function heritage() { order.push("heritage"); return class {}; }
            function key() { order.push("key"); return "method"; }
            """.Replace("EXPECTED_NAME", declaration == "class C" ? "'C'" : "'Named'")
            + definition + """
             extends heritage() {
              static ready = (order.push("field"), 42);
              [key()]() {}
              static { order.push("block"); }
            };
            assert.strictEqual(order.join(","),
              "evaluate:outer,evaluate:inner,heritage,key,apply:inner,apply:outer,field,block,initialize:inner,initialize:outer");
            """);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReplacementConstructionAndOriginalLexicalBinding(bool declaration)
    {
        Execute("decorator_replacement", """
            let original;
            let savedName;
            function replace(value, context) {
              original = value;
              savedName = context.name;
              context.addInitializer(function() {
                assert.strictEqual(this, Replacement);
                assert.strictEqual(original.self(), original);
                assert.strictEqual(original.staticSelf, original);
              });
              return Replacement;
            }
            function Replacement(value) { this.result = value; }
            """
            + (declaration ? "@replace class C" : "const C = @replace class C")
            + """
             {
              static staticSelf = C;
              static self() { return C; }
            };
            assert.strictEqual(C, Replacement);
            assert.strictEqual(savedName, "C");
            assert.notStrictEqual(original, Replacement);
            assert.strictEqual(new C(19).result, 19);
            assert.strictEqual(original.self(), original);
            """);
    }

    [Fact]
    public void AbruptDecorationAndStaticInitializationDoNotRunLaterInitializers()
    {
        Execute("decorator_abrupt_static", """
            const order = [];
            function fail(value, context) {
              order.push("decorate");
              throw new RangeError("expected");
            }
            assert.throws(() => {
              const C = @fail class {
                static value = order.push("field");
              };
            }, RangeError);
            assert.strictEqual(order.join(","), "decorate");
            function initialize(value, context) {
              context.addInitializer(function() { order.push("extra"); });
            }
            assert.throws(() => {
              const C = @initialize class {
                static { throw new RangeError("static"); }
              };
            }, RangeError);
            assert.strictEqual(order.join(","), "decorate");
            """);
    }

    [Fact]
    public void ReverseReplacementsAndFreshContexts()
    {
        Execute("decorator_context", """
            let firstContext;
            let original;
            function R() { this.replaced = true; }
            function inner(value, context) {
              original = value;
              firstContext = context;
              context.kind = "changed";
              context.name = "changed";
              return R;
            }
            function outer(value, context) {
              assert.strictEqual(value, R);
              assert.notStrictEqual(context, firstContext);
              assert.strictEqual(context.kind, "class");
              assert.strictEqual(context.name, "C");
              context.addInitializer(function() {
                assert.strictEqual(this, R);
                assert.throws(() => firstContext.addInitializer(function() {}), TypeError);
              });
            }
            const C = @outer @inner class C {};
            assert.strictEqual(C, R);
            assert.strictEqual(new C().replaced, true);
            """);
    }

    [Fact]
    public void InitializerValidationLifetimeAndReceiver()
    {
        Execute("decorator_initializers", """
            let saved;
            let calls = 0;
            function decorate(value, context) {
              saved = context.addInitializer;
              assert.strictEqual(saved.name, "addInitializer");
              assert.strictEqual(saved.length, 1);
              assert.throws(() => new saved(function() {}), TypeError);
              for (const invalid of [undefined, null, false, 0, "", {}]) {
                assert.throws(() => saved(invalid), TypeError);
              }
              saved(function() { assert.strictEqual(this, value); calls++; });
              saved(function() { assert.strictEqual(this, value); calls++; });
            }
            const C = @decorate class {};
            assert.strictEqual(calls, 2);
            assert.throws(() => saved(function() {}), TypeError);
            """);
    }

    [Fact]
    public void AnonymousNameAndMemberDecoratorThis()
    {
        Execute("decorator_name", """
            let seenName;
            const ns = { decorator(value, context) {
              "use strict";
              assert.strictEqual(this, undefined);
              seenName = context.name;
            } };
            const Inferred = @ns.decorator class {};
            assert.strictEqual(seenName, "Inferred");
            assert.strictEqual(Inferred.name, "Inferred");
            let anonymousName;
            function capture(value, context) { anonymousName = context.name; }
            void (@capture class {});
            assert.strictEqual(anonymousName, undefined);
            """);
    }

    [Fact]
    public void InvalidDecoratorsAndReplacementsAndAbruptCompletion()
    {
        Execute("decorator_errors", """
            let initializers = 0;
            let evaluated = false;
            function valid(value, context) {
              context.addInitializer(function() { initializers++; });
            }
            function checkInvalid(invalid) {
              assert.throws(() => { const C = @valid @(invalid) class {}; }, TypeError);
            }
            function checkReturn(invalid) {
              function invalidReturn() { return invalid; }
              assert.throws(() => { const C = @invalidReturn @valid class {}; }, TypeError);
            }
            checkInvalid(undefined);
            checkInvalid(null);
            checkInvalid(3);
            checkInvalid(false);
            checkInvalid("");
            checkInvalid({});
            checkReturn(null);
            checkReturn(3);
            checkReturn(false);
            checkReturn("");
            checkReturn({});
            assert.strictEqual(initializers, 0);
            function fail() { throw new RangeError("decorator"); }
            assert.throws(() => {
              const C = @fail() class extends (evaluated = true, Object) {};
            }, RangeError);
            assert.strictEqual(evaluated, false);
            """);
    }

    [Fact]
    public void InlineDecoratorCapturesEnclosingEnvironment()
    {
        Execute("decorator_inline", """
            function build(expected) {
              const C = @(function(value, context) {
                assert.strictEqual(context.name, "C");
                assert.strictEqual(expected, 12);
                context.addInitializer(function() { this.answer = expected; });
              }) class {};
              return C;
            }
            assert.strictEqual(build(12).answer, 12);
            const sameName = function(value, context) { value.decorated = true; };
            const C = @sameName class sameName {};
            assert.strictEqual(C.decorated, true);
            """);
    }

    [Fact]
    public void CallableNonConstructorReplacementIsAcceptedButNotConstructible()
    {
        Execute("decorator_nonconstructor", """
            const replacement = () => 23;
            function decorate(value, context) {
              context.addInitializer(function() { assert.strictEqual(this, replacement); });
              return replacement;
            }
            const C = @decorate class {};
            assert.strictEqual(C, replacement);
            assert.strictEqual(C(), 23);
            assert.throws(() => new C(), TypeError);
            """);
    }

    [Fact]
    public void ReplacementSubclassUsesOriginalConstructorAndInitializers()
    {
        Execute("decorator_subclass", """
            let original;
            function decorate(value, context) {
              original = value;
              context.addInitializer(value.initialize);
              return class Replacement extends value {
                extra = 29;
              };
            }
            @decorate class C {
              result = 17;
              static initialize() {
                assert.strictEqual(C, original);
                assert.notStrictEqual(this, C);
                this.initialized = true;
              }
            }
            assert.strictEqual(C.name, "Replacement");
            assert.strictEqual(C.initialized, true);
            const instance = new C();
            assert.strictEqual(instance.result, 17);
            assert.strictEqual(instance.extra, 29);
            """);
    }

    [Fact]
    public void AbruptApplicationClosesContextAndSkipsInitializers()
    {
        Execute("decorator_abrupt", """
            let saved;
            let ran = false;
            function decorate(value, context) {
              saved = context.addInitializer;
              saved(function() { ran = true; });
              throw new RangeError("fail");
            }
            assert.throws(() => { @decorate class C {} }, RangeError);
            assert.strictEqual(ran, false);
            assert.throws(() => saved(function() {}), TypeError);
            let observed;
            function outer(value, context) { observed = context; }
            function inner(value, context) {
              context.addInitializer(function() {
                assert.throws(() => observed.addInitializer(function() {}), TypeError);
              });
            }
            const C = @outer @inner class {};
            """);
    }

    [Fact]
    public void GeneratorDecoratorExpressionResumesBeforeClassEvaluation()
    {
        Execute("decorator_generator", """
            const order = [];
            function outer(value) { order.push("outer"); }
            function inner(value) { order.push("inner"); }
            function* create() {
              const C = @(yield outer) @(yield inner) class {
                static { order.push("static"); }
              };
              return C;
            }
            const iterator = create();
            assert.strictEqual(iterator.next().value, outer);
            assert.strictEqual(order.length, 0);
            assert.strictEqual(iterator.next(outer).value, inner);
            assert.strictEqual(order.length, 0);
            const result = iterator.next(inner);
            assert.strictEqual(result.done, true);
            assert.strictEqual(typeof result.value, "function");
            assert.strictEqual(order.join(","), "inner,outer,static");
            """);
    }

    [Theory]
    [InlineData("class C { @decorate method() {} }")]
    [InlineData("class C { @decorate static method() {} }")]
    [InlineData("class C { @decorate get value() {} }")]
    [InlineData("class C { @decorate value; }")]
    [InlineData("class C { @decorate #value; }")]
    public void RejectsUnimplementedElementDecorators(string source)
    {
        var parser = new JavaScriptParser();
        var ast = parser.ParseJavaScript(source, "element-decorator.js");
        var validation = new JavaScriptAstValidator().Validate(ast);
        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Class element decorators"));
    }
}
