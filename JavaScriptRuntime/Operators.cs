using System;
using System.Globalization;

namespace JavaScriptRuntime
{
    public static class Operators
    {
        private static double ToNumber(object? value)
        {
            if (value == null)
                return 0d;
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
        /// Implements JavaScript '-' semantics. Both operands are coerced to numbers; result is a double (may be NaN).
        /// </summary>
        public static object Subtract(object? a, object? b)
        {
            var da = ToNumber(a);
            var db = ToNumber(b);
            return da - db;
        }
    }
}
