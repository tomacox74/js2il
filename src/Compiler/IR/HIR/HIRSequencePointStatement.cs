using Js2IL.DebugSymbols;

namespace Js2IL.HIR;

/// <summary>
/// Marker statement inserted by the HIR builder to preserve source locations for debugging.
/// Lowering emits a corresponding LIR marker and the IL emitter can translate it into a PDB sequence point.
/// </summary>
public sealed class HIRSequencePointStatement : HIRStatement
{
    public required SourceSpan Span { get; init; }
}
