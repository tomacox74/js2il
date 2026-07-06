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
    /// Minimal surface for jroc codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    [IntrinsicObject("GlobalThis")]
    public class GlobalThis : IDynamicMetaObjectProvider, IDictionary<string, object?>
    {
        private static readonly ThreadLocal<ServiceContainer?> _serviceProvider = new(() => null);

        // Per-"realm" (thread) global object. This backs the ECMAScript globalThis value.
        // We represent it as a GlobalThis instance with ExpandoObject-like behavior.
        private static readonly ThreadLocal<GlobalThis?> _globalObject = new(() => null);

        private readonly ExpandoObject _expando = new();
        private IDictionary<string, object?> Properties => (IDictionary<string, object?>)_expando;

        private static readonly JavaScriptRuntime.Console _defaultConsole = new(new ConsoleOutputSinks());
        private static readonly JavaScriptRuntime.Node.Process _defaultProcess = new(new DefaultEnvironment());

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
        private static readonly Func<object[], object?, object> _bigIntFunctionValue = static (_, value) =>
            global::JavaScriptRuntime.BigInt.Call(value);
        private static readonly Func<object[], object?[]?, object?> _bigIntAsIntNValue = static (_, args) =>
        {
            args ??= global::System.Array.Empty<object?>();
            var bits = args.Length > 0 ? args[0] : null;
            var bigint = args.Length > 1 ? args[1] : null;
            return global::JavaScriptRuntime.BigInt.AsIntN(bits, bigint);
        };
        private static readonly Func<object[], object?[]?, object?> _bigIntAsUintNValue = static (_, args) =>
        {
            args ??= global::System.Array.Empty<object?>();
            var bits = args.Length > 0 ? args[0] : null;
            var bigint = args.Length > 1 ? args[1] : null;
            return global::JavaScriptRuntime.BigInt.AsUintN(bits, bigint);
        };

        private static readonly Func<object[], object?[]?, object?> _numberPrototypeToStringValue = static (_, __) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            var numberValue = JavaScriptRuntime.Number.ThisNumberValue(thisValue);
            return JavaScriptRuntime.DotNet2JSConversions.ToString(numberValue);
        };

        private static readonly Func<object[], object?[]?, object?> _numberPrototypeValueOfValue = static (_, __) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            return JavaScriptRuntime.Number.ThisNumberValue(thisValue);
        };
        private static readonly Func<object[], object?[]?, object?> _numberPrototypeToExponentialValue = static (_, args) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            var fractionDigits = args != null && args.Length > 0 ? args[0] : null;
            return JavaScriptRuntime.Number.ToExponentialString(thisValue, fractionDigits);
        };
        private static readonly Func<object[], object?[]?, object?> _numberPrototypeToFixedValue = static (_, args) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            var fractionDigits = args != null && args.Length > 0 ? args[0] : null;
            return JavaScriptRuntime.Number.ToFixedString(thisValue, fractionDigits);
        };
        private static readonly Func<object[], object?[]?, object?> _numberPrototypeToLocaleStringValue = static (_, __) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            return JavaScriptRuntime.Number.ToLocaleStringString(thisValue);
        };
        private static readonly Func<object[], object?[]?, object?> _numberPrototypeToPrecisionValue = static (_, args) =>
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            var precision = args != null && args.Length > 0 ? args[0] : null;
            return JavaScriptRuntime.Number.ToPrecisionString(thisValue, precision);
        };
        private static readonly Func<object[], object?, Delegate> _functionConstructorValue = static (_, __) =>
            throw new JavaScriptRuntime.Error("The Function constructor only supports compile-time string literal arguments in jroc.");

        // Placeholder Array constructor value. JROC uses dedicated codegen paths for `new Array(...)`.
        // We expose a callable value so libraries can reference `Array` as a global identifier and
        // access `Array.prototype.*` members.
        private static readonly Func<object[], object?[], object?> _arrayConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Array constructor is not supported as a callable value yet.");
        private static readonly Func<object?, bool> _arrayIsArrayValue = JavaScriptRuntime.Array.isArray;
        private static readonly Func<object[], object?[]?, object?> _arrayFromValue = static (_, args) =>
        {
            var source = args != null && args.Length > 0 ? args[0] : null;
            var mapFn = args != null && args.Length > 1 ? args[1] : null;
            var thisArg = args != null && args.Length > 2 ? args[2] : null;
            return JavaScriptRuntime.Array.from(source, mapFn, thisArg);
        };
        private static readonly Func<object?, object?, double> _parseIntValue = parseInt;
        private static readonly Func<object?, double> _parseFloatValue = parseFloat;
        private static readonly Func<object?, bool> _isFiniteValue = isFinite;
        private static readonly Func<object?, bool> _isNaNValue = isNaN;
        private static readonly Func<object?, bool> _numberIsFiniteValue = JavaScriptRuntime.Number.isFinite;
        private static readonly Func<object?, bool> _numberIsIntegerValue = JavaScriptRuntime.Number.isInteger;
        private static readonly Func<object?, bool> _numberIsNaNValue = JavaScriptRuntime.Number.isNaN;

        private static readonly Delegate _mapConstructorValue =
            CreateCollectionConstructorValue("Map", static iterable => new JavaScriptRuntime.Map(iterable));

        private static readonly Delegate _setConstructorValue =
            CreateCollectionConstructorValue("Set", static iterable => new JavaScriptRuntime.Set(iterable));

        private static readonly JsFuncNoScopes1 _weakMapConstructorValue = static (newTarget, iterable) =>
        {
            if (newTarget is null)
            {
                throw new TypeError("Constructor WeakMap requires 'new'");
            }

            return new JavaScriptRuntime.WeakMap(iterable);
        };

        private static readonly JsFuncNoScopes1 _weakSetConstructorValue = static (newTarget, iterable) =>
        {
            if (newTarget is null)
            {
                throw new TypeError("Constructor WeakSet requires 'new'");
            }

            return new JavaScriptRuntime.WeakSet(iterable);
        };

        private static readonly JsFuncNoScopes1 _promiseConstructorValue = static (newTarget, executor) =>
        {
            if (newTarget is null)
            {
                throw new global::JavaScriptRuntime.TypeError("Constructor Promise requires 'new'");
            }

            return new global::JavaScriptRuntime.Promise(executor);
        };
        private static readonly Func<object[], object?, object?> _promiseResolveValue = static (_, value) =>
            global::JavaScriptRuntime.Promise.ResolveForConstructor(RuntimeServices.GetCurrentThis(), value);
        private static readonly Func<object[], object?, object?> _promiseAllValue = static (_, iterable) =>
            global::JavaScriptRuntime.Promise.all(iterable);
        private static readonly Func<object[], object?, object?> _promiseRaceValue = static (_, iterable) =>
            global::JavaScriptRuntime.Promise.race(iterable);
        private static readonly Func<object[], object?, object?> _promiseRejectValue = static (_, reason) =>
            global::JavaScriptRuntime.Promise.reject(reason);
        private static readonly Func<object[], object?[]?, object?> _promiseTryValue = static (_, args) =>
        {
            args ??= global::System.Array.Empty<object?>();
            var callback = args.Length > 0 ? args[0] : null;
            object?[] callbackArgs;
            if (args.Length <= 1)
            {
                callbackArgs = global::System.Array.Empty<object?>();
            }
            else
            {
                callbackArgs = new object?[args.Length - 1];
                global::System.Array.Copy(args, 1, callbackArgs, 0, callbackArgs.Length);
            }

            return global::JavaScriptRuntime.Promise.TryForConstructor(
                RuntimeServices.GetCurrentThis(),
                callback,
                callbackArgs);
        };
        private static readonly Func<object[], object?[]?, object?> _speciesGetterValue = SpeciesGetter;

        private static readonly JsFuncNoScopes2 _proxyConstructorValue = static (newTarget, target, handler) =>
        {
            if (newTarget is null)
            {
                throw new global::JavaScriptRuntime.TypeError("Constructor Proxy requires 'new'");
            }

            return new global::JavaScriptRuntime.Proxy(target, handler);
        };

        private static readonly Func<object[], object?[]?, object?> _proxyRevocableValue = static (_, args) =>
        {
            args ??= global::System.Array.Empty<object?>();
            return global::JavaScriptRuntime.Proxy.revocable(
                args.Length > 0 ? args[0] : null,
                args.Length > 1 ? args[1] : null);
        };

        // Object constructor/function value. This enables patterns like `Object.prototype` and
        // allows libraries to pass `Object` around as a value.
        private static readonly Func<object[], object?, object> _objectConstructorValue = static (_, value) =>
            JavaScriptRuntime.Object.Construct(value);
        private static readonly Func<object[], object?[], object?> _regExpConstructorValue = static (_, args) =>
        {
            var pattern = (args != null && args.Length > 0) ? args[0] : null;
            var flags = (args != null && args.Length > 1) ? args[1] : null;
            return JavaScriptRuntime.RegExp.Call(pattern, flags);
        };

        private static readonly Func<object[], object?[], object?> _jsonStringifyValue = static (_, args) =>
        {
            var value = args != null && args.Length > 0 ? args[0] : null;
            var replacer = args != null && args.Length > 1 ? args[1] : null;
            var space = args != null && args.Length > 2 ? args[2] : null;
            return JavaScriptRuntime.JSON.Stringify(value, replacer, space);
        };

        private static readonly Func<object[], object?[], object?> _errorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.Error(message));

        private static readonly Func<object[], object?[], object?> _evalErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.EvalError(message));

        private static readonly Func<object[], object?[], object?> _rangeErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.RangeError(message));

        private static readonly Func<object[], object?[], object?> _referenceErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.ReferenceError(message));

        private static readonly Func<object[], object?[], object?> _syntaxErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.SyntaxError(message));

        private static readonly Func<object[], object?[], object?> _typeErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.TypeError(message));

        private static readonly Func<object[], object?[], object?> _uriErrorConstructorValue =
            CreateErrorConstructorValue(static message => new JavaScriptRuntime.URIError(message));

        private static readonly Func<object[], object?[], object?> _iteratorConstructorValue = static (_, __) =>
            throw new TypeError("Iterator is not directly constructible in jroc.");

        private static readonly Func<object[], object?[], object?> _asyncIteratorConstructorValue = static (_, __) =>
            throw new TypeError("AsyncIterator is not directly constructible in jroc.");

        private static readonly Func<object[], object?, object> _errorIsErrorValue = static (_, arg) =>
            arg is JavaScriptRuntime.Error;

        // Minimal Error.prototype object. Libraries may attach properties here.
        private static readonly object _errorPrototypeValue = new JsObject();
        private static readonly object _evalErrorPrototypeValue = new JsObject();
        private static readonly object _rangeErrorPrototypeValue = new JsObject();
        private static readonly object _referenceErrorPrototypeValue = new JsObject();
        private static readonly object _syntaxErrorPrototypeValue = new JsObject();
        private static readonly object _typeErrorPrototypeValue = new JsObject();
        private static readonly object _uriErrorPrototypeValue = new JsObject();

        // Minimal Object.prototype object used for descriptor/prototype-heavy libraries.
        // NOTE: We intentionally do not enable PrototypeChain here; Object.create/setPrototypeOf
        // opt into prototype semantics as needed.
        private static readonly object _objectPrototypeValue = new JsObject();
        private static readonly object _jsonValue = new JsObject();
        private static readonly object _intlValue = new JsObject();
        private static readonly object _atomicsValue = new JsObject();
        private static readonly object _numberPrototypeValue = new JsObject();
        private static readonly object _booleanPrototypeValue = new JsObject();
        private static readonly object _symbolPrototypeValue = new JsObject();
        private static readonly object _promisePrototypeValue = new JsObject();
        private static readonly object _arrayBufferPrototypeValue = new JsObject();
        private static readonly Func<object[], object?[]?, object?> _symbolFunctionValue = SymbolCall;

        // TypedArray intrinsic constructor and prototype
        private static readonly Func<object[], object?[], object?> _typedArrayConstructorValue = static (_, __) =>
            throw new TypeError("%TypedArray% is not directly constructible in jroc.");
        private static readonly object _typedArrayPrototypeValue = new JsObject();

        // Typed array constructor values - supported and unsupported
        private static readonly Func<object[], object?[], object?> _float64ArrayConstructorValue = 
            static (_, args) => new Float64Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _float32ArrayConstructorValue = 
            static (_, args) => new Float32Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _int32ArrayConstructorValue = 
            static (_, args) => new Int32Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _arrayBufferConstructorValue =
            static (_, args) => args != null && args.Length > 1
                ? new ArrayBuffer(args[0], args[1])
                : new ArrayBuffer(args != null && args.Length > 0 ? args[0] : null);
        private static readonly Func<object[], object?[], object?> _sharedArrayBufferConstructorValue =
            static (_, args) => new SharedArrayBuffer(args != null && args.Length > 0 ? args[0] : null);
        private static readonly Func<object[], object?[], object?> _int16ArrayConstructorValue = 
            static (_, args) => new Int16Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _int8ArrayConstructorValue = 
            static (_, args) => new Int8Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _uint32ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint32Array constructor is not yet supported in jroc.");
        private static readonly Func<object[], object?[], object?> _uint16ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint16Array constructor is not yet supported in jroc.");
        private static readonly Func<object[], object?[], object?> _uint8ArrayConstructorValue = 
            static (_, args) => new Uint8Array(args ?? global::System.Array.Empty<object?>());
        private static readonly Func<object[], object?[], object?> _uint8ClampedArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The Uint8ClampedArray constructor is not yet supported in jroc.");
        private static readonly Func<object[], object?[], object?> _bigInt64ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The BigInt64Array constructor is not yet supported in jroc.");
        private static readonly Func<object[], object?[], object?> _bigUint64ArrayConstructorValue = 
            static (_, __) => throw new NotSupportedException("The BigUint64Array constructor is not yet supported in jroc.");

        static GlobalThis()
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PrototypeChain.SetPrototype(JavaScriptRuntime.Function.Prototype, _objectPrototypeValue);
            PrototypeChain.SetPrototype(JavaScriptRuntime.Function.RestrictedPropertiesPrototype, JavaScriptRuntime.Function.Prototype);
            DefineIntrinsicToStringTagProperty(Math, "Math");
            DefineIntrinsicConstantDataProperty(Math, "E", JavaScriptRuntime.Math.E);
            DefineIntrinsicConstantDataProperty(Math, "LN10", JavaScriptRuntime.Math.LN10);
            DefineIntrinsicConstantDataProperty(Math, "LN2", JavaScriptRuntime.Math.LN2);
            DefineIntrinsicConstantDataProperty(Math, "LOG10E", JavaScriptRuntime.Math.LOG10E);
            DefineIntrinsicConstantDataProperty(Math, "LOG2E", JavaScriptRuntime.Math.LOG2E);
            DefineIntrinsicConstantDataProperty(Math, "PI", JavaScriptRuntime.Math.PI);
            DefineIntrinsicConstantDataProperty(Math, "SQRT1_2", JavaScriptRuntime.Math.SQRT1_2);
            DefineIntrinsicConstantDataProperty(Math, "SQRT2", JavaScriptRuntime.Math.SQRT2);
            DefineIntrinsicToStringTagProperty(JSON, "JSON");
            DefineIntrinsicToStringTagProperty(Reflect, "Reflect");
            DefineIntrinsicToStringTagProperty(_intlValue, "Intl");
            DefineIntrinsicDataProperty(_intlValue, "NumberFormat", typeof(JavaScriptRuntime.IntlNumberFormat));
            DefineIntrinsicDataProperty(_intlValue, "Segmenter", typeof(JavaScriptRuntime.IntlSegmenter));

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
                Value = JavaScriptRuntime.Array.ImmutablePrototype
            });
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.Array.ImmutablePrototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _arrayConstructorValue
            });
            ConfigureBuiltinFunctionObject(_arrayIsArrayValue);
            DefineUndefinedPrototypeProperty(_arrayIsArrayValue);
            PropertyDescriptorStore.DefineOrUpdate(_arrayConstructorValue, "isArray", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _arrayIsArrayValue
            });
            ConfigureBuiltinFunctionObject(_arrayFromValue);
            PropertyDescriptorStore.DefineOrUpdate(_arrayFromValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "from"
            });
            PropertyDescriptorStore.DefineOrUpdate(_arrayFromValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_arrayConstructorValue, "from", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _arrayFromValue
            });
            ConfigurePromiseIntrinsicSurface(_promiseConstructorValue, _promisePrototypeValue);
            JavaScriptRuntime.Function.InitializeFunctionInstance(_proxyConstructorValue, 2d, "Proxy");
            JavaScriptRuntime.Function.InitializeFunctionInstance(_proxyRevocableValue, 2d, "revocable");
            DefineUndefinedPrototypeProperty(_proxyRevocableValue);
            DefineIntrinsicDataProperty(_proxyConstructorValue, "revocable", _proxyRevocableValue);
            ConfigureCollectionIntrinsicSurface(_mapConstructorValue, JavaScriptRuntime.Map.Prototype);
            ConfigureCollectionIntrinsicSurface(_setConstructorValue, JavaScriptRuntime.Set.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakMapConstructorValue, JavaScriptRuntime.WeakMap.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakSetConstructorValue, JavaScriptRuntime.WeakSet.Prototype);
            ConfigureCollectionConstructorMetadata(_mapConstructorValue, "Map");
            ConfigureCollectionConstructorMetadata(_setConstructorValue, "Set");
            ConfigureCollectionConstructorMetadata(_weakMapConstructorValue, "WeakMap");
            ConfigureCollectionConstructorMetadata(_weakSetConstructorValue, "WeakSet");
            ConfigureConstructorPrototypeSurface(_promiseConstructorValue, JavaScriptRuntime.Promise.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_promiseConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_promiseConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Promise"
            });
            ConfigureBuiltinFunctionObject(_promiseResolveValue);
            DefineUndefinedPrototypeProperty(_promiseResolveValue);
            PropertyDescriptorStore.DefineOrUpdate(_promiseResolveValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_promiseResolveValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "resolve"
            });
            DefineIntrinsicDataProperty(_promiseConstructorValue, "resolve", _promiseResolveValue);
            DefineBuiltinFunctionProperty(_promiseConstructorValue, "all", _promiseAllValue, 1d);
            DefineBuiltinFunctionProperty(_promiseConstructorValue, "race", _promiseRaceValue, 1d);
            DefineBuiltinFunctionProperty(_promiseConstructorValue, "reject", _promiseRejectValue, 1d);
            DefineBuiltinFunctionProperty(_promiseConstructorValue, "try", _promiseTryValue, 1d);
            PropertyDescriptorStore.DefineOrUpdate(_booleanFunctionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _booleanPrototypeValue
            });
            ConfigureBuiltinFunctionObject(_symbolFunctionValue);
            PropertyDescriptorStore.DefineOrUpdate(_symbolFunctionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _symbolPrototypeValue
            });

            JavaScriptRuntime.Iterator.ConfigureIntrinsicSurface(_iteratorConstructorValue);
            JavaScriptRuntime.AsyncIterator.ConfigureIntrinsicSurface(_asyncIteratorConstructorValue);

            // Centralized Object constructor/prototype wiring lives on JavaScriptRuntime.Object.
            ConfigureBuiltinFunctionObject(_objectConstructorValue);
            JavaScriptRuntime.Object.ConfigureIntrinsicSurface(_objectConstructorValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(JavaScriptRuntime.Array.ImmutablePrototype, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_jsonValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_atomicsValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_numberPrototypeValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_booleanPrototypeValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_symbolPrototypeValue, _objectPrototypeValue);
            PropertyDescriptorStore.DefineOrUpdate(_numberPrototypeValue, JavaScriptRuntime.Number.NumberDataPropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = true,
                Value = 0d
            });
            DefineIntrinsicDataProperty(_jsonValue, "parse", (Func<object?, object?>)JavaScriptRuntime.JSON.Parse);
            DefineIntrinsicToStringTagProperty(_atomicsValue, "Atomics");
            DefineBuiltinFunctionProperty(_atomicsValue, "wait", (Func<object?, object?, object?, object?, string>)JavaScriptRuntime.Atomics.wait, 4d);
            ConfigureBuiltinFunctionObject(_jsonStringifyValue);
            PropertyDescriptorStore.DefineOrUpdate(_jsonStringifyValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "stringify"
            });
            PropertyDescriptorStore.DefineOrUpdate(_jsonStringifyValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 3d
            });
            DefineIntrinsicDataProperty(_jsonValue, "stringify", _jsonStringifyValue);
            DefineIntrinsicDataProperty(_numberPrototypeValue, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, "Number");
            ConfigureConstructorPrototypeSurface(_regExpConstructorValue, JavaScriptRuntime.RegExp.Prototype);
            ConfigureBuiltinFunctionObject(_numberFunctionValue);
            PropertyDescriptorStore.DefineOrUpdate(_numberFunctionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _numberPrototypeValue
            });
            DefineIntrinsicDataProperty(_numberPrototypeValue, "constructor", _numberFunctionValue);
            ConfigureBuiltinFunctionObject(_numberIsIntegerValue);
            ConfigureBuiltinFunctionObject(_numberIsFiniteValue);
            ConfigureBuiltinFunctionObject(_numberIsNaNValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isFinite", _numberIsFiniteValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isInteger", _numberIsIntegerValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isNaN", _numberIsNaNValue);
            DefineUndefinedPrototypeProperty(_numberIsFiniteValue);
            DefineUndefinedPrototypeProperty(_numberIsIntegerValue);
            DefineUndefinedPrototypeProperty(_numberIsNaNValue);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "toExponential", _numberPrototypeToExponentialValue, 1d);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "toFixed", _numberPrototypeToFixedValue, 1d);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "toLocaleString", _numberPrototypeToLocaleStringValue, 0d);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "toPrecision", _numberPrototypeToPrecisionValue, 1d);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "toString", _numberPrototypeToStringValue, 1d);
            DefineBuiltinFunctionProperty(_numberPrototypeValue, "valueOf", _numberPrototypeValueOfValue, 0d);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "MAX_VALUE", double.MaxValue);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "MIN_VALUE", double.Epsilon);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "MAX_SAFE_INTEGER", 9007199254740991d);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "MIN_SAFE_INTEGER", -9007199254740991d);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "NaN", double.NaN);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "NEGATIVE_INFINITY", double.NegativeInfinity);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "POSITIVE_INFINITY", double.PositiveInfinity);
            DefineIntrinsicConstantDataProperty(_numberFunctionValue, "EPSILON", 2.220446049250313e-16);
            DefineIntrinsicDataProperty(_numberFunctionValue, "parseFloat", _parseFloatValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "parseInt", _parseIntValue);
            JavaScriptRuntime.Function.InitializeFunctionInstance(_bigIntFunctionValue, 1d, "BigInt");
            DefineUndefinedPrototypeProperty(_bigIntFunctionValue);
            DefineBuiltinFunctionProperty(_bigIntFunctionValue, "asIntN", _bigIntAsIntNValue, 2d);
            DefineBuiltinFunctionProperty(_bigIntFunctionValue, "asUintN", _bigIntAsUintNValue, 2d);
            JavaScriptRuntime.Date.InitializeIntrinsicSurface(_objectPrototypeValue);
            ConfigureBuiltinFunctionObject(_stringFunctionValue);
            ConfigureBuiltinFunctionObject(_booleanFunctionValue);
            ConfigureBuiltinFunctionObject(_parseIntValue);
            ConfigureBuiltinFunctionObject(_parseFloatValue);
            ConfigureBuiltinFunctionObject(_isFiniteValue);
            ConfigureBuiltinFunctionObject(_isNaNValue);
            DefineUndefinedPrototypeProperty(_parseIntValue);
            DefineUndefinedPrototypeProperty(_parseFloatValue);
            DefineUndefinedPrototypeProperty(_isFiniteValue);
            DefineUndefinedPrototypeProperty(_isNaNValue);

            // Provide Error.prototype for patterns like `Error.prototype` and error-subclassing libraries.
            ConfigureErrorIntrinsicSurface(_errorConstructorValue, _errorPrototypeValue, "Error", parentPrototype: _objectPrototypeValue);
            ConfigureErrorIntrinsicSurface(_evalErrorConstructorValue, _evalErrorPrototypeValue, "EvalError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_rangeErrorConstructorValue, _rangeErrorPrototypeValue, "RangeError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_referenceErrorConstructorValue, _referenceErrorPrototypeValue, "ReferenceError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_syntaxErrorConstructorValue, _syntaxErrorPrototypeValue, "SyntaxError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_typeErrorConstructorValue, _typeErrorPrototypeValue, "TypeError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_uriErrorConstructorValue, _uriErrorPrototypeValue, "URIError", parentPrototype: _errorPrototypeValue);

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
            DefineIntrinsicDataProperty(_booleanPrototypeValue, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, "Boolean");

            PropertyDescriptorStore.DefineOrUpdate(_symbolPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _symbolFunctionValue
            });
            PropertyDescriptorStore.DefineOrUpdate(_symbolPrototypeValue, "toString", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = (Func<object[], object?[]?, object?>)((_, __) =>
                {
                    return TryGetThisSymbolValue(out var symbol)
                        ? symbol.toString()
                        : throw new TypeError("Symbol.prototype.toString called on incompatible receiver");
                })
            });
            PropertyDescriptorStore.DefineOrUpdate(_symbolPrototypeValue, "valueOf", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = (Func<object[], object?[]?, object?>)((_, __) =>
                {
                    return TryGetThisSymbolValue(out var symbol)
                        ? symbol.valueOf()
                        : throw new TypeError("Symbol.prototype.valueOf called on incompatible receiver");
                })
            });
            DefineIntrinsicDataProperty(_symbolPrototypeValue, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, "Symbol");
            DefineIntrinsicDataProperty(_symbolFunctionValue, "for", (Func<object?, object>)global::JavaScriptRuntime.Symbol.@for);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "keyFor", (Func<object?, object?>)global::JavaScriptRuntime.Symbol.keyFor);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "iterator", global::JavaScriptRuntime.Symbol.iterator);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "asyncIterator", global::JavaScriptRuntime.Symbol.asyncIterator);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "hasInstance", global::JavaScriptRuntime.Symbol.hasInstance);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "isConcatSpreadable", global::JavaScriptRuntime.Symbol.isConcatSpreadable);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "match", global::JavaScriptRuntime.Symbol.match);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "matchAll", global::JavaScriptRuntime.Symbol.matchAll);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "replace", global::JavaScriptRuntime.Symbol.replace);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "search", global::JavaScriptRuntime.Symbol.search);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "species", global::JavaScriptRuntime.Symbol.species);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "split", global::JavaScriptRuntime.Symbol.split);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "toPrimitive", global::JavaScriptRuntime.Symbol.toPrimitive);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "toStringTag", global::JavaScriptRuntime.Symbol.toStringTag);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "unscopables", global::JavaScriptRuntime.Symbol.unscopables);

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
            ConfigureTypedArrayInstancePrototype(_uint8ArrayConstructorValue, JavaScriptRuntime.Uint8Array.Prototype);
            ConfigureConstructorPrototypeSurface(_arrayBufferConstructorValue, _arrayBufferPrototypeValue);

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

        private static void ConfigureTypedArrayInstancePrototype(object constructorValue, object prototypeValue)
        {
            PrototypeChain.SetPrototype(prototypeValue, _typedArrayPrototypeValue);
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
        internal static ServiceContainer? ServiceProvider
        {
            get => _serviceProvider.Value;
            set
            {
                _serviceProvider.Value = value;
                if (value?.TryResolve<IPropertyDescriptorStore>(out var propertyDescriptorStore) == true
                    && propertyDescriptorStore != null)
                {
                    PropertyDescriptorStore.SetCurrentRuntimeStore(propertyDescriptorStore);
                }
                else
                {
                    PropertyDescriptorStore.SetCurrentRuntimeStore(null);
                }

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
        /// JROC models the global object as a dynamic bag (ExpandoObject) seeded with common globals.
        /// This allows libraries to read/write properties via globalThis (e.g., globalThis.window = ...).
        /// </remarks>
        public static object globalThis => GetOrCreateGlobalObject();

        /// <summary>
        /// Returns the current global object for codegen helpers.
        /// </summary>
        public static object GetGlobalThis() => globalThis;

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

        private void DefineNonEnumerableConstantDataProperty(string key, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(this, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = value
            });
        }

        private void DefineDataProperty(string key, object? value, RuntimeGlobalPropertyAttributes attributes)
        {
            PropertyDescriptorStore.DefineOrUpdate(this, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = attributes.Enumerable,
                Configurable = attributes.Configurable,
                Writable = attributes.Writable,
                Value = value
            });
        }


        private static void DefineIntrinsicDataProperty(object target, string key, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
        }

        private static void DefineIntrinsicConstantDataProperty(object target, string key, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = value
            });
        }

        private static void DefineIntrinsicToStringTagProperty(object target, string value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = value
            });
        }

        private static void DefineBuiltinFunctionProperty(object target, string key, Delegate functionValue, double length)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(functionValue, length, key);
            DefineUndefinedPrototypeProperty(functionValue);
            DefineIntrinsicDataProperty(target, key, functionValue);
        }

        private static void DefineUndefinedPrototypeProperty(Delegate functionValue)
        {
            PropertyDescriptorStore.DefineOrUpdate(functionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
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
            DefineNonEnumerableConstantDataProperty(nameof(GlobalThis.Infinity), dict[nameof(GlobalThis.Infinity)]);

            dict.TryAdd(nameof(GlobalThis.NaN), NaN);
            DefineNonEnumerableConstantDataProperty(nameof(GlobalThis.NaN), dict[nameof(GlobalThis.NaN)]);

            dict.TryAdd("undefined", null);
            DefineNonEnumerableConstantDataProperty("undefined", null);

            dict.TryAdd(nameof(GlobalThis.Boolean), Boolean);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Boolean), dict[nameof(GlobalThis.Boolean)]);

            dict.TryAdd(nameof(GlobalThis.String), String);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.String), dict[nameof(GlobalThis.String)]);

            dict.TryAdd(nameof(GlobalThis.Number), Number);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Number), dict[nameof(GlobalThis.Number)]);

            dict.TryAdd(nameof(GlobalThis.BigInt), BigInt);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.BigInt), dict[nameof(GlobalThis.BigInt)]);

            dict.TryAdd(nameof(GlobalThis.Function), Function);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Function), dict[nameof(GlobalThis.Function)]);

            dict.TryAdd(nameof(GlobalThis.SharedArrayBuffer), SharedArrayBuffer);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.SharedArrayBuffer), dict[nameof(GlobalThis.SharedArrayBuffer)]);

            dict.TryAdd(nameof(GlobalThis.ArrayBuffer), ArrayBuffer);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.ArrayBuffer), dict[nameof(GlobalThis.ArrayBuffer)]);

            dict.TryAdd(nameof(GlobalThis.Atomics), Atomics);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Atomics), dict[nameof(GlobalThis.Atomics)]);

            dict.TryAdd(nameof(GlobalThis.Array), Array);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Array), dict[nameof(GlobalThis.Array)]);

            dict.TryAdd(nameof(GlobalThis.Date), Date);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Date), dict[nameof(GlobalThis.Date)]);

            dict.TryAdd(nameof(GlobalThis.Promise), Promise);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Promise), dict[nameof(GlobalThis.Promise)]);

            dict.TryAdd(nameof(GlobalThis.Proxy), Proxy);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Proxy), dict[nameof(GlobalThis.Proxy)]);

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

            dict.TryAdd(nameof(GlobalThis.JSON), JSON);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.JSON), dict[nameof(GlobalThis.JSON)]);

            dict.TryAdd(nameof(GlobalThis.Intl), Intl);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Intl), dict[nameof(GlobalThis.Intl)]);

            dict.TryAdd(nameof(GlobalThis.RegExp), RegExp);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.RegExp), dict[nameof(GlobalThis.RegExp)]);

            dict.TryAdd(nameof(GlobalThis.Symbol), Symbol);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Symbol), dict[nameof(GlobalThis.Symbol)]);

            dict.TryAdd(nameof(GlobalThis.Math), Math);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Math), dict[nameof(GlobalThis.Math)]);

            dict.TryAdd(nameof(GlobalThis.Reflect), Reflect);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Reflect), dict[nameof(GlobalThis.Reflect)]);

            dict.TryAdd(nameof(GlobalThis.Error), Error);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Error), dict[nameof(GlobalThis.Error)]);

            dict.TryAdd(nameof(GlobalThis.EvalError), EvalError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.EvalError), dict[nameof(GlobalThis.EvalError)]);

            dict.TryAdd(nameof(GlobalThis.RangeError), RangeError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.RangeError), dict[nameof(GlobalThis.RangeError)]);

            dict.TryAdd(nameof(GlobalThis.ReferenceError), ReferenceError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.ReferenceError), dict[nameof(GlobalThis.ReferenceError)]);

            dict.TryAdd(nameof(GlobalThis.SyntaxError), SyntaxError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.SyntaxError), dict[nameof(GlobalThis.SyntaxError)]);

            dict.TryAdd(nameof(GlobalThis.TypeError), TypeError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.TypeError), dict[nameof(GlobalThis.TypeError)]);

            dict.TryAdd(nameof(GlobalThis.URIError), URIError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.URIError), dict[nameof(GlobalThis.URIError)]);

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

            dict.TryAdd(nameof(GlobalThis.parseInt), _parseIntValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.parseInt), dict[nameof(GlobalThis.parseInt)]);

            dict.TryAdd(nameof(GlobalThis.parseFloat), _parseFloatValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.parseFloat), dict[nameof(GlobalThis.parseFloat)]);

            dict.TryAdd(nameof(GlobalThis.isFinite), _isFiniteValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.isFinite), dict[nameof(GlobalThis.isFinite)]);

            dict.TryAdd(nameof(GlobalThis.isNaN), _isNaNValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.isNaN), dict[nameof(GlobalThis.isNaN)]);

            ApplyHostGlobalBindings(dict);
        }

        private void ApplyHostGlobalBindings(IDictionary<string, object?> dict)
        {
            var serviceProvider = ServiceProvider;
            if (serviceProvider == null
                || !serviceProvider.TryResolve<HostRuntimeIntrinsicDescriptors>(out var hostRuntimeIntrinsics)
                || hostRuntimeIntrinsics == null)
            {
                return;
            }

            foreach (var descriptor in hostRuntimeIntrinsics.GlobalBindings)
            {
                if (dict.ContainsKey(descriptor.Name)
                    && descriptor.OverwritePolicy == RuntimeGlobalOverwritePolicy.PreserveExisting)
                {
                    continue;
                }

                var value = descriptor.CreateValue();
                dict[descriptor.Name] = value;
                DefineDataProperty(descriptor.Name, value, descriptor.PropertyAttributes);
            }
        }

        /// <summary>
        /// Minimal process global with writable exitCode.
        /// </summary>
        /// <remarks>Expand as needed in the future.</remarks>
        public static JavaScriptRuntime.Node.Process process
        {
            get
            {
                var serviceProvider = _serviceProvider.Value;
                return serviceProvider != null
                    ? serviceProvider.Resolve<JavaScriptRuntime.Node.Process>()
                    : _defaultProcess;
            }
        }

        /// <summary>
        /// Global console object (lowercase) to mirror JS global. Provides access to log/error/warn via the Console intrinsic.
        /// Backed by a single shared instance.
        /// </summary>
        public static JavaScriptRuntime.Console console 
        {
            get
            {
                var serviceProvider = _serviceProvider.Value;
                return serviceProvider != null
                    ? serviceProvider.Resolve<JavaScriptRuntime.Console>()
                    : _defaultConsole;
            }
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
        /// ECMAScript global BigInt conversion function value.
        /// </summary>
        public static Func<object[], object?, object> BigInt => _bigIntFunctionValue;

        /// <summary>
        /// ECMAScript global Function constructor value (placeholder).
        /// Currently exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Function constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?, Delegate> Function => _functionConstructorValue;

        public static Delegate SharedArrayBuffer => _sharedArrayBufferConstructorValue;
        public static Delegate ArrayBuffer => _arrayBufferConstructorValue;

        public static object Atomics => _atomicsValue;

        /// <summary>
        /// ECMAScript global Array constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Array constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?[], object?> Array => _arrayConstructorValue;

        internal static bool IsArrayConstructorValue(object? value)
            => ReferenceEquals(value, _arrayConstructorValue);

        public static Type Date => typeof(JavaScriptRuntime.Date);

        public static Delegate Promise => _promiseConstructorValue;

        public static Delegate Proxy => _proxyConstructorValue;

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

        public static object JSON => _jsonValue;

        public static object Intl => _intlValue;

        public static Delegate Symbol => _symbolFunctionValue;

        public static Type Math => typeof(JavaScriptRuntime.Math);

        public static Type Reflect => typeof(JavaScriptRuntime.Reflect);

        public static Delegate RegExp => _regExpConstructorValue;

        /// <summary>
        /// ECMAScript global Error constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier and
        /// access <c>Error.prototype</c>.
        /// </summary>
        public static Func<object[], object?[], object?> Error => _errorConstructorValue;

        public static Func<object[], object?[], object?> EvalError => _evalErrorConstructorValue;

        public static Func<object[], object?[], object?> RangeError => _rangeErrorConstructorValue;

        public static Func<object[], object?[], object?> ReferenceError => _referenceErrorConstructorValue;

        public static Func<object[], object?[], object?> SyntaxError => _syntaxErrorConstructorValue;

        public static Func<object[], object?[], object?> TypeError => _typeErrorConstructorValue;

        public static Func<object[], object?[], object?> URIError => _uriErrorConstructorValue;

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

            var text = input is double inputDouble && inputDouble == 0.0
                ? "0"
                : DotNet2JSConversions.ToString(input);
            text = text.TrimStart();
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
                int digit = ch switch
                {
                    >= '0' and <= '9' => ch - '0',
                    >= 'a' and <= 'z' => ch - 'a' + 10,
                    >= 'A' and <= 'Z' => ch - 'A' + 10,
                    _ => -1
                };

                if (digit < 0 || digit >= radixValue)
                {
                    break;
                }

                value = (value * radixValue) + digit;
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

        /// <summary>
        /// Global isNaN implementation.
        /// </summary>
        public static bool isNaN(object? number)
        {
            return double.IsNaN(TypeUtilities.ToNumber(number));
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

        private static JsFuncNoScopes1 CreateCollectionConstructorValue(string name, Func<object?, object> factory)
        {
            return (newTarget, iterable) =>
            {
                if (newTarget is null)
                {
                    throw new TypeError($"Constructor {name} requires 'new'");
                }

                return factory(iterable);
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
            DefineSpeciesAccessorProperty(constructorValue);
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
        {
            ConfigureConstructorPrototypeSurface(constructorValue, prototypeValue);
            DefineSpeciesAccessorProperty(constructorValue);
        }

        private static void DefineSpeciesAccessorProperty(object constructorValue)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(_speciesGetterValue, 0d, "get [Symbol.species]");
            DefineUndefinedPrototypeProperty(_speciesGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, global::JavaScriptRuntime.Symbol.species.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _speciesGetterValue
            });
        }

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

        private static void ConfigureCollectionConstructorMetadata(object constructorValue, string name)
        {
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 0d
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = name
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

        private static bool TryGetThisSymbolValue([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JavaScriptRuntime.Symbol? symbol)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is JavaScriptRuntime.Symbol directSymbol)
            {
                symbol = directSymbol;
                return true;
            }

            if (thisValue != null
                && PropertyDescriptorStore.TryGetOwn(thisValue, JavaScriptRuntime.Object.PrimitiveValuePropertyName, out var descriptor)
                && descriptor.Value is JavaScriptRuntime.Symbol boxedSymbol)
            {
                symbol = boxedSymbol;
                return true;
            }

            symbol = null;
            return false;
        }

        internal static object ObjectPrototypeValue => _objectPrototypeValue;
        internal static object NumberPrototypeValue => _numberPrototypeValue;
        internal static object BooleanPrototypeValue => _booleanPrototypeValue;
        internal static object SymbolPrototypeValue => _symbolPrototypeValue;
        internal static object DatePrototypeValue => JavaScriptRuntime.Date.Prototype;
        internal static object ErrorPrototypeValue => _errorPrototypeValue;
        internal static object EvalErrorPrototypeValue => _evalErrorPrototypeValue;
        internal static object RangeErrorPrototypeValue => _rangeErrorPrototypeValue;
        internal static object ReferenceErrorPrototypeValue => _referenceErrorPrototypeValue;
        internal static object SyntaxErrorPrototypeValue => _syntaxErrorPrototypeValue;
        internal static object TypeErrorPrototypeValue => _typeErrorPrototypeValue;
        internal static object URIErrorPrototypeValue => _uriErrorPrototypeValue;
        internal static bool HasUndefinedPrototype(Delegate functionValue)
        {
            ArgumentNullException.ThrowIfNull(functionValue);

            return functionValue.Method == _arrayIsArrayValue.Method
                || functionValue.Method == _parseIntValue.Method
                || functionValue.Method == _isFiniteValue.Method
                || functionValue.Method == _isNaNValue.Method
                || functionValue.Method == _numberIsIntegerValue.Method
                || JavaScriptRuntime.Function.HasUndefinedPrototype(functionValue);
        }

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
                JavaScriptRuntime.EvalError => _evalErrorPrototypeValue,
                JavaScriptRuntime.RangeError => _rangeErrorPrototypeValue,
                JavaScriptRuntime.ReferenceError => _referenceErrorPrototypeValue,
                JavaScriptRuntime.SyntaxError => _syntaxErrorPrototypeValue,
                JavaScriptRuntime.TypeError => _typeErrorPrototypeValue,
                JavaScriptRuntime.URIError => _uriErrorPrototypeValue,
                _ => _errorPrototypeValue
            };

            PrototypeChain.SetPrototype(error, prototype);
        }

        internal static void ConfigureBuiltinFunctionObject(object functionValue)
        {
            JavaScriptRuntime.Function.ConfigureCallableObject(functionValue, hasRestrictedProperties: false);
        }

        private static object? SymbolCall(object[] scopes, object?[]? args)
        {
            var symbol = args != null && args.Length > 0
                ? (global::JavaScriptRuntime.Symbol)global::JavaScriptRuntime.Symbol.Call(args[0])
                : (global::JavaScriptRuntime.Symbol)global::JavaScriptRuntime.Symbol.Call();
            PrototypeChain.SetPrototype(symbol, _symbolPrototypeValue);
            return symbol;
        }
    }
}
