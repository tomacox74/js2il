namespace Js2IL.IR;

/// <summary>
/// An opaque identifier for a field in the IR layer.
/// This abstracts away the IL-specific FieldDefinitionHandle, making the IR backend-agnostic.
/// The actual handle is resolved via ScopeMetadataRegistry during IL emission.
/// </summary>
public readonly struct FieldId : IEquatable<FieldId>
{
    /// <summary>
    /// The scope name containing this field.
    /// </summary>
    public string ScopeName { get; }

    /// <summary>
    /// The variable/field name within the scope.
    /// </summary>
    public string FieldName { get; }

    public FieldId(string scopeName, string fieldName)
    {
        ScopeName = scopeName ?? string.Empty;
        FieldName = fieldName ?? string.Empty;
    }

    /// <summary>
    /// Returns true if this FieldId is not valid.
    /// </summary>
    public bool IsNil => string.IsNullOrEmpty(ScopeName) || string.IsNullOrEmpty(FieldName);

    /// <summary>
    /// A nil/invalid FieldId.
    /// </summary>
    public static FieldId Nil => new(string.Empty, string.Empty);

    public bool Equals(FieldId other) => ScopeName == other.ScopeName && FieldName == other.FieldName;

    public override bool Equals(object? obj) => obj is FieldId other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(ScopeName, FieldName);

    public static bool operator ==(FieldId left, FieldId right) => left.Equals(right);

    public static bool operator !=(FieldId left, FieldId right) => !left.Equals(right);

    public override string ToString() => IsNil ? "(nil)" : $"{ScopeName}.{FieldName}";
}
