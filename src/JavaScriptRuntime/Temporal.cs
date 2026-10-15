using System.Numerics;

namespace JavaScriptRuntime;

public static class Temporal
{
    internal static JsObject GetNamespace(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.Temporal,
            static () => new JsObject(),
            value => ConfigureIntrinsicSurface(value, intrinsics));

    private static void ConfigureIntrinsicSurface(JsObject value, RuntimeIntrinsics intrinsics)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
        PrototypeChain.SetPrototype(value, intrinsics.ObjectPrototype);
        GlobalThis.DefineIntrinsicToStringTagProperty(value, "Temporal");
        GlobalThis.DefineIntrinsicDataProperty(value, "Instant", GetInstantConstructor(intrinsics));
    }

    private static InstantConstructor GetInstantConstructor(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalInstantConstructor,
            static () => new InstantConstructor(),
            constructor =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                Function.InitializeFunctionInstance(constructor, 1d, "Instant", false);
                PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = GetInstantPrototype(intrinsics),
                    Writable = false,
                    Enumerable = false,
                    Configurable = false
                });
            });

    internal static JsObject GetInstantPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.TemporalInstantPrototype,
            static () => new JsObject(),
            prototype =>
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                PrototypeChain.SetPrototype(prototype, intrinsics.ObjectPrototype);
                GlobalThis.DefineIntrinsicToStringTagProperty(prototype, "Temporal.Instant");
                GlobalThis.DefineIntrinsicDataProperty(prototype, "constructor",
                    GetInstantConstructor(intrinsics));
                BuiltinFunction0 getter = static receiver => receiver is TemporalInstant instant
                    ? instant.EpochNanoseconds
                    : throw new TypeError("Temporal.Instant getter called on incompatible receiver");
                Function.InitializeFunctionInstance(getter, 0d, "get epochNanoseconds", false);
                Function.MarkUndefinedPrototype(getter);
                PropertyDescriptorStore.DefineOrUpdate(prototype, "epochNanoseconds", new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Get = getter,
                    Enumerable = false,
                    Configurable = true
                });
            });

    private sealed class InstantConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Temporal.Instant requires 'new'");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            var nanoseconds = BigInt.ToBigInt(arguments.GetArgument(0));
            var instant = new TemporalInstant(nanoseconds);
            var prototype = ObjectRuntime.GetItem(newTarget!, "prototype");
            if (!TypeUtilities.IsConstructorReturnOverride(prototype))
            {
                prototype = GetInstantPrototype(RuntimeIntrinsics.GetFunctionRealm(newTarget));
            }
            PrototypeChain.SetPrototype(instant, prototype);
            return instant;
        }
    }
}

public sealed class TemporalInstant : JsObject
{
    internal BigInteger EpochNanoseconds { get; }

    internal TemporalInstant(BigInteger nanoseconds)
    {
        if (BigInteger.Abs(nanoseconds) > new BigInteger(8_640_000_000_000_000L) * 1_000_000)
        {
            throw new RangeError("Temporal.Instant is outside the supported range");
        }
        EpochNanoseconds = nanoseconds;
        PrototypeChain.InitializePrototype(this, Temporal.GetInstantPrototype(RuntimeIntrinsics.Current));
    }
}
