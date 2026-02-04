using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

            bool hasParamsArray = expectedJsParamCount > 0
                && Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute));
            int fixedJsParamCount = hasParamsArray ? expectedJsParamCount - 1 : expectedJsParamCount;

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
                finalArgs[finalIndex++] = i < args.Length ? args[i] : null;
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
                finalArgs[finalIndex++] = packed;
            }

            try
            {
                // Delegate.DynamicInvoke returns boxed value types; null for void.
                return target.DynamicInvoke(finalArgs)!;
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
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

            throw new ArgumentException(
                $"Unsupported callable type for function call: target type = {target.GetType()}, args length = {args.Length}",
                nameof(target));
            }
            finally
            {
                RuntimeServices.SetCurrentArguments(previousArgs);
            }
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
