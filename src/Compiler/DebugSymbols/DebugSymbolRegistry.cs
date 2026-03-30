using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.DebugSymbols;

/// <summary>
/// Collects debug symbol info (currently sequence points) during compilation so that
/// the assembly writer can emit a Portable PDB.
/// </summary>
public sealed class DebugSymbolRegistry
{
    public readonly record struct MethodLocal(int Index, string Name);

    public sealed record MethodDebugInfo(
        MethodSequencePoint[] SequencePoints,
        StandaloneSignatureHandle LocalSignature,
        int IlLength,
        MethodLocal[] Locals);

    private readonly ConcurrentDictionary<MethodDefinitionHandle, MethodDebugInfo> _debugInfoByMethod = new();

    public void SetMethodDebugInfo(
        MethodDefinitionHandle method,
        MethodSequencePoint[] sequencePoints,
        StandaloneSignatureHandle localSignature,
        int ilLength,
        MethodLocal[] locals)
    {
        if (method.IsNil) return;

        _debugInfoByMethod[method] = new MethodDebugInfo(
            sequencePoints ?? Array.Empty<MethodSequencePoint>(),
            localSignature,
            ilLength,
            locals ?? Array.Empty<MethodLocal>());
    }

    public bool TryGetMethodDebugInfo(MethodDefinitionHandle method, out MethodDebugInfo debugInfo)
        => _debugInfoByMethod.TryGetValue(method, out debugInfo!);
}
