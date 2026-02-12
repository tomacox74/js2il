using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace JavaScriptRuntime
{
    public static class Closure
    {
        private static readonly MethodInfo InvokeWithArgsMethod = typeof(Closure).GetMethod(
            nameof(InvokeWithArgs),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(object), typeof(object[]), typeof(object[]) },
            modifiers: null)
            ?? throw new InvalidOperationException("Failed to resolve Closure.InvokeWithArgs(object, object[], object[]).");

        private static readonly MethodInfo InvokeWithArgsWithThisMethod = typeof(Closure).GetMethod(
            nameof(InvokeWithArgsWithThis),
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("Failed to resolve Closure.InvokeWithArgsWithThis(...).");

        private static T InvokeWithThis<T>(object? boundThis, Func<T> invoke)
        {
            var previous = RuntimeServices.SetCurrentThis(boundThis);
            try
            {
                return invoke();
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previous);
            }
        }

        private static object InvokeWithArgsWithThis(object? boundThis, object target, object[] scopes, object?[] args)
        {
            return InvokeWithThis(boundThis, () => InvokeWithArgs(target, scopes, args));
        }

        private static object InvokeDelegateWithArgs(Delegate target, object[] scopes, object?[] args)
        {
            var invoke = target.GetType().GetMethod("Invoke")
                ?? throw new ArgumentException($"Delegate type '{target.GetType()}' does not define Invoke().", nameof(target));
            var parameters = invoke.GetParameters();

            bool hasScopes = parameters.Length > 0 && parameters[0].ParameterType == typeof(object[]);
            int jsParamStart = hasScopes ? 1 : 0;
            int expectedJsParamCount = parameters.Length - jsParamStart;

            // ParamArrayAttribute is not preserved on delegate Invoke() parameters when a delegate is created
            // from a method using ldftn/newobj. For intrinsic delegates (e.g., timers), treat a trailing
            // object[] parameter as a params-array as well.
            bool hasParamsArray = expectedJsParamCount > 0
                && (
                    Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute))
                    || (parameters[^1].ParameterType.IsArray && parameters[^1].ParameterType.GetElementType() == typeof(object))
                );
            int fixedJsParamCount = hasParamsArray ? expectedJsParamCount - 1 : expectedJsParamCount;

            // Fast-path: most JS2IL-generated functions are strongly typed as Func<object[], object, ... , object>.
            // Avoid Delegate.DynamicInvoke() for these common cases to reduce overhead and (on some runtimes)
            // sidestep reflection invoke stub/JIT edge cases.
            if (!hasParamsArray)
            {
                object? Arg(int i) => i < args.Length ? args[i] : null;

                if (hasScopes)
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
                else
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
                finalArgs[finalIndex++] = scopes;
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

                if (Environment.GetEnvironmentVariable("JS2IL_CLOSURE_DIAG") == "1")
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
                if (Environment.GetEnvironmentVariable("JS2IL_CLOSURE_DIAG") == "1")
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

        private static Delegate CreateBoundDelegate(Delegate target, object[] boundScopes, object? boundThis)
        {
            var delegateType = target.GetType();
            var invoke = delegateType.GetMethod("Invoke")
                ?? throw new ArgumentException($"Delegate type '{delegateType}' does not define Invoke().", nameof(target));

            var parameters = invoke.GetParameters();
            var lambdaParameters = parameters
                .Select((p, i) => Expression.Parameter(p.ParameterType, p.Name ?? $"p{i}"))
                .ToArray();

            bool hasScopes = parameters.Length > 0 && parameters[0].ParameterType == typeof(object[]);
            int jsParamStart = hasScopes ? 1 : 0;

            var jsArgs = lambdaParameters
                .Skip(jsParamStart)
                .Select(p => (Expression)Expression.Convert(p, typeof(object)))
                .ToArray();
            var argsArray = Expression.NewArrayInit(typeof(object), jsArgs);

            Expression invokeWithArgsCall;
            if (boundThis == null)
            {
                invokeWithArgsCall = Expression.Call(
                    InvokeWithArgsMethod,
                    Expression.Constant((object)target, typeof(object)),
                    Expression.Constant(boundScopes, typeof(object[])),
                    argsArray);
            }
            else
            {
                invokeWithArgsCall = Expression.Call(
                    InvokeWithArgsWithThisMethod,
                    Expression.Constant(boundThis, typeof(object)),
                    Expression.Constant((object)target, typeof(object)),
                    Expression.Constant(boundScopes, typeof(object[])),
                    argsArray);
            }

            Expression body;
            if (invoke.ReturnType == typeof(void))
            {
                body = Expression.Block(invokeWithArgsCall, Expression.Empty());
            }
            else
            {
                body = Expression.Convert(invokeWithArgsCall, invoke.ReturnType);
            }

            return Expression.Lambda(delegateType, body, lambdaParameters).Compile();
        }

        // Bind a function delegate (object-typed) to a fixed scopes array AND a fixed set of JS arguments.
        // Returns a Func<object[], object> suitable for AsyncScope._moveNext (resume invokes with scopes only).
        public static object BindMoveNext(object target, object[] boundScopes, object?[] boundArgs)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            if (boundArgs == null) throw new ArgumentNullException(nameof(boundArgs));

            // Always return a Func<object[], object?> regardless of underlying delegate arity.
            // - Ignores the provided scopes at invocation time
            // - Uses the captured scopes + captured args
            return (Func<object[], object?>)(_ => InvokeWithArgs(target, boundScopes, boundArgs));
        }

        // Bind a function delegate (object-typed) to a fixed scopes array.
        // Returns a delegate of the same type as the input, but ignores the scopes argument
        // and uses the captured scopes array.
        public static object Bind(object target, object[] boundScopes)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));

            if (target is not Delegate del)
            {
                throw new ArgumentException("Expected a delegate for closure binding", nameof(target));
            }

            return CreateBoundDelegate(del, boundScopes, boundThis: null);
        }

        // Bind an arrow function delegate to a fixed scopes array AND a fixed lexical 'this'.
        // The runtime call sites may set a dynamic 'this' (receiver) before invocation; this binder
        // overrides it for the duration of the arrow function body to match ECMA-262 lexical semantics.
        public static object BindArrow(object target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));

            if (target is not Delegate del)
            {
                throw new ArgumentException("Expected a delegate for arrow closure binding", nameof(target));
            }

            return CreateBoundDelegate(del, boundScopes, boundThis);
        }

        // Invoke a function delegate with runtime type inspection to determine the correct arity.
        // This is used when calling a function stored in a variable where the parameter count isn't known at compile time.
        // args should NOT include the scopes array - this method will prepend it.
        public static object InvokeWithArgs(object target, object[] scopes, params object?[] args)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {

            // CommonJS require(...) is passed into scripts as a RequireDelegate, which does not include
            // the standard js2il scopes array parameter. Support calling it via the generic dispatcher.
            if (target is global::JavaScriptRuntime.CommonJS.RequireDelegate require)
            {
                return require(args.Length > 0 ? args[0] : null)!;
            }

            if (target is Delegate del)
            {
                // JavaScript semantics: missing args are 'undefined' (modeled as CLR null); extra args are ignored.
                return InvokeDelegateWithArgs(del, scopes, args);
            }

            // JavaScript/Node semantics: calling a non-callable throws a TypeError.
            // Use a stable, type-based message since we don't always have the identifier name available here.
            throw new TypeError($"Callee is not a function: it has type {TypeUtilities.Typeof(target)}.");
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        // Arity-specific overloads to avoid object[] allocations for common cases (0-3 args).
        // These forward to the main InvokeWithArgs method, but the compiler can emit direct calls
        // to avoid allocating the args array when the arity is known at compile time.

        public static object InvokeWithArgs0(object target, object[] scopes)
        {
            return InvokeWithArgs(target, scopes, System.Array.Empty<object>());
        }

        public static object InvokeWithArgs1(object target, object[] scopes, object? a0)
        {
            return InvokeWithArgs(target, scopes, new object?[] { a0 });
        }

        public static object InvokeWithArgs2(object target, object[] scopes, object? a0, object? a1)
        {
            return InvokeWithArgs(target, scopes, new object?[] { a0, a1 });
        }

        public static object InvokeWithArgs3(object target, object[] scopes, object? a0, object? a1, object? a2)
        {
            return InvokeWithArgs(target, scopes, new object?[] { a0, a1, a2 });
        }

        private static object? GetArg(object?[] args, int index)
        {
            return index < args.Length ? args[index] : null;
        }

        // Optimized direct-call path used by the compiler when the callee is known and needs
        // an implicit `arguments` object. This avoids the reflective dispatcher in InvokeWithArgs.
        public static object? InvokeDirectWithArgs(Func<object[], object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes);
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9), GetArg(args, 10));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9), GetArg(args, 10), GetArg(args, 11));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9), GetArg(args, 10), GetArg(args, 11), GetArg(args, 12));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9), GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?> target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes, GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4), GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9), GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc15 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc16 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc17 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc18 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc19 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc20 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc21 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc22 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc23 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc24 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc25 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc26 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc27 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc28 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26), GetArg(args, 27));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc29 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26), GetArg(args, 27), GetArg(args, 28));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc30 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26), GetArg(args, 27), GetArg(args, 28), GetArg(args, 29));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc31 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26), GetArg(args, 27), GetArg(args, 28), GetArg(args, 29),
                    GetArg(args, 30));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }

        public static object? InvokeDirectWithArgs(JsFunc32 target, object[] scopes, object?[] args)
        {
            var previousArgs = RuntimeServices.SetCurrentArguments(args);
            try
            {
                return target(scopes,
                    GetArg(args, 0), GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), GetArg(args, 4),
                    GetArg(args, 5), GetArg(args, 6), GetArg(args, 7), GetArg(args, 8), GetArg(args, 9),
                    GetArg(args, 10), GetArg(args, 11), GetArg(args, 12), GetArg(args, 13), GetArg(args, 14),
                    GetArg(args, 15), GetArg(args, 16), GetArg(args, 17), GetArg(args, 18), GetArg(args, 19),
                    GetArg(args, 20), GetArg(args, 21), GetArg(args, 22), GetArg(args, 23), GetArg(args, 24),
                    GetArg(args, 25), GetArg(args, 26), GetArg(args, 27), GetArg(args, 28), GetArg(args, 29),
                    GetArg(args, 30), GetArg(args, 31));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
        }
    }
}
