namespace Jroc.SymbolTables;

[Flags]
internal enum IntrinsicGuardEffects
{
    None = 0,
    MutatesIntrinsicPrototypeOrLink = 1 << 0,
    DefinesDeletesOrReconfiguresProperties = 1 << 1,
    InvokesUnknownOrEscapedCode = 1 << 2,
    MaySuspendOrYield = 1 << 3,
    EscapesGuardedValue = 1 << 4
}

internal readonly record struct IntrinsicGuardEffectSummary(
    IntrinsicGuardEffects Effects)
{
    public static IntrinsicGuardEffectSummary None { get; } =
        new(IntrinsicGuardEffects.None);

    public bool IsGuardHoistSafe =>
        Effects == IntrinsicGuardEffects.None;

    public IntrinsicGuardEffectSummary Union(
        IntrinsicGuardEffectSummary other)
        => new(Effects | other.Effects);
}
