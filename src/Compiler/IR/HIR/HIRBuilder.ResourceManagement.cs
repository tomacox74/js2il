using Acornima.Ast;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private static HIRResourceKind GetResourceKind(VariableDeclarationKind kind)
        => kind switch
        {
            VariableDeclarationKind.Using => HIRResourceKind.Sync,
            VariableDeclarationKind.AwaitUsing => HIRResourceKind.Async,
            _ => HIRResourceKind.None
        };

    private static HIRStatement MarkResourceDeclaration(HIRStatement declaration, VariableDeclarationKind kind)
        => declaration is HIRVariableDeclaration variable && GetResourceKind(kind) is not HIRResourceKind.None
            ? new HIRVariableDeclaration(variable.Name, variable.Initializer)
            {
                InferredName = variable.InferredName,
                ResourceKind = GetResourceKind(kind)
            }
            : declaration;

    internal static HIRResourceKind GetResourceKind(IEnumerable<HIRStatement> statements)
    {
        var kind = HIRResourceKind.None;
        foreach (var statement in statements)
        {
            var current = statement switch
            {
                HIRVariableDeclaration variable => variable.ResourceKind,
                HIRBlock { ResourceKind: HIRResourceKind.None } sequence => GetResourceKind(sequence.Statements),
                _ => HIRResourceKind.None
            };
            if (current == HIRResourceKind.Async)
                return current;
            if (current == HIRResourceKind.Sync)
                kind = current;
        }
        return kind;
    }
}
