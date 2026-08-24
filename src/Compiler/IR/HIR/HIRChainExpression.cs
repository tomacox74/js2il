namespace Jroc.HIR;

public sealed class HIRChainExpression : HIRExpression
{
    public HIRChainExpression(
        HIRExpression baseExpression,
        IReadOnlyList<HIRChainSegment> segments)
    {
        BaseExpression = baseExpression;
        Segments = segments;
    }

    public HIRExpression BaseExpression { get; }
    public IReadOnlyList<HIRChainSegment> Segments { get; }
}

public abstract record HIRChainSegment(bool Optional);

public sealed record HIRChainPropertySegment(
    string PropertyName,
    bool Optional) : HIRChainSegment(Optional);

public sealed record HIRChainIndexSegment(
    HIRExpression Index,
    bool Optional) : HIRChainSegment(Optional);

public sealed record HIRChainCallSegment(
    IReadOnlyList<HIRExpression> Arguments,
    bool Optional) : HIRChainSegment(Optional);
