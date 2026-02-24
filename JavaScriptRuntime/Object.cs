using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Object", IntrinsicCallKind.ObjectConstruct)]
    public class Object
    {
        private static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static bool IsNullAssignableTo(Type parameterType)
        {
            return !parameterType.IsValueType || IsNullableValueType(parameterType);
        }

        private static bool TryCoerceConstructorArg(object? value, Type parameterType, out object? coerced)
        {
            // JS undefined/null are represented as CLR null or JsNull.
            if (value is JsNull)
            {
                value = null;
            }

            if (value is null)
            {
                coerced = null;
                return IsNullAssignableTo(parameterType);
            }

            // Exact / reference assignability.
            var valueType = value.GetType();
            if (parameterType.IsAssignableFrom(valueType))
            {
                coerced = value;
                return true;
            }

            // Handle nullable value types by matching against the underlying type.
            var underlying = Nullable.GetUnderlyingType(parameterType);
            var targetType = underlying ?? parameterType;

            // Minimal numeric coercions: JS numbers are typically doubles.
            if (value is double d)
            {
                if (targetType == typeof(double))
                {
                    coerced = d;
                    return true;
                }

                if (targetType == typeof(float))
                {
                    coerced = (float)d;
                    return true;
                }

                if (targetType == typeof(decimal))
                {
                    try
                    {
                        coerced = (decimal)d;
                        return true;
                    }
                    catch
                    {
                        coerced = null;
                        return false;
                    }
                }

                if (targetType == typeof(int))
                {
                    if (!double.IsFinite(d) || !double.IsInteger(d) || d < int.MinValue || d > int.MaxValue)
                    {
                        coerced = null;
                        return false;
                    }
                    coerced = (int)d;
                    return true;
                }

                if (targetType == typeof(long))
                {
                    if (!double.IsFinite(d) || !double.IsInteger(d) || d < long.MinValue || d > long.MaxValue)
                    {
                        coerced = null;
                        return false;
                    }
                    coerced = (long)d;
                    return true;
                }

                if (targetType == typeof(short))
                {
                    if (!double.IsFinite(d) || !double.IsInteger(d) || d < short.MinValue || d > short.MaxValue)
                    {
                        coerced = null;
                        return false;
                    }
                    coerced = (short)d;
                    return true;
                }

                if (targetType == typeof(byte))
                {
                    if (!double.IsFinite(d) || !double.IsInteger(d) || d < byte.MinValue || d > byte.MaxValue)
                    {
                        coerced = null;
                        return false;
                    }
                    coerced = (byte)d;
                    return true;
                }

                if (targetType == typeof(bool))
                {
                    coerced = d != 0;
                    return true;
                }
            }

            // Minimal string coercion.
            if (targetType == typeof(string))
            {
                coerced = DotNet2JSConversions.ToString(value);
                return true;
            }

            coerced = null;
            return false;
        }

        private static bool IsViableConstructorCall(ParameterInfo[] parameters, object[] callArgs)
        {
            // Must be able to supply all parameters (missing args treated as undefined/null).
            if (callArgs.Length > parameters.Length)
            {
                return false;
            }

            // Provided args must be assignable/coercible.
            for (int i = 0; i < callArgs.Length; i++)
            {
                if (!TryCoerceConstructorArg(callArgs[i], parameters[i].ParameterType, out _))
                {
                    return false;
                }
            }

            // Missing args become undefined/null; reject ctors that require non-nullable value types.
            for (int i = callArgs.Length; i < parameters.Length; i++)
            {
                if (!IsNullAssignableTo(parameters[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

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

        private sealed class ObjectIntegrityState
        {
            public bool Extensible = true;
            public bool Sealed;
            public bool Frozen;
        }

        private static readonly ConditionalWeakTable<object, ObjectIntegrityState> _integrityStates = new();
        private static readonly ConcurrentDictionary<string, Symbol> _encodedSymbolKeys = new(StringComparer.Ordinal);

        private static ObjectIntegrityState GetIntegrityState(object target) => _integrityStates.GetOrCreateValue(target);

        private static bool IsExtensibleInternal(object target)
        {
            return !_integrityStates.TryGetValue(target, out var state) || state.Extensible;
        }

        private static bool IsEncodedSymbolKey(string? key)
        {
            return !string.IsNullOrEmpty(key) && _encodedSymbolKeys.ContainsKey(key);
        }

        private static bool TryDecodeEncodedSymbolKey(string? key, [NotNullWhen(true)] out Symbol? symbol)
        {
            if (!string.IsNullOrEmpty(key) && _encodedSymbolKeys.TryGetValue(key, out var mapped))
            {
                symbol = mapped;
                return true;
            }

            symbol = null;
            return false;
        }

        /// <summary>
        /// Implements <c>Object.is(value1, value2)</c> via SameValue semantics.
        /// </summary>
        public static bool @is(object? value1, object? value2) => Operators.SameValue(value1, value2);

        private static readonly Func<object[], object?, object> _objectGetOwnPropertyNamesValue = static (_, value) =>
            getOwnPropertyNames(value!);
        private static readonly Func<object[], object?, object> _objectKeysValue = static (_, value) =>
            keys(value!);
        private static readonly Func<object[], object?, object> _objectValuesValue = static (_, value) =>
            values(value!);
        private static readonly Func<object[], object?, object> _objectEntriesValue = static (_, value) =>
            entries(value!);
        private static readonly Func<object[], object?[], object> _objectAssignValue = static (_, args) =>
        {
            var target = (args == null || args.Length == 0) ? null : args[0];
            var sources = (args != null && args.Length > 1)
                ? args.Skip(1).ToArray()
                : System.Array.Empty<object?>();
            return assign(target!, sources);
        };
        private static readonly Func<object[], object?, object> _objectFromEntriesValue = static (_, iterable) =>
            fromEntries(iterable!);
        private static readonly Func<object[], object?[], object> _objectCreateValue = static (_, args) =>
        {
            var prototype = (args != null && args.Length > 0) ? args[0] : null;
            var properties = (args != null && args.Length > 1) ? args[1] : null;
            return create(prototype, properties);
        };
        private static readonly Func<object[], object?[], object> _objectDefinePropertyValue = static (_, args) =>
        {
            var target = (args != null && args.Length > 0) ? args[0] : null;
            var prop = (args != null && args.Length > 1) ? args[1] : null;
            var attributes = (args != null && args.Length > 2) ? args[2] : null;
            return defineProperty(target!, prop, attributes);
        };
        private static readonly Func<object[], object?[], object> _objectDefinePropertiesValue = static (_, args) =>
        {
            var target = (args != null && args.Length > 0) ? args[0] : null;
            var properties = (args != null && args.Length > 1) ? args[1] : null;
            return defineProperties(target!, properties);
        };
        private static readonly Func<object[], object?[], object> _objectGetOwnPropertyDescriptorValue = static (_, args) =>
        {
            var target = (args != null && args.Length > 0) ? args[0] : null;
            var prop = (args != null && args.Length > 1) ? args[1] : null;
            return getOwnPropertyDescriptor(target!, prop)!;
        };
        private static readonly Func<object[], object?, object> _objectGetOwnPropertyDescriptorsValue = static (_, value) =>
            getOwnPropertyDescriptors(value!);
        private static readonly Func<object[], object?, object> _objectGetOwnPropertySymbolsValue = static (_, value) =>
            getOwnPropertySymbols(value!);
        private static readonly Func<object[], object?, object> _objectGetPrototypeOfValue = static (_, value) =>
            getPrototypeOf(value!)!;
        private static readonly Func<object[], object?[], object> _objectSetPrototypeOfValue = static (_, args) =>
        {
            var target = (args != null && args.Length > 0) ? args[0] : null;
            var prototype = (args != null && args.Length > 1) ? args[1] : null;
            return setPrototypeOf(target!, prototype);
        };
        private static readonly Func<object[], object?[], object> _objectHasOwnValue = static (_, args) =>
        {
            var target = (args != null && args.Length > 0) ? args[0] : null;
            var prop = (args != null && args.Length > 1) ? args[1] : null;
            return hasOwn(target!, prop);
        };
        private static readonly Func<object[], object?[], object> _objectIsValue = static (_, args) =>
        {
            var value1 = (args != null && args.Length > 0) ? args[0] : null;
            var value2 = (args != null && args.Length > 1) ? args[1] : null;
            return @is(value1, value2);
        };
        private static readonly Func<object[], object?, object> _objectPreventExtensionsValue = static (_, value) =>
            preventExtensions(value!);
        private static readonly Func<object[], object?, object> _objectIsExtensibleValue = static (_, value) =>
            isExtensible(value!);
        private static readonly Func<object[], object?, object> _objectSealValue = static (_, value) =>
            seal(value!);
        private static readonly Func<object[], object?, object> _objectFreezeValue = static (_, value) =>
            freeze(value!);
        private static readonly Func<object[], object?, object> _objectIsSealedValue = static (_, value) =>
            isSealed(value!);
        private static readonly Func<object[], object?, object> _objectIsFrozenValue = static (_, value) =>
            isFrozen(value!);
        private static readonly Func<object[], object?[], object> _objectGroupByValue = static (_, args) =>
        {
            var items = (args != null && args.Length > 0) ? args[0] : null;
            var callback = (args != null && args.Length > 1) ? args[1] : null;
            return groupBy(items!, callback!);
        };

        private static readonly Func<object[], object?[], object?> _objectPrototypeHasOwnPropertyValue = PrototypeHasOwnProperty;
        private static readonly Func<object[], object?[], object?> _objectPrototypeIsPrototypeOfValue = PrototypeIsPrototypeOf;
        private static readonly Func<object[], object?[], object?> _objectPrototypePropertyIsEnumerableValue = PrototypePropertyIsEnumerable;
        private static readonly Func<object[], object?[], object?> _objectPrototypeToLocaleStringValue = PrototypeToLocaleString;
        private static readonly Func<object[], object?[], object?> _objectPrototypeToStringValue = PrototypeToString;
        private static readonly Func<object[], object?[], object?> _objectPrototypeValueOfValue = PrototypeValueOf;
        private static readonly Func<object[], object?[], object?> _objectPrototypeDefineGetterValue = PrototypeDefineGetter;
        private static readonly Func<object[], object?[], object?> _objectPrototypeDefineSetterValue = PrototypeDefineSetter;
        private static readonly Func<object[], object?[], object?> _objectPrototypeLookupGetterValue = PrototypeLookupGetter;
        private static readonly Func<object[], object?[], object?> _objectPrototypeLookupSetterValue = PrototypeLookupSetter;

        private static void DefineBuiltinDataProperty(object target, string name, object? value, bool enumerable = false, bool configurable = true, bool writable = true)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = enumerable,
                Configurable = configurable,
                Writable = writable,
                Value = value
            });
        }

        public static void ConfigureIntrinsicSurface(object objectConstructorValue, object objectPrototypeValue)
        {
            DefineBuiltinDataProperty(objectConstructorValue, "prototype", objectPrototypeValue, enumerable: false, configurable: true, writable: true);

            DefineBuiltinDataProperty(objectConstructorValue, "assign", _objectAssignValue);
            DefineBuiltinDataProperty(objectConstructorValue, "create", _objectCreateValue);
            DefineBuiltinDataProperty(objectConstructorValue, "defineProperties", _objectDefinePropertiesValue);
            DefineBuiltinDataProperty(objectConstructorValue, "defineProperty", _objectDefinePropertyValue);
            DefineBuiltinDataProperty(objectConstructorValue, "entries", _objectEntriesValue);
            DefineBuiltinDataProperty(objectConstructorValue, "freeze", _objectFreezeValue);
            DefineBuiltinDataProperty(objectConstructorValue, "fromEntries", _objectFromEntriesValue);
            DefineBuiltinDataProperty(objectConstructorValue, "getOwnPropertyDescriptor", _objectGetOwnPropertyDescriptorValue);
            DefineBuiltinDataProperty(objectConstructorValue, "getOwnPropertyDescriptors", _objectGetOwnPropertyDescriptorsValue);
            DefineBuiltinDataProperty(objectConstructorValue, "getOwnPropertyNames", _objectGetOwnPropertyNamesValue);
            DefineBuiltinDataProperty(objectConstructorValue, "getOwnPropertySymbols", _objectGetOwnPropertySymbolsValue);
            DefineBuiltinDataProperty(objectConstructorValue, "getPrototypeOf", _objectGetPrototypeOfValue);
            DefineBuiltinDataProperty(objectConstructorValue, "groupBy", _objectGroupByValue);
            DefineBuiltinDataProperty(objectConstructorValue, "hasOwn", _objectHasOwnValue);
            DefineBuiltinDataProperty(objectConstructorValue, "is", _objectIsValue);
            DefineBuiltinDataProperty(objectConstructorValue, "isExtensible", _objectIsExtensibleValue);
            DefineBuiltinDataProperty(objectConstructorValue, "isFrozen", _objectIsFrozenValue);
            DefineBuiltinDataProperty(objectConstructorValue, "isSealed", _objectIsSealedValue);
            DefineBuiltinDataProperty(objectConstructorValue, "keys", _objectKeysValue);
            DefineBuiltinDataProperty(objectConstructorValue, "preventExtensions", _objectPreventExtensionsValue);
            DefineBuiltinDataProperty(objectConstructorValue, "seal", _objectSealValue);
            DefineBuiltinDataProperty(objectConstructorValue, "setPrototypeOf", _objectSetPrototypeOfValue);
            DefineBuiltinDataProperty(objectConstructorValue, "values", _objectValuesValue);

            DefineBuiltinDataProperty(objectPrototypeValue, "constructor", objectConstructorValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "hasOwnProperty", _objectPrototypeHasOwnPropertyValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "isPrototypeOf", _objectPrototypeIsPrototypeOfValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "propertyIsEnumerable", _objectPrototypePropertyIsEnumerableValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "toLocaleString", _objectPrototypeToLocaleStringValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "toString", _objectPrototypeToStringValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "valueOf", _objectPrototypeValueOfValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "__defineGetter__", _objectPrototypeDefineGetterValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "__defineSetter__", _objectPrototypeDefineSetterValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "__lookupGetter__", _objectPrototypeLookupGetterValue, enumerable: false, configurable: true, writable: true);
            DefineBuiltinDataProperty(objectPrototypeValue, "__lookupSetter__", _objectPrototypeLookupSetterValue, enumerable: false, configurable: true, writable: true);
        }

        /// <summary>
        /// Minimal implementation of <c>Object.getPrototypeOf(obj)</c>.
        /// Note: this runtime does not currently model a default Object.prototype; if no prototype
        /// has been explicitly assigned, this returns CLR null (the runtime's representation of
        /// JavaScript <c>undefined</c>).
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
        /// ECMA-262 Object.create(proto [, properties]).
        /// Minimal implementation used for descriptor/prototype-heavy libraries (e.g., domino).
        /// </summary>
        public static object create(object? prototype)
        {
            return create(prototype, properties: null);
        }

        /// <summary>
        /// ECMA-262 Object.create(proto [, properties]).
        /// </summary>
        public static object create(object? prototype, object? properties)
        {
            if (!IsValidPrototypeValue(prototype))
            {
                throw new TypeError("Object prototype may only be an Object or null");
            }

            var obj = new System.Dynamic.ExpandoObject();

            // Explicitly set [[Prototype]] (including null-proto via JsNull).
            PrototypeChain.SetPrototype(obj, prototype);

            // Per ToObject(null/undefined), an explicit null properties argument must throw.
            if (properties is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (properties is not null)
            {
                defineProperties(obj, properties);
            }

            return obj;
        }

        /// <summary>
        /// ECMA-262 Object.getOwnPropertyDescriptor(O, P).
        /// Returns undefined (CLR null) when the property is not an own property.
        /// </summary>
        public static object? getOwnPropertyDescriptor(object obj, object? prop)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var key = ToPropertyKeyString(prop);

            if (TryGetOwnPropertyDescriptor(obj, key, out var desc))
            {
                return CreateDescriptorObject(desc);
            }

            return null;
        }

        /// <summary>
        /// ECMA-262 Object.getOwnPropertyNames(O).
        /// Minimal implementation returning string keys for own properties.
        /// </summary>
        public static object getOwnPropertyNames(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var keys = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            static void AddKey(List<string> keys, HashSet<string> seen, string? key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }

                if (IsEncodedSymbolKey(key))
                {
                    return;
                }

                if (seen.Add(key))
                {
                    keys.Add(key);
                }
            }

            foreach (var k in PropertyDescriptorStore.GetOwnKeys(obj))
            {
                AddKey(keys, seen, k);
            }

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                foreach (var k in dict.Keys)
                {
                    AddKey(keys, seen, k);
                }
            }

            if (obj is IDictionary<string, object?> dictGeneric)
            {
                foreach (var k in dictGeneric.Keys)
                {
                    AddKey(keys, seen, k);
                }
                // Skip CLR reflection for dictionary-backed objects: their own properties
                // are already captured above. Falling through to reflection would expose
                // the CLR interface properties (Keys, Values, Count, ...) as JS properties.
                return new JavaScriptRuntime.Array(keys);
            }

            if (obj is System.Collections.IDictionary dictObj)
            {
                var convertedKeys = new List<string>();
                foreach (var k in dictObj.Keys)
                {
                    convertedKeys.Add(DotNet2JSConversions.ToString(k));
                }

                // IDictionary key ordering can be unstable; sort for determinism.
                convertedKeys.Sort(StringComparer.Ordinal);
                foreach (var k in convertedKeys)
                {
                    AddKey(keys, seen, k);
                }
                return new JavaScriptRuntime.Array(keys);
            }

            // Reflection fallback for host objects.
            var type = obj.GetType();
            foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                AddKey(keys, seen, p.Name);
            }
            foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public).OrderBy(f => f.Name, StringComparer.Ordinal))
            {
                AddKey(keys, seen, f.Name);
            }

            return new JavaScriptRuntime.Array(keys);
        }

        /// <summary>
        /// ECMA-262 Object.keys(O).
        /// Returns an array of own enumerable string keys.
        /// </summary>
        public static object keys(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var keys = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            EnumerateOwnEnumerableProperties(
                obj,
                seen,
                (name, _) => keys.Add(name));

            return new JavaScriptRuntime.Array(keys);
        }

        /// <summary>
        /// ECMA-262 Object.values(O).
        /// Returns an array of own enumerable property values.
        /// </summary>
        public static object values(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var values = new List<object?>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            EnumerateOwnEnumerableProperties(
                obj,
                seen,
                (_, value) => values.Add(value));

            return new JavaScriptRuntime.Array(values);
        }

        /// <summary>
        /// ECMA-262 Object.entries(O).
        /// Returns an array of [key, value] pairs for own enumerable properties.
        /// </summary>
        public static object entries(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var entries = new List<object?>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            EnumerateOwnEnumerableProperties(
                obj,
                seen,
                (name, value) =>
                {
                    var pair = new JavaScriptRuntime.Array();
                    pair.Add(name);
                    pair.Add(value);
                    entries.Add(pair);
                });

            return new JavaScriptRuntime.Array(entries);
        }

        private static void EnumerateOwnEnumerableProperties(object obj, ISet<string> seen, Action<string, object?> processProperty, bool includeEncodedSymbolKeys = false)
        {
            foreach (var k in PropertyDescriptorStore.GetOwnKeys(obj))
            {
                if (!includeEncodedSymbolKeys && IsEncodedSymbolKey(k))
                {
                    continue;
                }

                if (!PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, k))
                {
                    continue;
                }

                if (seen.Add(k))
                {
                    object? value = null;
                    if (!TryGetOwnPropertyValue(obj, k, out value))
                    {
                        value = GetProperty(obj, k);
                    }

                    processProperty(k, value);
                }
            }

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                foreach (var kvp in dict)
                {
                    if (!includeEncodedSymbolKeys && IsEncodedSymbolKey(kvp.Key))
                    {
                        continue;
                    }

                    if (PropertyDescriptorStore.IsEnumerableOrDefaultTrue(exp, kvp.Key) && seen.Add(kvp.Key))
                    {
                        processProperty(kvp.Key, kvp.Value);
                    }
                }

                return;
            }

            if (obj is IDictionary<string, object?> dictGeneric)
            {
                foreach (var kvp in dictGeneric)
                {
                    if (!includeEncodedSymbolKeys && IsEncodedSymbolKey(kvp.Key))
                    {
                        continue;
                    }

                    if (PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, kvp.Key) && seen.Add(kvp.Key))
                    {
                        processProperty(kvp.Key, kvp.Value);
                    }
                }

                return;
            }

            if (obj is System.Collections.IDictionary dictObj)
            {
                var sortedKeys = new List<string>();
                foreach (var k in dictObj.Keys)
                {
                    sortedKeys.Add(DotNet2JSConversions.ToString(k));
                }

                sortedKeys.Sort(StringComparer.Ordinal);
                foreach (var k in sortedKeys)
                {
                    if (!includeEncodedSymbolKeys && IsEncodedSymbolKey(k))
                    {
                        continue;
                    }

                    if (PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, k) && seen.Add(k))
                    {
                        processProperty(k, dictObj[k]);
                    }
                }

                return;
            }

            var type = obj.GetType();
            foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                if (PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, p.Name) && seen.Add(p.Name))
                {
                    processProperty(p.Name, p.GetValue(obj));
                }
            }

            foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public).OrderBy(f => f.Name, StringComparer.Ordinal))
            {
                if (PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, f.Name) && seen.Add(f.Name))
                {
                    processProperty(f.Name, f.GetValue(obj));
                }
            }
        }

        /// <summary>
        /// ECMA-262 Object.assign(target, ...sources).
        /// Copies enumerable own properties from source objects to the target.
        /// </summary>
        public static object assign(object target, params object?[] sources)
        {
            if (target is null || target is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            foreach (var source in sources)
            {
                // Skip null/undefined sources
                if (source is null || source is JsNull)
                {
                    continue;
                }

                // Use SpreadInto which handles enumerable own properties
                SpreadInto(target, source);
            }

            return target;
        }

        /// <summary>
        /// ECMA-262 Object.fromEntries(iterable).
        /// Creates an object from an iterable of [key, value] pairs.
        /// </summary>
        public static object fromEntries(object iterable)
        {
            if (iterable is null || iterable is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)result;

            // Get iterator for the iterable
            var iterator = GetIterator(iterable);

            try
            {
                while (true)
                {
                    var iterResult = IteratorNext(iterator);
                    if (IteratorResultDone(iterResult))
                    {
                        break;
                    }

                    var entry = IteratorResultValue(iterResult);
                    if (entry is null || entry is JsNull)
                    {
                        throw new TypeError("Iterator value must be an object");
                    }

                    // Extract key and value from the entry
                    // Entry should be array-like with length >= 2
                    object? key;
                    object? value;

                    if (entry is JavaScriptRuntime.Array arr)
                    {
                        if (arr.Count < 2)
                        {
                            throw new TypeError("Iterator value must have at least 2 elements");
                        }
                        key = arr[0];
                        value = arr[1];
                    }
                    else if (entry is System.Collections.IList list)
                    {
                        if (list.Count < 2)
                        {
                            throw new TypeError("Iterator value must have at least 2 elements");
                        }
                        key = list[0];
                        value = list[1];
                    }
                    else
                    {
                        // Try to get via indexed access
                        key = GetItem(entry, 0.0);
                        value = GetItem(entry, 1.0);
                    }

                    var keyStr = ToPropertyKeyString(key);
                    dict[keyStr] = value;
                }
            }
            finally
            {
                IteratorClose(iterator);
            }

            return result;
        }

        /// <summary>
        /// ECMA-262 Object.defineProperty(O, P, Attributes).
        /// Returns the target object.
        /// </summary>
        public static object defineProperty(object obj, object? prop, object? attributes)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (attributes is null || attributes is JsNull)
            {
                throw new TypeError("Property description must be an object");
            }

            var key = ToPropertyKeyString(prop);

            if (!IsExtensibleInternal(obj) && !HasOwnProperty(obj, key))
            {
                throw new TypeError("Cannot define property on non-extensible object");
            }

            var hasExistingDescriptor = PropertyDescriptorStore.TryGetOwn(obj, key, out var existingDescriptor);

            // Determine descriptor kind.
            // Prefer the resolved get/set values; this is robust even when the attribute object
            // is not a plain ExpandoObject.
            var getValue = GetProperty(attributes, "get");
            var setValue = GetProperty(attributes, "set");
            bool isAccessor = (getValue is not null && getValue is not JsNull)
                || (setValue is not null && setValue is not JsNull)
                || HasOwnProperty(attributes, "get")
                || HasOwnProperty(attributes, "set");

            // In ECMAScript, absent boolean fields default to false.
            bool enumerable = HasOwnProperty(attributes, "enumerable") && TypeUtilities.ToBoolean(GetProperty(attributes, "enumerable"));
            bool configurable = HasOwnProperty(attributes, "configurable") && TypeUtilities.ToBoolean(GetProperty(attributes, "configurable"));
            bool writable = HasOwnProperty(attributes, "writable") && TypeUtilities.ToBoolean(GetProperty(attributes, "writable"));
            var value = HasOwnProperty(attributes, "value") ? GetProperty(attributes, "value") : null;

            if (hasExistingDescriptor && !existingDescriptor.Configurable)
            {
                if (configurable)
                {
                    throw new TypeError("Cannot redefine non-configurable property");
                }

                if (HasOwnProperty(attributes, "enumerable") && enumerable != existingDescriptor.Enumerable)
                {
                    throw new TypeError("Cannot redefine non-configurable property");
                }

                if (existingDescriptor.Kind != (isAccessor ? JsPropertyDescriptorKind.Accessor : JsPropertyDescriptorKind.Data))
                {
                    throw new TypeError("Cannot redefine non-configurable property");
                }

                if (existingDescriptor.Kind == JsPropertyDescriptorKind.Data)
                {
                    if (!existingDescriptor.Writable)
                    {
                        if (HasOwnProperty(attributes, "writable") && writable)
                        {
                            throw new TypeError("Cannot redefine non-configurable property");
                        }

                        if (HasOwnProperty(attributes, "value") && !Operators.SameValue(existingDescriptor.Value, value))
                        {
                            throw new TypeError("Cannot redefine non-configurable property");
                        }
                    }
                }
                else
                {
                    if (HasOwnProperty(attributes, "get") && !ReferenceEquals(existingDescriptor.Get, getValue))
                    {
                        throw new TypeError("Cannot redefine non-configurable property");
                    }

                    if (HasOwnProperty(attributes, "set") && !ReferenceEquals(existingDescriptor.Set, setValue))
                    {
                        throw new TypeError("Cannot redefine non-configurable property");
                    }
                }
            }

            if (isAccessor)
            {
                var desc = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = enumerable,
                    Configurable = configurable,
                    Get = getValue,
                    Set = setValue
                };

                // Ensure key presence for dictionary-backed object literal enumeration.
                if (obj is IDictionary<string, object?> dict && !dict.ContainsKey(key))
                {
                    dict[key] = null;
                }

                PropertyDescriptorStore.DefineOrUpdate(obj, key, desc);
                return obj;
            }

            var dataDesc = new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = enumerable,
                Configurable = configurable,
                Writable = writable,
                Value = value
            };

            PropertyDescriptorStore.DefineOrUpdate(obj, key, dataDesc);

            // Best-effort backing store update for dictionary-backed objects.
            if (obj is IDictionary<string, object?> dict2)
            {
                dict2[key] = value;
            }

            return obj;
        }

        /// <summary>
        /// ECMA-262 Object.defineProperties(O, Properties).
        /// Minimal implementation with best-effort all-or-throw behavior.
        /// </summary>
        public static object defineProperties(object obj, object? properties)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            // Per ToObject(null/undefined), an explicit null/undefined Properties argument must throw.
            if (properties is null || properties is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            // Snapshot keys first; if we cannot enumerate, throw rather than partially apply.
            var keys = GetEnumerableKeys(properties);
            var pending = new List<(string Key, object? Attributes)>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var k = DotNet2JSConversions.ToString(keys[i]) ?? string.Empty;
                var attrs = GetProperty(properties, k);
                if (attrs is null || attrs is JsNull)
                {
                    throw new TypeError("Property description must be an object");
                }
                pending.Add((k, attrs));
            }

            foreach (var (k, attrs) in pending)
            {
                defineProperty(obj, k, attrs);
            }

            return obj;
        }

        public static bool hasOwn(object obj, object? prop)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var key = ToPropertyKeyString(prop);
            return HasOwnProperty(obj, key);
        }

        public static object getOwnPropertyDescriptors(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var result = new System.Dynamic.ExpandoObject();
            if (getOwnPropertyNames(obj) is JavaScriptRuntime.Array names)
            {
                foreach (var key in names)
                {
                    var descriptor = getOwnPropertyDescriptor(obj, key);
                    if (descriptor is not null)
                    {
                        SetItem(result, key!, descriptor);
                    }
                }
            }

            if (getOwnPropertySymbols(obj) is JavaScriptRuntime.Array symbols)
            {
                foreach (var sym in symbols)
                {
                    var descriptor = getOwnPropertyDescriptor(obj, sym);
                    if (descriptor is not null)
                    {
                        SetItem(result, sym!, descriptor);
                    }
                }
            }

            return result;
        }

        public static object getOwnPropertySymbols(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var symbols = new List<object?>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (!seen.Add(key))
                {
                    continue;
                }

                if (TryDecodeEncodedSymbolKey(key, out var symbol))
                {
                    symbols.Add(symbol);
                }
            }

            return new JavaScriptRuntime.Array(symbols);
        }

        private static bool IsPrimitiveObjectOperationTarget(object obj)
        {
            return obj is string || obj is Symbol || obj.GetType().IsValueType;
        }

        private static IEnumerable<string> GetOwnKeysForIntegrity(object obj)
        {
            var keys = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            static void AddKey(List<string> keys, HashSet<string> seen, string? key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }

                if (seen.Add(key))
                {
                    keys.Add(key);
                }
            }

            foreach (var key in PropertyDescriptorStore.GetOwnKeys(obj))
            {
                AddKey(keys, seen, key);
            }

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                foreach (var key in dict.Keys)
                {
                    AddKey(keys, seen, key);
                }
            }

            if (obj is IDictionary<string, object?> dictGeneric)
            {
                foreach (var key in dictGeneric.Keys)
                {
                    AddKey(keys, seen, key);
                }
            }

            if (obj is System.Collections.IDictionary dictObj)
            {
                foreach (var key in dictObj.Keys)
                {
                    AddKey(keys, seen, DotNet2JSConversions.ToString(key));
                }
            }

            return keys;
        }

        private static void EnsureIntegrityDescriptorsForExistingOwnProperties(object obj)
        {
            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (PropertyDescriptorStore.TryGetOwn(obj, key, out _))
                {
                    continue;
                }

                _ = TryGetOwnPropertyValue(obj, key, out var value);
                PropertyDescriptorStore.DefineOrUpdate(obj, key, new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = true,
                    Configurable = true,
                    Writable = true,
                    Value = value
                });
            }
        }

        public static object preventExtensions(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return obj;
            }

            GetIntegrityState(obj).Extensible = false;
            return obj;
        }

        public static bool isExtensible(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return false;
            }

            return IsExtensibleInternal(obj);
        }

        public static object seal(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return obj;
            }

            EnsureIntegrityDescriptorsForExistingOwnProperties(obj);
            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (!PropertyDescriptorStore.TryGetOwn(obj, key, out var desc))
                {
                    continue;
                }

                desc.Configurable = false;
                PropertyDescriptorStore.DefineOrUpdate(obj, key, desc);
            }

            var state = GetIntegrityState(obj);
            state.Extensible = false;
            state.Sealed = true;
            return obj;
        }

        public static object freeze(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return obj;
            }

            EnsureIntegrityDescriptorsForExistingOwnProperties(obj);
            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (!PropertyDescriptorStore.TryGetOwn(obj, key, out var desc))
                {
                    continue;
                }

                desc.Configurable = false;
                if (desc.Kind == JsPropertyDescriptorKind.Data)
                {
                    desc.Writable = false;
                }
                PropertyDescriptorStore.DefineOrUpdate(obj, key, desc);
            }

            var state = GetIntegrityState(obj);
            state.Extensible = false;
            state.Sealed = true;
            state.Frozen = true;
            return obj;
        }

        public static bool isSealed(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return true;
            }

            if (IsExtensibleInternal(obj))
            {
                return false;
            }

            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (!PropertyDescriptorStore.TryGetOwn(obj, key, out var desc) || desc.Configurable)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isFrozen(object obj)
        {
            if (obj is null || obj is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (IsPrimitiveObjectOperationTarget(obj))
            {
                return true;
            }

            if (!isSealed(obj))
            {
                return false;
            }

            foreach (var key in GetOwnKeysForIntegrity(obj))
            {
                if (!PropertyDescriptorStore.TryGetOwn(obj, key, out var desc))
                {
                    return false;
                }

                if (desc.Kind == JsPropertyDescriptorKind.Data && desc.Writable)
                {
                    return false;
                }
            }

            return true;
        }

        public static object groupBy(object items, object callback)
        {
            if (items is null || items is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            if (callback is null || callback is JsNull || callback is not Delegate)
            {
                throw new TypeError("Object.groupBy callback must be a function");
            }

            var result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)result;
            var iterator = GetIterator(items);
            var index = 0;
            try
            {
                while (true)
                {
                    var step = IteratorNext(iterator);
                    if (IteratorResultDone(step))
                    {
                        break;
                    }

                    var value = IteratorResultValue(step);
                    var keyValue = InvokeCallable(callback, null!, new object?[] { value, (double)index });
                    var key = ToPropertyKeyString(keyValue);

                    if (!dict.TryGetValue(key, out var bucketObj) || bucketObj is not JavaScriptRuntime.Array bucket)
                    {
                        bucket = new JavaScriptRuntime.Array();
                        dict[key] = bucket;
                    }

                    bucket.Add(value);
                    index++;
                }
            }
            finally
            {
                IteratorClose(iterator);
            }

            return result;
        }

        private static object GetCurrentThisObjectOrThrow()
        {
            var target = RuntimeServices.GetCurrentThis();
            if (target is null || target is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            return target;
        }

        private static object? PrototypeHasOwnProperty(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            return hasOwn(target, prop);
        }

        private static object? PrototypeIsPrototypeOf(object[] scopes, object?[] args)
        {
            var prototypeCandidate = GetCurrentThisObjectOrThrow();
            var value = args != null && args.Length > 0 ? args[0] : null;

            if (value is null || value is JsNull || !IsObjectLikeForPrototype(value))
            {
                return false;
            }

            if (!PrototypeChain.Enabled)
            {
                return false;
            }

            var current = value;
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            while (PrototypeChain.TryGetPrototype(current, out var proto) && proto is not null && proto is not JsNull)
            {
                if (!visited.Add(proto))
                {
                    return false;
                }

                if (ReferenceEquals(proto, prototypeCandidate))
                {
                    return true;
                }

                if (!IsObjectLikeForPrototype(proto))
                {
                    return false;
                }

                current = proto;
            }

            return false;
        }

        private static object? PrototypePropertyIsEnumerable(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            var key = ToPropertyKeyString(prop);

            if (PropertyDescriptorStore.TryGetOwn(target, key, out var descriptor))
            {
                return descriptor.Enumerable;
            }

            if (target is ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                return dict.ContainsKey(key);
            }

            if (target is IDictionary<string, object?> dictGeneric)
            {
                return dictGeneric.ContainsKey(key);
            }

            if (target is System.Collections.IDictionary dictObj)
            {
                if (dictObj.Contains(key)) return true;
                foreach (var existingKey in dictObj.Keys)
                {
                    if (string.Equals(DotNet2JSConversions.ToString(existingKey), key, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
                return false;
            }

            var type = target.GetType();
            return type.GetProperty(key, BindingFlags.Instance | BindingFlags.Public) != null
                || type.GetField(key, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        private static object? PrototypeToLocaleString(object[] scopes, object?[] args)
        {
            _ = GetCurrentThisObjectOrThrow();
            return PrototypeToString(scopes, args);
        }

        private static object? PrototypeToString(object[] scopes, object?[] args)
        {
            var thisVal = RuntimeServices.GetCurrentThis();

            if (thisVal == null) return "[object Undefined]";
            if (thisVal is JsNull) return "[object Null]";

            // Try @@toStringTag (Symbol.toStringTag) first.
            var toStringTagSym = Symbol.toStringTag;
            var tag = GetItem(thisVal, toStringTagSym);
            if (tag is string tagStr)
            {
                return $"[object {tagStr}]";
            }

            // Built-in type tags.
            if (thisVal is JavaScriptRuntime.Array) return "[object Array]";
            if (thisVal is string) return "[object String]";
            if (thisVal is bool) return "[object Boolean]";
            if (thisVal is double or float or int or long) return "[object Number]";
            if (thisVal is Delegate) return "[object Function]";
            if (thisVal is JavaScriptRuntime.RegExp) return "[object RegExp]";
            if (thisVal is GeneratorObject) return "[object Generator]";
            if (thisVal is AsyncGeneratorObject) return "[object AsyncGenerator]";

            return "[object Object]";
        }

        private static object? PrototypeValueOf(object[] scopes, object?[] args)
        {
            return GetCurrentThisObjectOrThrow();
        }

        private static object? PrototypeDefineGetter(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            var getter = args != null && args.Length > 1 ? args[1] : null;
            if (getter is null || getter is JsNull || getter is not Delegate)
            {
                throw new TypeError("Getter must be a function");
            }

            var key = ToPropertyKeyString(prop);
            var enumerable = true;
            var configurable = true;
            object? setter = null;

            if (PropertyDescriptorStore.TryGetOwn(target, key, out var existing))
            {
                enumerable = existing.Enumerable;
                configurable = existing.Configurable;
                if (existing.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    setter = existing.Set;
                }
            }

            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = enumerable,
                Configurable = configurable,
                Get = getter,
                Set = setter
            });

            if (target is IDictionary<string, object?> dict && !dict.ContainsKey(key))
            {
                dict[key] = null;
            }

            return null;
        }

        private static object? PrototypeDefineSetter(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            var setter = args != null && args.Length > 1 ? args[1] : null;
            if (setter is null || setter is JsNull || setter is not Delegate)
            {
                throw new TypeError("Setter must be a function");
            }

            var key = ToPropertyKeyString(prop);
            var enumerable = true;
            var configurable = true;
            object? getter = null;

            if (PropertyDescriptorStore.TryGetOwn(target, key, out var existing))
            {
                enumerable = existing.Enumerable;
                configurable = existing.Configurable;
                if (existing.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    getter = existing.Get;
                }
            }

            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = enumerable,
                Configurable = configurable,
                Get = getter,
                Set = setter
            });

            if (target is IDictionary<string, object?> dict && !dict.ContainsKey(key))
            {
                dict[key] = null;
            }

            return null;
        }

        private static object? PrototypeLookupGetter(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            var key = ToPropertyKeyString(prop);

            if (PropertyDescriptorStore.TryGetOwn(target, key, out var own) && own.Kind == JsPropertyDescriptorKind.Accessor)
            {
                return own.Get;
            }

            if (!PrototypeChain.Enabled)
            {
                return null;
            }

            var current = target;
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance) { current };
            while (PrototypeChain.TryGetPrototype(current, out var proto) && proto is not null && proto is not JsNull)
            {
                if (!visited.Add(proto))
                {
                    break;
                }

                if (PropertyDescriptorStore.TryGetOwn(proto, key, out var descriptor) && descriptor.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    return descriptor.Get;
                }

                current = proto;
            }

            return null;
        }

        private static object? PrototypeLookupSetter(object[] scopes, object?[] args)
        {
            var target = GetCurrentThisObjectOrThrow();
            var prop = args != null && args.Length > 0 ? args[0] : null;
            var key = ToPropertyKeyString(prop);

            if (PropertyDescriptorStore.TryGetOwn(target, key, out var own) && own.Kind == JsPropertyDescriptorKind.Accessor)
            {
                return own.Set;
            }

            if (!PrototypeChain.Enabled)
            {
                return null;
            }

            var current = target;
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance) { current };
            while (PrototypeChain.TryGetPrototype(current, out var proto) && proto is not null && proto is not JsNull)
            {
                if (!visited.Add(proto))
                {
                    break;
                }

                if (PropertyDescriptorStore.TryGetOwn(proto, key, out var descriptor) && descriptor.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    return descriptor.Set;
                }

                current = proto;
            }

            return null;
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
            // In JavaScript, `new` on null/undefined throws a TypeError (not a host exception).
            // Libraries often probe for constructor availability inside try/catch (e.g., turndown).
            if (constructor is null || constructor is JsNull)
            {
                throw new TypeError("Value is not a constructor");
            }

            var callArgs = args ?? System.Array.Empty<object>();

            if (constructor is Type type)
            {
                try
                {
                    return Activator.CreateInstance(type, callArgs);
                }
                catch (MissingMethodException)
                {
                    // JS semantics allow missing arguments; treat them as null/undefined.
                    // Reflection activation does not pad arguments, so fall back to selecting a
                    // public instance ctor and padding missing args with null.
                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

                    foreach (var ctor in ctors.OrderBy(c => c.GetParameters().Length))
                    {
                        var parameters = ctor.GetParameters();
                        if (!IsViableConstructorCall(parameters, callArgs))
                        {
                            continue;
                        }

                        var invokeArgs = new object?[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (i < callArgs.Length)
                            {
                                _ = TryCoerceConstructorArg(callArgs[i], parameters[i].ParameterType, out var coerced);
                                invokeArgs[i] = coerced;
                            }
                            else
                            {
                                invokeArgs[i] = null;
                            }
                        }

                        try
                        {
                            return ctor.Invoke(invokeArgs);
                        }
                        catch (TargetInvocationException tie) when (tie.InnerException != null)
                        {
                            throw tie.InnerException;
                        }
                    }

                    throw;
                }
                catch (TargetInvocationException tie) when (tie.InnerException != null)
                {
                    throw tie.InnerException;
                }
            }

            if (constructor is Delegate del)
            {
                return JavaScriptRuntime.Function.Construct(del, callArgs);
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

            if (Environment.GetEnvironmentVariable("JS2IL_DOMINO_DIAG") == "1" && constructor is System.Dynamic.ExpandoObject expDiag)
            {
                var dict = (IDictionary<string, object?>)expDiag;
                var keys = string.Join(", ", dict.Keys.OrderBy(k => k, StringComparer.Ordinal).Take(12));
                if (dict.Count > 12)
                {
                    keys += ", ...";
                }

                throw new NotSupportedException($"Value is not constructible: {constructor.GetType().FullName} keys=[{keys}]");
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

            // Function.prototype.apply / Function.prototype.bind support.
            // In JS2IL, function values are represented as CLR delegates.
            if (receiver is Delegate del)
            {
                if (string.Equals(methodName, "apply", StringComparison.Ordinal))
                {
                    var thisArg = callArgs.Length > 0 ? callArgs[0] : null;
                    var argArray = callArgs.Length > 1 ? callArgs[1] : null;
                    return JavaScriptRuntime.Function.Apply(del, thisArg, argArray);
                }

                if (string.Equals(methodName, "call", StringComparison.Ordinal))
                {
                    var thisArg = callArgs.Length > 0 ? callArgs[0] : null;
                    var rest = callArgs.Length > 1
                        ? callArgs.Skip(1).Cast<object?>().ToArray()
                        : System.Array.Empty<object?>();
                    return JavaScriptRuntime.Function.Call(del, thisArg, rest);
                }

                if (string.Equals(methodName, "bind", StringComparison.Ordinal))
                {
                    var boundThis = callArgs.Length > 0 ? callArgs[0] : null;
                    var boundArgs = callArgs.Length > 1
                        ? callArgs.Skip(1).Cast<object?>().ToArray()
                        : System.Array.Empty<object?>();
                    return JavaScriptRuntime.Function.Bind(del, boundThis, boundArgs);
                }
            }

            // 1) String-like receiver -> direct fast-path helpers for parser-heavy operations.
            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                if (TryCallStringMemberFastPath(input, methodName, callArgs, out var stringResult))
                {
                    return stringResult;
                }

                return CallStringMemberViaReflection(input, methodName, callArgs);
            }

            // BigInt primitive helper dispatch.
            if (receiver is System.Numerics.BigInteger bigInt && string.Equals(methodName, "toString", StringComparison.Ordinal))
            {
                return callArgs.Length > 0
                    ? JavaScriptRuntime.BigInt.ToString(bigInt, callArgs[0])
                    : JavaScriptRuntime.BigInt.ToString(bigInt);
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

        // Arity-specific overloads to avoid object[] allocations for common cases (0-3 args).
        // These inline the logic to avoid creating arrays when possible.

        public static object? CallMember0(object receiver, string methodName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            // Function.prototype methods with 0 args
            if (receiver is Delegate del)
            {
                if (string.Equals(methodName, "apply", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Apply(del, null, null);
                }
                if (string.Equals(methodName, "call", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Call(del, null, System.Array.Empty<object?>());
                }
                if (string.Equals(methodName, "bind", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Bind(del, null, System.Array.Empty<object?>());
                }
            }

            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                if (TryCallStringMemberFastPath(input, methodName, 0, null, null, null, out var stringResult))
                {
                    return stringResult;
                }

                return CallMember(receiver, methodName, System.Array.Empty<object>());
            }

            // For other cases, fall back to the general method with empty array
            return CallMember(receiver, methodName, System.Array.Empty<object>());
        }

        public static object? CallMember1(object receiver, string methodName, object? a0)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            // Function.prototype methods with 1 arg
            if (receiver is Delegate del)
            {
                if (string.Equals(methodName, "apply", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Apply(del, a0, null);
                }
                if (string.Equals(methodName, "call", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Call(del, a0, System.Array.Empty<object?>());
                }
                if (string.Equals(methodName, "bind", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Bind(del, a0, System.Array.Empty<object?>());
                }
            }

            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                if (TryCallStringMemberFastPath(input, methodName, 1, a0, null, null, out var stringResult))
                {
                    return stringResult;
                }

                return CallMember(receiver, methodName, new object[] { a0! });
            }

            // For other cases, fall back to the general method
            return CallMember(receiver, methodName, new object[] { a0! });
        }

        public static object? CallMember2(object receiver, string methodName, object? a0, object? a1)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            // Function.prototype methods with 2 args
            if (receiver is Delegate del)
            {
                if (string.Equals(methodName, "apply", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Apply(del, a0, a1);
                }
                if (string.Equals(methodName, "call", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Call(del, a0, new object?[] { a1 });
                }
                if (string.Equals(methodName, "bind", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Bind(del, a0, new object?[] { a1 });
                }
            }

            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                if (TryCallStringMemberFastPath(input, methodName, 2, a0, a1, null, out var stringResult))
                {
                    return stringResult;
                }

                return CallMember(receiver, methodName, new object[] { a0!, a1! });
            }

            // For other cases, fall back to the general method
            return CallMember(receiver, methodName, new object[] { a0!, a1! });
        }

        public static object? CallMember3(object receiver, string methodName, object? a0, object? a1, object? a2)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            // Function.prototype methods with 3 args
            if (receiver is Delegate del)
            {
                if (string.Equals(methodName, "apply", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Apply(del, a0, a1);
                }
                if (string.Equals(methodName, "call", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Call(del, a0, new object?[] { a1, a2 });
                }
                if (string.Equals(methodName, "bind", StringComparison.Ordinal))
                {
                    return JavaScriptRuntime.Function.Bind(del, a0, new object?[] { a1, a2 });
                }
            }

            if (receiver is string || receiver is char[] || receiver is System.Text.StringBuilder)
            {
                var input = DotNet2JSConversions.ToString(receiver);
                if (TryCallStringMemberFastPath(input, methodName, 3, a0, a1, a2, out var stringResult))
                {
                    return stringResult;
                }

                return CallMember(receiver, methodName, new object[] { a0!, a1!, a2! });
            }

            // For other cases, fall back to the general method
            return CallMember(receiver, methodName, new object[] { a0!, a1!, a2! });
        }

        private static bool TryCallStringMemberFastPath(string input, string methodName, object[] callArgs, out object? result)
        {
            var argCount = callArgs.Length;
            var a0 = argCount > 0 ? callArgs[0] : null;
            var a1 = argCount > 1 ? callArgs[1] : null;
            var a2 = argCount > 2 ? callArgs[2] : null;
            return TryCallStringMemberFastPath(input, methodName, argCount, a0, a1, a2, out result);
        }

        private static object? CallStringMemberViaReflection(string input, string methodName, object[] callArgs)
        {
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
            invokeArgs[0] = input;

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

            for (int pi = 1 + jsArgCount; pi < ps.Length; pi++)
            {
                invokeArgs[pi] = ps[pi].ParameterType == typeof(bool) ? (object)false : null;
            }

            return chosen.Invoke(null, invokeArgs);
        }

        // Keep this switch intentionally limited to hot-path members.
        // Any method not listed here is still handled by CallStringMemberViaReflection,
        // preserving backward compatibility for String member dispatch.
        private static bool TryCallStringMemberFastPath(string input, string methodName, int argCount, object? a0, object? a1, object? a2, out object? result)
        {
            switch (methodName)
            {
                case "charCodeAt":
                    result = argCount <= 0
                        ? JavaScriptRuntime.String.CharCodeAt(input)
                        : JavaScriptRuntime.String.CharCodeAt(input, a0);
                    return true;

                case "substring":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.Substring(input, null),
                        1 => JavaScriptRuntime.String.Substring(input, a0),
                        _ => JavaScriptRuntime.String.Substring(input, a0, a1)
                    };
                    return true;

                case "substr":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.Substr(input, null),
                        1 => JavaScriptRuntime.String.Substr(input, a0),
                        _ => JavaScriptRuntime.String.Substr(input, a0, a1)
                    };
                    return true;

                case "slice":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.Slice(input, null),
                        1 => JavaScriptRuntime.String.Slice(input, a0),
                        _ => JavaScriptRuntime.String.Slice(input, a0, a1)
                    };
                    return true;

                case "indexOf":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.IndexOf(input, string.Empty),
                        1 => JavaScriptRuntime.String.IndexOf(input, DotNet2JSConversions.ToString(a0)),
                        _ => JavaScriptRuntime.String.IndexOf(input, DotNet2JSConversions.ToString(a0), a1)
                    };
                    return true;

                case "lastIndexOf":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.LastIndexOf(input, string.Empty),
                        1 => JavaScriptRuntime.String.LastIndexOf(input, DotNet2JSConversions.ToString(a0)),
                        _ => JavaScriptRuntime.String.LastIndexOf(input, DotNet2JSConversions.ToString(a0), a1)
                    };
                    return true;

                case "startsWith":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.StartsWith(input, string.Empty),
                        1 => JavaScriptRuntime.String.StartsWith(input, DotNet2JSConversions.ToString(a0)),
                        _ => JavaScriptRuntime.String.StartsWith(input, DotNet2JSConversions.ToString(a0), a1)
                    };
                    return true;

                case "endsWith":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.EndsWith(input, string.Empty),
                        1 => JavaScriptRuntime.String.EndsWith(input, DotNet2JSConversions.ToString(a0)),
                        _ => JavaScriptRuntime.String.EndsWith(input, DotNet2JSConversions.ToString(a0), a1)
                    };
                    return true;

                case "includes":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.Includes(input, string.Empty),
                        1 => JavaScriptRuntime.String.Includes(input, DotNet2JSConversions.ToString(a0)),
                        _ => JavaScriptRuntime.String.Includes(input, DotNet2JSConversions.ToString(a0), a1)
                    };
                    return true;

                case "trim":
                    result = JavaScriptRuntime.String.Trim(input);
                    return true;

                case "trimStart":
                case "trimLeft":
                    result = JavaScriptRuntime.String.TrimStart(input);
                    return true;

                case "trimEnd":
                case "trimRight":
                    result = JavaScriptRuntime.String.TrimEnd(input);
                    return true;

                case "toLowerCase":
                    result = JavaScriptRuntime.String.ToLowerCase(input);
                    return true;

                case "toUpperCase":
                    result = JavaScriptRuntime.String.ToUpperCase(input);
                    return true;

                case "split":
                    result = argCount switch
                    {
                        <= 0 => JavaScriptRuntime.String.Split(input, null),
                        1 => JavaScriptRuntime.String.Split(input, a0),
                        _ => JavaScriptRuntime.String.Split(input, a0, a1)
                    };
                    return true;

                case "replace":
                    if (argCount >= 2)
                    {
                        result = JavaScriptRuntime.String.Replace(input, a0!, a1!);
                        return true;
                    }
                    break;

                case "match":
                    if (argCount <= 0)
                    {
                        result = JavaScriptRuntime.String.Match(input, null);
                        return true;
                    }

                    if (argCount == 1)
                    {
                        result = JavaScriptRuntime.String.Match(input, a0);
                        return true;
                    }

                    break;

                case "localeCompare":
                    if (argCount <= 0)
                    {
                        result = JavaScriptRuntime.String.LocaleCompare(input, string.Empty, null, null);
                        return true;
                    }

                    if (argCount == 1)
                    {
                        result = JavaScriptRuntime.String.LocaleCompare(input, DotNet2JSConversions.ToString(a0), null, null);
                        return true;
                    }

                    if (argCount == 2)
                    {
                        result = JavaScriptRuntime.String.LocaleCompare(input, DotNet2JSConversions.ToString(a0), a1, null);
                        return true;
                    }

                    if (argCount == 3)
                    {
                        result = JavaScriptRuntime.String.LocaleCompare(input, DotNet2JSConversions.ToString(a0), a1, a2);
                        return true;
                    }

                    break;
            }

            result = null;
            return false;
        }

        private static string ToPropertyKeyString(object? key)
        {
            if (key is Symbol sym)
            {
                // NOTE: We don't yet model true ECMAScript symbol-keyed properties.
                // We encode Symbol keys to a stable internal string so computed
                // properties like obj[Symbol.iterator] can round-trip.
                var encoded = sym.DebugId;
                _encodedSymbolKeys.TryAdd(encoded, sym);
                return encoded;
            }

            return DotNet2JSConversions.ToString(key);
        }

        // Determines whether a computed key should be treated as an array index.
        // We intentionally require a *canonical* decimal representation for string keys:
        //  - "0", "1", ... are indices
        //  - "01", "1.0", "-1", "length", "true" are properties
        private static bool TryGetCanonicalArrayIndex(object index, string propName, out int intIndex)
        {
            intIndex = 0;

            switch (index)
            {
                case int ii:
                    if (ii < 0) return false;
                    intIndex = ii;
                    return true;
                case long ll:
                    if (ll < 0 || ll > int.MaxValue) return false;
                    intIndex = (int)ll;
                    return true;
                case short ss:
                    if (ss < 0) return false;
                    intIndex = ss;
                    return true;
                case byte bb:
                    intIndex = bb;
                    return true;
                case double dd:
                    if (double.IsNaN(dd) || double.IsInfinity(dd)) return false;
                    if (dd < 0 || dd > int.MaxValue) return false;
                    if (!double.IsInteger(dd)) return false;
                    intIndex = (int)dd;
                    return true;
                case float ff:
                    if (float.IsNaN(ff) || float.IsInfinity(ff)) return false;
                    if (ff < 0 || ff > int.MaxValue) return false;
                    if (!float.IsInteger(ff)) return false;
                    intIndex = (int)ff;
                    return true;
                case string s:
                    return TryParseCanonicalIndexString(s, out intIndex);
            }

            // Fallback: try the ToPropertyKey string representation.
            return TryParseCanonicalIndexString(propName, out intIndex);
        }

        private static bool TryParseCanonicalIndexString(string s, out int parsed)
        {
            parsed = 0;
            if (string.IsNullOrEmpty(s)) return false;
            if (!int.TryParse(s, global::System.Globalization.NumberStyles.None, global::System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return false;
            }
            if (parsed < 0) return false;
            return parsed.ToString(global::System.Globalization.CultureInfo.InvariantCulture) == s;
        }

        private static bool HasOwnProperty(object target, string name)
        {
            if (target is null || target is JsNull)
            {
                return false;
            }

            if (PropertyDescriptorStore.TryGetOwn(target, name, out _))
            {
                return true;
            }

            if (target is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                return dict.ContainsKey(name);
            }

            if (target is IDictionary<string, object?> dictGeneric)
            {
                return dictGeneric.ContainsKey(name);
            }

            if (target is System.Collections.IDictionary dictObj)
            {
                if (dictObj.Contains(name)) return true;
                foreach (var k in dictObj.Keys)
                {
                    if (string.Equals(DotNet2JSConversions.ToString(k), name, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
                return false;
            }

            try
            {
                var type = target.GetType();
                return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) != null
                    || type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) != null;
            }
            catch
            {
                return false;
            }
        }

        private static object? InvokeCallable(object? callable, object thisArg, object?[] args)
        {
            if (callable is null || callable is JsNull)
            {
                return null;
            }

            if (callable is Delegate)
            {
                var previousThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    return Closure.InvokeWithArgs(callable, System.Array.Empty<object>(), args);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

            throw new TypeError("Property accessor is not a function");
        }

        private static bool TryGetOwnPropertyDescriptor(object target, string propName, out JsPropertyDescriptor descriptor)
        {
            if (PropertyDescriptorStore.TryGetOwn(target, propName, out descriptor!))
            {
                return true;
            }

            // Default descriptors for existing properties (no attribute fidelity yet).
            if (target is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                if (dict.TryGetValue(propName, out var v))
                {
                    descriptor = new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Data,
                        Enumerable = true,
                        Configurable = true,
                        Writable = true,
                        Value = v
                    };
                    return true;
                }
            }

            if (target is IDictionary<string, object?> dictGeneric
                && dictGeneric.TryGetValue(propName, out var v2))
            {
                descriptor = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = true,
                    Configurable = true,
                    Writable = true,
                    Value = v2
                };
                return true;
            }

            // No implicit descriptor support for arrays/typed arrays/strings here.
            descriptor = null!;
            return false;
        }

        private static object CreateDescriptorObject(JsPropertyDescriptor desc)
        {
            var exp = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)exp;

            dict["enumerable"] = desc.Enumerable;
            dict["configurable"] = desc.Configurable;

            if (desc.Kind == JsPropertyDescriptorKind.Accessor)
            {
                dict["get"] = desc.Get;
                dict["set"] = desc.Set;
            }
            else
            {
                dict["value"] = desc.Value;
                dict["writable"] = desc.Writable;
            }

            return exp;
        }

        private static bool TryGetOwnPropertyValue(object target, string propName, out object? value)
        {
            return TryGetOwnPropertyValue(target, propName, target, out value);
        }

        private static bool TryGetOwnPropertyValue(object target, string propName, object receiverForAccessors, out object? value)
        {
            // Descriptor-defined properties (data/accessor)
            if (PropertyDescriptorStore.TryGetOwn(target, propName, out var desc))
            {
                if (desc.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    value = desc.Get is null || desc.Get is JsNull
                        ? null
                        : InvokeCallable(desc.Get, receiverForAccessors, System.Array.Empty<object>());
                    return true;
                }

                value = desc.Value;
                return true;
            }

            // ExpandoObject properties
            if (target is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                return dict.TryGetValue(propName, out value);
            }

            if (target is IDictionary<string, object?> dictGeneric)
            {
                return dictGeneric.TryGetValue(propName, out value);
            }

            // JavaScriptRuntime.Array / typed arrays: no custom properties yet
            if (target is Array || target is Int32Array)
            {
                value = null;
                return false;
            }

            // Delegate-backed functions behave like JS Function objects and must have a default
            // `.prototype` property (used heavily by real-world libraries like domino).
            // We model this lazily so existing tests that don't touch it don't pay for allocation.
            if (target is Delegate del && string.Equals(propName, "prototype", StringComparison.Ordinal))
            {
                var protoObj = new System.Dynamic.ExpandoObject();
                PropertyDescriptorStore.DefineOrUpdate(del, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = false,
                    Writable = true,
                    Value = protoObj
                });

                PropertyDescriptorStore.DefineOrUpdate(protoObj, "constructor", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = true,
                    Value = del
                });

                value = protoObj;
                return true;
            }

            static bool TryGetValue(Type type, object instance, string name, out object? result)
            {
                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null && prop.CanRead)
                {
                    result = prop.GetValue(instance);
                    return true;
                }

                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
                if (field != null)
                {
                    result = field.GetValue(instance);
                    return true;
                }

                var baseType = type.BaseType;
                if (baseType != null && baseType != typeof(object))
                {
                    return TryGetValue(baseType, instance, name, out result);
                }

                result = null;
                return false;
            }

            try
            {
                return TryGetValue(target.GetType(), target, propName, out value);
            }
            catch
            {
                value = null;
                return false;
            }
        }

        private static bool TryGetInheritedPropertyValue(object receiver, string propName, out object? value)
        {
            value = null;

            if (!PrototypeChain.Enabled)
            {
                return false;
            }

            var current = receiver;
            var proto = PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(proto, receiver))
            {
                return false;
            }

            if (TryGetOwnPropertyValue(proto, propName, receiver, out value))
            {
                return true;
            }

            current = proto;
            proto = PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                value = null;
                return false;
            }

            // Only allocate cycle-detection state if there is a chain to walk.
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance)
            {
                receiver,
                current
            };

            while (true)
            {
                if (!visited.Add(proto))
                {
                    value = null;
                    return false;
                }

                if (TryGetOwnPropertyValue(proto, propName, receiver, out value))
                {
                    return true;
                }

                current = proto;
                proto = PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    value = null;
                    return false;
                }
            }
        }

        private static bool TryInvokePrototypeSetter(object receiver, string propName, object? value)
        {
            if (!PrototypeChain.Enabled)
            {
                return false;
            }

            var current = receiver;
            var proto = PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(proto, receiver))
            {
                return false;
            }

            // Only allocate cycle-detection state if there is a chain to walk.
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance)
            {
                receiver
            };

            while (proto is not null && proto is not JsNull)
            {
                if (!visited.Add(proto))
                {
                    return false;
                }

                if (PropertyDescriptorStore.TryGetOwn(proto, propName, out var desc)
                    && desc.Kind == JsPropertyDescriptorKind.Accessor
                    && desc.Set is not null
                    && desc.Set is not JsNull)
                {
                    InvokeCallable(desc.Set, receiver, new object?[] { value });
                    return true;
                }

                current = proto;
                proto = PrototypeChain.GetPrototypeOrNull(current);
            }

            return false;
        }

        public static object GetItem(object obj, object index)
        {
            var propName = ToPropertyKeyString(index);

            // Proxy get trap: treat item access as property access using ToPropertyKey
            if (obj is JavaScriptRuntime.Proxy)
            {
                return GetProperty(obj, propName)!;
            }

            // Symbol keys are always properties.
            if (index is Symbol)
            {
                return GetProperty(obj, propName)!;
            }

            bool isIndex = TryGetCanonicalArrayIndex(index, propName, out int intIndex);

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (!isIndex)
                {
                    return GetProperty(obj, propName)!;
                }
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): numeric index coerces to property name string per JS ToPropertyKey
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                return GetProperty(obj, propName)!;
            }

            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return GetProperty(array, propName)!;
                }
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is Int32Array i32)
            {
                if (!isIndex)
                {
                    return GetProperty(i32, propName)!;
                }
                // Reads outside bounds return 0 per typed array semantics
                return i32[(double)intIndex];
            }
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return GetProperty(buffer, propName)!;
                }
                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[(double)intIndex]!;
            }
            else
            {
                // Generic object index access: treat index as a property key (JS ToPropertyKey -> string)
                // and fall back to dynamic property lookup (public fields/properties and ExpandoObject).
                return GetProperty(obj, propName)!;
            }
        }

        public static object GetItem(object obj, double index)
        {
            // Proxy get trap: numeric index coerces to property key
            if (obj is JavaScriptRuntime.Proxy)
            {
                var propName = ToPropertyKeyString(index);
                return GetProperty(obj, propName)!;
            }

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
                var propName = ToPropertyKeyString(index);
                return GetProperty(obj, propName)!;
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
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (double.IsNaN(index) || double.IsInfinity(index) || index % 1.0 != 0.0)
                {
                    var propName = ToPropertyKeyString(index);
                    return GetProperty(buffer, propName)!;
                }

                if (index < 0 || index > int.MaxValue)
                {
                    return null!;
                }

                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[index]!;
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
        /// Gets an item from an object and converts the result to a number (double).
        /// Provides a fast path for Int32Array receivers that avoids boxing the element value.
        /// For all other receivers, falls back to TypeUtilities.ToNumber(GetItem(obj, index)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetItemAsNumber(object obj, double index)
        {
            if (obj is Int32Array i32)
            {
                return i32[index];
            }
            return TypeUtilities.ToNumber(GetItem(obj, index));
        }

        /// <summary>
        /// Gets an item from an object and converts the result to a number (double).
        /// Provides a fast path for Int32Array receivers that avoids boxing the element value.
        /// For all other receivers, falls back to TypeUtilities.ToNumber(GetItem(obj, index)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetItemAsNumber(object obj, object index)
        {
            if (obj is Int32Array i32 && index is double d)
            {
                return i32[d];
            }
            return TypeUtilities.ToNumber(GetItem(obj, index));
        }

        /// <summary>
        /// Fast-path overload for string key reads.
        /// Avoids boxing at the call site when the compiler has proven the key is a string.
        /// Skips ToPropertyKeyString/Symbol checks and directly tests the canonical index string path.
        /// </summary>
        public static object GetItem(object obj, string key)
        {
            // Proxy get trap: treat item access as property access
            if (obj is JavaScriptRuntime.Proxy)
            {
                return GetProperty(obj, key)!;
            }

            bool isIndex = TryParseCanonicalIndexString(key, out int intIndex);

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (!isIndex)
                {
                    return GetProperty(obj, key)!;
                }
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): key is already a string property
            if (obj is System.Dynamic.ExpandoObject)
            {
                return GetProperty(obj, key)!;
            }

            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return GetProperty(array, key)!;
                }
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is Int32Array i32)
            {
                if (!isIndex)
                {
                    return GetProperty(i32, key)!;
                }
                // Reads outside bounds return 0 per typed array semantics
                return i32[(double)intIndex];
            }
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return GetProperty(buffer, key)!;
                }
                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[(double)intIndex]!;
            }
            else
            {
                return GetProperty(obj, key)!;
            }
        }

        /// <summary>
        /// Fast-path overload for string key reads that returns an unboxed number.
        /// Avoids boxing at the call site when the compiler has proven the key is a string
        /// and the result is consumed as a number.
        /// </summary>
        public static double GetItemAsNumber(object obj, string key)
        {
            return TypeUtilities.ToNumber(GetItem(obj, key));
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

            var propName = ToPropertyKeyString(index);
            bool isIndex = TryGetCanonicalArrayIndex(index, propName, out int intIndex);

            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy)
            {
                return SetProperty(obj, propName, value);
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                return SetProperty(obj, propName, value);
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (!isIndex)
                {
                    // Non-index keys behave like properties in JS (e.g. "length").
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
                if (!isIndex)
                {
                    return SetProperty(i32, propName, value);
                }
                // Index/value are numeric for typed arrays; coerce here so Int32Array can remain numeric.
                i32[(double)intIndex] = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return value;
            }

            // Buffer: coerce and store when in-bounds
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return SetProperty(buffer, propName, value);
                }
                // Buffer indexer expects numeric value
                buffer[(double)intIndex] = value;
                return value;
            }

            // Generic object: treat as property assignment (ToPropertyKey -> string)
            return SetProperty(obj, propName, value);
        }

        /// <summary>
        /// Fast-path overload for string key writes.
        /// Avoids boxing at the call site when the compiler has proven the key is a string.
        /// Skips ToPropertyKeyString/Symbol checks and directly tests the canonical index string path.
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetItem(object? obj, string key, object? value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy)
            {
                return SetProperty(obj, key, value);
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            bool isIndex = TryParseCanonicalIndexString(key, out int intIndex);

            if (obj is System.Dynamic.ExpandoObject)
            {
                return SetProperty(obj, key, value);
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return SetProperty(array, key, value);
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
                if (!isIndex)
                {
                    return SetProperty(i32, key, value);
                }
                // Index/value are numeric for typed arrays; coerce here so Int32Array can remain numeric.
                i32[(double)intIndex] = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return value;
            }

            // Buffer: coerce and store when in-bounds
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return SetProperty(buffer, key, value);
                }
                // Buffer indexer expects numeric value
                buffer[(double)intIndex] = value;
                return value;
            }

            // Generic object: treat as property assignment
            return SetProperty(obj, key, value);
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

            // Typed arrays: only use element write path if index is finite, integer, and in-bounds.
            // Otherwise treat as no-op (typed arrays do not store non-integer-index properties).
            if (obj is Int32Array i32)
            {
                // Check if index is a valid integer index
                if (!double.IsNaN(index) && !double.IsInfinity(index) && (index % 1.0 == 0.0))
                {
                    // Validate index is within int32 range before casting
                    if (index >= 0 && index <= int.MaxValue)
                    {
                        int i32Index = (int)index;
                        // Only write if in bounds [0, length)
                        if (i32Index < (int)i32.length)
                        {
                            i32.SetFromDouble(i32Index, value);
                        }
                        // Out-of-bounds: no-op (typed arrays don't expand)
                    }
                    // Negative or too large: no-op
                }
                // NaN/Infinity/fractional: no-op (do not treat as element 0 or property)
                return value;
            }

            // Buffer: only use element write path if index is finite, integer, and in-bounds.
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                // Check if index is a valid integer index
                if (!double.IsNaN(index) && !double.IsInfinity(index) && (index % 1.0 == 0.0))
                {
                    // Validate index is within int32 range before casting
                    if (index >= 0 && index <= int.MaxValue)
                    {
                        int bufferIndex = (int)index;
                        // Only write if in bounds [0, length)
                        if (bufferIndex < (int)buffer.length)
                        {
                            buffer[(double)bufferIndex] = value;
                        }
                        // Out-of-bounds: no-op (buffers don't expand)
                    }
                    // Negative or too large: no-op
                }
                // NaN/Infinity/fractional: no-op (do not treat as element 0 or property)
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

            // Fast-path: object literals are JsObject or ExpandoObject targets. For object spread, we want
            // CreateDataProperty semantics (define/overwrite an own data property) and must not
            // route through prototype setters.
            IDictionary<string, object?>? targetExpandoDict = null;
            if (target is JsObject || target is System.Dynamic.ExpandoObject)
            {
                targetExpandoDict = (IDictionary<string, object?>)target;
            }

            static void SetOwn(IDictionary<string, object?>? expandoDict, object targetObj, string key, object? value)
            {
                if (expandoDict is not null)
                {
                    expandoDict[key] = value;
                    return;
                }

                if (targetObj is IDictionary<string, object?> dict)
                {
                    dict[key] = value;
                    return;
                }

                // Fallback: best-effort for non-dynamic targets.
                SetProperty(targetObj, key, value);
            }

            // Per object spread semantics: null/undefined are skipped.
            if (source is null || source is JsNull)
            {
                return target;
            }

            // JsObject: enumerate all own enumerable properties (backing dict + descriptor store).
            if (source is JsObject jsObjSource)
            {
                var srcSeen = new HashSet<string>(StringComparer.Ordinal);
                EnumerateOwnEnumerableProperties(jsObjSource, srcSeen, (key, value) =>
                {
                    SetOwn(targetExpandoDict, target, key, value);
                }, includeEncodedSymbolKeys: true);
                return target;
            }

            // ExpandoObject: copy all enumerable own properties.
            if (source is System.Dynamic.ExpandoObject exp)
            {
                var src = (IDictionary<string, object?>)exp;
                foreach (var kvp in src)
                {
                    if (!PropertyDescriptorStore.IsEnumerableOrDefaultTrue(exp, kvp.Key))
                    {
                        continue;
                    }

                    SetOwn(targetExpandoDict, target, kvp.Key, kvp.Value);
                }
                return target;
            }

            // IDictionary<string, object?>: treat keys as enumerable properties.
            if (source is IDictionary<string, object?> dict)
            {
                foreach (var kvp in dict)
                {
                    if (!PropertyDescriptorStore.IsEnumerableOrDefaultTrue(source, kvp.Key))
                    {
                        continue;
                    }

                    SetOwn(targetExpandoDict, target, kvp.Key, kvp.Value);
                }
                return target;
            }

            // Strings: spread copies enumerable index properties ("0", "1", ...).
            if (source is string s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    SetOwn(targetExpandoDict, target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), s[i].ToString());
                }
                return target;
            }

            // JavaScriptRuntime.Array: spread copies element indices.
            if (source is Array arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    SetOwn(targetExpandoDict, target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), arr[i]);
                }
                return target;
            }

            // Int32Array: copy numeric indices as properties.
            if (source is Int32Array i32)
            {
                var len = (int)i32.length;
                for (int i = 0; i < len; i++)
                {
                    SetOwn(targetExpandoDict, target, i.ToString(System.Globalization.CultureInfo.InvariantCulture), i32[(double)i]);
                }
                return target;
            }

            // Reflection fallback for host objects: copy public instance properties/fields.
            try
            {
                var type = source.GetType();
                foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead))
                {
                    SetOwn(targetExpandoDict, target, p.Name, p.GetValue(source));
                }
                foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    SetOwn(targetExpandoDict, target, f.Name, f.GetValue(source));
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
        /// Returns a new JsObject with enumerable keys copied excluding the provided keys.
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

            var result = new JsObject();

            if (obj is JsObject jsObjSrc)
            {
                // Enumerate own enumerable properties (descriptor store + backing dict).
                var seen = new HashSet<string>(StringComparer.Ordinal);
                EnumerateOwnEnumerableProperties(jsObjSrc, seen, (key, value) =>
                {
                    if (!excluded.Contains(key))
                        result.SetValue(key, value);
                }, includeEncodedSymbolKeys: true);
                return result;
            }

            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var src = (IDictionary<string, object?>)exp;
                foreach (var kvp in src)
                {
                    if (excluded.Contains(kvp.Key)) continue;
                    if (!PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, kvp.Key)) continue;
                    result.SetValue(kvp.Key, kvp.Value);
                }
                return result;
            }

            // IDictionary<string, object?> fallback
            if (obj is IDictionary<string, object?> dictGeneric)
            {
                foreach (var kvp in dictGeneric)
                {
                    if (excluded.Contains(kvp.Key)) continue;
                    if (!PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, kvp.Key)) continue;
                    result.SetValue(kvp.Key, kvp.Value);
                }
                return result;
            }

            // Reflection fallback for host objects: copy public instance properties/fields.
            try
            {
                var type = obj.GetType();
                foreach (var p in type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && !excluded.Contains(p.Name)))
                {
                    result.SetValue(p.Name, p.GetValue(obj));
                }

                foreach (var f in type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => !excluded.Contains(f.Name)))
                {
                    result.SetValue(f.Name, f.GetValue(obj));
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
                var keys = new JavaScriptRuntime.Array(
                    dict.Keys
                        .Where(k => PropertyDescriptorStore.IsEnumerableOrDefaultTrue(exp, k))
                        .Cast<object?>());
                return keys;
            }

            if (obj is IDictionary<string, object?> dictGeneric)
            {
                var keys = new JavaScriptRuntime.Array(
                    dictGeneric.Keys
                        .Where(k => PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, k))
                        .Cast<object?>());
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
                    var strKey = DotNet2JSConversions.ToString(k);
                    if (strKey != null && PropertyDescriptorStore.IsEnumerableOrDefaultTrue(obj, strKey))
                    {
                        keys.Add(strKey);
                    }
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

            if (PropertyDescriptorStore.TryGetOwn(receiver, key, out var ownDescriptor)
                && !ownDescriptor.Configurable)
            {
                return false;
            }

            if (receiver is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                dict.Remove(key);
                PropertyDescriptorStore.Delete(receiver, key);
                return true;
            }

            if (receiver is IDictionary<string, object?> dictGeneric)
            {
                dictGeneric.Remove(key);
                PropertyDescriptorStore.Delete(receiver, key);
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

                PropertyDescriptorStore.Delete(receiver, key);
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
                case JavaScriptRuntime.Node.Buffer buffer:
                    return buffer.length;
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
            // Proxy get trap
            if (obj is JavaScriptRuntime.Proxy proxy)
            {
                var getTrap = GetProperty(proxy.Handler, "get");
                if (getTrap is not null && getTrap is not JsNull)
                {
                    return InvokeCallable(getTrap, proxy.Handler, new object?[] { proxy.Target, name, obj });
                }

                return GetProperty(proxy.Target, name);
            }

            // Legacy __proto__ accessor (opt-in)
            if (PrototypeChain.Enabled && string.Equals(name, "__proto__", StringComparison.Ordinal))
            {
                return PrototypeChain.TryGetPrototype(obj, out var proto) ? proto : null;
            }

            if (TryGetOwnPropertyValue(obj, name, out var ownValue))
            {
                return ownValue;
            }

            return TryGetInheritedPropertyValue(obj, name, out var inherited) ? inherited : null;
        }

        /// <summary>
        /// Sets a numeric property on a JavaScript object without boxing the double value.
        /// Called from generated IL during object literal initialization.
        /// For <see cref="JsObject"/> receivers the value is stored unboxed.
        /// Falls back to the general <see cref="SetProperty"/> for other receiver types.
        /// </summary>
        public static void SetPropertyNumber(object? obj, string key, double value)
        {
            if (obj is JsObject jsObj)
            {
                jsObj.SetNumber(key, value);
                return;
            }
            SetProperty(obj!, key, value);
        }

        /// <summary>
        /// Sets a boolean property on a JavaScript object without boxing the bool value.
        /// Called from generated IL during object literal initialization.
        /// </summary>
        public static void SetPropertyBoolean(object? obj, string key, bool value)
        {
            if (obj is JsObject jsObj)
            {
                jsObj.SetBoolean(key, value);
                return;
            }
            SetProperty(obj!, key, value);
        }

        /// <summary>
        /// Sets a string property on a JavaScript object.
        /// Called from generated IL during object literal initialization.
        /// </summary>
        public static void SetPropertyString(object? obj, string key, string? value)
        {
            if (obj is JsObject jsObj)
            {
                jsObj.SetString(key, value);
                return;
            }
            SetProperty(obj!, key, value);
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
            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy proxy)
            {
                var setTrap = GetProperty(proxy.Handler, "set");
                if (setTrap is not null && setTrap is not JsNull)
                {
                    _ = InvokeCallable(setTrap, proxy.Handler, new object?[] { proxy.Target, name, value, obj });
                    return value;
                }

                return SetProperty(proxy.Target, name, value);
            }

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

            var hasOwn = HasOwnProperty(obj, name);
            if (!hasOwn && TryInvokePrototypeSetter(obj, name, value))
            {
                return value;
            }

            if (!hasOwn && !IsExtensibleInternal(obj))
            {
                return value;
            }

            // Descriptor-defined own property handling (accessors + writable enforcement)
            if (PropertyDescriptorStore.TryGetOwn(obj, name, out var desc))
            {
                if (desc.Kind == JsPropertyDescriptorKind.Accessor)
                {
                    if (desc.Set is not null && desc.Set is not JsNull)
                    {
                        InvokeCallable(desc.Set, obj, new object?[] { value });
                    }
                    return value;
                }

                if (desc.Writable)
                {
                    desc.Value = value;
                    PropertyDescriptorStore.DefineOrUpdate(obj, name, desc);
                    if (obj is System.Dynamic.ExpandoObject expDesc)
                    {
                        var dict = (IDictionary<string, object?>)expDesc;
                        dict[name] = value;
                    }
                    else if (obj is IDictionary<string, object?> dictDesc)
                    {
                        dictDesc[name] = value;
                    }
                }
                return value;
            }

            // ExpandoObject support
            if (obj is System.Dynamic.ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;

                // Prototype-setter semantics: if no own property exists, and a prototype accessor
                // defines a setter, route the assignment to that setter.
                if (!dict.ContainsKey(name) && TryInvokePrototypeSetter(obj, name, value))
                {
                    return value;
                }

                dict[name] = value;
                return value;
            }

            if (obj is IDictionary<string, object?> dictGeneric)
            {
                if (!dictGeneric.ContainsKey(name) && TryInvokePrototypeSetter(obj, name, value))
                {
                    return value;
                }

                dictGeneric[name] = value;
                return value;
            }

            // Arrays / typed arrays: allow ad-hoc properties (arrays are objects in JS).
            // Numeric index semantics are handled elsewhere; this path covers things like
            // RegExp exec results setting `match.index` / `match.input`.
            if (obj is Array || obj is Int32Array)
            {
                if (obj is Array arr && string.Equals(name, "length", StringComparison.Ordinal))
                {
                    // Array.length is special: setting it truncates/extends the array.
                    // This is used heavily by parsers to clear buffers (e.g., buf.length = 0).
                    arr.length = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                    return value;
                }

                if (TryInvokePrototypeSetter(obj, name, value))
                {
                    return value;
                }

                PropertyDescriptorStore.DefineOrUpdate(obj, name, new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = value,
                    Writable = true,
                    Enumerable = true,
                    Configurable = true
                });

                return value;
            }

            // Function values (delegates) behave like objects in JavaScript and can have ad-hoc
            // properties (e.g., MyEvent.prototype = ...). Store these in the descriptor table.
            if (obj is Delegate)
            {
                if (TryInvokePrototypeSetter(obj, name, value))
                {
                    return value;
                }

                PropertyDescriptorStore.DefineOrUpdate(obj, name, new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = true,
                    Configurable = true,
                    Writable = true,
                    Value = value
                });

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

            // Prototype-setter semantics for "plain" CLR objects (e.g., JS2IL-generated scope classes).
            // If no own writable field/property exists, but a prototype accessor defines a setter,
            // route the assignment to that setter.
            if (TryInvokePrototypeSetter(obj, name, value))
            {
                return value;
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

            // Prefer js2il-style methods with a leading scopes array:
            // - legacy: (object[] scopes, [object x N])
            // - current JsFunc ABI: (object[] scopes, object newTarget, [object x N])
            MethodInfo? chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                if (ps.Length < 1 || ps[0].ParameterType != typeof(object[]))
                {
                    return false;
                }

                if (ps.Length == 1 + srcArgs.Length)
                {
                    return true;
                }

                return ps.Length == 2 + srcArgs.Length
                    && ps.Length >= 2
                    && ps[1].ParameterType == typeof(object);
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
            var expectsHiddenNewTarget = expectsLeadingScopes
                && psChosen.Length >= 2
                && psChosen[1].ParameterType == typeof(object);
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

            object? InvokeChosen(object?[] invokeArgs)
            {
                var previousThis = RuntimeServices.SetCurrentThis(instance);
                try
                {
                    try
                    {
                        return chosen.Invoke(instance, invokeArgs);
                    }
                    catch (TargetInvocationException tie) when (tie.InnerException != null)
                    {
                        ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                        throw; // unreachable
                    }
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }

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

                return InvokeChosen(new object?[] { coerced });
            }

            if (expectsLeadingScopes)
            {
                var scopes = ResolveScopesArray(instance);
                var coerced = new object[psChosen.Length];
                coerced[0] = scopes;

                int firstJsArgIndex = 1;
                if (expectsHiddenNewTarget)
                {
                    coerced[1] = null!;
                    firstJsArgIndex = 2;
                }

                var copyCount = System.Math.Min(src.Length, psChosen.Length - firstJsArgIndex);
                for (int i = 0; i < copyCount; i++)
                {
                    coerced[i + firstJsArgIndex] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
                }

                return InvokeChosen(coerced);
            }

            {
                var coerced = new object[psChosen.Length];
                var copyCount = System.Math.Min(src.Length, coerced.Length);
                for (int i = 0; i < copyCount; i++)
                {
                    coerced[i] = src[i] is null ? 0.0 : CoerceToJsNumber(src[i]);
                }

                return InvokeChosen(coerced);
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
                if (PropertyDescriptorStore.TryGetOwn(target, name, out _))
                {
                    return true;
                }

                // ExpandoObject (object literal)
                if (target is System.Dynamic.ExpandoObject exp)
                {
                    var dict = (IDictionary<string, object?>)exp;
                    return dict.ContainsKey(name);
                }

                // IDictionary<string, object?> (includes JsObject)
                if (target is IDictionary<string, object?> dictGeneric)
                {
                    return dictGeneric.ContainsKey(name);
                }

                if (target is System.Collections.IDictionary dictObj)
                {
                    if (dictObj.Contains(name))
                    {
                        return true;
                    }

                    foreach (var key in dictObj.Keys)
                    {
                        if (string.Equals(DotNet2JSConversions.ToString(key), name, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }

                    return false;
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

            // Avoid allocating cycle-detection state for the common case where no prototype
            // has been assigned.
            var current = obj;
            var proto = PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            if (ReferenceEquals(proto, obj))
            {
                return false;
            }

            if (HasOwnProperty(proto, propName))
            {
                return true;
            }

            current = proto;
            proto = PrototypeChain.GetPrototypeOrNull(current);
            if (proto is null || proto is JsNull)
            {
                return false;
            }

            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance)
            {
                obj,
                current
            };

            while (true)
            {
                if (!visited.Add(proto))
                {
                    return false;
                }

                if (HasOwnProperty(proto, propName))
                {
                    return true;
                }

                current = proto;
                proto = PrototypeChain.GetPrototypeOrNull(current);
                if (proto is null || proto is JsNull)
                {
                    return false;
                }
            }
        }
    }
}
