namespace JavaScriptRuntime;

/// <summary>
/// Minimal synchronous stack identity used by resource-management APIs.
/// Its complete disposal surface is intentionally separate from
/// <see cref="AsyncDisposableStack"/>.
/// </summary>
[IntrinsicObject("DisposableStack")]
public sealed class DisposableStack : JsObject
{
    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.DisposableStackPrototype,
            static () => new JsObject());

    public DisposableStack()
    {
        PrototypeChain.InitializePrototype(this, Prototype);
    }

    internal static void InitializeIntrinsicSurface(object objectPrototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        GlobalThis.ConfigureBuiltinFunctionObject(typeof(DisposableStack));
        PrototypeChain.SetPrototype(Prototype, objectPrototype);
        DefineDataProperty(typeof(DisposableStack), "prototype", Prototype, configurable: false, writable: false);
        DefineDataProperty(typeof(DisposableStack), "name", "DisposableStack", configurable: true, writable: false);
        DefineDataProperty(typeof(DisposableStack), "length", 0d, configurable: true, writable: false);
        DefineDataProperty(Prototype, "constructor", typeof(DisposableStack));
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "DisposableStack", configurable: true, writable: false);
    }

    private static void DefineDataProperty(
        object target,
        string key,
        object? value,
        bool configurable = true,
        bool writable = true)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = configurable,
            Writable = writable,
            Value = value
        });
    }
}
