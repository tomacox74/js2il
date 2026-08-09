using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Utilities;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private bool TryCreateStableDirectCallableTarget(
        CallExpression sourceCall,
        HIRExpression callee,
        out HIRStableDirectCallableTarget? target)
    {
        target = null;

        if (callee is not HIRVariableExpression { Name: var symbol }
            || !StableDirectCallableEligibility.TryGetEligibleCall(
                symbol.BindingInfo,
                sourceCall,
                _currentScope,
                out var initializer,
                out var callableScope,
                out _)
            || initializer == null
            || callableScope == null)
        {
            return false;
        }

        var callableId = initializer switch
        {
            ArrowFunctionExpression arrow =>
                CreateArrowFunctionCallableId(callableScope, arrow),
            FunctionExpression function =>
                CreateFunctionExpressionCallableId(
                    callableScope,
                    function,
                    GetDeclaringScopeName(callableScope.Parent!),
                    ArgumentsObjectSemantics.IsStrictScope(callableScope)),
            _ => throw new InvalidOperationException(
                $"Unexpected stable callable initializer {initializer.Type}.")
        };

        target = new HIRStableDirectCallableTarget(callableId, callableScope);
        return true;
    }
}
