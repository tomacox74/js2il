using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_Return(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRReturn lirReturn:
                // Constructors are void-returning - don't load any value before ret
                if (!methodDescriptor.ReturnsVoid)
                {
                    if (MethodBody.IsGenerator)
                    {
                        // Generator completion: mark done and return { value, done: true }
                        var scopeName = MethodBody.LeafScopeId.Name;

                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.LoadConstantI4(1);
                        var iterCreate = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.IteratorResult),
                            nameof(JavaScriptRuntime.IteratorResult.Create),
                            parameterTypes: new[] { typeof(object), typeof(bool) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(iterCreate);
                    }
                    else if (MethodBody.IsAsync && MethodBody.AsyncInfo is { HasAwaits: true })
                    {
                        // Full async state machine: resolve _deferred and return its promise.
                        // 1. Mark state as completed: _asyncState = -1
                        // 2. Call _deferred.resolve(value)
                        // 3. Return _deferred.promise
                        
                        var scopeName = MethodBody.LeafScopeId.Name;
                        
                        // _asyncState = -1 (completed)
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(-1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");
                        
                        // Load _deferred.resolve (it's a bound closure)
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                        var getResolveRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.resolve)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getResolveRef);
                        
                        // Call it with the return value: Closure.InvokeWithArgs(resolve, scopes, argsArray)
                        // Build a 1-element array containing the return value
                        EmitLoadScopesArray(ilEncoder, methodDescriptor);
                        ilEncoder.LoadConstantI4(1);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadConstantI4(0);
                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        
                        var invokeWithArgsRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                            parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(invokeWithArgsRef);
                        ilEncoder.OpCode(ILOpCode.Pop); // discard result
                        
                        // Return _deferred.promise
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                        var getPromiseRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.promise)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getPromiseRef);
                    }
                    else if (MethodBody.IsAsync)
                    {
                        // async function without awaits (MVP): return Promise.resolve(value)
                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                        var resolveRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.resolve),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(resolveRef);
                    }
                    else
                    {
                        EmitLoadTemp(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                    }
                }
                ilEncoder.OpCode(ILOpCode.Ret);
                break;

            default:
                return null;
        }

        return true;
    }
}
