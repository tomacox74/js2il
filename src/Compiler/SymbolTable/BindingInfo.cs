using Acornima.Ast;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;

namespace Jroc.SymbolTables;

public enum BindingKind
{
    Var,
    Let,
    Const,
    Function,
    Global
}

public class BindingInfo
{
    private Type? _clrType;
    private bool _isStableType;
    private Type? _stableElementClrType;

    public string Name { get; }
    public BindingKind Kind { get; }
    public Scope DeclaringScope { get; }
    public Node DeclarationNode { get; }
    // Optional: CLR runtime type known via static analysis (e.g., const fs = require('fs'))
    public Type? ClrType
    {
        get => _clrType;
        set
        {
            _clrType = value;
            if (_clrType != typeof(JavaScriptRuntime.Array))
            {
                _stableElementClrType = null;
            }
        }
    }
    
    /// <summary>
    /// Indicates whether this variable is captured (referenced) by any child scope.
    /// When false, the variable can be optimized to use local variables instead of fields.
    /// </summary>
    public bool IsCaptured { get; set; }

    /// <summary>
    /// Indicates whether the variable's type has been inferred during static analysis
    /// and is known to never change during the variable's lifetime.
    /// When true, any attempt to change ClrType is a bug.
    /// </summary>
    public bool IsStableType
    {
        get => _isStableType;
        set
        {
            _isStableType = value;
            if (!_isStableType)
            {
                _stableElementClrType = null;
            }
        }
    }

    /// <summary>
    /// For stable JavaScriptRuntime.Array bindings, indicates a conservative stable CLR type
    /// for numeric element values (e.g., string when all observed indexed writes are strings).
    /// Null means unknown or unstable element type.
    /// </summary>
    public Type? StableElementClrType
    {
        get => _stableElementClrType;
        set
        {
            if (value != null && (!IsStableType || ClrType != typeof(JavaScriptRuntime.Array)))
            {
                _stableElementClrType = null;
                return;
            }

            _stableElementClrType = value;
        }
    }

    /// <summary>
    /// True when this binding is the target of any write operation (assignment/update/initializer).
    /// This is used for conservative optimizations that require proving a binding is never reassigned.
    /// </summary>
    public bool HasWrite { get; set; }

    /// <summary>
    /// True when source code explicitly assigns to or updates this binding after declaration.
    /// </summary>
    public bool HasNonInitializationWrite { get; set; }

    /// <summary>
    /// True when whole-program analysis proves that this binding is a direct import of a
    /// generated Node module contract and no acquisition of that module singleton can install
    /// an own-property override.
    /// </summary>
    public bool CanSkipNodeModuleOverrideGuard { get; set; }

    /// <summary>
    /// True when a non-captured <c>var</c> or <c>let</c> binding is proven numeric and
    /// definitely initialized before every reachable read in its callable.
    /// </summary>
    public bool CanUseUnboxedLocal { get; set; }

    /// <summary>
    /// True when accesses to this binding must respect the temporal dead zone.
    /// This applies to lexical declarations (<c>let</c>, <c>const</c>, and class bindings modeled as <c>let</c>)
    /// but not to parameters injected into the scope.
    /// </summary>
    public bool RequiresTemporalDeadZoneChecks
        => (Kind == BindingKind.Let || Kind == BindingKind.Const)
           && !DeclaringScope.Parameters.Contains(Name);

    /// <summary>
    /// True when a captured lexical binding needs runtime TDZ machinery on its backing field.
    /// This is stricter than <see cref="RequiresTemporalDeadZoneChecks"/>: safe captured lexicals
    /// can still use typed fields and direct loads when the compiler proves they cannot be
    /// observed before initialization.
    /// </summary>
    public bool RequiresRuntimeTemporalDeadZoneChecks { get; set; }

    /// <summary>
    /// True when this captured binding is a <c>const</c> initialized from a primitive
    /// literal and all reads are known to occur after initialization.
    /// </summary>
    public bool IsCompileTimeConstant { get; set; }

    /// <summary>
    /// The JavaScript type of <see cref="CompileTimeConstantValue"/>.
    /// </summary>
    public JavascriptType CompileTimeConstantType { get; set; } = JavascriptType.Unknown;

    /// <summary>
    /// The primitive value substituted at eligible read sites.
    /// </summary>
    public object? CompileTimeConstantValue { get; set; }

    /// <summary>
    /// Shape analysis result when this binding is declared with an object literal initializer.
    /// Populated by <c>SymbolTableBuilder.AnalyzeObjectLiteralShapes</c>; null when the binding
    /// is not an object-literal declaration. Consumers must check
    /// <see cref="ObjectLiteralShapeInfo.IsEligible"/> before early-binding member access.
    /// </summary>
    public ObjectLiteralShapeInfo? ObjectLiteralShape { get; set; }

    /// <summary>
    /// Whole-program policy for function-valued binding initialization. HIR carries this
    /// semantic decision into lowering; LIR never inspects the source AST to derive it.
    /// </summary>
    public CallableMaterializationDecision? CallableMaterialization { get; set; }

    /// <summary>
    /// Canonical callable identity assigned during phase-one discovery for a binding
    /// that denotes a callable value.
    /// </summary>
    public CallableId? Callable { get; set; }

    /// <summary>
    /// Generated class scope assigned during phase-one discovery when this binding
    /// denotes a class declaration or expression.
    /// </summary>
    public Scope? ClassScope { get; set; }

    public BindingInfo(string name, BindingKind kind, Scope declaringScope, Node declarationNode)
    {
        Name = name;
        Kind = kind;
        DeclaringScope = declaringScope;
        DeclarationNode = declarationNode;
    }
}
