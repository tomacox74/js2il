namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Indicates where the scopes array comes from at runtime within a callable.
/// </summary>
public enum ScopesSource
{
    /// <summary>
    /// The callable does not access any parent scope variables.
    /// No scopes array is available or needed.
    /// </summary>
    None,

    /// <summary>
    /// The scopes array is passed as a method parameter (object[] scopes).
    /// Used by standalone functions and arrow functions.
    /// </summary>
    Argument,

    /// <summary>
    /// The scopes array is stored in this._scopes field.
    /// Used by class instance methods.
    /// </summary>
    ThisField
}

/// <summary>
/// Represents the ABI shape of a compiled callable method.
/// This defines how IL arguments map to JavaScript parameters and how scopes are accessed.
/// </summary>
/// <param name="IsInstanceMethod">Whether the generated method is instance (has 'this' as IL arg 0).</param>
/// <param name="HasScopesParam">Whether object[] scopes is present as a method parameter.</param>
/// <param name="ScopesSource">Where the callable reads its parent scopes from.</param>
/// <param name="JsParameterCount">Number of JavaScript parameters (excluding scopes array).</param>
/// <param name="MaxSupportedDelegateArity">Maximum supported delegate arity (currently 6).</param>
public sealed record CallableAbi(
    bool IsInstanceMethod,
    bool HasScopesParam,
    ScopesSource ScopesSource,
    int JsParameterCount,
    int MaxSupportedDelegateArity = 6
)
{
    /// <summary>
    /// Computes the IL argument index for a given JavaScript parameter index.
    /// </summary>
    /// <param name="jsParamIndex">0-based JavaScript parameter index.</param>
    /// <returns>The corresponding IL argument index.</returns>
    public int JsParamToIlArgIndex(int jsParamIndex)
    {
        // IL arg 0 is 'this' for instance methods
        // If HasScopesParam, scopes is the first non-this arg
        return (IsInstanceMethod ? 1 : 0) + (HasScopesParam ? 1 : 0) + jsParamIndex;
    }

    /// <summary>
    /// Gets the IL argument index for the scopes array parameter.
    /// Only valid when HasScopesParam is true.
    /// </summary>
    public int ScopesArgIndex => IsInstanceMethod ? 1 : 0;

    /// <summary>
    /// Creates a CallableAbi for a user-defined function or arrow function.
    /// Static method with scopes as first parameter.
    /// </summary>
    public static CallableAbi ForFunction(int jsParameterCount, bool needsParentScopes)
    {
        return new CallableAbi(
            IsInstanceMethod: false,
            HasScopesParam: needsParentScopes,
            ScopesSource: needsParentScopes ? ScopesSource.Argument : ScopesSource.None,
            JsParameterCount: jsParameterCount
        );
    }

    /// <summary>
    /// Creates a CallableAbi for a class constructor.
    /// Instance method that may have scopes as first parameter.
    /// </summary>
    public static CallableAbi ForConstructor(int jsParameterCount, bool needsParentScopes)
    {
        return new CallableAbi(
            IsInstanceMethod: true,
            HasScopesParam: needsParentScopes,
            ScopesSource: needsParentScopes ? ScopesSource.Argument : ScopesSource.None,
            JsParameterCount: jsParameterCount
        );
    }

    /// <summary>
    /// Creates a CallableAbi for a class instance method.
    /// Instance method that reads scopes from this._scopes field (no scopes parameter).
    /// </summary>
    public static CallableAbi ForClassMethod(int jsParameterCount, bool needsParentScopes)
    {
        return new CallableAbi(
            IsInstanceMethod: true,
            HasScopesParam: false,
            ScopesSource: needsParentScopes ? ScopesSource.ThisField : ScopesSource.None,
            JsParameterCount: jsParameterCount
        );
    }

    /// <summary>
    /// Creates a CallableAbi for a module Main method.
    /// Static method with no scopes parameter.
    /// </summary>
    public static CallableAbi ForModuleMain(int jsParameterCount)
    {
        return new CallableAbi(
            IsInstanceMethod: false,
            HasScopesParam: false,
            ScopesSource: ScopesSource.None,
            JsParameterCount: jsParameterCount
        );
    }
}
