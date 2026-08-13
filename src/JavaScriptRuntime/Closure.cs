using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Jroc.Runtime;

namespace JavaScriptRuntime
{
    public static class Closure
    {
        internal sealed class DelegateInvokeMetadata
        {
            public DelegateInvokeMetadata(
                ParameterInfo[] parameters,
                JsCallableScopeAbiDescriptor abi,
                bool isJsFuncDelegate,
                bool hasNewTarget,
                int jsParamStart,
                bool hasParamsArray,
                int fixedJsParamCount)
            {
                Parameters = parameters;
                Abi = abi;
                IsJsFuncDelegate = isJsFuncDelegate;
                HasNewTarget = hasNewTarget;
                JsParamStart = jsParamStart;
                HasParamsArray = hasParamsArray;
                FixedJsParamCount = fixedJsParamCount;
            }

            public ParameterInfo[] Parameters { get; }

            public JsCallableScopeAbiDescriptor Abi { get; }

            public bool IsJsFuncDelegate { get; }

            public bool HasNewTarget { get; }

            public int JsParamStart { get; }

            public bool HasParamsArray { get; }

            public int FixedJsParamCount { get; }
        }

        private sealed class DelegateMethodInvokeMetadataCache
        {
            private readonly ConcurrentDictionary<bool, DelegateInvokeMetadata> _metadataByBoundStaticState = new();

            public DelegateInvokeMetadata GetOrAdd(
                bool isBoundStatic,
                Func<DelegateInvokeMetadata> valueFactory)
                => _metadataByBoundStaticState.GetOrAdd(
                    isBoundStatic,
                    static (_, factory) => factory(),
                    valueFactory);
        }

        private sealed class DelegateTypeInvokeMetadataCache
        {
            public ConditionalWeakTable<MethodInfo, DelegateMethodInvokeMetadataCache> Methods { get; } = new();
        }

        private static readonly ConditionalWeakTable<Type, DelegateTypeInvokeMetadataCache> _delegateInvokeMetadata = new();

        private static bool TryInvokeTypedJsFuncDelegate(
            Delegate target,
            bool hasScopes,
            int fixedJsParamCount,
            object[] scopes,
            object?[] args,
            object? newTarget,
            out object? result)
        {
            object? Arg(int i) => i < args.Length ? args[i] : null;

            if (hasScopes)
            {
                switch (fixedJsParamCount)
                {
                    case 0:
                        if (target is JsFunc0 jsf0) { result = jsf0(scopes, newTarget)!; return true; }
                        break;
                    case 1:
                        if (target is JsFunc1 jsf1) { result = jsf1(scopes, newTarget, Arg(0))!; return true; }
                        break;
                    case 2:
                        if (target is JsFunc2 jsf2) { result = jsf2(scopes, newTarget, Arg(0), Arg(1))!; return true; }
                        break;
                    case 3:
                        if (target is JsFunc3 jsf3) { result = jsf3(scopes, newTarget, Arg(0), Arg(1), Arg(2))!; return true; }
                        break;
                    case 4:
                        if (target is JsFunc4 jsf4) { result = jsf4(scopes, newTarget, Arg(0), Arg(1), Arg(2), Arg(3))!; return true; }
                        break;
                    case 5:
                        if (target is JsFunc5 jsf5) { result = jsf5(scopes, newTarget, Arg(0), Arg(1), Arg(2), Arg(3), Arg(4))!; return true; }
                        break;
                }
            }
            else
            {
                switch (fixedJsParamCount)
                {
                    case 0:
                        if (target is JsFuncNoScopes0 jsf0) { result = jsf0(newTarget)!; return true; }
                        break;
                    case 1:
                        if (target is JsFuncNoScopes1 jsf1) { result = jsf1(newTarget, Arg(0))!; return true; }
                        break;
                    case 2:
                        if (target is JsFuncNoScopes2 jsf2) { result = jsf2(newTarget, Arg(0), Arg(1))!; return true; }
                        break;
                    case 3:
                        if (target is JsFuncNoScopes3 jsf3) { result = jsf3(newTarget, Arg(0), Arg(1), Arg(2))!; return true; }
                        break;
                    case 4:
                        if (target is JsFuncNoScopes4 jsf4) { result = jsf4(newTarget, Arg(0), Arg(1), Arg(2), Arg(3))!; return true; }
                        break;
                    case 5:
                        if (target is JsFuncNoScopes5 jsf5) { result = jsf5(newTarget, Arg(0), Arg(1), Arg(2), Arg(3), Arg(4))!; return true; }
                        break;
                }
            }

            result = null;
            return false;
        }

        internal static DelegateInvokeMetadata GetDelegateInvokeMetadata(Delegate target)
        {
            var delegateType = target.GetType();
            var methodCache = _delegateInvokeMetadata
                .GetOrCreateValue(delegateType)
                .Methods
                .GetOrCreateValue(target.Method);
            var invoke = delegateType.GetMethod("Invoke")
                ?? throw new ArgumentException($"Delegate type '{delegateType}' does not define Invoke().", "target");
            var parameters = invoke.GetParameters();
            var isBoundStatic = target.Target != null
                && target.Method.IsStatic
                && parameters.Length == System.Math.Max(
                    0,
                    target.Method.GetParameters().Length - 1);

            return methodCache.GetOrAdd(isBoundStatic, () =>
            {
                var abi = JsCallableScopeAbiResolver.Resolve(target);
                bool hasScopes = abi.HasExplicitScopePayload;
                bool hasNewTarget = JsCallableScopeAbiResolver.HasNewTargetParameter(target, parameters, abi.Kind);
                int jsParamStart = hasScopes
                    ? (hasNewTarget ? 2 : 1)
                    : (hasNewTarget ? 1 : 0);
                int expectedJsParamCount = parameters.Length - jsParamStart;

                // ParamArrayAttribute is not preserved on delegate Invoke() parameters when a delegate is created
                // from a method using ldftn/newobj. For intrinsic delegates (e.g., timers), treat a trailing
                // object[] parameter as a params-array as well.
                bool hasParamsArray = expectedJsParamCount > 0
                    && (
                        Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute))
                        || (parameters[^1].ParameterType.IsArray && parameters[^1].ParameterType.GetElementType() == typeof(object))
                    );

                return new DelegateInvokeMetadata(
                    parameters,
                    abi,
                    JsFuncDelegates.IsJsFuncDelegateType(delegateType),
                    hasNewTarget,
                    jsParamStart,
                    hasParamsArray,
                    hasParamsArray ? expectedJsParamCount - 1 : expectedJsParamCount);
            });
        }

        private static object InvokeDelegateWithArgs(
            Delegate target,
            DelegateInvokeMetadata metadata,
            object[] scopes,
            object?[] args,
            object? newTarget)
        {
            var parameters = metadata.Parameters;
            var abi = metadata.Abi;
            bool hasScopes = abi.HasExplicitScopePayload;
            bool isJsFuncDelegate = metadata.IsJsFuncDelegate;
            bool hasNewTarget = metadata.HasNewTarget;
            int jsParamStart = metadata.JsParamStart;
            bool hasParamsArray = metadata.HasParamsArray;
            int fixedJsParamCount = metadata.FixedJsParamCount;

            // Fast-path: most JROC-generated functions are strongly typed as Func<object[], object, ... , object>.
            // Avoid Delegate.DynamicInvoke() for these common cases to reduce overhead and (on some runtimes)
            // sidestep reflection invoke stub/JIT edge cases.
            if (!hasParamsArray)
            {
                object? Arg(int i) => i < args.Length ? args[i] : null;

                if (isJsFuncDelegate
                    && abi.Kind != CallableScopeAbiKind.SingleScope
                    && TryInvokeTypedJsFuncDelegate(target, abi.Kind == CallableScopeAbiKind.ScopeArray, fixedJsParamCount, scopes, args, newTarget, out var typedJsFuncResult))
                {
                    return typedJsFuncResult!;
                }

                if (abi.Kind == CallableScopeAbiKind.ScopeArray)
                {
                    switch (fixedJsParamCount)
                    {
                        case 0:
                            if (target is Func<object[], object?> f0) return f0(scopes)!;
                            if (target is Action<object[]> a0) { a0(scopes); return null!; }
                            break;
                        case 1:
                            if (target is Func<object[], object, object?> f1) return f1(scopes, Arg(0)!)!;
                            if (target is Action<object[], object> a1) { a1(scopes, Arg(0)!); return null!; }
                            break;
                        case 2:
                            if (target is Func<object[], object, object, object?> f2) return f2(scopes, Arg(0)!, Arg(1)!)!;
                            if (target is Action<object[], object, object> a2) { a2(scopes, Arg(0)!, Arg(1)!); return null!; }
                            break;
                        case 3:
                            if (target is Func<object[], object, object, object, object?> f3) return f3(scopes, Arg(0)!, Arg(1)!, Arg(2)!)!;
                            if (target is Action<object[], object, object, object> a3) { a3(scopes, Arg(0)!, Arg(1)!, Arg(2)!); return null!; }
                            break;
                        case 4:
                            if (target is Func<object[], object, object, object, object, object?> f4) return f4(scopes, Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!)!;
                            if (target is Action<object[], object, object, object, object> a4) { a4(scopes, Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!); return null!; }
                            break;
                        case 5:
                            if (target is Func<object[], object, object, object, object, object, object?> f5) return f5(scopes, Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!, Arg(4)!)!;
                            if (target is Action<object[], object, object, object, object, object> a5) { a5(scopes, Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!, Arg(4)!); return null!; }
                            break;
                    }
                }
                else if (abi.Kind == CallableScopeAbiKind.NoScopes)
                {
                    switch (fixedJsParamCount)
                    {
                        case 0:
                            if (target is Func<object?> g0) return g0()!;
                            if (target is Action ga0) { ga0(); return null!; }
                            break;
                        case 1:
                            if (target is Func<object, object?> g1) return g1(Arg(0)!)!;
                            if (target is Action<object> ga1) { ga1(Arg(0)!); return null!; }
                            break;
                        case 2:
                            if (target is Func<object, object, object?> g2) return g2(Arg(0)!, Arg(1)!)!;
                            if (target is Action<object, object> ga2) { ga2(Arg(0)!, Arg(1)!); return null!; }
                            break;
                        case 3:
                            if (target is Func<object, object, object, object?> g3) return g3(Arg(0)!, Arg(1)!, Arg(2)!)!;
                            if (target is Action<object, object, object> ga3) { ga3(Arg(0)!, Arg(1)!, Arg(2)!); return null!; }
                            break;
                        case 4:
                            if (target is Func<object, object, object, object, object?> g4) return g4(Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!)!;
                            if (target is Action<object, object, object, object> ga4) { ga4(Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!); return null!; }
                            break;
                        case 5:
                            if (target is Func<object, object, object, object, object, object?> g5) return g5(Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!, Arg(4)!)!;
                            if (target is Action<object, object, object, object, object> ga5) { ga5(Arg(0)!, Arg(1)!, Arg(2)!, Arg(3)!, Arg(4)!); return null!; }
                            break;
                    }
                }
            }

            // Build argument list matching delegate signature.
            // - If delegate includes scopes: first arg is scopes
            // - Missing JS args => null
            // - Extra JS args ignored
            var finalArgs = new object?[parameters.Length];

            int finalIndex = 0;
            if (hasScopes)
            {
                finalArgs[finalIndex++] = abi.Kind == CallableScopeAbiKind.ScopeArray
                    ? scopes
                    : JsCallableScopeAbiResolver.GetSingleScopeArgument(scopes, abi.SingleScopeType);
                if (hasNewTarget)
                {
                    finalArgs[finalIndex++] = newTarget;
                }
            }
            else if (hasNewTarget)
            {
                finalArgs[finalIndex++] = newTarget;
            }

            // Fixed parameters
            for (int i = 0; i < fixedJsParamCount; i++)
            {
                if (i < args.Length)
                {
                    finalArgs[finalIndex++] = args[i];
                    continue;
                }

                // Missing JS args are 'undefined' (modeled as null), but for array-typed CLR params we prefer
                // passing an empty array to avoid null dereferences in intrinsic implementations.
                var parameterType = parameters[jsParamStart + i].ParameterType;
                if (parameterType.IsArray)
                {
                    var elementType = parameterType.GetElementType() ?? typeof(object);
                    finalArgs[finalIndex++] = System.Array.CreateInstance(elementType, 0);
                }
                else
                {
                    finalArgs[finalIndex++] = null;
                }
            }

            // params array parameter packs remaining args (including zero args) into a CLR array.
            if (hasParamsArray)
            {
                var paramsElementType = parameters[^1].ParameterType.GetElementType() ?? typeof(object);

                int restCount = args.Length > fixedJsParamCount ? args.Length - fixedJsParamCount : 0;
                var packed = System.Array.CreateInstance(paramsElementType, restCount);
                for (int i = 0; i < restCount; i++)
                {
                    packed.SetValue(args[fixedJsParamCount + i], i);
                }
                finalArgs[finalIndex] = packed;
            }

            try
            {
                // Delegate.DynamicInvoke returns boxed value types; null for void.
                // NOTE: Some call sites may hand us an open-instance delegate (Target == null, Method.IsStatic == false).
                // In that case, DynamicInvoke will interpret finalArgs[0] as the instance receiver.
                // If it is null, DynamicInvoke throws ArgumentNullException("instance").
                return target.DynamicInvoke(finalArgs)!;
            }
            catch (ArgumentNullException ane)
            {
                var method = target.Method;
                var delegateType = target.GetType();
                var openInstance = target.Target == null && !method.IsStatic;

                if (Environment.GetEnvironmentVariable("JROC_CLOSURE_DIAG") == "1")
                {
                    global::System.Console.WriteLine("[closure] DynamicInvoke failed: null instance");
                    global::System.Console.WriteLine($"[closure] DelegateType: {delegateType.FullName}");
                    global::System.Console.WriteLine($"[closure] Method: {method.DeclaringType?.FullName}::{method.Name} (IsStatic={method.IsStatic})");
                    global::System.Console.WriteLine($"[closure] Target: {(target.Target == null ? "<null>" : target.Target.GetType().FullName)}");
                    global::System.Console.WriteLine($"[closure] OpenInstance: {openInstance}");
                    global::System.Console.WriteLine($"[closure] ParamTypes: {string.Join(", ", parameters.Select(p => p.ParameterType.FullName))}");
                    global::System.Console.WriteLine($"[closure] JS args length: {args.Length}");
                    global::System.Console.WriteLine($"[closure] CurrentThis: {(RuntimeServices.GetCurrentThis() == null ? "<null>" : RuntimeServices.GetCurrentThis()!.GetType().FullName)}");
                    global::System.Console.WriteLine($"[closure] ArgumentNullException.ParamName: {ane.ParamName ?? "<null>"}");
                }

                // Best-effort recovery for open-instance delegates: if the delegate has NO scopes parameter and
                // appears to be expecting an instance as its first parameter, try to use the current JS `this`.
                if (openInstance && !hasScopes && parameters.Length > 0)
                {
                    var thisArg = RuntimeServices.GetCurrentThis();
                    if (thisArg != null)
                    {
                        finalArgs[0] = thisArg;
                        return target.DynamicInvoke(finalArgs)!;
                    }
                }

                throw;
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                if (Environment.GetEnvironmentVariable("JROC_CLOSURE_DIAG") == "1")
                {
                    var method = target.Method;
                    var delegateType = target.GetType();

                    global::System.Console.WriteLine("[closure] DynamicInvoke threw TargetInvocationException");
                    global::System.Console.WriteLine($"[closure] DelegateType: {delegateType.FullName}");
                    global::System.Console.WriteLine($"[closure] Method: {method.DeclaringType?.FullName}::{method.Name} (IsStatic={method.IsStatic})");
                    global::System.Console.WriteLine($"[closure] Target: {(target.Target == null ? "<null>" : target.Target.GetType().FullName)}");
                    global::System.Console.WriteLine($"[closure] Inner: {tie.InnerException.GetType().FullName}: {tie.InnerException.Message}");

                    // Summarize args (avoid huge dumps)
                    string ArgSummary(object? o) => o == null ? "<null>" : (o.GetType().FullName ?? o.GetType().Name);
                    global::System.Console.WriteLine($"[closure] FinalArgs: {string.Join(", ", finalArgs.Select(ArgSummary))}");
                }

                // Preserve original stack trace from the invoked method.
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                throw; // unreachable
            }
        }

        internal static object? InvokeContinuationTarget(
            Delegate target,
            DelegateInvokeMetadata metadata,
            object[] scopes,
            object?[] arguments)
            => InvokeDelegateWithArgs(
                target,
                metadata,
                scopes,
                arguments,
                newTarget: null);

        internal static object? InvokeBuiltinDelegate(
            Delegate target,
            DelegateInvokeMetadata metadata,
            object[] scopes,
            in JsCallArguments arguments,
            object? newTarget)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(scopes);

            if (!metadata.HasParamsArray
                && metadata.FixedJsParamCount <= 5)
            {
                var hasScopes = metadata.Abi.HasExplicitScopePayload;

                if (metadata.IsJsFuncDelegate
                    && metadata.Abi.Kind != CallableScopeAbiKind.SingleScope)
                {
                    if (metadata.Abi.Kind == CallableScopeAbiKind.ScopeArray)
                    {
                        switch (metadata.FixedJsParamCount)
                        {
                            case 0 when target is JsFunc0 f0:
                                return f0(scopes, newTarget);
                            case 1 when target is JsFunc1 f1:
                                return f1(scopes, newTarget, arguments.GetArgument(0)!);
                            case 2 when target is JsFunc2 f2:
                                return f2(scopes, newTarget, arguments.GetArgument(0)!, arguments.GetArgument(1)!);
                            case 3 when target is JsFunc3 f3:
                                return f3(scopes, newTarget, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!);
                            case 4 when target is JsFunc4 f4:
                                return f4(scopes, newTarget, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!);
                            case 5 when target is JsFunc5 f5:
                                return f5(scopes, newTarget, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!, arguments.GetArgument(4)!);
                        }
                    }
                    else
                    {
                        switch (metadata.FixedJsParamCount)
                        {
                            case 0 when target is JsFuncNoScopes0 f0:
                                return f0(newTarget);
                            case 1 when target is JsFuncNoScopes1 f1:
                                return f1(newTarget, arguments.GetArgument(0));
                            case 2 when target is JsFuncNoScopes2 f2:
                                return f2(newTarget, arguments.GetArgument(0), arguments.GetArgument(1));
                            case 3 when target is JsFuncNoScopes3 f3:
                                return f3(newTarget, arguments.GetArgument(0), arguments.GetArgument(1), arguments.GetArgument(2));
                            case 4 when target is JsFuncNoScopes4 f4:
                                return f4(newTarget, arguments.GetArgument(0), arguments.GetArgument(1), arguments.GetArgument(2), arguments.GetArgument(3));
                            case 5 when target is JsFuncNoScopes5 f5:
                                return f5(newTarget, arguments.GetArgument(0), arguments.GetArgument(1), arguments.GetArgument(2), arguments.GetArgument(3), arguments.GetArgument(4));
                        }
                    }
                }

                if (metadata.Abi.Kind == CallableScopeAbiKind.ScopeArray)
                {
                    switch (metadata.FixedJsParamCount)
                    {
                        case 0:
                            if (target is Func<object[], object?> f0) return f0(scopes);
                            if (target is Action<object[]> a0) { a0(scopes); return null; }
                            break;
                        case 1:
                            if (target is Func<object[], object, object?> f1) return f1(scopes, arguments.GetArgument(0)!);
                            if (target is Action<object[], object> a1) { a1(scopes, arguments.GetArgument(0)!); return null; }
                            break;
                        case 2:
                            if (target is Func<object[], object, object, object?> f2) return f2(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!);
                            if (target is Action<object[], object, object> a2) { a2(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!); return null; }
                            break;
                        case 3:
                            if (target is Func<object[], object, object, object, object?> f3) return f3(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!);
                            if (target is Action<object[], object, object, object> a3) { a3(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!); return null; }
                            break;
                        case 4:
                            if (target is Func<object[], object, object, object, object, object?> f4) return f4(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!);
                            if (target is Action<object[], object, object, object, object> a4) { a4(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!); return null; }
                            break;
                        case 5:
                            if (target is Func<object[], object, object, object, object, object, object?> f5) return f5(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!, arguments.GetArgument(4)!);
                            if (target is Action<object[], object, object, object, object, object> a5) { a5(scopes, arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!, arguments.GetArgument(4)!); return null; }
                            break;
                    }
                }
                else if (!hasScopes)
                {
                    switch (metadata.FixedJsParamCount)
                    {
                        case 0:
                            if (target is Func<object?> f0) return f0();
                            if (target is Action a0) { a0(); return null; }
                            break;
                        case 1:
                            if (target is Func<object, object?> f1) return f1(arguments.GetArgument(0)!);
                            if (target is Action<object> a1) { a1(arguments.GetArgument(0)!); return null; }
                            break;
                        case 2:
                            if (target is Func<object, object, object?> f2) return f2(arguments.GetArgument(0)!, arguments.GetArgument(1)!);
                            if (target is Action<object, object> a2) { a2(arguments.GetArgument(0)!, arguments.GetArgument(1)!); return null; }
                            break;
                        case 3:
                            if (target is Func<object, object, object, object?> f3) return f3(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!);
                            if (target is Action<object, object, object> a3) { a3(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!); return null; }
                            break;
                        case 4:
                            if (target is Func<object, object, object, object, object?> f4) return f4(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!);
                            if (target is Action<object, object, object, object> a4) { a4(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!); return null; }
                            break;
                        case 5:
                            if (target is Func<object, object, object, object, object, object?> f5) return f5(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!, arguments.GetArgument(4)!);
                            if (target is Action<object, object, object, object, object> a5) { a5(arguments.GetArgument(0)!, arguments.GetArgument(1)!, arguments.GetArgument(2)!, arguments.GetArgument(3)!, arguments.GetArgument(4)!); return null; }
                            break;
                    }
                }
            }

            return InvokeDelegateWithArgs(
                target,
                metadata,
                scopes,
                arguments.ToArray(),
                newTarget);
        }

        private static object InvokeWithArgsCore(object target, object[] scopes, object? newTarget, object?[] args)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call(
                    target,
                    RuntimeServices.GetCurrentThis(),
                    args)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(args.Length > 0 ? args[0] : null)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        // Invoke a function delegate with runtime type inspection to determine the correct arity.
        // This is used when calling a function stored in a variable where the parameter count isn't known at compile time.
        // args should NOT include the scopes array - this method will prepend it.
        public static object InvokeWithArgs(object target, object[] scopes, params object?[] args)
        {
            return InvokeWithArgsCore(target, scopes, newTarget: null, args);
        }

        public static object InvokeWithArgsWithNewTarget(object target, object[] scopes, object? newTarget, params object?[] args)
        {
            return InvokeWithArgsCore(target, scopes, newTarget, args);
        }

        private static object? ResolveFunctionCallThis(object target)
        {
            return target is JsFunctionObject functionObject
                ? functionObject.ResolveThisArgument(null)
                : null;
        }

        public static object InvokeFunctionCallWithArgs(object target, object[] scopes, params object?[] args)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgsCore(target, scopes, newTarget: null, args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs0(object target, object[] scopes)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs0(target, scopes);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs1(object target, object[] scopes, object? a0)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs1(target, scopes, a0);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs2(object target, object[] scopes, object? a0, object? a1)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs2(target, scopes, a0, a1);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs3(object target, object[] scopes, object? a0, object? a1, object? a2)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs3(target, scopes, a0, a1, a2);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs4(
            object target,
            object[] scopes,
            object? a0,
            object? a1,
            object? a2,
            object? a3)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs4(target, scopes, a0, a1, a2, a3);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        public static object InvokeFunctionCallWithArgs5(
            object target,
            object[] scopes,
            object? a0,
            object? a1,
            object? a2,
            object? a3,
            object? a4)
        {
            var previousThis = RuntimeServices.SetCurrentThis(ResolveFunctionCallThis(target));
            try
            {
                return InvokeWithArgs5(target, scopes, a0, a1, a2, a3, a4);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        // Arity-specific overloads for common cases (0-5 args).
        // Raw bootstrap delegates are handled explicitly; JavaScript callables are function objects.

        public static object InvokeWithArgs0(object target, object[] scopes)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call0(target, RuntimeServices.GetCurrentThis())!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(null)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        public static object InvokeWithArgs1(object target, object[] scopes, object? a0)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call1(target, RuntimeServices.GetCurrentThis(), a0)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(a0)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        public static object InvokeWithArgs2(object target, object[] scopes, object? a0, object? a1)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call2(
                    target,
                    RuntimeServices.GetCurrentThis(),
                    a0,
                    a1)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(a0)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        public static object InvokeWithArgs3(object target, object[] scopes, object? a0, object? a1, object? a2)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call3(
                    target,
                    RuntimeServices.GetCurrentThis(),
                    a0,
                    a1,
                    a2)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(a0)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        public static object InvokeWithArgs4(object target, object[] scopes, object? a0, object? a1, object? a2, object? a3)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call4(
                    target,
                    RuntimeServices.GetCurrentThis(),
                    a0,
                    a1,
                    a2,
                    a3)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(a0)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

        public static object InvokeWithArgs5(object target, object[] scopes, object? a0, object? a1, object? a2, object? a3, object? a4)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            if (target is JsFunctionObject or global::JavaScriptRuntime.Proxy)
            {
                return CallableOperations.Call5(
                    target,
                    RuntimeServices.GetCurrentThis(),
                    a0,
                    a1,
                    a2,
                    a3,
                    a4)!;
            }

            if (target is global::JavaScriptRuntime.Modules.CommonJS.RequireDelegate require)
            {
                return require(a0)!;
            }

            throw new TypeError(
                $"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
        }

    }
}
