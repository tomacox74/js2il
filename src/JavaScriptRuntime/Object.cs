namespace JavaScriptRuntime
{
    [IntrinsicObject("Object", IntrinsicCallKind.ObjectConstruct)]
    public class Object
    {
        public static bool @is(object? value1, object? value2)
            => ObjectRuntime.@is(value1, value2);

        public static object? getPrototypeOf(object obj)
            => ObjectRuntime.getPrototypeOf(obj);

        public static object setPrototypeOf(object obj, object? prototype)
            => ObjectRuntime.setPrototypeOf(obj, prototype);

        public static object create(object? prototype)
            => ObjectRuntime.create(prototype);

        public static object create(object? prototype, object? properties)
            => ObjectRuntime.create(prototype, properties);

        public static object? getOwnPropertyDescriptor(object obj, object? prop)
            => ObjectRuntime.getOwnPropertyDescriptor(obj, prop);

        public static object getOwnPropertyNames(object obj)
            => ObjectRuntime.getOwnPropertyNames(obj);

        public static object keys(object obj)
            => ObjectRuntime.keys(obj);

        public static object values(object obj)
            => ObjectRuntime.values(obj);

        public static object entries(object obj)
            => ObjectRuntime.entries(obj);

        public static object assign(object target)
            => ObjectRuntime.assign(target);

        public static object assign(object target, object? source)
            => ObjectRuntime.assign(target, source);

        public static object assign(object target, params object?[] sources)
            => ObjectRuntime.assign(target, sources);

        public static object fromEntries(object iterable)
            => ObjectRuntime.fromEntries(iterable);

        public static object defineProperty(object obj, object? prop, object? attributes)
            => ObjectRuntime.defineProperty(obj, prop, attributes);

        public static object defineProperties(object obj, object? properties)
            => ObjectRuntime.defineProperties(obj, properties);

        public static bool hasOwn(object obj, object? prop)
            => ObjectRuntime.hasOwn(obj, prop);

        public static object getOwnPropertyDescriptors(object obj)
            => ObjectRuntime.getOwnPropertyDescriptors(obj);

        public static object getOwnPropertySymbols(object obj)
            => ObjectRuntime.getOwnPropertySymbols(obj);

        public static object preventExtensions(object obj)
            => ObjectRuntime.preventExtensions(obj);

        public static bool isExtensible(object obj)
            => ObjectRuntime.isExtensible(obj);

        public static object seal(object obj)
            => ObjectRuntime.seal(obj);

        public static object freeze(object obj)
            => ObjectRuntime.freeze(obj);

        public static bool isSealed(object obj)
            => ObjectRuntime.isSealed(obj);

        public static bool isFrozen(object obj)
            => ObjectRuntime.isFrozen(obj);

        public static object groupBy(object items, object callback)
            => ObjectRuntime.groupBy(items, callback);

        public static object Construct()
            => ObjectRuntime.Construct();

        public static object Construct(object? value)
            => ObjectRuntime.Construct(value);
    }
}
