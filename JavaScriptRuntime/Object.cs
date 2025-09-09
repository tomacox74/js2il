using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    public class Object
    {
        /// <summary>
        /// Generic member-call dispatcher. Given a receiver object, a method name, and arguments,
        /// selects and invokes an appropriate implementation based on runtime type:
        ///  - If receiver is a .NET string, dispatch to JavaScriptRuntime.String static helpers
        ///    with the receiver coerced to string as the first parameter.
        ///  - If receiver is a JavaScriptRuntime.Array, dispatch to its instance methods.
        ///  - Otherwise, fall back to reflection-based instance call on the receiver type.
        /// </summary>
        public static object? CallMember(object receiver, string methodName, object[]? args)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            var callArgs = args ?? System.Array.Empty<object>();

            // 1) String receiver -> route to JavaScriptRuntime.String static methods
            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                var stringType = typeof(JavaScriptRuntime.String);
                var candidates = stringType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase))
                    .Where(m =>
                    {
                        var ps = m.GetParameters();
                        return ps.Length >= 1 && ps[0].ParameterType == typeof(string);
                    })
                    .ToList();

                if (candidates.Count == 0)
                {
                    throw new NotSupportedException($"Host intrinsic method not found: String.{methodName}");
                }

                // Prefer methods that can accept provided arg count; allow padding with defaults
                int jsArgCount = callArgs.Length;
                var viable = candidates
                    .Where(m => m.GetParameters().Length >= 1 + jsArgCount)
                    .OrderBy(m => m.GetParameters().Length)
                    .ToList();
                if (viable.Count == 0)
                {
                    viable = candidates.Where(m => m.GetParameters().Length == 1 + jsArgCount).ToList();
                }
                var chosen = viable
                    .OrderByDescending(m => m.GetParameters().Skip(1).Take(jsArgCount).Count(p => p.ParameterType != typeof(object)))
                    .FirstOrDefault();

                if (chosen == null)
                {
                    throw new NotSupportedException($"No compatible overload found for String.{methodName} with {jsArgCount} argument(s)");
                }

                var ps = chosen.GetParameters();
                var invokeArgs = new object?[ps.Length];
                // first param is the string receiver
                invokeArgs[0] = input;

                // Fill provided args with basic coercions based on target param types
                for (int i = 0; i < jsArgCount && (i + 1) < ps.Length; i++)
                {
                    var target = ps[i + 1].ParameterType;
                    var src = callArgs[i];
                    if (target == typeof(string))
                    {
                        invokeArgs[i + 1] = DotNet2JSConversions.ToString(src);
                    }
                    else if (target == typeof(bool))
                    {
                        invokeArgs[i + 1] = JavaScriptRuntime.TypeUtilities.ToBoolean(src);
                    }
                    else
                    {
                        invokeArgs[i + 1] = src;
                    }
                }
                // Pad any remaining parameters (beyond provided args): false for bool, null otherwise
                for (int pi = 1 + jsArgCount; pi < ps.Length; pi++)
                {
                    invokeArgs[pi] = ps[pi].ParameterType == typeof(bool) ? (object)false : null;
                }

                return chosen.Invoke(null, invokeArgs);
            }

            // 2) JavaScriptRuntime.Array -> instance methods
            if (receiver is Array jsArray)
            {
                var type = typeof(Array);
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
                    .ToList();
                if (methods.Count == 0)
                {
                    throw new NotSupportedException($"Method not found: {type.FullName}.{methodName}");
                }
                // Prefer params object[] first, else exact arg count
                var chosen = methods.FirstOrDefault(mi =>
                {
                    var ps = mi.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == (callArgs?.Length ?? 0));

                if (chosen == null)
                {
                    // Fallback: pick smallest arity and let reflection coerce if possible
                    chosen = methods.OrderBy(mi => mi.GetParameters().Length).First();
                }

                var psChosen = chosen.GetParameters();
                var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
                var invokeArgs = expectsParamsArray ? new object?[] { callArgs } : (object[]?)callArgs;
                return chosen.Invoke(jsArray, invokeArgs);
            }

            // 3) Fallback to reflection on receiver type
            return CallInstanceMethod(receiver, methodName, callArgs);
        }
        public static object GetItem(object obj, double index)
        {
            var intIndex = Convert.ToInt32(index);

            if (obj is Array array)
            {
                return array[intIndex];
            }
            else if (obj is Int32Array i32)
            {
                // Reads outside bounds return 0 per typed array semantics
                return i32[intIndex];
            }
            else
            {
                // todo: add generic object index access support
                throw new Exception("Object does not support index access. Only arrays are supported for index access.");
            }
        }

        /// <summary>
        /// Dynamic indexed / computed property assignment used when the compiler
        /// cannot statically bind an Int32Array or Array element store. Returns the
        /// assigned value (boxed) to match JavaScript assignment expression result.
        /// Supports:
        ///  - JavaScriptRuntime.Array (List<object>) with numeric index (expands with nulls)
        ///  - JavaScriptRuntime.Int32Array (ignored if OOB)
        ///  - Fallback: throws for unsupported receiver types.
        /// </summary>
        public static object? AssignItem(object receiver, object index, object value)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));

            // Coerce index to int (JS ToInt32-ish truncation)
            int i;
            switch (index)
            {
                case int ii: i = ii; break;
                case double dd: i = (int)dd; break;
                case float ff: i = (int)ff; break;
                case long ll: i = (int)ll; break;
                case short ss: i = ss; break;
                case byte bb: i = bb; break;
                case string s when int.TryParse(s, out var pi): i = pi; break;
                case bool b: i = b ? 1 : 0; break;
                default:
                    try { i = Convert.ToInt32(index); }
                    catch { i = 0; }
                    break;
            }
            if (i < 0) return value; // negative indexes ignored for now

            if (receiver is Array jsArray)
            {
                // Expand with nulls if index >= Count (approximate JS dense array semantics for numeric indexes)
                while (i >= jsArray.Count) jsArray.Add(null!);
                jsArray[i] = value;
                return value;
            }
            if (receiver is Int32Array i32)
            {
                if (i < i32.length)
                {
                    // Coerce value to int32 similar to runtime semantics
                    int iv;
                    switch (value)
                    {
                        case int vi: iv = vi; break;
                        case double vd: iv = (int)vd; break;
                        case float vf: iv = (int)vf; break;
                        case long vl: iv = (int)vl; break;
                        case short vs: iv = vs; break;
                        case byte vb: iv = vb; break;
                        case bool vb2: iv = vb2 ? 1 : 0; break;
                        case string s when int.TryParse(s, out var ps): iv = ps; break;
                        default:
                            try { iv = Convert.ToInt32(value); }
                            catch { iv = 0; }
                            break;
                    }
                    i32[i] = iv;
                }
                return value;
            }

            // Future: object / expando numeric property assignment
            throw new NotSupportedException($"AssignItem not supported for receiver type '{receiver.GetType().FullName}'");
        }

        public static double GetLength(object obj)
        {
            switch (obj)
            {
                case Array arr:
                    return arr.length;
                case Int32Array i32:
                    return i32.length;
                case string s:
                    return s.Length;
                default:
                    // Fallback: try ICollection Count
                    if (obj is System.Collections.ICollection coll)
                        return coll.Count;
                    return 0.0;
            }
        }

        public static object? GetProperty(object obj, string name)
        {
            // ExpandoObject properties
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (dict.TryGetValue(name, out var value))
                {
                    return value;
                }
                return null; // closest to JS undefined for now
            }

            // JavaScriptRuntime.Array: expose known properties via dot (length handled elsewhere)
            if (obj is Array || obj is Int32Array)
            {
                // No custom properties yet; return null as missing
                return null;
            }

            // Reflection fallback: expose public instance properties/fields of host objects
            var type = obj.GetType();
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanRead)
            {
                return prop.GetValue(obj);
            }
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
            {
                return field.GetValue(obj);
            }
            // Unknown -> undefined/null
            return null;
        }

        // Dynamic instance method invocation fallback for host/intrinsic objects when the CLR type
        // is not known at compile-time (e.g., fs.readFileSync within a nested function).
        // Attempts a minimal overload resolution:
        //  - Prefer a single-parameter params object[] method
        //  - Otherwise prefer an exact parameter count match
        public static object? CallInstanceMethod(object instance, string methodName, object[] args)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrEmpty(methodName)) throw new ArgumentNullException(nameof(methodName));

            var type = instance.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                               .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
                               .ToList();

            if (methods.Count == 0)
            {
                throw new NotSupportedException($"Method not found: {type.FullName}.{methodName}");
            }

            MethodInfo? chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == (args?.Length ?? 0));

            if (chosen == null)
            {
                // Last resort: pick the first and attempt object[] packing when possible
                chosen = methods.OrderBy(mi => mi.GetParameters().Length).First();
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var empty = System.Array.Empty<object>();
            var invokeArgs = expectsParamsArray ? new object?[] { args ?? empty } : (object[])(args ?? empty);
            return chosen.Invoke(instance, invokeArgs);
        }

        // Support for the JavaScript 'in' operator (minimal implementation)
        // Parameter order matches evaluation order in emitter: left (key) then right (object)
        public static bool HasPropertyIn(object? key, object? obj)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Right-hand side of 'in' should be an object");
            }

            // Coerce key to property name (symbols not supported yet)
            string propName = key switch
            {
                null => "null",
                string s => s,
                double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
                float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
                int i => i.ToString(),
                long l => l.ToString(),
                short sh => sh.ToString(),
                byte by => by.ToString(),
                _ => DotNet2JSConversions.ToString(key)
            } ?? string.Empty;

            // ExpandoObject (object literal)
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                return dict.ContainsKey(propName);
            }

            // JS Array (numeric indexes + length)
            if (obj is Array jsArr)
            {
                if (propName == "length") return true;
                if (int.TryParse(propName, out var ai))
                {
                    return ai >= 0 && ai < jsArr.length;
                }
                return false;
            }

            // Int32Array (typed array minimal support)
            if (obj is Int32Array i32)
            {
                if (propName == "length") return true;
                if (int.TryParse(propName, out var ti))
                {
                    return ti >= 0 && ti < i32.length;
                }
                return false;
            }

            // string (indices + length)
            if (obj is string str)
            {
                if (propName == "length") return true;
                if (int.TryParse(propName, out var si))
                {
                    return si >= 0 && si < str.Length;
                }
                return false;
            }

            // Fallback: reflection public instance property/field presence
            var type = obj.GetType();
            var pi = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (pi != null) return true;
            var fi = type.GetField(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (fi != null) return true;
            return false;
        }
    }
}
