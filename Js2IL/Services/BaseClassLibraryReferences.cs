using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    public class BaseClassLibraryReferences
    {
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;

        // JS function delegates follow one of two ABIs:
        // With scopes: (object[] scopes, object? newTarget, object? a1..aN) -> object?
        // Without scopes: (object? newTarget, object? a1..aN) -> object?
        // Always use custom JsFunc delegates so the ABI is explicit and stable.
        internal static Type GetFunctionDelegateType(int jsParamCount, bool requiresScopes = true)
        {
            if (!requiresScopes)
            {
                // No-scopes optimized delegates
                return jsParamCount switch
                {
                    0 => typeof(JavaScriptRuntime.JsFuncNoScopes0),
                    1 => typeof(JavaScriptRuntime.JsFuncNoScopes1),
                    2 => typeof(JavaScriptRuntime.JsFuncNoScopes2),
                    3 => typeof(JavaScriptRuntime.JsFuncNoScopes3),
                    4 => typeof(JavaScriptRuntime.JsFuncNoScopes4),
                    5 => typeof(JavaScriptRuntime.JsFuncNoScopes5),
                    6 => typeof(JavaScriptRuntime.JsFuncNoScopes6),
                    7 => typeof(JavaScriptRuntime.JsFuncNoScopes7),
                    8 => typeof(JavaScriptRuntime.JsFuncNoScopes8),
                    9 => typeof(JavaScriptRuntime.JsFuncNoScopes9),
                    10 => typeof(JavaScriptRuntime.JsFuncNoScopes10),
                    11 => typeof(JavaScriptRuntime.JsFuncNoScopes11),
                    12 => typeof(JavaScriptRuntime.JsFuncNoScopes12),
                    13 => typeof(JavaScriptRuntime.JsFuncNoScopes13),
                    14 => typeof(JavaScriptRuntime.JsFuncNoScopes14),
                    15 => typeof(JavaScriptRuntime.JsFuncNoScopes15),
                    16 => typeof(JavaScriptRuntime.JsFuncNoScopes16),
                    17 => typeof(JavaScriptRuntime.JsFuncNoScopes17),
                    18 => typeof(JavaScriptRuntime.JsFuncNoScopes18),
                    19 => typeof(JavaScriptRuntime.JsFuncNoScopes19),
                    20 => typeof(JavaScriptRuntime.JsFuncNoScopes20),
                    21 => typeof(JavaScriptRuntime.JsFuncNoScopes21),
                    22 => typeof(JavaScriptRuntime.JsFuncNoScopes22),
                    23 => typeof(JavaScriptRuntime.JsFuncNoScopes23),
                    24 => typeof(JavaScriptRuntime.JsFuncNoScopes24),
                    25 => typeof(JavaScriptRuntime.JsFuncNoScopes25),
                    26 => typeof(JavaScriptRuntime.JsFuncNoScopes26),
                    27 => typeof(JavaScriptRuntime.JsFuncNoScopes27),
                    28 => typeof(JavaScriptRuntime.JsFuncNoScopes28),
                    29 => typeof(JavaScriptRuntime.JsFuncNoScopes29),
                    30 => typeof(JavaScriptRuntime.JsFuncNoScopes30),
                    31 => typeof(JavaScriptRuntime.JsFuncNoScopes31),
                    32 => typeof(JavaScriptRuntime.JsFuncNoScopes32),
                    _ => throw new NotSupportedException(
                        $"Delegate for {jsParamCount} parameters not supported (max supported is 32)")
                };
            }
            
            // Standard delegates with scopes
            return jsParamCount switch
            {
            0 => typeof(JavaScriptRuntime.JsFunc0),
            1 => typeof(JavaScriptRuntime.JsFunc1),
            2 => typeof(JavaScriptRuntime.JsFunc2),
            3 => typeof(JavaScriptRuntime.JsFunc3),
            4 => typeof(JavaScriptRuntime.JsFunc4),
            5 => typeof(JavaScriptRuntime.JsFunc5),
            6 => typeof(JavaScriptRuntime.JsFunc6),
            7 => typeof(JavaScriptRuntime.JsFunc7),
            8 => typeof(JavaScriptRuntime.JsFunc8),
            9 => typeof(JavaScriptRuntime.JsFunc9),
            10 => typeof(JavaScriptRuntime.JsFunc10),
            11 => typeof(JavaScriptRuntime.JsFunc11),
            12 => typeof(JavaScriptRuntime.JsFunc12),
            13 => typeof(JavaScriptRuntime.JsFunc13),
            14 => typeof(JavaScriptRuntime.JsFunc14),
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

        public MemberReferenceHandle JsCompiledModuleTypeAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Js2IL.Runtime.JsCompiledModuleTypeAttribute), new[] { typeof(string), typeof(string), typeof(string) });

        public MemberReferenceHandle JsModuleAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Js2IL.Runtime.JsModuleAttribute), new[] { typeof(string) });

        public MemberReferenceHandle DebuggableAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggableAttribute), new[] { typeof(bool), typeof(bool) });

        public MemberReferenceHandle DebuggerDisplayAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggerDisplayAttribute), new[] { typeof(string) });

        public MemberReferenceHandle GetFuncCtorRef(int jsParamCount, bool requiresScopes = true)
        {
            var delegateType = GetFunctionDelegateType(jsParamCount, requiresScopes);
            return _memberRefRegistry.GetOrAddConstructor(delegateType);
        }

        public MemberReferenceHandle GetFuncInvokeRef(int jsParamCount, bool requiresScopes = true)
        {
            var delegateType = GetFunctionDelegateType(jsParamCount, requiresScopes);
            return _memberRefRegistry.GetOrAddMethod(delegateType, "Invoke");
        }

        public MemberReferenceHandle GetInvokeDirectWithArgsRef(int jsParamCount)
        {
            var delegateType = GetFunctionDelegateType(jsParamCount);
            return _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Closure),
                nameof(JavaScriptRuntime.Closure.InvokeDirectWithArgs),
                new[] { delegateType, typeof(object[]), typeof(object[]) });
        }
    }
}
