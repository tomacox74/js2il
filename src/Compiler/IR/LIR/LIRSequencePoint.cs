using Js2IL.DebugSymbols;

namespace Js2IL.IR;

/// <summary>
/// Marker instruction that indicates the next emitted IL should be associated with the given source span.
/// This instruction emits no IL by itself.
/// </summary>
public sealed record LIRSequencePoint(SourceSpan Span) : LIRInstruction;
