namespace Jroc.IR;

public sealed record LIRVectorInt32Load(
    TempVariable Array, TempVariable Start, TempVariable End);

public sealed record LIRVectorInt32Or(TempVariable Mask);

public sealed record LIRVectorInt32Store(TempVariable Array);

// A fused range keeps the load, operation and store on the same contiguous view.
// Normalization lowers the range to the portable SIMD runtime primitive.
public sealed record LIRVectorInt32Range(
    LIRVectorInt32Load Load,
    LIRVectorInt32Or Operation,
    LIRVectorInt32Store Store,
    TempVariable Applied) : LIRInstruction;
