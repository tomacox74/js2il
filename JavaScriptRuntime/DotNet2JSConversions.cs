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
                return dd.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            if (value is float ff)
            {
                if (float.IsNaN(ff)) return "NaN";
                if (float.IsPositiveInfinity(ff)) return "Infinity";
                if (float.IsNegativeInfinity(ff)) return "-Infinity";
                if (ff == 0f && global::System.Math.CopySign(1f, ff) < 0f) return "-0";
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
