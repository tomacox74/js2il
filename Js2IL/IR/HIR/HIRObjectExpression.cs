using System.Collections.Immutable;

namespace Js2IL.HIR;

/// <summary>
/// Represents a JavaScript object literal expression, e.g., { name: "Alice", age: 31 }.
/// </summary>
public sealed class HIRObjectExpression : HIRExpression
{
    public HIRObjectExpression(IEnumerable<HIRObjectMember> members)
    {
        Members = members.ToImmutableArray();
    }

    /// <summary>
    /// The members of the object literal, in source evaluation order.
    /// </summary>
    public ImmutableArray<HIRObjectMember> Members { get; init; }
}

/// <summary>
/// Base type for object literal members.
/// </summary>
public abstract class HIRObjectMember
{
}

/// <summary>
/// Represents a single non-computed property in an object literal (key-value pair).
/// </summary>
public sealed class HIRObjectProperty : HIRObjectMember
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

/// <summary>
/// Represents a computed property in an object literal, e.g., { [expr]: value }.
/// </summary>
public sealed class HIRObjectComputedProperty : HIRObjectMember
{
    public HIRObjectComputedProperty(HIRExpression keyExpression, HIRExpression value)
    {
        KeyExpression = keyExpression;
        Value = value;
    }

    public HIRExpression KeyExpression { get; init; }
    public HIRExpression Value { get; init; }
}

/// <summary>
/// Represents a spread member in an object literal, e.g., { ...x }.
/// </summary>
public sealed class HIRObjectSpreadProperty : HIRObjectMember
{
    public HIRObjectSpreadProperty(HIRExpression argument)
    {
        Argument = argument;
    }

    public HIRExpression Argument { get; init; }
}
