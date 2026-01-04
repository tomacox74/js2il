namespace Js2IL.IR;

/// <summary>
/// An opaque identifier for a scope type in the IR layer.
/// This abstracts away the IL-specific TypeDefinitionHandle, making the IR backend-agnostic.
/// The actual handle is resolved via ScopeMetadataRegistry during IL emission.
/// </summary>
public readonly struct ScopeId : IEquatable<ScopeId>
{
    /// <summary>
    /// The scope name used to look up the actual type handle.
    /// </summary>
    public string Name { get; }

    public ScopeId(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// Returns true if this ScopeId is not valid (empty name).
    /// </summary>
    public bool IsNil => string.IsNullOrEmpty(Name);

    /// <summary>
    /// A nil/invalid ScopeId.
    /// </summary>
    public static ScopeId Nil => default;

    public bool Equals(ScopeId other) => Name == other.Name;

    public override bool Equals(object? obj) => obj is ScopeId other && Equals(other);

    public override int GetHashCode() => Name?.GetHashCode() ?? 0;

    public static bool operator ==(ScopeId left, ScopeId right) => left.Equals(right);

    public static bool operator !=(ScopeId left, ScopeId right) => !left.Equals(right);

    public override string ToString() => IsNil ? "(nil)" : Name;
}
