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

            LoadFuncTypes(metadataBuilder);
        }

        public AssemblyReferenceHandle SystemRuntimeAssembly { get; private init; }
        public AssemblyReferenceHandle SystemConsoleAssembly { get; private init; }
        public TypeReferenceHandle BooleanType { get; private init; }
        public TypeReferenceHandle DoubleType { get; private init; }
        public TypeReferenceHandle ObjectType { get; private init; }
        public TypeReferenceHandle StringType { get; private init; }
        public TypeReferenceHandle SystemMathType { get; private init; }

    // Removed legacy Action<> delegate references (now using Func returning object)

        public TypeSpecificationHandle IDictionary_StringObject_Type { get; private set; }
        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private init; }

        public MemberReferenceHandle Expando_Ctor_Ref { get; private set; }

        public MemberReferenceHandle Object_Ctor_Ref { get; private set; }

        public MemberReferenceHandle IDictionary_SetItem_Ref { get; private set; }

        public MemberReferenceHandle Array_Add_Ref { get; private set; }

        public MemberReferenceHandle Array_SetItem_Ref { get; private set; }

        public MemberReferenceHandle Array_GetCount_Ref { get; private set; }

        // Func delegates returning object
        public TypeReferenceHandle Func2Generic_TypeRef { get; private set; }
        public TypeReferenceHandle Func3Generic_TypeRef { get; private set; }
        public TypeSpecificationHandle FuncObjectObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectObject_Ctor_Ref { get; private set; }
        public MemberReferenceHandle FuncObjectObject_Invoke_Ref { get; private set; }
        public TypeSpecificationHandle FuncObjectObjectObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectObjectObject_Ctor_Ref { get; private set; }
        public MemberReferenceHandle FuncObjectObjectObject_Invoke_Ref { get; private set; }

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

    // Removed LoadActionTypes (legacy Action support)

        private void LoadFuncTypes(MetadataBuilder metadataBuilder)
        {
            // Func<T1, TResult>
            Func2Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`2"));

            // Func<T1, T2, TResult>
            Func3Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`3"));

            // Close over object types for parameters and return
            // Func<object, object>
            var func2SpecBlob = new BlobBuilder();
            var func2Inst = new BlobEncoder(func2SpecBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(Func2Generic_TypeRef, 2, isValueType: false);
            func2Inst.AddArgument().Type(ObjectType, isValueType: false); // T1
            func2Inst.AddArgument().Type(ObjectType, isValueType: false); // TResult
            var func2Spec = metadataBuilder.GetOrAddBlob(func2SpecBlob);
            FuncObjectObject_TypeSpec = metadataBuilder.AddTypeSpecification(func2Spec);

            // Func<object, object, object>
            var func3SpecBlob = new BlobBuilder();
            var func3Inst = new BlobEncoder(func3SpecBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(Func3Generic_TypeRef, 3, isValueType: false);
            func3Inst.AddArgument().Type(ObjectType, isValueType: false); // T1
            func3Inst.AddArgument().Type(ObjectType, isValueType: false); // T2
            func3Inst.AddArgument().Type(ObjectType, isValueType: false); // TResult
            var func3Spec = metadataBuilder.GetOrAddBlob(func3SpecBlob);
            FuncObjectObjectObject_TypeSpec = metadataBuilder.AddTypeSpecification(func3Spec);

            // Delegate .ctor signature (object, IntPtr)
            var funcCtorSig = new BlobBuilder();
            new BlobEncoder(funcCtorSig)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2, returnType => returnType.Void(), parameters => {
                    parameters.AddParameter().Type().Object();
                    parameters.AddParameter().Type().IntPtr();
                });
            var funcCtorSigHandle = metadataBuilder.GetOrAddBlob(funcCtorSig);

            FuncObjectObject_Ctor_Ref = metadataBuilder.AddMemberReference(
                FuncObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString(".ctor"),
                funcCtorSigHandle);
            FuncObjectObjectObject_Ctor_Ref = metadataBuilder.AddMemberReference(
                FuncObjectObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString(".ctor"),
                funcCtorSigHandle);

            // Invoke signatures
            var func2InvokeBlob = new BlobBuilder();
            new BlobEncoder(func2InvokeBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(1,
                    returnType => returnType.Type().GenericTypeParameter(1), // TResult
                    parameters => { parameters.AddParameter().Type().GenericTypeParameter(0); });
            var func2InvokeSig = metadataBuilder.GetOrAddBlob(func2InvokeBlob);
            FuncObjectObject_Invoke_Ref = metadataBuilder.AddMemberReference(
                FuncObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString("Invoke"),
                func2InvokeSig);

            var func3InvokeBlob = new BlobBuilder();
            new BlobEncoder(func3InvokeBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2,
                    returnType => returnType.Type().GenericTypeParameter(2), // TResult
                    parameters => {
                        parameters.AddParameter().Type().GenericTypeParameter(0); // T1
                        parameters.AddParameter().Type().GenericTypeParameter(1); // T2
                    });
            var func3InvokeSig = metadataBuilder.GetOrAddBlob(func3InvokeBlob);
            FuncObjectObjectObject_Invoke_Ref = metadataBuilder.AddMemberReference(
                FuncObjectObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString("Invoke"),
                func3InvokeSig);
        }
    }
}
