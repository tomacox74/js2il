using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<ControlFlowContext> _controlFlowStack = new();
    private readonly Stack<int> _protectedControlFlowDepthStack = new();

    private TempVariable EnsureConditionIsBoolean(TempVariable conditionTemp)
    {
        var conditionStorage = GetTempStorage(conditionTemp);
        if (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(bool))
        {
            return conditionTemp;
        }

        var isTruthyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(conditionTemp, isTruthyTemp));
        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return isTruthyTemp;
    }

    private readonly struct ControlFlowContext
    {
        public ControlFlowContext(int breakLabel, int? continueLabel, string? labelName)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
            LabelName = labelName;
        }

        public int BreakLabel { get; }
        public int? ContinueLabel { get; }
        public string? LabelName { get; }
    }

    private bool TryResolveControlFlowTarget(string? label, out int targetLabel, out int matchedAbsoluteIndex, bool isBreak)
    {
        targetLabel = default;
        matchedAbsoluteIndex = -1;

        // Enumerate from top to bottom; Stack<T>.ToArray() returns top-first.
        var contexts = _controlFlowStack.ToArray();
        var total = contexts.Length;

        if (string.IsNullOrEmpty(label))
        {
            if (isBreak)
            {
                if (total == 0)
                {
                    return false;
                }

                targetLabel = contexts[0].BreakLabel;
                matchedAbsoluteIndex = total - 1;
                return true;
            }

            // continue without label targets nearest loop context
            for (int i = 0; i < total; i++)
            {
                if (contexts[i].ContinueLabel is int continueLabel)
                {
                    targetLabel = continueLabel;
                    matchedAbsoluteIndex = total - 1 - i;
                    return true;
                }
            }

            return false;
        }

        for (int i = 0; i < total; i++)
        {
            var ctx = contexts[i];
            if (!string.Equals(ctx.LabelName, label, global::System.StringComparison.Ordinal))
            {
                continue;
            }

            if (isBreak)
            {
                targetLabel = ctx.BreakLabel;
                matchedAbsoluteIndex = total - 1 - i;
                return true;
            }

            if (ctx.ContinueLabel is int continueLabel)
            {
                targetLabel = continueLabel;
                matchedAbsoluteIndex = total - 1 - i;
                return true;
            }

            // Labeled continue targeting a non-loop labeled statement is invalid; do not fall through
            // to outer contexts with the same label.
            return false;
        }

        return false;
    }
}
