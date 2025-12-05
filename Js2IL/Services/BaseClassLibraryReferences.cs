using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    internal class BaseClassLibraryReferences
    {
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;
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
            this.ConsoleWriteLine_StringObject_Ref = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                metadataBuilder.GetOrAddBlob(consoleSig));

            // System.Reflection.MethodBase reference
            this.MethodBaseType = _typeRefRegistry.GetOrAdd(typeof(System.Reflection.MethodBase));
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

            LoadObjectTypes(metadataBuilder);

            LoadArrayTypes(metadataBuilder);
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
        public TypeReferenceHandle MethodBaseType { get; private init; }

        // Removed legacy Action<> delegate references (now using Func returning object)

        public TypeSpecificationHandle IDictionary_StringObject_Type { get; private set; }
        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private init; }
        public MemberReferenceHandle Expando_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Dynamic.ExpandoObject), Type.EmptyTypes);
        public TypeReferenceHandle ExpandoObjectType { get; private set; }
        public MemberReferenceHandle Object_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(object), Type.EmptyTypes);
        public MemberReferenceHandle IDictionary_SetItem_Ref { get; private set; }
        public MemberReferenceHandle Array_Add_Ref { get; private set; }
        public MemberReferenceHandle Array_SetItem_Ref { get; private set; }
        public MemberReferenceHandle Array_GetCount_Ref { get; private set; }
        public MemberReferenceHandle Action_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Action), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle MethodBase_GetCurrentMethod_Ref { get; private set; }

        private void LoadObjectTypes(MetadataBuilder metadataBuilder)
        {
            // ExpandObject reference
            // important for the generic case in JavaScript where objects are just property bags
            var systemCoreExpandoType = _typeRefRegistry.GetOrAdd(typeof(System.Dynamic.ExpandoObject));
            // store the ExpandoObject type reference for use as a base class
            ExpandoObjectType = systemCoreExpandoType;

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
            if (!_funcTypesByParamCount.TryGetValue(jsParamCount, out var funcType))
            {
                throw new NotSupportedException($"Delegate for {jsParamCount} parameters not supported");
            }

            return _memberRefRegistry.GetOrAddMethod(funcType, "Invoke");
        }
    }
}
