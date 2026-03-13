namespace Js2IL.HIR;

public sealed class HIRUpdateExpression : HIRExpression
{
    public HIRUpdateExpression(Acornima.Operator op, bool prefix, HIRExpression argument)
    {
        Operator = op;
        Prefix = prefix;
        Argument = argument;
    }

    public Acornima.Operator Operator { get; }

    public bool Prefix { get; }

    public HIRExpression Argument { get; }
}
