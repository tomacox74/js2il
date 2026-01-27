using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRForInStatement : HIRStatement
{
    public HIRForInStatement(
        HIRPattern target,
        bool isDeclaration,
        BindingKind declarationKind,
        IReadOnlyList<BindingInfo> loopHeadBindings,
        HIRExpression enumerable,
        HIRStatement body,
        string? label = null)
    {
        Target = target;
        IsDeclaration = isDeclaration;
        DeclarationKind = declarationKind;
        LoopHeadBindings = loopHeadBindings;
        Enumerable = enumerable;
        Body = body;
        Label = label;
    }

    public HIRPattern Target { get; }
    public bool IsDeclaration { get; }
    public BindingKind DeclarationKind { get; }
    public IReadOnlyList<BindingInfo> LoopHeadBindings { get; }
    public HIRExpression Enumerable { get; }
    public HIRStatement Body { get; }
    public string? Label { get; }
}
