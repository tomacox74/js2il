using System;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    public abstract partial class TypedArrayBase
    {
        private static readonly Func<object[], object?[], object?> _typedArrayConstructorValue = static (_, __) =>
            throw new TypeError("%TypedArray% is not directly constructible in jroc.");
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
        private static readonly BuiltinFunctionVariadic _typedArrayFillValue = TypedArrayPrototypeFill;
        private static readonly BuiltinFunctionVariadic _typedArrayReduceRightValue = TypedArrayPrototypeReduceRight;
        private static readonly BuiltinFunction0 _typedArrayToReversedValue = TypedArrayPrototypeToReversed;
        private static readonly BuiltinFunction0 _typedArrayEntriesValue = TypedArrayPrototypeEntries;
        private static readonly BuiltinFunction0 _typedArrayKeysValue = TypedArrayPrototypeKeys;
        private static readonly BuiltinFunction0 _typedArrayValuesValue = TypedArrayPrototypeValues;
        private static readonly BuiltinFunction2 _typedArrayMapValue = TypedArrayPrototypeMap;
        private static readonly BuiltinFunction2 _typedArrayFilterValue = TypedArrayPrototypeFilter;
        private static readonly BuiltinFunction2 _typedArrayEveryValue = TypedArrayPrototypeEvery;
        private static readonly BuiltinFunction2 _typedArraySomeValue = TypedArrayPrototypeSome;
        private static readonly BuiltinFunction2 _typedArrayFindValue = TypedArrayPrototypeFind;
        private static readonly BuiltinFunction2 _typedArrayFindIndexValue = TypedArrayPrototypeFindIndex;
        private static readonly BuiltinFunction3 _typedArrayFromValue = TypedArrayFrom;
        private static readonly BuiltinFunctionVariadic _typedArrayOfValue = TypedArrayOf;

        internal static void ConfigureIntrinsicSurface(RuntimeIntrinsics intrinsics)
        {
            GlobalThis.ConfigureBuiltinFunctionObject(_typedArrayConstructorValue);
            // Resolve prototypes from the bootstrapping realm, never from static storage.
            var prototype = intrinsics.TypedArrayPrototype;
            PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
            PropertyDescriptorStore.DefineOrUpdate(_typedArrayConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototype
            });
            GlobalThis.DefineSpeciesAccessorProperty(_typedArrayConstructorValue);
            GlobalThis.DefineBuiltinFunctionProperty(_typedArrayConstructorValue, "from", _typedArrayFromValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(_typedArrayConstructorValue, "of", _typedArrayOfValue, 0d);
            PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
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
            PropertyDescriptorStore.DefineOrUpdate(prototype, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _typedArrayLengthGetterValue
            });
            DefineTypedArrayAccessor(prototype, "buffer", _typedArrayBufferGetterValue);
            DefineTypedArrayAccessor(prototype, "byteOffset", _typedArrayByteOffsetGetterValue);
            DefineTypedArrayAccessor(prototype, "byteLength", _typedArrayByteLengthGetterValue);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "at", _typedArrayAtValue, 1d);
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                _typedArrayToStringTagGetterValue,
                0d,
                "get [Symbol.toStringTag]",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_typedArrayToStringTagGetterValue));
            GlobalThis.DefineUndefinedPrototypeProperty(_typedArrayToStringTagGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(
                prototype,
                global::JavaScriptRuntime.Symbol.toStringTag.DebugId,
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = false,
                    Configurable = true,
                    Get = _typedArrayToStringTagGetterValue
                });
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "sort", _typedArraySortValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "toSorted", _typedArrayToSortedValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "with", _typedArrayWithValue, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString", _typedArrayToStringValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "toLocaleString", _typedArrayToLocaleStringValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "copyWithin", _typedArrayCopyWithinValue, 2d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "fill", _typedArrayFillValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "findLast", _typedArrayFindLastValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "findLastIndex", _typedArrayFindLastIndexValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "reduceRight", _typedArrayReduceRightValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "toReversed", _typedArrayToReversedValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "entries", _typedArrayEntriesValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "keys", _typedArrayKeysValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "values", _typedArrayValuesValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "map", _typedArrayMapValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "filter", _typedArrayFilterValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "every", _typedArrayEveryValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "some", _typedArraySomeValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "find", _typedArrayFindValue, 1d);
            GlobalThis.DefineBuiltinFunctionProperty(prototype, "findIndex", _typedArrayFindIndexValue, 1d);
            PropertyDescriptorStore.DefineOrUpdate(
                prototype,
                global::JavaScriptRuntime.Symbol.iterator.DebugId,
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = true,
                    Value = _typedArrayValuesValue
                });
            ConfigureTypedArrayConstructorValue(GlobalThis.Float64Array, 8d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Float32Array, 4d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Int32Array, 4d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Int16Array, 2d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Int8Array, 1d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Uint32Array, 4d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Uint16Array, 2d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Uint8Array, 1d);
            ConfigureTypedArrayConstructorValue(GlobalThis.Uint8ClampedArray, 1d);
            ConfigureTypedArrayConstructorValue(GlobalThis.BigInt64Array, 8d);
            ConfigureTypedArrayConstructorValue(GlobalThis.BigUint64Array, 8d);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Uint8Array, JavaScriptRuntime.Uint8Array.Prototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Uint8ClampedArray, JavaScriptRuntime.Uint8ClampedArray.Prototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Float64Array, intrinsics.Float64ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Float32Array, intrinsics.Float32ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Int32Array, intrinsics.Int32ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Int16Array, intrinsics.Int16ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Int8Array, intrinsics.Int8ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Uint32Array, intrinsics.Uint32ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.Uint16Array, intrinsics.Uint16ArrayPrototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.BigInt64Array, JavaScriptRuntime.BigInt64Array.Prototype);
            ConfigureTypedArrayInstancePrototype(prototype, GlobalThis.BigUint64Array, JavaScriptRuntime.BigUint64Array.Prototype);
            JavaScriptRuntime.Uint8Array.ConfigureIntrinsicSurface(GlobalThis.Uint8Array);
        }

        private static void ConfigureTypedArrayConstructorValue(object constructorValue, double bytesPerElement)
        {
            GlobalThis.ConfigureBuiltinFunctionObject(constructorValue);
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                constructorValue,
                3d,
                GetTypedArrayConstructorName(constructorValue));
            JavaScriptRuntime.Function.MarkConstructible(constructorValue);
            PrototypeChain.SetPrototype(constructorValue, _typedArrayConstructorValue);
            GlobalThis.DefineIntrinsicConstantDataProperty(constructorValue, "BYTES_PER_ELEMENT", bytesPerElement);
        }

        private static string GetTypedArrayConstructorName(object constructorValue)
        {
            if (ReferenceEquals(constructorValue, GlobalThis.Float64Array)) return nameof(Float64Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Float32Array)) return nameof(Float32Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Int32Array)) return nameof(Int32Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Int16Array)) return nameof(Int16Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Int8Array)) return nameof(Int8Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Uint32Array)) return nameof(Uint32Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Uint16Array)) return nameof(Uint16Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Uint8Array)) return nameof(Uint8Array);
            if (ReferenceEquals(constructorValue, GlobalThis.Uint8ClampedArray)) return nameof(Uint8ClampedArray);
            if (ReferenceEquals(constructorValue, GlobalThis.BigInt64Array)) return nameof(BigInt64Array);
            if (ReferenceEquals(constructorValue, GlobalThis.BigUint64Array)) return nameof(BigUint64Array);
            throw new ArgumentOutOfRangeException(nameof(constructorValue));
        }

        private static void ConfigureTypedArrayInstancePrototype(object typedArrayPrototype, object constructorValue, object prototypeValue)
        {
            PrototypeChain.SetPrototype(prototypeValue, typedArrayPrototype);
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

        private static void DefineTypedArrayAccessor(
            object prototype,
            string name,
            BuiltinFunction0 getter)
        {
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {name}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
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

        private static object? TypedArrayPrototypeMap(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.map called on incompatible receiver");
            }

            return typedArray.map(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayPrototypeFilter(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.filter called on incompatible receiver");
            }

            return typedArray.filter(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayPrototypeEvery(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.every called on incompatible receiver");
            }

            return typedArray.every(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayPrototypeSome(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.some called on incompatible receiver");
            }

            return typedArray.some(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayPrototypeFind(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.find called on incompatible receiver");
            }

            return typedArray.find(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayPrototypeFindIndex(object? thisArgument, object? callback, object? thisArg)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.findIndex called on incompatible receiver");
            }

            return typedArray.findIndex(new object?[] { callback, thisArg });
        }

        private static object? TypedArrayFrom(object? thisArgument, object? source, object? mapFn, object? thisArg)
        {
            if (!CallableOperations.IsConstructor(thisArgument))
            {
                throw new TypeError("%TypedArray%.from called on a value that is not a constructor");
            }

            return CreateFromSource(source, mapFn, thisArg, length => CreateTypedArrayResult(thisArgument, length));
        }

        private static TypedArrayBase CreateFromSource(
            object? source,
            object? mapFn,
            object? thisArg,
            Func<double, TypedArrayBase> create)
        {
            var mapping = mapFn is not null;
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

            List<object?>? values = null;
            double length;
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

                length = values.Count;
            }
            else
            {
                source = ObjectRuntime.Construct(source);
                length = ToArrayLikeLength(ObjectRuntime.GetItem(source, "length"));
            }

            var target = create(length);
            for (long i = 0; i < length; i++)
            {
                var sourceValue = values is null
                    ? ObjectRuntime.GetItem(source, (double)i)
                    : values[(int)i];
                var value = mapping
                    ? CallableOperations.Call2(mapFn, thisArg, sourceValue, (double)i)
                    : sourceValue;
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
            var target = CreateTypedArrayResult(thisArgument, items.Length);
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

        private static TypedArrayBase CreateTypedArrayResult(object? constructor, double length)
        {
            var result = CallableOperations.Construct1(constructor, constructor, length);
            if (result is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray constructor must return a TypedArray");
            }

            if (typedArray.GetCurrentLengthForIteration() < length)
            {
                throw new TypeError("TypedArray constructor returned an insufficiently sized TypedArray");
            }

            return typedArray;
        }

        private static double ToArrayLikeLength(object? lengthValue)
        {
            var number = TypeUtilities.ToNumber(lengthValue);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            return System.Math.Min(System.Math.Truncate(number), 9007199254740991d);
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

        private static object? TypedArrayPrototypeFill(object? thisArgument, in JsCallArguments arguments)
        {
            if (thisArgument is not TypedArrayBase typedArray)
            {
                throw new TypeError("TypedArray.prototype.fill called on incompatible receiver");
            }

            return typedArray.fill(arguments.ToArray());
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
    }
}
