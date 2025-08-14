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

    public TypeReferenceHandle ExpandoObjectType { get; private set; }

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
        
        // Func delegates with scope array parameter (object[])
        public TypeSpecificationHandle FuncObjectArrayObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObject_Ctor_Ref { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObject_Invoke_Ref { get; private set; }
        public TypeSpecificationHandle FuncObjectArrayObjectObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObjectObject_Ctor_Ref { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObjectObject_Invoke_Ref { get; private set; }
    // Dynamic invoke refs for multi-parameter (scopes + N params)
    private readonly Dictionary<int, MemberReferenceHandle> _funcArrayParamInvokeRefs = new();
    // Additional Func delegate generic type refs for multi-parameter support (scopes + up to 6 js params + return)
    public TypeReferenceHandle Func4Generic_TypeRef { get; private set; } // scopes + 2 params + return
    public TypeReferenceHandle Func5Generic_TypeRef { get; private set; } // scopes + 3 params + return
    public TypeReferenceHandle Func6Generic_TypeRef { get; private set; } // scopes + 4 params + return
    public TypeReferenceHandle Func7Generic_TypeRef { get; private set; } // scopes + 5 params + return
    public TypeReferenceHandle Func8Generic_TypeRef { get; private set; } // scopes + 6 params + return

    private readonly Dictionary<int, TypeSpecificationHandle> _funcArrayParamTypeSpecs = new();
    private readonly Dictionary<int, MemberReferenceHandle> _funcArrayParamCtorRefs = new();

        private void LoadObjectTypes(MetadataBuilder metadataBuilder)
        {
            var dynamicNamespace = metadataBuilder.GetOrAddString("System.Dynamic");

            // ExpandObject reference
            // important for the generic case in JavaScript where objects are just property bags
            var systemCoreExpandoType = metadataBuilder.AddTypeReference(
                _systemLinqExpressions,
                dynamicNamespace,
                metadataBuilder.GetOrAddString("ExpandoObject"));
            // store the ExpandoObject type reference for use as a base class
            ExpandoObjectType = systemCoreExpandoType;
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
            // Additional generic Func references
            Func4Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`4"));
            Func5Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`5"));
            Func6Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`6"));
            Func7Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`7"));
            Func8Generic_TypeRef = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Func`8"));

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

            // Func<object[], object> type (scope array, no additional params)
            var funcArrayObjectBlob = new BlobBuilder();
            var funcArrayObjectEncoder = new BlobEncoder(funcArrayObjectBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(Func2Generic_TypeRef, 2, isValueType: false);
            funcArrayObjectEncoder.AddArgument().SZArray().Object();
            funcArrayObjectEncoder.AddArgument().Object();
            FuncObjectArrayObject_TypeSpec = metadataBuilder.AddTypeSpecification(
                metadataBuilder.GetOrAddBlob(funcArrayObjectBlob));

            var funcArrayCtorBlob = new BlobBuilder();
            new BlobEncoder(funcArrayCtorBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().Object(); // object target
                        parameters.AddParameter().Type().IntPtr(); // native int method
                    });
            var funcArrayCtorSig = metadataBuilder.GetOrAddBlob(funcArrayCtorBlob);
            FuncObjectArrayObject_Ctor_Ref = metadataBuilder.AddMemberReference(
                FuncObjectArrayObject_TypeSpec,
                metadataBuilder.GetOrAddString(".ctor"),
                funcArrayCtorSig);

            var funcArrayInvokeBlob = new BlobBuilder();
            new BlobEncoder(funcArrayInvokeBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(1,
                    returnType => returnType.Type().GenericTypeParameter(1), // TResult (object)
                    parameters => { parameters.AddParameter().Type().GenericTypeParameter(0); }); // object[] parameter
            var funcArrayInvokeSig = metadataBuilder.GetOrAddBlob(funcArrayInvokeBlob);
            FuncObjectArrayObject_Invoke_Ref = metadataBuilder.AddMemberReference(
                FuncObjectArrayObject_TypeSpec,
                metadataBuilder.GetOrAddString("Invoke"),
                funcArrayInvokeSig);

            // Func<object[], object, object> type (scope array, one additional param)
            var funcArrayObjectObjectBlob = new BlobBuilder();
            var funcArrayObjectObjectEncoder = new BlobEncoder(funcArrayObjectObjectBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(Func3Generic_TypeRef, 3, isValueType: false);
            funcArrayObjectObjectEncoder.AddArgument().SZArray().Object();
            funcArrayObjectObjectEncoder.AddArgument().Object();
            funcArrayObjectObjectEncoder.AddArgument().Object();
            FuncObjectArrayObjectObject_TypeSpec = metadataBuilder.AddTypeSpecification(
                metadataBuilder.GetOrAddBlob(funcArrayObjectObjectBlob));

            var funcArrayObjectCtorBlob = new BlobBuilder();
            new BlobEncoder(funcArrayObjectCtorBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().Object(); // object target
                        parameters.AddParameter().Type().IntPtr(); // native int method
                    });
            var funcArrayObjectCtorSig = metadataBuilder.GetOrAddBlob(funcArrayObjectCtorBlob);
            FuncObjectArrayObjectObject_Ctor_Ref = metadataBuilder.AddMemberReference(
                FuncObjectArrayObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString(".ctor"),
                funcArrayObjectCtorSig);

            var funcArrayObjectInvokeBlob = new BlobBuilder();
            new BlobEncoder(funcArrayObjectInvokeBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2,
                    returnType => returnType.Type().GenericTypeParameter(2), // TResult (object)
                    parameters => {
                        parameters.AddParameter().Type().GenericTypeParameter(0); // object[] parameter
                        parameters.AddParameter().Type().GenericTypeParameter(1); // T2 (object)
                    });
            var funcArrayObjectInvokeSig = metadataBuilder.GetOrAddBlob(funcArrayObjectInvokeBlob);
            FuncObjectArrayObjectObject_Invoke_Ref = metadataBuilder.AddMemberReference(
                FuncObjectArrayObjectObject_TypeSpec,
                metadataBuilder.GetOrAddString("Invoke"),
                funcArrayObjectInvokeSig);

            // Pre-build delegate types for 2..6 js parameters (total generic arity = scopes + params + return)
            for (int jsParamCount = 2; jsParamCount <= 6; jsParamCount++)
            {
                BuildFuncArrayParamDelegate(metadataBuilder, jsParamCount, funcCtorSigHandle);
            }
            // Build invoke signatures for 2..6 js parameter delegates
            for (int jsParamCount = 2; jsParamCount <= 6; jsParamCount++)
            {
                BuildFuncArrayParamInvoke(metadataBuilder, jsParamCount);
            }
        }

        private void BuildFuncArrayParamDelegate(MetadataBuilder metadataBuilder, int jsParamCount, BlobHandle funcCtorSigHandle)
        {
            // generic arity = scopes + jsParamCount + return
            int genericArity = jsParamCount + 2;
            TypeReferenceHandle funcGeneric = genericArity switch
            {
                4 => Func4Generic_TypeRef,
                5 => Func5Generic_TypeRef,
                6 => Func6Generic_TypeRef,
                7 => Func7Generic_TypeRef,
                8 => Func8Generic_TypeRef,
                _ => throw new NotSupportedException($"Unsupported generic arity {genericArity} for jsParamCount={jsParamCount}")
            };

            // Build closed generic instantiation: Func<object[], object, ..., object>
            var specBlob = new BlobBuilder();
            var inst = new BlobEncoder(specBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(funcGeneric, genericArity, isValueType: false);
            // scopes array
            inst.AddArgument().SZArray().Object();
            // js params
            for (int i = 0; i < jsParamCount; i++)
            {
                inst.AddArgument().Object();
            }
            // return object
            inst.AddArgument().Object();
            var specHandle = metadataBuilder.AddTypeSpecification(metadataBuilder.GetOrAddBlob(specBlob));
            _funcArrayParamTypeSpecs[jsParamCount] = specHandle;

            // Add constructor member reference on closed generic type
            var ctorRef = metadataBuilder.AddMemberReference(
                specHandle,
                metadataBuilder.GetOrAddString(".ctor"),
                funcCtorSigHandle);
            _funcArrayParamCtorRefs[jsParamCount] = ctorRef;
        }

        public (TypeSpecificationHandle typeSpec, MemberReferenceHandle ctorRef) GetFuncObjectArrayWithParams(int jsParamCount)
        {
            if (jsParamCount == 0) return (FuncObjectArrayObject_TypeSpec, FuncObjectArrayObject_Ctor_Ref);
            if (jsParamCount == 1) return (FuncObjectArrayObjectObject_TypeSpec, FuncObjectArrayObjectObject_Ctor_Ref);
            if (_funcArrayParamTypeSpecs.TryGetValue(jsParamCount, out var spec))
            {
                return (spec, _funcArrayParamCtorRefs[jsParamCount]);
            }
            throw new NotSupportedException($"Delegate for {jsParamCount} parameters not initialized");
        }

        public MemberReferenceHandle GetFuncArrayParamInvokeRef(int jsParamCount)
        {
            // For 0 params, historical snapshots reference Invoke on Func<object[], object>
            if (jsParamCount == 0) return FuncObjectArrayObject_Invoke_Ref;         // Func<object[], object>.Invoke
            // For 1 param, snapshots reference Invoke on Func<object, object, object>
            if (jsParamCount == 1) return FuncObjectObjectObject_Invoke_Ref;         // Func<object, object, object>.Invoke
            if (_funcArrayParamInvokeRefs.TryGetValue(jsParamCount, out var invoke)) return invoke;
            throw new NotSupportedException($"Invoke ref for {jsParamCount} parameters not initialized");
        }

        private void BuildFuncArrayParamInvoke(MetadataBuilder metadataBuilder, int jsParamCount)
        {
            // generic arity = scopes + jsParamCount + return
            int genericArity = jsParamCount + 2;
            TypeReferenceHandle funcGeneric = genericArity switch
            {
                4 => Func4Generic_TypeRef,
                5 => Func5Generic_TypeRef,
                6 => Func6Generic_TypeRef,
                7 => Func7Generic_TypeRef,
                8 => Func8Generic_TypeRef,
                _ => throw new NotSupportedException($"Unsupported generic arity {genericArity} for invoke")
            };
            // Build method signature: instance invoke with (scopes, param1..N) returning object (last generic)
            var invokeBlob = new BlobBuilder();
            new BlobEncoder(invokeBlob)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(jsParamCount + 1, // scopes + N params
                    returnType => returnType.Type().GenericTypeParameter(genericArity - 1), // TResult
                    parameters =>
                    {
                        // scopes param
                        parameters.AddParameter().Type().GenericTypeParameter(0);
                        for (int i = 0; i < jsParamCount; i++)
                        {
                            parameters.AddParameter().Type().GenericTypeParameter(i + 1);
                        }
                    });
            var invokeSigHandle = metadataBuilder.GetOrAddBlob(invokeBlob);
            // Build closed type spec again to attach MemberReference (same as in delegate build)
            var specBlob = new BlobBuilder();
            var inst = new BlobEncoder(specBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(funcGeneric, genericArity, isValueType: false);
            inst.AddArgument().SZArray().Object();
            for (int i = 0; i < jsParamCount; i++) inst.AddArgument().Object();
            inst.AddArgument().Object();
            var specHandle = metadataBuilder.AddTypeSpecification(metadataBuilder.GetOrAddBlob(specBlob));
            var invokeRef = metadataBuilder.AddMemberReference(specHandle, metadataBuilder.GetOrAddString("Invoke"), invokeSigHandle);
            _funcArrayParamInvokeRefs[jsParamCount] = invokeRef;
        }
    }
}
