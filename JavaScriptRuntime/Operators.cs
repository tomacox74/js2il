using System;
using System.Globalization;

namespace JavaScriptRuntime
{
    public static class Operators
    {
        private static double ToNumber(object? value)
        {
            // JS ToNumber(undefined) => NaN (undefined is represented as CLR null)
            if (value == null)
                return double.NaN;
            switch (value)
            {
                case double d:
                    return d;
                case float f:
                    return (double)f;
                case int i:
                    return i;
                case long l:
                    return l;
                case short s:
                    return s;
                case byte b:
                    return b;
                case bool bo:
                    return bo ? 1d : 0d;
                case string str:
                    // JS ToNumber on strings: trim; empty -> 0; 0x.. hex allowed; otherwise parse as float
                    var trimmed = str.Trim();
                    if (trimmed.Length == 0)
                        return 0d;
                    if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        if (long.TryParse(trimmed.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var hex))
                            return (double)hex;
                        return double.NaN;
                    }
                    return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : double.NaN;
                case JsNull:
                    return 0d;
            }
            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return double.NaN;
            }
        }
        /// <summary>
        /// Implements JavaScript '+' semantics for two operands boxed as objects.
        /// - If either operand is a string, both are coerced to string and concatenated.
        /// - Otherwise, both are coerced to numbers and added (as double).
        /// This is a minimal subset sufficient for current tests (strings and numbers).
        /// </summary>
        public static object Add(object? a, object? b)
        {
            // If either is a string, concatenate string representations
            if (a is string || b is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            // Try numeric addition (JavaScript uses double-precision floats)
            // Accept common numeric types and convert to double
            try
            {
                double da = ToNumber(a);
                double db = ToNumber(b);
                return da + db; // boxed double
            }
            catch
            {
                // Fallback: concat string representations (closest to JS when non-numeric)
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }
        }

        /// <summary>
        /// Implements JavaScript '+' semantics where the left operand is already an unboxed double.
        /// Avoids boxing the double in common numeric hot paths.
        /// </summary>
        public static object Add(double a, object? b)
        {
            if (b is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            var db = ToNumber(b);
            return a + db; // boxed double
        }

        /// <summary>
        /// Implements JavaScript '+' semantics where the right operand is already an unboxed double.
        /// Avoids boxing the double in common numeric hot paths.
        /// </summary>
        public static object Add(object? a, double b)
        {
            if (a is string)
            {
                var sa = DotNet2JSConversions.ToString(a);
                var sb = DotNet2JSConversions.ToString(b);
                return string.Concat(sa, sb);
            }

            var da = ToNumber(a);
            return da + b; // boxed double
        }

        /// <summary>
        /// Implements JavaScript '-' semantics. Both operands are coerced to numbers; result is a double (may be NaN).
        /// </summary>
        public static object Subtract(object? a, object? b)
        {
            var da = ToNumber(a);
            var db = ToNumber(b);
            return da - db;
        }

        /// <summary>
        /// Implements JavaScript '*' semantics. Both operands are coerced to numbers; result is a double (may be NaN).
        /// </summary>
        public static object Multiply(object? a, object? b)
        {
            var da = ToNumber(a);
            var db = ToNumber(b);
            return da * db;
        }

        /// <summary>
        /// Implements JavaScript '==' (loose equality) semantics.
        /// </summary>
        public static bool Equal(object? a, object? b)
        {
            // Identical references
            if (ReferenceEquals(a, b))
                return true;

            // JavaScript null/undefined special-case: null == undefined is true,
            // but null/undefined are not equal to any other value.
            var aIsNullish = a == null || a is JsNull;
            var bIsNullish = b == null || b is JsNull;
            if (aIsNullish || bIsNullish)
                return aIsNullish && bIsNullish;

            // Type coercion: compare as numbers if both can be numeric
            // Note: JavaScript uses IEEE 754 floating-point comparison semantics where
            // direct equality (==) is intentional and matches JS behavior (NaN != NaN, etc.)
            if (a is double da && b is double db)
                return da == db;

            if (a is int ia && b is int ib)
                return ia == ib;

            if (a is bool ba && b is bool bb)
                return ba == bb;

            // String comparison
            if (a is string sa && b is string sb)
                return sa == sb;

            // Mixed types: numeric comparison when both are convertible
            if (CanConvertToNumber(a) && CanConvertToNumber(b))
            {
                var na = ToNumber(a);
                var nb = ToNumber(b);
                return na == nb;
            }

            return false;
        }

        /// <summary>
        /// Checks if a value can be converted to a number without throwing.
        /// </summary>
        private static bool CanConvertToNumber(object? value)
        {
            return value == null
                   || value is JsNull
                   || value is bool
                   || value is int
                   || value is double
                   || value is string;
        }

        /// <summary>
        /// Implements JavaScript '!=' (loose inequality) semantics.
        /// </summary>
        public static bool NotEqual(object? a, object? b)
        {
            return !Equal(a, b);
        }

        /// <summary>
        /// Implements JavaScript '===' (strict equality) semantics.
        /// </summary>
        public static bool StrictEqual(object? a, object? b)
        {
            // Identical references
            if (ReferenceEquals(a, b))
                return true;

            // Must be same type
            if (a == null || b == null)
                return a == null && b == null;

            if (a.GetType() != b.GetType())
            {
                // Special case: int and double can be compared
                // Note: JavaScript uses IEEE 754 floating-point comparison semantics
                if ((a is double || a is int) && (b is double || b is int))
                {
                    return ToNumber(a) == ToNumber(b);
                }
                return false;
            }

            // Same type, value comparison
            return a.Equals(b);
        }

        /// <summary>
        /// Implements JavaScript '!==' (strict inequality) semantics.
        /// </summary>
        public static bool StrictNotEqual(object? a, object? b)
        {
            return !StrictEqual(a, b);
        }

        /// <summary>
        /// Implements JavaScript 'in' operator. Checks if property exists in object.
        /// </summary>
        /// <remarks>
        /// Performance note: For objects that are not dictionaries, arrays, or ExpandoObject,
        /// this method falls back to reflection which can be slow. The reflection results
        /// are not cached, so avoid using 'in' operator in hot paths with CLR objects.
        /// Consider using dictionary-based objects for performance-critical code.
        /// </remarks>
        public static bool In(object? property, object? obj)
        {
            if (obj == null)
                return false;

            // Proxy has trap
            if (obj is JavaScriptRuntime.Proxy proxy)
            {
                // Convert property to string key (minimal; symbols not yet surfaced here)
                var proxyPropName = DotNet2JSConversions.ToString(property);

                var hasTrap = JavaScriptRuntime.Object.GetProperty(proxy.Handler, "has");
                if (hasTrap is not null && hasTrap is not JsNull)
                {
                    var prev = RuntimeServices.SetCurrentThis(proxy.Handler);
                    try
                    {
                        var trapResult = Closure.InvokeWithArgs(hasTrap, System.Array.Empty<object>(), new object?[] { proxy.Target, proxyPropName });
                        return TypeUtilities.ToBoolean(trapResult);
                    }
                    finally
                    {
                        RuntimeServices.SetCurrentThis(prev);
                    }
                }

                // Fallback: apply normal 'in' semantics to the proxy target.
                return In(proxyPropName, proxy.Target);
            }

            // Convert property to string
            var propName = DotNet2JSConversions.ToString(property);
            
            static bool HasOwn(object target, string name)
            {
                if (target is System.Collections.IDictionary dict)
                {
                    return dict.Contains(name);
                }

                // For arrays, check if index exists
                if (target is object?[] array)
                {
                    if (int.TryParse(name, out var index))
                    {
                        return index >= 0 && index < array.Length;
                    }
                    return false;
                }

                // For generic objects (ExpandoObject, dynamic objects)
                if (target is System.Dynamic.ExpandoObject expando)
                {
                    var dict2 = (System.Collections.Generic.IDictionary<string, object?>)expando;
                    return dict2.ContainsKey(name);
                }

                // Fallback: check using reflection (not cached - see performance note above)
                var type = target.GetType();
                var prop = type.GetProperty(name);
                if (prop != null)
                    return true;
                var field = type.GetField(name);
                return field != null;
            }

            if (HasOwn(obj, propName))
            {
                return true;
            }

            if (!JavaScriptRuntime.PrototypeChain.Enabled)
            {
                return false;
            }

            // Avoid allocating cycle-detection state for the common case where no prototype
            // has been assigned.
            var current = obj;
            var proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(proto, obj))
            {
                return false;
            }

            if (HasOwn(proto, propName))
            {
                return true;
            }

            current = proto;
            proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            var visited = new System.Collections.Generic.HashSet<object>(System.Collections.Generic.ReferenceEqualityComparer.Instance)
            {
                obj,
                current
            };

            while (true)
            {
                if (!visited.Add(proto))
                {
                    return false;
                }

                if (HasOwn(proto, propName))
                {
                    return true;
                }

                current = proto;
                proto = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Implements JavaScript <c>instanceof</c> operator.
        /// Minimal semantics: checks whether <paramref name="value"/>'s prototype chain contains <c>ctor.prototype</c>.
        /// </summary>
        public static bool InstanceOf(object? value, object? ctor)
        {
            // Primitives (including undefined/null) are never instances.
            if (value is null) return false;
            if (value is JsNull) return false;
            if (value is string) return false;
            if (value.GetType().IsValueType) return false;

            if (ctor is null || ctor is JsNull)
            {
                throw new TypeError("Right-hand side of 'instanceof' is not callable");
            }

            // Minimal: require a callable delegate-backed function value.
            if (ctor is not Delegate)
            {
                throw new TypeError("Right-hand side of 'instanceof' is not callable");
            }

            // Spec: let proto = ctor.prototype; if proto is not an object, throw.
            var proto = JavaScriptRuntime.Object.GetItem(ctor, "prototype");
            if (proto is null || proto is JsNull || proto is string || proto.GetType().IsValueType)
            {
                throw new TypeError("Function has non-object prototype in instanceof check");
            }

            if (!JavaScriptRuntime.PrototypeChain.Enabled)
            {
                // If prototype chains are not enabled/assigned, we cannot observe any inheritance.
                return false;
            }

            var current = value;
            var next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (next is null || next is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(next, proto))
            {
                return true;
            }

            if (ReferenceEquals(next, current))
            {
                return false;
            }

            current = next;
            next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
            if (next is null || next is JsNull)
            {
                return false;
            }

            var visited = new System.Collections.Generic.HashSet<object>(System.Collections.Generic.ReferenceEqualityComparer.Instance)
            {
                value,
                current
            };

            while (true)
            {
                if (!visited.Add(next))
                {
                    return false;
                }

                if (ReferenceEquals(next, proto))
                {
                    return true;
                }

                current = next;
                next = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(current);
                if (next is null || next is JsNull)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tests if a value is truthy according to JavaScript semantics.
        /// </summary>
        public static bool IsTruthy(double value)
        {
            // JavaScript uses IEEE 754 semantics: 0, -0, and NaN are falsy
            return value != 0 && !double.IsNaN(value);
        }

        public static bool IsTruthy(bool value) => value;

        public static bool IsTruthy(object? value)
        {
            if (value == null)
                return false;
            if (value is JsNull)
                return false;
            if (value is bool b)
                return b;
            // Note: JavaScript uses IEEE 754 semantics - 0, -0, and NaN are falsy
            if (value is double d)
                return d != 0 && !double.IsNaN(d);
            if (value is int i)
                return i != 0;
            if (value is string s)
                return s.Length > 0;
            // Objects are truthy
            return true;
        }
    }
}
