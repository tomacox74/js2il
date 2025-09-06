using System;
using System.Collections.Generic;
using System.Linq;
using JavaScriptRuntime;
using Xunit;

namespace Js2IL.Tests.Array
{
    public class RuntimeSliceTests
    {
        private static JavaScriptRuntime.Array Arr(params object[] items)
        {
            var a = new JavaScriptRuntime.Array(items.Length);
            foreach (var it in items) a.Add(it);
            return a;
        }

        private static double[] D(params double[] nums) => nums;

        private static void AssertSequence(JavaScriptRuntime.Array result, double[] expected)
        {
            Assert.Equal(expected.Length, result.Count);
            for (int i = 0; i < expected.Length; i++)
            {
                // Elements are boxed numbers (double) in our runtime
                var v = result[i];
                Assert.IsType<double>(v);
                Assert.Equal(expected[i], (double)v, precision: 10);
            }
        }

        [Fact]
        public void Slice_Basic_Cases()
        {
            var arr = Arr(0d, 1d, 2d, 3d, 4d, 5d);

            AssertSequence((JavaScriptRuntime.Array)arr.slice(), D(0, 1, 2, 3, 4, 5));
            AssertSequence((JavaScriptRuntime.Array)arr.slice(2d), D(2, 3, 4, 5));
            AssertSequence((JavaScriptRuntime.Array)arr.slice(2d, 4d), D(2, 3));
            AssertSequence((JavaScriptRuntime.Array)arr.slice(-2d), D(4, 5));
            AssertSequence((JavaScriptRuntime.Array)arr.slice(1d, -1d), D(1, 2, 3, 4));
            AssertSequence((JavaScriptRuntime.Array)arr.slice(10d), System.Array.Empty<double>());
            AssertSequence((JavaScriptRuntime.Array)arr.slice(-10d, 2d), D(0, 1));
        }

        [Fact]
        public void Slice_NullVsUndefined_EndHandling()
        {
            var arr = Arr(0d, 1d, 2d, 3d);

            // end undefined (C# null) => default to len
            var s1 = (JavaScriptRuntime.Array)arr.slice(new object[] { 1d, null! });
            AssertSequence(s1, D(1, 2, 3));

            // end JS null => +0
            var s2 = (JavaScriptRuntime.Array)arr.slice(new object[] { 1d, (JsNull)JsNull.Null });
            AssertSequence(s2, System.Array.Empty<double>());

            // start JS null => +0; no end provided => default len
            var s3 = (JavaScriptRuntime.Array)arr.slice(new object[] { (JsNull)JsNull.Null });
            AssertSequence(s3, D(0, 1, 2, 3));
        }

        [Fact]
        public void Slice_End_NaN_And_NonNumeric_CoerceToZero()
        {
            var arr = Arr(0d, 1d, 2d, 3d);

            // end = NaN => +0
            var s1 = (JavaScriptRuntime.Array)arr.slice(new object[] { 1d, double.NaN });
            AssertSequence(s1, System.Array.Empty<double>());

            // end = non-numeric string => +0
            var s2 = (JavaScriptRuntime.Array)arr.slice(new object[] { 1d, "foo" });
            AssertSequence(s2, System.Array.Empty<double>());
        }

        [Fact]
        public void Slice_Both_Null_Arguments_CoerceTo_Empty()
        {
            var arr = Arr(0d, 1d, 2d, 3d);

            // start JS null => 0; end JS null => 0 => empty
            var s = (JavaScriptRuntime.Array)arr.slice(new object[] { (JsNull)JsNull.Null, (JsNull)JsNull.Null });
            AssertSequence(s, System.Array.Empty<double>());
        }

        [Fact]
        public void Slice_DoesNotMutate_And_IsShallow()
        {
            var shared = new Dictionary<string, object?> { ["a"] = 1d };
            var arr = Arr(0d, shared, 2d);

            var beforeCount = arr.Count;
            var slice = (JavaScriptRuntime.Array)arr.slice(1d, 2d);

            // Original unchanged
            Assert.Equal(beforeCount, arr.Count);
            Assert.Equal(0d, (double)arr[0]);

            // Shallow copy: same reference for object elements
            Assert.Single(slice);
            Assert.Same(shared, slice[0]);
        }
    }
}
