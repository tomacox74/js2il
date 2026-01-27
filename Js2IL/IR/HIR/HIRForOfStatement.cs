using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRForOfStatement : HIRStatement
{
    public HIRForOfStatement(
        HIRPattern target,
        bool isDeclaration,
        BindingKind declarationKind,
        IReadOnlyList<BindingInfo> loopHeadBindings,
        HIRExpression iterable,
        HIRStatement body,
        string? label = null)
    {
        Target = target;
        IsDeclaration = isDeclaration;
        DeclarationKind = declarationKind;
        LoopHeadBindings = loopHeadBindings;
        Iterable = iterable;
        Body = body;
        Label = label;
    }

    public HIRPattern Target { get; }
    public bool IsDeclaration { get; }
    public BindingKind DeclarationKind { get; }
    public IReadOnlyList<BindingInfo> LoopHeadBindings { get; }
    public HIRExpression Iterable { get; }
    public HIRStatement Body { get; }
    public string? Label { get; }
}
