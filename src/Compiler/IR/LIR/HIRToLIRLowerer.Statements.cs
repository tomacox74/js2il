using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using System.Linq;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

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
