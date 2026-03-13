namespace Js2IL.Runtime;

/// <summary>
/// Represents an exported JavaScript constructor (class/function) that can be invoked with <c>new</c> semantics.
/// </summary>
public interface IJsConstructor<out TInstance> : IJsHandle
    where TInstance : class
{
    TInstance Construct(params object?[] args);
}
