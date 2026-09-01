using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
    /// Minimal surface for jroc codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    [IntrinsicObject("GlobalThis")]
    public class GlobalThis : JsObject, IDictionary<string, object?>
    {
        private static readonly ThreadLocal<GlobalThis?> _fallbackGlobalObject = new(() => null);

        private static readonly JavaScriptRuntime.Console _defaultConsole = new(new ConsoleOutputSinks());
        private static readonly JavaScriptRuntime.Node.Process _defaultProcess = new(new DefaultEnvironment());

        private readonly RuntimeIntrinsics _intrinsics;
        private readonly object _seedGate = new();
        private volatile bool _seeded;
        private int _seedingThreadId;

        /// <summary>
        /// Cached bootstrap delegate: <see cref="Bootstrap"/> runs on every
        /// <c>globalThis</c> resolution, so the instance method group must not be
        /// converted to a new delegate each time.
        /// </summary>
        private Action? _initializeIntrinsics;

        /// <summary>
        /// The realm-owned intrinsic object graph backing this global object's
        /// well-known prototypes/namespace objects (ECMA-262 [[Intrinsics]]).
        /// </summary>
        internal RuntimeIntrinsics Intrinsics => _intrinsics;

        public GlobalThis() : this(RuntimeIntrinsics.Current)
        {
            Bootstrap();
        }

        internal GlobalThis(RuntimeIntrinsics intrinsics)
        {
            _intrinsics = intrinsics ?? throw new ArgumentNullException(nameof(intrinsics));
        }

        /// <summary>
        /// Wires this realm's intrinsic object graph and seeds the global object's
        /// well-known bindings. Idempotent and safe to call from any thread: the realm
        /// wiring runs exactly once per realm inside
        /// <see cref="RuntimeIntrinsics.EnsureBootstrapped"/> (concurrent callers block
        /// until it is complete), and the binding seed runs exactly once per instance.
        /// </summary>
        /// <remarks>
        /// Split out from the constructor so callers that publish the instance ambiently
        /// (<see cref="RuntimeExecutionContext"/>) can record the reference *before*
        /// running the wiring pass: bootstrap reenters
        /// <c>GlobalThis.globalThis</c>/<c>GetOrCreateGlobalObject()</c> (for example via
        /// <see cref="Function.MarkConstructible"/> creating an ordinary function
        /// prototype), and without publishing first that reentrant lookup would
        /// recursively construct another instance. Reentrant calls made while this thread
        /// is building the intrinsic graph return immediately with the graph under
        /// construction, which is also what keeps the bootstrap out of the runtime's lock
        /// order (no intrinsic initializer ever waits on a bootstrap gate).
        /// </remarks>
        internal void Bootstrap()
        {
            if (!_intrinsics.IsBootstrapped)
            {
                _intrinsics.EnsureBootstrapped(_initializeIntrinsics ??= InitializeIntrinsics);
            }

            SeedOnce();
        }

        private void SeedOnce()
        {
            if (_seeded)
            {
                return;
            }

            if (Volatile.Read(ref _seedingThreadId) == Environment.CurrentManagedThreadId
                || RuntimeIntrinsics.IsInitializingOnCurrentThread)
            {
                // Reentrant lookup from this instance's own seed pass, or from an
                // intrinsic initializer: the caller is part of the bootstrap and must
                // observe the object under construction instead of waiting for itself.
                return;
            }

            lock (_seedGate)
            {
                if (_seeded)
                {
                    return;
                }

                Volatile.Write(ref _seedingThreadId, Environment.CurrentManagedThreadId);
                try
                {
                    SeedGlobalObjectIfMissing();
                    _seeded = true;
                }
                finally
                {
                    Volatile.Write(ref _seedingThreadId, 0);
                }
            }
        }

        // Some ECMAScript globals are callable (e.g., Boolean(x)). When used in expression position
        // (e.g., arr.filter(Boolean)), we expose them as function values (delegates) so the compiler
        // can bind them as intrinsic globals.
        private static readonly Func<object[], object?, bool> _booleanFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToBoolean(value);

        private static readonly BuiltinFunction0 _booleanPrototypeToStringValue = static thisArgument =>
        {
            var booleanValue = JavaScriptRuntime.Boolean.ThisBooleanValue(thisArgument);
            return booleanValue ? "true" : "false";
        };

        private static readonly BuiltinFunction0 _booleanPrototypeValueOfValue = static thisArgument =>
            JavaScriptRuntime.Boolean.ThisBooleanValue(thisArgument);

        private static readonly Func<object[], object?, string> _stringFunctionValue = static (_, value) =>
            JavaScriptRuntime.DotNet2JSConversions.ToString(value);

        private static readonly Func<object[], object?, double> _numberFunctionValue = static (_, value) =>
            JavaScriptRuntime.Number.FromNumberConstructorArgument(value);
        private static readonly Func<object[], object?, object> _bigIntFunctionValue = static (_, value) =>
            global::JavaScriptRuntime.BigInt.Call(value);
        // Static; the receiver is ignored (issue #1895).
        private static readonly BuiltinFunction2 _bigIntAsIntNValue = static (_, bits, bigint) =>
            global::JavaScriptRuntime.BigInt.AsIntN(bits, bigint);
        // Static; the receiver is ignored (issue #1895).
        private static readonly BuiltinFunction2 _bigIntAsUintNValue = static (_, bits, bigint) =>
            global::JavaScriptRuntime.BigInt.AsUintN(bits, bigint);
        private static readonly BuiltinFunction1 _bigIntPrototypeToStringValue = static (thisArgument, radix) =>
            global::JavaScriptRuntime.BigInt.ToString(global::JavaScriptRuntime.BigInt.ThisBigIntValue(thisArgument), radix);
        private static readonly BuiltinFunction0 _bigIntPrototypeToLocaleStringValue = static thisArgument =>
            global::JavaScriptRuntime.BigInt.ToLocaleString(thisArgument);
        private static readonly BuiltinFunction0 _bigIntPrototypeValueOfValue = static thisArgument =>
            global::JavaScriptRuntime.BigInt.ThisBigIntValue(thisArgument);

        private static readonly BuiltinFunction1 _numberPrototypeToStringValue = static (thisArgument, radix) =>
            JavaScriptRuntime.Number.ToStringWithRadix(thisArgument, radix);

        private static readonly BuiltinFunction0 _numberPrototypeValueOfValue = static thisArgument =>
            JavaScriptRuntime.Number.ThisNumberValue(thisArgument);
        private static readonly BuiltinFunction1 _numberPrototypeToExponentialValue = static (thisArgument, fractionDigits) =>
            JavaScriptRuntime.Number.ToExponentialString(thisArgument, fractionDigits);
        private static readonly BuiltinFunction1 _numberPrototypeToFixedValue = static (thisArgument, fractionDigits) =>
            JavaScriptRuntime.Number.ToFixedString(thisArgument, fractionDigits);
        private static readonly BuiltinFunction0 _numberPrototypeToLocaleStringValue = static thisArgument =>
            JavaScriptRuntime.Number.ToLocaleStringString(thisArgument);
        private static readonly BuiltinFunction1 _numberPrototypeToPrecisionValue = static (thisArgument, precision) =>
            JavaScriptRuntime.Number.ToPrecisionString(thisArgument, precision);
        private static readonly Func<object[], object?, Delegate> _functionConstructorValue = static (_, __) =>
            throw new JavaScriptRuntime.Error("The Function constructor only supports compile-time string literal arguments in jroc.");

        private static readonly Func<object[], object?[], object?> _arrayConstructorValue =
            static (_, args) => JavaScriptRuntime.Array.Construct(args ?? System.Array.Empty<object?>());
        private static readonly Func<object?, bool> _arrayIsArrayValue = JavaScriptRuntime.Array.isArray;
        private static readonly BuiltinFunction3 _arrayFromValue = static (_, source, mapFn, thisArg) =>
            JavaScriptRuntime.Array.from(source, mapFn, thisArg);
        private static readonly Func<object?, object?, double> _parseIntValue = parseInt;
        private static readonly Func<object?, double> _parseFloatValue = parseFloat;
        private static readonly Func<object?, bool> _isFiniteValue = isFinite;
        private static readonly Func<object?, bool> _isNaNValue = isNaN;
        private static readonly Func<object?, string> _decodeURIValue = decodeURI;
        private static readonly Func<object?, string> _encodeURIValue = encodeURI;
        private static readonly Func<object?, string> _decodeURIComponentValue = decodeURIComponent;
        private static readonly Func<object?, string> _encodeURIComponentValue = encodeURIComponent;
        private static readonly UTF8Encoding _strictUtf8 = new(false, true);
        private static readonly Func<object?, bool> _numberIsFiniteValue = JavaScriptRuntime.Number.isFinite;
        private static readonly Func<object?, bool> _numberIsIntegerValue = JavaScriptRuntime.Number.isInteger;
        private static readonly Func<object?, bool> _numberIsNaNValue = JavaScriptRuntime.Number.isNaN;
        private static readonly Func<object?, bool> _numberIsSafeIntegerValue = JavaScriptRuntime.Number.isSafeInteger;

        private static readonly Delegate _mapConstructorValue =
            CreateCollectionConstructorValue("Map", static iterable => new JavaScriptRuntime.Map(iterable));
        private static readonly BuiltinFunction2 _mapGroupByValue = static (_, items, callback) =>
            JavaScriptRuntime.Map.groupBy(items, callback);

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

        private static readonly JsFuncNoScopes1 _weakRefConstructorValue = static (newTarget, target) =>
        {
            if (newTarget is null)
            {
                throw new TypeError("Constructor WeakRef requires 'new'");
            }

            return new JavaScriptRuntime.WeakRef(target);
        };

        private static readonly JsFuncNoScopes1 _finalizationRegistryConstructorValue = static (newTarget, cleanupCallback) =>
        {
            if (newTarget is null)
            {
                throw new TypeError("Constructor FinalizationRegistry requires 'new'");
            }

            return new JavaScriptRuntime.FinalizationRegistry(cleanupCallback);
        };

        private static readonly JsFuncNoScopes3 _dataViewConstructorValue = static (
            newTarget,
            buffer,
            byteOffset,
            byteLength) =>
        {
            if (newTarget is null)
            {
                throw new TypeError("Constructor DataView requires 'new'");
            }

            return new JavaScriptRuntime.DataView(buffer, byteOffset, byteLength);
        };

        private static readonly JsFuncNoScopes1 _promiseConstructorValue = static (newTarget, executor) =>
        {
            if (newTarget is null)
            {
                throw new global::JavaScriptRuntime.TypeError("Constructor Promise requires 'new'");
            }

            return new global::JavaScriptRuntime.Promise(executor);
        };
        private static readonly BuiltinFunction1 _promiseResolveValue = static (thisArgument, value) =>
            global::JavaScriptRuntime.Promise.ResolveForConstructor(thisArgument, value);
        private static readonly BuiltinFunction1 _promiseAllValue = static (thisArgument, iterable) =>
            global::JavaScriptRuntime.Promise.AllForConstructor(
                thisArgument,
                iterable);
        private static readonly BuiltinFunction1 _promiseRaceValue = static (_, iterable) =>
            global::JavaScriptRuntime.Promise.race(iterable);
        private static readonly BuiltinFunction1 _promiseRejectValue = static (_, reason) =>
            global::JavaScriptRuntime.Promise.reject(reason);
        private static readonly BuiltinFunctionVariadic _promiseTryValue = static (thisArgument, in arguments) =>
        {
            var callback = arguments.Count > 0 ? arguments.GetArgument(0) : null;
            var callbackArgs = arguments.Count > 1
                ? arguments.ToArray().Skip(1).ToArray()
                : global::System.Array.Empty<object?>();

            return global::JavaScriptRuntime.Promise.TryForConstructor(
                thisArgument,
                callback,
                callbackArgs);
        };
        private static readonly BuiltinFunction0 _speciesGetterValue = SpeciesGetter;

        private static readonly JsFuncNoScopes2 _proxyConstructorValue = static (newTarget, target, handler) =>
        {
            if (newTarget is null)
            {
                throw new global::JavaScriptRuntime.TypeError("Constructor Proxy requires 'new'");
            }

            return new global::JavaScriptRuntime.Proxy(target, handler);
        };

        private static readonly BuiltinFunction2 _proxyRevocableValue = static (_, target, handler) =>
            global::JavaScriptRuntime.Proxy.revocable(target, handler);

        // Object constructor/function value. This enables patterns like `Object.prototype` and
        // allows libraries to pass `Object` around as a value.
        private static readonly Func<object[], object?, object> _objectConstructorValue = static (_, value) =>
            ObjectRuntime.Construct(value);
        private static readonly Func<object[], object?[], object?> _regExpConstructorValue = static (_, args) =>
        {
            var pattern = (args != null && args.Length > 0) ? args[0] : null;
            var flags = (args != null && args.Length > 1) ? args[1] : null;
            return JavaScriptRuntime.RegExp.Call(pattern, flags);
        };
        private static readonly Func<object[], object?[], object?> _regExpEscapeValue = static (_, args) =>
            JavaScriptRuntime.RegExp.Escape(args != null && args.Length > 0 ? args[0] : null);

        private static readonly Func<object[], object?[], object?> _jsonStringifyValue = static (_, args) =>
        {
            var value = args != null && args.Length > 0 ? args[0] : null;
            var replacer = args != null && args.Length > 1 ? args[1] : null;
            var space = args != null && args.Length > 2 ? args[2] : null;
            return JavaScriptRuntime.JSON.Stringify(value, replacer, space);
        };
        private static readonly Func<object[], object?[], object?> _jsonParseValue = static (_, args) =>
        {
            var text = args != null && args.Length > 0 ? args[0] : null;
            var reviver = args != null && args.Length > 1 ? args[1] : null;
            return JavaScriptRuntime.JSON.Parse(text, reviver);
        };
        private static readonly Func<object[], object?[], object?> _jsonRawJsonValue = static (_, args) =>
            JavaScriptRuntime.JSON.RawJSON(args != null && args.Length > 0 ? args[0] : null);
        private static readonly Func<object[], object?[], object?> _jsonIsRawJsonValue = static (_, args) =>
            JavaScriptRuntime.JSON.IsRawJSON(args != null && args.Length > 0 ? args[0] : null);

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

        private static readonly Func<object[], object?[], object?> _aggregateErrorConstructorValue = static (_, args) =>
            JavaScriptRuntime.AggregateError.Construct(args ?? System.Array.Empty<object?>());

        private static readonly Func<object[], object?[], object?> _suppressedErrorConstructorValue = static (_, args) =>
            JavaScriptRuntime.SuppressedError.Construct(args ?? System.Array.Empty<object?>());

        private static readonly Func<object[], object?[], object?> _iteratorConstructorValue = static (_, __) =>
            throw new TypeError("Iterator is not directly constructible in jroc.");

        private static readonly Func<object[], object?[], object?> _asyncIteratorConstructorValue = static (_, __) =>
            throw new TypeError("AsyncIterator is not directly constructible in jroc.");

        private static readonly BuiltinFunction1 _errorIsErrorValue = static (_, arg) =>
            arg is JavaScriptRuntime.Error;

        // Minimal Error.prototype object. Libraries may attach properties here.
        // Realm-owned: see RuntimeIntrinsics.
        private object _errorPrototypeValue => _intrinsics.ErrorPrototype;
        private object _evalErrorPrototypeValue => _intrinsics.EvalErrorPrototype;
        private object _rangeErrorPrototypeValue => _intrinsics.RangeErrorPrototype;
        private object _referenceErrorPrototypeValue => _intrinsics.ReferenceErrorPrototype;
        private object _syntaxErrorPrototypeValue => _intrinsics.SyntaxErrorPrototype;
        private object _typeErrorPrototypeValue => _intrinsics.TypeErrorPrototype;
        private object _uriErrorPrototypeValue => _intrinsics.URIErrorPrototype;
        private object _aggregateErrorPrototypeValue => _intrinsics.AggregateErrorPrototype;
        private object _suppressedErrorPrototypeValue => _intrinsics.SuppressedErrorPrototype;

        // Realm-owned Object.prototype (issue #1824). Every other intrinsic prototype
        // chains up to it, so it must be created per realm together with them.
        private object _objectPrototypeValue => _intrinsics.ObjectPrototype;
        private object _jsonValue => _intrinsics.Json;
        private object _intlValue => _intrinsics.Intl;
        private object _atomicsValue => _intrinsics.Atomics;
        private object _numberPrototypeValue => _intrinsics.NumberPrototype;
        private object _booleanPrototypeValue => _intrinsics.BooleanPrototype;
        private object _bigIntPrototypeValue => _intrinsics.BigIntPrototype;
        private object _symbolPrototypeValue => _intrinsics.SymbolPrototype;
        private object _promisePrototypeValue => _intrinsics.GlobalPromisePrototype;
        // Static; the receiver is ignored (issue #1895).
        private readonly BuiltinFunction1 _symbolFunctionValue = SymbolCall;
        private readonly BuiltinFunction0 _symbolPrototypeDescriptionGetterValue = SymbolPrototypeDescription;
        private readonly BuiltinFunction0 _symbolPrototypeToPrimitiveValue = SymbolPrototypeToPrimitive;

        // TypedArray intrinsic constructor and prototype
        private static readonly Func<object[], object?[], object?> _typedArrayConstructorValue = static (_, __) =>
            throw new TypeError("%TypedArray% is not directly constructible in jroc.");
        // Realm-owned: see RuntimeIntrinsics.
        private object _typedArrayPrototypeValue => _intrinsics.TypedArrayPrototype;
        private object _float64ArrayPrototypeValue => _intrinsics.Float64ArrayPrototype;
        private object _float32ArrayPrototypeValue => _intrinsics.Float32ArrayPrototype;
        private object _int32ArrayPrototypeValue => _intrinsics.Int32ArrayPrototype;
        private object _int16ArrayPrototypeValue => _intrinsics.Int16ArrayPrototype;
        private object _int8ArrayPrototypeValue => _intrinsics.Int8ArrayPrototype;
        private object _uint32ArrayPrototypeValue => _intrinsics.Uint32ArrayPrototype;
        private object _uint16ArrayPrototypeValue => _intrinsics.Uint16ArrayPrototype;
        private static readonly BuiltinFunctionVariadic _typedArraySortValue = TypedArrayPrototypeSort;
        private static readonly BuiltinFunctionVariadic _typedArrayToSortedValue = TypedArrayPrototypeToSorted;
        private static readonly BuiltinFunctionVariadic _typedArrayWithValue = TypedArrayPrototypeWith;
        private static readonly BuiltinFunction1 _typedArrayAtValue = TypedArrayPrototypeAt;
        private static readonly BuiltinFunction0 _typedArrayLengthGetterValue = TypedArrayPrototypeLength;
        private static readonly BuiltinFunction0 _typedArrayBufferGetterValue = TypedArrayPrototypeBuffer;
        private static readonly BuiltinFunction0 _typedArrayByteOffsetGetterValue = TypedArrayPrototypeByteOffset;
        private static readonly BuiltinFunction0 _typedArrayByteLengthGetterValue = TypedArrayPrototypeByteLength;
        private static readonly BuiltinFunction0 _typedArrayToStringTagGetterValue = TypedArrayPrototypeToStringTag;
        private static readonly BuiltinFunction0 _typedArrayToStringValue = TypedArrayPrototypeToString;
        private static readonly BuiltinFunction0 _typedArrayToLocaleStringValue = TypedArrayPrototypeToLocaleString;
        private static readonly BuiltinFunctionVariadic _typedArrayFindLastValue = TypedArrayPrototypeFindLast;
        private static readonly BuiltinFunctionVariadic _typedArrayFindLastIndexValue = TypedArrayPrototypeFindLastIndex;
        private static readonly BuiltinFunctionVariadic _typedArrayCopyWithinValue = TypedArrayPrototypeCopyWithin;
        private static readonly BuiltinFunctionVariadic _typedArrayReduceRightValue = TypedArrayPrototypeReduceRight;
        private static readonly BuiltinFunction0 _typedArrayToReversedValue = TypedArrayPrototypeToReversed;
        private static readonly BuiltinFunction0 _typedArrayEntriesValue = TypedArrayPrototypeEntries;
        private static readonly BuiltinFunction0 _typedArrayKeysValue = TypedArrayPrototypeKeys;
        private static readonly BuiltinFunction0 _typedArrayValuesValue = TypedArrayPrototypeValues;
        private static readonly BuiltinFunction3 _typedArrayFromValue = TypedArrayFrom;
        private static readonly BuiltinFunctionVariadic _typedArrayOfValue = TypedArrayOf;

        // Typed array constructor values - supported and unsupported
        private static readonly Func<object[], object?[], object?> _float64ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Float64Array(), static a => new Float64Array(a), static (a, b) => new Float64Array(a, b), static (a, b, c) => new Float64Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _float32ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Float32Array(), static a => new Float32Array(a), static (a, b) => new Float32Array(a, b), static (a, b, c) => new Float32Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _int32ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Int32Array(), static a => new Int32Array(a), static (a, b) => new Int32Array(a, b), static (a, b, c) => new Int32Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _arrayBufferConstructorValue =
            static (_, args) => args != null && args.Length > 1
                ? new ArrayBuffer(args[0], args[1])
                : new ArrayBuffer(args != null && args.Length > 0 ? args[0] : null);
        private static readonly Func<object[], object?, bool> _arrayBufferIsViewValue =
            static (_, value) => JavaScriptRuntime.ArrayBuffer.isView(value);
        private static readonly BuiltinFunction2 _arrayBufferPrototypeSliceValue = static (thisArgument, start, end) =>
        {
            if (thisArgument is not JavaScriptRuntime.ArrayBuffer buffer || thisArgument is JavaScriptRuntime.SharedArrayBuffer)
            {
                throw new TypeError("ArrayBuffer.prototype.slice called on incompatible receiver");
            }

            return buffer.slice(start, end);
        };
        private static readonly BuiltinFunction1 _arrayBufferPrototypeResizeValue = static (thisArgument, newLength) =>
        {
            if (thisArgument is not JavaScriptRuntime.ArrayBuffer buffer || thisArgument is JavaScriptRuntime.SharedArrayBuffer)
            {
                throw new TypeError("ArrayBuffer.prototype.resize called on incompatible receiver");
            }

            return buffer.resize(newLength);
        };
        private static readonly BuiltinFunction1 _arrayBufferPrototypeTransferValue = static (thisArgument, newLength) =>
        {
            if (thisArgument is not JavaScriptRuntime.ArrayBuffer buffer || thisArgument is JavaScriptRuntime.SharedArrayBuffer)
            {
                throw new TypeError("ArrayBuffer.prototype.transfer called on incompatible receiver");
            }

            return buffer.transfer(newLength);
        };
        private static readonly BuiltinFunction1 _arrayBufferPrototypeTransferToFixedLengthValue = static (thisArgument, newLength) =>
        {
            if (thisArgument is not JavaScriptRuntime.ArrayBuffer buffer || thisArgument is JavaScriptRuntime.SharedArrayBuffer)
            {
                throw new TypeError("ArrayBuffer.prototype.transferToFixedLength called on incompatible receiver");
            }

            return buffer.transferToFixedLength(newLength);
        };
        private static readonly Func<object[], object?[], object?> _sharedArrayBufferConstructorValue =
            static (_, args) => new SharedArrayBuffer(args != null && args.Length > 0 ? args[0] : null);
        private static readonly BuiltinFunction2 _sharedArrayBufferPrototypeSliceValue = static (thisArgument, start, end) =>
        {
            if (thisArgument is not JavaScriptRuntime.SharedArrayBuffer buffer)
            {
                throw new TypeError("SharedArrayBuffer.prototype.slice called on incompatible receiver");
            }

            return buffer.slice(start, end);
        };
        private static readonly Func<object[], object?[], object?> _int16ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Int16Array(), static a => new Int16Array(a), static (a, b) => new Int16Array(a, b), static (a, b, c) => new Int16Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _int8ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Int8Array(), static a => new Int8Array(a), static (a, b) => new Int8Array(a, b), static (a, b, c) => new Int8Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _uint32ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Uint32Array(), static a => new Uint32Array(a), static (a, b) => new Uint32Array(a, b), static (a, b, c) => new Uint32Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _uint16ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Uint16Array(), static a => new Uint16Array(a), static (a, b) => new Uint16Array(a, b), static (a, b, c) => new Uint16Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _uint8ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Uint8Array(), static a => new Uint8Array(a), static (a, b) => new Uint8Array(a, b), static (a, b, c) => new Uint8Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _uint8ClampedArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new Uint8ClampedArray(), static a => new Uint8ClampedArray(a), static (a, b) => new Uint8ClampedArray(a, b), static (a, b, c) => new Uint8ClampedArray(a, b, c));
        private static readonly Func<object[], object?[], object?> _bigInt64ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new BigInt64Array(), static a => new BigInt64Array(a), static (a, b) => new BigInt64Array(a, b), static (a, b, c) => new BigInt64Array(a, b, c));
        private static readonly Func<object[], object?[], object?> _bigUint64ArrayConstructorValue =
            static (_, args) => ConstructTypedArray(args, static () => new BigUint64Array(), static a => new BigUint64Array(a), static (a, b) => new BigUint64Array(a, b), static (a, b, c) => new BigUint64Array(a, b, c));

        private void InitializeIntrinsics()
        {
            // Runs once per realm inside the realm-bootstrap slot (issue #1824): every
            // intrinsic object is realm-owned, and the slot protocol both serializes
            // concurrent bootstraps of the same realm and keeps this pass and the lazy
            // slot creation it triggers inside a single lock order. Bootstrap runs once
            // per realm and never on a script's hot path.

            // Every realm establishes the intrinsic descriptor baseline for its own
            // intrinsic graph: the objects being written are this realm's objects and
            // the descriptor baseline for non-JsObject targets is realm-owned too
            // (PropertyDescriptorStore._intrinsicStore).
            using var intrinsicBaselineScope =
                PropertyDescriptorStore.BeginIntrinsicInitialization();

            InitializeIntrinsicsCore();
        }

        private void InitializeIntrinsicsCore()
        {
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
            DefineIntrinsicToStringTagProperty(_jsonValue, "JSON");
            DefineIntrinsicToStringTagProperty(Reflect, "Reflect");
            DefineIntrinsicToStringTagProperty(_intlValue, "Intl");
            DefineIntrinsicDataProperty(_intlValue, "NumberFormat", typeof(JavaScriptRuntime.IntlNumberFormat));
            DefineIntrinsicDataProperty(_intlValue, "Segmenter", typeof(JavaScriptRuntime.IntlSegmenter));

            // Attach minimal prototypes to callable globals so patterns like
            // `Function.prototype.apply.bind(Array.prototype.push)` work even when code only
            // references GlobalThis static properties and never touches the globalThis object.
            ConfigureBuiltinFunctionObject(_functionConstructorValue);
            JavaScriptRuntime.Function.MarkConstructible(
                _functionConstructorValue);
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
            JavaScriptRuntime.Function.MarkConstructible(
                _arrayConstructorValue);
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
            JavaScriptRuntime.Function.MarkConstructible(
                _proxyConstructorValue);
            JavaScriptRuntime.Function.MarkUndefinedPrototype(
                _proxyConstructorValue);
            DefineBuiltinFunctionProperty(_proxyConstructorValue, "revocable", _proxyRevocableValue, 2d);
            ConfigureCollectionIntrinsicSurface(_mapConstructorValue, JavaScriptRuntime.Map.Prototype);
            ConfigureCollectionIntrinsicSurface(_setConstructorValue, JavaScriptRuntime.Set.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakMapConstructorValue, JavaScriptRuntime.WeakMap.Prototype);
            ConfigureCollectionIntrinsicSurface(_weakSetConstructorValue, JavaScriptRuntime.WeakSet.Prototype);
            ConfigureWeakRefIntrinsicSurface();
            ConfigureFinalizationRegistryIntrinsicSurface();
            ConfigureCollectionConstructorMetadata(_mapConstructorValue, "Map");
            ConfigureCollectionConstructorMetadata(_setConstructorValue, "Set");
            ConfigureCollectionConstructorMetadata(_weakMapConstructorValue, "WeakMap");
            ConfigureCollectionConstructorMetadata(_weakSetConstructorValue, "WeakSet");
            DefineBuiltinFunctionProperty(_mapConstructorValue, "groupBy", _mapGroupByValue, 2d);
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
            JavaScriptRuntime.Function.MarkConstructible(
                _booleanFunctionValue);
            ConfigureBuiltinFunctionObject(_symbolFunctionValue);
            // The "description" parameter is optional (Symbol ( [ description ] )), so the
            // spec-mandated length is 0. BuiltinFunction1's automatic length inference always
            // reports 1 (one JS-visible parameter), so it must be overridden explicitly here to
            // preserve the pre-migration length value that the legacy array-based ABI computed.
            JavaScriptRuntime.Function.DefineMetadataProperty(_symbolFunctionValue, "length", 0d);
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

            // Centralized Object constructor/prototype wiring lives on ObjectRuntime.
            ConfigureBuiltinFunctionObject(_objectConstructorValue);
            JavaScriptRuntime.Function.MarkConstructible(
                _objectConstructorValue);
            ObjectRuntime.ConfigureIntrinsicSurface(_objectConstructorValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_objectPrototypeValue, JsNull.Null);
            PrototypeChain.SetPrototype(Math, _objectPrototypeValue);
            PrototypeChain.SetPrototype(JavaScriptRuntime.Array.ImmutablePrototype, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_jsonValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_atomicsValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_numberPrototypeValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_booleanPrototypeValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_bigIntPrototypeValue, _objectPrototypeValue);
            PrototypeChain.SetPrototype(_symbolPrototypeValue, _objectPrototypeValue);
            PropertyDescriptorStore.DefineOrUpdate(_numberPrototypeValue, JavaScriptRuntime.Number.NumberDataPropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = true,
                Value = 0d
            });
            PropertyDescriptorStore.DefineOrUpdate(_booleanPrototypeValue, ObjectRuntime.PrimitiveValuePropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = false
            });
            // Unlike Number.prototype/Boolean.prototype, the BigInt prototype object is
            // *not* a BigInt object and must not carry a [[BigIntData]] internal slot
            // (sec-properties-of-the-bigint-prototype-object), so no PrimitiveValue data
            // property is defined here; ThisBigIntValue(BigInt.prototype) must throw.
            DefineBuiltinFunctionProperty(_jsonValue, "parse", _jsonParseValue, 2d);
            DefineBuiltinFunctionProperty(_jsonValue, "rawJSON", _jsonRawJsonValue, 1d);
            DefineBuiltinFunctionProperty(_jsonValue, "isRawJSON", _jsonIsRawJsonValue, 1d);
            PropertyDescriptorStore.Delete(_jsonParseValue, "prototype");
            PropertyDescriptorStore.Delete(_jsonRawJsonValue, "prototype");
            PropertyDescriptorStore.Delete(_jsonIsRawJsonValue, "prototype");
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
            DefineBuiltinFunctionProperty(_regExpConstructorValue, "escape", _regExpEscapeValue, 1d);
            ConfigureBuiltinFunctionObject(_numberFunctionValue);
            JavaScriptRuntime.Function.MarkConstructible(
                _numberFunctionValue);
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
            ConfigureBuiltinFunctionObject(_numberIsSafeIntegerValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isFinite", _numberIsFiniteValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isInteger", _numberIsIntegerValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isNaN", _numberIsNaNValue);
            DefineIntrinsicDataProperty(_numberFunctionValue, "isSafeInteger", _numberIsSafeIntegerValue);
            DefineUndefinedPrototypeProperty(_numberIsFiniteValue);
            DefineUndefinedPrototypeProperty(_numberIsIntegerValue);
            DefineUndefinedPrototypeProperty(_numberIsNaNValue);
            DefineUndefinedPrototypeProperty(_numberIsSafeIntegerValue);
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
            JavaScriptRuntime.Function.MarkConstructible(
                _bigIntFunctionValue);
            PropertyDescriptorStore.DefineOrUpdate(_bigIntFunctionValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _bigIntPrototypeValue
            });
            DefineBuiltinFunctionProperty(_bigIntFunctionValue, "asIntN", _bigIntAsIntNValue, 2d);
            DefineBuiltinFunctionProperty(_bigIntFunctionValue, "asUintN", _bigIntAsUintNValue, 2d);
            DefineIntrinsicDataProperty(_bigIntPrototypeValue, "constructor", _bigIntFunctionValue);
            DefineBuiltinFunctionProperty(_bigIntPrototypeValue, "toLocaleString", _bigIntPrototypeToLocaleStringValue, 0d);
            // BigInt.prototype.toString ( [ radix ] ): radix is an optional parameter,
            // so per the built-in function length convention its "length" is 0.
            DefineBuiltinFunctionProperty(_bigIntPrototypeValue, "toString", _bigIntPrototypeToStringValue, 0d);
            DefineBuiltinFunctionProperty(_bigIntPrototypeValue, "valueOf", _bigIntPrototypeValueOfValue, 0d);
            DefineIntrinsicToStringTagProperty(_bigIntPrototypeValue, "BigInt");
            JavaScriptRuntime.Date.InitializeIntrinsicSurface(_objectPrototypeValue);
            JavaScriptRuntime.AbortController.InitializeIntrinsicSurface(_objectPrototypeValue);
            JavaScriptRuntime.AbortSignal.InitializeIntrinsicSurface(_objectPrototypeValue);
            ConfigureBuiltinFunctionObject(_stringFunctionValue);
            ConfigureBuiltinFunctionObject(_booleanFunctionValue);
            ConfigureBuiltinFunctionObject(_parseIntValue);
            ConfigureBuiltinFunctionObject(_parseFloatValue);
            ConfigureBuiltinFunctionObject(_isFiniteValue);
            ConfigureBuiltinFunctionObject(_isNaNValue);
            ConfigureBuiltinFunctionObject(_decodeURIValue);
            ConfigureBuiltinFunctionObject(_encodeURIValue);
            ConfigureBuiltinFunctionObject(_decodeURIComponentValue);
            ConfigureBuiltinFunctionObject(_encodeURIComponentValue);
            // ECMAScript: parseInt/parseFloat are ordinary (non-constructor) built-in functions
            // and therefore must not expose an own "prototype" property (sec-parseint-string-radix).
            DefineUndefinedPrototypeProperty(_isFiniteValue);
            DefineUndefinedPrototypeProperty(_isNaNValue);
            DefineUndefinedPrototypeProperty(_decodeURIValue);
            DefineUndefinedPrototypeProperty(_encodeURIValue);
            DefineUndefinedPrototypeProperty(_decodeURIComponentValue);
            DefineUndefinedPrototypeProperty(_encodeURIComponentValue);

            // Provide Error.prototype for patterns like `Error.prototype` and error-subclassing libraries.
            ConfigureErrorIntrinsicSurface(_errorConstructorValue, _errorPrototypeValue, "Error", parentPrototype: _objectPrototypeValue);
            ConfigureErrorIntrinsicSurface(_evalErrorConstructorValue, _evalErrorPrototypeValue, "EvalError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_rangeErrorConstructorValue, _rangeErrorPrototypeValue, "RangeError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_referenceErrorConstructorValue, _referenceErrorPrototypeValue, "ReferenceError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_syntaxErrorConstructorValue, _syntaxErrorPrototypeValue, "SyntaxError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_typeErrorConstructorValue, _typeErrorPrototypeValue, "TypeError", parentPrototype: _errorPrototypeValue);
            ConfigureErrorIntrinsicSurface(_uriErrorConstructorValue, _uriErrorPrototypeValue, "URIError", parentPrototype: _errorPrototypeValue);
            PrototypeChain.SetPrototype(_evalErrorConstructorValue, _errorConstructorValue);
            PrototypeChain.SetPrototype(_rangeErrorConstructorValue, _errorConstructorValue);
            PrototypeChain.SetPrototype(_referenceErrorConstructorValue, _errorConstructorValue);
            PrototypeChain.SetPrototype(_syntaxErrorConstructorValue, _errorConstructorValue);
            PrototypeChain.SetPrototype(_typeErrorConstructorValue, _errorConstructorValue);
            PrototypeChain.SetPrototype(_uriErrorConstructorValue, _errorConstructorValue);
            ConfigureAggregateErrorIntrinsicSurface();
            ConfigureSuppressedErrorIntrinsicSurface();

            PropertyDescriptorStore.DefineOrUpdate(_booleanPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _booleanFunctionValue
            });
            DefineBuiltinFunctionProperty(_booleanPrototypeValue, "toString", _booleanPrototypeToStringValue, 0d);
            DefineBuiltinFunctionProperty(_booleanPrototypeValue, "valueOf", _booleanPrototypeValueOfValue, 0d);
            DefineIntrinsicDataProperty(_booleanPrototypeValue, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, "Boolean");

            PropertyDescriptorStore.DefineOrUpdate(_symbolPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _symbolFunctionValue
            });
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _symbolPrototypeDescriptionGetterValue,
                0d,
                "get description",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_symbolPrototypeDescriptionGetterValue));
            DefineUndefinedPrototypeProperty(_symbolPrototypeDescriptionGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(_symbolPrototypeValue, "description", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _symbolPrototypeDescriptionGetterValue
            });
            DefineBuiltinFunctionProperty(
                _symbolPrototypeValue,
                "toString",
                (BuiltinFunction0)(thisArgument =>
                    TryGetThisSymbolValue(thisArgument, out var symbol)
                        ? symbol.toString()
                        : throw new TypeError("Symbol.prototype.toString called on incompatible receiver")),
                0d);
            DefineBuiltinFunctionProperty(
                _symbolPrototypeValue,
                "valueOf",
                (BuiltinFunction0)(thisArgument =>
                    TryGetThisSymbolValue(thisArgument, out var symbol)
                        ? symbol.valueOf()
                        : throw new TypeError("Symbol.prototype.valueOf called on incompatible receiver")),
                0d);
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _symbolPrototypeToPrimitiveValue,
                1d,
                "[Symbol.toPrimitive]",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_symbolPrototypeToPrimitiveValue));
            DefineUndefinedPrototypeProperty(_symbolPrototypeToPrimitiveValue);
            PropertyDescriptorStore.DefineOrUpdate(
                _symbolPrototypeValue,
                global::JavaScriptRuntime.Symbol.toPrimitive.DebugId,
                new JsPropertyDescriptor
                {
                        Kind = JsPropertyDescriptorKind.Data,
                        Enumerable = false,
                        Configurable = true,
                        Writable = false,
                        Value = _symbolPrototypeToPrimitiveValue
                });
            DefineIntrinsicToStringTagProperty(_symbolPrototypeValue, "Symbol");
            DefineIntrinsicDataProperty(_symbolFunctionValue, "for", (Func<object?, object>)global::JavaScriptRuntime.Symbol.@for);
            DefineIntrinsicDataProperty(_symbolFunctionValue, "keyFor", (Func<object?, object?>)global::JavaScriptRuntime.Symbol.keyFor);
            DefineWellKnownSymbolProperty("iterator", global::JavaScriptRuntime.Symbol.iterator);
            DefineWellKnownSymbolProperty("asyncIterator", global::JavaScriptRuntime.Symbol.asyncIterator);
            DefineWellKnownSymbolProperty("hasInstance", global::JavaScriptRuntime.Symbol.hasInstance);
            DefineWellKnownSymbolProperty("isConcatSpreadable", global::JavaScriptRuntime.Symbol.isConcatSpreadable);
            DefineWellKnownSymbolProperty("match", global::JavaScriptRuntime.Symbol.match);
            DefineWellKnownSymbolProperty("matchAll", global::JavaScriptRuntime.Symbol.matchAll);
            DefineWellKnownSymbolProperty("replace", global::JavaScriptRuntime.Symbol.replace);
            DefineWellKnownSymbolProperty("search", global::JavaScriptRuntime.Symbol.search);
            DefineWellKnownSymbolProperty("species", global::JavaScriptRuntime.Symbol.species);
            DefineWellKnownSymbolProperty("split", global::JavaScriptRuntime.Symbol.split);
            DefineWellKnownSymbolProperty("toPrimitive", global::JavaScriptRuntime.Symbol.toPrimitive);
            DefineWellKnownSymbolProperty("toStringTag", global::JavaScriptRuntime.Symbol.toStringTag);
            DefineWellKnownSymbolProperty("unscopables", global::JavaScriptRuntime.Symbol.unscopables);
            DefineWellKnownSymbolProperty("dispose", global::JavaScriptRuntime.Symbol.dispose);
            DefineWellKnownSymbolProperty("asyncDispose", global::JavaScriptRuntime.Symbol.asyncDispose);

            DefineBuiltinFunctionProperty(_errorConstructorValue, "isError", _errorIsErrorValue, 1d);

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
            DefineBuiltinFunctionProperty(_errorPrototypeValue, "toString", (BuiltinFunction0)ErrorPrototypeToString, 0d);

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
            DefineSpeciesAccessorProperty(_typedArrayConstructorValue);
            DefineBuiltinFunctionProperty(_typedArrayConstructorValue, "from", _typedArrayFromValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayConstructorValue, "of", _typedArrayOfValue, 0d);
            PropertyDescriptorStore.DefineOrUpdate(_typedArrayPrototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _typedArrayConstructorValue
            });
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _typedArrayLengthGetterValue,
                0d,
                "get length",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_typedArrayLengthGetterValue));
            PropertyDescriptorStore.DefineOrUpdate(_typedArrayPrototypeValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _typedArrayLengthGetterValue
            });
            DefineTypedArrayAccessor("buffer", _typedArrayBufferGetterValue);
            DefineTypedArrayAccessor("byteOffset", _typedArrayByteOffsetGetterValue);
            DefineTypedArrayAccessor("byteLength", _typedArrayByteLengthGetterValue);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "at", _typedArrayAtValue, 1d);
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _typedArrayToStringTagGetterValue,
                0d,
                "get [Symbol.toStringTag]",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_typedArrayToStringTagGetterValue));
            DefineUndefinedPrototypeProperty(_typedArrayToStringTagGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(
                _typedArrayPrototypeValue,
                global::JavaScriptRuntime.Symbol.toStringTag.DebugId,
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = false,
                    Configurable = true,
                    Get = _typedArrayToStringTagGetterValue
                });
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "sort", _typedArraySortValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "toSorted", _typedArrayToSortedValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "with", _typedArrayWithValue, 2d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "toString", _typedArrayToStringValue, 0d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "toLocaleString", _typedArrayToLocaleStringValue, 0d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "copyWithin", _typedArrayCopyWithinValue, 2d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "findLast", _typedArrayFindLastValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "findLastIndex", _typedArrayFindLastIndexValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "reduceRight", _typedArrayReduceRightValue, 1d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "toReversed", _typedArrayToReversedValue, 0d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "entries", _typedArrayEntriesValue, 0d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "keys", _typedArrayKeysValue, 0d);
            DefineBuiltinFunctionProperty(_typedArrayPrototypeValue, "values", _typedArrayValuesValue, 0d);
            PropertyDescriptorStore.DefineOrUpdate(
                _typedArrayPrototypeValue,
                global::JavaScriptRuntime.Symbol.iterator.DebugId,
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = true,
                    Value = _typedArrayValuesValue
                });
            ConfigureTypedArrayConstructorValue(_float64ArrayConstructorValue, 8d);
            ConfigureTypedArrayConstructorValue(_float32ArrayConstructorValue, 4d);
            ConfigureTypedArrayConstructorValue(_int32ArrayConstructorValue, 4d);
            ConfigureTypedArrayConstructorValue(_int16ArrayConstructorValue, 2d);
            ConfigureTypedArrayConstructorValue(_int8ArrayConstructorValue, 1d);
            ConfigureTypedArrayConstructorValue(_uint32ArrayConstructorValue, 4d);
            ConfigureTypedArrayConstructorValue(_uint16ArrayConstructorValue, 2d);
            ConfigureTypedArrayConstructorValue(_uint8ArrayConstructorValue, 1d);
            ConfigureTypedArrayConstructorValue(_uint8ClampedArrayConstructorValue, 1d);
            ConfigureTypedArrayConstructorValue(_bigInt64ArrayConstructorValue, 8d);
            ConfigureTypedArrayConstructorValue(_bigUint64ArrayConstructorValue, 8d);
            ConfigureTypedArrayInstancePrototype(_uint8ArrayConstructorValue, JavaScriptRuntime.Uint8Array.Prototype);
            ConfigureTypedArrayInstancePrototype(_uint8ClampedArrayConstructorValue, JavaScriptRuntime.Uint8ClampedArray.Prototype);
            ConfigureTypedArrayInstancePrototype(_float64ArrayConstructorValue, _float64ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_float32ArrayConstructorValue, _float32ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_int32ArrayConstructorValue, _int32ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_int16ArrayConstructorValue, _int16ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_int8ArrayConstructorValue, _int8ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_uint32ArrayConstructorValue, _uint32ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_uint16ArrayConstructorValue, _uint16ArrayPrototypeValue);
            ConfigureTypedArrayInstancePrototype(_bigInt64ArrayConstructorValue, JavaScriptRuntime.BigInt64Array.Prototype);
            ConfigureTypedArrayInstancePrototype(_bigUint64ArrayConstructorValue, JavaScriptRuntime.BigUint64Array.Prototype);
            JavaScriptRuntime.Uint8Array.ConfigureIntrinsicSurface(_uint8ArrayConstructorValue);
            ConfigureArrayBufferIntrinsicSurface();
            ConfigureSharedArrayBufferIntrinsicSurface();
            ConfigureDataViewIntrinsicSurface();

            JavaScriptRuntime.String.ConfigureIntrinsicSurface(_stringFunctionValue);
        }

        private static object? ErrorPrototypeToString(object? thisArgument)
        {
            var thisVal = thisArgument;
            if (TypeUtilities.IsPrimitive(thisVal))
            {
                throw new TypeError("Error.prototype.toString called on incompatible receiver");
            }

            var nameValue = JavaScriptRuntime.ObjectRuntime.GetItem(thisVal!, "name");
            var messageValue = JavaScriptRuntime.ObjectRuntime.GetItem(thisVal!, "message");

            var name = nameValue is null
                ? "Error"
                : DotNet2JSConversions.ToStringRejectingSymbols(nameValue);
            var message = messageValue is null
                ? string.Empty
                : DotNet2JSConversions.ToStringRejectingSymbols(messageValue);

            if (string.IsNullOrEmpty(name)) return message;
            if (string.IsNullOrEmpty(message)) return name;
            return $"{name}: {message}";
        }

        private static void ConfigureTypedArrayConstructorValue(object constructorValue, double bytesPerElement)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                constructorValue,
                3d,
                GetTypedArrayConstructorName(constructorValue));
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
            PrototypeChain.SetPrototype(constructorValue, _typedArrayConstructorValue);
            DefineIntrinsicConstantDataProperty(constructorValue, "BYTES_PER_ELEMENT", bytesPerElement);
        }

        private static string GetTypedArrayConstructorName(object constructorValue)
        {
            if (ReferenceEquals(constructorValue, _float64ArrayConstructorValue)) return nameof(Float64Array);
            if (ReferenceEquals(constructorValue, _float32ArrayConstructorValue)) return nameof(Float32Array);
            if (ReferenceEquals(constructorValue, _int32ArrayConstructorValue)) return nameof(Int32Array);
            if (ReferenceEquals(constructorValue, _int16ArrayConstructorValue)) return nameof(Int16Array);
            if (ReferenceEquals(constructorValue, _int8ArrayConstructorValue)) return nameof(Int8Array);
            if (ReferenceEquals(constructorValue, _uint32ArrayConstructorValue)) return nameof(Uint32Array);
            if (ReferenceEquals(constructorValue, _uint16ArrayConstructorValue)) return nameof(Uint16Array);
            if (ReferenceEquals(constructorValue, _uint8ArrayConstructorValue)) return nameof(Uint8Array);
            if (ReferenceEquals(constructorValue, _uint8ClampedArrayConstructorValue)) return nameof(Uint8ClampedArray);
            if (ReferenceEquals(constructorValue, _bigInt64ArrayConstructorValue)) return nameof(BigInt64Array);
            if (ReferenceEquals(constructorValue, _bigUint64ArrayConstructorValue)) return nameof(BigUint64Array);
            throw new ArgumentOutOfRangeException(nameof(constructorValue));
        }

        private void ConfigureTypedArrayInstancePrototype(object constructorValue, object prototypeValue)
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
            if (PropertyDescriptorStore.TryGetOwn(constructorValue, "BYTES_PER_ELEMENT", out var bytesPerElement))
            {
                PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "BYTES_PER_ELEMENT", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = false,
                    Writable = false,
                    Value = bytesPerElement.Value
                });
            }
        }

        private void DefineTypedArrayAccessor(
            string name,
            BuiltinFunction0 getter)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {name}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(_typedArrayPrototypeValue, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }

        private static object? TypedArrayPrototypeFindLast(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.findLast called on incompatible receiver");
            }

            return typedArray.findLast(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeAt(object? thisArgument, object? index)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.at called on incompatible receiver");
            }

            return typedArray.at(index);
        }

        private static object? TypedArrayPrototypeEntries(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.entries called on incompatible receiver");
            }

            return typedArray.entries();
        }

        private static object? TypedArrayPrototypeKeys(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.keys called on incompatible receiver");
            }

            return typedArray.keys();
        }

        private static object? TypedArrayPrototypeValues(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.values called on incompatible receiver");
            }

            return typedArray.values();
        }

        private static object? TypedArrayFrom(object? thisArgument, object? source, object? mapFn, object? thisArg)
        {
            if (!CallableOperations.IsConstructor(thisArgument))
            {
                throw new TypeError("%TypedArray%.from called on a value that is not a constructor");
            }

            var mapping = mapFn is not null && mapFn is not JsNull;
            if (mapping && !CallableOperations.IsCallable(mapFn))
            {
                throw new TypeError("%TypedArray%.from: mapfn is not callable");
            }

            if (source is null || source is JsNull)
            {
                throw new TypeError("%TypedArray%.from called with null or undefined source");
            }

            object? iteratorMethod = ObjectRuntime.GetItem(source, global::JavaScriptRuntime.Symbol.iterator);
            if (iteratorMethod is JsNull)
            {
                iteratorMethod = null;
            }

            List<object?> values;
            if (iteratorMethod is not null)
            {
                if (!CallableOperations.IsCallable(iteratorMethod))
                {
                    throw new TypeError("Symbol.iterator is not a function");
                }

                values = new List<object?>();
                var iterator = ObjectRuntime.GetIteratorFromMethod(source, iteratorMethod);
                while (true)
                {
                    var step = iterator.Next();
                    if (step.done)
                    {
                        break;
                    }

                    values.Add(step.value);
                }
            }
            else
            {
                var length = ToArrayLikeLength(ObjectRuntime.GetItem(source, "length"));
                values = new List<object?>(length);
                for (var i = 0; i < length; i++)
                {
                    values.Add(ObjectRuntime.GetItem(source, (double)i));
                }
            }

            var target = CallableOperations.Construct1(thisArgument, thisArgument, (double)values.Count);
            for (var i = 0; i < values.Count; i++)
            {
                var value = mapping
                    ? CallableOperations.Call2(mapFn, thisArg, values[i], (double)i)
                    : values[i];
                ObjectRuntime.SetProperty(
                    target!,
                    i.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    value,
                    throwOnError: true);
            }

            return target;
        }

        private static object? TypedArrayOf(object? thisArgument, in JsCallArguments arguments)
        {
            if (!CallableOperations.IsConstructor(thisArgument))
            {
                throw new TypeError("%TypedArray%.of called on a value that is not a constructor");
            }

            var items = arguments.ToArray();
            var target = CallableOperations.Construct1(thisArgument, thisArgument, (double)items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                ObjectRuntime.SetProperty(
                    target!,
                    i.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    items[i],
                    throwOnError: true);
            }

            return target;
        }

        private static int ToArrayLikeLength(object? lengthValue)
        {
            var number = TypeUtilities.ToNumber(lengthValue);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            var truncated = System.Math.Min(System.Math.Truncate(number), 9007199254740991d);
            return (int)System.Math.Min(truncated, int.MaxValue);
        }

        private static object? TypedArrayPrototypeSort(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.sort called on incompatible receiver");
            }

            return typedArray.sort(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeToSorted(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.toSorted called on incompatible receiver");
            }

            return typedArray.toSorted(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeWith(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.with called on incompatible receiver");
            }

            return typedArray.with(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeLength(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.length called on incompatible receiver");
            }

            return typedArray.length;
        }

        private static object? TypedArrayPrototypeBuffer(object? thisArgument)
            => GetTypedArrayReceiver(thisArgument, "buffer").buffer;

        private static object? TypedArrayPrototypeByteOffset(object? thisArgument)
            => GetTypedArrayReceiver(thisArgument, "byteOffset").byteOffset;

        private static object? TypedArrayPrototypeByteLength(object? thisArgument)
            => GetTypedArrayReceiver(thisArgument, "byteLength").byteLength;

        private static object? TypedArrayPrototypeToStringTag(object? thisArgument)
            => thisArgument is TypedArrayBase typedArray
                ? typedArray.TypedArrayNameValue
                : null;

        private static TypedArrayBase GetTypedArrayReceiver(object? thisArgument, string propertyName)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError($"TypedArray.prototype.{propertyName} called on incompatible receiver");
            }

            return typedArray;
        }

        private static object? TypedArrayPrototypeToString(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.toString called on incompatible receiver");
            }

            return typedArray.toString();
        }

        private static object? TypedArrayPrototypeToLocaleString(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.toLocaleString called on incompatible receiver");
            }

            return typedArray.toLocaleString();
        }

        private static object? TypedArrayPrototypeFindLastIndex(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.findLastIndex called on incompatible receiver");
            }

            return typedArray.findLastIndex(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeCopyWithin(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.copyWithin called on incompatible receiver");
            }

            return typedArray.copyWithin(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeReduceRight(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.reduceRight called on incompatible receiver");
            }

            return typedArray.reduceRight(arguments.ToArray());
        }

        private static object? TypedArrayPrototypeToReversed(object? thisArgument)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.toReversed called on incompatible receiver");
            }

            return typedArray.toReversed();
        }

        private static object ConstructTypedArray(
            object?[]? args,
            Func<object> constructEmpty,
            Func<object?, object> constructOne,
            Func<object?, object?, object> constructTwo,
            Func<object?, object?, object?, object> constructThree)
            => args?.Length switch
            {
                null or 0 => constructEmpty(),
                1 => constructOne(args[0]),
                2 => constructTwo(args[0], args[1]),
                _ => constructThree(args[0], args[1], args[2])
            };

        internal static ServiceContainer? ServiceProvider
        {
            get => RuntimeExecutionContext.Current?.Services;
            set
            {
                RuntimeExecutionContext.SetLegacyServiceProvider(value);
                _fallbackGlobalObject.Value = null;
            }
        }

        public override void Clear() =>
            throw new NotSupportedException("Clearing the global object is not supported.");

        /// <summary>
        /// ECMA-262 globalThis value.
        /// Returns the global object for the current execution context.
        /// </summary>
        /// <remarks>
        /// JROC models the global object as a <see cref="JsObject"/> seeded with common globals.
        /// This allows libraries to read/write properties via globalThis (e.g., globalThis.window = ...).
        /// </remarks>
        public static object globalThis => GetOrCreateGlobalObject();

        /// <summary>
        /// Returns the current global object for codegen helpers.
        /// </summary>
        public static object GetGlobalThis() => globalThis;

        private static GlobalThis GetOrCreateGlobalObject()
        {
            if (RuntimeExecutionContext.Current is { } executionContext)
            {
                return executionContext.GetOrCreateGlobalObject();
            }

            var obj = _fallbackGlobalObject.Value;
            if (obj == null)
            {
                // Publish before bootstrapping (see GlobalThis.Bootstrap remarks): a
                // reentrant globalThis/GetOrCreateGlobalObject() lookup that happens
                // during this realm's own bootstrap must observe this instance
                // instead of recursively constructing another one.
                obj = new GlobalThis(RuntimeIntrinsics.Current);
                _fallbackGlobalObject.Value = obj;
                obj.Bootstrap();
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

        private void DefineWellKnownSymbolProperty(string key, global::JavaScriptRuntime.Symbol value)
        {
            PropertyDescriptorStore.DefineOrUpdate(_symbolFunctionValue, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
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
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                functionValue,
                length,
                key,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(functionValue));
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
            PrototypeChain.SetPrototype(this, _objectPrototypeValue);

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

            dict.TryAdd(nameof(GlobalThis.DataView), DataView);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.DataView), dict[nameof(GlobalThis.DataView)]);

            dict.TryAdd(nameof(GlobalThis.Atomics), _atomicsValue);
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

            dict.TryAdd(nameof(GlobalThis.WeakRef), WeakRef);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.WeakRef), dict[nameof(GlobalThis.WeakRef)]);

            dict.TryAdd(nameof(GlobalThis.FinalizationRegistry), FinalizationRegistry);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.FinalizationRegistry), dict[nameof(GlobalThis.FinalizationRegistry)]);

            dict.TryAdd(nameof(GlobalThis.Object), Object);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.Object), dict[nameof(GlobalThis.Object)]);

            dict.TryAdd(nameof(GlobalThis.JSON), _jsonValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.JSON), dict[nameof(GlobalThis.JSON)]);

            dict.TryAdd(nameof(GlobalThis.Intl), _intlValue);
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

            dict.TryAdd(nameof(GlobalThis.AggregateError), AggregateError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.AggregateError), dict[nameof(GlobalThis.AggregateError)]);

            dict.TryAdd(nameof(GlobalThis.SuppressedError), SuppressedError);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.SuppressedError), dict[nameof(GlobalThis.SuppressedError)]);

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

            dict.TryAdd(nameof(GlobalThis.decodeURI), _decodeURIValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.decodeURI), dict[nameof(GlobalThis.decodeURI)]);

            dict.TryAdd(nameof(GlobalThis.encodeURI), _encodeURIValue);
            DefineNonEnumerableDataProperty(nameof(GlobalThis.encodeURI), dict[nameof(GlobalThis.encodeURI)]);

            dict.TryAdd("decodeURIComponent", _decodeURIComponentValue);
            DefineNonEnumerableDataProperty("decodeURIComponent", dict["decodeURIComponent"]);

            dict.TryAdd("encodeURIComponent", _encodeURIComponentValue);
            DefineNonEnumerableDataProperty("encodeURIComponent", dict["encodeURIComponent"]);

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
        /// Returns this realm's built-in function object for a global function such as
        /// <c>setTimeout</c> or <c>parseInt</c>. Realm-owned (issue #1824): the delegate
        /// behind it is immutable CLR metadata, but the JavaScript-visible function
        /// object identity belongs to the realm.
        /// </summary>
        public static JsFunctionObject GetFunctionValue(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            return RuntimeIntrinsics.Current.GlobalFunctionValues.GetOrAdd(
                name,
                static functionName =>
                {
                    var (target, length) = functionName switch
                    {
                        nameof(setTimeout) => ((Delegate)(Func<object, object, object[], object>)setTimeout, 2d),
                        nameof(clearTimeout) => ((Delegate)(Func<object, object?>)clearTimeout, 1d),
                        nameof(setImmediate) => ((Delegate)(Func<object, object[], object>)setImmediate, 1d),
                        nameof(setInterval) => ((Delegate)(Func<object, object, object[], object>)setInterval, 2d),
                        nameof(clearImmediate) => ((Delegate)(Func<object, object?>)clearImmediate, 1d),
                        nameof(clearInterval) => ((Delegate)(Func<object, object?>)clearInterval, 1d),
                        nameof(gc) => ((Delegate)(Func<object?>)gc, 0d),
                        nameof(parseInt) => ((Delegate)(Func<object?, object?, double>)parseInt, 2d),
                        nameof(parseFloat) => ((Delegate)(Func<object?, double>)parseFloat, 1d),
                        nameof(isFinite) => ((Delegate)(Func<object?, bool>)isFinite, 1d),
                        nameof(isNaN) => ((Delegate)(Func<object?, bool>)isNaN, 1d),
                        nameof(decodeURI) => ((Delegate)(Func<object?, string>)decodeURI, 1d),
                        nameof(encodeURI) => ((Delegate)(Func<object?, string>)encodeURI, 1d),
                        _ => throw new ArgumentOutOfRangeException(
                            nameof(name),
                            functionName,
                            "Unknown global function.")
                    };
                    var adapter =
                        BuiltinDelegateFunctionAdapter.FromDelegate(target);
                    JavaScriptRuntime.Function.InitializeFunctionInstance(
                        adapter,
                        length,
                        functionName,
                        requiresInvocationContext: false);
                    JavaScriptRuntime.Function.MarkUndefinedPrototype(adapter);
                    return adapter;
                });
        }

        internal static bool IsNumberConstructorTarget(Delegate target)
            => ReferenceEquals(target, _numberFunctionValue)
                || target.Method == _numberFunctionValue.Method;

        internal static bool IsStringConstructorTarget(Delegate target)
            => ReferenceEquals(target, _stringFunctionValue)
                || target.Method == _stringFunctionValue.Method;

        internal static bool IsBigIntConstructorTarget(Delegate target)
            => ReferenceEquals(target, _bigIntFunctionValue)
                || target.Method == _bigIntFunctionValue.Method;

        internal static bool IsPromiseConstructorValue(object? value)
            => ReferenceEquals(value, _promiseConstructorValue)
                || value is BuiltinDelegateFunctionAdapter adapter
                    && ReferenceEquals(
                        adapter.Target,
                        _promiseConstructorValue);

        /// <summary>
        /// Minimal process global with writable exitCode.
        /// </summary>
        /// <remarks>Expand as needed in the future.</remarks>
        public static JavaScriptRuntime.Node.Process process
        {
            get
            {
                var serviceProvider = ServiceProvider;
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
                var serviceProvider = ServiceProvider;
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
        internal static object ArrayBufferIntrinsicConstructor
        {
            get
            {
                _ = globalThis;
                return BuiltinDelegateFunctionAdapter.FromDelegate(_arrayBufferConstructorValue);
            }
        }

        public static Delegate DataView => _dataViewConstructorValue;

        public static object Atomics => BootstrappedIntrinsics().Atomics;

        /// <summary>
        /// ECMAScript global Array constructor value (placeholder).
        /// Exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Array constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?[], object?> Array => _arrayConstructorValue;

        internal static bool IsArrayConstructorValue(object? value)
            => ReferenceEquals(value, _arrayConstructorValue)
                || value is BuiltinDelegateFunctionAdapter adapter
                    && ReferenceEquals(adapter.Target, _arrayConstructorValue);

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

        public static Delegate WeakRef => _weakRefConstructorValue;

        public static Delegate FinalizationRegistry => _finalizationRegistryConstructorValue;

        public static Func<object[], object?, object> Object => _objectConstructorValue;

        public static object JSON => BootstrappedIntrinsics().Json;

        public static object Intl => BootstrappedIntrinsics().Intl;

        public Delegate Symbol => _symbolFunctionValue;

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

        public static Func<object[], object?[], object?> AggregateError => _aggregateErrorConstructorValue;

        public static Func<object[], object?[], object?> SuppressedError => _suppressedErrorConstructorValue;

        public static Func<object[], object?[], object?> Iterator => _iteratorConstructorValue;

        public static Func<object[], object?[], object?> AsyncIterator => _asyncIteratorConstructorValue;

        public static Type AbortController => typeof(JavaScriptRuntime.AbortController);

        public static Type AbortSignal => typeof(JavaScriptRuntime.AbortSignal);

        public static Delegate URL
        {
            get
            {
                // Materializes this realm's URL.prototype, which wires the constructor
                // surface for this realm (issue #1824).
                _ = JavaScriptRuntime.Node.URL.Prototype;
                return JavaScriptRuntime.Node.Url.URLConstructorValue;
            }
        }

        public static Delegate URLSearchParams
        {
            get
            {
                _ = JavaScriptRuntime.Node.URLSearchParams.Prototype;
                return JavaScriptRuntime.Node.Url.URLSearchParamsConstructorValue;
            }
        }

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
            var serviceProvider = ServiceProvider;
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
            while (i < text.Length && char.IsAsciiDigit(text[i]))
            {
                sawDigit = true;
                i++;
            }

            if (i < text.Length && text[i] == '.')
            {
                i++;
                while (i < text.Length && char.IsAsciiDigit(text[i]))
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
                while (i < text.Length && char.IsAsciiDigit(text[i]))
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

        /// <summary>
        /// Decodes a URI using the percent-decoding algorithm specified for the global
        /// <c>decodeURI</c> function.
        /// </summary>
        public static string decodeURI(object? encodedURI)
            => DecodeUri(encodedURI, preserveReserved: true);

        private static string decodeURIComponent(object? encodedURI)
            => DecodeUri(encodedURI, preserveReserved: false);

        private static string DecodeUri(object? encodedURI, bool preserveReserved)
        {
            var input = DotNet2JSConversions.ToString(encodedURI);
            var result = new StringBuilder(input.Length);
            var index = 0;
            Span<byte> bytes = stackalloc byte[4];

            while (index < input.Length)
            {
                if (input[index] != '%')
                {
                    result.Append(input[index++]);
                    continue;
                }

                var escapeStart = index;
                var firstByte = ParseUriHexOctet(input, ref index);
                if (firstByte <= 0x7F)
                {
                    var decodedCharacter = (char)firstByte;
                    if (preserveReserved && IsUriReserved(decodedCharacter))
                    {
                        result.Append(input, escapeStart, index - escapeStart);
                    }
                    else
                    {
                        result.Append(decodedCharacter);
                    }

                    continue;
                }

                bytes[0] = firstByte;
                var sequenceLength = GetUtf8SequenceLength(firstByte);
                for (var byteIndex = 1; byteIndex < sequenceLength; byteIndex++)
                {
                    bytes[byteIndex] = ParseUriHexOctet(input, ref index);
                }

                try
                {
                    result.Append(_strictUtf8.GetString(bytes[..sequenceLength]));
                }
                catch (DecoderFallbackException)
                {
                    throw new URIError("URI malformed");
                }
            }

            return result.ToString();
        }

        private static byte ParseUriHexOctet(string input, ref int index)
        {
            if (index + 2 >= input.Length
                || input[index] != '%'
                || !TryParseHexDigit(input[index + 1], out var high)
                || !TryParseHexDigit(input[index + 2], out var low))
            {
                throw new URIError("URI malformed");
            }

            index += 3;
            return (byte)((high << 4) | low);
        }

        private static bool TryParseHexDigit(char value, out int digit)
        {
            if (value is >= '0' and <= '9')
            {
                digit = value - '0';
                return true;
            }

            if (value is >= 'A' and <= 'F')
            {
                digit = value - 'A' + 10;
                return true;
            }

            if (value is >= 'a' and <= 'f')
            {
                digit = value - 'a' + 10;
                return true;
            }

            digit = 0;
            return false;
        }

        private static int GetUtf8SequenceLength(byte firstByte)
        {
            return firstByte switch
            {
                >= 0xC2 and <= 0xDF => 2,
                >= 0xE0 and <= 0xEF => 3,
                >= 0xF0 and <= 0xF4 => 4,
                _ => throw new URIError("URI malformed")
            };
        }

        private static bool IsUriReserved(char value)
        {
            return value is ';' or '/' or '?' or ':' or '@' or '&' or '=' or '+' or '$' or ',' or '#';
        }

        /// <summary>
        /// Encodes a URI using the percent-encoding algorithm specified for the global
        /// <c>encodeURI</c> function.
        /// </summary>
        public static string encodeURI(object? uri)
            => EncodeUri(uri, preserveReserved: true);

        private static string encodeURIComponent(object? uri)
            => EncodeUri(uri, preserveReserved: false);

        private static string EncodeUri(object? uri, bool preserveReserved)
        {
            var input = DotNet2JSConversions.ToString(uri);
            var result = new StringBuilder(input.Length);

            for (var index = 0; index < input.Length; index++)
            {
                var codeUnit = input[index];
                if (char.IsHighSurrogate(codeUnit))
                {
                    if (index + 1 >= input.Length || !char.IsLowSurrogate(input[index + 1]))
                    {
                        throw new URIError("URI malformed");
                    }

                    AppendUriEncodedCodePoint(result, char.ConvertToUtf32(codeUnit, input[++index]));
                    continue;
                }

                if (char.IsLowSurrogate(codeUnit))
                {
                    throw new URIError("URI malformed");
                }

                if (IsEncodeUriUnescaped(codeUnit, preserveReserved))
                {
                    result.Append(codeUnit);
                }
                else
                {
                    AppendUriEncodedCodePoint(result, codeUnit);
                }
            }

            return result.ToString();
        }

        private static bool IsEncodeUriUnescaped(char value, bool preserveReserved)
        {
            var unescaped = value is >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or >= '0' and <= '9'
                or '-' or '_' or '.' or '!' or '~' or '*' or '\'' or '(' or ')';
            return unescaped || (preserveReserved && IsUriReserved(value));
        }

        private static void AppendUriEncodedCodePoint(StringBuilder result, int codePoint)
        {
            Span<byte> utf8 = stackalloc byte[4];
            var count = new System.Text.Rune(codePoint).EncodeToUtf8(utf8);
            for (var index = 0; index < count; index++)
            {
                result.Append('%');
                result.Append(utf8[index].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        private static Timers GetTimers()
        {
            return ServiceProvider!.Resolve<Timers>();
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

        private void ConfigurePromiseIntrinsicSurface(object constructorValue, object prototypeValue)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
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

        private void ConfigureCollectionIntrinsicSurface(object constructorValue, object prototypeValue)
        {
            ConfigureConstructorPrototypeSurface(constructorValue, prototypeValue);
            DefineSpeciesAccessorProperty(constructorValue);
        }

        private void ConfigureWeakRefIntrinsicSurface()
        {
            ConfigureConstructorPrototypeSurface(_weakRefConstructorValue, JavaScriptRuntime.WeakRef.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_weakRefConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_weakRefConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "WeakRef"
            });
        }

        private void ConfigureFinalizationRegistryIntrinsicSurface()
        {
            ConfigureConstructorPrototypeSurface(
                _finalizationRegistryConstructorValue,
                JavaScriptRuntime.FinalizationRegistry.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_finalizationRegistryConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_finalizationRegistryConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "FinalizationRegistry"
            });
        }

        private void ConfigureDataViewIntrinsicSurface()
        {
            ConfigureConstructorPrototypeSurface(_dataViewConstructorValue, JavaScriptRuntime.DataView.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_dataViewConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_dataViewConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "DataView"
            });
            DefineDataViewAccessor("buffer", DataViewBufferGetter);
            DefineDataViewAccessor("byteLength", DataViewByteLengthGetter);
            DefineDataViewAccessor("byteOffset", DataViewByteOffsetGetter);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getInt8", (BuiltinFunction1)DataViewGetInt8, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getUint8", (BuiltinFunction1)DataViewGetUint8, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getInt16", (BuiltinFunction2)DataViewGetInt16, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getUint16", (BuiltinFunction2)DataViewGetUint16, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getInt32", (BuiltinFunction2)DataViewGetInt32, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getUint32", (BuiltinFunction2)DataViewGetUint32, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getFloat32", (BuiltinFunction2)DataViewGetFloat32, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "getFloat64", (BuiltinFunction2)DataViewGetFloat64, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setInt8", (BuiltinFunction2)DataViewSetInt8, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setUint8", (BuiltinFunction2)DataViewSetUint8, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setInt16", (BuiltinFunction3)DataViewSetInt16, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setUint16", (BuiltinFunction3)DataViewSetUint16, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setInt32", (BuiltinFunction3)DataViewSetInt32, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setUint32", (BuiltinFunction3)DataViewSetUint32, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setFloat32", (BuiltinFunction3)DataViewSetFloat32, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.DataView.Prototype, "setFloat64", (BuiltinFunction3)DataViewSetFloat64, 2d);
            DefineIntrinsicToStringTagProperty(JavaScriptRuntime.DataView.Prototype, "DataView");
        }

        private void ConfigureArrayBufferIntrinsicSurface()
        {
            ConfigureConstructorPrototypeSurface(_arrayBufferConstructorValue, JavaScriptRuntime.ArrayBuffer.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(_arrayBufferConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data, Enumerable = false, Configurable = true, Writable = false, Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_arrayBufferConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data, Enumerable = false, Configurable = true, Writable = false, Value = "ArrayBuffer"
            });
            DefineBuiltinFunctionProperty(_arrayBufferConstructorValue, "isView", _arrayBufferIsViewValue, 1d);
            DefineSpeciesAccessorProperty(_arrayBufferConstructorValue);
            DefineArrayBufferAccessor("byteLength", static buffer => buffer.byteLength);
            DefineArrayBufferAccessor("detached", static buffer => buffer.detached);
            DefineArrayBufferAccessor("maxByteLength", static buffer => buffer.maxByteLength);
            DefineArrayBufferAccessor("resizable", static buffer => buffer.resizable);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.ArrayBuffer.Prototype, "resize", _arrayBufferPrototypeResizeValue, 1d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.ArrayBuffer.Prototype, "slice", _arrayBufferPrototypeSliceValue, 2d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.ArrayBuffer.Prototype, "transfer", _arrayBufferPrototypeTransferValue, 0d);
            DefineBuiltinFunctionProperty(JavaScriptRuntime.ArrayBuffer.Prototype, "transferToFixedLength", _arrayBufferPrototypeTransferToFixedLengthValue, 0d);
            DefineIntrinsicToStringTagProperty(JavaScriptRuntime.ArrayBuffer.Prototype, "ArrayBuffer");
        }

        private void ConfigureSharedArrayBufferIntrinsicSurface()
        {
            ConfigureConstructorPrototypeSurface(
                _sharedArrayBufferConstructorValue,
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype);
            PropertyDescriptorStore.DefineOrUpdate(_sharedArrayBufferConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data, Enumerable = false, Configurable = true, Writable = false, Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(_sharedArrayBufferConstructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data, Enumerable = false, Configurable = true, Writable = false, Value = "SharedArrayBuffer"
            });
            DefineSharedArrayBufferAccessor("byteLength", static buffer => buffer.byteLength);
            DefineSharedArrayBufferAccessor("maxByteLength", static buffer => buffer.maxByteLength);
            DefineSharedArrayBufferAccessor("growable", static _ => false);
            DefineBuiltinFunctionProperty(
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype,
                "slice",
                _sharedArrayBufferPrototypeSliceValue,
                2d);
            DefineIntrinsicToStringTagProperty(JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, "SharedArrayBuffer");
        }

        private static void DefineSharedArrayBufferAccessor(
            string propertyName,
            Func<JavaScriptRuntime.SharedArrayBuffer, object?> read)
        {
            BuiltinFunction0 getter = thisArgument =>
            {
                if (thisArgument is not JavaScriptRuntime.SharedArrayBuffer buffer)
                {
                    throw new TypeError($"get SharedArrayBuffer.prototype.{propertyName} called on incompatible receiver");
                }
                return read(buffer);
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {propertyName}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor, Enumerable = false, Configurable = true, Get = getter
            });
        }

        private static void DefineArrayBufferAccessor(string propertyName, Func<JavaScriptRuntime.ArrayBuffer, object?> read)
        {
            BuiltinFunction0 getter = thisArgument =>
            {
                if (thisArgument is not JavaScriptRuntime.ArrayBuffer buffer
                    || thisArgument is JavaScriptRuntime.SharedArrayBuffer)
                {
                    throw new TypeError($"get ArrayBuffer.prototype.{propertyName} called on incompatible receiver");
                }
                return read(buffer);
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {propertyName}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.ArrayBuffer.Prototype, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor, Enumerable = false, Configurable = true, Get = getter
            });
        }

        private static void DefineDataViewAccessor(
            string propertyName,
            BuiltinFunction0 getter)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {propertyName}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.DataView.Prototype, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }

        private static object? DataViewBufferGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "buffer", isAccessor: true).buffer;

        private static object? DataViewByteLengthGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "byteLength", isAccessor: true).byteLength;

        private static object? DataViewByteOffsetGetter(object? thisArgument)
            => GetDataViewThis(thisArgument, "byteOffset", isAccessor: true).byteOffset;

        private static object? DataViewGetInt8(object? thisArgument, object? byteOffset)
            => GetDataViewThis(thisArgument, "getInt8").getInt8(byteOffset);

        private static object? DataViewGetUint8(object? thisArgument, object? byteOffset)
            => GetDataViewThis(thisArgument, "getUint8").getUint8(byteOffset);

        private static object? DataViewGetInt16(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getInt16").getInt16(byteOffset, littleEndian);

        private static object? DataViewGetUint16(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getUint16").getUint16(byteOffset, littleEndian);

        private static object? DataViewGetInt32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getInt32").getInt32(byteOffset, littleEndian);

        private static object? DataViewGetUint32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getUint32").getUint32(byteOffset, littleEndian);

        private static object? DataViewGetFloat32(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getFloat32").getFloat32(byteOffset, littleEndian);

        private static object? DataViewGetFloat64(object? thisArgument, object? byteOffset, object? littleEndian)
            => GetDataViewThis(thisArgument, "getFloat64").getFloat64(byteOffset, littleEndian);

        private static object? DataViewSetInt8(object? thisArgument, object? byteOffset, object? value)
            => GetDataViewThis(thisArgument, "setInt8").setInt8(byteOffset, value);

        private static object? DataViewSetUint8(object? thisArgument, object? byteOffset, object? value)
            => GetDataViewThis(thisArgument, "setUint8").setUint8(byteOffset, value);

        private static object? DataViewSetInt16(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setInt16").setInt16(byteOffset, value, littleEndian);

        private static object? DataViewSetUint16(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setUint16").setUint16(byteOffset, value, littleEndian);

        private static object? DataViewSetInt32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setInt32").setInt32(byteOffset, value, littleEndian);

        private static object? DataViewSetUint32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setUint32").setUint32(byteOffset, value, littleEndian);

        private static object? DataViewSetFloat32(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setFloat32").setFloat32(byteOffset, value, littleEndian);

        private static object? DataViewSetFloat64(
            object? thisArgument,
            object? byteOffset,
            object? value,
            object? littleEndian)
            => GetDataViewThis(thisArgument, "setFloat64").setFloat64(byteOffset, value, littleEndian);

        private static JavaScriptRuntime.DataView GetDataViewThis(
            object? thisArgument,
            string memberName,
            bool isAccessor = false)
        {
            if (thisArgument is not JavaScriptRuntime.DataView dataView)
            {
                var prefix = isAccessor ? "get " : string.Empty;
                throw new TypeError($"{prefix}DataView.prototype.{memberName} called on incompatible receiver");
            }

            return dataView;
        }


        private static void DefineSpeciesAccessorProperty(object constructorValue)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _speciesGetterValue,
                0d,
                "get [Symbol.species]",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_speciesGetterValue));
            DefineUndefinedPrototypeProperty(_speciesGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, global::JavaScriptRuntime.Symbol.species.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _speciesGetterValue
            });
        }

        private void ConfigureConstructorPrototypeSurface(object constructorValue, object prototypeValue)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
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

        private void ConfigureErrorSubclassIntrinsicSurface(object constructorValue, object prototypeValue, string name)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
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

        private static object? SpeciesGetter(object? thisArgument)
        {
            return thisArgument;
        }

        private static bool TryGetThisSymbolValue(
            object? thisValue,
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JavaScriptRuntime.Symbol? symbol)
        {
            if (thisValue is JavaScriptRuntime.Symbol directSymbol)
            {
                symbol = directSymbol;
                return true;
            }

            if (thisValue != null
                && PropertyDescriptorStore.TryGetOwn(thisValue, ObjectRuntime.PrimitiveValuePropertyName, out var descriptor)
                && descriptor.Value is JavaScriptRuntime.Symbol boxedSymbol)
            {
                symbol = boxedSymbol;
                return true;
            }

            symbol = null;
            return false;
        }

        private static object? SymbolPrototypeDescription(object? thisArgument)
        {
            if (!TryGetThisSymbolValue(thisArgument, out var symbol))
            {
                throw new TypeError("Symbol.prototype.description called on incompatible receiver");
            }

            return symbol.Description;
        }

        internal static object ObjectPrototypeValue => RuntimeIntrinsics.Current.ObjectPrototype;

        /// <summary>
        /// Resolves the ambient realm's intrinsic graph for callers that need it fully
        /// wired (primitive/error prototypes get their properties from the realm
        /// bootstrap, not from a per-slot initializer).
        /// </summary>
        /// <remarks>
        /// The common case is a volatile read of the realm's bootstrap state followed by
        /// the intrinsic slot's lock-free fast path: no global-object lock, and no
        /// per-access execution-context lock, is taken on these hot accessors.
        /// </remarks>
        private static RuntimeIntrinsics BootstrappedIntrinsics()
        {
            var intrinsics = RuntimeIntrinsics.Current;
            return intrinsics.IsBootstrapped
                ? intrinsics
                : EnsureRealmBootstrapped(intrinsics);
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static RuntimeIntrinsics EnsureRealmBootstrapped(RuntimeIntrinsics intrinsics)
        {
            // Materializing the global object performs (or waits for) this realm's
            // one-time bootstrap; the graph itself is then read through the intrinsics.
            _ = GetOrCreateGlobalObject();
            return intrinsics;
        }

        internal static object GetTypedArrayInstancePrototype(TypedArrayBase typedArray)
        {
            var current = BootstrappedIntrinsics();
            return typedArray switch
            {
                JavaScriptRuntime.Float64Array => current.Float64ArrayPrototype,
                JavaScriptRuntime.Float32Array => current.Float32ArrayPrototype,
                JavaScriptRuntime.Int32Array => current.Int32ArrayPrototype,
                JavaScriptRuntime.Int16Array => current.Int16ArrayPrototype,
                JavaScriptRuntime.Int8Array => current.Int8ArrayPrototype,
                JavaScriptRuntime.Uint32Array => current.Uint32ArrayPrototype,
                JavaScriptRuntime.Uint16Array => current.Uint16ArrayPrototype,
                JavaScriptRuntime.Uint8Array => JavaScriptRuntime.Uint8Array.Prototype,
                JavaScriptRuntime.Uint8ClampedArray => JavaScriptRuntime.Uint8ClampedArray.Prototype,
                JavaScriptRuntime.BigInt64Array => JavaScriptRuntime.BigInt64Array.Prototype,
                JavaScriptRuntime.BigUint64Array => JavaScriptRuntime.BigUint64Array.Prototype,
                _ => current.TypedArrayPrototype
            };
        }
        internal static object NumberPrototypeValue => BootstrappedIntrinsics().NumberPrototype;
        internal static object BooleanPrototypeValue => BootstrappedIntrinsics().BooleanPrototype;
        internal static object BigIntPrototypeValue => BootstrappedIntrinsics().BigIntPrototype;
        internal static object SymbolPrototypeValue => BootstrappedIntrinsics().SymbolPrototype;
        internal static object DatePrototypeValue => JavaScriptRuntime.Date.Prototype;
        internal static object ErrorPrototypeValue => BootstrappedIntrinsics().ErrorPrototype;
        internal static object EvalErrorPrototypeValue => BootstrappedIntrinsics().EvalErrorPrototype;
        internal static object RangeErrorPrototypeValue => BootstrappedIntrinsics().RangeErrorPrototype;
        internal static object ReferenceErrorPrototypeValue => BootstrappedIntrinsics().ReferenceErrorPrototype;
        internal static object SyntaxErrorPrototypeValue => BootstrappedIntrinsics().SyntaxErrorPrototype;
        internal static object TypeErrorPrototypeValue => BootstrappedIntrinsics().TypeErrorPrototype;
        internal static object URIErrorPrototypeValue => BootstrappedIntrinsics().URIErrorPrototype;
        internal static object AggregateErrorPrototypeValue => BootstrappedIntrinsics().AggregateErrorPrototype;
        internal static object SuppressedErrorPrototypeValue => BootstrappedIntrinsics().SuppressedErrorPrototype;
        private static Func<object[], object?[], object?> CreateErrorConstructorValue(Func<object?, object> factory)
        {
            return (_, args) =>
            {
                var message = args != null && args.Length > 0 ? args[0] : null;
                var error = factory(message);
                if (args != null && args.Length > 1)
                {
                    JavaScriptRuntime.Error.InstallCause(error, args[1]);
                }

                return error;
            };
        }

        private static void ConfigureErrorIntrinsicSurface(object constructorValue, object prototypeValue, string name, object parentPrototype, double length = 1d)
        {
            ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
            PrototypeChain.SetPrototype(prototypeValue, parentPrototype);

            // Error and the NativeError constructors (EvalError, RangeError, ReferenceError,
            // SyntaxError, TypeError, URIError) all have a length of 1; AggregateError and
            // SuppressedError pass their own larger arities via the length parameter.
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = length
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
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

        private void ConfigureAggregateErrorIntrinsicSurface()
        {
            ConfigureErrorIntrinsicSurface(
                _aggregateErrorConstructorValue,
                _aggregateErrorPrototypeValue,
                "AggregateError",
                _errorPrototypeValue);
            PrototypeChain.SetPrototype(_aggregateErrorConstructorValue, _errorConstructorValue);

            PropertyDescriptorStore.DefineOrUpdate(_aggregateErrorConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 2d
            });
            PropertyDescriptorStore.DefineOrUpdate(_aggregateErrorConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _aggregateErrorPrototypeValue
            });
        }

        private void ConfigureSuppressedErrorIntrinsicSurface()
        {
            ConfigureErrorIntrinsicSurface(
                _suppressedErrorConstructorValue,
                _suppressedErrorPrototypeValue,
                "SuppressedError",
                _errorPrototypeValue);
            PrototypeChain.SetPrototype(_suppressedErrorConstructorValue, _errorConstructorValue);

            PropertyDescriptorStore.DefineOrUpdate(_suppressedErrorConstructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 3d
            });
            PropertyDescriptorStore.DefineOrUpdate(_suppressedErrorConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = _suppressedErrorPrototypeValue
            });
        }

        internal static void AssignBuiltInErrorPrototype(JavaScriptRuntime.Error error)
        {
            ArgumentNullException.ThrowIfNull(error);

            var current = BootstrappedIntrinsics();

            // Keep this aligned with the explicitly exposed built-in error constructor values above.
            var prototype = error switch
            {
                JavaScriptRuntime.EvalError => current.EvalErrorPrototype,
                JavaScriptRuntime.RangeError => current.RangeErrorPrototype,
                JavaScriptRuntime.ReferenceError => current.ReferenceErrorPrototype,
                JavaScriptRuntime.SyntaxError => current.SyntaxErrorPrototype,
                JavaScriptRuntime.TypeError => current.TypeErrorPrototype,
                JavaScriptRuntime.URIError => current.URIErrorPrototype,
                JavaScriptRuntime.AggregateError => current.AggregateErrorPrototype,
                JavaScriptRuntime.SuppressedError => current.SuppressedErrorPrototype,
                _ => current.ErrorPrototype
            };

            PrototypeChain.SetPrototype(error, prototype);
        }

        internal static void ConfigureBuiltinFunctionObject(object functionValue)
        {
            JavaScriptRuntime.Function.ConfigureCallableObject(functionValue, hasRestrictedProperties: false);
        }

        private static object? SymbolCall(object? thisArgument, object? description)
        {
            var symbol = (global::JavaScriptRuntime.Symbol)global::JavaScriptRuntime.Symbol.Call(description);
            PrototypeChain.SetPrototype(symbol, SymbolPrototypeValue);
            return symbol;
        }

        private static object? SymbolPrototypeToPrimitive(object? thisArgument)
        {
            return TryGetThisSymbolValue(thisArgument, out var symbol)
                ? symbol
                : throw new TypeError("Symbol.prototype[Symbol.toPrimitive] called on incompatible receiver");
        }
    }
}
