using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class TypedArrayRuntimeTests
{
    [Theory]
    [InlineData("set-array-like", """
        const target = new Uint8Array(2);
        let log = "";
        const source = {
          length: 2,
          get [Symbol.iterator]() { throw new Error("iterator must not be read"); },
          get 0() { log += "get0;"; return { valueOf() { log += "convert0;"; return 7; } }; },
          get 1() { log += "get1;"; assert.strictEqual(target[0], 7); return 8; }
        };
        target.set(source);
        assert.strictEqual(log, "get0;convert0;get1;");
        target.set("34");
        assert.strictEqual(target.join(), "3,4");
        target.set([9], -0.5);
        assert.strictEqual(target[0], 9);
        """)]
    [InlineData("set-validation-order", """
        const target = new Uint8Array(1);
        const sentinel = {};
        const source = { get length() { throw sentinel; } };
        for (const offset of [2, Infinity, 2 ** 40]) {
          let caught;
          try { target.set(source, offset); } catch (error) { caught = error; }
          assert.strictEqual(caught, sentinel);
        }
        assert.throws(() => target.set(new BigInt64Array(0)), TypeError);
        const buffer = new ArrayBuffer(1);
        const detached = new Uint8Array(buffer);
        buffer.transfer();
        assert.throws(() => target.set(detached, Infinity), TypeError);
        assert.throws(() => detached.set([], Infinity), TypeError);
        """)]
    [InlineData("set-no-cached-values", """
        const target = new Uint8Array([1, 2]);
        const source = [{ valueOf() { source[1] = 9; return 7; } }, 3];
        target.set(source);
        assert.strictEqual(target.join(), "7,9");
        const overflow = { length: 3, get 0() { throw new Error("must check length first"); } };
        assert.throws(() => target.set(overflow), RangeError);
        const buffer = new ArrayBuffer(2, { maxByteLength: 2 });
        const view = new Uint8Array(buffer);
        let conversions = 0;
        view.set({ length: 2, get 0() { buffer.resize(0); return 1; },
          get 1() { return { valueOf() { conversions++; return 2; } }; } });
        assert.strictEqual(conversions, 1);
        """)]
    [InlineData("slice-empty-species", """
        const buffer = new ArrayBuffer(8);
        const source = new Uint8Array(buffer);
        let calls = 0;
        source.constructor = { [Symbol.species]: function(length) {
          calls++;
          buffer.transfer();
          return new Uint8Array(length);
        } };
        assert.strictEqual(source.slice(0, 0).length, 0);
        assert.strictEqual(calls, 1);
        """)]
    [InlineData("raw-bits-and-overlap", """
        const buffer = new ArrayBuffer(8);
        const bits = new Uint32Array(buffer);
        bits[0] = 0x7f800001;
        bits[1] = 0x80000000;
        const source = new Float32Array(buffer);
        assert.strictEqual(new Uint32Array(source.slice().buffer)[0], 0x7f800001);
        const target = new Float32Array(2);
        target.set(source);
        assert.strictEqual(new Uint32Array(target.buffer)[0], 0x7f800001);
        source.reverse();
        assert.strictEqual(bits[1], 0x7f800001);
        assert.strictEqual(bits[0], 0x80000000);
        const overlap = new Uint8Array([1, 2, 3, 4]);
        overlap.constructor = { [Symbol.species]: function() {
          return new Uint8Array(overlap.buffer, 1, 3);
        } };
        assert.strictEqual(overlap.slice(0, 3).join(), "1,1,1");
        const copied = new Uint8Array([1, 2, 3, 4]);
        copied.set(new Uint8Array(copied.buffer, 0, 3), 1);
        assert.strictEqual(copied.join(), "1,1,2,3");
        """)]
    [InlineData("iterator-completion", """
        for (const method of ["keys", "values", "entries"]) {
          for (const generic of [false, true]) {
            const buffer = new ArrayBuffer(1, { maxByteLength: 2 });
            const view = new Uint8Array(buffer);
            const iterator = generic ? Array.prototype[method].call(view) : view[method]();
            iterator.next();
            assert.strictEqual(iterator.next().done, true);
            buffer.resize(2);
            assert.strictEqual(iterator.next().done, true);
            buffer.transfer();
            assert.strictEqual(iterator.next().done, true);
            const rab = new ArrayBuffer(2, { maxByteLength: 2 });
            const fixed = new Uint8Array(rab, 0, 2);
            const failed = generic ? Array.prototype[method].call(fixed) : fixed[method]();
            rab.resize(0);
            assert.throws(() => failed.next(), TypeError);
            rab.resize(2);
            assert.strictEqual(failed.next().done, true);
          }
        }
        """)]
    [InlineData("generic-iterator-abrupt-and-reentrant", """
        let iterator;
        let reads = 0;
        const receiver = { get length() {
          reads++;
          assert.throws(() => iterator.next(), TypeError);
          return 1;
        }, 0: 42 };
        iterator = Array.prototype.values.call(receiver);
        assert.strictEqual(iterator.next().value, 42);
        assert.strictEqual(iterator.next().done, true);
        assert.strictEqual(iterator.next().done, true);
        assert.strictEqual(reads, 2);
        const sentinel = {};
        const throwing = Array.prototype.values.call({ length: 1, get 0() { throw sentinel; } });
        let caught;
        try { throwing.next(); } catch (error) { caught = error; }
        assert.strictEqual(caught, sentinel);
        assert.strictEqual(throwing.next().done, true);
        """)]
    [InlineData("search-and-join-arguments", """
        const view = new Uint8Array([5, 5]);
        assert.strictEqual(view.lastIndexOf(5), 1);
        assert.strictEqual(view.lastIndexOf(5, undefined), 0);
        assert.strictEqual(view.lastIndexOf(5, null), 0);
        const empty = new Uint8Array(0);
        let calls = 0;
        assert.strictEqual(empty.join({ toString() { calls++; return "-"; } }), "");
        assert.strictEqual(calls, 1);
        assert.throws(() => empty.join(Symbol()), TypeError);
        view.buffer.transfer();
        assert.throws(() => view.includes(), TypeError);
        """)]
    [InlineData("locale-dispatch", """
        assert.strictEqual(["", ""].toLocaleString(), ",");
        assert.strictEqual(Object.prototype.toLocaleString.call({ toString() { return "custom"; } }), "custom");
        assert.throws(() => Object.prototype.toLocaleString.call({ toString: 3 }), TypeError);
        const locales = {};
        const options = {};
        const element = { toLocaleString() {
          assert.strictEqual(this, element);
          assert.strictEqual(arguments.length, 2);
          assert.strictEqual(arguments[0], locales);
          assert.strictEqual(arguments[1], options);
          return "local";
        } };
        assert.strictEqual([element, null, undefined].toLocaleString(locales, options, 123), "local,,");
        assert.strictEqual(Array.prototype.toLocaleString.call([element], locales, options), "local");
        const original = Number.prototype.toLocaleString;
        try {
          const buffer = new ArrayBuffer(3, { maxByteLength: 3 });
          const view = new Uint8Array(buffer);
          view.set([1, 2, 3]);
          let calls = 0;
          Object.defineProperty(view, "length", { get() { throw new Error("own length"); } });
          Number.prototype.toLocaleString = function() {
            assert.strictEqual(arguments.length, 2);
            assert.strictEqual(arguments[0], undefined);
            assert.strictEqual(arguments[1], undefined);
            calls++;
            buffer.resize(0);
            return "first";
          };
          assert.strictEqual(view.toLocaleString(), "first,,");
          assert.strictEqual(calls, 1);
        } finally {
          Number.prototype.toLocaleString = original;
        }
        """)]
    [InlineData("prototype-methods", """
        const prototype = Object.getPrototypeOf(Uint8Array.prototype);
        for (const pair of [["set", 1], ["slice", 2], ["indexOf", 1], ["lastIndexOf", 1], ["join", 1], ["reverse", 0]]) {
          const descriptor = Object.getOwnPropertyDescriptor(prototype, pair[0]);
          assert.strictEqual(typeof descriptor.value, "function");
          assert.strictEqual(descriptor.value.length, pair[1]);
          assert.strictEqual(descriptor.value.name, pair[0]);
          assert.strictEqual(descriptor.enumerable, false);
          assert.strictEqual(descriptor.writable, true);
          assert.strictEqual(descriptor.configurable, true);
          assert.throws(() => descriptor.value.call({}), TypeError);
        }
        const view = new Uint8Array([5, 5]);
        assert.strictEqual(prototype.lastIndexOf.call(view, 5), 1);
        assert.strictEqual(prototype.lastIndexOf.call(view, 5, undefined), 0);
        assert.strictEqual(prototype.slice.call(view, 1).join(), "5");
        """)]
    [InlineData("map-invalidated-species", """
        const source = new Uint8Array([1, 2]);
        const buffer = new ArrayBuffer(2, { maxByteLength: 2 });
        const target = new Uint8Array(buffer);
        source.constructor = { [Symbol.species]: function() { return target; } };
        let calls = 0;
        source.map(value => ({ valueOf() {
          calls++;
          buffer.resize(0);
          return value;
        } }));
        assert.strictEqual(calls, 2);
        const bigint = new BigInt64Array([1n]);
        const bigintTarget = new BigInt64Array(1);
        bigint.constructor = { [Symbol.species]: function() { return bigintTarget; } };
        assert.throws(() => bigint.map(() => {
          bigintTarget.buffer.transfer();
          return 1;
        }), TypeError);
        """)]
    public void ReviewedMethods_PreserveObservableSemantics(string name, string script)
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            $"typed-array-review-{name}",
            "TypedArray.SpecReview",
            _ => ("const assert = require('assert');\n" + script, null));
        Assert.Equal(string.Empty, result.Output);
    }

    [Fact]
    public void BigIntTypedArray_Reverse_PreservesBigIntElements()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "bigint-typed-array-reverse",
            "TypedArray.BigIntReverse",
            static _ => ("""
                const values = new BigInt64Array([1n, -2n, 3n]);
                const result = values.reverse();
                console.log(
                  (result === values) + ":" +
                  result[0] + ":" +
                  result[1] + ":" +
                  result[2]
                );
                """, null));

        Assert.Equal($"true:3:-2:1{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void BigIntTypedArray_SharedMethods_PreserveBigIntElements()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "bigint-typed-array-shared-methods",
            "TypedArray.BigIntSharedMethods",
            static _ => ("""
                const values = new BigInt64Array([1n, 2n, 3n]);
                console.log(Array.from(values.toReversed()).join(","));
                console.log(Array.from(values.map(value => value + 1n)).join(","));
                console.log(Array.from(values.filter(value => value > 1n)).join(","));
                console.log(values.reduce((sum, value) => sum + value));
                console.log(Array.from(values.with(1, 9n)).join(","));

                const filled = new BigUint64Array(2);
                filled.fill(5n);
                console.log(Array.from(filled).join(","));
                console.log(values.includes(2n) + ":" + values.includes(2));
                console.log(values.indexOf(3n) + ":" + values.indexOf(3));
                """, null));

        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "3,2,1",
                "2,3,4",
                "2,3",
                "6",
                "1,9,3",
                "5,5",
                "true:false",
                "2:-1",
                string.Empty),
            result.Output);
    }

    [Fact]
    public void TypedArraySubarray_AcceptsOutOfBoundsSpeciesResult()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "typed-array-subarray-out-of-bounds-species-result",
            "TypedArray.SubarrayOutOfBoundsSpeciesResult",
            static _ => ("""
                const buffer = new ArrayBuffer(8, { maxByteLength: 8 });
                const source = new Uint8Array(buffer, 0, 4);
                let speciesResult;

                source.constructor = {};
                source.constructor[Symbol.species] = function() {
                  speciesResult = new Uint8Array(buffer, 0, 4);
                  buffer.resize(0);
                  return speciesResult;
                };

                console.log(source.subarray() === speciesResult);
                console.log(speciesResult.length);
                """, null));

        Assert.Equal(
            string.Join(Environment.NewLine, "true", "0", string.Empty),
            result.Output);
    }

    [Fact]
    public void TypedArraySet_CoercesOffsetBeforeValidatingResizedTarget()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "typed-array-set-resizable-offset-order",
            "TypedArray.SetResizableOffsetOrder",
            static _ => ("""
                const buffer = new ArrayBuffer(4, { maxByteLength: 4 });
                const values = new Uint8Array(buffer, 0, 4);
                const offset = {
                  valueOf() {
                    buffer.resize(0);
                    return 0;
                  }
                };

                try {
                  values.set([], offset);
                  console.log("no throw");
                } catch (error) {
                  console.log(error instanceof TypeError);
                }
                """, null));

        Assert.Equal($"true{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void TypedArraySort_IgnoresWritesAfterComparatorShrinksBuffer()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "typed-array-sort-resizable-write",
            "TypedArray.SortResizableWrite",
            static _ => ("""
                const buffer = new ArrayBuffer(4, { maxByteLength: 4 });
                const values = new Uint8Array(buffer);
                values.set([4, 3, 2, 1]);

                values.sort((left, right) => {
                  buffer.resize(0);
                  return left - right;
                });

                console.log(values.length);
                """, null));

        Assert.Equal($"0{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void TypedArraySlice_ReclampsSourceAfterSpeciesResizesBuffer()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "typed-array-slice-resizable-species",
            "TypedArray.SliceResizableSpecies",
            static _ => ("""
                const buffer = new ArrayBuffer(32, { maxByteLength: 32 });
                const source = new Float64Array(buffer);
                source.set([1, 2, 3, 4]);
                source.constructor = {};
                source.constructor[Symbol.species] = function() {
                  buffer.resize(16);
                  return new Float64Array(4);
                };

                console.log(Array.from(source.slice()).join(","));
                """, null));

        Assert.Equal($"1,2,0,0{Environment.NewLine}", result.Output);
    }
}
