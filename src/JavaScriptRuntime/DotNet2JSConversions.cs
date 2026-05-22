using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

            // Handle Array (List<object?>) - should join elements with comma
            if (value is Array jsArray)
            {
                // Arrays convert to string using join(',')
                var items = new List<string>();
                foreach (var item in jsArray)
                {
                    if (item == null)
                    {
                        items.Add("");  // undefined becomes empty string in join
                    }
                    else if (item is JsNull)
                    {
                        items.Add("");  // null becomes empty string in join
                    }
                    else
                    {
                        items.Add(ToString(item));
                    }
                }
                return string.Join(",", items);
            }

            if (JavaScriptRuntime.Number.TryGetWrappedNumberValue(value, out var wrappedNumber))
            {
                return ToString(wrappedNumber);
            }

            if (PropertyDescriptorStore.TryGetOwn(value, JavaScriptRuntime.String.StringDataPropertyName, out var stringDataDescriptor)
                && stringDataDescriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                return ToString(stringDataDescriptor.Value);
            }

            if (!value.GetType().IsValueType
                && TypeUtilities.TryCoerceObjectToPrimitive(value, "string", out var primitive))
            {
                return ToString(primitive);
            }

            if (value is IDictionary<string, object?> dictObject)
            {
                return FormatObject(dictObject);
            }

            // Numbers: normalize to JS-like string forms using invariant culture and exact tokens
            if (value is double dd)
            {
                return NumberToString(dd);
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

            if (value is BigInteger bigInteger)
            {
                return bigInteger.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Add more conversions as needed
            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }
            return value!.ToString()!;
        }

        private static string NumberToString(double value)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "Infinity";
            if (double.IsNegativeInfinity(value)) return "-Infinity";
            if (value == 0.0) return double.IsNegative(value) ? "-0" : "0";

            var abs = global::System.Math.Abs(value);
            if (double.IsInteger(value))
            {
                if (abs >= 1e21)
                {
                    return NormalizeExponent(value.ToString("0.#################e+0", System.Globalization.CultureInfo.InvariantCulture));
                }

                if (abs >= 1e-6)
                {
                    return value.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            double nearest = global::System.Math.Round(value);
            if (nearest != 0.0 && global::System.Math.Abs(value - nearest) <= 1e-12 && global::System.Math.Abs(nearest) < 1e21)
            {
                return nearest.ToString("0", System.Globalization.CultureInfo.InvariantCulture);
            }

            return NormalizeExponent(value.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        }

        private static string NormalizeExponent(string value)
        {
            var normalized = value.Replace('E', 'e');
            var exponentIndex = normalized.IndexOf('e');
            if (exponentIndex < 0)
            {
                return normalized;
            }

            var mantissa = normalized[..exponentIndex].TrimEnd('0').TrimEnd('.');
            var exponent = normalized[(exponentIndex + 1)..];
            var sign = string.Empty;
            if (exponent.StartsWith("+", StringComparison.Ordinal) || exponent.StartsWith("-", StringComparison.Ordinal))
            {
                sign = exponent[..1];
                exponent = exponent[1..];
            }

            exponent = exponent.TrimStart('0');
            if (exponent.Length == 0)
            {
                exponent = "0";
            }

            return $"{mantissa}e{sign}{exponent}";
        }

        private static string FormatObject(IDictionary<string, object?> dict)
        {
            // Default object representation
            string propertyValues = string.Join(", ", dict
                .Select(kvp =>
                {
                    var value = kvp.Value is string ? $"'{kvp.Value}'" : ToString(kvp.Value);

                    return $"{kvp.Key}: {value}";
                }));

            return string.Format("{{ {0} }}", propertyValues);
        }
    }
}
