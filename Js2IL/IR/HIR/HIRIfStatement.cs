namespace Js2IL.HIR;

/// <summary>
/// Represents an if statement with optional else branch.
/// </summary>
public sealed class HIRIfStatement : HIRStatement
{
    public HIRIfStatement(HIRExpression test, HIRStatement consequent, HIRStatement? alternate)
    {
        Test = test;
        Consequent = consequent;
        Alternate = alternate;
    }

    public HIRExpression Test { get; }
    public HIRStatement Consequent { get; }
    public HIRStatement? Alternate { get; }
}
