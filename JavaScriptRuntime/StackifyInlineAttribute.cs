using System;

namespace JavaScriptRuntime;

/// <summary>
/// Marks a static runtime method (including property getters) as safe for Stackify to inline/re-emit.
/// Intended only for pure, side-effect-free methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class StackifyInlineAttribute : Attribute
{
}
