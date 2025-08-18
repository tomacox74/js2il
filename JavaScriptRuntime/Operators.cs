using System;

namespace JavaScriptRuntime
{
    public static class Operators
    {
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
                // Handle null as NaN-like; for our limited tests, treat null as 0
                // but realistically JS would coerce null to 0. We'll follow that.
                double da = a == null ? 0 : Convert.ToDouble(a);
                double db = b == null ? 0 : Convert.ToDouble(b);
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
    }
}
