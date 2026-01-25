using System;
using System.Dynamic;
using System.Numerics;

namespace JavaScriptRuntime
{
    public static class TypeUtilities
    {
        /// <summary>
        /// JavaScript constructor semantics: if a constructor explicitly returns an object (or function),
        /// the result of <c>new C()</c> is that value; otherwise the constructed instance is used.
        /// In this runtime, primitives are typically boxed value types (double/bool/etc) or string;
        /// JS null is represented as <see cref="JsNull"/> and JS undefined is CLR null.
        /// </summary>
        public static bool IsConstructorReturnOverride(object? value)
        {
            // undefined / null never override
            if (value is null) return false;
            if (value is JsNull) return false;

            // JS primitives never override
            if (value is string) return false;
            if (value.GetType().IsValueType) return false;

            // Any remaining reference type (ExpandoObject, Array, Delegate, host objects, etc.) counts as an object.
            return true;
        }

        // JS ToNumber coercion used by codegen slow paths when operand type is uncertain
        public static double ToNumber(object? value)
        {
            // JS ToNumber(undefined) => NaN (undefined is represented as CLR null)
            if (value == null) return double.NaN;
            switch (value)
            {
                case double d: return d;
                case float f: return (double)f;
                case int i: return i;
                case long l: return l;
                case short s: return s;
                case byte b: return b;
                case bool bo: return bo ? 1d : 0d;
                case string str:
                    {
                        var trimmed = str.Trim();
                        if (trimmed.Length == 0) return 0d;
                        if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            if (long.TryParse(trimmed.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out var hex))
                                return (double)hex;
                            return double.NaN;
                        }
                        return double.TryParse(trimmed, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
                            ? parsed
                            : double.NaN;
                    }
                case JsNull: return 0d; // JS ToNumber(null) => +0 (JsNull represents JS null)
            }
            try
            {
                return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return double.NaN;
            }
        }
        // Minimal typeof implementation for our runtime object shapes
        public static string Typeof(object? value)
        {
            // JS: typeof null === 'object'; in our model CLR null is 'undefined' and JsNull represents JS null.
            if (value is null) return "undefined";
            switch (value)
            {
                case string: return "string";
                case bool: return "boolean";
                case double: return "number";
                case float: return "number";
                case int: return "number";
                case long: return "number";
                case short: return "number";
                case byte: return "number";
                case BigInteger: return "bigint";
                case JsNull: return "object"; // JS null reports as 'object'
                case JavaScriptRuntime.Symbol: return "symbol";
            }
            if (value is ExpandoObject) return "object";
            if (value is Array) return "object";
            // Functions are delegates in our model; detect common delegate base
            if (value is Delegate) return "function";
            return "object";
        }

        // JS ToBoolean coercion used in conditional tests and logical contexts
        public static bool ToBoolean(double value)
        {
            // JavaScript uses IEEE 754 semantics: 0, -0, and NaN are falsy
            return value != 0.0 && !double.IsNaN(value);
        }

        public static bool ToBoolean(object? value)
        {
            // undefined (CLR null) and null => false
            if (value is null) return false;
            switch (value)
            {
                case bool b:
                    return b;
                case JsNull:
                    return false;
                case string s:
                    return s.Length != 0;
                case double d:
                    return ToBoolean(d);
                case float f:
                    return f != 0.0f && !float.IsNaN(f);
                case int i:
                    return i != 0;
                case long l:
                    return l != 0;
                case short sh:
                    return sh != 0;
                case byte by:
                    return by != 0;
            }
            // Objects (including arrays, functions, expando) are truthy
            return true;
        }
    }
}
