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

            for (int i = 0; i < expectedJsParamCount; i++)
            {
                finalArgs[finalIndex++] = i < args.Length ? args[i] : null;
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

        private static Type GetFunctionDelegateType(int jsParamCount)
        {
            return jsParamCount switch
            {
                0 => typeof(Func<object[], object?>),
                1 => typeof(Func<object[], object?, object?>),
                2 => typeof(Func<object[], object?, object?, object?>),
                3 => typeof(Func<object[], object?, object?, object?, object?>),
                4 => typeof(Func<object[], object?, object?, object?, object?, object?>),
                5 => typeof(Func<object[], object?, object?, object?, object?, object?, object?>),
                6 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?>),
                7 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?>),
                8 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                9 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                10 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                11 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                12 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                13 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                14 => typeof(Func<object[], object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>),
                15 => typeof(JsFunc15),
                16 => typeof(JsFunc16),
                17 => typeof(JsFunc17),
                18 => typeof(JsFunc18),
                19 => typeof(JsFunc19),
                20 => typeof(JsFunc20),
                21 => typeof(JsFunc21),
                22 => typeof(JsFunc22),
                23 => typeof(JsFunc23),
                24 => typeof(JsFunc24),
                25 => typeof(JsFunc25),
                26 => typeof(JsFunc26),
                27 => typeof(JsFunc27),
                28 => typeof(JsFunc28),
                29 => typeof(JsFunc29),
                30 => typeof(JsFunc30),
                31 => typeof(JsFunc31),
                32 => typeof(JsFunc32),
                _ => throw new NotSupportedException($"Unsupported parameter count {jsParamCount} (max supported is 32)")
            };
        }

        // Creates a delegate instance pointing to the provided method with the standard js2il
        // signature: Func<object[], [object x N], object>. The number of additional object
        // parameters is specified by paramCount.
        public static object CreateSelfDelegate(MethodBase method, int paramCount)
        {
            if (method is not MethodInfo mi)
            {
                throw new ArgumentException("Expected MethodInfo", nameof(method));
            }

            var delegateType = GetFunctionDelegateType(paramCount);
            return Delegate.CreateDelegate(delegateType, null, mi);
        }

        // Invoke a function delegate with runtime type inspection to determine the correct arity.
        // This is used when calling a function stored in a variable where the parameter count isn't known at compile time.
        // args should NOT include the scopes array - this method will prepend it.
        public static object InvokeWithArgs(object target, object[] scopes, params object?[] args)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));

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
    }
}
