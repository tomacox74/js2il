using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Function.prototype helpers for delegate-backed function values.
    ///
    /// JS2IL models JavaScript functions as CLR delegates; these helpers provide a small subset
    /// of Function.prototype behavior (apply/bind) for those values.
    /// </summary>
    public static class Function
    {
        internal static readonly ExpandoObject Prototype = CreatePrototype();

        private static ExpandoObject CreatePrototype()
        {
            var exp = new ExpandoObject();
            var dict = (IDictionary<string, object?>)exp;
            dict["apply"] = (Func<object[], object?[], object?>)PrototypeApply;
            dict["call"] = (Func<object[], object?[], object?>)PrototypeCall;
            return exp;
        }

        private static object? PrototypeApply(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (target is not Delegate del)
            {
                throw new TypeError("Function.prototype.apply called on non-function");
            }

            var thisArg = args != null && args.Length > 0 ? args[0] : null;
            var argArray = args != null && args.Length > 1 ? args[1] : null;
            return Apply(del, thisArg, argArray);
        }

        private static object? PrototypeCall(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (target is not Delegate del)
            {
                throw new TypeError("Function.prototype.call called on non-function");
            }

            var thisArg = args != null && args.Length > 0 ? args[0] : null;
            var callArgs = args != null && args.Length > 1
                ? args.Skip(1).ToArray()
                : System.Array.Empty<object?>();

            return Call(del, thisArg, callArgs);
        }

        public static object? Apply(Delegate target, object? thisArg, object? argArray)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            object?[] argsList;
            if (argArray is null || argArray is JsNull)
            {
                argsList = System.Array.Empty<object?>();
            }
            else if (argArray is JavaScriptRuntime.Array jsArr)
            {
                argsList = jsArr.ToArray();
            }
            else if (argArray is object?[] objArr)
            {
                argsList = objArr;
            }
            else if (argArray is IEnumerable enumerable && argArray is not string)
            {
                var list = new List<object?>();
                foreach (var item in enumerable)
                {
                    list.Add(item);
                }
                argsList = list.ToArray();
            }
            else
            {
                throw new TypeError("apply arguments must be an array (or null/undefined)");
            }

            var prevThis = RuntimeServices.SetCurrentThis(thisArg);
            try
            {
                return Closure.InvokeWithArgs(target, System.Array.Empty<object>(), argsList);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(prevThis);
            }
        }

        public static object? Call(Delegate target, object? thisArg, object?[] args)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            args ??= System.Array.Empty<object?>();

            var prevThis = RuntimeServices.SetCurrentThis(thisArg);
            try
            {
                return Closure.InvokeWithArgs(target, System.Array.Empty<object>(), args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(prevThis);
            }
        }

        public static Func<object[], object?[], object?> Bind(Delegate target, object? thisArg, object?[] boundArgs)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            boundArgs ??= System.Array.Empty<object?>();

            // Return a callable delegate that supports arbitrary JS arg counts.
            // Signature uses a trailing object[] parameter which Closure.InvokeWithArgs treats
            // as a params-array.
            return (Func<object[], object?[], object?>)((scopes, runtimeArgs) =>
            {
                runtimeArgs ??= System.Array.Empty<object?>();
                var finalArgs = boundArgs.Length == 0
                    ? runtimeArgs
                    : boundArgs.Concat(runtimeArgs).ToArray();

                var prevThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    return Closure.InvokeWithArgs(target, scopes ?? System.Array.Empty<object>(), finalArgs);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(prevThis);
                }
            });
        }

        public static object? Construct(Delegate constructor, object[]? args)
        {
            if (constructor is null) throw new ArgumentNullException(nameof(constructor));

            // JS `new` semantics for function constructors:
            // 1) Create a new instance object
            // 2) Set its [[Prototype]] to ctor.prototype when available
            // 3) Invoke the constructor with `this` bound to the instance
            // 4) If ctor returns an object, use that; otherwise return the instance

            var callArgs = args ?? System.Array.Empty<object>();
            var instance = new System.Dynamic.ExpandoObject();

            // Default proto: ctor.prototype when it is an object or null; otherwise undefined.
            // If undefined/non-object, we intentionally skip prototype assignment (minimal behavior).
            var proto = JavaScriptRuntime.Object.GetItem(constructor, "prototype");
            if (proto is JsNull || TypeUtilities.IsConstructorReturnOverride(proto))
            {
                PrototypeChain.SetPrototype(instance, proto);
            }

            var previousThis = RuntimeServices.SetCurrentThis(instance);
            try
            {
                var result = Closure.InvokeWithArgsWithNewTarget(constructor, System.Array.Empty<object>(), constructor, callArgs);
                return TypeUtilities.IsConstructorReturnOverride(result) ? result : instance;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }
    }
}
