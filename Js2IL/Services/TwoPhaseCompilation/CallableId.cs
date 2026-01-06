using Acornima.Ast;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// The kind of callable construct in JavaScript.
/// </summary>
public enum CallableKind
{
    /// <summary>A named function declaration: function foo() {}</summary>
    FunctionDeclaration,
    
    /// <summary>A function expression: const f = function() {} or (function() {})</summary>
    FunctionExpression,
    
    /// <summary>An arrow function: () => {} or x => x</summary>
    Arrow,
    
    /// <summary>A class constructor: class C { constructor() {} }</summary>
    ClassConstructor,
    
    /// <summary>A class instance method: class C { m() {} }</summary>
    ClassMethod,
    
    /// <summary>A class static method: class C { static m() {} }</summary>
    ClassStaticMethod
}

/// <summary>
/// Source location for stable identification of anonymous callables.
/// </summary>
/// <param name="Line">1-based line number</param>
/// <param name="Column">1-based column number</param>
public readonly record struct SourceLocation(int Line, int Column)
{
    public override string ToString() => $"L{Line}C{Column}";
    
    /// <summary>
    /// Creates a SourceLocation from an AST node's position.
    /// </summary>
    public static SourceLocation FromNode(Node node)
    {
        var loc = node.Location;
        return new SourceLocation(loc.Start.Line, loc.Start.Column + 1); // Acornima columns are 0-based
    }
}

/// <summary>
/// A stable identifier for a callable, used everywhere in the two-phase compilation pipeline.
/// This identifier uniquely identifies any callable (function, arrow, class constructor/method)
/// and can be derived deterministically from the AST and symbol table.
/// </summary>
/// <remarks>
/// The CallableId is not a runtime feature; it is strictly a compilation/planning key.
/// It enables:
/// - Stable graph nodes for dependency analysis
/// - Cache keys for callable metadata
/// - Diagnostics that identify specific callables
/// </remarks>
public sealed record CallableId
{
    /// <summary>The kind of callable (function declaration, arrow, class method, etc.)</summary>
    public required CallableKind Kind { get; init; }
    
    /// <summary>
    /// The module-qualified scope name where this callable is declared.
    /// For top-level functions: "moduleName"
    /// For nested functions: "moduleName/parentFunction"
    /// For class methods: "moduleName" or "moduleName/enclosingFunction"
    /// </summary>
    public required string DeclaringScopeName { get; init; }
    
    /// <summary>
    /// The name of the callable (for named constructs).
    /// For function declarations: the function name
    /// For class constructors: the class name
    /// For class methods: "ClassName.methodName"
    /// For anonymous function expressions/arrows: null
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Source location for stable identification of anonymous callables.
    /// Used when Name is null (function expressions, arrows without assignment target).
    /// </summary>
    public SourceLocation? Location { get; init; }
    
    /// <summary>
    /// The JavaScript parameter count (excluding implicit scopes parameter).
    /// Used for delegate type selection at call sites.
    /// </summary>
    public int JsParamCount { get; init; }
    
    /// <summary>
    /// Optional reference to the original AST node for diagnostics.
    /// Not used for equality comparison.
    /// </summary>
    public Node? AstNode { get; init; }

    /// <summary>
    /// Gets a display-friendly string for this callable (for diagnostics/logging).
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return $"{Kind}:{Name}";
            }
            if (Location.HasValue)
            {
                return $"{Kind}@{Location}";
            }
            return $"{Kind}:anonymous";
        }
    }

    /// <summary>
    /// Gets a unique key string for this callable (for dictionary lookups).
    /// </summary>
    public string UniqueKey
    {
        get
        {
            var nameOrLoc = !string.IsNullOrEmpty(Name) ? Name : Location?.ToString() ?? "anonymous";
            return $"{DeclaringScopeName}/{Kind}:{nameOrLoc}";
        }
    }

    public override string ToString() => DisplayName;

    /// <summary>
    /// Custom equality that excludes AstNode (reference comparison would break caching).
    /// Only compares Kind, DeclaringScopeName, Name, Location, and JsParamCount.
    /// </summary>
    public bool Equals(CallableId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Kind == other.Kind &&
               DeclaringScopeName == other.DeclaringScopeName &&
               Name == other.Name &&
               Location == other.Location &&
               JsParamCount == other.JsParamCount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, DeclaringScopeName, Name, Location, JsParamCount);
    }
}
