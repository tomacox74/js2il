using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Output of Phase 2 body compilation for a single callable.
/// The MethodDef row is emitted later (Phase 2 finalization) to preserve ECMA-335 ordering.
/// </summary>
public sealed record CompiledCallableBody
{
    public required CallableId Callable { get; init; }

    public required string MethodName { get; init; }

    /// <summary>Expected MethodDef token preallocated in Phase 1.</summary>
    public required MethodDefinitionHandle ExpectedMethodDef { get; init; }

    public required MethodAttributes Attributes { get; init; }

    public required BlobHandle Signature { get; init; }

    public required int BodyOffset { get; init; }

    public required string[] ParameterNames { get; init; }

    public void Validate()
    {
        if (ExpectedMethodDef.IsNil) throw new InvalidOperationException("ExpectedMethodDef cannot be nil.");
        if (string.IsNullOrWhiteSpace(MethodName)) throw new InvalidOperationException("MethodName cannot be null or empty.");
        if (BodyOffset < 0) throw new InvalidOperationException("BodyOffset must be a valid method body offset.");
        if (ParameterNames == null) throw new InvalidOperationException("ParameterNames cannot be null.");
    }
}
