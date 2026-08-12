using Acornima.Ast;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;
using Jroc.Utilities;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private HIRStaticClassMethodTarget? ResolveStaticClassMethodTarget(
        HIRExpression callee)
    {
        if (callee is not HIRPropertyAccessExpression propertyAccess)
        {
            return null;
        }

        Scope? classScope = null;
        var setsCurrentThis = false;
        var validatesPrivateReceiver = false;

        if (propertyAccess.Object is HIRVariableExpression classBinding)
        {
            classScope = classBinding.Name.BindingInfo.ClassScope;
            setsCurrentThis = classScope != null;
        }
        else if (propertyAccess.Object is HIRThisExpression thisExpression
                 && (thisExpression.StaticClassRegistryName != null
                     || _currentScope.Callable?.Kind == CallableKind.ClassStaticMethod))
        {
            classScope = _currentScope;
            while (classScope?.Kind != ScopeKind.Class)
            {
                classScope = classScope?.Parent;
            }
            validatesPrivateReceiver = classScope != null
                && thisExpression.StaticClassRegistryName == null;
        }

        var method = classScope?.ClassSemantics?.Methods.FirstOrDefault(candidate =>
            candidate.IsStatic
            && candidate.IsPrivate
            && string.Equals(
                candidate.PropertyName,
                propertyAccess.PropertyName,
                StringComparison.Ordinal));
        if (classScope == null || method == null)
        {
            return null;
        }

        return new HIRStaticClassMethodTarget(
            propertyAccess.Object,
            classScope,
            method,
            setsCurrentThis,
            validatesPrivateReceiver && method.IsPrivate);
    }

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
