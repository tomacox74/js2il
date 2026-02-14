using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

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

                    // Static functions: scopes is arg0. Instance constructors: scopes is arg1.
                    ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1);
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

                    // new.target follows scopes in the method signature
                    // Static functions: newTarget is arg1 (after scopes at arg0)
                    // Instance constructors: newTarget is arg2 (after this at arg0, scopes at arg1)
                    int newTargetArgIndex = methodDescriptor.IsStatic ? 1 : 2;
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
