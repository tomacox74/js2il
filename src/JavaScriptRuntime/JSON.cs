using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace JavaScriptRuntime
{
    [IntrinsicObject("JSON")]
    public static class JSON
    {
        // JSON.parse(text[, reviver])
        public static object? Parse(object? text)
            => Parse(text, null);

        public static object? Parse(object? text, object? reviver)
        {
            var s = DotNet2JSConversions.ToString(text);

            try
            {
                using var doc = JsonDocument.Parse(s ?? "undefined");
                var parsed = FromElement(doc.RootElement);
                if (reviver is not Delegate reviverFunction)
                {
                    return parsed;
                }

                var root = Object.CreateOrdinaryObject();
                root.SetBoxedValue(string.Empty, parsed);
                return InternalizeJsonProperty(root, string.Empty, reviverFunction);
            }
            catch (JsonException ex)
            {
                // Map JSON parsing failures to JavaScript SyntaxError
                throw new SyntaxError(ex.Message);
            }
        }

        private static object? InternalizeJsonProperty(object holder, string name, Delegate reviver)
        {
            var value = ObjectRuntime.GetItem(holder, name);
            if (value is Array array)
            {
                var length = (int)array.length;
                for (var i = 0; i < length; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    var revived = InternalizeJsonProperty(array, key, reviver);
                    if (revived is null)
                    {
                        ObjectRuntime.DeleteProperty(array, key);
                    }
                    else
                    {
                        ObjectRuntime.SetItem(array, key, revived);
                    }
                }
            }
            else if (value is not null and not JsNull
                     && value is not string
                     && value is not bool
                     && value is not double
                     && value is not float
                     && value is not int
                     && value is not long
                     && value is not decimal)
            {
                foreach (var key in Object.GetOwnEnumerableKeysInOrder(value))
                {
                    var revived = InternalizeJsonProperty(value, key, reviver);
                    if (revived is null)
                    {
                        ObjectRuntime.DeleteProperty(value, key);
                    }
                    else
                    {
                        ObjectRuntime.SetItem(value, key, revived);
                    }
                }
            }

            var previousThis = RuntimeServices.SetCurrentThis(holder);
            try
            {
                return Closure.InvokeWithArgs(reviver, System.Array.Empty<object>(), new object?[] { name, value });
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object? Stringify(object? value)
            => Stringify(value, null, null);

        public static object? Stringify(object? value, object? replacer)
            => Stringify(value, replacer, null);

        public static object? Stringify(object? value, object? replacer, object? space)
        {
            var propertyList = CreatePropertyList(replacer);
            var replacerFunction = replacer as Delegate;
            var gap = CreateGap(space);
            var holder = Object.CreateOrdinaryObject();
            ObjectRuntime.SetItem(holder, string.Empty, value);
            return SerializeProperty(holder, string.Empty, propertyList, replacerFunction, new HashSet<object>(ReferenceEqualityComparer.Instance), gap, string.Empty);
        }

        private static string CreateGap(object? space)
        {
            if (space is null || space is JsNull)
            {
                return string.Empty;
            }

            double numericSpace;
            if (Number.TryGetWrappedNumberValue(space, out var wrappedNumber))
            {
                numericSpace = wrappedNumber;
            }
            else if (space is double or float or int or long or short or byte or sbyte or uint or ulong or ushort)
            {
                numericSpace = TypeUtilities.ToNumber(space);
            }
            else
            {
                string? stringSpace = null;
                if (space is string s)
                {
                    stringSpace = s;
                }
                else if (PropertyDescriptorStore.TryGetOwn(space, String.StringDataPropertyName, out var stringData)
                    && stringData.Kind == JsPropertyDescriptorKind.Data)
                {
                    stringSpace = DotNet2JSConversions.ToString(stringData.Value);
                }

                return stringSpace is null
                    ? string.Empty
                    : stringSpace[..global::System.Math.Min(10, stringSpace.Length)];
            }

            if (!double.IsFinite(numericSpace) || numericSpace <= 0)
            {
                return string.Empty;
            }

            var spaceCount = (int)global::System.Math.Min(10d, global::System.Math.Floor(numericSpace));
            return new string(' ', spaceCount);
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
                key = ToJsonPropertyKeyString(item);
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
                key = ToJsonPropertyKeyString(numberValue);
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

        private static object? InvokeToJsonIfPresent(object? value, string key)
        {
            if (value is null
                || value is JsNull
                || value is Delegate
                || value is Symbol
                || value is string
                || value.GetType().IsValueType)
            {
                return value;
            }

            var toJson = ObjectRuntime.GetItem(value, "toJSON");
            if (toJson is not Delegate toJsonFunction)
            {
                return value;
            }

            var previousThis = RuntimeServices.SetCurrentThis(value);
            try
            {
                return Closure.InvokeWithArgs(toJsonFunction, System.Array.Empty<object>(), new object?[] { key });
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static void PushToStackOrThrowIfCircular(HashSet<object> stack, object value)
        {
            if (!stack.Add(value))
            {
                throw new TypeError("Converting circular structure to JSON");
            }
        }

        private static object? ApplyReplacer(Delegate? replacerFunction, object holder, string key, object? value)
        {
            if (replacerFunction is null)
            {
                return value;
            }

            var previousThis = RuntimeServices.SetCurrentThis(holder);
            try
            {
                return Closure.InvokeWithArgs(replacerFunction, System.Array.Empty<object>(), new object?[] { key, value });
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static string? SerializeProperty(
            object holder,
            string key,
            List<string>? propertyList,
            Delegate? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            var value = ObjectRuntime.GetItem(holder, key);
            value = InvokeToJsonIfPresent(value, key);
            value = ApplyReplacer(replacerFunction, holder, key, value);
            return SerializeValue(value, propertyList, replacerFunction, stack, gap, indent);
        }

        private static string? SerializeValue(
            object? value,
            List<string>? propertyList,
            Delegate? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
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
                return SerializeArray(array, propertyList, replacerFunction, stack, gap, indent);
            }

            return SerializeObject(value!, propertyList, replacerFunction, stack, gap, indent);
        }

        private static string SerializeNumber(double value)
        {
            if (!double.IsFinite(value))
            {
                return "null";
            }

            if (value == 0d)
            {
                return "0";
            }

            return DotNet2JSConversions.ToString(value);
        }

        private static string ToJsonPropertyKeyString(object? value)
        {
            if (value is double d && d == 0d)
            {
                return "0";
            }

            if (value is float f && f == 0f)
            {
                return "0";
            }

            return DotNet2JSConversions.ToString(value);
        }

        private static string SerializeArray(
            Array array,
            List<string>? propertyList,
            Delegate? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            PushToStackOrThrowIfCircular(stack, array);

            var length = (int)array.length;
            var items = new List<string>(length);
            var stepBack = indent;
            indent += gap;
            try
            {
                for (var i = 0; i < length; i++)
                {
                    items.Add(SerializeProperty(array, i.ToString(CultureInfo.InvariantCulture), propertyList, replacerFunction, stack, gap, indent) ?? "null");
                }
            }
            finally
            {
                stack.Remove(array);
            }

            if (items.Count == 0)
            {
                return "[]";
            }

            if (gap.Length > 0)
            {
                return "[\n" + indent + string.Join(",\n" + indent, items) + "\n" + stepBack + "]";
            }

            return "[" + string.Join(",", items) + "]";
        }

        private static string SerializeObject(
            object value,
            List<string>? propertyList,
            Delegate? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            PushToStackOrThrowIfCircular(stack, value);

            var keys = propertyList ?? Object.GetOwnEnumerableKeysInOrder(value);
            var parts = new List<string>();
            var stepBack = indent;
            indent += gap;
            try
            {
                foreach (var key in keys)
                {
                    if (!Object.hasOwn(value, key))
                    {
                        continue;
                    }

                    var serialized = SerializeProperty(value, key, propertyList, replacerFunction, stack, gap, indent);
                    if (serialized is null)
                    {
                        continue;
                    }

                    var separator = gap.Length > 0 ? ": " : ":";
                    parts.Add(Quote(key) + separator + serialized);
                }
            }
            finally
            {
                stack.Remove(value);
            }

            if (parts.Count == 0)
            {
                return "{}";
            }

            if (gap.Length > 0)
            {
                return "{\n" + indent + string.Join(",\n" + indent, parts) + "\n" + stepBack + "}";
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
                    var obj = Object.CreateOrdinaryObject();
                    foreach (var prop in el.EnumerateObject())
                    {
                        obj.SetBoxedValue(prop.Name, FromElement(prop.Value));
                    }
                    return obj;

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
