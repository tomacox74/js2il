namespace Jroc.HIR;

public sealed class HIRClassNameTDZExpression(string name) : HIRExpression
{
    public string Name { get; } = name;
}
