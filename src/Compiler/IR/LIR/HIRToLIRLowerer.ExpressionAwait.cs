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
    private bool TryLowerAwaitExpression(HIRAwaitExpression awaitExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // await is only supported inside async functions
        if (!_isAsync || _methodBodyIR.AsyncInfo == null)
        {
            IRPipelineMetrics.RecordFailure("await expression found outside async function context");
            return false;
        }

        // Lower the awaited expression first
        if (!TryLowerExpression(awaitExpr.Argument, out var awaitedValueTemp))
        {
            return false;
        }

        // Ensure the awaited value is boxed to object
        awaitedValueTemp = EnsureObject(awaitedValueTemp);

        // Allocate state ID and label for resumption
        var asyncInfo = _methodBodyIR.AsyncInfo;
        var awaitId = asyncInfo.AllocateAwaitId();
        var resumeStateId = asyncInfo.AllocateResumeStateId();
        var resumeLabel = CreateLabel();

        // Create result temp for the await expression
        resultTempVar = CreateTempVariable();
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        // Record the await point
        asyncInfo.AwaitPoints.Add(new AwaitPointInfo
        {
            AwaitId = awaitId,
            ResumeStateId = resumeStateId,
            ResumeLabelId = resumeLabel,
            ResultTemp = resultTempVar
        });

        asyncInfo.RegisterResumeLabel(resumeStateId, resumeLabel);

        int? rejectStateId = null;
        string? pendingExceptionField = null;
        if (_asyncTryCatchStack.Count > 0)
        {
            var ctx = _asyncTryCatchStack.Peek();
            rejectStateId = ctx.CatchStateId;
            pendingExceptionField = ctx.PendingExceptionFieldName;
        }

        // Emit the await instruction
        _methodBodyIR.Instructions.Add(new LIRAwait(
            awaitedValueTemp,
            awaitId,
            resumeStateId,
            resumeLabel,
            resultTempVar,
            rejectStateId,
            pendingExceptionField));

        return true;
    }
}
