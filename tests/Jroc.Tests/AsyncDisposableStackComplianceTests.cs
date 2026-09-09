using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class AsyncDisposableStackComplianceTests
{
    public static TheoryData<string, string> Scenarios => new()
    {
        {
            "invalid-receiver-rejects",
            """
            for (const receiver of [undefined, null, 1, {}, AsyncDisposableStack.prototype, new DisposableStack()]) {
              const promise = AsyncDisposableStack.prototype.disposeAsync.call(receiver);
              assert(promise instanceof Promise);
              await promise.then(() => assert.fail("must reject"), error => assert(error instanceof TypeError));
            }
            """
        },
        {
            "sync-stack-is-not-an-async-stack",
            """
            const receiver = new DisposableStack();
            const prototype = AsyncDisposableStack.prototype;
            assert.throws(() => prototype.use.call(receiver, null), TypeError);
            assert.throws(() => prototype.adopt.call(receiver, null, () => {}), TypeError);
            assert.throws(() => prototype.defer.call(receiver, () => {}), TypeError);
            assert.throws(() => prototype.move.call(receiver), TypeError);
            const getter = Object.getOwnPropertyDescriptor(prototype, "disposed").get;
            assert.throws(() => getter.call(receiver), TypeError);
            await prototype.disposeAsync.call(receiver).then(
              () => assert.fail("must reject"), error => assert(error instanceof TypeError));
            """
        },
        {
            "sync-disposer-return-ignored",
            """
            const stack = new AsyncDisposableStack();
            let touched = false;
            const resource = { [Symbol.dispose]() {
              assert.strictEqual(this, resource);
              return { get then() { touched = true; throw new Error("must not assimilate"); } };
            } };
            stack.use(resource);
            stack.use({ [Symbol.dispose]() { return new Promise(() => {}); } });
            await stack.disposeAsync();
            assert.strictEqual(touched, false);
            """
        },
        {
            "nullish-await-once",
            """
            for (const value of [null, undefined]) {
              const stack = new AsyncDisposableStack();
              stack.use(value);
              stack.use(value);
              const sequence = [];
              const first = Promise.resolve().then(() => {}).then(() => { sequence.push("first"); });
              const disposal = stack.disposeAsync().then(() => { sequence.push("dispose"); });
              const last = Promise.resolve().then(() => {}).then(() => { sequence.push("last"); });
              await Promise.all([first, disposal, last]);
              assert.strictEqual(sequence.join(","), "first,dispose,last");
            }
            """
        },
        {
            "empty-and-repeated-disposal-settle-immediately",
            """
            const stack = new AsyncDisposableStack();
            const sequence = [];
            const first = stack.disposeAsync();
            const second = stack.disposeAsync();
            assert.notStrictEqual(first, second);
            first.then(() => { sequence.push("first"); });
            second.then(() => { sequence.push("second"); });
            Promise.resolve().then(() => { sequence.push("tick"); });
            await second;
            assert.strictEqual(sequence.join(","), "first,second,tick");
            """
        },
        {
            "nullish-registration-does-not-add-a-second-await",
            """
            const stack = new AsyncDisposableStack();
            stack.use(undefined);
            stack.defer(() => undefined);
            stack.use(null);
            const sequence = [];
            const first = Promise.resolve().then(() => {}).then(() => { sequence.push("first"); });
            const disposal = stack.disposeAsync().then(() => { sequence.push("dispose"); });
            const last = Promise.resolve().then(() => {}).then(() => { sequence.push("last"); });
            await Promise.all([first, disposal, last]);
            assert.strictEqual(sequence.join(","), "first,dispose,last");
            """
        },
        {
            "sync-disposer-throw-awaits",
            """
            const stack = new AsyncDisposableStack();
            const order = [];
            stack.defer(() => { order.push("older"); });
            stack.use({ [Symbol.dispose]() { order.push("sync"); throw undefined; } });
            const disposal = stack.disposeAsync();
            order.push("returned");
            let rejected = false;
            await disposal.then(() => assert.fail("must reject"), reason => {
              rejected = true;
              assert.strictEqual(reason, undefined);
            });
            assert.strictEqual(rejected, true);
            assert.strictEqual(order.join(","), "sync,returned,older");
            """
        },
        {
            "async-disposer-sync-throw-does-not-await",
            """
            const stack = new AsyncDisposableStack();
            const order = [];
            stack.defer(() => { order.push("older"); });
            stack.use({ [Symbol.asyncDispose]() { order.push("async"); throw undefined; } });
            const disposal = stack.disposeAsync();
            order.push("returned");
            await disposal.then(() => assert.fail("must reject"), reason => assert.strictEqual(reason, undefined));
            assert.strictEqual(order.join(","), "async,older,returned");
            """
        },
        {
            "undefined-errors-suppressed",
            """
            const stack = new AsyncDisposableStack();
            stack.defer(() => { throw null; });
            stack.defer(() => Promise.reject(undefined));
            await stack.disposeAsync().then(() => assert.fail("must reject"), error => {
              assert(error instanceof SuppressedError);
              assert.strictEqual(error.error, null);
              assert.strictEqual(error.suppressed, undefined);
              for (const key of ["error", "suppressed"]) {
                const descriptor = Object.getOwnPropertyDescriptor(error, key);
                assert.strictEqual(descriptor.writable, true);
                assert.strictEqual(descriptor.enumerable, false);
                assert.strictEqual(descriptor.configurable, true);
              }
            });
            """
        },
        {
            "thenable-assimilation-is-a-job",
            """
            const stack = new AsyncDisposableStack();
            const order = [];
            stack.defer(() => ({ get then() {
              order.push("get");
              return resolve => { order.push("call"); resolve(); };
            } }));
            const disposal = stack.disposeAsync();
            order.push("returned");
            await disposal;
            assert.strictEqual(order.join(","), "get,returned,call");
            """
        },
        {
            "first-resolution-wins-before-thenable-settlement",
            """
            const deferred = Promise.withResolvers();
            deferred.resolve({ then(resolve) { resolve(1); } });
            deferred.reject(new Error("ignored rejection"));
            deferred.resolve(2);
            assert.strictEqual(await deferred.promise, 1);
            const stack = new AsyncDisposableStack();
            stack.defer(() => new Promise((resolve, reject) => {
              resolve({ then(resolve) { resolve(); } });
              reject(new Error("ignored rejection"));
              throw new Error("ignored exception");
            }));
            await stack.disposeAsync();
            """
        },
        {
            "thenable-throws-preserve-reason",
            """
            const stack = new AsyncDisposableStack();
            stack.defer(() => ({ then() { throw undefined; } }));
            await stack.disposeAsync().then(() => assert.fail("must reject"), reason => {
              assert.strictEqual(reason, undefined);
            });
            """
        },
        {
            "throwing-promise-constructor",
            """
            const stack = new AsyncDisposableStack();
            const marker = {};
            let continued = false;
            stack.defer(() => { continued = true; });
            stack.defer(() => {
              const promise = Promise.resolve();
              Object.defineProperty(promise, "constructor", { get() { throw marker; } });
              return promise;
            });
            const disposal = stack.disposeAsync();
            await disposal.then(() => assert.fail("must reject"), reason => assert.strictEqual(reason, marker));
            assert.strictEqual(continued, true);
            """
        },
        {
            "internal-await-does-not-call-then-or-species",
            """
            const stack = new AsyncDisposableStack();
            const value = Promise.resolve();
            Object.defineProperty(value, "then", { get() { throw new Error("then"); } });
            Object.defineProperty(Promise, Symbol.species, { get() { throw new Error("species"); }, configurable: true });
            stack.defer(() => value);
            await stack.disposeAsync();
            delete Promise[Symbol.species];
            """
        },
        {
            "settlement-does-not-assimilate-internal-promises",
            """
            const stack = new AsyncDisposableStack();
            stack.defer(() => undefined);
            const original = Object.getOwnPropertyDescriptor(Promise.prototype, "then");
            Object.defineProperty(Promise.prototype, "then", { get() { throw new Error("internal then observed"); }, configurable: true });
            try {
              await stack.disposeAsync();
            } finally {
              Object.defineProperty(Promise.prototype, "then", original);
            }
            """
        },
        {
            "constructors-ignore-excess-arguments",
            """
            const C = AsyncDisposableStack;
            const stack = new C(1, 2, 3);
            const prototype = {};
            let gets = 0;
            function Target() {}
            const target = new Proxy(Target, { get(t, key) {
              if (key === "prototype") { gets++; return prototype; }
              return Reflect.get(t, key);
            } });
            const custom = Reflect.construct(C, [1, 2, 3], target);
            assert.strictEqual(Object.getPrototypeOf(custom), prototype);
            assert.strictEqual(gets, 1);
            await AsyncDisposableStack.prototype.disposeAsync.call(custom);
            await stack.disposeAsync();
            """
        },
        {
            "subclass-fields-and-methods",
            """
            class Stack extends AsyncDisposableStack {
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
            assert.strictEqual(Object.getPrototypeOf(moved), AsyncDisposableStack.prototype);
            await moved.disposeAsync();
            assert.strictEqual(called, true);
            """
        },
        {
            "subclass-private-brand",
            """
            class Stack extends AsyncDisposableStack {
              #value = 9;
              read() { return this.#value; }
              static branded(value) { return #value in value; }
            }
            const stack = new Stack();
            assert.strictEqual(stack.read(), 9);
            assert.strictEqual(Stack.branded(stack), true);
            assert.strictEqual(Stack.branded(new AsyncDisposableStack()), false);
            const moved = stack.move();
            assert.strictEqual(Stack.branded(moved), false);
            await moved.disposeAsync();
            """
        },
        {
            "methods-honor-overrides",
            """
            const stack = new AsyncDisposableStack();
            stack.use = value => value + 1;
            assert.strictEqual(stack.use(3), 4);
            const saved = AsyncDisposableStack.prototype.defer;
            AsyncDisposableStack.prototype.defer = () => 42;
            assert.strictEqual(new AsyncDisposableStack().defer(null), 42);
            AsyncDisposableStack.prototype.defer = saved;
            """
        },
        {
            "reentrant-move-during-resource-lookup",
            """
            const stack = new AsyncDisposableStack();
            let moved, called = false;
            stack.use({ get [Symbol.asyncDispose]() {
              moved = stack.move();
              return () => { called = true; };
            } });
            await moved.disposeAsync();
            assert.strictEqual(called, true);
            """
        }
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void DisposalSemantics(string name, string body)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            name, "AsyncDisposableStack.Compliance",
            _ => ("""
                const assert = require("assert");
                (async () => {
                await undefined;
                """ + body + """
                })().then(() => console.log("ok"), error => console.log("FAILED", error));
                """, null));
        Assert.True(result.Output == $"ok{Environment.NewLine}", $"Unexpected output for {name}: {result.Output}");
    }

    [Fact]
    public void SynchronousThrowsDoNotOverflowTheHostStack()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var stack = new AsyncDisposableStack();
        BuiltinFunction0 disposer = _ => throw new JsThrownValueException(null);
        for (var i = 0; i < 20000; i++)
        {
            stack.defer(BuiltinDelegateFunctionAdapter.FromDelegate(disposer));
        }
        var promise = (JavaScriptRuntime.Promise)stack.disposeAsync();
        promise.then(null, (BuiltinFunction1)((_, _) => null));
    }

    [Fact]
    public void HostProcessExitIsNotSuppressedAsADisposalError()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var exit = new ScriptProcessExitException();
        var disposer = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(_ => throw exit));
        var sync = new DisposableStack();
        sync.defer(disposer);
        Assert.Same(exit, Assert.Throws<ScriptProcessExitException>(() => sync.dispose()));
        var async = new AsyncDisposableStack();
        async.defer(disposer);
        Assert.Same(exit, Assert.Throws<ScriptProcessExitException>(() => async.disposeAsync()));
    }

    [Fact]
    public void DisposedStackReleasesResourceStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var stack = new AsyncDisposableStack();
        stack.adopt(new object(), BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((_, _) => null)));
        stack.disposeAsync();
        var resources = typeof(AsyncDisposableStack).GetField(
            "_resources", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        Assert.Empty((System.Collections.IEnumerable)resources.GetValue(stack)!);
    }
}
