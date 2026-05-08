using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace JavaScriptRuntime
{
    [IntrinsicObject("JSON")]
    public static class JSON
    {
        // JSON.parse(text)
        public static object? Parse(object? text)
        {
            // Spec: If first argument is not a string, it should throw a TypeError
            if (text is not string s)
            {
                throw new TypeError("JSON.parse requires a string argument");
            }

            try
            {
                using var doc = JsonDocument.Parse(s);
                return FromElement(doc.RootElement);
            }
            catch (JsonException ex)
            {
                // Map JSON parsing failures to JavaScript SyntaxError
                throw new SyntaxError(ex.Message);
            }
        }

        public static object? Stringify(object? value)
            => Stringify(value, null, null);

        public static object? Stringify(object? value, object? replacer)
            => Stringify(value, replacer, null);

        public static object? Stringify(object? value, object? replacer, object? space)
        {
            var propertyList = CreatePropertyList(replacer);
            var holder = Object.CreateOrdinaryObject();
            ObjectRuntime.SetItem(holder, string.Empty, value);
            return SerializeProperty(holder, string.Empty, propertyList);
        }

        private static List<string>? CreatePropertyList(object? replacer)
        {
            if (replacer is null || replacer is JsNull || replacer is Delegate)
            {
                return null;
            }

            var isArrayLike = replacer is Array or System.Collections.IList;
            if (!isArrayLike && replacer is Proxy proxy)
            {
                var target = proxy.GetTarget("JSON.stringify replacer");
                isArrayLike = target is Array or System.Collections.IList;
            }

            if (!isArrayLike)
            {
                return null;
            }

            var length = global::System.Math.Max(0, TypeUtilities.ToInt32(ObjectRuntime.GetItem(replacer, "length")));
            var keys = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < length; i++)
            {
                var item = ObjectRuntime.GetItem(replacer, (double)i);
                if (!TryGetReplacerKey(item, out var key) || !seen.Add(key))
                {
                    continue;
                }

                keys.Add(key);
            }

            return keys;
        }

        private static bool TryGetReplacerKey(object? item, out string key)
        {
            key = string.Empty;
            if (item is null || item is JsNull || item is Symbol)
            {
                return false;
            }

            if (item is string || item.GetType().IsValueType)
            {
                key = DotNet2JSConversions.ToString(item);
                return true;
            }

            if (TryInvokeObjectToString(item, out var primitive))
            {
                if (primitive is null || primitive is JsNull || primitive is Symbol)
                {
                    return false;
                }

                key = DotNet2JSConversions.ToString(primitive);
                return true;
            }

            if (Number.TryGetWrappedNumberValue(item, out var numberValue))
            {
                key = DotNet2JSConversions.ToString(numberValue);
                return true;
            }

            if (PropertyDescriptorStore.TryGetOwn(item, String.StringDataPropertyName, out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                key = DotNet2JSConversions.ToString(descriptor.Value);
                return true;
            }

            return false;
        }

        private static bool TryInvokeObjectToString(object receiver, out object? result)
        {
            var toString = ObjectRuntime.GetItem(receiver, "toString");
            if (toString is not Delegate)
            {
                result = null;
                return false;
            }

            var previousThis = RuntimeServices.SetCurrentThis(receiver);
            try
            {
                result = Closure.InvokeWithArgs(toString, System.Array.Empty<object>(), System.Array.Empty<object?>());
                return result is null || result is JsNull || result is string || result is Symbol || result.GetType().IsValueType;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static string? SerializeProperty(object holder, string key, List<string>? propertyList)
        {
            var value = ObjectRuntime.GetItem(holder, key);
            return SerializeValue(value, propertyList);
        }

        private static string? SerializeValue(object? value, List<string>? propertyList)
        {
            switch (value)
            {
                case null:
                case Delegate:
                case Symbol:
                    return null;
                case JsNull:
                    return "null";
                case bool b:
                    return b ? "true" : "false";
                case string s:
                    return Quote(s);
                case double d:
                    return SerializeNumber(d);
                case float f:
                    return SerializeNumber(f);
                case int or long or short or byte:
                    return DotNet2JSConversions.ToString(value);
            }

            if (Number.TryGetWrappedNumberValue(value, out var wrappedNumber))
            {
                return SerializeNumber(wrappedNumber);
            }

            if (PropertyDescriptorStore.TryGetOwn(value!, String.StringDataPropertyName, out var stringData)
                && stringData.Kind == JsPropertyDescriptorKind.Data)
            {
                return Quote(DotNet2JSConversions.ToString(stringData.Value));
            }

            if (value is Boolean booleanObject)
            {
                return booleanObject.valueOf() ? "true" : "false";
            }

            if (value is Array array)
            {
                return SerializeArray(array, propertyList);
            }

            return SerializeObject(value!, propertyList);
        }

        private static string SerializeNumber(double value)
        {
            if (!double.IsFinite(value))
            {
                return "null";
            }

            return DotNet2JSConversions.ToString(value);
        }

        private static string SerializeArray(Array array, List<string>? propertyList)
        {
            var length = (int)array.length;
            var items = new List<string>(length);
            for (var i = 0; i < length; i++)
            {
                items.Add(SerializeValue(ObjectRuntime.GetItem(array, (double)i), propertyList) ?? "null");
            }

            return "[" + string.Join(",", items) + "]";
        }

        private static string SerializeObject(object value, List<string>? propertyList)
        {
            var keys = propertyList ?? Object.GetOwnEnumerableKeysInOrder(value);
            var parts = new List<string>();
            foreach (var key in keys)
            {
                if (!Object.hasOwn(value, key))
                {
                    continue;
                }

                var serialized = SerializeProperty(value, key, propertyList);
                if (serialized is null)
                {
                    continue;
                }

                parts.Add(Quote(key) + ":" + serialized);
            }

            return "{" + string.Join(",", parts) + "}";
        }

        private static string Quote(string value)
        {
            var builder = new StringBuilder(value.Length + 2);
            builder.Append('"');
            foreach (var ch in value)
            {
                builder.Append(ch switch
                {
                    '"' => "\\\"",
                    '\\' => "\\\\",
                    '\b' => "\\b",
                    '\f' => "\\f",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    _ when ch < ' ' => "\\u" + ((int)ch).ToString("x4", CultureInfo.InvariantCulture),
                    _ => ch.ToString()
                });
            }

            builder.Append('"');
            return builder.ToString();
        }

        private static object? FromElement(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    var expando = new ExpandoObject() as IDictionary<string, object?>;
                    foreach (var prop in el.EnumerateObject())
                    {
                        expando[prop.Name] = FromElement(prop.Value);
                    }
                    return (ExpandoObject)expando;

                case JsonValueKind.Array:
                    var arr = new Array();
                    foreach (var item in el.EnumerateArray())
                    {
                        arr.Add(FromElement(item)!);
                    }
                    return arr;

                case JsonValueKind.String:
                    return el.GetString();

                case JsonValueKind.Number:
                    // Use double to model JS number
                    return el.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                    // Represent JavaScript null distinctly from CLR null (undefined)
                    return JsNull.Null;

                default:
                    // JSON doesn't produce Undefined; treat anything else as null
                    return null;
            }
        }
    }
}
