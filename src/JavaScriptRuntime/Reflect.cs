using System.Collections;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Reflect")]
    public static class Reflect
    {
        public static object? construct(object? target, object? argumentsList, object? newTarget = null)
        {
            if (!Object.IsConstructibleValue(target))
            {
                throw new TypeError("Reflect.construct target is not a constructor");
            }

            newTarget ??= target;
            if (!Object.IsConstructibleValue(newTarget))
            {
                throw new TypeError("Reflect.construct newTarget is not a constructor");
            }

            return Object.ConstructValue(target!, NormalizeArgumentsList(argumentsList), newTarget);
        }

        public static bool defineProperty(object target, object? propertyKey, object? attributes)
        {
            Object.defineProperty(target, propertyKey, attributes);
            return true;
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
