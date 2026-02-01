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
        private static bool IsObjectLikeForPrototype(object value)
        {
            // JS null/undefined are not objects.
            if (value is null) return false;
            if (value is JsNull) return false;

            // JS primitives (string/number/boolean/etc) are not objects.
            if (value is string) return false;
            if (value.GetType().IsValueType) return false;

            return true;
        }

        private static bool IsValidPrototypeValue(object? value)
        {
            // In JS, [[Prototype]] must be an object or null.
            if (value is JsNull) return true;
            return TypeUtilities.IsConstructorReturnOverride(value);
        }

        /// <summary>
        /// Minimal implementation of <c>Object.getPrototypeOf(obj)</c>.
        /// Note: this runtime does not currently model default Object.prototype; if no prototype
        /// has been explicitly assigned, this returns undefined (null).
        /// </summary>
        public static object? getPrototypeOf(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }
            if (!IsObjectLikeForPrototype(obj))
            {
                throw new TypeError("Object.getPrototypeOf called on non-object");
            }

            // Calling getPrototypeOf is itself an opt-in signal.
            PrototypeChain.Enable();

            return PrototypeChain.TryGetPrototype(obj, out var prototype) ? prototype : null;
        }

        /// <summary>
        /// Minimal implementation of <c>Object.setPrototypeOf(obj, proto)</c>.
        /// Returns the target object.
        /// </summary>
        public static object setPrototypeOf(object obj, object? prototype)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }
            if (!IsObjectLikeForPrototype(obj))
            {
                throw new TypeError("Object.setPrototypeOf called on non-object");
            }
            if (!IsValidPrototypeValue(prototype))
            {
                throw new TypeError("Object prototype may only be an Object or null");
            }

            PrototypeChain.SetPrototype(obj, prototype);
            return obj;
        }

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
        /// Implements JavaScript <c>new ctor(...args)</c> semantics for cases where the constructor
        /// is not statically known at compile time (e.g. <c>const C = require('...'); new C()</c>).
        ///
        /// Supported constructor value shapes:
        /// - <see cref="Type"/>: invokes a public instance constructor (reflection).
        /// - <see cref="Delegate"/>: invokes via <see cref="Closure.InvokeWithArgs"/> (scopes are empty for now).
        /// - <see cref="System.Dynamic.ExpandoObject"/> with a callable <c>Construct</c> property.
        /// - Any object with a public instance method named <c>Construct</c>.
        ///
        /// Note: This is intentionally minimal; it enables CommonJS export/import patterns where
        /// constructor values cross module boundaries.
        /// </summary>
        public static object? ConstructValue(object constructor, object[]? args)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));

            var callArgs = args ?? System.Array.Empty<object>();

            if (constructor is Type type)
            {
                try
                {
                    return Activator.CreateInstance(type, callArgs);
                }
                catch (TargetInvocationException tie) when (tie.InnerException != null)
                {
                    throw tie.InnerException;
                }
            }

            if (constructor is Delegate del)
            {
                return Closure.InvokeWithArgs(del, System.Array.Empty<object>(), callArgs);
            }

            if (constructor is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (dict.TryGetValue("Construct", out var constructValue) && constructValue is Delegate constructDel)
                {
                    return Closure.InvokeWithArgs(constructDel, System.Array.Empty<object>(), callArgs);
                }
            }

            // Generic reflection fallback: instance method named Construct(...)
            try
            {
                var method = constructor.GetType().GetMethod("Construct", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (method != null)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]))
                    {
                        return method.Invoke(constructor, new object?[] { callArgs });
                    }

                    // Otherwise, pass as individual arguments (pad missing with null)
                    var invokeArgs = new object?[parameters.Length];
                    for (int i = 0; i < invokeArgs.Length; i++)
                    {
                        invokeArgs[i] = i < callArgs.Length ? callArgs[i] : null;
                    }
                    return method.Invoke(constructor, invokeArgs);
                }
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }

            throw new NotSupportedException($"Value is not constructible: {constructor.GetType().FullName}");
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
                    throw new TypeError($"String.{methodName} is not a function");
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
                    throw new TypeError($"String.{methodName} is not a function");
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
                    throw new TypeError($"{methodName} is not a function");
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
                var propValue = GetProperty(receiver, methodName);
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

                throw new TypeError($"{methodName} is not a function");
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

        private static string ToPropertyKeyString(object key)
        {
            if (key is Symbol sym)
            {
                // NOTE: We don't yet model true ECMAScript symbol-keyed properties.
                // We encode Symbol keys to a stable internal string so computed
                // properties like obj[Symbol.iterator] can round-trip.
                return sym.DebugId;
            }

            return DotNet2JSConversions.ToString(key);
        }

        public static object GetItem(object obj, object index)
        {
            // If the key is a Symbol (or a non-numeric string), it must be treated as a property key.
            // The previous implementation coerced all keys to an integer index first, which caused
            // Symbol keys (e.g. Symbol.asyncIterator) to incorrectly read index 0 on arrays.
            if (index is Symbol)
            {
                var propName = ToPropertyKeyString(index);
                if (obj is System.Dynamic.ExpandoObject expSym)
                {
                    var dict = (IDictionary<string, object?>)expSym;
                    if (dict.TryGetValue(propName, out var value))
                    {
                        return value!;
                    }

                    // Legacy __proto__ accessor (opt-in)
                    if (PrototypeChain.Enabled && string.Equals(propName, "__proto__", StringComparison.Ordinal))
                    {
                        return PrototypeChain.TryGetPrototype(obj, out var proto) ? proto! : null!;
                    }

                    if (PrototypeChain.Enabled)
                    {
                        static bool TryGetOwnProperty(object target, string name, out object? value)
                        {
                            if (target is System.Dynamic.ExpandoObject exp)
                            {
                                var d = (IDictionary<string, object?>)exp;
                                return d.TryGetValue(name, out value);
                            }

                            // Arrays/typed arrays don't have custom named props yet
                            if (target is Array || target is Int32Array)
                            {
                                value = null;
                                return false;
                            }

                            // Reflection fallback for host objects
                            object? GetValue(Type type)
                            {
                                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                                if (prop != null && prop.CanRead)
                                {
                                    return prop.GetValue(target);
                                }
                                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
                                if (field != null)
                                {
                                    return field.GetValue(target);
                                }

                                var baseType = type.BaseType;
                                if (baseType != null && baseType != typeof(object))
                                {
                                    return GetValue(baseType);
                                }

                                return null;
                            }

                            try
                            {
                                value = GetValue(target.GetType());
                                return value != null;
                            }
                            catch
                            {
                                value = null;
                                return false;
                            }
                        }

                        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
                        object? current = obj;
                        while (current != null && visited.Add(current))
                        {
                            var proto = PrototypeChain.GetPrototypeOrNull(current);
                            if (proto is null || proto is JsNull)
                            {
                                break;
                            }

                            if (TryGetOwnProperty(proto, propName, out var inherited))
                            {
                                return inherited!;
                            }

                            current = proto;
                        }
                    }

                    return null!;
                }

                return GetProperty(obj, propName)!;
            }

            if (index is string sKey && !int.TryParse(sKey, out _))
            {
                // JS ToPropertyKey for non-index strings.
                if (obj is System.Dynamic.ExpandoObject expStr)
                {
                    var dict = (IDictionary<string, object?>)expStr;
                    if (dict.TryGetValue(sKey, out var value))
                    {
                        return value!;
                    }

                    // Legacy __proto__ accessor (opt-in)
                    if (PrototypeChain.Enabled && string.Equals(sKey, "__proto__", StringComparison.Ordinal))
                    {
                        return PrototypeChain.TryGetPrototype(obj, out var proto) ? proto! : null!;
                    }

                    // Prototype-chain lookup for normal property access (opt-in)
                    if (PrototypeChain.Enabled)
                    {
                        static bool TryGetOwnProperty(object target, string name, out object? value)
                        {
                            if (target is System.Dynamic.ExpandoObject exp)
                            {
                                var d = (IDictionary<string, object?>)exp;
                                return d.TryGetValue(name, out value);
                            }

                            // Arrays/typed arrays don't have custom named props yet
                            if (target is Array || target is Int32Array)
                            {
                                value = null;
                                return false;
                            }

                            // Reflection fallback for host objects
                            object? GetValue(Type type)
                            {
                                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                                if (prop != null && prop.CanRead)
                                {
                                    return prop.GetValue(target);
                                }
                                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
                                if (field != null)
                                {
                                    return field.GetValue(target);
                                }

                                var baseType = type.BaseType;
                                if (baseType != null && baseType != typeof(object))
                                {
                                    return GetValue(baseType);
                                }

                                return null;
                            }

                            try
                            {
                                value = GetValue(target.GetType());
                                return value != null;
                            }
                            catch
                            {
                                value = null;
                                return false;
                            }
                        }

                        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
                        object? current = obj;
                        while (current != null && visited.Add(current))
                        {
                            var proto = PrototypeChain.GetPrototypeOrNull(current);
                            if (proto is null || proto is JsNull)
                            {
                                break;
                            }

                            if (TryGetOwnProperty(proto, sKey, out var inherited))
                            {
                                return inherited!;
                            }

                            current = proto;
                        }
                    }

                    return null!;
                }

                return GetProperty(obj, sKey)!;
            }

            // Coerce index to int (JS ToInt32-ish truncation)
            int intIndex = 0;
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
                var propName = ToPropertyKeyString(index);
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
                var propName = ToPropertyKeyString(index);
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
                var propName = ToPropertyKeyString(index);
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
                var propName = ToPropertyKeyString(index);
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

            var propName = ToPropertyKeyString(index);

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            // ExpandoObject (object literal): assign by property name
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;

                // Legacy __proto__ mutator (opt-in). In JS, setting __proto__ changes [[Prototype]]
                // when the RHS is an object or null; otherwise it is ignored.
                if (PrototypeChain.Enabled && string.Equals(propName, "__proto__", StringComparison.Ordinal))
                {
                    if (IsValidPrototypeValue(value))
                    {
                        PrototypeChain.SetPrototype(obj, value);
                    }
                    return value;
                }

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
        /// Gets an iterator for for..of using the iterator protocol.
        /// Supports:
        ///  - Arrays, strings, Int32Array
        ///  - User-defined iterables via [Symbol.iterator]
        ///  - .NET IEnumerable as a best-effort fallback
        /// </summary>
        public static IJavaScriptIterator GetIterator(object? iterable)
        {
            if (iterable is null || iterable is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot iterate over null or undefined");
            }

            // Native iterator objects can be used directly.
            if (iterable is IJavaScriptIterator nativeIterator)
            {
                return nativeIterator;
            }

            // Built-ins
            if (iterable is string s)
            {
                return new StringIterator(s);
            }

            if (iterable is JavaScriptRuntime.Array arr)
            {
                return new ArrayIterator(arr);
            }

            if (iterable is JavaScriptRuntime.Int32Array i32)
            {
                return new Int32ArrayIterator(i32);
            }

            // User-defined iterables: call obj[Symbol.iterator]().
            object? iteratorMethod;
            try
            {
                iteratorMethod = GetItem(iterable, Symbol.iterator);
            }
            catch
            {
                iteratorMethod = null;
            }

            if (iteratorMethod is Delegate del)
            {
                var previousThis = RuntimeServices.SetCurrentThis(iterable);
                try
                {
                    var iteratorObj = Closure.InvokeWithArgs(del, System.Array.Empty<object>(), System.Array.Empty<object>());
                    if (iteratorObj is null)
                    {
                        throw new JavaScriptRuntime.TypeError("Iterator method returned null or undefined");
                    }
                    if (iteratorObj is IJavaScriptIterator native)
                    {
                        return native;
                    }
                    return new DynamicIterator(iteratorObj);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            if (iteratorMethod != null)
            {
                throw new JavaScriptRuntime.TypeError("Symbol.iterator is not a function");
            }

            // Best-effort fallback: treat .NET IEnumerable as iterable.
            if (iterable is System.Collections.IEnumerable en)
            {
                return new EnumerableIterator(en);
            }

            throw new JavaScriptRuntime.TypeError("Object is not iterable");
        }

        /// <summary>
        /// Gets an async iterator for for await..of using the async iterator protocol.
        ///
        /// If [Symbol.asyncIterator] is not present, it falls back to [Symbol.iterator]
        /// and wraps the sync iterator (CreateAsyncFromSyncIterator semantics).
        /// </summary>
        public static IJavaScriptAsyncIterator GetAsyncIterator(object? iterable)
        {
            if (iterable is null || iterable is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot iterate over null or undefined");
            }

            // Native async iterator objects can be used directly.
            // If a sync iterator is provided, wrap it (CreateAsyncFromSyncIterator semantics).
            if (iterable is IJavaScriptAsyncIterator nativeAsyncIterator)
            {
                return nativeAsyncIterator;
            }

            if (iterable is IJavaScriptIterator nativeSyncIterator)
            {
                return new AsyncFromSyncIterator(nativeSyncIterator);
            }

            // User-defined async iterables: call obj[Symbol.asyncIterator]().
            object? asyncIteratorMethod;
            try
            {
                asyncIteratorMethod = GetItem(iterable, Symbol.asyncIterator);
            }
            catch
            {
                asyncIteratorMethod = null;
            }

            if (asyncIteratorMethod is Delegate asyncDel)
            {
                var previousThis = RuntimeServices.SetCurrentThis(iterable);
                try
                {
                    var iteratorObj = Closure.InvokeWithArgs(asyncDel, System.Array.Empty<object>(), System.Array.Empty<object>());
                    if (iteratorObj is null)
                    {
                        throw new JavaScriptRuntime.TypeError("Async iterator method returned null or undefined");
                    }

                    if (iteratorObj is IJavaScriptAsyncIterator native)
                    {
                        return native;
                    }

                    // If the async iterator method returns a sync iterator, it is still valid: we will await its next() result.
                    if (iteratorObj is IJavaScriptIterator sync)
                    {
                        return new AsyncFromSyncIterator(sync);
                    }

                    return new AsyncDynamicIterator(iteratorObj);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            if (asyncIteratorMethod != null)
            {
                throw new JavaScriptRuntime.TypeError("Symbol.asyncIterator is not a function");
            }

            // Fallback: use sync iterator protocol and wrap.
            return new AsyncFromSyncIterator(GetIterator(iterable));
        }

        /// <summary>
        /// Advances an async iterator.
        /// The returned value is always awaited by the compiler.
        /// </summary>
        public static object? AsyncIteratorNext(object iterator)
        {
            if (iterator is IJavaScriptAsyncIterator it)
            {
                return it.Next();
            }

            if (iterator is IJavaScriptIterator sync)
            {
                return sync.Next();
            }

            throw new JavaScriptRuntime.TypeError("Iterator is not an async iterator");
        }

        /// <summary>
        /// Closes an async iterator on abrupt completion.
        /// If iterator has a callable 'return' member, it is invoked.
        ///
        /// The returned value is always awaited by the compiler.
        /// </summary>
        public static object? AsyncIteratorClose(object? iterator)
        {
            if (iterator is null || iterator is JsNull)
            {
                return null;
            }

            if (iterator is IJavaScriptAsyncIterator it)
            {
                if (!it.HasReturn)
                {
                    return null;
                }

                return it.Return();
            }

            // Sync iterator: close synchronously.
            if (iterator is IJavaScriptIterator sync)
            {
                if (!sync.HasReturn)
                {
                    return null;
                }

                sync.Return();
                return null;
            }

            throw new JavaScriptRuntime.TypeError("Iterator is not an iterator");
        }

        /// <summary>
        /// Advances an iterator via the iterator protocol.
        /// Returns an iterator result object of the form: { value, done }.
        /// </summary>
        public static object IteratorNext(object iterator)
        {
            if (iterator is IJavaScriptIterator it)
            {
                return it.Next();
            }

            throw new JavaScriptRuntime.TypeError("Iterator is not an iterator");
        }

        public static bool IteratorResultDone(object iteratorResult)
        {
            if (iteratorResult is IIteratorResult res)
            {
                return res.done;
            }

            // Fallback for foreign iterator results.
            var doneObj = GetItem(iteratorResult, "done");
            return JavaScriptRuntime.TypeUtilities.ToBoolean(doneObj);
        }

        public static object? IteratorResultValue(object iteratorResult)
        {
            if (iteratorResult is IIteratorResult res)
            {
                return res.value;
            }

            // Fallback for foreign iterator results.
            return GetItem(iteratorResult, "value");
        }

        /// <summary>
        /// Closes an iterator on abrupt completion (IteratorClose).
        /// If iterator has a callable 'return' member, it is invoked.
        /// </summary>
        public static void IteratorClose(object? iterator)
        {
            if (iterator is null || iterator is JsNull)
            {
                return;
            }

            if (iterator is IJavaScriptIterator it)
            {
                if (!it.HasReturn)
                {
                    return;
                }

                it.Return();
                return;
            }

            // ExpandoObject: common for user-defined iterators in this runtime.
            if (iterator is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (!dict.TryGetValue("return", out var ret) || ret is null)
                {
                    return;
                }

                if (ret is not Delegate del)
                {
                    throw new JavaScriptRuntime.TypeError("Iterator.return is not a function");
                }

                var previousThis = RuntimeServices.SetCurrentThis(iterator);
                try
                {
                    Closure.InvokeWithArgs(del, System.Array.Empty<object>(), System.Array.Empty<object>());
                    return;
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            // Host object: look for a delegate-valued property.
            var memberValue = GetProperty(iterator, "return");
            if (memberValue is null)
            {
                return;
            }
            if (memberValue is not Delegate memberDel)
            {
                throw new JavaScriptRuntime.TypeError("Iterator.return is not a function");
            }

            var prev = RuntimeServices.SetCurrentThis(iterator);
            try
            {
                Closure.InvokeWithArgs(memberDel, System.Array.Empty<object>(), System.Array.Empty<object>());
            }
            finally
            {
                RuntimeServices.SetCurrentThis(prev);
            }
        }

        private sealed class ArrayIterator : IJavaScriptIterator
        {
            private readonly JavaScriptRuntime.Array _arr;
            private int _index;
            private bool _isClosed;

            public ArrayIterator(JavaScriptRuntime.Array arr)
            {
                _arr = arr;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed || _index >= _arr.Count)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var value = _arr[_index++];
                return new IteratorResultObject(value, done: false);
            }

            public void Return()
            {
                _isClosed = true;
            }
        }

        private sealed class StringIterator : IJavaScriptIterator
        {
            private readonly string _s;
            private int _index;
            private bool _isClosed;

            public StringIterator(string s)
            {
                _s = s;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed || _index >= _s.Length)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var ch = _s[_index++].ToString();
                return new IteratorResultObject(ch, done: false);
            }

            public void Return()
            {
                _isClosed = true;
            }
        }

        private sealed class Int32ArrayIterator : IJavaScriptIterator
        {
            private readonly JavaScriptRuntime.Int32Array _arr;
            private int _index;
            private bool _isClosed;

            public Int32ArrayIterator(JavaScriptRuntime.Int32Array arr)
            {
                _arr = arr;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed || _index >= _arr.length)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var value = (double)_arr[_index++];
                return new IteratorResultObject(value, done: false);
            }

            public void Return()
            {
                _isClosed = true;
            }
        }

        private sealed class EnumerableIterator : IJavaScriptIterator
        {
            private readonly System.Collections.IEnumerator _enumerator;
            private bool _isClosed;

            public EnumerableIterator(System.Collections.IEnumerable en)
            {
                _enumerator = en.GetEnumerator();
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var moved = _enumerator.MoveNext();
                if (!moved)
                {
                    return new IteratorResultObject(null, done: true);
                }

                return new IteratorResultObject(_enumerator.Current, done: false);
            }

            public void Return()
            {
                _isClosed = true;
                if (_enumerator is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }

        private sealed class DynamicIterator : IJavaScriptIterator
        {
            private readonly object _iterator;
            private readonly Delegate _next;
            private readonly object? _return;

            public DynamicIterator(object iterator)
            {
                _iterator = iterator;

                var nextMember = GetProperty(_iterator, "next");
                if (nextMember is not Delegate nextDel)
                {
                    throw new JavaScriptRuntime.TypeError("Iterator.next is not a function");
                }
                _next = nextDel;

                _return = GetProperty(_iterator, "return");
            }

            public bool HasReturn => _return != null;

            public IteratorResultObject Next()
            {
                var previousThis = RuntimeServices.SetCurrentThis(_iterator);
                try
                {
                    var result = Closure.InvokeWithArgs(_next, System.Array.Empty<object>(), System.Array.Empty<object>());
                    if (result is IteratorResultObject ro)
                    {
                        return ro;
                    }

                    if (result is IIteratorResult ir)
                    {
                        return new IteratorResultObject(ir.value, ir.done);
                    }

                    // Normalize foreign iterator results to a strongly-typed shape.
                    var doneObj = GetItem(result, "done");
                    var done = JavaScriptRuntime.TypeUtilities.ToBoolean(doneObj);
                    var value = GetItem(result, "value");
                    return new IteratorResultObject(value, done);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            public void Return()
            {
                if (_return is null)
                {
                    return;
                }

                if (_return is not Delegate del)
                {
                    throw new JavaScriptRuntime.TypeError("Iterator.return is not a function");
                }

                var previousThis = RuntimeServices.SetCurrentThis(_iterator);
                try
                {
                    Closure.InvokeWithArgs(del, System.Array.Empty<object>(), System.Array.Empty<object>());
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }
        }

        private sealed class AsyncFromSyncIterator : IJavaScriptAsyncIterator
        {
            private readonly IJavaScriptIterator _sync;

            public AsyncFromSyncIterator(IJavaScriptIterator sync)
            {
                _sync = sync;
            }

            public bool HasReturn => _sync.HasReturn;

            public object? Next()
            {
                // Async-from-sync iterator semantics: next() returns a Promise resolved
                // with the underlying sync iterator result.
                return JavaScriptRuntime.Promise.resolve(_sync.Next());
            }

            public object? Return()
            {
                _sync.Return();
                // Async iterator close awaits the return() result, so return a resolved promise.
                return JavaScriptRuntime.Promise.resolve(null);
            }
        }

        private sealed class AsyncDynamicIterator : IJavaScriptAsyncIterator
        {
            private readonly object _iterator;
            private readonly Delegate _next;
            private readonly object? _return;

            public AsyncDynamicIterator(object iterator)
            {
                _iterator = iterator;

                var nextMember = GetProperty(_iterator, "next");
                if (nextMember is not Delegate nextDel)
                {
                    throw new JavaScriptRuntime.TypeError("Iterator.next is not a function");
                }
                _next = nextDel;

                _return = GetProperty(_iterator, "return");
            }

            public bool HasReturn => _return != null;

            public object? Next()
            {
                var previousThis = RuntimeServices.SetCurrentThis(_iterator);
                try
                {
                    return Closure.InvokeWithArgs(_next, System.Array.Empty<object>(), System.Array.Empty<object>());
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            public object? Return()
            {
                if (_return is null)
                {
                    return null;
                }

                if (_return is not Delegate del)
                {
                    throw new JavaScriptRuntime.TypeError("Iterator.return is not a function");
                }

                var previousThis = RuntimeServices.SetCurrentThis(_iterator);
                try
                {
                    return Closure.InvokeWithArgs(del, System.Array.Empty<object>(), System.Array.Empty<object>());
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }
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
        /// Object spread helper used by object literals: { ...source }.
        /// Copies enumerable own properties from <paramref name="source"/> into <paramref name="target"/>
        /// and returns <paramref name="target"/>.
        ///
        /// Minimal semantics:
        ///  - null/undefined (null or JsNull) are ignored
        ///  - ExpandoObject: copy all keys
        ///  - JavaScriptRuntime.Array / Int32Array / string: copy index keys
        ///  - IDictionary&lt;string, object?&gt;: copy keys
        ///  - host objects: copy public instance properties/fields
        ///
        /// This intentionally does not model full ECMAScript property attributes or symbol keys.
        /// </summary>
        public static object SpreadInto(object target, object? source)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Per object spread semantics: null/undefined are skipped.
            if (source is null || source is JsNull)
            {
                return target;
            }

            // ExpandoObject: copy all enumerable own properties.
            if (source is System.Dynamic.ExpandoObject exp)
            {
                var src = (IDictionary<string, object?>)exp;
                foreach (var kvp in src)
                {
                    SetProperty(target, kvp.Key, kvp.Value);
                }
                return target;
            }

            // IDictionary<string, object?>: treat keys as enumerable properties.
            if (source is IDictionary<string, object?> dict)
            {
                foreach (var kvp in dict)
                {
                    SetProperty(target, kvp.Key, kvp.Value);
                }
                return target;
            }

            // Strings: spread copies enumerable index properties ("0", "1", ...).
            if (source is string s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    SetProperty(target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), s[i].ToString());
                }
                return target;
            }

            // JavaScriptRuntime.Array: spread copies element indices.
            if (source is Array arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    SetProperty(target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), arr[i]);
                }
                return target;
            }

            // Int32Array: copy numeric indices as properties.
            if (source is Int32Array i32)
            {
                var len = (int)i32.length;
                for (int i = 0; i < len; i++)
                {
                    SetProperty(target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), i32[(double)i]);
                }
                return target;
            }

            // Reflection fallback for host objects: copy public instance properties/fields.
            try
            {
                var type = source.GetType();
                foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead))
                {
                    SetProperty(target, p.Name, p.GetValue(source));
                }
                foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    SetProperty(target, f.Name, f.GetValue(source));
                }
            }
            catch (AmbiguousMatchException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }
            catch (MethodAccessException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }
            catch (NotSupportedException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }
            catch (TargetInvocationException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }
            catch (TargetException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }
            catch (ArgumentException)
            {
                // Swallow to mimic JS permissiveness on exotic host objects.
            }

            return target;
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
            catch (AmbiguousMatchException)
            {
                // Best-effort; return whatever we could copy.
            }
            catch (MethodAccessException)
            {
                // Best-effort; return whatever we could copy.
            }
            catch (NotSupportedException)
            {
                // Best-effort; return whatever we could copy.
            }
            catch (TargetInvocationException)
            {
                // Best-effort; return whatever we could copy.
            }
            catch (TargetException)
            {
                // Best-effort; return whatever we could copy.
            }
            catch (ArgumentException)
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
        /// ECMA-262 14.7.5.9 EnumerateObjectProperties (O).
        /// Returns a For-In Iterator object that yields enumerable property keys.
        ///
        /// This is mutation-aware (e.g., deletion during enumeration) and is used by the compiler's
        /// for..in lowering.
        /// </summary>
        public static IJavaScriptIterator EnumerateObjectProperties(object? obj)
        {
            return CreateForInIterator(obj);
        }

        /// <summary>
        /// ECMA-262 14.7.5.10.1 CreateForInIterator (object).
        /// Returns a native iterator implementing the For-In iterator protocol.
        /// </summary>
        public static IJavaScriptIterator CreateForInIterator(object? obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Right-hand side of 'for...in' should be an object");
            }

            return new ForInIterator(obj);
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
        /// Implements the JavaScript <c>delete obj.prop</c> runtime semantics (minimal).
        /// Returns true if the deletion succeeds or the property is not present.
        /// </summary>
        public static bool DeleteProperty(object? receiver, object? propName)
        {
            if (receiver is null || receiver is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot convert undefined or null to object");
            }

            var key = DotNet2JSConversions.ToString(propName);

            if (receiver is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                dict.Remove(key);
                return true;
            }

            if (receiver is System.Collections.IDictionary dictObj)
            {
                // Fast path for string-keyed dictionaries.
                if (dictObj.Contains(key))
                {
                    dictObj.Remove(key);
                    return true;
                }

                // Best-effort: find a matching key by ToString conversion.
                object? match = null;
                foreach (var k in dictObj.Keys)
                {
                    if (string.Equals(DotNet2JSConversions.ToString(k), key, StringComparison.Ordinal))
                    {
                        match = k;
                        break;
                    }
                }
                if (match != null)
                {
                    dictObj.Remove(match);
                }
                return true;
            }

            // Arrays/typed arrays/strings and CLR objects: deletion is a no-op in this runtime for now.
            // (Full JS semantics for array holes and non-configurable properties are not modeled.)
            return true;
        }

        /// <summary>
        /// Implements the JavaScript <c>delete obj[index]</c> runtime semantics (minimal).
        /// </summary>
        public static bool DeleteItem(object? receiver, object? index)
        {
            return DeleteProperty(receiver, DotNet2JSConversions.ToString(index));
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
            int i = 0;
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

            // Legacy __proto__ accessor (opt-in)
            if (PrototypeChain.Enabled && string.Equals(name, "__proto__", StringComparison.Ordinal))
            {
                return PrototypeChain.TryGetPrototype(obj, out var proto) ? proto : null;
            }

            static bool TryGetOwnProperty(object target, string propName, out object? value)
            {
                // ExpandoObject properties
                if (target is System.Dynamic.ExpandoObject exp)
                {
                    var dict = (IDictionary<string, object?>)exp;
                    if (dict.TryGetValue(propName, out var v))
                    {
                        value = v;
                        return true;
                    }

                    value = null;
                    return false;
                }

                // JavaScriptRuntime.Array / typed arrays: no custom properties yet
                if (target is Array || target is Int32Array)
                {
                    value = null;
                    return false;
                }

                object? GetValue(Type type)
                {
                    var prop = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                    if (prop != null && prop.CanRead)
                    {
                        return prop.GetValue(target);
                    }
                    var field = type.GetField(propName, BindingFlags.Instance | BindingFlags.Public);
                    if (field != null)
                    {
                        return field.GetValue(target);
                    }

                    var baseType = type.BaseType;
                    if (baseType != null && baseType != typeof(object))
                    {
                        return GetValue(baseType);
                    }

                    return null;
                }

                // Reflection fallback
                try
                {
                    value = GetValue(target.GetType());
                    return value != null;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }

            if (TryGetOwnProperty(obj, name, out var ownValue))
            {
                return ownValue;
            }

            // Prototype-chain lookup (opt-in)
            if (!PrototypeChain.Enabled)
            {
                return null;
            }

            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            object? current = obj;
            while (current != null && visited.Add(current))
            {
                var proto = PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    return null;
                }

                if (TryGetOwnProperty(proto, name, out var inherited))
                {
                    return inherited;
                }

                current = proto;
            }

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

            // Legacy __proto__ mutator (opt-in). In JS, setting __proto__ changes [[Prototype]]
            // when the RHS is an object or null; otherwise it is ignored.
            if (PrototypeChain.Enabled && string.Equals(name, "__proto__", StringComparison.Ordinal))
            {
                if (IsValidPrototypeValue(value))
                {
                    PrototypeChain.SetPrototype(obj, value);
                }
                return value;
            }

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
                throw new TypeError($"{methodName} is not a function");
            }

            var srcArgs = args ?? System.Array.Empty<object>();

            // Prefer js2il-style methods with a leading scopes array: (object[] scopes, [object x N])
            MethodInfo? chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 + srcArgs.Length && ps.Length >= 1 && ps[0].ParameterType == typeof(object[]);
            });

            // Next: prefer a single-parameter params object[] method
            chosen ??= methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });

            // Next: exact parameter count match
            chosen ??= methods.FirstOrDefault(mi => mi.GetParameters().Length == srcArgs.Length);

            if (chosen == null)
            {
                // Last resort: pick the first and attempt object[] packing when possible
                chosen = methods.OrderBy(mi => mi.GetParameters().Length).First();
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var expectsLeadingScopes = psChosen.Length >= 2 && psChosen[0].ParameterType == typeof(object[]);
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

            var src = srcArgs;

            // Resolve scopes for js2il-style calls.
            object[] ResolveScopesArray(object target)
            {
                try
                {
                    var scopesField = target.GetType().GetField("_scopes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (scopesField?.GetValue(target) is object[] s && s.Length > 0)
                    {
                        return s;
                    }
                }
                catch
                {
                    // ignore and fall back
                }

                // ABI-compatible default: 1-element array with null.
                return new object[] { null! };
            }

            // JavaScript semantics: extra args are ignored (unless the target explicitly accepts params object[]).
            // Also ensure we never write past the end of the argument array.
            if (expectsParamsArray && !expectsLeadingScopes)
            {
                var coerced = new object[src.Length];
                for (int i = 0; i < src.Length; i++)
                {
                    coerced[i] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
                }

                var previousThis = RuntimeServices.SetCurrentThis(instance);
                try
                {
                    return chosen.Invoke(instance, new object?[] { coerced });
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            if (expectsLeadingScopes)
            {
                var scopes = ResolveScopesArray(instance);
                var coerced = new object[psChosen.Length];
                coerced[0] = scopes;

                var copyCount = System.Math.Min(src.Length, psChosen.Length - 1);
                for (int i = 0; i < copyCount; i++)
                {
                    coerced[i + 1] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
                }

                var previousThis = RuntimeServices.SetCurrentThis(instance);
                try
                {
                    return chosen.Invoke(instance, coerced);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            {
                var coerced = new object[psChosen.Length];
                var copyCount = System.Math.Min(src.Length, coerced.Length);
                for (int i = 0; i < copyCount; i++)
                {
                    coerced[i] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
                }

                var previousThis = RuntimeServices.SetCurrentThis(instance);
                try
                {
                    return chosen.Invoke(instance, coerced);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }
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

            static bool HasOwnProperty(object target, string name)
            {
                // ExpandoObject (object literal)
                if (target is System.Dynamic.ExpandoObject exp)
                {
                    var dict = (IDictionary<string, object?>)exp;
                    return dict.ContainsKey(name);
                }

                // JS Array (numeric indexes + length)
                if (target is Array jsArr)
                {
                    if (name == "length") return true;
                    if (int.TryParse(name, out var ai))
                    {
                        return ai >= 0 && ai < jsArr.length;
                    }
                    return false;
                }

                // Int32Array (typed array minimal support)
                if (target is Int32Array i32)
                {
                    if (name == "length") return true;
                    if (int.TryParse(name, out var ti))
                    {
                        return ti >= 0 && ti < i32.length;
                    }
                    return false;
                }

                // string (indices + length)
                if (target is string str)
                {
                    if (name == "length") return true;
                    if (int.TryParse(name, out var si))
                    {
                        return si >= 0 && si < str.Length;
                    }
                    return false;
                }

                // Fallback: reflection public instance property/field presence
                var type = target.GetType();
                var pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (pi != null) return true;
                var fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (fi != null) return true;
                return false;
            }

            if (HasOwnProperty(obj, propName))
            {
                return true;
            }

            if (!PrototypeChain.Enabled)
            {
                return false;
            }

            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            object? current = obj;
            while (current != null && visited.Add(current))
            {
                var proto = PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    return false;
                }

                if (HasOwnProperty(proto, propName))
                {
                    return true;
                }

                current = proto;
            }

            return false;
        }
    }
}
