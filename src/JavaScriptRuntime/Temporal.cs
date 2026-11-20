using System.Numerics;

namespace JavaScriptRuntime;

public static partial class Temporal
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
        GlobalThis.DefineIntrinsicDataProperty(value, "PlainTime", GetPlainTimeConstructor(intrinsics));
        GlobalThis.DefineIntrinsicDataProperty(value, "Duration", GetDurationConstructor(intrinsics));
        GlobalThis.DefineIntrinsicDataProperty(value, "PlainDate", GetPlainDateConstructor(intrinsics));
        ConfigureCalendarBearingTypes(value, intrinsics);
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
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "from",
                    (BuiltinFunction1)(static (_, value) => new TemporalInstant(ToTemporalInstant(value).EpochNanoseconds)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "fromEpochNanoseconds",
                    (BuiltinFunction1)(static (_, value) => new TemporalInstant(BigInt.ToBigInt(value))), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "fromEpochMilliseconds",
                    (BuiltinFunction1)(static (_, value) =>
                        new TemporalInstant(new BigInteger(ToIntegerIfIntegral(value)) * 1_000_000)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(constructor, "compare",
                    (BuiltinFunction2)(static (_, left, right) =>
                    {
                        var first = ToTemporalInstant(left);
                        var second = ToTemporalInstant(right);
                        return (double)System.Math.Sign(first.EpochNanoseconds.CompareTo(second.EpochNanoseconds));
                    }), 2d);
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
                DefineTemporalGetter(prototype, "epochMilliseconds",
                    receiver => (double)FloorDivide(RequireInstant(receiver).EpochNanoseconds, 1_000_000));
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "equals",
                    (BuiltinFunction1)(static (receiver, other) =>
                        RequireInstant(receiver).EpochNanoseconds == ToTemporalInstant(other).EpochNanoseconds), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "add",
                    (BuiltinFunction1)(static (receiver, duration) => AddInstant(receiver, duration, false)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "subtract",
                    (BuiltinFunction1)(static (receiver, duration) => AddInstant(receiver, duration, true)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "since",
                    (BuiltinFunction2)(static (receiver, other, options) => DifferenceInstant(receiver, other, options, true)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "until",
                    (BuiltinFunction2)(static (receiver, other, options) => DifferenceInstant(receiver, other, options, false)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "round",
                    (BuiltinFunction1)(static (receiver, options) => RoundInstant(receiver, options)), 1d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toString",
                    (BuiltinFunction1)(static (receiver, options) => FormatInstant(RequireInstant(receiver), options)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toJSON",
                    (BuiltinFunction0)(static receiver => FormatInstant(RequireInstant(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "toLocaleString",
                    (BuiltinFunction0)(static receiver => FormatInstant(RequireInstant(receiver), null)), 0d);
                GlobalThis.DefineBuiltinFunctionProperty(prototype, "valueOf",
                    (BuiltinFunction0)(static _ => throw new TypeError("Cannot convert Temporal.Instant to a primitive value")), 0d);
            });

    private sealed class InstantConstructor : JsFunctionObject
    {
        public override bool IsConstructor => true;
        internal override bool SelectsConstructedPrototype => true;

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
