using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    internal class BaseClassLibraryReferences
    {
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;
        private readonly Dictionary<int, MemberReferenceHandle> _funcArrayParamInvokeRefs = new();
        private readonly Dictionary<int, Type> _funcTypesByParamCount = new()
        {
            { 0, typeof(System.Func<object[], object>) },
            { 1, typeof(System.Func<object[], object, object>) },
            { 2, typeof(System.Func<object[], object, object, object>) },
            { 3, typeof(System.Func<object[], object, object, object, object>) },
            { 4, typeof(System.Func<object[], object, object, object, object, object>) },
            { 5, typeof(System.Func<object[], object, object, object, object, object, object>) },
            { 6, typeof(System.Func<object[], object, object, object, object, object, object, object>) }
        };

        public BaseClassLibraryReferences(MetadataBuilder metadataBuilder)
        {
            _typeRefRegistry = new TypeReferenceRegistry(metadataBuilder);
            _memberRefRegistry = new MemberReferenceRegistry(metadataBuilder, _typeRefRegistry);

            // Common Runtime References
            this.BooleanType = _typeRefRegistry.GetOrAdd(typeof(bool));

            this.DoubleType = _typeRefRegistry.GetOrAdd(typeof(double));

            this.Int32Type = _typeRefRegistry.GetOrAdd(typeof(int));

            this.ObjectType = _typeRefRegistry.GetOrAdd(typeof(object));

            this.StringType = _typeRefRegistry.GetOrAdd(typeof(string));            

            // System.Exception Reference (for catch handlers)
            this.ExceptionType = _typeRefRegistry.GetOrAdd(typeof(System.Exception));

            // System.Math References
            this.SystemMathType = _typeRefRegistry.GetOrAdd(typeof(System.Math));

            // System.Console References
            var systemConsoleTypeReference = _typeRefRegistry.GetOrAdd(typeof(System.Console));

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

            // System.Action reference
            var actionTypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Action));

            var actionCtorSig = new BlobBuilder();
            new BlobEncoder(actionCtorSig)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().Object();
                        parameters.AddParameter().Type().IntPtr();
                    });
            Action_Ctor_Ref = metadataBuilder.AddMemberReference(
                actionTypeRef,
                metadataBuilder.GetOrAddString(".ctor"),
                metadataBuilder.GetOrAddBlob(actionCtorSig));

            // System.Reflection.MethodBase and GetCurrentMethod()
            MethodBaseType = _typeRefRegistry.GetOrAdd(typeof(System.Reflection.MethodBase));

            var getCurrentMethodSig = new BlobBuilder();
            new BlobEncoder(getCurrentMethodSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(0,
                    returnType => returnType.Type().Type(MethodBaseType, isValueType: false),
                    parameters => { });
            MethodBase_GetCurrentMethod_Ref = metadataBuilder.AddMemberReference(
                MethodBaseType,
                metadataBuilder.GetOrAddString("GetCurrentMethod"),
                metadataBuilder.GetOrAddBlob(getCurrentMethodSig));
        }

        public TypeReferenceRegistry TypeRefRegistry => _typeRefRegistry;
        public MemberReferenceRegistry MemberRefRegistry => _memberRefRegistry;

        public TypeReferenceHandle BooleanType { get; private init; }
        public TypeReferenceHandle DoubleType { get; private init; }
        public TypeReferenceHandle Int32Type { get; private init; }
        public TypeReferenceHandle ObjectType { get; private init; }
        public TypeReferenceHandle StringType { get; private init; }
        public TypeReferenceHandle ExceptionType { get; private init; }
        public TypeReferenceHandle SystemMathType { get; private init; }
        public TypeReferenceHandle MethodBaseType { get; private set; }
        public MemberReferenceHandle MethodBase_GetCurrentMethod_Ref { get; private set; }

        // Removed legacy Action<> delegate references (now using Func returning object)

        public TypeSpecificationHandle IDictionary_StringObject_Type { get; private set; }
        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private init; }
        public MemberReferenceHandle Expando_Ctor_Ref { get; private set; }
        public TypeReferenceHandle ExpandoObjectType { get; private set; }
        public MemberReferenceHandle Object_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(object), Type.EmptyTypes);
        public MemberReferenceHandle IDictionary_SetItem_Ref { get; private set; }
        public MemberReferenceHandle Array_Add_Ref { get; private set; }
        public MemberReferenceHandle Array_SetItem_Ref { get; private set; }
        public MemberReferenceHandle Array_GetCount_Ref { get; private set; }
        public MemberReferenceHandle Action_Ctor_Ref { get; private set; }

        // Func delegates returning object
        public TypeReferenceHandle Func2Generic_TypeRef { get; private set; }
        public TypeReferenceHandle Func3Generic_TypeRef { get; private set; }
        
        // Func delegates with scope array parameter (object[])
        public TypeSpecificationHandle FuncObjectArrayObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObject_Invoke_Ref { get; private set; }
        public TypeSpecificationHandle FuncObjectArrayObjectObject_TypeSpec { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObjectObject_Ctor_Ref { get; private set; }
        public MemberReferenceHandle FuncObjectArrayObjectObject_Invoke_Ref { get; private set; }

        // Additional Func delegate generic type refs for multi-parameter support (scopes + up to 6 js params + return)
        public TypeReferenceHandle Func4Generic_TypeRef { get; private set; } // scopes + 2 params + return
        public TypeReferenceHandle Func5Generic_TypeRef { get; private set; } // scopes + 3 params + return
        public TypeReferenceHandle Func6Generic_TypeRef { get; private set; } // scopes + 4 params + return
        public TypeReferenceHandle Func7Generic_TypeRef { get; private set; } // scopes + 5 params + return
        public TypeReferenceHandle Func8Generic_TypeRef { get; private set; } // scopes + 6 params + return

        private void LoadObjectTypes(MetadataBuilder metadataBuilder)
        {
            // ExpandObject reference
            // important for the generic case in JavaScript where objects are just property bags
            var systemCoreExpandoType = _typeRefRegistry.GetOrAdd(typeof(System.Dynamic.ExpandoObject));
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

            // IDictionary Bound Type Reference <System.String, System.Object>
            var unboundIDictionaryType = _typeRefRegistry.GetOrAdd(typeof(System.Collections.Generic.IDictionary<,>));


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
            var unboundListType = _typeRefRegistry.GetOrAdd(typeof(System.Collections.Generic.List<>));

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
            Func2Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,>));

            // Func<T1, T2, TResult>
            Func3Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,>));
            // Additional generic Func references
            Func4Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,,>));
            Func5Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,,,>));
            Func6Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,,,,>));
            Func7Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,,,,,>));
            Func8Generic_TypeRef = _typeRefRegistry.GetOrAdd(typeof(System.Func<,,,,,,,>));

            // Func<object[], object> type (scope array, no additional params)
            var funcArrayObjectBlob = new BlobBuilder();
            var funcArrayObjectEncoder = new BlobEncoder(funcArrayObjectBlob)
                .TypeSpecificationSignature()
                .GenericInstantiation(Func2Generic_TypeRef, 2, isValueType: false);
            funcArrayObjectEncoder.AddArgument().SZArray().Object();
            funcArrayObjectEncoder.AddArgument().Object();
            FuncObjectArrayObject_TypeSpec = metadataBuilder.AddTypeSpecification(
                metadataBuilder.GetOrAddBlob(funcArrayObjectBlob));

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

            FuncObjectArrayObjectObject_Ctor_Ref = _memberRefRegistry.GetOrAddConstructor(
                typeof(System.Func<object[], object, object>));

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

            // Build invoke signatures for 2..6 js parameter delegates
            for (int jsParamCount = 2; jsParamCount <= 6; jsParamCount++)
            {
                BuildFuncArrayParamInvoke(metadataBuilder, jsParamCount);
            }
        }

        public MemberReferenceHandle GetFuncCtorRef(int jsParamCount)
        {
            if (!_funcTypesByParamCount.TryGetValue(jsParamCount, out var funcType))
            {
                throw new NotSupportedException($"Delegate for {jsParamCount} parameters not supported");
            }

            return _memberRefRegistry.GetOrAddConstructor(funcType);
        }

        public MemberReferenceHandle GetFuncInvokeRef(int jsParamCount)
        {
            // For 0 params, historical snapshots reference Invoke on Func<object[], object>
            if (jsParamCount == 0) return FuncObjectArrayObject_Invoke_Ref;         // Func<object[], object>.Invoke
            // For 1 param, use Func<object[], object, object>
            if (jsParamCount == 1) return FuncObjectArrayObjectObject_Invoke_Ref;         // Func<object[], object, object>.Invoke
            if (_funcArrayParamInvokeRefs.TryGetValue(jsParamCount, out var invoke)) return invoke;
            throw new NotSupportedException($"Invoke ref for {jsParamCount} parameters not initialized");
        }

        private void BuildFuncArrayParamInvoke(MetadataBuilder metadataBuilder, int jsParamCount)
        {
            // Build the constructed generic type for GetOrAddMethod
            Type funcType = (jsParamCount + 2) switch
            {
                4 => typeof(System.Func<object[], object, object, object>),
                5 => typeof(System.Func<object[], object, object, object, object>),
                6 => typeof(System.Func<object[], object, object, object, object, object>),
                7 => typeof(System.Func<object[], object, object, object, object, object, object>),
                8 => typeof(System.Func<object[], object, object, object, object, object, object, object>),
                _ => throw new NotSupportedException($"Unsupported generic arity {jsParamCount + 2} for invoke")
            };

            var invokeRef = _memberRefRegistry.GetOrAddMethod(funcType, "Invoke");
            _funcArrayParamInvokeRefs[jsParamCount] = invokeRef;
        }
    }
}
