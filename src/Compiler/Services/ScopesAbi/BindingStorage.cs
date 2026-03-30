using Js2IL.IR;

namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Where a binding's value lives at runtime.
/// </summary>
public enum BindingStorageKind
{
    /// <summary>
    /// The binding is stored in an IL local variable.
    /// Used for bindings declared in the current scope that are not captured.
    /// </summary>
    IlLocal,

    /// <summary>
    /// The binding is an IL method argument.
    /// Used for JavaScript parameters that are not captured.
    /// </summary>
    IlArgument,

    /// <summary>
    /// The binding is a field on the current (leaf) scope instance.
    /// Used for bindings declared in the current scope that are captured by inner functions.
    /// </summary>
    LeafScopeField,

    /// <summary>
    /// The binding is a field on a parent scope instance (accessed via scopes array).
    /// Used for bindings declared in an ancestor scope and referenced here.
    /// </summary>
    ParentScopeField
}

/// <summary>
/// Describes the runtime storage location for a binding.
/// </summary>
/// <param name="Kind">The storage kind (local, argument, field).</param>
/// <param name="LocalIndex">For IlLocal: the local variable index. Otherwise -1.</param>
/// <param name="JsParameterIndex">For IlArgument: the JavaScript parameter index (0-based). Otherwise -1.</param>
/// <param name="Field">For LeafScopeField/ParentScopeField: the field identifier. Otherwise default.</param>
/// <param name="DeclaringScope">For LeafScopeField/ParentScopeField: the scope identifier. Otherwise default.</param>
/// <param name="ParentScopeIndex">For ParentScopeField: the index in the scopes array (object[] scopes). Otherwise -1.</param>
public sealed record BindingStorage(
    BindingStorageKind Kind,
    int LocalIndex = -1,
    int JsParameterIndex = -1,
    FieldId Field = default,
    ScopeId DeclaringScope = default,
    int ParentScopeIndex = -1
)
{
    /// <summary>
    /// Creates a BindingStorage for an IL local variable.
    /// </summary>
    public static BindingStorage ForLocal(int localIndex)
    {
        return new BindingStorage(BindingStorageKind.IlLocal, LocalIndex: localIndex);
    }

    /// <summary>
    /// Creates a BindingStorage for an IL argument (JavaScript parameter).
    /// </summary>
    public static BindingStorage ForArgument(int jsParameterIndex)
    {
        return new BindingStorage(BindingStorageKind.IlArgument, JsParameterIndex: jsParameterIndex);
    }

    /// <summary>
    /// Creates a BindingStorage for a field on the leaf (current) scope.
    /// </summary>
    public static BindingStorage ForLeafScopeField(FieldId field, ScopeId scope)
    {
        return new BindingStorage(
            BindingStorageKind.LeafScopeField,
            Field: field,
            DeclaringScope: scope
        );
    }

    /// <summary>
    /// Creates a BindingStorage for a field on a parent scope.
    /// </summary>
    public static BindingStorage ForParentScopeField(FieldId field, ScopeId scope, int parentScopeIndex)
    {
        return new BindingStorage(
            BindingStorageKind.ParentScopeField,
            Field: field,
            DeclaringScope: scope,
            ParentScopeIndex: parentScopeIndex
        );
    }
}
