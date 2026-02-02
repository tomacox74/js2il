using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerReturnStatement(HIRReturnStatement returnStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        TempVariable returnTempVar;
        if (_callableKind == CallableKind.Constructor && returnStmt.Expression != null)
        {
            // Constructors are void-returning in IL, but JavaScript allows `return <expr>`.
            // Stash the value so the `new` call site can apply JS override semantics.
            if (!TryLowerExpression(returnStmt.Expression, out var ctorReturnTemp))
            {
                return false;
            }

            ctorReturnTemp = EnsureObject(ctorReturnTemp);

            if (!TryGetEnclosingClassRegistryName(out var registryClassName))
            {
                return false;
            }

            lirInstructions.Add(new LIRStoreUserClassInstanceField(
                RegistryClassName: registryClassName!,
                FieldName: "__js2il_ctorReturn",
                IsPrivateField: true,
                Value: ctorReturnTemp));

            // Control-flow return value is irrelevant for constructors.
            returnTempVar = CreateTempVariable();
            lirInstructions.Add(new LIRConstUndefined(returnTempVar));
            DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else if (returnStmt.Expression != null)
        {
            // Lower the return expression
            // Special-case: if we inferred `return this` for a class method, keep it typed as the
            // user-defined class (metadata TypeDef handle) so the return matches the class-typed ABI.
            if (_scope?.StableReturnIsThis == true
                && returnStmt.Expression is HIRThisExpression
                && _classRegistry != null
                && TryGetEnclosingClassRegistryName(out var registryClassName)
                && registryClassName != null
                && _classRegistry.TryGet(registryClassName, out var thisTypeHandle)
                && !thisTypeHandle.IsNil)
            {
                returnTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadThis(returnTempVar));
                DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object), thisTypeHandle));
            }
            else
            {
                if (!TryLowerExpression(returnStmt.Expression, out returnTempVar))
                {
                    return false;
                }
            }

            // Default ABI returns object. If we inferred a stable return type for this callable,
            // preserve/produce the matching unboxed or typed value.
            // Typed/unboxed returns are only supported for class methods currently.
            // Keep ABI consistent: other callables must return object.
            var stableReturnClrType = (_scope?.Kind == ScopeKind.Function && _scope?.Parent?.Kind == ScopeKind.Class)
                ? _scope.StableReturnClrType
                : null;
            if (stableReturnClrType == typeof(double))
            {
                returnTempVar = EnsureNumber(returnTempVar);
            }
            else if (stableReturnClrType == typeof(bool))
            {
                returnTempVar = EnsureBoolean(returnTempVar);
            }
            else if (_scope?.StableReturnIsThis != true)
            {
                returnTempVar = EnsureObject(returnTempVar);
            }
        }
        else
        {
            // Bare return - return undefined (null)
            returnTempVar = CreateTempVariable();
            lirInstructions.Add(new LIRConstUndefined(returnTempVar));
            DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        // Async try/finally lowering: a return inside a protected region must flow through finally.
        if (_isAsync
            && _methodBodyIR.AsyncInfo?.HasAwaits == true
            && !_methodBodyIR.LeafScopeId.IsNil
            && _asyncTryFinallyStack.Count > 0)
        {
            var ctx = _asyncTryFinallyStack.Peek();
            var scopeName = _methodBodyIR.LeafScopeId.Name;

            returnTempVar = EnsureObject(returnTempVar);

            // pendingReturnValue = returnTempVar
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, returnTempVar));

            // hasPendingReturn = true
            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, hasReturnTemp));

            // hasPendingException = false; pendingException = null (return overrides)
            var clearHasExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, clearHasExTemp));
            DefineTempStorage(clearHasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, clearHasExTemp));

            var clearExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(clearExTemp));
            DefineTempStorage(clearExTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, clearExTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));
            return true;
        }

        // If we are inside a protected region with a finally handler, we must use leave
        // so finally runs before returning.
        if (_protectedControlFlowDepthStack.Count > 0 && _methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            if (!TryEmitReturnViaEpilogue(returnTempVar))
            {
                return false;
            }
            return true;
        }

        lirInstructions.Add(new LIRReturn(returnTempVar));
        return true;
    }
}
