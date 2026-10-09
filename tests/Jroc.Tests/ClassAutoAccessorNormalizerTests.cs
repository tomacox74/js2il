using Acornima.Ast;
using Jroc.Services;

namespace Jroc.Tests;

public sealed class ClassAutoAccessorNormalizerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PreservesLocationsAndInitializerOrder(bool module)
    {
        const string source = "class C { before = 1; accessor value = 2; after = 3; }";
        var parser = new JavaScriptParser();
        var program = module ? parser.ParseJavaScriptModule(source, "original.js") : parser.ParseJavaScript(source, "original.js");
        var body = Assert.IsType<ClassDeclaration>(program.Body[0]).Body;
        Assert.Equal(5, body.Body.Count);
        var field = Assert.IsType<PropertyDefinition>(body.Body[1]);
        Assert.IsType<PrivateIdentifier>(field.Key);
        Assert.Equal(2d, Assert.IsType<NumericLiteral>(field.Value).Value);
        Assert.Equal(source.IndexOf("accessor", StringComparison.Ordinal), field.Start);
        Assert.Equal("original.js", field.Location.SourceFile);
        var getter = Assert.IsType<MethodDefinition>(body.Body[2]);
        var setter = Assert.IsType<MethodDefinition>(body.Body[3]);
        Assert.Equal(PropertyKind.Get, getter.Kind);
        Assert.Equal(PropertyKind.Set, setter.Kind);
        Assert.Equal(field.Location, getter.Value.Location);
        Assert.Equal(field.Location, setter.Value.Location);
        Assert.Empty(getter.Value.Params);
        Assert.Single(setter.Value.Params);
        Assert.True(getter.Value.Body.Strict);
        Assert.Equal("after", Assert.IsType<Identifier>(Assert.IsType<PropertyDefinition>(body.Body[4]).Key).Name);
    }

    [Fact]
    public void AvoidsPrivateNameCaptureAndNormalizesNestedClasses()
    {
        var program = new JavaScriptParser().ParseJavaScript("""
            class C {
              #__jroc_auto_accessor_0;
              accessor value = class { accessor inner; read(obj) { return obj.#__jroc_auto_accessor_0; } };
              accessor other;
            }
            """, "collision.js");
        var privateFields = new List<string>();
        new JavaScriptParser().VisitAst(program, node =>
        {
            Assert.IsNotType<AccessorProperty>(node);
            if (node is PropertyDefinition { Key: PrivateIdentifier key })
                privateFields.Add(key.Name);
        });
        Assert.Equal(4, privateFields.Count);
        Assert.Equal(4, privateFields.Distinct().Count());
    }

    [Fact]
    public void PreservesClassDecoratorNodes()
    {
        var program = new JavaScriptParser().ParseJavaScript("@decorate class C { accessor value; }", "decorated.js");
        var declaration = Assert.IsType<ClassDeclaration>(program.Body[0]);
        Assert.Single(declaration.Decorators);
        Assert.Equal("decorate", Assert.IsType<Identifier>(declaration.Decorators[0].Expression).Name);
    }

    [Theory]
    [InlineData("class C { accessor [key()] = 1; }", "Computed auto-accessors")]
    [InlineData("class C { accessor #value = 1; }", "Private auto-accessors")]
    [InlineData("class C { @decorate accessor value = 1; }", "Decorated auto-accessors")]
    public void RejectsUnsupportedShapesWithSourceLocation(string source, string message)
    {
        var exception = Assert.Throws<NotSupportedException>(() => new JavaScriptParser().ParseJavaScript(source, "unsupported.js"));
        Assert.Contains(message, exception.Message);
        Assert.Contains("unsupported.js:1:", exception.Message);
    }

    [Theory]
    [InlineData("class C { accessor value = 1; }")]
    [InlineData("const C = class { accessor value = 1; };")]
    public void NativeReadsWritesDescriptorsAndBranding(string declaration)
    {
        var result = InMemoryTestCompiler.CompileAndExecute("auto_accessor", "ClassAutoAccessor", _ => ("""
            const assert = require("assert");
            """ + declaration + """
            const first = new C();
            const second = new C();
            assert.strictEqual(first.value, 1);
            first.value = 9;
            assert.strictEqual(first.value, 9);
            assert.strictEqual(second.value, 1);
            assert.strictEqual(Object.getOwnPropertyDescriptor(first, "value"), undefined);
            assert.strictEqual(Object.getOwnPropertyNames(first).length, 0);
            const descriptor = Object.getOwnPropertyDescriptor(C.prototype, "value");
            assert.strictEqual(typeof descriptor.get, "function");
            assert.strictEqual(typeof descriptor.set, "function");
            assert.strictEqual(descriptor.enumerable, false);
            assert.strictEqual(descriptor.configurable, true);
            assert.strictEqual(descriptor.get.length, 0);
            assert.strictEqual(descriptor.set.length, 1);
            assert.throws(() => descriptor.get.call({}), TypeError);
            assert.throws(() => descriptor.set.call({}, 3), TypeError);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void NativeEscapedNamesAndUninitializedBackingFields()
    {
        var result = InMemoryTestCompiler.CompileAndExecute("auto_accessor_names", "ClassAutoAccessor", _ => ("""
            const assert = require("assert");
            const C = class {
              accessor $;
              accessor _;
              accessor \u{6F};
              accessor \u2118;
              accessor ZW_\u200C_NJ;
              accessor ZW_\u200D_J;
            };
            const instance = new C();
            const names = ["$", "_", "o", "\u2118", "ZW_\u200C_NJ", "ZW_\u200D_J"];
            for (const name of names) {
              assert.strictEqual(instance[name], undefined);
              instance[name] = name;
              assert.strictEqual(instance[name], name);
              const descriptor = Object.getOwnPropertyDescriptor(C.prototype, name);
              assert.strictEqual(typeof descriptor.get, "function");
              assert.strictEqual(typeof descriptor.set, "function");
              assert.strictEqual(descriptor.enumerable, false);
              assert.strictEqual(descriptor.configurable, true);
            }
            assert.strictEqual(Object.getOwnPropertyNames(instance).length, 0);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void NativeStaticInstanceInheritanceAndInitializerOrder()
    {
        var result = InMemoryTestCompiler.CompileAndExecute("auto_accessor_order", "ClassAutoAccessor", _ => ("""
            const assert = require("assert");
            const order = [];
            class C {
              before = order.push("before");
              accessor value = order.push("value");
              after = order.push("after");
              static accessor staticValue = 10;
            }
            class D extends C {}
            const instance = new D();
            assert.strictEqual(order.join(","), "before,value,after");
            assert.strictEqual(instance.value, 2);
            instance.value = 30;
            assert.strictEqual(instance.value, 30);
            assert.strictEqual(C.staticValue, 10);
            C.staticValue = 20;
            assert.strictEqual(C.staticValue, 20);
            const descriptor = Object.getOwnPropertyDescriptor(C, "staticValue");
            assert.strictEqual(descriptor.enumerable, false);
            assert.strictEqual(descriptor.configurable, true);
            assert.throws(() => descriptor.get.call(D), TypeError);
            assert.throws(() => descriptor.set.call(D, 3), TypeError);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }
}
