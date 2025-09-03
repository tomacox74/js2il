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
            else
            {
                // todo: add generic object index access support
                throw new Exception("Object does not support index access. Only arrays are supported for index access.");
            }
        }

        public static double GetLength(object obj)
        {
            switch (obj)
            {
                case Array arr:
                    return arr.length;
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
            if (obj is Array)
            {
                // No custom properties yet; return null as missing
                return null;
            }

            // Fallback: no dynamic properties supported
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
    }
}
