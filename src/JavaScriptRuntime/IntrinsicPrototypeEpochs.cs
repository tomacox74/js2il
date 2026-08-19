using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>
/// Identifies an intrinsic prototype family whose default lookup chain can be
/// guarded by generated code.
/// </summary>
public enum IntrinsicPrototypeFamily
{
    String = 0
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
}
