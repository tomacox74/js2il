using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Object", IntrinsicCallKind.ObjectConstruct)]
    public class Object
    {
        /// <summary>
        /// Implements the JavaScript Object() callable semantics: returns a new empty object.
        /// </summary>
        public static object Construct()
        {
            return new JavaScriptRuntime.Object();
        }

        /// <summary>
        /// Implements the JavaScript Object(value) callable semantics: returns a new empty object
        /// for null/undefined, otherwise returns the value unchanged (minimal behavior).
        /// </summary>
        public static object Construct(object? value)
        {
            if (value is null || value is JsNull)
            {
                return new JavaScriptRuntime.Object();
            }

            return value;
        }

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

                // Prefer exact-arity overloads (receiver + provided args). Only fall back to
                // longer-arity methods (padding with defaults) when no exact match exists.
                int jsArgCount = callArgs.Length;
                var exact = candidates
                    .Where(m => m.GetParameters().Length == 1 + jsArgCount)
                    .ToList();

                var viable = exact.Count > 0
                    ? exact
                    : candidates
                        .Where(m => m.GetParameters().Length >= 1 + jsArgCount)
                        .OrderBy(m => m.GetParameters().Length)
                        .ToList();
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

            // 3) ExpandoObject (object literal): properties may contain function delegates
            if (receiver is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (dict.TryGetValue(methodName, out var propValue) && propValue != null)
                {
                    // If the property value is a delegate, invoke it using Closure.InvokeWithArgs
                    if (propValue is Delegate)
                    {
                        var previousThis = RuntimeServices.SetCurrentThis(receiver);
                        try
                        {
                            return Closure.InvokeWithArgs(propValue, System.Array.Empty<object>(), callArgs);
                        }
                        finally
                        {
                            RuntimeServices.SetCurrentThis(previousThis);
                        }
                    }
                    throw new NotSupportedException($"Property '{methodName}' on object is not callable (type: {propValue.GetType().FullName})");
                }
                throw new NotSupportedException($"Property not found on object: {methodName}");
            }

            // 3b) Host object properties may also contain function delegates.
            // This enables patterns like:
            //   const r = Promise.withResolvers(); r.resolve(123);
            // where `resolve` is exposed as a delegate-valued property.
            var memberValue = GetProperty(receiver, methodName);
            if (memberValue is Delegate)
            {
                var previousThis = RuntimeServices.SetCurrentThis(receiver);
                try
                {
                    return Closure.InvokeWithArgs(memberValue, System.Array.Empty<object>(), callArgs);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            // 4) Fallback to reflection on receiver type
            return CallInstanceMethod(receiver, methodName, callArgs);
        }
        public static object GetItem(object obj, object index)
        {
            // Coerce index to int (JS ToInt32-ish truncation)
            int intIndex;
            switch (index)
            {
                case int ii: intIndex = ii; break;
                case double dd: intIndex = (int)dd; break;
                case float ff: intIndex = (int)ff; break;
                case long ll: intIndex = (int)ll; break;
                case short ss: intIndex = ss; break;
                case byte bb: intIndex = bb; break;
                case string s when int.TryParse(s, out var pi): intIndex = pi; break;
                case bool b: intIndex = b ? 1 : 0; break;
                default:
                    try { intIndex = Convert.ToInt32(index); }
                    catch { intIndex = 0; }
                    break;
            }

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): numeric index coerces to property name string per JS ToPropertyKey
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                var propName = DotNet2JSConversions.ToString(index);
                if (dict.TryGetValue(propName, out var value))
                {
                    return value!;
                }
                return null!; // closest to JS 'undefined' (we model as null)
            }

            if (obj is Array array)
            {
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is Int32Array i32)
            {
                // Reads outside bounds return 0 per typed array semantics
                return i32[(double)intIndex];
            }
            else
            {
                // Generic object index access: treat index as a property key (JS ToPropertyKey -> string)
                // and fall back to dynamic property lookup (public fields/properties and ExpandoObject).
                var propName = DotNet2JSConversions.ToString(index);
                return GetProperty(obj, propName)!;
            }
        }

        public static object GetItem(object obj, double index)
        {
            // Coerce index to int (JS ToInt32-ish truncation)
            int intIndex = (int)index;

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): numeric index coerces to property name string per JS ToPropertyKey
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                var propName = DotNet2JSConversions.ToString(index);
                if (dict.TryGetValue(propName, out var value))
                {
                    return value!;
                }
                return null!; // closest to JS 'undefined' (we model as null)
            }

            if (obj is Array array)
            {
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is Int32Array i32)
            {
                // Reads outside bounds return 0 per typed array semantics
                return i32[(double)intIndex];
            }
            else
            {
                // Generic object index access: treat index as a property key (JS ToPropertyKey -> string)
                // and fall back to dynamic property lookup (public fields/properties and ExpandoObject).
                var propName = DotNet2JSConversions.ToString(index);
                return GetProperty(obj, propName)!;
            }
        }

        /// <summary>
        /// Sets an item on an object by index/key.
        /// Used by codegen for computed member assignment and property assignment.
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetItem(object? obj, object index, object? value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            // Compute both numeric index and property key string.
            int intIndex;
            switch (index)
            {
                case int ii: intIndex = ii; break;
                case double dd: intIndex = (int)dd; break;
                case float ff: intIndex = (int)ff; break;
                case long ll: intIndex = (int)ll; break;
                case short ss: intIndex = ss; break;
                case byte bb: intIndex = bb; break;
                case string s when int.TryParse(s, out var pi): intIndex = pi; break;
                case bool b: intIndex = b ? 1 : 0; break;
                default:
                    try { intIndex = Convert.ToInt32(index); }
                    catch { intIndex = 0; }
                    break;
            }

            var propName = DotNet2JSConversions.ToString(index);

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            // ExpandoObject (object literal): assign by property name
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                dict[propName] = value;
                return value;
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (intIndex < 0)
                {
                    // Negative indices behave like properties in JS; treat as property for host safety.
                    return SetProperty(array, propName, value);
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value!;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                // Extend with undefined (null) up to the index, then add.
                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: coerce and store when in-bounds
            if (obj is Int32Array i32)
            {
                // Index/value are numeric for typed arrays; coerce here so Int32Array can remain numeric.
                i32[(double)intIndex] = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return value;
            }

            // Generic object: treat as property assignment (ToPropertyKey -> string)
            return SetProperty(obj, propName, value);
        }

        /// <summary>
        /// Fast-path overload for numeric index and numeric value.
        /// Avoids boxing at the call site when the compiler has unboxed doubles.
        /// Returns the assigned value (boxed) to match JavaScript assignment expression semantics.
        /// </summary>
        public static object SetItem(object? obj, double index, double value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            int intIndex;
            if (double.IsNaN(index) || double.IsInfinity(index))
            {
                intIndex = 0;
            }
            else
            {
                try { intIndex = (int)index; }
                catch { intIndex = 0; }
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (intIndex < 0)
                {
                    // Negative indices behave like properties in JS; treat as property for host safety.
                    return SetProperty(array, intIndex.ToString(System.Globalization.CultureInfo.InvariantCulture), value) ?? value;
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: store when in-bounds (non-boxing).
            if (obj is Int32Array i32)
            {
                i32.SetFromDouble(intIndex, value);
                return value;
            }

            // Fallback: treat numeric index as a property key string.
            return SetProperty(obj, DotNet2JSConversions.ToString(index), value) ?? value;
        }

        /// <summary>
        /// Object rest helper used by destructuring: { a, ...rest }.
        /// Returns a new ExpandoObject with enumerable keys copied excluding the provided keys.
        /// </summary>
        public static object Rest(object? obj, object[] excludedKeys)
        {
            if (obj is null || obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot destructure null or undefined");
            }

            var excluded = new HashSet<string>(StringComparer.Ordinal);
            foreach (var k in excludedKeys)
            {
                excluded.Add(DotNet2JSConversions.ToString(k));
            }

            var result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)result;

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var src = (IDictionary<string, object?>)exp;
                foreach (var kvp in src)
                {
                    if (excluded.Contains(kvp.Key)) continue;
                    dict[kvp.Key] = kvp.Value;
                }
                return result;
            }

            // Reflection fallback for host objects: copy public instance properties/fields.
            try
            {
                var type = obj.GetType();
                foreach (var p in type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && !excluded.Contains(p.Name)))
                {
                    dict[p.Name] = p.GetValue(obj);
                }

                foreach (var f in type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => !excluded.Contains(f.Name)))
                {
                    dict[f.Name] = f.GetValue(obj);
                }
            }
            catch
            {
                // Best-effort; return whatever we could copy.
            }

            return result;
        }

        /// <summary>
        /// Throws a Node/V8-compatible TypeError for destructuring when the source value is null or undefined.
        /// This is centralized to allow future localization of error messages.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowDestructuringNullOrUndefined(object? sourceValue, string? sourceVariableName, string? targetVariableName)
        {
            // In this runtime:
            // - JS undefined is represented as CLR null
            // - JS null is represented as JavaScriptRuntime.JsNull
            if (sourceValue is not null && sourceValue is not JsNull)
            {
                throw new InvalidOperationException($"{nameof(ThrowDestructuringNullOrUndefined)} must only be called for null/undefined source values.");
            }

            string kind = sourceValue is null ? "undefined" : "null";
            string sourceName = string.IsNullOrWhiteSpace(sourceVariableName) ? kind : sourceVariableName!;
            string targetName = string.IsNullOrWhiteSpace(targetVariableName) ? "<unknown>" : targetVariableName!;

            // Node/V8 style:
            // TypeError: Cannot destructure property 'a' of 'x' as it is undefined
            throw new JavaScriptRuntime.TypeError($"Cannot destructure property '{targetName}' of '{sourceName}' as it is {kind}");
        }

        /// <summary>
        /// Normalizes an iterable for use by for..of desugaring.
        /// Returns a value compatible with GetLength/GetItem (string, Array, Int32Array).
        /// - string stays string (GetItem returns 1-length strings)
        /// - Array/Int32Array pass through
        /// - Set and any other IEnumerable are converted via Array.from
        /// </summary>
        public static object NormalizeForOfIterable(object? iterable)
        {
            if (iterable is null || iterable is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot iterate over null or undefined");
            }

            if (iterable is string)
            {
                return iterable;
            }

            if (iterable is Array || iterable is Int32Array)
            {
                return iterable;
            }

            if (iterable is System.Collections.IEnumerable)
            {
                return JavaScriptRuntime.Array.from(iterable)!;
            }

            throw new JavaScriptRuntime.TypeError("Object is not iterable");
        }

        /// <summary>
        /// Returns enumerable property keys for for..in. Minimal implementation:
        /// - ExpandoObject: own keys
        /// - Array/Int32Array/string: numeric indices as strings
        /// - IDictionary: keys coerced to strings
        /// Other types: empty list
        /// </summary>
        public static JavaScriptRuntime.Array GetEnumerableKeys(object? obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Right-hand side of 'for...in' should be an object");
            }

            // ExpandoObject (object literal)
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                var keys = new JavaScriptRuntime.Array(dict.Keys.Cast<object?>());
                return keys;
            }

            // JS Array: enumerate indices
            if (obj is JavaScriptRuntime.Array jsArr)
            {
                var keys = new JavaScriptRuntime.Array();
                for (int i = 0; i < jsArr.Count; i++)
                {
                    keys.Add(i.ToString());
                }
                return keys;
            }

            // Typed array: enumerate indices
            if (obj is JavaScriptRuntime.Int32Array i32)
            {
                var keys = new JavaScriptRuntime.Array();
                for (int i = 0; i < i32.length; i++)
                {
                    keys.Add(i.ToString());
                }
                return keys;
            }

            // String: enumerate indices
            if (obj is string s)
            {
                var keys = new JavaScriptRuntime.Array();
                for (int i = 0; i < s.Length; i++)
                {
                    keys.Add(i.ToString());
                }
                return keys;
            }

            // IDictionary: enumerate keys
            if (obj is System.Collections.IDictionary dictObj)
            {
                var keys = new JavaScriptRuntime.Array();
                foreach (var k in dictObj.Keys)
                {
                    keys.Add(DotNet2JSConversions.ToString(k));
                }
                return keys;
            }

            return JavaScriptRuntime.Array.Empty;
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
                    i32[(double)i] = iv;
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
            // Null/undefined -> undefined (modeled as null)
            if (obj is null) return null;
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

            object? GetValue(Type type)
            {
                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null && prop.CanRead)
                {
                    return prop.GetValue(obj);
                }
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
                if (field != null)
                {
                    return field.GetValue(obj);
                }

                var baseType = type.BaseType;
                if (baseType != null && baseType != typeof(object))
                {
                    return GetValue(baseType);
                }

                return null;
            }

            // Reflection fallback: expose public instance properties/fields of host objects
            try
            {
                return GetValue(obj.GetType());
            }
            catch
            {
                // Swallow and return undefined/null on reflection failures
            }

            // Property not found; return undefined (null)
            return null;
        }

        /// <summary>
        /// Dynamic property assignment used when the compiler cannot bind a static setter.
        /// Supports:
        ///  - ExpandoObject (object literal): sets/overwrites the named property
        ///  - Reflection fallback: sets public instance property/field when writable
        ///  - Arrays/typed arrays: currently no custom properties; silently ignore and return value
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetProperty(object obj, string name, object? value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(name)) return value;

            // ExpandoObject support
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                dict[name] = value;
                return value;
            }

            // Arrays / typed arrays: ignore arbitrary properties for now
            if (obj is Array || obj is Int32Array)
            {
                return value;
            }

            // Reflection fallback: settable property or public field
            try
            {
                var type = obj.GetType();
                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                    return value;
                }
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return value;
                }
            }
            catch
            {
                // Swallow and fall through to return value to mimic JS permissiveness
            }

            return value;
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

            // Helper to coerce primitive numeric CLR types to JS number (double)
            static object CoerceToJsNumber(object o)
            {
                switch (o)
                {
                    case double _: return o;
                    case float f: return (double)f;
                    case int i: return (double)i;
                    case long l: return (double)l;
                    case short s: return (double)s;
                    case byte b: return (double)b;
                    case sbyte sb: return (double)sb;
                    case uint ui: return (double)ui;
                    case ulong ul: return (double)ul;
                    case ushort us: return (double)us;
                    case decimal d: return (double)d;
                    default: return o;
                }
            }

            var src = args ?? empty;

            // JavaScript semantics: extra args are ignored (unless the target explicitly accepts params object[]).
            // Also ensure we never write past the end of the argument array.
            var coerced = expectsParamsArray ? new object[src.Length] : new object[psChosen.Length];
            var copyCount = expectsParamsArray ? src.Length : System.Math.Min(src.Length, coerced.Length);
            for (int i = 0; i < copyCount; i++)
            {
                coerced[i] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
            }

            object?[] invokeArgs = expectsParamsArray ? new object?[] { coerced } : coerced;
            return chosen.Invoke(instance, invokeArgs);
        }

        /// <summary>
        /// Safely coerces an object to int32, following JavaScript semantics.
        /// Handles double, other numeric types, and null (coerces to 0).
        /// </summary>
        public static int CoerceToInt32(object? value)
        {
            if (value is null) return 0;
            
            switch (value)
            {
                case double d: return (int)d;
                case float f: return (int)f;
                case int i: return i;
                case long l: return (int)l;
                case short s: return s;
                case byte b: return b;
                case sbyte sb: return sb;
                case uint ui: return (int)ui;
                case ulong ul: return (int)ul;
                case ushort us: return us;
                case decimal dec: return (int)dec;
                case bool bo: return bo ? 1 : 0;
                case string str:
                    if (double.TryParse(str, out double parsed))
                        return (int)parsed;
                    return 0;
                default:
                    // Non-numeric objects coerce to NaN in JS, which becomes 0 when converted to int
                    return 0;
            }
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
