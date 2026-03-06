using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime.Node
{
    [NodeModule("util")]
    public sealed class Util
    {
        private readonly object _types;
        private readonly Symbol _inspectCustomSymbol;
        private readonly Delegate _inspectFunction;

        public Util()
        {
            _inspectCustomSymbol = (Symbol)Symbol.@for("nodejs.util.inspect.custom");

            // Expose util.inspect as a delegate-valued property so util.inspect.custom is observable.
            _inspectFunction = new Func<object[], object?[], object?>((scopes, args) =>
            {
                var value = args.Length > 0 ? args[0] : null;
                var options = args.Length > 1 ? args[1] : null;
                return inspect(value, options);
            });

            PropertyDescriptorStore.DefineOrUpdate(this, "inspect", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = _inspectFunction
            });

            // Node.js: util.inspect.custom === Symbol.for('nodejs.util.inspect.custom')
            PropertyDescriptorStore.DefineOrUpdate(_inspectFunction, "custom", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = _inspectCustomSymbol
            });

            // Expose util.format as a function-valued property.
            PropertyDescriptorStore.DefineOrUpdate(this, "format", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = true,
                Configurable = true,
                Writable = true,
                Value = new Func<object[], object?[], object?>((scopes, args) => format(args))
            });

            _types = CreateTypesObject();
        }

        public object promisify(object callback)
        {
            if (callback is not Delegate original)
            {
                throw new TypeError("The \"original\" argument must be of type function");
            }

            // Return a function that when called, returns a Promise
            return new Func<object[], object?, object?>((scopes, args) =>
            {
                // Use withResolvers to get promise and resolve/reject functions
                var resolvers = Promise.withResolvers();
                
                // Create a wrapped callback that follows error-first convention
                var wrappedCallback = new Func<object[], object[], object?>((cbScopes, cbArgs) =>
                {
                    if (cbArgs.Length > 0 && cbArgs[0] != null && cbArgs[0] is not JsNull)
                    {
                        // Error-first: first arg is error, reject the promise
                        Closure.InvokeWithArgs(resolvers.reject, new object[0], cbArgs[0]);
                    }
                    else
                    {
                        // Success: resolve with second argument (or undefined if not present)
                        var result = cbArgs.Length > 1 ? cbArgs[1] : null;
                        Closure.InvokeWithArgs(resolvers.resolve, new object[0], result);
                    }
                    return null;
                });

                // Prepare arguments array with the wrapped callback appended
                var argsArray = args != null ? (args as object[] ?? new object[] { args }) : new object[0];
                var newArgs = new List<object>(argsArray) { wrappedCallback };

                // Invoke the original callback-style function
                try
                {
                    Closure.InvokeWithArgs(original, scopes, newArgs.ToArray());
                }
                catch (Exception ex)
                {
                    return Promise.reject(new Error(ex.Message, ex));
                }

                return resolvers.promise;
            });
        }

        public object? inherits(object constructor, object superConstructor)
        {
            if (constructor is not Delegate)
            {
                throw new TypeError("The \"constructor\" argument must be of type function");
            }

            if (superConstructor is not Delegate)
            {
                throw new TypeError("The \"superConstructor\" argument must be of type function");
            }

            // Set up prototype chain: constructor.prototype.__proto__ = superConstructor.prototype
            var ctorProto = Object.GetProperty(constructor, "prototype");
            if (ctorProto == null || ctorProto is JsNull)
            {
                ctorProto = new System.Dynamic.ExpandoObject();
                Object.SetProperty(constructor, "prototype", ctorProto);
            }

            var superProto = Object.GetProperty(superConstructor, "prototype");
            
            // Set the __proto__ of constructor.prototype to superConstructor.prototype
            Object.SetProperty(ctorProto, "__proto__", superProto);

            // Also set constructor property on the child prototype
            Object.SetProperty(ctorProto, "constructor", constructor);

            // Set super_ property (Node.js convention)
            Object.SetProperty(constructor, "super_", superConstructor);

            return null;
        }

        public object types => _types;

        public string inspect(object? value, object? options = null)
        {
            var depth = 2;
            var showHidden = false;
            var colors = false;

            if (options != null && options is not JsNull)
            {
                var depthProp = Object.GetProperty(options, "depth");
                if (depthProp is double d)
                {
                    depth = (int)d;
                }
                else if (depthProp is int i)
                {
                    depth = i;
                }

                var showHiddenProp = Object.GetProperty(options, "showHidden");
                if (showHiddenProp is bool sh)
                {
                    showHidden = sh;
                }

                var colorsProp = Object.GetProperty(options, "colors");
                if (colorsProp is bool c)
                {
                    colors = c;
                }
            }

            _ = showHidden;
            _ = colors;

            // Custom inspector: obj[util.inspect.custom](depth, options, inspect)
            if (value != null && value is not JsNull)
            {
                object? customInspector;
                try
                {
                    customInspector = Object.GetItem(value, _inspectCustomSymbol);
                }
                catch
                {
                    customInspector = null;
                }

                if (customInspector is Delegate del)
                {
                    var previousThis = RuntimeServices.SetCurrentThis(value);
                    try
                    {
                        var result = Closure.InvokeWithArgs(del, System.Array.Empty<object>(), (double)depth, options ?? (object)JsNull.Null, _inspectFunction);
                        if (result is string s)
                        {
                            return s;
                        }

                        return InspectValue(result, depth, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
                    }
                    finally
                    {
                        RuntimeServices.SetCurrentThis(previousThis);
                    }
                }
            }

            return InspectValue(value, depth, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        public string format(params object?[] args)
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }

            // Node.js: if first arg is not a string, join inspected args with spaces.
            if (args[0] is not string fmt)
            {
                return string.Join(" ", args.Select(a => InspectForFormat(a)));
            }

            var sb = new System.Text.StringBuilder(fmt.Length + 32);
            int argIndex = 1;

            for (int i = 0; i < fmt.Length; i++)
            {
                var ch = fmt[i];
                if (ch != '%' || i == fmt.Length - 1)
                {
                    sb.Append(ch);
                    continue;
                }

                var next = fmt[i + 1];
                if (next == '%')
                {
                    sb.Append('%');
                    i++;
                    continue;
                }

                bool consumes = next is 's' or 'd' or 'i' or 'f' or 'j' or 'o' or 'O';
                object? arg = argIndex < args.Length ? args[argIndex] : null;

                if (consumes && argIndex < args.Length)
                {
                    argIndex++;
                }

                switch (next)
                {
                    case 's':
                        sb.Append(DotNet2JSConversions.ToString(arg));
                        i++;
                        break;
                    case 'd':
                    case 'i':
                        sb.Append(DotNet2JSConversions.ToString(TypeUtilities.ToNumber(arg)));
                        i++;
                        break;
                    case 'f':
                        sb.Append(DotNet2JSConversions.ToString(TypeUtilities.ToNumber(arg)));
                        i++;
                        break;
                    case 'j':
                        sb.Append(FormatJson(arg));
                        i++;
                        break;
                    case 'o':
                    case 'O':
                        sb.Append(inspect(arg));
                        i++;
                        break;
                    default:
                        // Unknown specifier: do not consume an argument.
                        sb.Append('%').Append(next);
                        i++;
                        break;
                }
            }

            // Trailing args are appended with spaces; objects are inspected.
            for (int i = argIndex; i < args.Length; i++)
            {
                sb.Append(' ');
                sb.Append(InspectForFormat(args[i]));
            }

            return sb.ToString();
        }

        private string InspectForFormat(object? value)
        {
            if (value is null || value is JsNull)
            {
                return DotNet2JSConversions.ToString(value);
            }

            if (value is string s)
            {
                return s;
            }

            if (value is bool || value is double || value is float || value is int || value is long || value is short || value is byte || value is decimal || value is System.Numerics.BigInteger)
            {
                return DotNet2JSConversions.ToString(value);
            }

            return inspect(value);
        }

        private static string FormatJson(object? value)
        {
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            try
            {
                return FormatJsonValue(value, visited);
            }
            catch (InvalidOperationException)
            {
                return "[Circular]";
            }
        }

        private static string FormatJsonValue(object? value, HashSet<object> visited)
        {
            if (value is null)
            {
                // JSON.stringify(undefined) returns undefined; util.format('%j', undefined) prints undefined.
                return "undefined";
            }

            if (value is JsNull)
            {
                return "null";
            }

            if (value is bool b)
            {
                return b ? "true" : "false";
            }

            if (value is string s)
            {
                return System.Text.Json.JsonSerializer.Serialize(s);
            }

            if (value is double || value is float || value is int || value is long || value is short || value is byte || value is decimal)
            {
                // Keep formatting stable; JSON uses JS number syntax for our subset.
                return DotNet2JSConversions.ToString(TypeUtilities.ToNumber(value));
            }

            if (value is JavaScriptRuntime.Array arr)
            {
                if (!visited.Add(arr))
                {
                    throw new InvalidOperationException("Converting circular structure to JSON");
                }

                var len = arr.length is double dl ? (int)dl : 0;
                var items = new List<string>(len);
                for (int i = 0; i < len; i++)
                {
                    items.Add(FormatJsonValue(arr[(double)i], visited));
                }

                visited.Remove(arr);
                return "[" + string.Join(",", items) + "]";
            }

            if (value is IDictionary<string, object?> dict)
            {
                if (!visited.Add(dict))
                {
                    throw new InvalidOperationException("Converting circular structure to JSON");
                }

                var props = new List<string>();
                foreach (var kvp in dict)
                {
                    props.Add(System.Text.Json.JsonSerializer.Serialize(kvp.Key) + ":" + FormatJsonValue(kvp.Value, visited));
                }

                visited.Remove(dict);
                return "{" + string.Join(",", props) + "}";
            }

            // Best-effort fallback
            return System.Text.Json.JsonSerializer.Serialize(DotNet2JSConversions.ToString(value));
        }

        private static object CreateTypesObject()
        {
            var typesObj = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)typesObj;

            dict["isArray"] = new Func<object?, bool>(v => v is JavaScriptRuntime.Array);
            dict["isDate"] = new Func<object?, bool>(v => v is DateTime);
            dict["isError"] = new Func<object?, bool>(v => v is Error || v is Exception);
            dict["isFunction"] = new Func<object?, bool>(v => v is Delegate);
            dict["isPromise"] = new Func<object?, bool>(v => v is Promise);
            dict["isRegExp"] = new Func<object?, bool>(v => v is System.Text.RegularExpressions.Regex || v is JavaScriptRuntime.RegExp);
            dict["isString"] = new Func<object?, bool>(v => v is string);
            dict["isNumber"] = new Func<object?, bool>(v => v is double || v is float || v is int || v is long || v is short || v is byte || v is decimal);
            dict["isBoolean"] = new Func<object?, bool>(v => v is bool);
            dict["isUndefined"] = new Func<object?, bool>(v => v == null);
            dict["isNull"] = new Func<object?, bool>(v => v is JsNull);
            dict["isObject"] = new Func<object?, bool>(v => v != null && v is not JsNull && !(v is double || v is float || v is int || v is long || v is short || v is byte || v is decimal || v is string || v is bool));
            dict["isBigInt"] = new Func<object?, bool>(v => v is System.Numerics.BigInteger);
            dict["isSymbol"] = new Func<object?, bool>(v => v is Symbol);
            dict["isAsyncFunction"] = new Func<object?, bool>(v => v is Delegate d && d.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute), false).Length > 0);

            // Expanded for #787 (runtime-backed checks only)
            dict["isMap"] = new Func<object?, bool>(v => v is JavaScriptRuntime.Map);
            dict["isSet"] = new Func<object?, bool>(v => v is JavaScriptRuntime.Set);
            dict["isProxy"] = new Func<object?, bool>(v => v is JavaScriptRuntime.Proxy);
            dict["isTypedArray"] = new Func<object?, bool>(v => v is JavaScriptRuntime.Int32Array || v is JavaScriptRuntime.Node.Buffer);

            return typesObj;
        }

        private string InspectValue(object? value, int maxDepth, int currentDepth, HashSet<object> visited)
        {
            if (value == null)
            {
                return "undefined";
            }

            if (value is JsNull)
            {
                return "null";
            }

            if (value is string str)
            {
                return $"'{str}'";
            }

            if (value is bool b)
            {
                return b ? "true" : "false";
            }

            if (value is double || value is float || value is int || value is long || value is short || value is byte || value is decimal)
            {
                return value.ToString() ?? "0";
            }

            if (value is System.Numerics.BigInteger bi)
            {
                return bi.ToString() + "n";
            }

            if (value is Symbol sym)
            {
                return sym.ToString();
            }

            if (value is Delegate)
            {
                return "[Function]";
            }

            if (value is JavaScriptRuntime.Array arr)
            {
                if (visited.Contains(value))
                {
                    return "[Circular]";
                }

                if (currentDepth >= maxDepth)
                {
                    return "[Array]";
                }

                visited.Add(value);
                var elements = new List<string>();
                var length = arr.length is double len ? (int)len : 0;

                for (int i = 0; i < System.Math.Min(length, 100); i++)
                {
                    var elem = arr[(double)i];
                    elements.Add(InspectValue(elem, maxDepth, currentDepth + 1, visited));
                }

                visited.Remove(value);

                if (length > 100)
                {
                    elements.Add($"... {length - 100} more items");
                }

                return $"[ {string.Join(", ", elements)} ]";
            }

            if (value is Error err)
            {
                return err.ToString();
            }

            if (value is Promise)
            {
                return "Promise { <pending> }";
            }

            // Generic object inspection
            if (visited.Contains(value))
            {
                return "[Circular]";
            }

            if (currentDepth >= maxDepth)
            {
                return "[Object]";
            }

            visited.Add(value);
            var props = new List<string>();

            if (value is IDictionary<string, object?> dict)
            {
                foreach (var kvp in dict.Take(50))
                {
                    var propValue = InspectValue(kvp.Value, maxDepth, currentDepth + 1, visited);
                    props.Add($"{kvp.Key}: {propValue}");
                }

                if (dict.Count > 50)
                {
                    props.Add($"... {dict.Count - 50} more properties");
                }
            }
            else
            {
                // Try reflection for other objects
                var type = value.GetType();
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                foreach (var prop in properties.Take(50))
                {
                    try
                    {
                        var propValue = prop.GetValue(value);
                        var inspected = InspectValue(propValue, maxDepth, currentDepth + 1, visited);
                        props.Add($"{prop.Name}: {inspected}");
                    }
                    catch
                    {
                        props.Add($"{prop.Name}: [Error]");
                    }
                }

                if (properties.Length > 50)
                {
                    props.Add($"... {properties.Length - 50} more properties");
                }
            }

            visited.Remove(value);

            if (props.Count == 0)
            {
                return "{}";
            }

            return $"{{ {string.Join(", ", props)} }}";
        }
    }
}
