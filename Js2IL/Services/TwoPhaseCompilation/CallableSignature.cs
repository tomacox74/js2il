using System.Reflection.Metadata;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Specifies the invoke shape (delegate type) for a callable.
/// This determines which System.Func or System.Action type is used for delegate creation.
/// </summary>
public enum CallableInvokeShape
{
    /// <summary>Func&lt;object?, object?&gt; - 0 JS parameters</summary>
    Func0,
    /// <summary>Func&lt;object?, object?, object?&gt; - 1 JS parameter</summary>
    Func1,
    /// <summary>Func&lt;object?, object?, object?, object?&gt; - 2 JS parameters</summary>
    Func2,
    /// <summary>Func&lt;object?, object?, object?, object?, object?&gt; - 3 JS parameters</summary>
    Func3,
    /// <summary>Func&lt;object?, object?, object?, object?, object?, object?&gt; - 4 JS parameters</summary>
    Func4,
    /// <summary>Variable-args - uses object[] for parameters beyond 4</summary>
    FuncVarArgs
}

/// <summary>
/// The metadata needed to declare a callable and create an IL call target.
/// This is populated during Phase 1 (discovery/declaration) and consumed during Phase 2 (body compilation).
/// </summary>
/// <remarks>
/// CallableSignature contains enough information to:
/// 1. Emit delegate creation IL (ldnull, ldftn, newobj)
/// 2. Emit direct call IL
/// 3. Determine if a scopes parameter is required
/// </remarks>
public sealed record CallableSignature
{
    /// <summary>
    /// The .NET type that owns this callable method.
    /// For function declarations: Modules.&lt;ModuleName&gt;.&lt;FunctionName&gt;
    /// For class methods: Classes.&lt;ModuleName&gt;.&lt;ClassName&gt;
    /// For arrows/function expressions: the enclosing owner type
    /// </summary>
    public required TypeDefinitionHandle OwnerTypeHandle { get; init; }
    
    /// <summary>
    /// Whether the callable requires a scopes parameter (object[] scopes).
    /// True when the callable references variables from an enclosing scope.
    /// </summary>
    public bool RequiresScopesParameter { get; init; }
    
    /// <summary>
    /// The number of JavaScript parameters (excluding the scopes parameter).
    /// </summary>
    public int JsParamCount { get; init; }
    
    /// <summary>
    /// The invoke shape (delegate type) for this callable.
    /// Determined by JsParamCount.
    /// </summary>
    public CallableInvokeShape InvokeShape { get; init; }
    
    /// <summary>
    /// Whether this is an instance method (vs static).
    /// Class instance methods are instance; most other callables are static.
    /// </summary>
    public bool IsInstanceMethod { get; init; }
    
    /// <summary>
    /// The IL method name for this callable.
    /// For function declarations: __js_call__
    /// For arrows/function expressions: __js_call__
    /// For class constructors: .ctor
    /// For class methods: the method name
    /// </summary>
    public required string ILMethodName { get; init; }
    
    /// <summary>
    /// Optional cached method signature blob handle (for performance).
    /// Can be lazily populated during Phase 1.
    /// </summary>
    public BlobHandle? SignatureBlob { get; init; }

    /// <summary>
    /// Computes the invoke shape from the JS parameter count.
    /// </summary>
    public static CallableInvokeShape GetInvokeShape(int jsParamCount)
    {
        return jsParamCount switch
        {
            0 => CallableInvokeShape.Func0,
            1 => CallableInvokeShape.Func1,
            2 => CallableInvokeShape.Func2,
            3 => CallableInvokeShape.Func3,
            4 => CallableInvokeShape.Func4,
            _ => CallableInvokeShape.FuncVarArgs
        };
    }
}
