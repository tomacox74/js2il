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

        public MemberReferenceHandle Expando_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Dynamic.ExpandoObject), Type.EmptyTypes);
        public MemberReferenceHandle Object_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(object), Type.EmptyTypes);
        public MemberReferenceHandle IDictionary_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.IDictionary<string, object>), "set_Item");
        public MemberReferenceHandle Array_Add_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), "Add");
        public MemberReferenceHandle Array_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), "set_Item");
        public MemberReferenceHandle Array_GetCount_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), "get_Count");
        public MemberReferenceHandle Action_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Action), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle MethodBase_GetCurrentMethod_Ref { get; private set; }

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
