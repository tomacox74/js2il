namespace Jroc.SymbolTables;

public enum CallableMaterializationKind
{
    DirectOnly,
    IdentityObservable,
    UnknownMaterialize
}

[Flags]
public enum CallableMaterializationReason
{
    None = 0,
    MutableOrHoistedBinding = 1 << 0,
    Reassigned = 1 << 1,
    AsyncOrGenerator = 1 << 2,
    NamedFunctionExpression = 1 << 3,
    InvocationContextRequired = 1 << 4,
    WithEnvironment = 1 << 5,
    InitializationNotProven = 1 << 6,
    OptionalCall = 1 << 7,
    SpreadCall = 1 << 8,
    NonCallRead = 1 << 9,
    Alias = 1 << 10,
    Export = 1 << 11,
    Return = 1 << 12,
    PropertyStorage = 1 << 13,
    ArrayStorage = 1 << 14,
    UnknownArgument = 1 << 15,
    CallApplyBind = 1 << 16,
    Reflection = 1 << 17,
    CapturedValueRead = 1 << 18,
    RecursiveReference = 1 << 19,
    MutuallyRecursiveScc = 1 << 20,
    UnboundEvaluation = 1 << 21
}

public sealed record CallableMaterializationDecision(
    CallableMaterializationKind Kind,
    CallableMaterializationReason Reasons,
    int RuntimeUseCount,
    int DirectCallCount)
{
    public static CallableMaterializationDecision UnboundEvaluation { get; } = new(
        CallableMaterializationKind.UnknownMaterialize,
        CallableMaterializationReason.UnboundEvaluation,
        RuntimeUseCount: 0,
        DirectCallCount: 0);

    public string ToDiagnosticText()
        => $"{Kind}; uses={RuntimeUseCount}; direct-calls={DirectCallCount}; reasons={FormatReasons(Reasons)}";

    private static string FormatReasons(CallableMaterializationReason reasons)
    {
        if (reasons == CallableMaterializationReason.None)
        {
            return "None";
        }

        return string.Join(
            "|",
            Enum.GetValues<CallableMaterializationReason>()
                .Where(reason => reason != CallableMaterializationReason.None
                    && reasons.HasFlag(reason)));
    }
}
