using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    public class BaseClassLibraryReferences
    {
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;

        // JS function delegates follow the js2il ABI:
        //   (object[] scopes, object? a1..aN) -> object?
        // System.Func<> supports only up to 15 generic args, which limits us to 14 JS parameters
        // (because scopes is the first parameter). For larger arities we use custom delegates.
        private static Type GetFunctionDelegateType(int jsParamCount)
        {
            return jsParamCount switch
            {
                0 => typeof(System.Func<object[], object?>),
                1 => typeof(System.Func<object[], object?, object?>),
                2 => typeof(System.Func<object[], object?, object?, object?>),
                3 => typeof(System.Func<object[], object?, object?, object?, object?>),
                4 => typeof(System.Func<object[], object?, object?, object?, object?, object?>),
                5 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?>),
                6 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?>),
                7 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?>),
                8 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                9 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                10 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                11 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                12 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                13 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                14 => typeof(System.Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),

                15 => typeof(JavaScriptRuntime.JsFunc15),
                16 => typeof(JavaScriptRuntime.JsFunc16),
                17 => typeof(JavaScriptRuntime.JsFunc17),
                18 => typeof(JavaScriptRuntime.JsFunc18),
                19 => typeof(JavaScriptRuntime.JsFunc19),
                20 => typeof(JavaScriptRuntime.JsFunc20),
                21 => typeof(JavaScriptRuntime.JsFunc21),
                22 => typeof(JavaScriptRuntime.JsFunc22),
                23 => typeof(JavaScriptRuntime.JsFunc23),
                24 => typeof(JavaScriptRuntime.JsFunc24),
                25 => typeof(JavaScriptRuntime.JsFunc25),
                26 => typeof(JavaScriptRuntime.JsFunc26),
                27 => typeof(JavaScriptRuntime.JsFunc27),
                28 => typeof(JavaScriptRuntime.JsFunc28),
                29 => typeof(JavaScriptRuntime.JsFunc29),
                30 => typeof(JavaScriptRuntime.JsFunc30),
                31 => typeof(JavaScriptRuntime.JsFunc31),
                32 => typeof(JavaScriptRuntime.JsFunc32),

                _ => throw new NotSupportedException(
                    $"Delegate for {jsParamCount} parameters not supported (max supported is 32)")
            };
        }

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

        public MemberReferenceHandle DebuggableAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggableAttribute), new[] { typeof(bool), typeof(bool) });

        public MemberReferenceHandle DebuggerDisplayAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggerDisplayAttribute), new[] { typeof(string) });

        public MemberReferenceHandle GetFuncCtorRef(int jsParamCount)
        {
            var delegateType = GetFunctionDelegateType(jsParamCount);
            return _memberRefRegistry.GetOrAddConstructor(delegateType);
        }

        public MemberReferenceHandle GetFuncInvokeRef(int jsParamCount)
        {
            var delegateType = GetFunctionDelegateType(jsParamCount);
            return _memberRefRegistry.GetOrAddMethod(delegateType, "Invoke");
        }
    }
}
