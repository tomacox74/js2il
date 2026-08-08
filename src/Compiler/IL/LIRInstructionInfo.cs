using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Jroc.IR;

namespace Jroc.IL;

[Flags]
internal enum LIRInstructionEffects
{
    None = 0,
    ReadsMutableSlot = 1 << 0,
    WritesMutableSlot = 1 << 1,
    ReadsScope = 1 << 2,
    WritesScope = 1 << 3,
    ReadsHeap = 1 << 4,
    WritesHeap = 1 << 5,
    Calls = 1 << 6,
    MayThrow = 1 << 7,
    Allocates = 1 << 8,
    ControlFlow = 1 << 9,
    Suspension = 1 << 10,
    ScopeReplacement = 1 << 11,
    EmitsInternalControlFlow = 1 << 12,
    UnsupportedBarrier = 1 << 13
}

internal enum LIRImplicitStackInput
{
    None,
    CatchException
}

internal enum LIRDefinitionKind
{
    None,
    InstructionResult,
    CatchException,
    ResumeResult
}

internal readonly record struct LIRStackSignature(int Pops, int Pushes);

internal readonly record struct LIRInstructionMetadata(
    LIRInstructionEffects Effects,
    InstructionDisposition DefaultDisposition,
    LIRImplicitStackInput ImplicitStackInput,
    LIRDefinitionKind DefinitionKind,
    LIRStackSignature StackSignature,
    bool IsSchedulingBoundary);

/// <summary>
/// Canonical scheduler-facing metadata for every concrete LIR instruction.
/// Unknown or newly-added instructions fail closed until added to the explicit
/// inventory and classified.
/// </summary>
internal static partial class LIRInstructionInfo
{
    private const LIRInstructionEffects CallEffects =
        LIRInstructionEffects.Calls
        | LIRInstructionEffects.MayThrow
        | LIRInstructionEffects.ReadsHeap
        | LIRInstructionEffects.WritesHeap;

    private const LIRInstructionEffects HeapReadEffects =
        LIRInstructionEffects.ReadsHeap
        | LIRInstructionEffects.MayThrow;

    private const LIRInstructionEffects HeapWriteEffects =
        LIRInstructionEffects.ReadsHeap
        | LIRInstructionEffects.WritesHeap
        | LIRInstructionEffects.MayThrow;

    private static readonly Type[] _knownInstructionTypes =
    {
        typeof(LIRAddAndToNumber),
        typeof(LIRAddDynamic),
        typeof(LIRAddDynamicDoubleObject),
        typeof(LIRAddDynamicObjectDouble),
        typeof(LIRAddNumber),
        typeof(LIRArrayAdd),
        typeof(LIRArrayPushRange),
        typeof(LIRAsyncCallMoveNext),
        typeof(LIRAsyncInitialize),
        typeof(LIRAsyncLoadAwaitedResult),
        typeof(LIRAsyncLoadState),
        typeof(LIRAsyncReject),
        typeof(LIRAsyncResolve),
        typeof(LIRAsyncReturnPromise),
        typeof(LIRAsyncStateSwitch),
        typeof(LIRAsyncStoreAwaitedResult),
        typeof(LIRAsyncStoreState),
        typeof(LIRAwait),
        typeof(LIRBinaryDynamicOperator),
        typeof(LIRBitwiseAnd),
        typeof(LIRBitwiseNotDynamic),
        typeof(LIRBitwiseNotNumber),
        typeof(LIRBitwiseOr),
        typeof(LIRBitwiseXor),
        typeof(LIRBranch),
        typeof(LIRBranchIfFalse),
        typeof(LIRBranchIfTrue),
        typeof(LIRBuildArray),
        typeof(LIRBuildScopesArray),
        typeof(LIRCallDeclaredCallable),
        typeof(LIRCallFunction),
        typeof(LIRCallFunctionBaseConstructor),
        typeof(LIRCallFunctionValue),
        typeof(LIRCallFunctionValue0),
        typeof(LIRCallFunctionValue1),
        typeof(LIRCallFunctionValue2),
        typeof(LIRCallFunctionValue3),
        typeof(LIRCallFunctionWithArgsArray),
        typeof(LIRCallImport),
        typeof(LIRCallInstanceMethod),
        typeof(LIRCallIntrinsic),
        typeof(LIRCallIntrinsicBaseConstructor),
        typeof(LIRCallIntrinsicGlobalFunction),
        typeof(LIRCallIntrinsicStatic),
        typeof(LIRCallIntrinsicStaticVoid),
        typeof(LIRCallIntrinsicStaticVoidWithArgsArray),
        typeof(LIRCallIntrinsicStaticWithArgsArray),
        typeof(LIRCallIsTruthy),
        typeof(LIRCallIsTruthyBool),
        typeof(LIRCallIsTruthyDouble),
        typeof(LIRCallMember),
        typeof(LIRCallMember0),
        typeof(LIRCallMember1),
        typeof(LIRCallMember2),
        typeof(LIRCallMember3),
        typeof(LIRCallNodeModuleContractMember),
        typeof(LIRCallRequire),
        typeof(LIRCallRuntimeServicesStatic),
        typeof(LIRCallTypedMember),
        typeof(LIRCallTypedMemberWithFallback),
        typeof(LIRCallUserClassBaseConstructor),
        typeof(LIRCallUserClassBaseInstanceMethod),
        typeof(LIRCallUserClassInstanceMethod),
        typeof(LIRCompareBooleanEqual),
        typeof(LIRCompareBooleanNotEqual),
        typeof(LIRCompareNumberEqual),
        typeof(LIRCompareNumberGreaterThan),
        typeof(LIRCompareNumberGreaterThanOrEqual),
        typeof(LIRCompareNumberLessThan),
        typeof(LIRCompareNumberLessThanOrEqual),
        typeof(LIRCompareNumberNotEqual),
        typeof(LIRConcatStrings),
        typeof(LIRConstBoolean),
        typeof(LIRConstNull),
        typeof(LIRConstNumber),
        typeof(LIRConstructValue),
        typeof(LIRConstString),
        typeof(LIRConstUndefined),
        typeof(LIRConvertToBoolean),
        typeof(LIRConvertToNumber),
        typeof(LIRConvertToNumberDiscard),
        typeof(LIRConvertToObject),
        typeof(LIRConvertToString),
        typeof(LIRCopyTemp),
        typeof(LIRCreateBoundArrowFunction),
        typeof(LIRCreateBoundFunctionExpression),
        typeof(LIRCreateLeafScopeInstance),
        typeof(LIRCreateScopeInstance),
        typeof(LIRDivNumber),
        typeof(LIREndFinally),
        typeof(LIREqualDynamic),
        typeof(LIRExpNumber),
        typeof(LIRGeneratorStateSwitch),
        typeof(LIRGetInferredMember),
        typeof(LIRGetInt32ArrayElement),
        typeof(LIRGetInt32ArrayLength),
        typeof(LIRGetIntrinsicGlobal),
        typeof(LIRGetIntrinsicGlobalFunction),
        typeof(LIRGetItem),
        typeof(LIRGetItemAsNumber),
        typeof(LIRGetItemAsNumberString),
        typeof(LIRGetJsArrayElement),
        typeof(LIRGetJsArrayLength),
        typeof(LIRGetLength),
        typeof(LIRGetStringLength),
        typeof(LIRGetUserClassType),
        typeof(LIRInOperator),
        typeof(LIRInstanceOfOperator),
        typeof(LIRIsInstanceOf),
        typeof(LIRLabel),
        typeof(LIRLeave),
        typeof(LIRLeftShift),
        typeof(LIRLoadLeafScopeField),
        typeof(LIRLoadNewTarget),
        typeof(LIRLoadParameter),
        typeof(LIRLoadParentScopeField),
        typeof(LIRLoadScopeField),
        typeof(LIRLoadScopeFieldByName),
        typeof(LIRLoadScopesArgument),
        typeof(LIRLoadThis),
        typeof(LIRLoadUserClassInstanceField),
        typeof(LIRLoadUserClassStaticField),
        typeof(LIRLogicalNot),
        typeof(LIRModNumber),
        typeof(LIRMulDynamic),
        typeof(LIRMulNumber),
        typeof(LIRNegateNumber),
        typeof(LIRNegateNumberDynamic),
        typeof(LIRNewBuiltInError),
        typeof(LIRNewInferredJsObject),
        typeof(LIRNewIntrinsicObject),
        typeof(LIRNewJsArray),
        typeof(LIRNewJsObject),
        typeof(LIRNewUserClass),
        typeof(LIRNotEqualDynamic),
        typeof(LIRReturn),
        typeof(LIRReturnUndefinedImmediate),
        typeof(LIRRightShift),
        typeof(LIRSequencePoint),
        typeof(LIRSetInferredMember),
        typeof(LIRSetInt32ArrayElement),
        typeof(LIRSetItem),
        typeof(LIRSetJsArrayElement),
        typeof(LIRSetJsArrayLength),
        typeof(LIRStoreException),
        typeof(LIRStoreLeafScopeField),
        typeof(LIRStoreParameter),
        typeof(LIRStoreParentScopeField),
        typeof(LIRStoreScopeField),
        typeof(LIRStoreScopeFieldByName),
        typeof(LIRStoreUserClassInstanceField),
        typeof(LIRStoreUserClassStaticField),
        typeof(LIRStrictEqualDynamic),
        typeof(LIRStrictNotEqualDynamic),
        typeof(LIRSubNumber),
        typeof(LIRTailCallFunctionReturn),
        typeof(LIRThrow),
        typeof(LIRThrowNewTypeError),
        typeof(LIRTypeof),
        typeof(LIRUnsignedRightShift),
        typeof(LIRUnwrapCatchException),
        typeof(LIRYield)
    };

    private static readonly HashSet<Type> _knownInstructionTypeSet =
        new(_knownInstructionTypes);

    private static readonly ReadOnlyCollection<Type> _readOnlyKnownInstructionTypes =
        Array.AsReadOnly(_knownInstructionTypes);

    private static readonly HashSet<Type> _schedulingBoundaryTypeSet =
    [
        typeof(LIRLabel),
        typeof(LIRBranch),
        typeof(LIRBranchIfFalse),
        typeof(LIRBranchIfTrue),
        typeof(LIRLeave),
        typeof(LIREndFinally),
        typeof(LIRReturn),
        typeof(LIRReturnUndefinedImmediate),
        typeof(LIRTailCallFunctionReturn),
        typeof(LIRThrow),
        typeof(LIRThrowNewTypeError),
        typeof(LIRSequencePoint),
        typeof(LIRStoreException),
        typeof(LIRUnwrapCatchException),
        typeof(LIRAwait),
        typeof(LIRYield),
        typeof(LIRGeneratorStateSwitch),
        typeof(LIRAsyncInitialize),
        typeof(LIRAsyncCallMoveNext),
        typeof(LIRAsyncReturnPromise),
        typeof(LIRAsyncLoadState),
        typeof(LIRAsyncStoreState),
        typeof(LIRAsyncResolve),
        typeof(LIRAsyncReject),
        typeof(LIRAsyncStateSwitch),
        typeof(LIRAsyncStoreAwaitedResult),
        typeof(LIRAsyncLoadAwaitedResult),
        typeof(LIRCallNodeModuleContractMember),
        typeof(LIRCreateLeafScopeInstance),
        typeof(LIRCreateScopeInstance)
    ];

    private static readonly ConcurrentDictionary<Type, LIRInstructionEffects>
        _staticEffectsByType = new();

    internal static IReadOnlyList<Type> KnownInstructionTypes =>
        _readOnlyKnownInstructionTypes;

    internal static bool IsKnownInstructionType(Type type)
        => _knownInstructionTypeSet.Contains(type);

    internal static LIRInstructionMetadata GetMetadata(LIRInstruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);

        var instructionType = instruction.GetType();
        if (!_knownInstructionTypeSet.Contains(instructionType))
        {
            return new LIRInstructionMetadata(
                LIRInstructionEffects.UnsupportedBarrier,
                InstructionDisposition.EmitNormally,
                LIRImplicitStackInput.None,
                LIRDefinitionKind.None,
                new LIRStackSignature(Pops: 0, Pushes: 0),
                IsSchedulingBoundary: true);
        }

        var effects = GetEffects(instruction);
        var implicitInput = instruction is LIRStoreException
            ? LIRImplicitStackInput.CatchException
            : LIRImplicitStackInput.None;
        var hasDefinition = TryGetDefinedTemp(instruction, out _);
        var definitionKind = instruction switch
        {
            LIRStoreException => LIRDefinitionKind.CatchException,
            LIRAwait or LIRYield => LIRDefinitionKind.ResumeResult,
            _ when hasDefinition => LIRDefinitionKind.InstructionResult,
            _ => LIRDefinitionKind.None
        };
        var stackSignature = GetStackSignature(
            instruction,
            implicitInput,
            hasDefinition);

        return new LIRInstructionMetadata(
            effects,
            InstructionDisposition.EmitNormally,
            implicitInput,
            definitionKind,
            stackSignature,
            IsSchedulingBoundary(instruction));
    }

    internal static bool IsSchedulingBoundary(LIRInstruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);

        var instructionType = instruction.GetType();
        return !_knownInstructionTypeSet.Contains(instructionType)
            || _schedulingBoundaryTypeSet.Contains(instructionType);
    }

    internal static LIRInstructionEffects GetEffectsForScheduling(
        LIRInstruction instruction)
    {
        ArgumentNullException.ThrowIfNull(instruction);
        return IsKnownInstructionType(instruction.GetType())
            ? GetEffects(instruction)
            : LIRInstructionEffects.UnsupportedBarrier;
    }

    private static LIRStackSignature GetStackSignature(
        LIRInstruction instruction,
        LIRImplicitStackInput implicitInput,
        bool hasDefinition)
    {
        var visitor = new TempCountVisitor();
        VisitUsedTemps(instruction, ref visitor);
        var pops = visitor.Count
            + (implicitInput == LIRImplicitStackInput.CatchException ? 1 : 0);
        var pushes = instruction is LIRStoreException or LIRAwait or LIRYield
            ? 0
            : hasDefinition ? 1 : 0;
        return new LIRStackSignature(pops, pushes);
    }

    private static LIRInstructionEffects GetEffects(LIRInstruction instruction)
    {
        // Runtime TDZ requirements are binding-instance data, so these three
        // instruction families cannot use the otherwise type-stable cache.
        if (instruction is LIRLoadLeafScopeField loadLeaf)
        {
            return LIRInstructionEffects.ReadsScope
                | (loadLeaf.Binding.RequiresRuntimeTemporalDeadZoneChecks
                    ? LIRInstructionEffects.MayThrow
                    : LIRInstructionEffects.None);
        }

        if (instruction is LIRLoadParentScopeField loadParent)
        {
            return LIRInstructionEffects.ReadsScope
                | (loadParent.Binding.RequiresRuntimeTemporalDeadZoneChecks
                    ? LIRInstructionEffects.MayThrow
                    : LIRInstructionEffects.None);
        }

        if (instruction is LIRLoadScopeField loadScope)
        {
            return LIRInstructionEffects.ReadsScope
                | (loadScope.Binding.RequiresRuntimeTemporalDeadZoneChecks
                    ? LIRInstructionEffects.MayThrow
                    : LIRInstructionEffects.None);
        }

        return _staticEffectsByType.GetOrAdd(
            instruction.GetType(),
            static (_, value) => GetStaticEffects(value),
            instruction);
    }

    private static LIRInstructionEffects GetStaticEffects(LIRInstruction instruction)
        => instruction switch
        {
            // Native constants and typed operators are pure and non-throwing.
            LIRConstNumber
                or LIRConstString
                or LIRConstBoolean
                or LIRConstUndefined
                or LIRConstNull
                or LIRAddNumber
                or LIRSubNumber
                or LIRMulNumber
                or LIRDivNumber
                or LIRModNumber
                or LIRExpNumber
                or LIRBitwiseAnd
                or LIRBitwiseOr
                or LIRBitwiseXor
                or LIRLeftShift
                or LIRRightShift
                or LIRUnsignedRightShift
                or LIRCompareNumberLessThan
                or LIRCompareNumberGreaterThan
                or LIRCompareNumberLessThanOrEqual
                or LIRCompareNumberGreaterThanOrEqual
                or LIRCompareNumberEqual
                or LIRCompareNumberNotEqual
                or LIRCompareBooleanEqual
                or LIRCompareBooleanNotEqual
                or LIRNegateNumber
                or LIRBitwiseNotNumber
                or LIRIsInstanceOf
                or LIRTypeof
                or LIRCopyTemp
                => LIRInstructionEffects.None,

            LIRLoadParameter
                or LIRLoadThis
                or LIRLoadScopesArgument
                or LIRLoadNewTarget
                => LIRInstructionEffects.ReadsMutableSlot,

            LIRStoreParameter
                => LIRInstructionEffects.WritesMutableSlot,

            LIRLoadLeafScopeField
                or LIRLoadParentScopeField
                or LIRLoadScopeField
                => LIRInstructionEffects.ReadsScope,
            LIRLoadScopeFieldByName => LIRInstructionEffects.ReadsScope,

            LIRStoreLeafScopeField
                or LIRStoreParentScopeField
                or LIRStoreScopeField
                or LIRStoreScopeFieldByName
                => LIRInstructionEffects.WritesScope,

            LIRCreateLeafScopeInstance
                or LIRCreateScopeInstance
                => LIRInstructionEffects.ScopeReplacement
                    | LIRInstructionEffects.Allocates,

            LIRLabel
                or LIRBranch
                or LIRBranchIfFalse
                or LIRBranchIfTrue
                or LIRLeave
                or LIREndFinally
                or LIRReturn
                or LIRReturnUndefinedImmediate
                => LIRInstructionEffects.ControlFlow,

            LIRThrow
                or LIRThrowNewTypeError
                => LIRInstructionEffects.ControlFlow
                    | LIRInstructionEffects.MayThrow,

            LIRSequencePoint => LIRInstructionEffects.None,
            LIRStoreException => LIRInstructionEffects.None,
            LIRUnwrapCatchException =>
                LIRInstructionEffects.EmitsInternalControlFlow
                | LIRInstructionEffects.MayThrow,

            LIRAwait
                or LIRYield
                => LIRInstructionEffects.Suspension
                    | LIRInstructionEffects.EmitsInternalControlFlow
                    | CallEffects,

            LIRGeneratorStateSwitch
                or LIRAsyncInitialize
                or LIRAsyncCallMoveNext
                or LIRAsyncReturnPromise
                or LIRAsyncLoadState
                or LIRAsyncStoreState
                or LIRAsyncResolve
                or LIRAsyncReject
                or LIRAsyncStateSwitch
                or LIRAsyncStoreAwaitedResult
                or LIRAsyncLoadAwaitedResult
                => LIRInstructionEffects.EmitsInternalControlFlow
                    | CallEffects,

            LIRGetItem
                or LIRGetItemAsNumber
                or LIRGetItemAsNumberString
                or LIRGetJsArrayElement
                or LIRGetInt32ArrayElement
                or LIRGetLength
                or LIRGetJsArrayLength
                or LIRGetInt32ArrayLength
                or LIRGetStringLength
                or LIRGetInferredMember
                or LIRLoadUserClassInstanceField
                or LIRLoadUserClassStaticField
                or LIRGetIntrinsicGlobal
                or LIRGetIntrinsicGlobalFunction
                or LIRGetUserClassType
                => HeapReadEffects,

            LIRSetItem
                or LIRSetJsArrayLength
                or LIRSetJsArrayElement
                or LIRSetInt32ArrayElement
                or LIRSetInferredMember
                or LIRArrayPushRange
                or LIRArrayAdd
                or LIRStoreUserClassInstanceField
                or LIRStoreUserClassStaticField
                => HeapWriteEffects,

            LIRBuildArray
                or LIRBuildScopesArray
                or LIRNewJsArray
                or LIRNewJsObject
                or LIRNewInferredJsObject
                => LIRInstructionEffects.Allocates
                    | LIRInstructionEffects.MayThrow,

            LIRNewBuiltInError
                or LIRNewIntrinsicObject
                or LIRNewUserClass
                or LIRConstructValue
                or LIRCreateBoundArrowFunction
                or LIRCreateBoundFunctionExpression
                => CallEffects
                    | LIRInstructionEffects.Allocates,

            LIRCallIntrinsic
                or LIRCallIntrinsicGlobalFunction
                or LIRCallIntrinsicStatic
                or LIRCallIntrinsicStaticWithArgsArray
                or LIRCallIntrinsicStaticVoid
                or LIRCallIntrinsicStaticVoidWithArgsArray
                or LIRCallIntrinsicBaseConstructor
                or LIRCallInstanceMethod
                or LIRCallFunction
                or LIRTailCallFunctionReturn
                or LIRCallFunctionWithArgsArray
                or LIRCallFunctionValue
                or LIRCallFunctionValue0
                or LIRCallFunctionValue1
                or LIRCallFunctionValue2
                or LIRCallFunctionValue3
                or LIRCallRequire
                or LIRCallImport
                or LIRCallMember
                or LIRCallMember0
                or LIRCallMember1
                or LIRCallMember2
                or LIRCallMember3
                or LIRCallNodeModuleContractMember
                or LIRCallTypedMember
                or LIRCallTypedMemberWithFallback
                or LIRCallUserClassInstanceMethod
                or LIRCallUserClassBaseConstructor
                or LIRCallUserClassBaseInstanceMethod
                or LIRCallFunctionBaseConstructor
                or LIRCallDeclaredCallable
                or LIRCallRuntimeServicesStatic
                => CallEffects,

            LIRConcatStrings
                => LIRInstructionEffects.Allocates
                    | LIRInstructionEffects.MayThrow,

            LIRAddAndToNumber
                or LIRAddDynamic
                or LIRAddDynamicDoubleObject
                or LIRAddDynamicObjectDouble
                or LIRMulDynamic
                or LIRBinaryDynamicOperator
                or LIREqualDynamic
                or LIRNotEqualDynamic
                or LIRStrictEqualDynamic
                or LIRStrictNotEqualDynamic
                or LIRInOperator
                or LIRInstanceOfOperator
                or LIRConvertToObject
                or LIRConvertToNumber
                or LIRConvertToNumberDiscard
                or LIRConvertToBoolean
                or LIRConvertToString
                or LIRNegateNumberDynamic
                or LIRBitwiseNotDynamic
                or LIRLogicalNot
                or LIRCallIsTruthy
                or LIRCallIsTruthyDouble
                or LIRCallIsTruthyBool
                => CallEffects,

            _ => LIRInstructionEffects.UnsupportedBarrier
        };

    private struct TempCountVisitor : ITempUseVisitor
    {
        internal int Count { get; private set; }

        public void Visit(TempVariable temp)
        {
            Count++;
        }
    }

    private struct TempMatchVisitor : ITempUseVisitor
    {
        private readonly int _targetIndex;

        internal TempMatchVisitor(int targetIndex)
        {
            _targetIndex = targetIndex;
        }

        internal bool Found { get; private set; }

        public void Visit(TempVariable temp)
        {
            Found |= temp.Index == _targetIndex;
        }
    }
}
