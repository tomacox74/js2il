using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class DisposableStackComplianceTests
{
    public static TheoryData<string, string> Scenarios => new()
    {
        {
            "surface-and-descriptors",
            """
            const p = DisposableStack.prototype;
            assert.strictEqual(DisposableStack.name, "DisposableStack");
            assert.strictEqual(DisposableStack.length, 0);
            assert.strictEqual(Object.getPrototypeOf(p), Object.prototype);
            assert.strictEqual(p.constructor, DisposableStack);
            assert.strictEqual(p[Symbol.dispose], p.dispose);
            assert.strictEqual(Object.prototype.toString.call(new DisposableStack()), "[object DisposableStack]");
            for (const key of ["constructor", "use", "adopt", "defer", "move", "dispose", Symbol.dispose]) {
              const d = Object.getOwnPropertyDescriptor(p, key);
              assert.strictEqual(d.enumerable, false);
              assert.strictEqual(d.configurable, true);
              assert.strictEqual(d.writable, true);
            }
            for (const [key, length] of [["use", 1], ["adopt", 2], ["defer", 1], ["move", 0], ["dispose", 0]]) {
              assert.strictEqual(p[key].length, length);
              assert.strictEqual(p[key].name, key);
              assert.strictEqual(Object.hasOwn(p[key], "prototype"), false);
              assert.throws(() => new p[key](), TypeError);
            }
            const tag = Object.getOwnPropertyDescriptor(p, Symbol.toStringTag);
            assert.strictEqual(tag.writable, false);
            assert.strictEqual(tag.enumerable, false);
            assert.strictEqual(tag.configurable, true);
            const d = Object.getOwnPropertyDescriptor(p, "disposed");
            assert.strictEqual(d.get.name, "get disposed");
            assert.strictEqual(d.get.length, 0);
            assert.strictEqual(d.set, undefined);
            assert.strictEqual(d.enumerable, false);
            assert.strictEqual(d.configurable, true);
            const prototype = Object.getOwnPropertyDescriptor(DisposableStack, "prototype");
            assert.strictEqual(prototype.writable, false);
            assert.strictEqual(prototype.enumerable, false);
            assert.strictEqual(prototype.configurable, false);
            assert.throws(() => DisposableStack(), TypeError);
            """
        },
        {
            "receiver-brand-checks",
            """
            const p = DisposableStack.prototype;
            const getter = Object.getOwnPropertyDescriptor(p, "disposed").get;
            for (const receiver of [undefined, null, 1, {}, p, new AsyncDisposableStack(), new Proxy(new DisposableStack(), {})]) {
              assert.throws(() => p.use.call(receiver, null), TypeError);
              assert.throws(() => p.adopt.call(receiver, null, () => {}), TypeError);
              assert.throws(() => p.defer.call(receiver, () => {}), TypeError);
              assert.throws(() => p.move.call(receiver), TypeError);
              assert.throws(() => p.dispose.call(receiver), TypeError);
              assert.throws(() => getter.call(receiver), TypeError);
            }
            """
        },
        {
            "lifo-callback-arguments",
            """
            const stack = new DisposableStack();
            const order = [];
            const value = {};
            const resource = { [Symbol.dispose]: function() {
              "use strict";
              assert.strictEqual(this, resource);
              assert.strictEqual(arguments.length, 0);
              order.push("use");
            } };
            assert.strictEqual(stack.use(resource), resource);
            assert.strictEqual(stack.adopt(value, function(v) {
              "use strict";
              assert.strictEqual(this, undefined);
              assert.strictEqual(arguments.length, 1);
              assert.strictEqual(v, value);
              order.push("adopt");
            }), value);
            assert.strictEqual(stack.defer(function() {
              "use strict";
              assert.strictEqual(this, undefined);
              assert.strictEqual(arguments.length, 0);
              order.push("defer");
            }), undefined);
            assert.strictEqual(stack.disposed, false);
            assert.strictEqual(stack.dispose(), undefined);
            assert.strictEqual(stack.disposed, true);
            assert.strictEqual(stack[Symbol.dispose](), undefined);
            assert.strictEqual(order.join(","), "defer,adopt,use");
            """
        },
        {
            "validation-and-nullish-resources",
            """
            const stack = new DisposableStack();
            assert.strictEqual(stack.use(undefined), undefined);
            assert.strictEqual(stack.use(null), null);
            for (const value of [1, true, "s", Symbol(), 1n, {}, { [Symbol.dispose]: null }, { [Symbol.dispose]: 1 }, { [Symbol.asyncDispose]() {} }]) {
              assert.throws(() => stack.use(value), TypeError);
            }
            for (const callback of [undefined, null, 1, {}]) {
              assert.throws(() => stack.adopt(null, callback), TypeError);
              assert.throws(() => stack.defer(callback), TypeError);
            }
            stack.dispose();
            """
        },
        {
            "lookup-is-once-and-only-sync",
            """
            const stack = new DisposableStack();
            let gets = 0, calls = 0;
            const resource = {
              get [Symbol.dispose]() { gets++; return () => { calls++; }; },
              get [Symbol.asyncDispose]() { throw new Error("must not read"); }
            };
            stack.use(resource);
            assert.strictEqual(gets, 1);
            Object.defineProperty(resource, Symbol.dispose, { value: null });
            stack.dispose();
            assert.strictEqual(gets, 1);
            assert.strictEqual(calls, 1);
            const marker = {};
            const other = new DisposableStack();
            let caught = false;
            try { other.use({ get [Symbol.dispose]() { throw marker; } }); }
            catch (error) { caught = true; assert.strictEqual(error, marker); }
            assert.strictEqual(caught, true);
            assert.strictEqual(other.disposed, false);
            other.dispose();
            """
        },
        {
            "disposed-validation-precedes-property-access",
            """
            const stack = new DisposableStack();
            stack.dispose();
            assert.throws(() => stack.use({ get [Symbol.dispose]() { throw new Error("lookup"); } }), ReferenceError);
            assert.throws(() => stack.use(null), ReferenceError);
            assert.throws(() => stack.adopt(null, null), ReferenceError);
            assert.throws(() => stack.defer(null), ReferenceError);
            assert.throws(() => stack.move(), ReferenceError);
            assert.strictEqual(stack.dispose(), undefined);
            """
        },
        {
            "move-transfers-without-observable-construction",
            """
            const stack = new DisposableStack();
            let calls = 0;
            stack.defer(() => { calls++; });
            Object.defineProperty(stack, "constructor", { get() { throw new Error("constructor"); } });
            const moved = stack.move();
            assert.notStrictEqual(moved, stack);
            assert.strictEqual(Object.getPrototypeOf(moved), DisposableStack.prototype);
            assert.strictEqual(stack.disposed, true);
            assert.strictEqual(moved.disposed, false);
            stack.dispose();
            assert.strictEqual(calls, 0);
            moved.dispose();
            assert.strictEqual(calls, 1);
            """
        },
        {
            "reentrancy",
            """
            const stack = new DisposableStack();
            let moved, calls = 0;
            stack.use({ get [Symbol.dispose]() {
              moved = stack.move();
              return () => { calls++; };
            } });
            moved.defer(() => {
              assert.strictEqual(moved.disposed, true);
              assert.strictEqual(moved.dispose(), undefined);
              assert.throws(() => moved.defer(() => {}), ReferenceError);
            });
            moved.dispose();
            assert.strictEqual(calls, 1);
            const second = new DisposableStack();
            second.use({ get [Symbol.dispose]() {
              second.dispose();
              return () => { throw new Error("must never run"); };
            } });
            second.dispose();
            """
        },
        {
            "return-values-never-awaited-or-assimilated",
            """
            const stack = new DisposableStack();
            const order = [];
            const hostile = { get then() { throw new Error("assimilated"); } };
            stack.use({ [Symbol.dispose]() { order.push("use"); return hostile; } });
            stack.adopt(null, () => { order.push("adopt"); return new Promise(() => {}); });
            stack.defer(() => { order.push("defer"); return hostile; });
            stack.dispose();
            order.push("returned");
            assert.strictEqual(order.join(","), "defer,adopt,use,returned");
            """
        },
        {
            "suppressed-errors-preserve-undefined",
            """
            const stack = new DisposableStack();
            const marker = {};
            stack.defer(() => { throw marker; });
            stack.defer(() => { throw null; });
            stack.defer(() => { throw undefined; });
            let caught = false;
            try { stack.dispose(); } catch (error) {
              caught = true;
              assert(error instanceof SuppressedError);
              assert.strictEqual(error.error, marker);
              assert(error.suppressed instanceof SuppressedError);
              assert.strictEqual(error.suppressed.error, null);
              assert.strictEqual(error.suppressed.suppressed, undefined);
              for (const key of ["error", "suppressed"]) {
                const d = Object.getOwnPropertyDescriptor(error, key);
                assert.strictEqual(d.writable, true);
                assert.strictEqual(d.enumerable, false);
                assert.strictEqual(d.configurable, true);
              }
            }
            assert.strictEqual(caught, true);
            assert.strictEqual(stack.disposed, true);
            assert.strictEqual(stack.dispose(), undefined);
            """
        },
        {
            "single-errors-preserve-identity-and-continue",
            """
            for (const value of [undefined, null, 5, {}, new Error("marker")]) {
              const stack = new DisposableStack();
              let continued = false, caught = false;
              stack.defer(() => { continued = true; });
              stack.defer(() => { throw value; });
              try { stack.dispose(); }
              catch (error) { caught = true; assert.strictEqual(error, value); }
              assert.strictEqual(caught, true);
              assert.strictEqual(continued, true);
            }
            """
        },
        {
            "constructors-and-subclasses",
            """
            const C = DisposableStack;
            const plain = new C(1, 2, 3);
            plain.dispose();
            class Stack extends DisposableStack {
              value = 7;
              constructor(...args) { super(...args); }
              read() { return this.value; }
            }
            const stack = new Stack("ignored");
            assert(stack instanceof Stack);
            assert.strictEqual(stack.read(), 7);
            let called = false;
            stack.defer(() => { called = true; });
            const moved = stack.move();
            assert(!(moved instanceof Stack));
            moved.dispose();
            assert.strictEqual(called, true);
            """
        },
        {
            "methods-honor-overrides",
            """
            const stack = new DisposableStack();
            stack.use = value => value + 1;
            assert.strictEqual(stack.use(3), 4);
            const saved = DisposableStack.prototype.defer;
            DisposableStack.prototype.defer = () => 42;
            assert.strictEqual(new DisposableStack().defer(null), 42);
            DisposableStack.prototype.defer = saved;
            """
        },
        {
            "reflect-construct-new-target",
            """
            const prototype = {};
            let gets = 0;
            function Target() {}
            const target = new Proxy(Target, { get(t, key) {
              if (key === "prototype") { gets++; return prototype; }
              return Reflect.get(t, key);
            } });
            const stack = Reflect.construct(DisposableStack, [1, 2, 3], target);
            assert.strictEqual(Object.getPrototypeOf(stack), prototype);
            assert.strictEqual(gets, 1);
            DisposableStack.prototype.dispose.call(stack);
            """
        }
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void DisposalSemantics(string name, string body)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            name, "DisposableStack.Compliance",
            _ => ("const assert = require('assert');\n" + body + "\nconsole.log('ok');", null));
        Assert.True(result.Output == $"ok{Environment.NewLine}", $"Unexpected output for {name}: {result.Output}");
    }

    [Fact]
    public void SynchronousThrowsDoNotOverflowTheHostStack()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var stack = new DisposableStack();
        BuiltinFunction0 disposer = _ => throw new JsThrownValueException(null);
        for (var i = 0; i < 20000; i++)
        {
            stack.defer(BuiltinDelegateFunctionAdapter.FromDelegate(disposer));
        }

        object? error = Assert.Throws<SuppressedError>(() => stack.dispose());
        for (var i = 0; i < 19999; i++)
        {
            var suppressed = Assert.IsType<SuppressedError>(error);
            Assert.Null(ObjectRuntime.GetProperty(suppressed, "error"));
            error = ObjectRuntime.GetProperty(suppressed, "suppressed");
        }
        Assert.Null(error);
        Assert.Null(stack.dispose());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DisposedStackReleasesResourceStorage(bool throws)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var stack = new DisposableStack();
        stack.adopt(new object(), BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((_, _) => null)));
        if (throws)
        {
            stack.defer(BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction0)(_ => throw new JsThrownValueException(null))));
        }
        var resources = typeof(DisposableStack).GetField(
            "_resources", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        var original = (System.Collections.IEnumerable)resources.GetValue(stack)!;
        if (throws)
        {
            Assert.Throws<JsThrownValueException>(() => stack.dispose());
        }
        else
        {
            stack.dispose();
        }
        Assert.Empty(original);
        Assert.Empty((System.Collections.IEnumerable)resources.GetValue(stack)!);
    }
}
