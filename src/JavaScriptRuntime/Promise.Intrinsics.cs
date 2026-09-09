namespace JavaScriptRuntime;

public sealed partial class Promise
{
    private static readonly BuiltinFunction1 _promiseResolveValue = static (thisArgument, value) =>
        ResolveForConstructor(thisArgument, value);
    private static readonly BuiltinFunction1 _promiseAllValue = static (thisArgument, iterable) =>
        AllForConstructor(thisArgument, iterable);
    private static readonly BuiltinFunction1 _promiseAllSettledValue = static (thisArgument, iterable) =>
        AllSettledForConstructor(thisArgument, iterable);
    private static readonly BuiltinFunction1 _promiseAnyValue = static (thisArgument, iterable) =>
        AnyForConstructor(thisArgument, iterable);
    private static readonly BuiltinFunction1 _promiseAllKeyedValue = static (thisArgument, dictionary) =>
        AllKeyedForConstructor(thisArgument, dictionary);
    private static readonly BuiltinFunction1 _promiseAllSettledKeyedValue = static (thisArgument, dictionary) =>
        AllSettledKeyedForConstructor(thisArgument, dictionary);
    private static readonly BuiltinFunction1 _promiseRaceValue = static (_, iterable) => race(iterable);
    private static readonly BuiltinFunction1 _promiseRejectValue = static (_, reason) => reject(reason);
    private static readonly BuiltinFunctionVariadic _promiseTryValue = static (thisArgument, in arguments) =>
    {
        var callback = arguments.Count > 0 ? arguments.GetArgument(0) : null;
        var callbackArgs = arguments.Count > 1
            ? arguments.ToArray().Skip(1).ToArray()
            : System.Array.Empty<object?>();

        return TryForConstructor(thisArgument, callback, callbackArgs);
    };

    // Keep the early prototype/species setup separate to preserve realm bootstrap order.
    internal static void ConfigureIntrinsicPrototype(object constructorValue, RuntimeIntrinsics intrinsics)
    {
        var prototypeValue = intrinsics.GlobalPromisePrototype;
        GlobalThis.ConfigureBuiltinFunctionObject(constructorValue);
        Function.MarkConstructible(constructorValue);
        PrototypeChain.SetPrototype(prototypeValue, intrinsics.ObjectPrototype);
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = prototypeValue
        });
        GlobalThis.DefineSpeciesAccessorProperty(constructorValue);
        DefineDataProperty(prototypeValue, "constructor", constructorValue);
    }

    internal static void ConfigureIntrinsicSurface(object constructorValue, RuntimeIntrinsics intrinsics)
    {
        var prototypeValue = Prototype;
        GlobalThis.ConfigureBuiltinFunctionObject(constructorValue);
        Function.MarkConstructible(constructorValue);
        PrototypeChain.SetPrototype(prototypeValue, intrinsics.ObjectPrototype);
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = prototypeValue
        });
        DefineDataProperty(prototypeValue, "constructor", constructorValue);
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, "length", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = false,
            Value = 1d
        });
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, "name", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = false,
            Value = "Promise"
        });
        GlobalThis.ConfigureBuiltinFunctionObject(_promiseResolveValue);
        GlobalThis.DefineUndefinedPrototypeProperty(_promiseResolveValue);
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
        DefineDataProperty(constructorValue, "resolve", _promiseResolveValue);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "all", _promiseAllValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "allSettled", _promiseAllSettledValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "allKeyed", _promiseAllKeyedValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "allSettledKeyed", _promiseAllSettledKeyedValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "any", _promiseAnyValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "race", _promiseRaceValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "reject", _promiseRejectValue, 1d);
        GlobalThis.DefineBuiltinFunctionProperty(constructorValue, "try", _promiseTryValue, 1d);
    }
}
