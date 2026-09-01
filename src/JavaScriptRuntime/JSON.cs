using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace JavaScriptRuntime
{
    [IntrinsicObject("JSON")]
    public static class JSON
    {
        private sealed class RawJsonData
        {
            public RawJsonData(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }

        private sealed class JsonParseNode
        {
            private readonly Dictionary<string, JsonParseNode>? _children;

            public JsonParseNode(object? value, string source)
            {
                Value = value;
                Source = source;
            }

            public JsonParseNode(object value, Dictionary<string, JsonParseNode> children)
            {
                Value = value;
                _children = children;
            }

            public object? Value { get; }
            public string? Source { get; }

            public bool Matches(object? value)
                => Source is not null
                    ? Operators.SameValue(Value, value)
                    : ReferenceEquals(Value, value);

            public JsonParseNode? GetChild(string key)
                => _children is not null && _children.TryGetValue(key, out var child)
                    ? child
                    : null;
        }

        private static readonly ConditionalWeakTable<JsObject, RawJsonData> RawJsonObjects = new();

        // JSON.parse(text[, reviver])
        public static object? Parse(object? text)
            => Parse(text, null);

        public static object? Parse(object? text, object? reviver)
        {
            var s = DotNet2JSConversions.ToString(text);

            try
            {
                using var doc = JsonDocument.Parse(s ?? "undefined");
                var parsed = FromElement(doc.RootElement, out var parseNode);
                if (!CallableOperations.IsCallable(reviver))
                {
                    return parsed;
                }

                var root = ObjectRuntime.CreateOrdinaryObject();
                root.SetBoxedValue(string.Empty, parsed);
                return InternalizeJsonProperty(root, string.Empty, reviver!, parseNode);
            }
            catch (JsonException ex)
            {
                // Map JSON parsing failures to JavaScript SyntaxError
                throw new SyntaxError(ex.Message);
            }
        }

        private static object? InternalizeJsonProperty(
            object holder,
            string name,
            object reviver,
            JsonParseNode? parseNode)
        {
            var value = ObjectRuntime.GetItem(holder, name);
            if (parseNode is not null && !parseNode.Matches(value))
            {
                parseNode = null;
            }

            if (value is Array array)
            {
                var length = (int)array.length;
                for (var i = 0; i < length; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    var revived = InternalizeJsonProperty(
                        array,
                        key,
                        reviver,
                        parseNode?.GetChild(key));
                    if (revived is null)
                    {
                        // Spec: Perform ? val.[[Delete]](key) - a failed (non-configurable)
                        // deletion completes normally without throwing.
                        ObjectRuntime.DeletePropertyNonStrict(array, key);
                    }
                    else
                    {
                        // Spec: Perform ? CreateDataProperty(val, key, newElement) - a failed
                        // (e.g. non-configurable) definition completes normally without throwing.
                        ObjectRuntime.CreateDataProperty(array, key, revived);
                    }
                }
            }
            else if (!TypeUtilities.IsPrimitive(value))
            {
                foreach (var key in ObjectRuntime.GetOwnEnumerableKeysInOrder(value))
                {
                    var revived = InternalizeJsonProperty(
                        value,
                        key,
                        reviver,
                        parseNode?.GetChild(key));
                    if (revived is null)
                    {
                        // Spec: Perform ? val.[[Delete]](P) - a failed (non-configurable)
                        // deletion completes normally without throwing.
                        ObjectRuntime.DeletePropertyNonStrict(value, key);
                    }
                    else
                    {
                        // Spec: Perform ? CreateDataProperty(val, P, newElement) - a failed
                        // (e.g. non-configurable) definition completes normally without throwing.
                        ObjectRuntime.CreateDataProperty(value, key, revived);
                    }
                }
            }

            var context = ObjectRuntime.CreateOrdinaryObject();
            if (parseNode?.Source is string source)
            {
                context.SetBoxedValue("source", source);
            }

            return CallableOperations.Call3(reviver, holder, name, value, context);
        }

        public static object RawJSON(object? text)
        {
            if (text is Symbol)
            {
                throw new TypeError("Cannot convert a Symbol value to a string");
            }

            var jsonText = DotNet2JSConversions.ToString(text);
            if (string.IsNullOrEmpty(jsonText)
                || IsJsonWhitespace(jsonText[0])
                || IsJsonWhitespace(jsonText[^1]))
            {
                throw new SyntaxError("Invalid JSON text");
            }

            try
            {
                using var document = JsonDocument.Parse(jsonText);
                if (document.RootElement.ValueKind is JsonValueKind.Array or JsonValueKind.Object)
                {
                    throw new SyntaxError("JSON.rawJSON only accepts primitive JSON values");
                }
            }
            catch (JsonException ex)
            {
                throw new SyntaxError(ex.Message);
            }

            var result = new JsObject();
            PrototypeChain.SetPrototype(result, JsNull.Null);
            PropertyDescriptorStore.DefineOrUpdate(result, "rawJSON", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = jsonText
            });
            ObjectRuntime.freeze(result);
            RawJsonObjects.Add(result, new RawJsonData(jsonText));
            return result;
        }

        public static bool IsRawJSON(object? value)
            => value is JsObject rawJson && RawJsonObjects.TryGetValue(rawJson, out _);

        public static object? Stringify(object? value)
            => Stringify(value, null, null);

        public static object? Stringify(object? value, object? replacer)
            => Stringify(value, replacer, null);

        public static object? Stringify(object? value, object? replacer, object? space)
        {
            var propertyList = CreatePropertyList(replacer);
            var replacerFunction = CallableOperations.IsCallable(replacer)
                ? replacer
                : null;
            var gap = CreateGap(space);
            var holder = ObjectRuntime.CreateOrdinaryObject();
            PropertyDescriptorStore.DefineOrUpdate(holder, string.Empty, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = value
            });
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
                if (!TypeUtilities.TryCoerceObjectToPrimitive(space, "number", out var primitive))
                {
                    throw new TypeError("Cannot convert object to primitive value");
                }

                numericSpace = TypeUtilities.ToNumber(primitive);
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
                    if (!TypeUtilities.TryCoerceObjectToPrimitive(space, "string", out var primitive))
                    {
                        throw new TypeError("Cannot convert object to primitive value");
                    }

                    stringSpace = DotNet2JSConversions.ToString(primitive);
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
            if (replacer is null || replacer is JsNull || CallableOperations.IsCallable(replacer))
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

            if (item is string
                || item is double or float or int or long or short or byte
                    or sbyte or uint or ulong or ushort or decimal)
            {
                key = ToJsonPropertyKeyString(item);
                return true;
            }

            if (Number.TryGetWrappedNumberValue(item, out _))
            {
                key = DotNet2JSConversions.ToStringRejectingSymbols(item);
                return true;
            }

            if (PropertyDescriptorStore.TryGetOwn(item, String.StringDataPropertyName, out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                key = DotNet2JSConversions.ToStringRejectingSymbols(item);
                return true;
            }

            return false;
        }

        private static object? InvokeToJsonIfPresent(object? value, string key)
        {
            if (value is null
                || value is JsNull
                || CallableOperations.IsCallable(value)
                || value is Symbol
                || value is string
                || (value.GetType().IsValueType
                    && value is not global::System.Numerics.BigInteger))
            {
                return value;
            }

            var toJson = ObjectRuntime.GetItem(value, "toJSON");
            if (!CallableOperations.IsCallable(toJson))
            {
                return value;
            }

            return CallableOperations.Call1(toJson, value, key);
        }

        private static void PushToStackOrThrowIfCircular(HashSet<object> stack, object value)
        {
            if (!stack.Add(value))
            {
                throw new TypeError("Converting circular structure to JSON");
            }
        }

        private static object? ApplyReplacer(object? replacerFunction, object holder, string key, object? value)
        {
            if (replacerFunction is null)
            {
                return value;
            }

            return CallableOperations.Call2(replacerFunction, holder, key, value);
        }

        private static string? SerializeProperty(
            object holder,
            string key,
            List<string>? propertyList,
            object? replacerFunction,
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
            object? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            if (value is JsObject rawJson && RawJsonObjects.TryGetValue(rawJson, out var rawJsonData))
            {
                return rawJsonData.Text;
            }

            if (CallableOperations.IsCallable(value))
            {
                return null;
            }

            switch (value)
            {
                case null:
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
                case global::System.Numerics.BigInteger:
                    throw new TypeError("Do not know how to serialize a BigInt");
            }

            if (Number.TryGetWrappedNumberValue(value, out _))
            {
                return SerializeNumber(TypeUtilities.ToNumber(value));
            }

            if (PropertyDescriptorStore.TryGetOwn(value!, String.StringDataPropertyName, out var stringData)
                && stringData.Kind == JsPropertyDescriptorKind.Data)
            {
                return Quote(DotNet2JSConversions.ToStringRejectingSymbols(value));
            }

            if (value is Boolean booleanObject)
            {
                return booleanObject.valueOf() ? "true" : "false";
            }

            if (PropertyDescriptorStore.TryGetOwn(
                    value!,
                    ObjectRuntime.PrimitiveValuePropertyName,
                    out var primitiveValue)
                && primitiveValue.Kind == JsPropertyDescriptorKind.Data
                && primitiveValue.Value is global::System.Numerics.BigInteger)
            {
                throw new TypeError("Do not know how to serialize a BigInt");
            }

            if (Array.isArray(value))
            {
                return SerializeArray(value!, propertyList, replacerFunction, stack, gap, indent);
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
            object array,
            List<string>? propertyList,
            object? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            PushToStackOrThrowIfCircular(stack, array);

            var lengthValue = TypeUtilities.ToNumber(ObjectRuntime.GetItem(array, "length"));
            var length = double.IsNaN(lengthValue) || lengthValue <= 0d
                ? 0
                : lengthValue > int.MaxValue
                    ? throw new RangeError("JSON.stringify array length exceeds runtime limits")
                    : (int)global::System.Math.Floor(lengthValue);
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
            object? replacerFunction,
            HashSet<object> stack,
            string gap,
            string indent)
        {
            PushToStackOrThrowIfCircular(stack, value);

            var keys = propertyList ?? ObjectRuntime.GetOwnEnumerableKeysInOrder(value);
            var parts = new List<string>();
            var stepBack = indent;
            indent += gap;
            try
            {
                foreach (var key in keys)
                {
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
            for (var index = 0; index < value.Length; index++)
            {
                var ch = value[index];
                if (char.IsHighSurrogate(ch))
                {
                    if (index + 1 < value.Length && char.IsLowSurrogate(value[index + 1]))
                    {
                        builder.Append(ch);
                        builder.Append(value[++index]);
                    }
                    else
                    {
                        AppendUnicodeEscape(builder, ch);
                    }

                    continue;
                }

                if (char.IsLowSurrogate(ch))
                {
                    AppendUnicodeEscape(builder, ch);
                    continue;
                }

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

        private static void AppendUnicodeEscape(StringBuilder builder, char value)
        {
            builder.Append("\\u");
            builder.Append(((int)value).ToString("x4", CultureInfo.InvariantCulture));
        }

        private static bool IsJsonWhitespace(char value)
            => value is '\t' or '\n' or '\r' or ' ';

        private static object? FromElement(JsonElement el, out JsonParseNode node)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    var obj = ObjectRuntime.CreateOrdinaryObject();
                    var properties = new Dictionary<string, JsonParseNode>(StringComparer.Ordinal);
                    foreach (var prop in el.EnumerateObject())
                    {
                        obj.SetBoxedValue(prop.Name, FromElement(prop.Value, out var child));
                        properties[prop.Name] = child;
                    }
                    node = new JsonParseNode(obj, properties);
                    return obj;

                case JsonValueKind.Array:
                    var arr = new Array();
                    var elements = new Dictionary<string, JsonParseNode>(StringComparer.Ordinal);
                    var index = 0;
                    foreach (var item in el.EnumerateArray())
                    {
                        arr.Add(FromElement(item, out var child)!);
                        elements[index.ToString(CultureInfo.InvariantCulture)] = child;
                        index++;
                    }
                    node = new JsonParseNode(arr, elements);
                    return arr;

                case JsonValueKind.String:
                    var stringValue = el.GetString();
                    node = new JsonParseNode(stringValue, el.GetRawText());
                    return stringValue;

                case JsonValueKind.Number:
                    // Use double to model JS number
                    var numberValue = el.GetDouble();
                    node = new JsonParseNode(numberValue, el.GetRawText());
                    return numberValue;

                case JsonValueKind.True:
                    node = new JsonParseNode(true, el.GetRawText());
                    return true;

                case JsonValueKind.False:
                    node = new JsonParseNode(false, el.GetRawText());
                    return false;

                case JsonValueKind.Null:
                    // Represent JavaScript null distinctly from CLR null (undefined)
                    node = new JsonParseNode(JsNull.Null, el.GetRawText());
                    return JsNull.Null;

                default:
                    // JSON doesn't produce Undefined; treat anything else as null
                    node = new JsonParseNode(null, el.GetRawText());
                    return null;
            }
        }
    }
}
