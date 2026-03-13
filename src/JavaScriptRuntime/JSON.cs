using System;
using System.Collections.Generic;
using System.Dynamic;
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
