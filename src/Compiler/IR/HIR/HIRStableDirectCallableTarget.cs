using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;

namespace Jroc.HIR;

public sealed record HIRStableDirectCallableTarget(
    CallableId CallableId,
    Scope CallableScope);
