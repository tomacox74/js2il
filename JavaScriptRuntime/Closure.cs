using System;

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
    }
}
