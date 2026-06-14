namespace Jroc.HIR;

public sealed class HIRClassHeritageValidationExpression : HIRExpression
{
    public HIRClassHeritageValidationExpression(HIRExpression heritage)
    {
        Heritage = heritage;
    }

    public HIRExpression Heritage { get; }
}
