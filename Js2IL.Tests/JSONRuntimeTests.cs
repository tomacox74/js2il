using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;
using JSONRuntime = JavaScriptRuntime.JSON;
using JsArray = JavaScriptRuntime.Array;

namespace Js2IL.Tests
{
    public class JSONRuntimeTests
    {
        [Fact]
        public void JSON_Parse_Object_Primitives_And_Null()
        {
            var input = "{\"a\":1,\"b\":true,\"c\":null}";
            var result = JSONRuntime.Parse(input);

            var obj = Assert.IsAssignableFrom<ExpandoObject>(result);
            var dict = (IDictionary<string, object?>)obj;
            Assert.Equal(1d, dict["a"]);
            Assert.Equal(true, dict["b"]);
            Assert.Null(dict["c"]);
        }

        [Fact]
        public void JSON_Parse_Array_MixedTypes()
        {
            var input = "[1,true,\"x\",null]";
            var result = JSONRuntime.Parse(input);

            var arr = Assert.IsType<JsArray>(result);
            Assert.Equal(4, arr.length);
            Assert.Equal(1d, arr[0]);
            Assert.Equal(true, arr[1]);
            Assert.Equal("x", arr[2]);
            Assert.Null(arr[3]);
        }

        [Fact]
        public void JSON_Parse_Nested_Object_And_Array()
        {
            var input = "{\"o\":{\"x\":2},\"arr\":[1,2,3]}";
            var result = JSONRuntime.Parse(input);

            var obj = Assert.IsAssignableFrom<ExpandoObject>(result);
            var dict = (IDictionary<string, object?>)obj;

            var inner = Assert.IsAssignableFrom<ExpandoObject>(dict["o"]);
            var innerDict = (IDictionary<string, object?>)inner;
            Assert.Equal(2d, innerDict["x"]);

            var arr = Assert.IsType<JsArray>(dict["arr"]);
            Assert.Equal(3, arr.length);
            Assert.Equal(1d, arr[0]);
            Assert.Equal(2d, arr[1]);
            Assert.Equal(3d, arr[2]);
        }

        [Fact]
        public void JSON_Parse_Invalid_Throws_SyntaxError()
        {
            var input = "{\"a\":1"; // missing closing brace
            var ex = Assert.Throws<JavaScriptRuntime.SyntaxError>(() => JSONRuntime.Parse(input));
            Assert.False(string.IsNullOrWhiteSpace(ex.Message));
        }

        [Fact]
        public void JSON_Parse_NonString_Throws_TypeError()
        {
            var ex = Assert.Throws<JavaScriptRuntime.TypeError>(() => JSONRuntime.Parse(123));
            Assert.Contains("JSON.parse", ex.Message);
        }
    }
}
