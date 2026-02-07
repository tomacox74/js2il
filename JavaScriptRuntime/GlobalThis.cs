using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public class GlobalThis : IDynamicMetaObjectProvider, IDictionary<string, object?>
    {
        private static readonly ThreadLocal<ServiceContainer?> _serviceProvider = new(() => null);

        // Per-"realm" (thread) global object. This backs the ECMAScript globalThis value.
        // We represent it as a GlobalThis instance with ExpandoObject-like behavior.
        private static readonly ThreadLocal<GlobalThis?> _globalObject = new(() => null);

        private readonly ExpandoObject _expando = new();
        private IDictionary<string, object?> Properties => (IDictionary<string, object?>)_expando;

        public GlobalThis()
        {
            SeedGlobalObjectIfMissing();
        }

        // Some ECMAScript globals are callable (e.g., Boolean(x)). When used in expression position
        // (e.g., arr.filter(Boolean)), we expose them as function values (delegates) so the compiler
        // can bind them as intrinsic globals.
        private static readonly Func<object[], object?, bool> _booleanFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToBoolean(value);

        private static readonly Func<object[], object?, string> _stringFunctionValue = static (_, value) =>
            JavaScriptRuntime.DotNet2JSConversions.ToString(value);

        // String.fromCharCode(...codeUnits)
        private static readonly Func<object[], object?[]?, object?> _stringFromCharCodeValue = static (_, args) =>
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }

            var chars = new char[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                double d;
                try
                {
                    d = JavaScriptRuntime.TypeUtilities.ToNumber(args[i]);
                }
                catch
                {
                    d = 0;
                }

                if (double.IsNaN(d) || double.IsInfinity(d))
                {
                    d = 0;
                }

                // JS: ToUint16
                uint u16 = (uint)((int)global::System.Math.Truncate(d)) & 0xFFFFu;
                chars[i] = (char)u16;
            }

            return new string(chars);
        };

        private static readonly Func<object[], object?, double> _numberFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToNumber(value);

        private static readonly Func<object[], object?, Delegate> _functionConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Function constructor is not supported yet.");

        // Placeholder Array constructor value. JS2IL uses dedicated codegen paths for `new Array(...)`.
        // We expose a callable value so libraries can reference `Array` as a global identifier and
        // access `Array.prototype.*` members.
        private static readonly Func<object[], object?[], object?> _arrayConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Array constructor is not supported as a callable value yet.");

        // Object constructor/function value. This enables patterns like `Object.prototype` and
        // allows libraries to pass `Object` around as a value.
        private static readonly Func<object[], object?, object> _objectConstructorValue = static (_, value) =>
            JavaScriptRuntime.Object.Construct(value);

        private static readonly Func<object[], object?, object> _objectGetOwnPropertyNamesValue = static (_, value) =>
            JavaScriptRuntime.Object.getOwnPropertyNames(value!);

        // Placeholder Error constructor value.
        // Exposed so libraries can reference `Error` and access `Error.prototype`.
        // Calling it as a constructor/function is not implemented yet.
        private static readonly Func<object[], object?[], object?> _errorConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Error constructor is not supported as a callable value yet.");

        // Minimal Error.prototype object. Libraries may attach properties here.
        private static readonly object _errorPrototypeValue = new ExpandoObject();

        // Minimal Object.prototype object used for descriptor/prototype-heavy libraries.
        // NOTE: We intentionally do not enable PrototypeChain here; Object.create/setPrototypeOf
        // opt into prototype semantics as needed.
        private static readonly object _objectPrototypeValue = new ExpandoObject();

        static GlobalThis()
        {
            // Attach minimal prototypes to callable globals so patterns like
            // `Function.prototype.apply.bind(Array.prototype.push)` work even when code only
            // references GlobalThis static properties and never touches the globalThis object.
            PropertyDescriptorStore.DefineOrUpdate(_functionConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = JavaScriptRuntime.Function.Prototype
            });
            PropertyDescriptorStore.DefineOrUpdate(_arrayConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = JavaScriptRuntime.Array.Prototype
            });

            // Provide Object.prototype for patterns like `Object.create(Object.prototype, ...)`.
            PropertyDescriptorStore.DefineOrUpdate(_objectConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _objectPrototypeValue
            });

            // Provide Object.getOwnPropertyNames
            PropertyDescriptorStore.DefineOrUpdate(_objectConstructorValue, "getOwnPropertyNames", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _objectGetOwnPropertyNamesValue
            });

            // Provide Object.prototype.hasOwnProperty for descriptor-heavy libraries.
            // This is frequently used via: Object.prototype.hasOwnProperty.call(obj, prop).
            if (_objectPrototypeValue is ExpandoObject exp)
            {
                var dict = (IDictionary<string, object?>)exp;
                dict["hasOwnProperty"] = (Func<object[], object?[], object?>)ObjectPrototypeHasOwnProperty;
            }

            // Provide Error.prototype for patterns like `Error.prototype` and error-subclassing libraries.
            PropertyDescriptorStore.DefineOrUpdate(_errorConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _errorPrototypeValue
            });

            // Provide String.fromCharCode for parsers/libraries.
            PropertyDescriptorStore.DefineOrUpdate(_stringFunctionValue, "fromCharCode", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _stringFromCharCodeValue
            });
        }

        private static object? ObjectPrototypeHasOwnProperty(object[] scopes, object?[] args)
        {
            var target = RuntimeServices.GetCurrentThis();
            var prop = args != null && args.Length > 0 ? args[0] : null;

            if (target is null || target is JsNull)
            {
                throw new TypeError("Cannot convert undefined or null to object");
            }

            var name = DotNet2JSConversions.ToString(prop);

            // Own-property check (minimal):
            // 1) Descriptor store (our primary object-model)
            // 2) Expando/Dictionary
            // 3) Reflection for host objects
            if (PropertyDescriptorStore.TryGetOwn(target, name, out var _descriptor))
            {
                return true;
            }

            if (target is ExpandoObject exp2)
            {
                var expDict = (IDictionary<string, object?>)exp2;
                return expDict.ContainsKey(name);
            }

            if (target is IDictionary<string, object?> dictGeneric)
            {
                return dictGeneric.ContainsKey(name);
            }

            if (target is IDictionary dictObj)
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

            var t = target.GetType();
            return t.GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public) != null
                || t.GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public) != null;
        }

        internal static ServiceContainer? ServiceProvider
        {
            get => _serviceProvider.Value;
            set
            {
                _serviceProvider.Value = value;

                // Each configured runtime corresponds to a new execution context/realm.
                // Ensure we don't leak a prior global object across Engine.Execute calls on the same thread.
                _globalObject.Value = null;
            }
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return ((IDynamicMetaObjectProvider)_expando).GetMetaObject(parameter);
        }

        public object? this[string key]
        {
            get => Properties[key];
            set => Properties[key] = value;
        }

        public ICollection<string> Keys => Properties.Keys;

        public ICollection<object?> Values => Properties.Values;

        public int Count => Properties.Count;

        public bool IsReadOnly => Properties.IsReadOnly;

        public void Add(string key, object? value) => Properties.Add(key, value);

        public bool ContainsKey(string key) => Properties.ContainsKey(key);

        public bool Remove(string key) => Properties.Remove(key);

        public bool TryGetValue(string key, out object? value) => Properties.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object?> item) => Properties.Add(item);

        void ICollection<KeyValuePair<string, object?>>.Clear() =>
            throw new NotSupportedException("Clearing the global object is not supported.");

        public bool Contains(KeyValuePair<string, object?> item) => Properties.Contains(item);

        void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) =>
            Properties.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object?> item) => Properties.Remove(item);

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => Properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Properties.GetEnumerator();

        /// <summary>
        /// ECMA-262 globalThis value.
        /// Returns the global object for the current execution context.
        /// </summary>
        /// <remarks>
        /// JS2IL models the global object as a dynamic bag (ExpandoObject) seeded with common globals.
        /// This allows libraries to read/write properties via globalThis (e.g., globalThis.window = ...).
        /// </remarks>
        public static object globalThis => GetOrCreateGlobalObject();

        private static GlobalThis GetOrCreateGlobalObject()
        {
            var obj = _globalObject.Value;
            if (obj == null)
            {
                obj = new GlobalThis();
                _globalObject.Value = obj;
            }
            return obj;
        }

        private void DefineNonEnumerableDataProperty(string key, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(this, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
        }

        private void SeedGlobalObjectIfMissing()
        {
            var dict = (IDictionary<string, object?>)this;

            // Self reference.
            dict[nameof(GlobalThis.globalThis)] = this;
            DefineNonEnumerableDataProperty(nameof(GlobalThis.globalThis), this);

            // Seed common globals without overwriting user overrides.
            dict.TryAdd(nameof(GlobalThis.console), console);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.console), dict[nameof(GlobalThis.console)]);

            dict.TryAdd(nameof(GlobalThis.process), process);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.process), dict[nameof(GlobalThis.process)]);

            dict.TryAdd(nameof(GlobalThis.Infinity), Infinity);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Infinity), dict[nameof(GlobalThis.Infinity)]);

            dict.TryAdd(nameof(GlobalThis.NaN), NaN);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.NaN), dict[nameof(GlobalThis.NaN)]);

            dict.TryAdd(nameof(GlobalThis.Boolean), Boolean);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Boolean), dict[nameof(GlobalThis.Boolean)]);

            dict.TryAdd(nameof(GlobalThis.String), String);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.String), dict[nameof(GlobalThis.String)]);

            dict.TryAdd(nameof(GlobalThis.Number), Number);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Number), dict[nameof(GlobalThis.Number)]);

            dict.TryAdd(nameof(GlobalThis.Function), Function);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Function), dict[nameof(GlobalThis.Function)]);

            dict.TryAdd(nameof(GlobalThis.Array), Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Array), dict[nameof(GlobalThis.Array)]);

            dict.TryAdd(nameof(GlobalThis.Object), Object);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Object), dict[nameof(GlobalThis.Object)]);

            dict.TryAdd(nameof(GlobalThis.Error), Error);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Error), dict[nameof(GlobalThis.Error)]);

            // Global functions exposed as delegates.
            dict.TryAdd(nameof(GlobalThis.setTimeout), (Func<object, object, object[], object>)setTimeout);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.setTimeout), dict[nameof(GlobalThis.setTimeout)]);

            dict.TryAdd(nameof(GlobalThis.clearTimeout), (Func<object, object?>)clearTimeout);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.clearTimeout), dict[nameof(GlobalThis.clearTimeout)]);

            dict.TryAdd(nameof(GlobalThis.setImmediate), (Func<object, object[], object>)setImmediate);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.setImmediate), dict[nameof(GlobalThis.setImmediate)]);

            dict.TryAdd(nameof(GlobalThis.clearImmediate), (Func<object, object?>)clearImmediate);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.clearImmediate), dict[nameof(GlobalThis.clearImmediate)]);

            dict.TryAdd(nameof(GlobalThis.setInterval), (Func<object, object, object[], object>)setInterval);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.setInterval), dict[nameof(GlobalThis.setInterval)]);

            dict.TryAdd(nameof(GlobalThis.clearInterval), (Func<object, object?>)clearInterval);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.clearInterval), dict[nameof(GlobalThis.clearInterval)]);

            dict.TryAdd(nameof(GlobalThis.parseInt), (Func<object?, object?, double>)parseInt);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.parseInt), dict[nameof(GlobalThis.parseInt)]);

            dict.TryAdd(nameof(GlobalThis.parseFloat), (Func<object?, double>)parseFloat);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.parseFloat), dict[nameof(GlobalThis.parseFloat)]);

            dict.TryAdd(nameof(GlobalThis.isFinite), (Func<object?, bool>)isFinite);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.isFinite), dict[nameof(GlobalThis.isFinite)]);
        }

        /// <summary>
        /// Minimal process global with writable exitCode.
        /// </summary>
        /// <remarks>Expand as needed in the future.</remarks>
        public static JavaScriptRuntime.Node.Process process
        {
            get => _serviceProvider.Value!.Resolve<JavaScriptRuntime.Node.Process>();
        }

        /// <summary>
        /// Global console object (lowercase) to mirror JS global. Provides access to log/error/warn via the Console intrinsic.
        /// Backed by a single shared instance.
        /// </summary>
        public static JavaScriptRuntime.Console console 
        {
            get => _serviceProvider.Value!.Resolve<JavaScriptRuntime.Console>();
        }

        /// <summary>
        /// ECMAScript global Boolean conversion function value.
        /// This enables patterns like <c>array.filter(Boolean)</c>.
        /// </summary>
        public static Func<object[], object?, bool> Boolean => _booleanFunctionValue;

        /// <summary>
        /// ECMAScript global String conversion function value.
        /// This enables patterns like <c>array.map(String)</c> and type-marker comparisons (e.g., <c>x === String</c>).
        /// </summary>
        public static Func<object[], object?, string> String => _stringFunctionValue;

        /// <summary>
        /// ECMAScript global Number conversion function value.
        /// This enables patterns like <c>array.map(Number)</c> and type-marker comparisons (e.g., <c>x === Number</c>).
        /// </summary>
        public static Func<object[], object?, double> Number => _numberFunctionValue;

        /// <summary>
        /// ECMAScript global Function constructor value (placeholder).
        /// Currently exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Function constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?, Delegate> Function => _functionConstructorValue;

        /// <summary>
        /// ECMAScript global Array constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Array constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?[], object?> Array => _arrayConstructorValue;

        public static Func<object[], object?, object> Object => _objectConstructorValue;

        /// <summary>
        /// ECMAScript global Error constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier and
        /// access <c>Error.prototype</c>.
        /// </summary>
        public static Func<object[], object?[], object?> Error => _errorConstructorValue;

        /// <summary>
        /// ECMAScript global Infinity value (+∞).
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double Infinity => double.PositiveInfinity;

        /// <summary>
        /// ECMAScript global NaN value.
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double NaN => double.NaN;

        public static object setTimeout(object callback, object delay, params object[] args)
        {
            return GetTimers().setTimeout(callback, delay, args);
        }

        public static object? clearTimeout(object handle)
        {
            return GetTimers().clearTimeout(handle);
        }

        public static object setImmediate(object callback, params object[] args)
        {
            return GetTimers().setImmediate(callback, args);
        }

        public static object setInterval(object callback, object delay, params object[] args)
        {
            return GetTimers().setInterval(callback, delay, args);
        }

        public static object? clearImmediate(object handle)
        {
            return GetTimers().clearImmediate(handle);
        }

        public static object? clearInterval(object handle)
        {
            return GetTimers().clearInterval(handle);
        }

        /// <summary>
        /// Minimal parseInt implementation for numeric strings (radix 2-36).
        /// Returns NaN on invalid input.
        /// </summary>
        public static double parseInt(object? input, object? radix = null)
        {
            if (input == null) return double.NaN;

            var text = DotNet2JSConversions.ToString(input).TrimStart();
            if (text.Length == 0) return double.NaN;

            int sign = 1;
            if (text[0] == '+')
            {
                text = text.Substring(1);
            }
            else if (text[0] == '-')
            {
                sign = -1;
                text = text.Substring(1);
            }

            int radixValue = 0;
            if (radix != null)
            {
                try
                {
                    radixValue = Convert.ToInt32(radix, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    return double.NaN;
                }
            }

            if (radixValue == 0)
            {
                if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    radixValue = 16;
                    text = text.Substring(2);
                }
                else
                {
                    radixValue = 10;
                }
            }

            // Per spec, if radix is 16, an optional 0x/0X prefix is allowed and must be stripped.
            if (radixValue == 16 && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(2);
            }

            if (radixValue < 2 || radixValue > 36)
            {
                return double.NaN;
            }

            long value = 0;
            int digits = 0;
            foreach (var ch in text)
            {
                int d = ch switch
                {
                    >= '0' and <= '9' => ch - '0',
                    >= 'a' and <= 'z' => ch - 'a' + 10,
                    >= 'A' and <= 'Z' => ch - 'A' + 10,
                    _ => -1
                };

                if (d < 0 || d >= radixValue)
                {
                    break;
                }

                value = (value * radixValue) + d;
                digits++;
            }

            if (digits == 0)
            {
                return double.NaN;
            }

            return (double)(sign * value);
        }

        /// <summary>
        /// Minimal parseFloat implementation.
        /// Accepts leading whitespace, an optional sign, decimals, and an optional exponent.
        /// Stops parsing at the first invalid character; returns NaN if no valid prefix.
        /// </summary>
        public static double parseFloat(object? input)
        {
            if (input == null) return double.NaN;

            var text = DotNet2JSConversions.ToString(input).TrimStart();
            if (text.Length == 0) return double.NaN;

            // Infinity tokens
            if (text.StartsWith("Infinity", StringComparison.Ordinal)) return double.PositiveInfinity;
            if (text.StartsWith("+Infinity", StringComparison.Ordinal)) return double.PositiveInfinity;
            if (text.StartsWith("-Infinity", StringComparison.Ordinal)) return double.NegativeInfinity;

            int i = 0;
            if (text[i] == '+' || text[i] == '-')
            {
                i++;
                if (i >= text.Length) return double.NaN;
            }

            bool sawDigit = false;
            while (i < text.Length && char.IsDigit(text[i]))
            {
                sawDigit = true;
                i++;
            }

            if (i < text.Length && text[i] == '.')
            {
                i++;
                while (i < text.Length && char.IsDigit(text[i]))
                {
                    sawDigit = true;
                    i++;
                }
            }

            // Optional exponent
            if (sawDigit && i < text.Length && (text[i] == 'e' || text[i] == 'E'))
            {
                int expStart = i;
                i++;
                if (i < text.Length && (text[i] == '+' || text[i] == '-'))
                {
                    i++;
                }

                int expDigits = 0;
                while (i < text.Length && char.IsDigit(text[i]))
                {
                    expDigits++;
                    i++;
                }

                if (expDigits == 0)
                {
                    // Roll back; exponent marker not followed by digits.
                    i = expStart;
                }
            }

            if (!sawDigit)
            {
                return double.NaN;
            }

            var prefix = text.Substring(0, i);
            return double.TryParse(
                prefix,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var parsed)
                ? parsed
                : double.NaN;
        }

        /// <summary>
        /// Minimal global isFinite implementation.
        /// Coerces to number and returns true only when the result is a finite IEEE754 double.
        /// </summary>
        public static bool isFinite(object? number)
        {
            var d = TypeUtilities.ToNumber(number);
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        private static Timers GetTimers()
        {
            return _serviceProvider.Value!.Resolve<Timers>();
        }
    }
}
