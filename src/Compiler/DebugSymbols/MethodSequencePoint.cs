namespace Js2IL.DebugSymbols;

/// <summary>
/// A debug sequence point captured during IL emission.
/// IL offset is relative to the start of the method's IL stream (not including the method header).
/// </summary>
public readonly record struct MethodSequencePoint(int IlOffset, SourceSpan Span);
