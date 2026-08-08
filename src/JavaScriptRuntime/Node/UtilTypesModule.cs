namespace JavaScriptRuntime.Node
{
    [NodeModule("util/types")]
    public sealed partial class UtilTypesModule : JsObject
    {
        public UtilTypesModule()
        {
            AddPredicate("isArray", value => value is JavaScriptRuntime.Array);
            AddPredicate("isError", value => value is Error or Exception);
            AddPredicate("isFunction", CallableOperations.IsCallable);
            AddPredicate("isString", value => value is string);
            AddPredicate("isNumber", IsNumber);
            AddPredicate("isBoolean", value => value is bool);
            AddPredicate("isUndefined", value => value == null);
            AddPredicate("isNull", value => value is JsNull);
            AddPredicate(
                "isObject",
                value => value != null
                    && value is not JsNull
                    && !IsNumber(value)
                    && value is not string
                    && value is not bool);
            AddPredicate("isBigInt", value => value is System.Numerics.BigInteger);
            AddPredicate("isSymbol", value => value is Symbol);

            AddPredicate("isAnyArrayBuffer", ContractIsAnyArrayBuffer);
            AddPredicate("isArrayBufferView", ContractIsArrayBufferView);
            AddPredicate("isArgumentsObject", ContractIsArgumentsObject);
            AddPredicate("isArrayBuffer", ContractIsArrayBuffer);
            AddPredicate("isAsyncFunction", ContractIsAsyncFunction);
            AddPredicate("isBigInt64Array", ContractIsBigInt64Array);
            AddPredicate("isBigUint64Array", ContractIsBigUint64Array);
            AddPredicate("isCryptoKey", ContractIsCryptoKey);
            AddPredicate("isDataView", ContractIsDataView);
            AddPredicate("isDate", ContractIsDate);
            AddPredicate("isExternal", ContractIsExternal);
            AddPredicate("isFloat16Array", ContractIsFloat16Array);
            AddPredicate("isFloat32Array", ContractIsFloat32Array);
            AddPredicate("isFloat64Array", ContractIsFloat64Array);
            AddPredicate("isGeneratorObject", ContractIsGeneratorObject);
            AddPredicate("isInt8Array", ContractIsInt8Array);
            AddPredicate("isInt16Array", ContractIsInt16Array);
            AddPredicate("isInt32Array", ContractIsInt32Array);
            AddPredicate("isMap", ContractIsMap);
            AddPredicate("isNativeError", ContractIsNativeError);
            AddPredicate("isPromise", ContractIsPromise);
            AddPredicate("isProxy", ContractIsProxy);
            AddPredicate("isRegExp", ContractIsRegExp);
            AddPredicate("isSet", ContractIsSet);
            AddPredicate("isSharedArrayBuffer", ContractIsSharedArrayBuffer);
            AddPredicate("isTypedArray", ContractIsTypedArray);
            AddPredicate("isUint8Array", ContractIsUint8Array);
            AddPredicate("isUint8ClampedArray", ContractIsUint8ClampedArray);
            AddPredicate("isUint16Array", ContractIsUint16Array);
            AddPredicate("isUint32Array", ContractIsUint32Array);
            AddPredicate("isWeakMap", ContractIsWeakMap);
            AddPredicate("isWeakSet", ContractIsWeakSet);
        }

        private void AddPredicate(string name, Func<object?, bool> predicate)
            => this[name] = predicate;

        private static bool IsNumber(object? value)
            => value is double or float or int or long or short or byte or decimal;

        private bool ContractIsAnyArrayBuffer(object? value)
            => value is ArrayBuffer or SharedArrayBuffer;

        private bool ContractIsArrayBufferView(object? value)
            => value is TypedArrayBase or DataView or Buffer;

        private bool ContractIsArgumentsObject(object? value)
            => value is ArgumentsObject;

        private bool ContractIsArrayBuffer(object? value)
            => value is ArrayBuffer and not SharedArrayBuffer;

        private bool ContractIsAsyncFunction(object? value)
            => value is JsAsyncFunctionObject
                || value is Delegate callback
                    && callback.Method.GetCustomAttributes(
                        typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute),
                        false).Length > 0;

        private bool ContractIsBigInt64Array(object? value)
            => false;

        private bool ContractIsBigUint64Array(object? value)
            => false;

        private bool ContractIsCryptoKey(object? value)
            => value is CryptoKey;

        private bool ContractIsDataView(object? value)
            => value is DataView;

        private bool ContractIsDate(object? value)
            => value is JavaScriptRuntime.Date or DateTime;

        private bool ContractIsExternal(object? value)
            => false;

        private bool ContractIsFloat16Array(object? value)
            => false;

        private bool ContractIsFloat32Array(object? value)
            => value is Float32Array;

        private bool ContractIsFloat64Array(object? value)
            => value is Float64Array;

        private bool ContractIsGeneratorObject(object? value)
            => value is GeneratorObject;

        private bool ContractIsInt8Array(object? value)
            => value is Int8Array;

        private bool ContractIsInt16Array(object? value)
            => value is Int16Array;

        private bool ContractIsInt32Array(object? value)
            => value is Int32Array;

        private bool ContractIsMap(object? value)
            => value is JavaScriptRuntime.Map;

        private bool ContractIsNativeError(object? value)
            => value is Error or Exception;

        private bool ContractIsPromise(object? value)
            => value is Promise;

        private bool ContractIsProxy(object? value)
            => value is Proxy;

        private bool ContractIsRegExp(object? value)
            => value is RegExp or System.Text.RegularExpressions.Regex;

        private bool ContractIsSet(object? value)
            => value is JavaScriptRuntime.Set;

        private bool ContractIsSharedArrayBuffer(object? value)
            => value is SharedArrayBuffer;

        private bool ContractIsTypedArray(object? value)
            => value is TypedArrayBase or Buffer;

        private bool ContractIsUint8Array(object? value)
            => value is Uint8Array or Buffer;

        private bool ContractIsUint8ClampedArray(object? value)
            => value is Uint8ClampedArray;

        private bool ContractIsUint16Array(object? value)
            => value is Uint16Array;

        private bool ContractIsUint32Array(object? value)
            => value is Uint32Array;

        private bool ContractIsWeakMap(object? value)
            => value is WeakMap;

        private bool ContractIsWeakSet(object? value)
            => value is WeakSet;
    }
}
