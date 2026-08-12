using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;

namespace Jroc.HIR;

/// <summary>
/// A phase-one-resolved static class method call target.
/// </summary>
public sealed record HIRStaticClassMethodTarget(
    HIRExpression Receiver,
    Scope ClassScope,
    ClassMethodSemantics Method,
    bool SetsCurrentThis,
    bool ValidatesPrivateReceiver);
