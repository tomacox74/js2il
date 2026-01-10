using System;
using System.Linq;
using System.Reflection;
using Js2IL.IR;

namespace Js2IL.IL;

internal static class IntrinsicInlining
{
    /// <summary>
    /// Returns true when the intrinsic static call is safe for Stackify to inline/re-emit.
    /// This feature is intentionally limited to zero-argument methods.
    /// </summary>
    public static bool IsStackifyInlineable(LIRCallIntrinsicStatic instruction)
    {
        if (instruction.Arguments.Count != 0)
        {
            return false;
        }

        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            return false;
        }

        // Limit to true zero-parameter methods (no params object[] fallback).
        var chosen = ResolveZeroArgMethod(intrinsicType, instruction.MethodName);
        if (chosen == null)
        {
            return false;
        }

        return chosen.IsDefined(typeof(JavaScriptRuntime.StackifyInlineAttribute), inherit: false);
    }

    private static MethodInfo? ResolveZeroArgMethod(Type intrinsicType, string methodName)
    {
        var allMethods = intrinsicType.GetMethods(BindingFlags.Public | BindingFlags.Static);
        return allMethods
            .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault(mi => mi.GetParameters().Length == 0);
    }
}
