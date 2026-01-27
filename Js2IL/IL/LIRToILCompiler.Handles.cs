using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Handle Resolution Helpers

    /// <summary>
    /// Resolves a scope type handle from the registry with improved error context.
    /// </summary>
    private TypeDefinitionHandle ResolveScopeTypeHandle(string scopeName, string context)
    {
        try
        {
            return _scopeMetadataRegistry.GetScopeTypeHandle(scopeName);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve scope type handle for '{scopeName}' during {context}.",
                ex);
        }
    }

    /// <summary>
    /// Resolves a field handle from the registry with improved error context.
    /// </summary>
    private EntityHandle ResolveFieldToken(string scopeName, string fieldName, string context)
    {
        try
        {
            if (TryResolveAsyncScopeBaseFieldToken(fieldName, out var token))
                return token;

            if (TryResolveGeneratorScopeBaseFieldToken(fieldName, out token))
                return token;

            return _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve field token for '{fieldName}' in scope '{scopeName}' during {context}.",
                ex);
        }
    }

    /// <summary>
    /// Emits IL to load a field by name from the scope instance (local 0).
    /// Assumes the scope instance is already on the stack.
    /// </summary>
    private void EmitLoadFieldByName(InstructionEncoder ilEncoder, string scopeName, string fieldName)
    {
        var fieldHandle = TryResolveAsyncScopeBaseFieldToken(fieldName, out var token)
            ? token
            : TryResolveGeneratorScopeBaseFieldToken(fieldName, out token)
                ? token
                : _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        ilEncoder.OpCode(ILOpCode.Ldfld);
        ilEncoder.Token(fieldHandle);
    }

    /// <summary>
    /// Emits IL to store to a field by name on the scope instance.
    /// Assumes the scope instance and value are on the stack (scope, value).
    /// </summary>
    private void EmitStoreFieldByName(InstructionEncoder ilEncoder, string scopeName, string fieldName)
    {
        var fieldHandle = TryResolveAsyncScopeBaseFieldToken(fieldName, out var token)
            ? token
            : TryResolveGeneratorScopeBaseFieldToken(fieldName, out token)
                ? token
                : _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        ilEncoder.OpCode(ILOpCode.Stfld);
        ilEncoder.Token(fieldHandle);
    }

    /// <summary>
    /// Emits the state switch at the entry of an async function.
    /// This dispatches to the appropriate resume point based on _asyncState.
    /// State 0 = initial entry (fall through to function body)
    /// State 1, 2, 3, ... = resume points after each await
    /// </summary>
    private void EmitAsyncStateSwitch(
        InstructionEncoder ilEncoder,
        Dictionary<int, LabelHandle> labelMap,
        AsyncStateMachineInfo asyncInfo)
    {
        var scopeName = MethodBody.LeafScopeId.Name;
        
        // Load _asyncState from scope instance (local 0)
        ilEncoder.LoadLocal(0);
        EmitLoadFieldByName(ilEncoder, scopeName, "_asyncState");
        
        // Build switch table for resume states.
        // The switch instruction expects targets for cases 0, 1, 2, ...
        // Case 0 = initial entry (fall through - we'll use a label that goes right after the switch)
        // Case 1, 2, ... = resume points
        
        var fallThroughLabel = ilEncoder.DefineLabel();
        int branchCount = asyncInfo.MaxResumeStateId + 1;
        
        // Collect switch targets
        var switchTargets = new LabelHandle[branchCount];
        
        // Default all cases to fall through to function body
        for (int i = 0; i < branchCount; i++)
        {
            switchTargets[i] = fallThroughLabel;
        }
        
        // Cases 1, 2, 3, ...: jump to resume labels
        foreach (var kvp in asyncInfo.ResumeLabels)
        {
            var stateId = kvp.Key;
            var labelId = kvp.Value;
            if (stateId <= 0 || stateId >= branchCount)
            {
                continue;
            }
            if (!labelMap.TryGetValue(labelId, out var resumeLabel))
            {
                resumeLabel = ilEncoder.DefineLabel();
                labelMap[labelId] = resumeLabel;
            }
            switchTargets[stateId] = resumeLabel;
        }
        
        // Emit switch instruction using SwitchInstructionEncoder
        var switchEncoder = ilEncoder.Switch(branchCount);
        for (int i = 0; i < branchCount; i++)
        {
            switchEncoder.Branch(switchTargets[i]);
        }
        
        // Mark the fall-through label (for state 0 or values > max state)
        ilEncoder.MarkLabel(fallThroughLabel);
    }

    #endregion
}