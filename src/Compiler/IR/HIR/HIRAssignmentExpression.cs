using Js2IL.SymbolTables;

namespace Js2IL.HIR;

/// <summary>
/// Represents an assignment expression in HIR (e.g., x = 5, y = x + 1).
/// Currently only supports simple identifier targets (not destructuring or property assignment).
/// </summary>
public sealed class HIRAssignmentExpression : HIRExpression
{
    public HIRAssignmentExpression(Symbol target, Acornima.Operator op, HIRExpression value)
    {
        Target = target;
        Operator = op;
        Value = value;
    }

    /// <summary>
    /// The target variable being assigned to.
    /// </summary>
    public Symbol Target { get; }

    /// <summary>
    /// The assignment operator (Assignment for =, or compound operators like AdditionAssignment for +=).
    /// </summary>
    public Acornima.Operator Operator { get; }

    /// <summary>
    /// The value expression being assigned.
    /// </summary>
    public HIRExpression Value { get; }
}
