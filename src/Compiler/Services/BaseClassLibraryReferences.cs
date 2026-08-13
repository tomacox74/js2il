using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services
{
    public class BaseClassLibraryReferences
    {
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;

        // Generated resumable state machines retain one private delegate boundary.
        // These delegates are immediately wrapped in CompiledContinuation and never
        // become JavaScript function values.
        internal static Type GetContinuationDelegateType(int jsParamCount)
        {
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
                    $"Continuation delegate for {jsParamCount} parameters not supported (max supported is 32)")
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
        public TypeReferenceHandle SystemMathType => _typeRefRegistry.GetOrAdd(typeof(System.Math));
        public TypeReferenceHandle MethodBaseType => _typeRefRegistry.GetOrAdd(typeof(System.Reflection.MethodBase));
        public TypeReferenceHandle JsFunctionObjectType => _typeRefRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsFunctionObject));
        public TypeReferenceHandle JsClassConstructorObjectType =>
            _typeRefRegistry.GetOrAdd(
                typeof(JavaScriptRuntime.JsClassConstructorObject));
        public TypeReferenceHandle JsAsyncFunctionObjectType =>
            _typeRefRegistry.GetOrAdd(
                typeof(JavaScriptRuntime.JsAsyncFunctionObject));
        public TypeReferenceHandle PromiseType =>
            _typeRefRegistry.GetOrAdd(typeof(JavaScriptRuntime.Promise));
        public TypeReferenceHandle JsCallArgumentsType => _typeRefRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsCallArguments));
        public TypeReferenceHandle InAttributeType => _typeRefRegistry.GetOrAdd(typeof(System.Runtime.InteropServices.InAttribute));
        public EntityHandle ObjectArrayType => _memberRefRegistry.GetOrAddTypeHandle(typeof(object[]));

        public MemberReferenceHandle Object_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(object), Type.EmptyTypes);
        public MemberReferenceHandle JsObject_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.JsObject), Type.EmptyTypes);
        public MemberReferenceHandle JsFunctionObject_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.JsFunctionObject), Type.EmptyTypes);
        public MemberReferenceHandle JsClassConstructorObject_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(
                typeof(JavaScriptRuntime.JsClassConstructorObject),
                Type.EmptyTypes);
        public MemberReferenceHandle JsAsyncFunctionObject_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(
                typeof(JavaScriptRuntime.JsAsyncFunctionObject),
                Type.EmptyTypes);
        public MemberReferenceHandle JsCallArguments_GetArgument_Ref => _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.JsCallArguments),
            nameof(JavaScriptRuntime.JsCallArguments.GetArgument),
            new[] { typeof(int) });
        public MemberReferenceHandle JsCallArguments_ToArray_Ref => _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.JsCallArguments),
            nameof(JavaScriptRuntime.JsCallArguments.ToArray),
            Type.EmptyTypes);
        public MemberReferenceHandle Function_ConstructGeneratedFunctionObject_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Function),
                nameof(JavaScriptRuntime.Function.ConstructGeneratedFunctionObject),
                new[]
                {
                    typeof(JavaScriptRuntime.JsFunctionObject),
                    typeof(JavaScriptRuntime.JsCallArguments),
                    typeof(object)
                });
        public MemberReferenceHandle TypeUtilities_ToNumber_Object_Ref => _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
            new[] { typeof(object) });
        public MemberReferenceHandle TypeUtilities_ToBoolean_Object_Ref => _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            new[] { typeof(object) });
        public MemberReferenceHandle DotNet2JSConversions_ToString_Ref => _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.DotNet2JSConversions),
            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
            new[] { typeof(object) });
        public MemberReferenceHandle TypeError_Ctor_String_Ref => _memberRefRegistry.GetOrAddConstructor(
            typeof(JavaScriptRuntime.TypeError),
            new[] { typeof(string) });
        public MemberReferenceHandle GeneratorObject_InitializeGeneratorFunctionSurface_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.GeneratorObject),
                nameof(JavaScriptRuntime.GeneratorObject.InitializeGeneratorFunctionSurface),
                new[] { typeof(object) });
        public MemberReferenceHandle AsyncGeneratorFunction_InitializeFunctionObject_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.AsyncGeneratorFunction),
                nameof(JavaScriptRuntime.AsyncGeneratorFunction.InitializeFunctionObject),
                new[] { typeof(object) });
        public MemberReferenceHandle GeneratorObject_InitializeInstanceFromFunction_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.GeneratorObject),
                nameof(JavaScriptRuntime.GeneratorObject.InitializeInstanceFromFunction),
                new[] { typeof(object), typeof(object) });
        public MemberReferenceHandle RuntimeServices_ResolveLexicalThis_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.ResolveLexicalThis),
                new[] { typeof(object) });
        public MemberReferenceHandle RuntimeServices_GetCurrentNewTarget_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.GetCurrentNewTarget),
                Type.EmptyTypes);
        public MemberReferenceHandle RuntimeServices_GetArgumentOrUndefined_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.GetArgumentOrUndefined),
                new[] { typeof(object[]), typeof(int) });
        public MemberReferenceHandle RuntimeServices_PushGeneratedFunctionDirectCall_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.PushGeneratedFunctionDirectCall),
                new[]
                {
                    typeof(JavaScriptRuntime.JsFunctionObject),
                    typeof(object[])
                });
        public MemberReferenceHandle RuntimeServices_PopGeneratedFunctionDirectCall_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.PopGeneratedFunctionDirectCall),
                Type.EmptyTypes);
        public MemberReferenceHandle Type_GetTypeFromHandle_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(Type),
                nameof(Type.GetTypeFromHandle),
                new[] { typeof(RuntimeTypeHandle) });
        public MemberReferenceHandle RuntimeServices_ResolveGeneratedClassMethodReceiver_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.ResolveGeneratedClassMethodReceiver),
                new[]
                {
                    typeof(object),
                    typeof(Type),
                    typeof(object[]),
                    typeof(object),
                    typeof(JavaScriptRuntime.JsFunctionObject)
                });
        public MemberReferenceHandle RuntimeServices_ValidateGeneratedStaticMethodReceiver_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.ValidateGeneratedStaticMethodReceiver),
                new[]
                {
                    typeof(object),
                    typeof(Type),
                    typeof(object),
                    typeof(JavaScriptRuntime.JsFunctionObject)
                });
        public MemberReferenceHandle Function_ResolveOrdinaryThisArgument_Ref =>
            _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Function),
                nameof(JavaScriptRuntime.Function.ResolveOrdinaryThisArgument),
                new[] { typeof(object) });
        public MemberReferenceHandle AsyncScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.AsyncScope), Type.EmptyTypes);
        public MemberReferenceHandle AsyncGeneratorScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.AsyncGeneratorScope), Type.EmptyTypes);
        public MemberReferenceHandle GeneratorScope_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.GeneratorScope), Type.EmptyTypes);
        public MemberReferenceHandle IDictionary_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Collections.Generic.IDictionary<string, object>), "set_Item");
        public MemberReferenceHandle Array_Add_Ref => _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Array), nameof(JavaScriptRuntime.Array.Add), new[] { typeof(object) });
        public MemberReferenceHandle Array_SetItem_Ref => _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Array), "set_Item");
        public MemberReferenceHandle Array_GetCount_Ref => _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Array), $"get_{nameof(JavaScriptRuntime.Array.Count)}");
        public MemberReferenceHandle Action_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(System.Action), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle ModuleMainDelegate_Ctor_Ref => _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Modules.CommonJS.ModuleMainDelegate), new[] { typeof(object), typeof(IntPtr) });
        public MemberReferenceHandle MethodBase_GetCurrentMethod_Ref => _memberRefRegistry.GetOrAddMethod(typeof(System.Reflection.MethodBase), nameof(System.Reflection.MethodBase.GetCurrentMethod), Type.EmptyTypes);
        public MemberReferenceHandle String_Concat_Ref => _memberRefRegistry.GetOrAddMethod(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) });

        public MemberReferenceHandle JsCompiledModuleAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Jroc.Runtime.JsCompiledModuleAttribute), new[] { typeof(string) });

        public MemberReferenceHandle JsCompiledModuleTypeAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Jroc.Runtime.JsCompiledModuleTypeAttribute), new[] { typeof(string), typeof(string), typeof(string) });

        public MemberReferenceHandle JsModuleAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Jroc.Runtime.JsModuleAttribute), new[] { typeof(string) });

        public MemberReferenceHandle JsCallableScopeAbiAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(Jroc.Runtime.JsCallableScopeAbiAttribute), new[] { typeof(Jroc.Runtime.CallableScopeAbiKind) });

        public MemberReferenceHandle DebuggableAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggableAttribute), new[] { typeof(bool), typeof(bool) });

        public MemberReferenceHandle DebuggerDisplayAttribute_Ctor_Ref =>
            _memberRefRegistry.GetOrAddConstructor(typeof(System.Diagnostics.DebuggerDisplayAttribute), new[] { typeof(string) });

        public MemberReferenceHandle GetContinuationDelegateCtorRef(
            int jsParamCount)
        {
            var delegateType = GetContinuationDelegateType(jsParamCount);
            return _memberRefRegistry.GetOrAddConstructor(delegateType);
        }
    }
}
