namespace Jroc.Tests;

public sealed class NativeDerivedConstructorTests
{
    public static TheoryData<string, string> Scenarios => new()
    {
        {
            "native-replacement-fields-and-methods",
            """
            class Stack extends AsyncDisposableStack {
              value = 7;
              constructor(...args) { super(...args); }
              read() { return this.value; }
              write(value) { this.value = value; }
              increment() { this.value++; return this["value"]; }
              self() { return this; }
              indirect() { return this.read(); }
            }
            const stack = new Stack("ignored", 42);
            assert.strictEqual(Stack.length, 0);
            assert(stack instanceof Stack);
            assert(stack instanceof AsyncDisposableStack);
            assert.strictEqual(stack.read(), 7);
            assert.strictEqual(stack.increment(), 8);
            assert.strictEqual(stack.self(), stack);
            stack.write("changed");
            assert.strictEqual(stack.indirect(), "changed");
            assert.strictEqual(stack.value, "changed");
            assert.strictEqual(Object.getOwnPropertyDescriptor(stack, "value").enumerable, true);
            await stack.disposeAsync();
            """
        },
        {
            "native-replacement-private-fields",
            """
            class Stack extends AsyncDisposableStack {
              #value = 9;
              read() { return this.#value; }
              write(value) { this.#value = value; }
              static branded(value) { return #value in value; }
              static read(value) { return value.#value; }
            }
            const stack = new Stack();
            assert.strictEqual(stack.read(), 9);
            stack.write("changed");
            assert.strictEqual(Stack.read(stack), "changed");
            assert.strictEqual(Stack.branded(stack), true);
            const other = new AsyncDisposableStack();
            assert.strictEqual(Stack.branded(other), false);
            assert.throws(() => Stack.read(other), TypeError);
            const moved = stack.move();
            assert.strictEqual(Stack.branded(moved), false);
            assert.throws(() => Stack.read(moved), TypeError);
            await moved.disposeAsync();
            """
        },
        {
            "synchronous-native-base",
            """
            class Stack extends DisposableStack {
              value = 7;
              constructor(first, ...rest) { super(first, ...rest); this.args = rest; }
              read() { return this.value; }
            }
            const stack = new Stack(1, 2, 3);
            assert.strictEqual(Stack.length, 1);
            assert.strictEqual(stack.read(), 7);
            assert.strictEqual(stack.args.join(","), "2,3");
            assert(stack instanceof DisposableStack);
            stack.dispose();
            """
        },
        {
            "aliased-native-base-and-dynamic-construction",
            """
            const Base = AsyncDisposableStack;
            class Stack extends Base {
              value = 7;
              constructor(...args) { super(...args); }
              read() { return this.value; }
            }
            const Constructor = Stack;
            const stack = new Constructor(1, 2, 3);
            assert.strictEqual(stack.read(), 7);
            assert.strictEqual(Object.getPrototypeOf(stack), Stack.prototype);
            await stack.disposeAsync();
            """
        },
        {
            "native-class-expression",
            """
            const Stack = class extends AsyncDisposableStack {
              value = 7;
              constructor(...args) { super(...args); }
              read() { return this.value; }
            };
            const stack = new Stack("ignored");
            assert.strictEqual(stack.read(), 7);
            assert.strictEqual(Stack.prototype.constructor, Stack);
            assert(stack instanceof AsyncDisposableStack);
            await stack.disposeAsync();
            """
        },
        {
            "default-native-derived-constructor",
            """
            class Stack extends AsyncDisposableStack {
              value = 7;
              read() { return this.value; }
            }
            const stack = new Stack(1, 2, 3);
            assert.strictEqual(stack.read(), 7);
            await stack.disposeAsync();
            """
        },
        {
            "ordinary-rest-constructor-arity",
            """
            class Value {
              constructor(first, ...rest) {
                this.first = first;
                this.rest = rest;
              }
            }
            const value = new Value(1, 2, 3);
            assert.strictEqual(value.first, 1);
            assert.strictEqual(value.rest.join(","), "2,3");
            """
        },
        {
            "proxy-base-preserves-new-target",
            """
            let observed;
            const Base = new Proxy(AsyncDisposableStack, {
              construct(target, args, newTarget) {
                observed = newTarget;
                return Reflect.construct(target, args, newTarget);
              }
            });
            class Stack extends Base {
              value = 7;
              read() { return this.value; }
            }
            const stack = new Stack();
            assert.strictEqual(observed, Stack);
            assert.strictEqual(Object.getPrototypeOf(stack), Stack.prototype);
            assert.strictEqual(stack.read(), 7);
            await stack.disposeAsync();
            """
        }
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void ConstructorAndReceiverSemantics(string name, string body)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            name, "NativeDerivedConstructor",
            _ => ("""
                const assert = require("assert");
                (async () => {
                await undefined;
                """ + body + """
                })().then(() => console.log("ok"), error => console.log("FAILED", error));
                """, null));
        Assert.True(result.Output == $"ok{Environment.NewLine}", $"Unexpected output for {name}: {result.Output}");
    }
}
