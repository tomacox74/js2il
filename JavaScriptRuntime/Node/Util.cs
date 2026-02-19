using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime.Node
{
    [NodeModule("util")]
    public sealed class Util
    {
        private readonly object _types;

        public Util()
        {
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

            return InspectValue(value, depth, 0, new HashSet<object>());
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
            dict["isRegExp"] = new Func<object?, bool>(v => v is System.Text.RegularExpressions.Regex);
            dict["isString"] = new Func<object?, bool>(v => v is string);
            dict["isNumber"] = new Func<object?, bool>(v => v is double || v is float || v is int || v is long || v is short || v is byte || v is decimal);
            dict["isBoolean"] = new Func<object?, bool>(v => v is bool);
            dict["isUndefined"] = new Func<object?, bool>(v => v == null);
            dict["isNull"] = new Func<object?, bool>(v => v is JsNull);
            dict["isObject"] = new Func<object?, bool>(v => v != null && v is not JsNull && !(v is double || v is float || v is int || v is long || v is short || v is byte || v is decimal || v is string || v is bool));
            dict["isBigInt"] = new Func<object?, bool>(v => v is System.Numerics.BigInteger);
            dict["isSymbol"] = new Func<object?, bool>(v => v is Symbol);
            dict["isAsyncFunction"] = new Func<object?, bool>(v => v is Delegate d && d.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute), false).Length > 0);

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
