using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class TypedArrayRuntimeTests
{
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
}
