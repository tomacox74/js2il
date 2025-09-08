using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace JavaScriptRuntime
{
    public class DotNet2JSConversions
    {
        public static string ToString(object? value)
        {
            // In our runtime: CLR null represents JavaScript 'undefined'
            if (value == null)
            {
                return "undefined";
            }
            // JavaScript 'null' is represented by the JsNull enum
            if (value is JsNull)
            {
                return "null";
            }
            if (value is string strValue)
            {
                return strValue;
            }
            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            if (value is ExpandoObject expandObject)
            {
                string propertyValues = string.Join(", ", expandObject
                    .Select(kvp =>
                    {
                        var value = kvp.Value is string ? $"'{kvp.Value}'" : ToString(kvp.Value);

                        return $"{kvp.Key}: {value}";
                     }));

                return string.Format("{{ {0} }}", propertyValues);
            }

            // Numbers: normalize to JS-like string forms using invariant culture and exact tokens
            if (value is double dd)
            {
                if (double.IsNaN(dd)) return "NaN";
                if (double.IsPositiveInfinity(dd)) return "Infinity";
                if (double.IsNegativeInfinity(dd)) return "-Infinity";
                // Preserve -0
                if (dd == 0.0 && double.IsNegative(dd)) return "-0";

                // Cross-platform stabilization: if a finite non-zero double is extremely close
                // to an integer (common from transcendental ops on some runtimes), format it
                // as an integer to match JS Number#toString minimal representation.
                // Avoid snapping to zero to preserve tiny magnitudes like 1e-16.
                if (!double.IsNaN(dd) && !double.IsInfinity(dd))
                {
                    double nearest = global::System.Math.Round(dd);
                    if (nearest != 0.0)
                    {
                        double delta = global::System.Math.Abs(dd - nearest);
                        // A conservative epsilon for double near-unit magnitudes
                        if (delta <= 1e-12)
                        {
                            return nearest.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
                return dd.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            if (value is float ff)
            {
                if (float.IsNaN(ff)) return "NaN";
                if (float.IsPositiveInfinity(ff)) return "Infinity";
                if (float.IsNegativeInfinity(ff)) return "-Infinity";
                if (ff == 0f && global::System.Math.CopySign(1f, ff) < 0f) return "-0";
                // Stabilize near-integer formatting for floats too (but don't snap to zero)
                if (!float.IsNaN(ff) && !float.IsInfinity(ff))
                {
                    float nearest = (float)global::System.Math.Round(ff);
                    if (nearest != 0f)
                    {
                        float delta = global::System.Math.Abs(ff - nearest);
                        if (delta <= 1e-6f)
                        {
                            return ((double)nearest).ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
                return ff.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Add more conversions as needed
            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }
            return value!.ToString()!;
        }
    }
}
