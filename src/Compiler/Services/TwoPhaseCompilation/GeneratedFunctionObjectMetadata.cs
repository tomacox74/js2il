using System.Reflection.Metadata;

namespace Jroc.Services.TwoPhaseCompilation;

public enum GeneratedFunctionReturnKind
{
    Value,
    Constructor,
    Promise,
    Generator,
    AsyncGenerator
}

public enum GeneratedFunctionStateKind
{
    LexicalThis,
    LexicalNewTarget,
    HomeObject,
    LexicalSuperScopes,
    TransitionalScopeArray,
    PrivateBrand
}

public sealed record GeneratedFunctionCapturePlan(
    string FieldName,
    string ScopeName,
    int ScopeIndex);

public sealed record GeneratedFunctionStatePlan(
    string FieldName,
    GeneratedFunctionStateKind Kind);

public sealed record GeneratedFunctionEntryPointPlan(
    string Name,
    IReadOnlyList<Type?> ParameterClrTypes,
    Type? ReturnClrType,
    MethodDefinitionHandle MethodHandle);

public sealed record GeneratedFunctionObjectPlan
{
    public required CallableId Callable { get; init; }

    public required CallableSignature Signature { get; init; }

    public required string Namespace { get; init; }

    public required string ModuleName { get; init; }

    public required string TypeName { get; init; }

    public required string CanonicalOwnerTypeName { get; init; }

    public IReadOnlyList<GeneratedFunctionCapturePlan> Captures { get; init; } =
        Array.Empty<GeneratedFunctionCapturePlan>();

    public IReadOnlyList<GeneratedFunctionStatePlan> StateFields { get; init; } =
        Array.Empty<GeneratedFunctionStatePlan>();

    public int ScopeChainSlotCount { get; init; }

    public bool IsConstructable { get; init; }

    public bool RequiresInvocationContext { get; init; }

    public bool UsesNonStrictThisBinding { get; init; }

    public bool RequiresArrayCallAdapter { get; init; }

    public GeneratedFunctionReturnKind ReturnKind { get; init; }
}

public sealed record GeneratedFunctionObjectMetadata
{
    public required GeneratedFunctionObjectPlan Plan { get; init; }

    public required TypeDefinitionHandle TypeHandle { get; init; }

    public required TypeDefinitionHandle CanonicalOwnerTypeHandle { get; init; }

    public required MethodDefinitionHandle ConstructorHandle { get; init; }

    public required MethodDefinitionHandle CallAdapterHandle { get; init; }

    public MethodDefinitionHandle ArrayCallAdapterHandle { get; init; }

    public required MethodDefinitionHandle IsConstructorGetterHandle { get; init; }

    public required MethodDefinitionHandle RequiresInvocationContextGetterHandle { get; init; }

    public MethodDefinitionHandle OrdinaryThisResolverHandle { get; init; }

    public required IReadOnlyDictionary<GeneratedFunctionStateKind, MethodDefinitionHandle>
        StateAccessorHandles { get; init; }

    public MethodDefinitionHandle ConstructAdapterHandle { get; init; }

    public MethodDefinitionHandle ConstructBodyAdapterHandle { get; init; }

    public required IReadOnlyDictionary<string, FieldDefinitionHandle> FieldHandles { get; init; }

    public required IReadOnlyList<GeneratedFunctionEntryPointPlan> EntryPoints { get; init; }
}
