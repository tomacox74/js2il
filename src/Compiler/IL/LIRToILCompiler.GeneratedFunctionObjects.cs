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

        EmitGeneratedFunctionObjectConstructorArguments(
            metadata,
            createArrow.ScopesArray,
            isArrow: true,
            homeObject: null,
            privateBrand: null,
            ilEncoder,
            allocation,
            methodDescriptor);

        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(metadata.ConstructorHandle);
        EmitInitializeGeneratedFunctionInstance(
            createArrow.CallableId,
            createArrow.IsAsync,
            markUndefinedPrototype: true,
            metadata,
            ilEncoder);
        return true;
    }

    private bool TryEmitGeneratedOrdinaryFunctionObject(
        LIRCreateBoundFunctionExpression createFunction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (createFunction.IsAsync
            || IsGeneratorCallable(createFunction.CallableId)
            || !_generatedFunctionObjectRegistry.TryGetMetadata(
                createFunction.CallableId,
                out var metadata))
        {
            return false;
        }

        EmitGeneratedFunctionObjectConstructorArguments(
            metadata,
            createFunction.ScopesArray,
            isArrow: false,
            createFunction.HomeObject,
            createFunction.PrivateBrand,
            ilEncoder,
            allocation,
            methodDescriptor);

        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(metadata.ConstructorHandle);
        EmitInitializeGeneratedFunctionInstance(
            createFunction.CallableId,
            isAsync: false,
            markUndefinedPrototype: false,
            metadata,
            ilEncoder,
            createFunction.FunctionName);
        return true;
    }

    private void EmitGeneratedFunctionObjectConstructorArguments(
        GeneratedFunctionObjectMetadata metadata,
        TempVariable scopesArray,
        bool isArrow,
        TempVariable? homeObject,
        TempVariable? privateBrand,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        foreach (var capture in metadata.Plan.Captures)
        {
            EmitLoadTemp(
                scopesArray,
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
            EmitGeneratedFunctionStateArgument(
                state.Kind,
                isArrow,
                homeObject,
                privateBrand,
                ilEncoder,
                scopesArray,
                allocation,
                methodDescriptor);
        }

    }

    private void EmitGeneratedFunctionStateArgument(
        GeneratedFunctionStateKind stateKind,
        bool isArrow,
        TempVariable? homeObject,
        TempVariable? privateBrand,
        InstructionEncoder ilEncoder,
        TempVariable scopesArray,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (stateKind)
        {
            case GeneratedFunctionStateKind.LexicalThis:
                if (!isArrow)
                {
                    throw new InvalidOperationException(
                        "Ordinary function object unexpectedly requested lexical this state.");
                }
                EmitLoadArrowLexicalThis(ilEncoder, methodDescriptor);
                break;

            case GeneratedFunctionStateKind.LexicalNewTarget:
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.RuntimeServices),
                    nameof(JavaScriptRuntime.RuntimeServices.GetCurrentNewTarget)));
                break;

            case GeneratedFunctionStateKind.HomeObject:
                if (homeObject.HasValue)
                {
                    EmitLoadTemp(
                        homeObject.Value,
                        ilEncoder,
                        allocation,
                        methodDescriptor);
                }
                else
                {
                    EmitLoadArrowLexicalSuperReceiver(ilEncoder, methodDescriptor);
                }
                break;

            case GeneratedFunctionStateKind.LexicalSuperScopes:
                if (homeObject.HasValue)
                {
                    EmitLoadTemp(
                        scopesArray,
                        ilEncoder,
                        allocation,
                        methodDescriptor);
                }
                else
                {
                    EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                }
                break;

            case GeneratedFunctionStateKind.TransitionalScopeArray:
                EmitLoadTemp(
                    scopesArray,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                break;

            case GeneratedFunctionStateKind.PrivateBrand:
                if (privateBrand.HasValue)
                {
                    EmitLoadTemp(
                        privateBrand.Value,
                        ilEncoder,
                        allocation,
                        methodDescriptor);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                }
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

    private void EmitInitializeGeneratedFunctionInstance(
        CallableId callableId,
        bool isAsync,
        bool markUndefinedPrototype,
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder ilEncoder,
        string? functionName = null)
    {
        ilEncoder.LoadConstantI4(GetExpectedFunctionLength(callableId));
        ilEncoder.OpCode(ILOpCode.Conv_r8);
        ilEncoder.Ldstr(
            _metadataBuilder,
            functionName ?? GetFunctionName(callableId));
        ilEncoder.LoadConstantI4(metadata.Plan.RequiresInvocationContext ? 1 : 0);
        ilEncoder.LoadConstantI4(callableId.HasRestrictedFunctionProperties ? 1 : 0);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddGenericFunctionInitializer(
            isAsync
                ? typeof(JavaScriptRuntime.AsyncFunction)
                : typeof(JavaScriptRuntime.Function),
            nameof(JavaScriptRuntime.Function.InitializeFunctionInstance),
            metadata.TypeHandle,
            isValueType: false));
        if (markUndefinedPrototype)
        {
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(_memberRefRegistry.GetOrAddGenericUnaryMethod(
                typeof(JavaScriptRuntime.Function),
                nameof(JavaScriptRuntime.Function.MarkUndefinedPrototype),
                metadata.TypeHandle,
                isValueType: false));
        }
    }
}
