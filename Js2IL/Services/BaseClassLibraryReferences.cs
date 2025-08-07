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
        private readonly AssemblyReferenceHandle _systemLinqExpressions;
        private readonly AssemblyReferenceHandle _systemCollections;

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

            _systemLinqExpressions = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Linq.Expressions"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            _systemCollections = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Collections"),
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
                    parameters =>
                    {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });
            var writeLineSig = metadataBuilder.GetOrAddBlob(consoleSig);

            this.ConsoleWriteLine_StringObject_Ref = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);

            LoadObjectTypes(metadataBuilder);

            LoadArrayTypes(metadataBuilder);

            LoadActionTypes(metadataBuilder);
        }

        public AssemblyReferenceHandle SystemRuntimeAssembly { get; private init; }
        public AssemblyReferenceHandle SystemConsoleAssembly { get; private init; }
        public TypeReferenceHandle BooleanType { get; private init; }
        public TypeReferenceHandle DoubleType { get; private init; }
        public TypeReferenceHandle ObjectType { get; private init; }
        public TypeReferenceHandle StringType { get; private init; }
        public TypeReferenceHandle SystemMathType { get; private init; }

        public TypeReferenceHandle Action_TypeRef { get; private set; }
        
        public TypeReferenceHandle ActionGeneric_TypeRef { get; private set; }
        
        public TypeSpecificationHandle ActionObject_TypeSpec { get; private set; }

        public TypeSpecificationHandle IDictionary_StringObject_Type { get; private set; }
        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private init; }

        public MemberReferenceHandle Expando_Ctor_Ref { get; private set; }

        public MemberReferenceHandle Object_Ctor_Ref { get; private set; }

        public MemberReferenceHandle IDictionary_SetItem_Ref { get; private set; }

        public MemberReferenceHandle Array_Add_Ref { get; private set; }

        public MemberReferenceHandle Array_SetItem_Ref { get; private set; }

        public MemberReferenceHandle Array_GetCount_Ref { get; private set; }

        public MemberReferenceHandle Action_Ctor_Ref { get; private set; }

        public MemberReferenceHandle Action_Invoke_Ref { get; private set; }
        
        public MemberReferenceHandle ActionObject_Ctor_Ref { get; private set; }

        public MemberReferenceHandle ActionObject_Invoke_Ref { get; private set; }

        private void LoadObjectTypes(MetadataBuilder metadataBuilder)
        {
            var dynamicNamespace = metadataBuilder.GetOrAddString("System.Dynamic");

            // ExpandObject reference
            // important for the generic case in JavaScript where objects are just property bags
            var systemCoreExpandoType = metadataBuilder.AddTypeReference(
                _systemLinqExpressions,
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

            // Object constructor reference
            var objectCtorSigBuilder = new BlobBuilder();
            new BlobEncoder(objectCtorSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var objectCtorSig = metadataBuilder.GetOrAddBlob(objectCtorSigBuilder);
            Object_Ctor_Ref = metadataBuilder.AddMemberReference(
                this.ObjectType,
                metadataBuilder.GetOrAddString(".ctor"),
                objectCtorSig);

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
                    });

            var setItemSig = metadataBuilder.GetOrAddBlob(msBlob);

            IDictionary_StringObject_Type = closedDictSpec;

            // 5) The MemberReference on the *closed* IDictionary<string,object>
            IDictionary_SetItem_Ref = metadataBuilder.AddMemberReference(
                closedDictSpec,                             // <string,object> TypeSpec
                metadataBuilder.GetOrAddString("set_Item"),
                setItemSig);

        }

        private void LoadArrayTypes(MetadataBuilder metadataBuilder)
        {
            // List Bound Type Reference <System.Object>
            var unboundListType = metadataBuilder.AddTypeReference(
                _systemCollections,
                metadataBuilder.GetOrAddString("System.Collections.Generic"),
                metadataBuilder.GetOrAddString("List`1"));

            // 3) Build a TypeSpec blob for IDictionary<string, object>
            var tsBlob = new BlobBuilder();
            var tsEncoder = new BlobEncoder(tsBlob);

            // .TypeSpecificationSignature() kicks off a TypeSpec
            var genInst = tsEncoder
                .TypeSpecificationSignature()
                .GenericInstantiation(
                    unboundListType,   // our open-generic TypeReferenceHandle
                    genericArgumentCount: 1,
                    isValueType: false);

            // now emit the type arg
            genInst.AddArgument().PrimitiveType(PrimitiveTypeCode.Object);  // System.Object

            // bake it into metadata
            var tsBlobHandle = metadataBuilder.GetOrAddBlob(tsBlob);
            var closedListSpec = metadataBuilder.AddTypeSpecification(tsBlobHandle);

            // 4) Create the signature for set_Item(int,object)
            var setItemBuilder = new BlobBuilder();
            new BlobEncoder(setItemBuilder)
                .MethodSignature(
                    genericParameterCount: 0,  // NOT a generic *method*
                    isInstanceMethod: true)
                .Parameters(
                    parameterCount: 2,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().Int32();
                        parameters.AddParameter().Type().GenericTypeParameter(0);
                    });

            var setItemSig = metadataBuilder.GetOrAddBlob(setItemBuilder);

            // 5) The MemberReference on the *closed* IDictionary<string,object>
            Array_SetItem_Ref = metadataBuilder.AddMemberReference(
                closedListSpec,                             // <string,object> TypeSpec
                metadataBuilder.GetOrAddString("set_Item"),
                setItemSig);

            var getCountBuilder = new BlobBuilder();
            new BlobEncoder(getCountBuilder)
                .MethodSignature(
                    genericParameterCount: 0,  // NOT a generic *method*
                    isInstanceMethod: true)
                .Parameters(
                    parameterCount: 0,
                    returnType => returnType.Type().Int32(),
                    parameters => { });

            var getCountSig = metadataBuilder.GetOrAddBlob(getCountBuilder);

            Array_GetCount_Ref = metadataBuilder.AddMemberReference(
                closedListSpec,
                metadataBuilder.GetOrAddString("get_Count"),
                getCountSig);

            // 4) Create the signature for set_Item(int,object)
            var addItemBuilder = new BlobBuilder();
            new BlobEncoder(addItemBuilder)
                .MethodSignature(
                    genericParameterCount: 0,  // NOT a generic *method*
                    isInstanceMethod: true)
                .Parameters(
                    parameterCount: 1,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().GenericTypeParameter(0);
                    });

            var addItemSig = metadataBuilder.GetOrAddBlob(addItemBuilder);

            // 5) The MemberReference on the *closed* IDictionary<string,object>
            Array_Add_Ref = metadataBuilder.AddMemberReference(
                closedListSpec,                             // <string,object> TypeSpec
                metadataBuilder.GetOrAddString("Add"),
                addItemSig);
        }

        private void LoadActionTypes(MetadataBuilder metadataBuilder)
        {
            Action_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Action"));

            var actionCtorSigBuilder = new BlobBuilder();
            new BlobEncoder(actionCtorSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2, returnType => returnType.Void(), parameters => { 
                    parameters.AddParameter().Type().Object();
                    parameters.AddParameter().Type().IntPtr();
                });
            var expandoCtorSig = metadataBuilder.GetOrAddBlob(actionCtorSigBuilder);
            Action_Ctor_Ref = metadataBuilder.AddMemberReference(
                Action_TypeRef,
                metadataBuilder.GetOrAddString(".ctor"),
                expandoCtorSig);

            var actionInvokeSigBuilder = new BlobBuilder();
            new BlobEncoder(actionInvokeSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => {});
            var actionInvokeSig = metadataBuilder.GetOrAddBlob(actionInvokeSigBuilder);
            Action_Invoke_Ref = metadataBuilder.AddMemberReference(
                Action_TypeRef,
                metadataBuilder.GetOrAddString("Invoke"),
                actionInvokeSig);

            // Load Action<Object> type reference (Action`1)
            var actionGenericTypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Action`1"));
            
            ActionGeneric_TypeRef = actionGenericTypeRef;

            // Create Action<Object> type specification
            var actionObjectSigBuilder = new BlobBuilder();
            new BlobEncoder(actionObjectSigBuilder)
                .TypeSpecificationSignature()
                .GenericInstantiation(actionGenericTypeRef, 1, isValueType: false)
                .AddArgument().Type(ObjectType, isValueType: false);
            var actionObjectSig = metadataBuilder.GetOrAddBlob(actionObjectSigBuilder);
            ActionObject_TypeSpec = metadataBuilder.AddTypeSpecification(actionObjectSig);

            // Create Action<Object> constructor reference
            var actionObjectCtorSigBuilder = new BlobBuilder();
            new BlobEncoder(actionObjectCtorSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2, returnType => returnType.Void(), parameters => { 
                    parameters.AddParameter().Type().Object();
                    parameters.AddParameter().Type().IntPtr();
                });
            var actionObjectCtorSig = metadataBuilder.GetOrAddBlob(actionObjectCtorSigBuilder);
            ActionObject_Ctor_Ref = metadataBuilder.AddMemberReference(
                ActionObject_TypeSpec,
                metadataBuilder.GetOrAddString(".ctor"),
                actionObjectCtorSig);

            // Create Action<Object> Invoke reference
            var actionObjectInvokeSigBuilder = new BlobBuilder();
            new BlobEncoder(actionObjectInvokeSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(1, returnType => returnType.Void(), parameters => {
                    parameters.AddParameter().Type().GenericTypeParameter(0);
                });
            var actionObjectInvokeSig = metadataBuilder.GetOrAddBlob(actionObjectInvokeSigBuilder);
            ActionObject_Invoke_Ref = metadataBuilder.AddMemberReference(
                ActionObject_TypeSpec,
                metadataBuilder.GetOrAddString("Invoke"),
                actionObjectInvokeSig);
        }
    }
}
