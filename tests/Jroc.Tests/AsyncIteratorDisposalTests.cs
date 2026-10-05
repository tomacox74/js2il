namespace Jroc.Tests;

public sealed class AsyncIteratorDisposalTests
{
    public static TheoryData<string, string> Scenarios => new()
    {
        {
            "nullish-return-resolves-undefined",
            """
            for (const receiver of [{}, { return: undefined }, { return: null }, 1]) {
              assert.strictEqual(await dispose.call(receiver), undefined);
            }
            """
        },
        {
            "invalid-receiver-and-method-reject",
            """
            for (const receiver of [undefined, null, { return: 1 }]) {
              const promise = dispose.call(receiver);
              assert(promise instanceof Promise);
              await promise.then(
                () => assert.fail("must reject"),
                error => assert(error instanceof TypeError));
            }
            """
        },
        {
            "receiver-argument-and-pending-return",
            """
            const pending = Promise.withResolvers();
            let reads = 0, calls = 0, settled = false;
            const receiver = { get return() {
              reads++;
              return function(value) {
                calls++;
                assert.strictEqual(this, receiver);
                assert.strictEqual(arguments.length, 1);
                assert.strictEqual(value, undefined);
                return pending.promise;
              };
            } };
            const result = dispose.call(receiver);
            result.then(() => { settled = true; });
            await undefined;
            assert.strictEqual(reads, 1);
            assert.strictEqual(calls, 1);
            assert.strictEqual(settled, false);
            pending.resolve(42);
            assert.strictEqual(await result, undefined);
            assert.strictEqual(settled, true);
            """
        },
        {
            "thenable-assimilation-and-undefined-rejection",
            """
            const order = [];
            const result = dispose.call({ return() {
              return { get then() {
                order.push("get");
                return resolve => { order.push("call"); resolve(42); };
              } };
            } });
            order.push("returned");
            assert.strictEqual(await result, undefined);
            assert.strictEqual(order.join(","), "get,returned,call");
            let rejected = false;
            await dispose.call({ return() { throw undefined; } }).then(
              () => assert.fail("must reject"),
              reason => { rejected = true; assert.strictEqual(reason, undefined); });
            assert.strictEqual(rejected, true);
            """
        },
        {
            "intrinsic-promise-resolution-and-reactions",
            """
            const promise = Promise.resolve(42);
            const species = Object.getOwnPropertyDescriptor(Promise, Symbol.species);
            Object.defineProperty(Promise, Symbol.species, {
              get() { throw new Error("must not read species"); },
              configurable: true
            });
            promise.then = () => { throw new Error("must not invoke public then"); };
            const originalResolve = Promise.resolve;
            Promise.resolve = () => { throw new Error("must use intrinsic resolution"); };
            try {
              assert.strictEqual(await dispose.call({ return() { return promise; } }), undefined);
            } finally {
              Promise.resolve = originalResolve;
              Object.defineProperty(Promise, Symbol.species, species);
            }
            """
        },
        {
            "throwing-promise-constructor-rejects",
            """
            const promise = Promise.resolve();
            const marker = {};
            Object.defineProperty(promise, "constructor", { get() { throw marker; } });
            await dispose.call({ return() { return promise; } }).then(
              () => assert.fail("must reject"),
              reason => assert.strictEqual(reason, marker));
            """
        }
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void DisposalSemantics(string name, string body)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            name, "AsyncIterator.Disposal",
            _ => ("""
                const assert = require("assert");
                async function* generator() {}
                const prototype = Object.getPrototypeOf(Object.getPrototypeOf(generator.prototype));
                const dispose = prototype[Symbol.asyncDispose];
                (async () => {
                await undefined;
                """ + body + """
                })().then(() => console.log("ok"), error => console.log("FAILED", error));
                """, null));
        Assert.True(result.Output == $"ok{Environment.NewLine}", $"Unexpected output for {name}: {result.Output}");
    }
}
