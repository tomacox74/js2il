using System.Reflection.Metadata;

namespace Jroc.Services.TwoPhaseCompilation;

/// <summary>
/// Canonical, compilation-scoped CallableId-keyed store for generated function-object plans and metadata.
/// </summary>
public sealed class GeneratedFunctionObjectRegistry
{
    private readonly Dictionary<CallableId, GeneratedFunctionObjectPlan> _plans = new();
    private readonly Dictionary<CallableId, GeneratedFunctionObjectMetadata> _metadata = new();
    private readonly List<CallableId> _stableOrder = new();

    public bool StrictMode { get; set; }

    public void Plan(GeneratedFunctionObjectPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (!_plans.TryGetValue(plan.Callable, out var existing))
        {
            _plans.Add(plan.Callable, plan);
            _stableOrder.Add(plan.Callable);
            return;
        }

        if (!PlansEquivalent(existing, plan))
        {
            throw new InvalidOperationException(
                $"Generated function object '{plan.Callable.DisplayName}' was planned inconsistently.");
        }
    }

    private static bool PlansEquivalent(
        GeneratedFunctionObjectPlan left,
        GeneratedFunctionObjectPlan right)
    {
        return left.Callable == right.Callable
            && SignaturesEquivalent(left.Signature, right.Signature)
            && left.Namespace == right.Namespace
            && left.ModuleName == right.ModuleName
            && left.TypeName == right.TypeName
            && left.CanonicalOwnerTypeName == right.CanonicalOwnerTypeName
            && left.ScopeChainSlotCount == right.ScopeChainSlotCount
            && left.IsConstructable == right.IsConstructable
            && left.RequiresInvocationContext == right.RequiresInvocationContext
            && left.UsesNonStrictThisBinding == right.UsesNonStrictThisBinding
            && left.ReturnKind == right.ReturnKind
            && left.Captures.SequenceEqual(right.Captures)
            && left.StateFields.SequenceEqual(right.StateFields);
    }

    private static bool SignaturesEquivalent(
        CallableSignature left,
        CallableSignature right)
    {
        return left.OwnerTypeHandle == right.OwnerTypeHandle
            && left.ScopeAbiKind == right.ScopeAbiKind
            && left.SingleScopeScopeName == right.SingleScopeScopeName
            && left.JsParamCount == right.JsParamCount
            && left.ParameterClrTypes.SequenceEqual(right.ParameterClrTypes)
            && left.ReturnClrType == right.ReturnClrType
            && left.InvokeShape == right.InvokeShape
            && left.IsInstanceMethod == right.IsInstanceMethod
            && left.ILMethodName == right.ILMethodName
            && left.SignatureBlob == right.SignatureBlob;
    }

    public void SetMetadata(GeneratedFunctionObjectMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (!_plans.ContainsKey(metadata.Plan.Callable))
        {
            throw new InvalidOperationException(
                $"Cannot set generated function-object metadata for unplanned callable '{metadata.Plan.Callable.DisplayName}'.");
        }

        if (_metadata.TryGetValue(metadata.Plan.Callable, out var existing))
        {
            if (existing != metadata)
            {
                throw new InvalidOperationException(
                    $"Generated function-object metadata for '{metadata.Plan.Callable.DisplayName}' was assigned more than once.");
            }
            return;
        }

        _metadata.Add(metadata.Plan.Callable, metadata);
    }

    public bool TryGetPlan(CallableId callable, out GeneratedFunctionObjectPlan plan)
        => _plans.TryGetValue(callable, out plan!);

    public bool TryGetMetadata(CallableId callable, out GeneratedFunctionObjectMetadata metadata)
        => _metadata.TryGetValue(callable, out metadata!);

    public void AddSpecializedEntryPoint(
        CallableId callable,
        GeneratedFunctionEntryPointPlan entryPoint)
    {
        ArgumentNullException.ThrowIfNull(entryPoint);
        if (!_metadata.TryGetValue(callable, out var metadata))
        {
            throw new InvalidOperationException(
                $"Cannot add a specialized entry point before generated metadata exists for '{callable.DisplayName}'.");
        }

        var updated = metadata with
        {
            EntryPoints = metadata.EntryPoints.Concat([entryPoint]).ToArray()
        };
        _metadata[callable] = updated;
    }

    public GeneratedFunctionObjectMetadata GetMetadata(CallableId callable)
    {
        if (_metadata.TryGetValue(callable, out var metadata))
        {
            return metadata;
        }

        if (StrictMode)
        {
            throw new InvalidOperationException(
                $"Missing generated function-object metadata for '{callable.DisplayName}'.");
        }

        return null!;
    }

    public IReadOnlyList<GeneratedFunctionObjectPlan> GetPlansInStableOrder()
    {
        return _stableOrder
            .Where(_plans.ContainsKey)
            .Select(callable => _plans[callable])
            .ToArray();
    }

    public IReadOnlyList<GeneratedFunctionObjectMetadata> GetMetadataInStableOrder()
    {
        return _stableOrder
            .Where(_metadata.ContainsKey)
            .Select(callable => _metadata[callable])
            .ToArray();
    }
}
