using System.Dynamic;

namespace JavaScriptRuntime;

/// <summary>
/// Representation-specific backing-store operations for ordinary JavaScript objects.
/// <see cref="JsObject"/> is the primary representation; <see cref="ExpandoObject"/>
/// remains only as a transitional compatibility path while receiver migrations continue.
/// Descriptor, prototype, proxy, and integrity semantics stay in their existing runtime layers.
/// </summary>
internal static class OrdinaryObjectOperations
{
    internal static bool IsOrdinaryObject(object target)
        => target is JsObject or ExpandoObject;

    internal static bool TryGetOwnValue(object target, string key, out object? value)
    {
        if (target is JsObject jsObject)
        {
            return jsObject.TryGetBoxedValue(key, out value);
        }

        if (target is ExpandoObject expando)
        {
            return ((IDictionary<string, object?>)expando).TryGetValue(key, out value);
        }

        value = null;
        return false;
    }

    internal static bool HasOwnValue(object target, string key)
    {
        if (target is JsObject jsObject)
        {
            return jsObject.ContainsKey(key);
        }

        return target is ExpandoObject expando
            && ((IDictionary<string, object?>)expando).ContainsKey(key);
    }

    internal static bool TrySetOwnValue(object target, string key, object? value)
    {
        if (target is JsObject jsObject)
        {
            jsObject.SetBoxedValue(key, value);
            return true;
        }

        if (target is ExpandoObject expando)
        {
            ((IDictionary<string, object?>)expando)[key] = value;
            return true;
        }

        return false;
    }

    internal static bool TryDeleteOwnValue(object target, string key)
    {
        if (target is JsObject jsObject)
        {
            jsObject.Remove(key);
            return true;
        }

        if (target is ExpandoObject expando)
        {
            ((IDictionary<string, object?>)expando).Remove(key);
            return true;
        }

        return false;
    }

    internal static IEnumerable<string> GetOwnKeys(object target)
    {
        if (target is JsObject jsObject)
        {
            return jsObject.GetOwnPropertyNames();
        }

        if (target is ExpandoObject expando)
        {
            return ((IDictionary<string, object?>)expando).Keys;
        }

        return System.Array.Empty<string>();
    }
}
