using Jroc.IR;
using Jroc.Services.ILGenerators;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities.Ecma335;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_ParametersAndThis(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRBuildScopesArray buildScopes:
                {
                    if (!IsMaterialized(buildScopes.Result, allocation))
                    {
                        break;
                    }

                    if (buildScopes.Slots.Count == 0)
                    {
                        // Empty scopes array - create 1-element array with null for ABI compatibility
                        // (Functions always expect at least a 1-element array)
                        EmitEmptyScopesArray(ilEncoder);
                    }
                    else
                    {
                        EmitPopulateScopesArray(ilEncoder, buildScopes.Slots, methodDescriptor, allocation);
                    }

                    EmitStoreTemp(buildScopes.Result, ilEncoder, allocation);
                    break;
                }
            case LIRLoadThis loadThis:
                {
                    if (!IsMaterialized(loadThis.Result, allocation))
                    {
                        break;
                    }

                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.RuntimeServices), nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);
                        ilEncoder.Call(_bclReferences.RuntimeServices_ResolveLexicalThis_Ref);
                        EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
                        break;
                    }

                    if (methodDescriptor.IsDerivedConstructor)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.RuntimeServices), nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        var resolveThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.ResolveLexicalThis),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(resolveThisRef);
                        EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
                        break;
                    }

                    ilEncoder.LoadArgument(0);
                    EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadScopesArgument loadScopesArg:
                {
                    if (!IsMaterialized(loadScopesArg.Result, allocation))
                    {
                        break;
                    }

                    if (!methodDescriptor.HasScopesParameter)
                    {
                        return false;
                    }

                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    EmitStoreTemp(loadScopesArg.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadNewTarget loadNewTarget:
                {
                    if (!IsMaterialized(loadNewTarget.Result, allocation))
                    {
                        break;
                    }

                    if (!methodDescriptor.HasNewTargetParameter)
                    {
                        return false;
                    }

                    // new.target follows scopes when a scopes parameter exists.
                    int newTargetArgIndex = methodDescriptor.IsStatic
                        ? methodDescriptor.HasScopesParameter ? 1 : 0
                        : methodDescriptor.HasScopesParameter ? 2 : 1;
                    ilEncoder.LoadArgument(newTargetArgIndex);
                    EmitStoreTemp(loadNewTarget.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadParameter loadParam:
                {
                    if (!IsMaterialized(loadParam.Result, allocation))
                    {
                        break;
                    }

                    int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, loadParam.ParameterIndex);
                    ilEncoder.LoadArgument(ilArgIndex);
                    if (GetTempStorage(loadParam.Result).ClrType
                        == typeof(JavaScriptRuntime.BuiltinDelegateFunctionAdapter))
                    {
                        EmitMaterializeRequireFunctionValue(ilEncoder);
                    }
                    EmitStoreTemp(loadParam.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreParameter storeParam:
                {
                    int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, storeParam.ParameterIndex);
                    EmitLoadTemp(storeParam.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.StoreArgument(ilArgIndex);
                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
