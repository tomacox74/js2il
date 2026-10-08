using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jroc.DebugSymbols;
using Jroc.IL;
using Jroc.IR;

namespace Jroc;

public sealed record CompilationCoverageCounts(
    [property: JsonPropertyName("directIl")] int DirectIl,
    [property: JsonPropertyName("runtimeIntrinsic")] int RuntimeIntrinsic,
    [property: JsonPropertyName("runtimeDispatch")] int RuntimeDispatch,
    [property: JsonPropertyName("unsupported")] int Unsupported)
{
    [JsonPropertyName("total")]
    public int Total => DirectIl + RuntimeIntrinsic + RuntimeDispatch + Unsupported;
}

public sealed record CompilationCoverageSite(
    [property: JsonPropertyName("file")] string File,
    [property: JsonPropertyName("line")] int Line,
    [property: JsonPropertyName("column")] int Column,
    [property: JsonPropertyName("endLine")] int EndLine,
    [property: JsonPropertyName("endColumn")] int EndColumn,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("reasons")] IReadOnlyList<string> Reasons);

public sealed record CompilationCoverageReport(
    [property: JsonPropertyName("compilationSucceeded")] bool CompilationSucceeded,
    [property: JsonPropertyName("counts")] CompilationCoverageCounts Counts,
    [property: JsonPropertyName("sites")] IReadOnlyList<CompilationCoverageSite> Sites,
    [property: JsonPropertyName("diagnostics")] IReadOnlyList<string> Diagnostics)
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion => 1;

    [JsonPropertyName("measurement")]
    public string Measurement => "statement-source-sites-v1";

    [JsonPropertyName("complete")]
    public bool Complete => CompilationSucceeded;

    [JsonPropertyName("percentages")]
    public IReadOnlyDictionary<string, double> Percentages => new Dictionary<string, double>
    {
        ["directIl"] = Percent(Counts.DirectIl),
        ["runtimeIntrinsic"] = Percent(Counts.RuntimeIntrinsic),
        ["runtimeDispatch"] = Percent(Counts.RuntimeDispatch),
        ["unsupported"] = Percent(Counts.Unsupported)
    };

    private double Percent(int count)
        => Counts.Total == 0 ? 0 : 100d * count / Counts.Total;

    public string ToJson()
        => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

    public string ToText()
    {
        var text = new StringBuilder("JROC compilation coverage\n");
        text.AppendLine($"Measurement: {Measurement}");
        if (!Complete)
        {
            text.AppendLine("Partial report: compilation failed; uncompiled source is not counted as direct IL.");
        }
        text.AppendLine();
        Append("Direct / optimized IL", Counts.DirectIl);
        Append("Bound runtime intrinsic", Counts.RuntimeIntrinsic);
        Append("Runtime / dynamic dispatch", Counts.RuntimeDispatch);
        Append("Unsupported", Counts.Unsupported);
        text.AppendLine($"Total source sites: {Counts.Total}");
        foreach (var site in Sites.Where(site => site.Mode != "directIl"))
        {
            text.AppendLine($"{site.File}:{site.Line}:{site.Column}  {site.Mode}  {string.Join("; ", site.Reasons)}");
        }
        foreach (var diagnostic in Diagnostics)
        {
            text.AppendLine($"Diagnostic: {diagnostic}");
        }
        return text.ToString();

        void Append(string label, int count)
            => text.AppendLine(string.Create(CultureInfo.InvariantCulture,
                $"{label,-28} {Percent(count),6:F2}% ({count})"));
    }
}

public sealed record CompilationCoverageAnalysis(
    JrocCompiledAssemblyArtifact? Artifact,
    CompilationCoverageReport Report);

internal enum CompilationMode
{
    DirectIl,
    RuntimeIntrinsic,
    RuntimeDispatch,
    Unsupported
}

internal sealed class CompilationCoverageCollector
{
    private sealed class SiteState
    {
        public CompilationMode Mode;
        public HashSet<string> Reasons { get; } = new(StringComparer.Ordinal);
    }

    private readonly Dictionary<SourceSpan, SiteState> _sites = new();
    private readonly HashSet<string> _diagnostics = new(StringComparer.Ordinal);

    internal void RecordMethod(MethodBodyIR body)
    {
        SourceSpan? current = null;
        foreach (var instruction in body.Instructions)
        {
            if (instruction is LIRSequencePoint point)
            {
                current = point.Span.IsHidden ? null : point.Span;
            }
            else if (current is { } span && instruction is not LIRLabel)
            {
                var (mode, reason) = CompilationCoverageClassifier.Classify(instruction);
                Record(span, mode, reason);
            }
        }
    }

    internal void RecordUnsupported(SourceSpan span, string reason)
        => Record(span, CompilationMode.Unsupported, reason);

    internal void RecordDiagnostic(string reason) => _diagnostics.Add(reason);

    private void Record(SourceSpan span, CompilationMode mode, string reason)
    {
        if (!_sites.TryGetValue(span, out var state))
        {
            state = new SiteState { Mode = mode };
            _sites.Add(span, state);
        }
        if (mode > state.Mode)
        {
            state.Mode = mode;
            state.Reasons.Clear();
        }
        if (mode == state.Mode)
        {
            state.Reasons.Add(reason);
        }
    }

    internal CompilationCoverageReport CreateReport(bool succeeded)
    {
        var sites = _sites.OrderBy(item => item.Key.Document, StringComparer.Ordinal)
            .ThenBy(item => item.Key.Start.Line).ThenBy(item => item.Key.Start.Column)
            .ThenBy(item => item.Key.End.Line).ThenBy(item => item.Key.End.Column)
            .Select(item => new CompilationCoverageSite(
                item.Key.Document, item.Key.Start.Line, item.Key.Start.Column,
                item.Key.End.Line, item.Key.End.Column, ModeName(item.Value.Mode),
                item.Value.Reasons.Order(StringComparer.Ordinal).ToArray())).ToArray();
        return new CompilationCoverageReport(succeeded,
            new CompilationCoverageCounts(
                sites.Count(site => site.Mode == "directIl"),
                sites.Count(site => site.Mode == "runtimeIntrinsic"),
                sites.Count(site => site.Mode == "runtimeDispatch"),
                sites.Count(site => site.Mode == "unsupported")),
            sites, _diagnostics.Order(StringComparer.Ordinal).ToArray());
    }

    private static string ModeName(CompilationMode mode) => mode switch
    {
        CompilationMode.DirectIl => "directIl",
        CompilationMode.RuntimeIntrinsic => "runtimeIntrinsic",
        CompilationMode.RuntimeDispatch => "runtimeDispatch",
        CompilationMode.Unsupported => "unsupported",
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}

internal static class CompilationCoverageClassifier
{
    internal static (CompilationMode Mode, string Reason) Classify(LIRInstruction instruction)
    {
        var operation = instruction.GetType().Name;
        if (instruction is LIRAddDynamic or LIRAddDynamicDoubleObject or LIRAddDynamicObjectDouble
            or LIRAddAndToNumber or LIRMulDynamic or LIRNegateNumberDynamic
            or LIRBitwiseNotDynamic or LIRBinaryDynamicOperator
            or LIREqualDynamic or LIRNotEqualDynamic or LIRStrictEqualDynamic or LIRStrictNotEqualDynamic)
        {
            return (CompilationMode.RuntimeDispatch, $"Generic JavaScript operator/coercion: {operation}.");
        }
        if (instruction is LIRCallTypedMemberWithFallback or LIRCallGuardedIntrinsicMember
            or LIRCallGuardedStringIntrinsic or LIRGetGuardedInferredMember or LIRSetGuardedInferredMember
            or LIRCallNodeModuleContractMember { RequiresOverrideGuard: true })
        {
            return (CompilationMode.RuntimeDispatch, $"Guarded specialization retains a runtime fallback: {operation}.");
        }
        if (instruction is LIRGetItem or LIRGetItemAsNumber or LIRGetItemAsNumberString
            or LIRSetItem or LIRGetLength or LIRInOperator or LIRInstanceOfOperator
            or LIRConstructValue or LIRConstructValueFixed
            or LIRCallFunctionValue or LIRCallFunctionValue0 or LIRCallFunctionValue1
            or LIRCallFunctionValue2 or LIRCallFunctionValue3 or LIRCallFunctionValue4 or LIRCallFunctionValue5
            or LIRCallMember or LIRCallMember0 or LIRCallMember1 or LIRCallMember2
            or LIRCallMember3 or LIRCallMember4 or LIRCallMember5 or LIRCallComputedMemberFixed
            or LIRConvertToNumber or LIRConvertToNumberDiscard or LIRConvertToString
            or LIRConvertToStringDiscard or LIRConvertToBoolean or LIRCallIsTruthy
            or LIRCallFunctionBaseConstructor or LIRAwait or LIRYield
            or LIRLoadScopeFieldByName or LIRStoreScopeFieldByName
            or LIRLogicalNot or LIRCallFunctionWithArgsArray { CallableId: null }
            or LIRTypeof or LIRCallRuntimeServicesStatic or LIRCallRequire or LIRCallImport)
        {
            return (CompilationMode.RuntimeDispatch, $"Runtime-selected target, property, conversion or generic helper: {operation}.");
        }
        if (instruction is LIRCallDeclaredCallable or LIRCallFunction or LIRTailCallFunctionReturn
            or LIRCallFunctionWithArgsArray or LIRCallTypedMember or LIRCallUserClassInstanceMethod
            or LIRCallUserClassBaseInstanceMethod or LIRCallUserClassBaseConstructor
            or LIRNewUserClass)
        {
            return (CompilationMode.DirectIl, $"Statically bound generated callable: {operation}.");
        }
        if (!LIRInstructionInfo.IsKnownInstructionType(instruction.GetType()))
        {
            return (CompilationMode.RuntimeDispatch, $"Unclassified operation conservatively treated as generic: {operation}.");
        }
        if (instruction is LIRLoadLeafScopeField { Binding.RequiresRuntimeTemporalDeadZoneChecks: true }
            or LIRLoadParentScopeField { Binding.RequiresRuntimeTemporalDeadZoneChecks: true }
            or LIRLoadScopeField { Binding.RequiresRuntimeTemporalDeadZoneChecks: true })
        {
            return (CompilationMode.RuntimeIntrinsic, $"Runtime temporal-dead-zone check for a statically bound variable: {operation}.");
        }
        if (instruction is LIRConstNumber or LIRConstString or LIRConstBoolean or LIRConstNull or LIRConstUndefined
            or LIRAddNumber or LIRSubNumber or LIRMulNumber or LIRDivNumber or LIRModNumber or LIRNegateNumber
            or LIRCompareNumberEqual or LIRCompareNumberNotEqual or LIRCompareNumberLessThan
            or LIRCompareNumberLessThanOrEqual or LIRCompareNumberGreaterThan or LIRCompareNumberGreaterThanOrEqual
            or LIRCompareBooleanEqual or LIRCompareBooleanNotEqual or LIRCopyTemp or LIRConvertToObject
            or LIRLoadParameter or LIRStoreParameter or LIRLoadScopesArgument or LIRLoadThis
            or LIRLoadLeafScopeField or LIRStoreLeafScopeField or LIRLoadParentScopeField or LIRStoreParentScopeField
            or LIRLoadScopeField or LIRStoreScopeField
            or LIRBranch or LIRBranchIfTrue or LIRBranchIfFalse or LIRLeave or LIREndFinally
            or LIRReturn or LIRReturnUndefinedImmediate or LIRThrow or LIRStoreException)
        {
            return (CompilationMode.DirectIl, $"Direct IL operation: {operation}.");
        }
        if (instruction is LIRArrayAdd or LIRArrayPushRange
            or LIRAsyncCallMoveNext or LIRAsyncInitialize or LIRAsyncLoadAwaitedResult or LIRAsyncLoadState
            or LIRAsyncReject or LIRAsyncResolve or LIRAsyncReturnPromise or LIRAsyncStateSwitch
            or LIRAsyncStoreAwaitedResult or LIRAsyncStoreState or LIRGeneratorStateSwitch
            or LIRBitwiseAnd or LIRBitwiseNotNumber or LIRBitwiseOr or LIRBitwiseXor
            or LIRLeftShift or LIRRightShift or LIRUnsignedRightShift
            or LIRBuildArray or LIRBuildScopesArray or LIRConcatStrings or LIRExpNumber
            or LIRCallInstanceMethod or LIRCallIntrinsic or LIRCallIntrinsicBaseConstructor
            or LIRCallIntrinsicGlobalFunction or LIRCallIntrinsicStatic or LIRCallIntrinsicStaticVoid
            or LIRCallIntrinsicStaticVoidWithArgsArray or LIRCallIntrinsicStaticWithArgsArray
            or LIRCallIsTruthyBool or LIRCallIsTruthyDouble or LIRCallNodeModuleContractMember
            or LIRCaptureIntrinsicPrototypeAssumption
            or LIRCreateBoundArrowFunction or LIRCreateBoundFunctionExpression
            or LIRCreateLeafScopeInstance or LIRCreateScopeInstance
            or LIRGetInferredMember or LIRSetInferredMember or LIRNewInferredJsObject
            or LIRGetInt32ArrayElement or LIRGetInt32ArrayLength or LIRSetInt32ArrayElement
            or LIRGetJsArrayElement or LIRGetJsArrayLength or LIRSetJsArrayElement or LIRSetJsArrayLength
            or LIRGetIntrinsicGlobal or LIRGetIntrinsicGlobalFunction or LIRGetStringLength or LIRGetUserClassType
            or LIRIsInstanceOf or LIRPrivateBrandCheck or LIRLoadPrivateReceiverField or LIRStorePrivateReceiverField
            or LIRLoadNewTarget or LIRLoadUserClassInstanceField or LIRLoadUserClassStaticField
            or LIRStoreUserClassInstanceField or LIRStoreUserClassStaticField
            or LIRNewBuiltInError or LIRNewIntrinsicObject or LIRNewJsArray or LIRNewJsObject
            or LIRThrowNewTypeError or LIRUnwrapCatchException or LIRVectorInt32Range)
        {
            return (CompilationMode.RuntimeIntrinsic, $"Statically bound typed operation/intrinsic/runtime support: {operation}.");
        }
        return (CompilationMode.RuntimeDispatch, $"Unclassified operation conservatively treated as generic: {operation}.");
    }
}
