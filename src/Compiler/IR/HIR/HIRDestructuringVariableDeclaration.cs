namespace Js2IL.HIR;

public sealed class HIRDestructuringVariableDeclaration : HIRStatement
{
    public HIRDestructuringVariableDeclaration(HIRPattern pattern, HIRExpression initializer)
    {
        Pattern = pattern;
        Initializer = initializer;
    }

    public HIRPattern Pattern { get; }

    public HIRExpression Initializer { get; }
}
