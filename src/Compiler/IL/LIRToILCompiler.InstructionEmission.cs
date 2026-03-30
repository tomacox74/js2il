using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Instruction Emission

    private bool TryCompileInstructionToIL(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor,
        Dictionary<int, LabelHandle> labelMap,
        StackifyResult stackifyResult)
    {
        var tempsAndExceptionsResult = TryCompileInstructionToIL_TempsAndExceptions(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (tempsAndExceptionsResult.HasValue)
        {
            return tempsAndExceptionsResult.Value;
        }

        var dynamicOperatorsResult = TryCompileInstructionToIL_DynamicOperators(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (dynamicOperatorsResult.HasValue)
        {
            return dynamicOperatorsResult.Value;
        }

        var arrayAndObjectLiteralsResult = TryCompileInstructionToIL_ArrayAndObjectLiterals(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (arrayAndObjectLiteralsResult.HasValue)
        {
            return arrayAndObjectLiteralsResult.Value;
        }

        var collectionsResult = TryCompileInstructionToIL_Collections(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (collectionsResult.HasValue)
        {
            return collectionsResult.Value;
        }

        var userClassFieldsResult = TryCompileInstructionToIL_UserClassFields(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (userClassFieldsResult.HasValue)
        {
            return userClassFieldsResult.Value;
        }

        var callsResult = TryCompileInstructionToIL_Calls(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor,
            stackifyResult);

        if (callsResult.HasValue)
        {
            return callsResult.Value;
        }

        var scopesResult = TryCompileInstructionToIL_Scopes(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (scopesResult.HasValue)
        {
            return scopesResult.Value;
        }

        var leafScopeInstanceResult = TryCompileInstructionToIL_LeafScopeInstance(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor,
            labelMap);

        if (leafScopeInstanceResult.HasValue)
        {
            return leafScopeInstanceResult.Value;
        }

        var asyncAndGeneratorResult = TryCompileInstructionToIL_AsyncAndGenerator(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor,
            labelMap);

        if (asyncAndGeneratorResult.HasValue)
        {
            return asyncAndGeneratorResult.Value;
        }

        var scopeFieldsResult = TryCompileInstructionToIL_ScopeFields(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (scopeFieldsResult.HasValue)
        {
            return scopeFieldsResult.Value;
        }

        var parametersAndThisResult = TryCompileInstructionToIL_ParametersAndThis(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (parametersAndThisResult.HasValue)
        {
            return parametersAndThisResult.Value;
        }

        var arithmeticResult = TryCompileInstructionToIL_Arithmetic(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (arithmeticResult.HasValue)
        {
            return arithmeticResult.Value;
        }

        var intrinsicsResult = TryCompileInstructionToIL_Intrinsics(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (intrinsicsResult.HasValue)
        {
            return intrinsicsResult.Value;
        }

        var newUserClassResult = TryCompileInstructionToIL_NewUserClass(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (newUserClassResult.HasValue)
        {
            return newUserClassResult.Value;
        }

        var returnResult = TryCompileInstructionToIL_Return(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);

        if (returnResult.HasValue)
        {
            return returnResult.Value;
        }

        return false;
    }

    /// <summary>
    /// Gets a member reference handle for the default constructor of a scope type.
    /// </summary>
    private MemberReferenceHandle GetScopeConstructorRef(TypeDefinitionHandle scopeType)
    {
        // The scope constructor is a parameterless instance method
        // Signature: void .ctor()
        var ctorSignature = new BlobBuilder();
        new BlobEncoder(ctorSignature)
            .MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true)
            .Parameters(0, returnType => returnType.Void(), parameters => { });

        return _metadataBuilder.AddMemberReference(
            scopeType,
            _metadataBuilder.GetOrAddString(".ctor"),
            _metadataBuilder.GetOrAddBlob(ctorSignature));
    }

    #endregion
}