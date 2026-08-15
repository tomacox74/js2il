using System;

namespace JavaScriptRuntime;

/// <summary>
/// Storage for ECMAScript-like [[Prototype]] chains.
/// </summary>
/// <remarks>
/// <para>
/// Prototype-chain support used to be gated behind a process-wide <c>_enabled</c>
/// switch that the compiler flipped on via <see cref="Enable"/> only when it detected
/// prototype-related usage in the compiled program. That switch was process-global:
/// once any realm's intrinsic bootstrap (which always wires prototype relationships
/// such as <c>Function.prototype -&gt; Object.prototype</c>) flipped it on, every other
/// realm/program sharing the process silently paid for (and observed, e.g. via
/// <c>__proto__</c> semantics in <c>ObjectRuntime.Operations</c>) full prototype-chain
/// behavior even if its own compiled code never opted in. Prototype-chain semantics are
/// now always active; <see cref="Enable"/>/<see cref="Enabled"/> remain as no-op/true
/// compatibility shims for existing call sites and compiler-emitted prologue calls.
/// </para>
/// <para>
/// <see cref="JsObject"/> stores its [[Prototype]] inline, so it is realm-correct by
/// construction (every intrinsic <see cref="JsObject"/> is realm-owned; see
/// <see cref="RuntimeIntrinsics"/>). Everything else — <c>Map</c>, <c>Set</c>,
/// <c>RegExp</c>, <c>Date</c>, <c>Promise</c> instances and the <see cref="Type"/>
/// objects used as constructor identities — uses the realm's fallback slot table
/// (<see cref="RuntimeIntrinsics.PrototypeSlots"/>). That table has to be realm-owned:
/// a shared <c>Date</c> constructor identity whose [[Prototype]] link lived in a
/// process-wide table would otherwise resolve to whichever realm bootstrapped last.
/// </para>
/// </remarks>
public static class PrototypeChain
{
    /// <summary>Always <see langword="true"/>; retained as a compatibility shim (see remarks).</summary>
    public static bool Enabled => true;

    /// <summary>No-op; retained as a compatibility shim (see remarks) for existing call sites
    /// and compiler-emitted prologue calls.</summary>
    public static void Enable()
    {
    }

    public static bool TryGetPrototype(object obj, out object? prototype)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        obj = BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(obj);

        if (obj is JsObject jsObject)
        {
            return jsObject.TryGetInlinePrototype(out prototype);
        }

        if (RuntimeIntrinsics.Current.PrototypeSlots.TryGetValue(obj, out var slot))
        {
            prototype = slot.Prototype;
            return true;
        }

        prototype = null;
        return false;
    }

    public static object? GetPrototypeOrNull(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        obj = BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(obj);

        if (obj is JsObject jsObject)
        {
            return jsObject.TryGetInlinePrototype(out var prototype)
                ? prototype
                : null;
        }

        return RuntimeIntrinsics.Current.PrototypeSlots.TryGetValue(obj, out var slot)
            ? slot.Prototype
            : null;
    }

    public static void SetPrototype(object obj, object? prototype)
    {
        SetPrototypeCore(obj, prototype);
        if (obj is Array array)
        {
            array.DisableDenseGrowthFastPath();
        }
        Array.NotifyPrototypeMutation();
    }

    internal static void InitializePrototype(object obj, object? prototype)
        => SetPrototypeCore(obj, prototype);

    private static void SetPrototypeCore(object obj, object? prototype)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        obj = BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(obj);
        prototype =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(
                prototype);

        if (obj is JsObject jsObject)
        {
            jsObject.SetInlinePrototype(prototype);
            return;
        }

        var slot = RuntimeIntrinsics.Current.PrototypeSlots.GetOrCreateValue(obj);
        slot.Prototype = prototype;
    }
}
