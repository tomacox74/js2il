using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.IR;
using Jroc.Services.ILGenerators;
using Jroc.Services.TwoPhaseCompilation;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private bool TryEmitGeneratedArrowFunctionObject(
        LIRCreateBoundArrowFunction createArrow,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (!_generatedFunctionObjectRegistry.TryGetMetadata(
                createArrow.CallableId,
                out var metadata))
        {
            return false;
        }

        foreach (var capture in metadata.Plan.Captures)
        {
            EmitLoadTemp(
                createArrow.ScopesArray,
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.LoadConstantI4(capture.ScopeIndex);
            ilEncoder.OpCode(ILOpCode.Ldelem_ref);
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(_scopeMetadataRegistry.GetScopeTypeHandle(capture.ScopeName));
        }

        foreach (var state in metadata.Plan.StateFields)
        {
            EmitArrowStateArgument(
                state.Kind,
                ilEncoder,
                createArrow.ScopesArray,
                allocation,
                methodDescriptor);
        }

        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(metadata.ConstructorHandle);
        EmitInitializeGeneratedArrowFunctionInstance(
            createArrow.CallableId,
            createArrow.IsAsync,
            metadata,
            ilEncoder);
        return true;
    }

    private void EmitArrowStateArgument(
        GeneratedFunctionStateKind stateKind,
        InstructionEncoder ilEncoder,
        TempVariable scopesArray,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (stateKind)
        {
            case GeneratedFunctionStateKind.LexicalThis:
                EmitLoadArrowLexicalThis(ilEncoder, methodDescriptor);
                break;

            case GeneratedFunctionStateKind.LexicalNewTarget:
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.RuntimeServices),
                    nameof(JavaScriptRuntime.RuntimeServices.GetCurrentNewTarget)));
                break;

            case GeneratedFunctionStateKind.HomeObject:
                EmitLoadArrowLexicalSuperReceiver(ilEncoder, methodDescriptor);
                break;

            case GeneratedFunctionStateKind.LexicalSuperScopes:
                EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                break;

            case GeneratedFunctionStateKind.TransitionalScopeArray:
                EmitLoadTemp(
                    scopesArray,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                break;

            case GeneratedFunctionStateKind.PrivateBrand:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported generated arrow state kind '{stateKind}'.");
        }
    }

    private void EmitLoadArrowLexicalThis(
        InstructionEncoder ilEncoder,
        MethodDescriptor methodDescriptor)
    {
        if (methodDescriptor.IsDerivedConstructor || methodDescriptor.IsStatic)
        {
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis)));
            return;
        }

        ilEncoder.LoadArgument(0);
    }

    private void EmitLoadArrowLexicalSuperReceiver(
        InstructionEncoder ilEncoder,
        MethodDescriptor methodDescriptor)
    {
        if (methodDescriptor.IsDerivedConstructor || !methodDescriptor.IsStatic)
        {
            ilEncoder.LoadArgument(0);
            return;
        }

        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentLexicalSuperReceiver)));
    }

    private void EmitInitializeGeneratedArrowFunctionInstance(
        CallableId callableId,
        bool isAsync,
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder ilEncoder)
    {
        ilEncoder.LoadConstantI4(GetExpectedFunctionLength(callableId));
        ilEncoder.OpCode(ILOpCode.Conv_r8);
        ilEncoder.Ldstr(_metadataBuilder, GetFunctionName(callableId));
        ilEncoder.LoadConstantI4(metadata.Plan.RequiresInvocationContext ? 1 : 0);
        ilEncoder.LoadConstantI4(callableId.HasRestrictedFunctionProperties ? 1 : 0);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
            isAsync
                ? typeof(JavaScriptRuntime.AsyncFunction)
                : typeof(JavaScriptRuntime.Function),
            nameof(JavaScriptRuntime.Function.InitializeFunctionInstance),
            new[]
            {
                typeof(object),
                typeof(double),
                typeof(string),
                typeof(bool),
                typeof(bool)
            }));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Function),
            nameof(JavaScriptRuntime.Function.MarkUndefinedPrototype),
            new[] { typeof(object) }));
        ilEncoder.OpCode(ILOpCode.Castclass);
        ilEncoder.Token(metadata.TypeHandle);
    }
}
