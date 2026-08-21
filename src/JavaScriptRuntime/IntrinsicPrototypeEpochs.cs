using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>
/// Identifies an intrinsic prototype family whose default lookup chain can be
/// guarded by generated code.
/// </summary>
public enum IntrinsicPrototypeFamily
{
    String = 0,
    Array = 1,
    TypedArray = 2
}

/// <summary>
/// Allocation-free access to realm-owned intrinsic prototype mutation epochs.
/// </summary>
public static class IntrinsicPrototypeEpochs
{
    public const long PristineEpoch = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Read(IntrinsicPrototypeFamily family)
        => RuntimeIntrinsics.Current.ReadPrototypeMutationEpoch(family);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPristine(
        IntrinsicPrototypeFamily family)
        => IsCurrent(family, PristineEpoch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCurrent(
        IntrinsicPrototypeFamily family,
        long expectedEpoch)
        => Read(family) == expectedEpoch;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasDefaultPrototype(
        object receiver,
        IntrinsicPrototypeFamily family)
    {
        var expectedPrototype = family switch
        {
            IntrinsicPrototypeFamily.Array
                when receiver is Array => Array.Prototype,
            IntrinsicPrototypeFamily.TypedArray
                when receiver is TypedArrayBase typedArray =>
                    GlobalThis.GetTypedArrayInstancePrototype(typedArray),
            _ => null
        };

        return expectedPrototype != null
            && ReferenceEquals(
                PrototypeChain.GetPrototypeOrNull(receiver),
                expectedPrototype);
    }

    public static string? TryUnwrapStringObjectReceiver(
        object receiver,
        string memberName)
    {
        if (receiver is not JsObject
            || !ReferenceEquals(
                PrototypeChain.GetPrototypeOrNull(receiver),
                String.Prototype)
            || ObjectRuntime.HasOwnIntrinsicMemberOverride(
                receiver,
                memberName)
            || !PropertyDescriptorStore.TryGetOwn(
                receiver,
                String.StringDataPropertyName,
                out var descriptor)
            || descriptor.Kind != JsPropertyDescriptorKind.Data
            || descriptor.Value is not string value)
        {
            return null;
        }

        return value;
    }
}
