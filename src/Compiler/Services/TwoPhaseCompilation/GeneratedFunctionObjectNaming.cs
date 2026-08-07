using Acornima.Ast;
using Jroc.SymbolTables;

namespace Jroc.Services.TwoPhaseCompilation;

internal static class GeneratedFunctionObjectNaming
{
    public const string WrapperTypeName = "FunctionObject";

    public static string AvoidArrowWrapperCollision(string requestedName, Scope scope)
    {
        if (!string.Equals(requestedName, WrapperTypeName, StringComparison.Ordinal))
        {
            return requestedName;
        }

        for (var current = scope.Parent; current != null; current = current.Parent)
        {
            if (current.Kind != ScopeKind.Function)
            {
                continue;
            }

            if (current.AstNode is not ArrowFunctionExpression)
            {
                return requestedName;
            }

            var location = SourceLocation.FromNode((Node)scope.AstNode!);
            return $"<User>{WrapperTypeName}_{location}";
        }

        return requestedName;
    }
}
