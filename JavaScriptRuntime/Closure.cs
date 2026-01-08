using System;
using System.Reflection;

namespace JavaScriptRuntime
{
    public static class Closure
    {
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

            // JavaScript semantics: missing args are 'undefined' (modeled as CLR null); extra args are ignored.
            // Dispatch on delegate type (arity) rather than args.Length.
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
