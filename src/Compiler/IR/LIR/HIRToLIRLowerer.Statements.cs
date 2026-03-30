using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using System.Linq;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    public bool TryLowerStatements(IEnumerable<HIRStatement> statements)
    {
        return statements.All(statement =>
        {
            if (TryLowerStatement(statement))
            {
                return true;
            }

            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering statement {statement.GetType().Name}");
            return false;
        });
    }
}
