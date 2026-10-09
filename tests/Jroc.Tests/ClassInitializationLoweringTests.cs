namespace Jroc.Tests;

public sealed class ClassInitializationLoweringTests
{
    [Fact]
    public void BaseConstructionRestoresNewTargetAfterAnException()
    {
        var result = InMemoryTestCompiler.CompileAndExecute("class_abrupt_new_target", "Classes", _ => ("""
            const assert = require("assert");
            class C { constructor() { throw new Error("expected"); } }
            function caller() {
              try { new C(); } catch (error) {}
              return new.target;
            }
            assert.strictEqual(caller(), undefined);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void FusedFieldConstructionPreservesTheConstructedClassNewTarget()
    {
        var result = InMemoryTestCompiler.CompileAndExecute("class_fused_new_target", "Classes", _ => ("""
            const assert = require("assert");
            class Leaf {
              constructor() { this.target = new.target; }
            }
            class Owner {
              constructor() { this.child = new Leaf(); }
            }
            assert.strictEqual(new Owner().child.target, Leaf);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void StaticAutoAccessorIsAvailableDuringClassInitialization()
    {
        var result = InMemoryTestCompiler.CompileAndExecute("class_static_accessor", "Classes", _ => ("""
            const assert = require("assert");
            class C {
              static accessor value = 7;
              static copy = C.value;
              static { C.value = 9; }
            }
            assert.strictEqual(C.copy, 7);
            assert.strictEqual(C.value, 9);
            console.log("ok");
            """, null));
        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }
}
