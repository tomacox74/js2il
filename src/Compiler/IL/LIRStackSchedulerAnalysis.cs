using Jroc.IR;

namespace Jroc.IL;

internal sealed class LIRStackSchedulerAnalysis
{
    private readonly int[] _definitionIndex;
    private readonly int[] _definitionCount;
    private readonly int[] _useIndex;
    private readonly int[] _useCount;
    private readonly int[] _useOffsets;
    private readonly TempVariable[] _uses;

    internal LIRStackSchedulerAnalysis(MethodBodyIR methodBody)
    {
        var tempCount = methodBody.Temps.Count;
        _definitionIndex = new int[tempCount];
        _definitionCount = new int[tempCount];
        _useIndex = new int[tempCount];
        _useCount = new int[tempCount];
        _useOffsets = new int[methodBody.Instructions.Count + 1];
        Array.Fill(_definitionIndex, -1);
        Array.Fill(_useIndex, -1);
        var uses = new List<TempVariable>(methodBody.Instructions.Count);

        for (var index = 0; index < methodBody.Instructions.Count; index++)
        {
            var instruction = methodBody.Instructions[index];
            if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined)
                && (uint)defined.Index < (uint)tempCount)
            {
                _definitionCount[defined.Index]++;
                _definitionIndex[defined.Index] = index;
            }

            _useOffsets[index] = uses.Count;
            var visitor = new UseVisitor(_useCount, _useIndex, uses, index);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        _useOffsets[^1] = uses.Count;
        _uses = uses.ToArray();
    }

    internal ReadOnlySpan<int> DefinitionIndex => _definitionIndex;
    internal ReadOnlySpan<int> DefinitionCount => _definitionCount;
    internal ReadOnlySpan<int> UseIndex => _useIndex;
    internal ReadOnlySpan<int> UseCount => _useCount;

    internal ReadOnlySpan<TempVariable> GetUses(int instructionIndex)
        => _uses.AsSpan(
            _useOffsets[instructionIndex],
            _useOffsets[instructionIndex + 1] - _useOffsets[instructionIndex]);

    private readonly struct UseVisitor(
        int[] useCount,
        int[] useIndex,
        List<TempVariable> uses,
        int instructionIndex) : ITempUseVisitor
    {
        public void Visit(TempVariable temp)
        {
            uses.Add(temp);
            if ((uint)temp.Index < (uint)useCount.Length)
            {
                useCount[temp.Index]++;
                useIndex[temp.Index] = instructionIndex;
            }
        }
    }
}
