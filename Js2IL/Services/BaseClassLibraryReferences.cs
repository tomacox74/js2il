using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    public class BaseClassLibraryReferences
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

        public BaseClassLibraryReferences(TypeReferenceRegistry typeRefRegistr, MemberReferenceRegistry memberRefRegistr)
        {
            _typeRefRegistry = typeRefRegistr;
            _memberRefRegistry = memberRefRegistr;
        }

        internal TypeReferenceRegistry TypeReferenceRegistry => _typeRefRegistry;
        
        public TypeReferenceHandle BooleanType => _typeRefRegistry.GetOrAdd(typeof(bool));
        public TypeReferenceHandle DoubleType => _typeRefRegistry.GetOrAdd(typeof(double));
        public TypeReferenceHandle Int32Type => _typeRefRegistry.GetOrAdd(typeof(int));
        public TypeReferenceHandle ObjectType => _typeRefRegistry.GetOrAdd(typeof(object));
        public TypeReferenceHandle StringType => _typeRefRegistry.GetOrAdd(typeof(string));
        public TypeReferenceHandle ExceptionType => _typeRefRegistry.GetOrAdd(typeof(System.Exception));
        public TypeReferenceHandle SystemMathType => _typeRefRegistry.GetOrAdd(typeof(System.Math));
        public TypeReferenceHandle MethodBaseType => _typeRefRegistry.GetOrAdd(typeof(System.Reflection.MethodBase));

        public MemberReferenceHandle Expando_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Dynamic.ExpandoObject), Type.EmptyTypes);
        public MemberReferenceHandle Object_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(object), Type.EmptyTypes);
        public MemberReferenceHandle AsyncScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.AsyncScope), Type.EmptyTypes);
        public MemberReferenceHandle AsyncGeneratorScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.AsyncGeneratorScope), Type.EmptyTypes);
        public MemberReferenceHandle GeneratorScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.GeneratorScope), Type.EmptyTypes);
        public MemberReferenceHandle IDictionary_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.IDictionary<string, object>), "set_Item");
        public MemberReferenceHandle Array_Add_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), nameof(System.Collections.Generic.List<object>.Add));
        public MemberReferenceHandle Array_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), "set_Item");
        public MemberReferenceHandle Array_GetCount_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.List<object>), $"get_{nameof(System.Collections.Generic.List<object>.Count)}");
        public MemberReferenceHandle Action_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Action), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle ModuleMainDelegate_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.CommonJS.ModuleMainDelegate), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle MethodBase_GetCurrentMethod_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Reflection.MethodBase), nameof(System.Reflection.MethodBase.GetCurrentMethod), Type.EmptyTypes);
        public MemberReferenceHandle String_Concat_Ref => _memberRefRegistry.GetOrAddMethod(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) });

        public MemberReferenceHandle JsCompiledModuleAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Js2IL.Runtime.JsCompiledModuleAttribute), new[] { typeof(string) });

        public MemberReferenceHandle JsModuleAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Js2IL.Runtime.JsModuleAttribute), new[] { typeof(string) });

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

            return _memberRefRegistry.GetOrAddMethod(funcType, nameof(System.Func<object[], object>.Invoke));
        }
    }
}
