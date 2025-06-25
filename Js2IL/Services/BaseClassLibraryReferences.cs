using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    internal class BaseClassLibraryReferences
    {
        public BaseClassLibraryReferences(MetadataBuilder metadataBuilder, Version bclVersion, byte[] publicKeyToken)
        {
            // public key token
            var publicKeyTokenHandle = metadataBuilder.GetOrAddBlob(publicKeyToken);

            // Assembly References
            this.SystemConsoleAssembly = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Console"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            this.SystemRuntimeAssembly = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Runtime"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            var systemLinqExpressions = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Linq.Expressions"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            // Common Runtime References
            this.BooleanType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Boolean")
            );

            this.DoubleType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Double")
            );

            this.ObjectType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Object")
            );

            this.StringType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("String")
            );

            // IDictionary Bound Type Reference <System.String, System.Object>
            var unboundIDictionaryType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System.Collections.Generic"),
                metadataBuilder.GetOrAddString("IDictionary`2"));


            // 3) Build a TypeSpec blob for IDictionary<string, object>
            var tsBlob = new BlobBuilder();
            var tsEncoder = new BlobEncoder(tsBlob);

            // .TypeSpecificationSignature() kicks off a TypeSpec
            var genInst = tsEncoder
                .TypeSpecificationSignature()
                .GenericInstantiation(
                    unboundIDictionaryType,   // our open-generic TypeReferenceHandle
                    genericArgumentCount: 2,
                    isValueType: false);

            // now emit the two type args in order:
            genInst.AddArgument().PrimitiveType(PrimitiveTypeCode.String);  // System.String
            genInst.AddArgument().PrimitiveType(PrimitiveTypeCode.Object);  // System.Object

            // bake it into metadata
            var tsBlobHandle = metadataBuilder.GetOrAddBlob(tsBlob);
            var closedDictSpec = metadataBuilder.AddTypeSpecification(tsBlobHandle);

            // 4) Create the signature for set_Item(string,object)
            var msBlob = new BlobBuilder();
            new BlobEncoder(msBlob)
                .MethodSignature(
                    genericParameterCount: 0,  // NOT a generic *method*
                    isInstanceMethod: true)
                .Parameters(
                    parameterCount: 2,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().GenericTypeParameter(0);
                        parameters.AddParameter().Type().GenericTypeParameter(1);
                        //parameters.AddParameter().Type().String();
                        //parameters.AddParameter().Type().Object();
                    });

            var setItemSig = metadataBuilder.GetOrAddBlob(msBlob);

            IDictionary_StringObject_Type = closedDictSpec;

            // 5) The MemberReference on the *closed* IDictionary<string,object>
            IDictionary_SetItem_Ref = metadataBuilder.AddMemberReference(
                closedDictSpec,                             // <string,object> TypeSpec
                metadataBuilder.GetOrAddString("set_Item"),
                setItemSig);


            /*
            var typeSpecBlob = new BlobBuilder();
            var dictionaryTypeArgs = new BlobEncoder(typeSpecBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(unboundIDictionaryType, 2, false);

            dictionaryTypeArgs.AddArgument().String(); // Key type
            dictionaryTypeArgs.AddArgument().Object(); // Value type

            var dictionaryTypeSpecHandle = metadataBuilder.GetOrAddBlob(typeSpecBlob);
            var closedIDictionarySpec = metadataBuilder.AddTypeSpecification(dictionaryTypeSpecHandle);

            // IDictionary.set_Item Reference
            var setItemSigBuilder = new BlobBuilder();
            new BlobEncoder(setItemSigBuilder)
                .MethodSignature(genericParameterCount: 2, isInstanceMethod: true)
                .Parameters(
                    parameterCount: 2,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });

            var setItemSig = metadataBuilder.GetOrAddBlob(setItemSigBuilder);

            IDictionary_SetItem_Ref = metadataBuilder.AddMemberReference(
                closedIDictionarySpec,
                metadataBuilder.GetOrAddString("set_Item"),
                setItemSig);*/

            // System.Core References
            var dynamicNamespace = metadataBuilder.GetOrAddString("System.Dynamic");

            // ExpandObject reference
            // important for the generic case in JavaScript where objects are just property bags
            var systemCoreExpandoType = metadataBuilder.AddTypeReference(
                systemLinqExpressions,
                dynamicNamespace,
                metadataBuilder.GetOrAddString("ExpandoObject"));
            var expandoSigBuilder = new BlobBuilder();
            new BlobEncoder(expandoSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var expandoCtorSig = metadataBuilder.GetOrAddBlob(expandoSigBuilder);
            Expando_Ctor_Ref = metadataBuilder.AddMemberReference(
                systemCoreExpandoType,
                metadataBuilder.GetOrAddString(".ctor"),
                expandoCtorSig);

            // System.Math References
            this.SystemMathType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Math"));

            // System.Console References
            var systemConsoleTypeReference = metadataBuilder.AddTypeReference(
                this.SystemConsoleAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Console"));

            // Create method signature: void WriteLine(string)
            var consoleSig = new BlobBuilder();
            new BlobEncoder(consoleSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });
            var writeLineSig = metadataBuilder.GetOrAddBlob(consoleSig);

            this.ConsoleWriteLine_StringObject_Ref = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);
        }

        public AssemblyReferenceHandle SystemRuntimeAssembly { get; private init; }
        public AssemblyReferenceHandle SystemConsoleAssembly { get; private init; }
        public TypeReferenceHandle BooleanType { get; private init; }
        public TypeReferenceHandle DoubleType { get; private init; }
        public TypeReferenceHandle ObjectType { get; private init; }
        public TypeReferenceHandle StringType { get; private init; }
        public TypeReferenceHandle SystemMathType { get; private init; }

        public TypeSpecificationHandle IDictionary_StringObject_Type { get; private init; }
        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private init; }

        public MemberReferenceHandle Expando_Ctor_Ref { get; private init; }

        public MemberReferenceHandle IDictionary_SetItem_Ref { get; private init; }
    }
}
