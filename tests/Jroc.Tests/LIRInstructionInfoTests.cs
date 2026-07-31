using Acornima.Ast;
using Jroc.DebugSymbols;
using Jroc.IL;
using Jroc.IR;
using Jroc.Services;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class LIRInstructionInfoTests
{
    [Fact]
    public void KnownInstructionTypes_ExhaustivelyMatchConcreteLirTypes()
    {
        var actual = typeof(LIRInstruction).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsAbstract: false }
                && typeof(LIRInstruction).IsAssignableFrom(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
        var known = LIRInstructionInfo.KnownInstructionTypes
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(actual, known);
        Assert.Equal(known.Length, known.Distinct().Count());
    }

    [Fact]
    public void TypedNumericInstructions_ArePureAndNonBoundary()
    {
        var metadata = LIRInstructionInfo.GetMetadata(new LIRAddNumber(
            new TempVariable(0),
            new TempVariable(1),
            new TempVariable(2)));

        Assert.Equal(LIRInstructionEffects.None, metadata.Effects);
        Assert.Equal(InstructionDisposition.EmitNormally, metadata.DefaultDisposition);
        Assert.Equal(LIRImplicitStackInput.None, metadata.ImplicitStackInput);
        Assert.Equal(LIRDefinitionKind.InstructionResult, metadata.DefinitionKind);
        Assert.Equal(new LIRStackSignature(Pops: 2, Pushes: 1), metadata.StackSignature);
        Assert.False(metadata.IsSchedulingBoundary);
    }

    [Fact]
    public void CallInstructions_HaveConservativeCallEffects()
    {
        var metadata = LIRInstructionInfo.GetMetadata(new LIRCallMember0(
            new TempVariable(0),
            "method",
            new TempVariable(1)));

        AssertEffects(
            metadata,
            LIRInstructionEffects.Calls
                | LIRInstructionEffects.MayThrow
                | LIRInstructionEffects.ReadsHeap
                | LIRInstructionEffects.WritesHeap);
        Assert.False(metadata.IsSchedulingBoundary);
    }

    [Fact]
    public void MutableAndHeapStores_AreClassifiedAsWrites()
    {
        var parameterStore = LIRInstructionInfo.GetMetadata(
            new LIRStoreParameter(1, new TempVariable(0)));
        var itemStore = LIRInstructionInfo.GetMetadata(new LIRSetItem(
            new TempVariable(0),
            new TempVariable(1),
            new TempVariable(2),
            new TempVariable(3)));

        AssertEffects(parameterStore, LIRInstructionEffects.WritesMutableSlot);
        AssertEffects(
            itemStore,
            LIRInstructionEffects.ReadsHeap
                | LIRInstructionEffects.WritesHeap
                | LIRInstructionEffects.MayThrow);
    }

    [Fact]
    public void TdzCheckedScopeLoad_IsMayThrowAndOrderingSensitive()
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript("let value;", "tdz.js");
        var declaration = Assert.IsType<VariableDeclaration>(Assert.Single(program.Body));
        var scope = new Scope("global", ScopeKind.Global, parent: null, program);
        var binding = new BindingInfo("value", BindingKind.Let, scope, declaration)
        {
            RequiresRuntimeTemporalDeadZoneChecks = true
        };
        var metadata = LIRInstructionInfo.GetMetadata(new LIRLoadLeafScopeField(
            binding,
            default,
            new ScopeId("global"),
            new TempVariable(0)));

        AssertEffects(
            metadata,
            LIRInstructionEffects.ReadsScope | LIRInstructionEffects.MayThrow);
        Assert.False(metadata.IsSchedulingBoundary);
    }

    [Fact]
    public void ScopeCreation_IsReplacementBoundary()
    {
        var metadata = LIRInstructionInfo.GetMetadata(
            new LIRCreateLeafScopeInstance(new ScopeId("block")));

        AssertEffects(
            metadata,
            LIRInstructionEffects.ScopeReplacement | LIRInstructionEffects.Allocates);
        Assert.True(metadata.IsSchedulingBoundary);
    }

    [Fact]
    public void CatchStore_ConsumesImplicitExceptionAndDefinesMaterializedResult()
    {
        var instruction = new LIRStoreException(new TempVariable(0));
        var metadata = LIRInstructionInfo.GetMetadata(instruction);

        Assert.Equal(
            LIRImplicitStackInput.CatchException,
            metadata.ImplicitStackInput);
        Assert.Equal(LIRDefinitionKind.CatchException, metadata.DefinitionKind);
        Assert.Equal(new LIRStackSignature(Pops: 1, Pushes: 0), metadata.StackSignature);
        Assert.True(metadata.IsSchedulingBoundary);
        Assert.True(LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(0, defined.Index);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AwaitAndYield_DefineResumeResultsAndAreOpaqueBoundaries(bool isAwait)
    {
        var input = new TempVariable(0);
        var result = new TempVariable(1);
        LIRInstruction instruction = isAwait
            ? new LIRAwait(input, AwaitId: 0, ResumeStateId: 1, ResumeLabelId: 10, result)
            : new LIRYield(input, ResumeStateId: 1, ResumeLabelId: 10, result);

        var metadata = LIRInstructionInfo.GetMetadata(instruction);

        Assert.Equal(LIRDefinitionKind.ResumeResult, metadata.DefinitionKind);
        Assert.True(metadata.Effects.HasFlag(LIRInstructionEffects.Suspension));
        Assert.True(metadata.Effects.HasFlag(LIRInstructionEffects.EmitsInternalControlFlow));
        Assert.True(metadata.IsSchedulingBoundary);
        Assert.True(LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(result, defined);
        Assert.True(LIRInstructionInfo.UsesTemp(instruction, input));
    }

    [Fact]
    public void CatchUnwrap_UsesExceptionDefinesResultAndEmitsInternalControlFlow()
    {
        var exception = new TempVariable(0);
        var result = new TempVariable(1);
        var instruction = new LIRUnwrapCatchException(exception, result);
        var metadata = LIRInstructionInfo.GetMetadata(instruction);

        Assert.True(LIRInstructionInfo.UsesTemp(instruction, exception));
        Assert.True(LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined));
        Assert.Equal(result, defined);
        Assert.True(metadata.Effects.HasFlag(LIRInstructionEffects.EmitsInternalControlFlow));
        Assert.True(metadata.IsSchedulingBoundary);
    }

    [Fact]
    public void UnknownInstruction_FailsClosedAsUnsupportedBoundary()
    {
        var metadata = LIRInstructionInfo.GetMetadata(new UnknownInstruction());

        Assert.Equal(
            LIRInstructionEffects.UnsupportedBarrier,
            metadata.Effects);
        Assert.True(metadata.IsSchedulingBoundary);
        Assert.False(LIRInstructionInfo.IsKnownInstructionType(typeof(UnknownInstruction)));
    }

    [Fact]
    public void HardBoundaryMatrix_RemainsOpaqueToGeneralScheduling()
    {
        var value = new TempVariable(0);
        var result = new TempVariable(1);
        LIRInstruction[] boundaries =
        [
            new LIRLabel(1),
            new LIRBranch(1),
            new LIRBranchIfFalse(value, 1),
            new LIRBranchIfTrue(value, 1),
            new LIRLeave(1),
            new LIREndFinally(),
            new LIRReturn(value),
            new LIRReturnUndefinedImmediate(),
            new LIRThrow(value),
            new LIRSequencePoint(SourceSpan.Hidden("boundary.js")),
            new LIRAwait(value, 0, 1, 1, result),
            new LIRYield(value, 1, 1, result),
            new LIRCreateLeafScopeInstance(new ScopeId("block")),
            new LIRCreateScopeInstance(new ScopeId("block"), result),
            new LIRUnwrapCatchException(value, result),
            new UnknownInstruction()
        ];

        Assert.All(
            boundaries,
            instruction => Assert.True(
                LIRInstructionInfo.IsSchedulingBoundary(instruction),
                instruction.GetType().Name));
    }

    [Fact]
    public void SequencePoint_IsAlwaysSchedulingBoundary()
    {
        var metadata = LIRInstructionInfo.GetMetadata(
            new LIRSequencePoint(SourceSpan.Hidden("source.js")));

        Assert.True(metadata.IsSchedulingBoundary);
    }

    private static void AssertEffects(
        LIRInstructionMetadata metadata,
        LIRInstructionEffects expected)
        => Assert.Equal(expected, metadata.Effects);

    private sealed record UnknownInstruction : LIRInstruction;
}
