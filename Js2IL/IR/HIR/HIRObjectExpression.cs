using System.Collections.Immutable;

namespace Js2IL.HIR;

/// <summary>
/// Represents a JavaScript object literal expression, e.g., { name: "Alice", age: 31 }.
/// </summary>
public sealed class HIRObjectExpression : HIRExpression
{
    public HIRObjectExpression(IEnumerable<HIRObjectProperty> properties)
    {
        Properties = properties.ToImmutableArray();
    }

    /// <summary>
    /// The properties of the object literal.
    /// </summary>
    public ImmutableArray<HIRObjectProperty> Properties { get; init; }
}

/// <summary>
/// Represents a single property in an object literal (key-value pair).
/// </summary>
public sealed class HIRObjectProperty
{
    public HIRObjectProperty(string key, HIRExpression value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// The property key (name).
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// The property value expression.
    /// </summary>
    public HIRExpression Value { get; init; }
}
