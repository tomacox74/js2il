using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services.Contracts;

internal sealed record GeneratedContractMetadataReferences(
    MethodDefinitionHandle JsModuleAttributeCtor,
    MethodDefinitionHandle JsExportNameAttributeCtor,
    MethodDefinitionHandle JsExportValueAttributeCtor,
    MethodDefinitionHandle JsObjectContractAttributeCtor,
    MethodDefinitionHandle JsArrayContractAttributeCtor,
    MethodDefinitionHandle JsCallableContractAttributeCtor,
    MethodDefinitionHandle JsBuiltinContractAttributeCtor);

internal sealed class GeneratedContractMetadataEmitter
{
    private const string AttributeNamespace = "Jroc.Generated.Metadata";

    private readonly MetadataBuilder _metadata;
    private readonly BaseClassLibraryReferences _bcl;
    private readonly MemberReferenceRegistry _memberReferences;
    private readonly MethodBodyStreamEncoder _methodBodyStream;

    public GeneratedContractMetadataEmitter(
        MetadataBuilder metadata,
        BaseClassLibraryReferences bcl,
        MemberReferenceRegistry memberReferences,
        MethodBodyStreamEncoder methodBodyStream)
    {
        _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
        _memberReferences = memberReferences ?? throw new ArgumentNullException(nameof(memberReferences));
        _methodBodyStream = methodBodyStream;
    }

    public GeneratedContractMetadataReferences Emit()
    {
        var jsModuleCtor = EmitAttributeType("JsModuleAttribute", hasStringArgument: true);
        var jsExportNameCtor = EmitAttributeType("JsExportNameAttribute", hasStringArgument: true);
        var jsExportValueCtor = EmitAttributeType("JsExportValueAttribute", hasStringArgument: false);
        var jsObjectContractCtor = EmitAttributeType("JsObjectContractAttribute", hasStringArgument: false);
        var jsArrayContractCtor = EmitAttributeType("JsArrayContractAttribute", hasStringArgument: false);
        var jsCallableContractCtor = EmitAttributeType("JsCallableContractAttribute", hasStringArgument: false);
        var jsBuiltinContractCtor = EmitAttributeType("JsBuiltinContractAttribute", hasStringArgument: true);

        return new GeneratedContractMetadataReferences(
            jsModuleCtor,
            jsExportNameCtor,
            jsExportValueCtor,
            jsObjectContractCtor,
            jsArrayContractCtor,
            jsCallableContractCtor,
            jsBuiltinContractCtor);
    }

    private MethodDefinitionHandle EmitAttributeType(string typeName, bool hasStringArgument)
    {
        var typeBuilder = new TypeBuilder(_metadata, AttributeNamespace, typeName);
        var ctor = typeBuilder.AddMethodDefinition(
            MethodAttributes.Public
            | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName
            | MethodAttributes.RTSpecialName,
            ".ctor",
            BuildConstructorSignature(hasStringArgument),
            EmitConstructorBody(),
            AddConstructorParameters(hasStringArgument));

        typeBuilder.AddTypeDefinition(
            TypeAttributes.NotPublic
            | TypeAttributes.Class
            | TypeAttributes.Sealed
            | TypeAttributes.BeforeFieldInit,
            _bcl.TypeReferenceRegistry.GetOrAdd(typeof(Attribute)));

        return ctor;
    }

    private ParameterHandle AddConstructorParameters(bool hasStringArgument)
    {
        if (!hasStringArgument)
        {
            return MetadataTokens.ParameterHandle(_metadata.GetRowCount(TableIndex.Param) + 1);
        }

        return _metadata.AddParameter(
            ParameterAttributes.None,
            _metadata.GetOrAddString("value"),
            sequenceNumber: 1);
    }

    private BlobHandle BuildConstructorSignature(bool hasStringArgument)
    {
        var signature = new BlobBuilder();
        new BlobEncoder(signature)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                hasStringArgument ? 1 : 0,
                returnType => returnType.Void(),
                parameters =>
                {
                    if (hasStringArgument)
                    {
                        parameters.AddParameter().Type().String();
                    }
                });

        return _metadata.GetOrAddBlob(signature);
    }

    private int EmitConstructorBody()
    {
        var bodyBuilder = new BlobBuilder();
        var il = new InstructionEncoder(bodyBuilder);
        il.LoadArgument(0);
        il.OpCode(ILOpCode.Call);
        il.Token(_memberReferences.GetOrAddConstructor(typeof(Attribute), Type.EmptyTypes));
        il.OpCode(ILOpCode.Ret);

        return _methodBodyStream.AddMethodBody(
            il,
            maxStack: 1,
            localVariablesSignature: default,
            attributes: MethodBodyAttributes.None);
    }
}
