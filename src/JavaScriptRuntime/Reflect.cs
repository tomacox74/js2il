using System.Collections;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Reflect")]
    public static class Reflect
    {
        public static object? apply(object? target, object? thisArgument, object? argumentsList)
        {
            if (!IsCallableValue(target))
            {
                throw new TypeError("Reflect.apply target is not a function");
            }

            return Function.Apply(target!, thisArgument, CreateListFromArrayLike(argumentsList, "Reflect.apply"));
        }

        public static object? construct(object? target, object? argumentsList, object? newTarget = null)
        {
            if (!ObjectRuntime.IsConstructibleValue(target))
            {
                throw new TypeError("Reflect.construct target is not a constructor");
            }

            newTarget ??= target;
            if (!ObjectRuntime.IsConstructibleValue(newTarget))
            {
                throw new TypeError("Reflect.construct newTarget is not a constructor");
            }

            return ObjectRuntime.ConstructValue(target!, NormalizeArgumentsList(argumentsList), newTarget);
        }

        public static bool defineProperty(object target, object? propertyKey, object? attributes)
            => ObjectRuntime.TryDefineProperty(target, propertyKey, attributes);

        public static bool deleteProperty(object target, object? propertyKey)
        {
            RequireObjectTarget(target, "deleteProperty");
            return ObjectRuntime.DeletePropertyNonStrict(target, propertyKey);
        }

        public static object? get(object target, object? propertyKey, object? receiver = null)
        {
            RequireObjectTarget(target, "get");
            return ObjectRuntime.ReflectGet(target, propertyKey, receiver ?? target);
        }

        public static object? getOwnPropertyDescriptor(object target, object? propertyKey)
        {
            RequireObjectTarget(target, "getOwnPropertyDescriptor");
            return ObjectRuntime.getOwnPropertyDescriptor(target, propertyKey);
        }

        public static object? getPrototypeOf(object target)
        {
            RequireObjectTarget(target, "getPrototypeOf");
            return ObjectRuntime.getPrototypeOf(target);
        }

        public static bool has(object target, object? propertyKey)
        {
            RequireObjectTarget(target, "has");
            return Operators.In(ObjectRuntime.ToExternalPropertyKey(ObjectRuntime.ToPropertyKeyString(propertyKey)), target);
        }

        public static bool isExtensible(object target)
        {
            RequireObjectTarget(target, "isExtensible");
            return ObjectRuntime.isExtensible(target);
        }

        public static bool preventExtensions(object target)
        {
            RequireObjectTarget(target, "preventExtensions");
            ObjectRuntime.preventExtensions(target);
            return true;
        }

        public static bool setPrototypeOf(object target, object? proto)
        {
            RequireObjectTarget(target, "setPrototypeOf");
            if (proto is not null && proto is not JsNull && !Proxy.IsObjectLikeValue(proto))
            {
                throw new TypeError("Reflect.setPrototypeOf proto must be an object or null");
            }

            return ObjectRuntime.ReflectSetPrototypeOf(target, proto);
        }

        public static bool set(object target, object? propertyKey, object? value)
        {
            if (!Proxy.IsObjectLikeValue(target))
            {
                throw new TypeError("Reflect.set target must be an object");
            }

            return ObjectRuntime.ReflectSet(target, propertyKey, value);
        }

        public static object ownKeys(object target)
        {
            if (!Proxy.IsObjectLikeValue(target))
            {
                throw new TypeError("Reflect.ownKeys target must be an object");
            }

            return new Array(
                ObjectRuntime.GetOwnPropertyKeysInOrder(target, includeEncodedSymbolKeys: true)
                    .Select(ObjectRuntime.ToExternalPropertyKey));
        }

        private static void RequireObjectTarget(object? target, string methodName)
        {
            if (!Proxy.IsObjectLikeValue(target))
            {
                throw new TypeError($"Reflect.{methodName} called on non-object");
            }
        }

        private static bool IsCallableValue(object? value)
            => CallableOperations.IsCallable(value);

        /// <summary>
        /// ECMA-262 CreateListFromArrayLike: reads <c>length</c> and every index, so holes
        /// surface as <c>undefined</c> and the resulting list keeps the array-like length.
        /// </summary>
        private static object?[] CreateListFromArrayLike(object? argumentsList, string methodName)
        {
            if (!Proxy.IsObjectLikeValue(argumentsList))
            {
                throw new TypeError($"{methodName} argumentsList must be an object");
            }

            var length = ToLength(ObjectRuntime.GetItem(argumentsList!, "length"));
            if (length > int.MaxValue)
            {
                throw new TypeError($"{methodName} argumentsList is too large");
            }

            var list = new object?[(int)length];
            for (var index = 0; index < list.Length; index++)
            {
                list[index] = ObjectRuntime.GetItem(
                    argumentsList!,
                    index.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }

            return list;
        }

        /// <summary>ECMA-262 ToLength.</summary>
        private static double ToLength(object? value)
        {
            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            return global::System.Math.Min(global::System.Math.Truncate(number), 9007199254740991d);
        }

        private static object[] NormalizeArgumentsList(object? argumentsList)
        {
            if (argumentsList is null || argumentsList is JsNull)
            {
                throw new TypeError("Reflect.construct argumentsList must be an object");
            }

            if (argumentsList is Array jsArray)
            {
                return jsArray.ToArray().Cast<object>().ToArray();
            }

            if (argumentsList is object[] objectArray)
            {
                return objectArray;
            }

            if (argumentsList is IEnumerable enumerable && argumentsList is not string)
            {
                var list = new List<object>();
                foreach (var item in enumerable)
                {
                    list.Add(item!);
                }

                return list.ToArray();
            }

            throw new TypeError("Reflect.construct argumentsList must be array-like");
        }
    }
}
