using System;
using System.Reflection;

namespace JavaScriptRuntime
{
    public static class Closure
    {
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

        // Bind a function delegate (object-typed) to a fixed scopes array AND a fixed set of JS arguments.
        // Returns a Func<object[], object> suitable for AsyncScope._moveNext (resume invokes with scopes only).
        public static object BindMoveNext(object target, object[] boundScopes, object?[] boundArgs)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            if (boundArgs == null) throw new ArgumentNullException(nameof(boundArgs));

            if (target is Func<object[], object> f0)
            {
                return (Func<object[], object>)(_ => f0(boundScopes));
            }

            var a1 = boundArgs.Length > 0 ? boundArgs[0] : null;
            var a2 = boundArgs.Length > 1 ? boundArgs[1] : null;
            var a3 = boundArgs.Length > 2 ? boundArgs[2] : null;
            var a4 = boundArgs.Length > 3 ? boundArgs[3] : null;
            var a5 = boundArgs.Length > 4 ? boundArgs[4] : null;
            var a6 = boundArgs.Length > 5 ? boundArgs[5] : null;

            if (target is Func<object[], object?, object> f1)
            {
                return (Func<object[], object>)(_ => f1(boundScopes, a1));
            }
            if (target is Func<object[], object?, object?, object> f2)
            {
                return (Func<object[], object>)(_ => f2(boundScopes, a1, a2));
            }
            if (target is Func<object[], object?, object?, object?, object> f3)
            {
                return (Func<object[], object>)(_ => f3(boundScopes, a1, a2, a3));
            }
            if (target is Func<object[], object?, object?, object?, object?, object> f4)
            {
                return (Func<object[], object>)(_ => f4(boundScopes, a1, a2, a3, a4));
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object> f5)
            {
                return (Func<object[], object>)(_ => f5(boundScopes, a1, a2, a3, a4, a5));
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object?, object> f6)
            {
                return (Func<object[], object>)(_ => f6(boundScopes, a1, a2, a3, a4, a5, a6));
            }

            throw new ArgumentException("Unsupported delegate type for MoveNext binding", nameof(target));
        }

        // Bind a function delegate (object-typed) to a fixed scopes array.
        // Supports Func<object[], object> and Func<object[], object, object>.
        public static object Bind(object target, object[] boundScopes)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            if (target is Func<object[], object> f0)
            {
                return (Func<object[], object>)(_ => f0(boundScopes));
            }
            if (target is Func<object[], object, object> f1)
            {
                return (Func<object[], object, object>)((_, a1) => f1(boundScopes, a1));
            }
            if (target is Func<object[], object, object, object> f2)
            {
                return (Func<object[], object, object, object>)((_, a1, a2) => f2(boundScopes, a1, a2));
            }
            if (target is Func<object[], object, object, object, object> f3)
            {
                return (Func<object[], object, object, object, object>)((_, a1, a2, a3) => f3(boundScopes, a1, a2, a3));
            }
            if (target is Func<object[], object, object, object, object, object> f4)
            {
                return (Func<object[], object, object, object, object, object>)((_, a1, a2, a3, a4) => f4(boundScopes, a1, a2, a3, a4));
            }
            if (target is Func<object[], object, object, object, object, object, object> f5)
            {
                return (Func<object[], object, object, object, object, object, object>)((_, a1, a2, a3, a4, a5) => f5(boundScopes, a1, a2, a3, a4, a5));
            }
            if (target is Func<object[], object, object, object, object, object, object, object> f6)
            {
                return (Func<object[], object, object, object, object, object, object, object>)((_, a1, a2, a3, a4, a5, a6) => f6(boundScopes, a1, a2, a3, a4, a5, a6));
            }
            throw new ArgumentException("Unsupported delegate type for closure binding", nameof(target));
        }

        // Bind an arrow function delegate to a fixed scopes array AND a fixed lexical 'this'.
        // The runtime call sites may set a dynamic 'this' (receiver) before invocation; this binder
        // overrides it for the duration of the arrow function body to match ECMA-262 lexical semantics.
        public static object BindArrow(object target, object[] boundScopes, object? boundThis)
        {
            if (target is Func<object[], object> f0)
            {
                return BindArrow0(f0, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object> f1)
            {
                return BindArrow1(f1, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object?, object> f2)
            {
                return BindArrow2(f2, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object?, object?, object> f3)
            {
                return BindArrow3(f3, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object?, object?, object?, object> f4)
            {
                return BindArrow4(f4, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object> f5)
            {
                return BindArrow5(f5, boundScopes, boundThis);
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object?, object> f6)
            {
                return BindArrow6(f6, boundScopes, boundThis);
            }

            throw new ArgumentException("Unsupported delegate type for arrow closure binding", nameof(target));
        }

        public static Func<object[], object> BindArrow0(Func<object[], object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object>)(_ => InvokeWithThis(boundThis, () => target(boundScopes)));
        }

        public static Func<object[], object?, object> BindArrow1(Func<object[], object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object>)((_, a1) => InvokeWithThis(boundThis, () => target(boundScopes, a1)));
        }

        public static Func<object[], object?, object?, object> BindArrow2(Func<object[], object?, object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object?, object>)((_, a1, a2) => InvokeWithThis(boundThis, () => target(boundScopes, a1, a2)));
        }

        public static Func<object[], object?, object?, object?, object> BindArrow3(Func<object[], object?, object?, object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object?, object?, object>)((_, a1, a2, a3) => InvokeWithThis(boundThis, () => target(boundScopes, a1, a2, a3)));
        }

        public static Func<object[], object?, object?, object?, object?, object> BindArrow4(Func<object[], object?, object?, object?, object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object?, object?, object?, object>)((_, a1, a2, a3, a4) => InvokeWithThis(boundThis, () => target(boundScopes, a1, a2, a3, a4)));
        }

        public static Func<object[], object?, object?, object?, object?, object?, object> BindArrow5(Func<object[], object?, object?, object?, object?, object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object?, object?, object?, object?, object>)((_, a1, a2, a3, a4, a5) => InvokeWithThis(boundThis, () => target(boundScopes, a1, a2, a3, a4, a5)));
        }

        public static Func<object[], object?, object?, object?, object?, object?, object?, object> BindArrow6(Func<object[], object?, object?, object?, object?, object?, object?, object> target, object[] boundScopes, object? boundThis)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (Func<object[], object?, object?, object?, object?, object?, object?, object>)((_, a1, a2, a3, a4, a5, a6) => InvokeWithThis(boundThis, () => target(boundScopes, a1, a2, a3, a4, a5, a6)));
        }

        // Bind a zero-parameter JS function (aside from the scopes array) to a fixed scopes array
        public static Func<object[], object> Bind(Func<object[], object> target, object[] boundScopes)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return _ => target(boundScopes);
        }

        // Bind a one-parameter JS function (besides scopes) to a fixed scopes array; pass along the remaining arg
        public static Func<object[], object, object> Bind(Func<object[], object, object> target, object[] boundScopes)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (boundScopes == null) throw new ArgumentNullException(nameof(boundScopes));
            return (ignoredScopes, a1) => target(boundScopes, a1);
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

            return paramCount switch
            {
                0 => (object)Delegate.CreateDelegate(typeof(Func<object[], object>), null, mi),
                1 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object>), null, mi),
                2 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object, object>), null, mi),
                3 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object, object, object>), null, mi),
                4 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object, object, object, object>), null, mi),
                5 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object, object, object, object, object>), null, mi),
                6 => (object)Delegate.CreateDelegate(typeof(Func<object[], object, object, object, object, object, object, object>), null, mi),
                _ => throw new NotSupportedException($"Unsupported parameter count {paramCount} for self delegate")
            };
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

            // JavaScript semantics: missing args are 'undefined' (modeled as CLR null); extra args are ignored.
            // Dispatch on delegate type (arity) rather than args.Length.
            if (target is Func<object[], bool> b0)
            {
                return b0(scopes);
            }
            if (target is Func<object[], object?, bool> b1)
            {
                return b1(scopes, args.Length > 0 ? args[0] : null);
            }
            if (target is Func<object[], object?, object?, bool> b2)
            {
                return b2(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null);
            }
            if (target is Func<object[], object?, object?, object?, bool> b3)
            {
                return b3(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null);
            }
            if (target is Func<object[], object?, object?, object?, object?, bool> b4)
            {
                return b4(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null,
                    args.Length > 3 ? args[3] : null);
            }

            if (target is Func<object[], object> f0)
            {
                return f0(scopes);
            }
            if (target is Func<object[], object?, object> f1)
            {
                return f1(scopes, args.Length > 0 ? args[0] : null);
            }
            if (target is Func<object[], object?, object?, object> f2)
            {
                return f2(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null);
            }
            if (target is Func<object[], object?, object?, object?, object> f3)
            {
                return f3(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null);
            }
            if (target is Func<object[], object?, object?, object?, object?, object> f4)
            {
                return f4(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null,
                    args.Length > 3 ? args[3] : null);
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object> f5)
            {
                return f5(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null,
                    args.Length > 3 ? args[3] : null,
                    args.Length > 4 ? args[4] : null);
            }
            if (target is Func<object[], object?, object?, object?, object?, object?, object?, object> f6)
            {
                return f6(scopes,
                    args.Length > 0 ? args[0] : null,
                    args.Length > 1 ? args[1] : null,
                    args.Length > 2 ? args[2] : null,
                    args.Length > 3 ? args[3] : null,
                    args.Length > 4 ? args[4] : null,
                    args.Length > 5 ? args[5] : null);
            }

            throw new ArgumentException($"Unsupported delegate type for function call: target type = {target.GetType()}, args length = {args.Length}", nameof(target));
        }
    }
}
