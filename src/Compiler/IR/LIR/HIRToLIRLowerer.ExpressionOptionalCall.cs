using System;
using System.Collections.Generic;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerOptionalCallExpression(HIROptionalCallExpression optionalCallExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Evaluate callee once, then short-circuit if nullish.
        if (!TryLowerExpression(optionalCallExpr.Callee, out var calleeTemp))
        {
            IRPipelineMetrics.RecordFailureIfUnset(
                $"HIR->LIR: failed lowering optional call callee {optionalCallExpr.Callee.GetType().Name}");
            return false;
        }

        var calleeBoxed = EnsureObject(calleeTemp);

        int nullishLabel = CreateLabel();
        int endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(calleeBoxed, nullishLabel));

        var isJsNullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), calleeBoxed, isJsNullTemp));
        DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, nullishLabel));

        if (!TryLowerCallArgumentsToArgsArray(optionalCallExpr.Arguments, out var argsArrayTemp))
        {
            IRPipelineMetrics.RecordFailureIfUnset("HIR->LIR: failed lowering optional call arguments");
            return false;
        }

        var scopesTemp = CreateTempVariable();
        if (!TryBuildCurrentScopesArray(scopesTemp))
        {
            IRPipelineMetrics.RecordFailureIfUnset("HIR->LIR: failed building scopes array for optional call");
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(calleeBoxed, scopesTemp, argsArrayTemp, resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        _methodBodyIR.Instructions.Add(new LIRLabel(nullishLabel));
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
