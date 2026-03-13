namespace Js2IL.HIR;

public sealed class HIRDestructuringAssignmentExpression : HIRExpression
{
    public HIRDestructuringAssignmentExpression(HIRPattern pattern, HIRExpression value)
    {
        Pattern = pattern;
        Value = value;
    }

    public HIRPattern Pattern { get; }

    public HIRExpression Value { get; }
}
