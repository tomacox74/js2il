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

        private static readonly Func<object[], object?[]?, object?> _booleanPrototypeToStringValue = static (_, __) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            var booleanValue = JavaScriptRuntime.Boolean.ThisBooleanValue(thisValue);
            return booleanValue ? "true" : "false";
        };

        private static readonly Func<object[], object?[]?, object?> _booleanPrototypeValueOfValue = static (_, __) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            return JavaScriptRuntime.Boolean.ThisBooleanValue(thisValue);
        };

        private static readonly Func<object[], object?, string> _stringFunctionValue = static (_, value) =>
            JavaScriptRuntime.DotNet2JSConversions.ToString(value);

        private static readonly Func<object[], object?, double> _numberFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToNumber(value);

        private static readonly Func<object[], object?, Delegate> _functionConstructorValue = static (_, __) =>
            throw new JavaScriptRuntime.Error("The Function constructor only supports compile-time string literal arguments in js2il.");

        // Placeholder Array constructor value. JS2IL uses dedicated codegen paths for `new Array(...)`.
        // We expose a callable value so libraries can reference `Array` as a global identifier and
        // access `Array.prototype.*` members.
        private static readonly Func<object[], object?[], object?> _arrayConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Array constructor is not supported as a callable value yet.");
        private static readonly Func<object[], object?[]?, object?> _arrayIsArrayValue = static (_, args) =>
            JavaScriptRuntime.Array.isArray(args != null && args.Length > 0 ? args[0] : null);

        private static readonly Delegate _mapConstructorValue =
            CreateCollectionConstructorValue("Map", static () => new JavaScriptRuntime.Map());

        private static readonly Delegate _setConstructorValue =
            CreateCollectionConstructorValue("Set", static () => new JavaScriptRuntime.Set());

        private static readonly Delegate _weakMapConstructorValue =
            CreateCollectionConstructorValue("WeakMap", static () => new JavaScriptRuntime.WeakMap());

        private static readonly Delegate _weakSetConstructorValue =
            CreateCollectionConstructorValue("WeakSet", static () => new JavaScriptRuntime.WeakSet());

        private static readonly JsFuncNoScopes1 _promiseConstructorValue = static (newTarget, executor) =>
        {
            if (newTarget is null)
            {
                throw new global::JavaScriptRuntime.TypeError("Constructor Promise requires 'new'");
            }

            return new global::JavaScriptRuntime.Promise(executor);
        };

        // Object constructor/function value. This enables patterns like `Object.prototype` and
        // allows libraries to pass `Object` around as a value.
        private static readonly Func<object[], object?, object> _objectConstructorValue = static (_, value) =>
            JavaScriptRuntime.Object.Construct(value);

        private static readonly Func<object[], object?[], object?> _errorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.Error(message));

        private static readonly Func<object[], object?[], object?> _typeErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.TypeError(message));

        private static readonly Func<object[], object?[], object?> _iteratorConstructorValue = static (_, __) =>
            throw new TypeError("Iterator is not directly constructible in js2il.");

        private static readonly Func<object[], object?[], object?> _asyncIteratorConstructorValue = static (_, __) =>
            throw new TypeError("AsyncIterator is not directly constructible in js2il.");

        private static readonly Func<object[], object?, object> _errorIsErrorValue = static (_, arg) =>
            arg is JavaScriptRuntime.Error;

        // Minimal Error.prototype object. Libraries may attach properties here.
        private static readonly object _errorPrototypeValue = new JsObject();
        private static readonly object _typeErrorPrototypeValue = new JsObject();

        // Minimal Object.prototype object used for descriptor/prototype-heavy libraries.
        // NOTE: We intentionally do not enable PrototypeChain here; Object.create/setPrototypeOf
        // opt into prototype semantics as needed.
        private static readonly object _objectPrototypeValue = new JsObject();
        private static readonly object _booleanPrototypeValue = new JavaScriptRuntime.Boolean(false);
        private static readonly object _promisePrototypeValue = new JsObject();

        // TypedArray intrinsic constructor and prototype
        private static readonly Func<object[], object?[], object?> _typedArrayConstructorValue = static (_, __) =>
            throw new TypeError("%TypedArray% is not directly constructible in js2il.");
        private static readonly object _typedArrayPrototypeValue = new JsObject();

        // Typed array constructor values - supported and unsupported
        private static readonly Func<object[], object?[], object?> _float64ArrayConstructorValue = 
            static (_, args) => new Float64Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _float32ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Float32Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _int32ArrayConstructorValue = 
            static (_, args) => new Int32Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _int16ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Int16Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _int8ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Int8Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _uint32ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint32Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _uint16ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint16Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _uint8ArrayConstructorValue = 
            static (_, args) => new Uint8Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _uint8ClampedArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint8ClampedArray constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _bigInt64ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The BigInt64Array constructor is not yet supported in js2il.");
        private static readonly Func<object[], object?[], object?> _bigUint64ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The BigUint64Array constructor is not yet supported in js2il.");

        static GlobalThis()
        {
            PrototypeChain.SetPrototype(JavaScriptRuntime.Function.Prototype, _objectPrototypeValue);
            PrototypeChain.SetPrototype(JavaScriptRuntime.Function.RestrictedPropertiesPrototype, JavaScriptRuntime.Function.Prototype);

            // Attach minimal prototypes to callable globals so patterns like
            // `Function.prototype.apply.bind(Array.prototype.push)` work even when code only
            // references GlobalThis static properties and never touches the globalThis object.
            ConfigureBuiltinFunctionObject(_functionConstructorValue);
            PropertyDescriptorStore.DefineOrUpdate(_functionConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = JavaScriptRuntime.Function.Prototype
            });
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.Function.Prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _functionConstructorValue
            });
            PropertyDescriptorStore.DefineOrUpdate(_arrayConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = JavaScriptRuntime.Array.Prototype
            });
            ConfigureBuiltinFunctionObject(_arrayIsArrayValue);
            PropertyDescriptorStore.DefineOrUpdate(_arrayConstructorValue, "isArray", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _arrayIsArrayValue
            });
            ConfigurePromiseIntrinsicSurface(_promiseConstructorValue, _promisePrototypeValue);
            ConfigureCollectionIntrinsicSurface(_mapConstructorValue, JavaScriptRuntime.Map.Prototype);
            ConfigureCollectionIntrinsicSurface(_setConstructorValue, JavaScriptRuntime.Set.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakMapConstructorValue, JavaScriptRuntime.WeakMap.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakSetConstructorValue, JavaScriptRuntime.WeakSet.Prototype);
            ConfigureConstructorPrototypeSurface(_promiseConstructorValue, JavaScriptRuntime.Promise.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_booleanFunctionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _booleanPrototypeValue
            });

            JavaScriptRuntime.Iterator.ConfigureIntrinsicSurface(_iteratorConstructorValue);
            JavaScriptRuntime.AsyncIterator.ConfigureIntrinsicSurface(_asyncIteratorConstructorValue);

            // Centralized Object constructor/prototype wiring lives on JavaScriptRuntime.Object.
            JavaScriptRuntime.Object.ConfigureIntrinsicSurface(_objectConstructorValue, _objectPrototypeValue);

            // Provide Error.prototype for patterns like `Error.prototype` and error-subclassing libraries.
            ConfigureErrorIntrinsicSurface(_errorConstructorValue, _errorPrototypeValue, "Error", parentPrototype: _objectPrototypeValue);
            ConfigureErrorIntrinsicSurface(_typeErrorConstructorValue, _typeErrorPrototypeValue, "TypeError", parentPrototype: _errorPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(_booleanPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _booleanFunctionValue
            });
            PropertyDescriptorStore.DefineOrUpdate(_booleanPrototypeValue, "toString", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _booleanPrototypeToStringValue
            });
            PropertyDescriptorStore.DefineOrUpdate(_booleanPrototypeValue, "valueOf", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _booleanPrototypeValueOfValue
            });

            PropertyDescriptorStore.DefineOrUpdate(_errorConstructorValue, "isError", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _errorIsErrorValue
            });

            PropertyDescriptorStore.DefineOrUpdate(_errorPrototypeValue, "message", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = string.Empty
            });
            PropertyDescriptorStore.DefineOrUpdate(_errorPrototypeValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = "Error"
            });
            PropertyDescriptorStore.DefineOrUpdate(_errorPrototypeValue, "toString", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = (Func<object[], object?[], object?>)ErrorPrototypeToString
            });
            ConfigureBuiltinFunctionObject(_typeErrorConstructorValue);
            PropertyDescriptorStore.DefineOrUpdate(_typeErrorConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _typeErrorPrototypeValue
            });
            PrototypeChain.SetPrototype(_typeErrorPrototypeValue, _errorPrototypeValue);
            PropertyDescriptorStore.DefineOrUpdate(_typeErrorPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _typeErrorConstructorValue
            });
            PropertyDescriptorStore.DefineOrUpdate(_typeErrorPrototypeValue, "message", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = string.Empty
            });
            PropertyDescriptorStore.DefineOrUpdate(_typeErrorPrototypeValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = "TypeError"
            });

            ConfigureBuiltinFunctionObject(_typedArrayConstructorValue);
            PrototypeChain.SetPrototype(_typedArrayPrototypeValue, _objectPrototypeValue);
            PropertyDescriptorStore.DefineOrUpdate(_typedArrayConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _typedArrayPrototypeValue
            });
            ConfigureTypedArrayConstructorValue(_float64ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_float32ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_int32ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_int16ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_int8ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_uint32ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_uint16ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_uint8ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_uint8ClampedArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_bigInt64ArrayConstructorValue);
            ConfigureTypedArrayConstructorValue(_bigUint64ArrayConstructorValue);

            JavaScriptRuntime.String.ConfigureIntrinsicSurface(_stringFunctionValue);
        }

        private static object? ErrorPrototypeToString(object[] scopes, object?[] args)
        {
            var thisVal = RuntimeServices.GetCurrentThis();
            if (thisVal is null || thisVal is JsNull)
            {
                throw new TypeError("Error.prototype.toString called on null or undefined");
            }

            var nameValue = JavaScriptRuntime.ObjectRuntime.GetItem(thisVal, "name");
            var messageValue = JavaScriptRuntime.ObjectRuntime.GetItem(thisVal, "message");

            var name = (nameValue is null || nameValue is JsNull)
                ? "Error"
                : DotNet2JSConversions.ToString(nameValue);
            var message = (messageValue is null || messageValue is JsNull)
                ? string.Empty
                : DotNet2JSConversions.ToString(messageValue);

            if (string.IsNullOrEmpty(name)) return message;
            if (string.IsNullOrEmpty(message)) return name;
            return $"{name}: {message}";
        }

        private static void ConfigureTypedArrayConstructorValue(object constructorValue)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            PrototypeChain.SetPrototype(constructorValue, _typedArrayConstructorValue);
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

        private static bool ShouldExposeGc()
        {
            var serviceProvider = ServiceProvider;
            return serviceProvider != null
                && serviceProvider.TryResolve<GlobalThisOptions>(out var options)
                && options != null
                && options.ExposeGc;
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

            dict.TryAdd(nameof(GlobalThis.Promise), Promise);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Promise), dict[nameof(GlobalThis.Promise)]);

            dict.TryAdd(nameof(GlobalThis.Float64Array), Float64Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Float64Array), dict[nameof(GlobalThis.Float64Array)]);

            dict.TryAdd(nameof(GlobalThis.Float32Array), Float32Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Float32Array), dict[nameof(GlobalThis.Float32Array)]);

            dict.TryAdd(nameof(GlobalThis.Int32Array), Int32Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Int32Array), dict[nameof(GlobalThis.Int32Array)]);

            dict.TryAdd(nameof(GlobalThis.Int16Array), Int16Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Int16Array), dict[nameof(GlobalThis.Int16Array)]);

            dict.TryAdd(nameof(GlobalThis.Int8Array), Int8Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Int8Array), dict[nameof(GlobalThis.Int8Array)]);

            dict.TryAdd(nameof(GlobalThis.Uint32Array), Uint32Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Uint32Array), dict[nameof(GlobalThis.Uint32Array)]);

            dict.TryAdd(nameof(GlobalThis.Uint16Array), Uint16Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Uint16Array), dict[nameof(GlobalThis.Uint16Array)]);

            dict.TryAdd(nameof(GlobalThis.Uint8Array), Uint8Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Uint8Array), dict[nameof(GlobalThis.Uint8Array)]);

            dict.TryAdd(nameof(GlobalThis.Uint8ClampedArray), Uint8ClampedArray);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Uint8ClampedArray), dict[nameof(GlobalThis.Uint8ClampedArray)]);

            dict.TryAdd(nameof(GlobalThis.BigInt64Array), BigInt64Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.BigInt64Array), dict[nameof(GlobalThis.BigInt64Array)]);

            dict.TryAdd(nameof(GlobalThis.BigUint64Array), BigUint64Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.BigUint64Array), dict[nameof(GlobalThis.BigUint64Array)]);

            dict.TryAdd(nameof(GlobalThis.Map), Map);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Map), dict[nameof(GlobalThis.Map)]);

            dict.TryAdd(nameof(GlobalThis.Set), Set);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Set), dict[nameof(GlobalThis.Set)]);

            dict.TryAdd(nameof(GlobalThis.WeakMap), WeakMap);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.WeakMap), dict[nameof(GlobalThis.WeakMap)]);

            dict.TryAdd(nameof(GlobalThis.WeakSet), WeakSet);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.WeakSet), dict[nameof(GlobalThis.WeakSet)]);

            dict.TryAdd(nameof(GlobalThis.Object), Object);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Object), dict[nameof(GlobalThis.Object)]);

            dict.TryAdd(nameof(GlobalThis.Error), Error);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Error), dict[nameof(GlobalThis.Error)]);

            dict.TryAdd(nameof(GlobalThis.TypeError), TypeError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.TypeError), dict[nameof(GlobalThis.TypeError)]);

            dict.TryAdd(nameof(GlobalThis.Iterator), Iterator);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Iterator), dict[nameof(GlobalThis.Iterator)]);

            dict.TryAdd(nameof(GlobalThis.AsyncIterator), AsyncIterator);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.AsyncIterator), dict[nameof(GlobalThis.AsyncIterator)]);

            dict.TryAdd(nameof(GlobalThis.AbortController), AbortController);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.AbortController), dict[nameof(GlobalThis.AbortController)]);

            dict.TryAdd(nameof(GlobalThis.AbortSignal), AbortSignal);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.AbortSignal), dict[nameof(GlobalThis.AbortSignal)]);

            dict.TryAdd(nameof(GlobalThis.URL), URL);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.URL), dict[nameof(GlobalThis.URL)]);

            dict.TryAdd(nameof(GlobalThis.URLSearchParams), URLSearchParams);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.URLSearchParams), dict[nameof(GlobalThis.URLSearchParams)]);

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

            if (ShouldExposeGc())
            {
                dict.TryAdd(nameof(GlobalThis.gc), (Func<object?>)gc);
                DefineNonEnumerableDataProperty(nameof(GlobalThis.gc), dict[nameof(GlobalThis.gc)]);
            }

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

        public static Delegate Promise => _promiseConstructorValue;

        public static Delegate Float64Array => _float64ArrayConstructorValue;

        public static Delegate Float32Array => _float32ArrayConstructorValue;

        public static Delegate Int32Array => _int32ArrayConstructorValue;

        public static Delegate Int16Array => _int16ArrayConstructorValue;

        public static Delegate Int8Array => _int8ArrayConstructorValue;

        public static Delegate Uint32Array => _uint32ArrayConstructorValue;

        public static Delegate Uint16Array => _uint16ArrayConstructorValue;

        public static Delegate Uint8Array => _uint8ArrayConstructorValue;

        public static Delegate Uint8ClampedArray => _uint8ClampedArrayConstructorValue;

        public static Delegate BigInt64Array => _bigInt64ArrayConstructorValue;

        public static Delegate BigUint64Array => _bigUint64ArrayConstructorValue;

        public static Delegate Map => _mapConstructorValue;

        public static Delegate Set => _setConstructorValue;

        public static Delegate WeakMap => _weakMapConstructorValue;

        public static Delegate WeakSet => _weakSetConstructorValue;

        public static Func<object[], object?, object> Object => _objectConstructorValue;

        /// <summary>
        /// ECMAScript global Error constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier and
        /// access <c>Error.prototype</c>.
        /// </summary>
        public static Func<object[], object?[], object?> Error => _errorConstructorValue;

        public static Func<object[], object?[], object?> TypeError => _typeErrorConstructorValue;

        public static Func<object[], object?[], object?> Iterator => _iteratorConstructorValue;

        public static Func<object[], object?[], object?> AsyncIterator => _asyncIteratorConstructorValue;

        public static Type AbortController => typeof(JavaScriptRuntime.AbortController);

        public static Type AbortSignal => typeof(JavaScriptRuntime.AbortSignal);

        public static Delegate URL => JavaScriptRuntime.Node.Url.URLConstructorValue;

        public static Delegate URLSearchParams => JavaScriptRuntime.Node.Url.URLSearchParamsConstructorValue;

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
        /// Host/testing helper that forces a .NET GC and queues any resulting FinalizationRegistry cleanup jobs.
        /// This is intentionally non-standard and exists so tests can drive weak-reference cleanup deterministically.
        /// </summary>
        public static object? gc()
        {
            var serviceProvider = _serviceProvider.Value;
            if (serviceProvider == null
                || !serviceProvider.IsRegistered<JavaScriptRuntime.EngineCore.IFinalizationRegistryHost>())
            {
                return null;
            }

            serviceProvider.Resolve<JavaScriptRuntime.EngineCore.IFinalizationRegistryHost>()
                .CollectAndQueueCleanupJobs(forceCollection: true);
            return null;
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
                // ECMA-262: Let R be ℝ(? ToInt32(radix))
                radixValue = TypeUtilities.ToInt32(radix);
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

            // Use double arithmetic for large numbers to match JavaScript behavior
            double value = 0.0;
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

            return sign * value;
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

        /// <summary>
        /// Creates a callable built-in error constructor that applies the shared ECMAScript
        /// message-argument coercion used by the exposed global error constructor values.
        /// </summary>
        private static Func<object[], object?[], object?> CreateErrorConstructorValue(Func<string?, JavaScriptRuntime.Error> factory)
        {
            return (_, args) =>
            {
                string? message = null;
                if (args != null && args.Length > 0 && args[0] is not null and not JsNull)
                {
                    message = DotNet2JSConversions.ToString(args[0]);
                }

                return factory(message);
            };
        }

        private static JsFuncNoScopes1 CreateCollectionConstructorValue(string name, Func<object> factory)
        {
            return (newTarget, iterable) =>
            {
                if (newTarget is null)
                {
                    throw new TypeError($"Constructor {name} requires 'new'");
                }

                if (iterable is not null && iterable is not JsNull)
                {
                    throw new NotSupportedException($"The {name} constructor only supports zero arguments in js2il.");
                }

                return factory();
            };
        }

        private static void ConfigurePromiseIntrinsicSurface(object constructorValue, object prototypeValue)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            PrototypeChain.SetPrototype(prototypeValue, _objectPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototypeValue
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, Symbol.species.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = (Func<object[], object?[]?, object?>)SpeciesGetter
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructorValue
            });
        }

        private static void ConfigureCollectionIntrinsicSurface(object constructorValue, object prototypeValue)
            => ConfigureConstructorPrototypeSurface(constructorValue, prototypeValue);

        private static void ConfigureConstructorPrototypeSurface(object constructorValue, object prototypeValue)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            PrototypeChain.SetPrototype(prototypeValue, _objectPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototypeValue
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructorValue
            });
        }

        private static void ConfigureErrorSubclassIntrinsicSurface(object constructorValue, object prototypeValue, string name)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            PrototypeChain.SetPrototype(prototypeValue, _errorPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototypeValue
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructorValue
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = name
            });
        }

        private static object? SpeciesGetter(object[] scopes, object?[]? args)
        {
            return RuntimeServices.GetCurrentThis();
        }

        internal static object ObjectPrototypeValue => _objectPrototypeValue;
        internal static object ErrorPrototypeValue => _errorPrototypeValue;
        internal static object TypeErrorPrototypeValue => _typeErrorPrototypeValue;

        private static Func<object[], object?[], object?> CreateErrorConstructorValue(Func<string?, object> factory)
        {
            return (_, args) =>
            {
                string? message = null;
                if (args != null && args.Length > 0 && args[0] is not null && args[0] is not JsNull)
                {
                    message = DotNet2JSConversions.ToString(args[0]);
                }

                return factory(message);
            };
        }

        private static void ConfigureErrorIntrinsicSurface(object constructorValue, object prototypeValue, string name, object parentPrototype)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            PrototypeChain.SetPrototype(prototypeValue, parentPrototype);

            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = prototypeValue
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = name
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructorValue
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "message", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = string.Empty
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = name
            });
        }

        internal static void AssignBuiltInErrorPrototype(JavaScriptRuntime.Error error)
        {
            ArgumentNullException.ThrowIfNull(error);

            // Keep this aligned with the explicitly exposed built-in error constructor values above.
            var prototype = error switch
            {
                JavaScriptRuntime.TypeError => _typeErrorPrototypeValue,
                _ => _errorPrototypeValue
            };

            PrototypeChain.SetPrototype(error, prototype);
        }

        internal static void ConfigureBuiltinFunctionObject(object functionValue)
        {
            JavaScriptRuntime.Function.ConfigureCallableObject(functionValue, hasRestrictedProperties: false);
        }
    }
}

