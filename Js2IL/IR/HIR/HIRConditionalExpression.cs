namespace Js2IL.HIR;

public sealed class HIRConditionalExpression : HIRExpression
{
    public HIRExpression Test { get; }
    public HIRExpression Consequent { get; }
    public HIRExpression Alternate { get; }

    public HIRConditionalExpression(HIRExpression test, HIRExpression consequent, HIRExpression alternate)
    {
        Test = test;
        Consequent = consequent;
        Alternate = alternate;
    }
}
