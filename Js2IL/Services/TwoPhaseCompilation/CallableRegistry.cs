using System.Reflection.Metadata;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Complete information about a callable, combining identity and signature.
/// This is stored in the CallableRegistry.
/// </summary>
public sealed record CallableInfo
{
    /// <summary>The stable identifier for this callable.</summary>
    public required CallableId Id { get; init; }
    
    /// <summary>The signature/metadata needed to emit IL references to this callable.</summary>
    public required CallableSignature Signature { get; init; }
    
    /// <summary>
    /// The declared method token (populated during Phase 1 declaration).
    /// This handle is valid even before the method body is compiled.
    /// </summary>
    public MethodDefinitionHandle? MethodToken { get; init; }
    
    /// <summary>
    /// Whether the body has been compiled (set during Phase 2).
    /// Used for diagnostics and invariant checking.
    /// </summary>
    public bool BodyCompiled { get; init; }
}

/// <summary>
/// Read-only view of the callable catalog for querying callable information.
/// </summary>
public interface ICallableCatalog
{
    /// <summary>Attempts to retrieve callable info by its ID.</summary>
    bool TryGet(CallableId id, out CallableInfo? info);
    
    /// <summary>Gets all registered callable IDs.</summary>
    IReadOnlyCollection<CallableId> AllCallables { get; }
    
    /// <summary>Checks if a callable is registered.</summary>
    bool Contains(CallableId id);
}

/// <summary>
/// Write interface for Phase 1: declaring callables and setting their tokens.
/// </summary>
public interface ICallableDeclarationWriter
{
    /// <summary>
    /// Declares a callable with its signature (Phase 1 - discovery).
    /// </summary>
    void Declare(CallableId id, CallableSignature signature);
    
    /// <summary>
    /// Sets the method token for a previously declared callable (Phase 1 - token allocation).
    /// </summary>
    void SetToken(CallableId id, MethodDefinitionHandle token);
}

/// <summary>
/// Read interface for Phase 2: looking up declared callable tokens.
/// </summary>
public interface ICallableDeclarationReader
{
    /// <summary>
    /// Gets the declared method token for a callable (must exist in strict mode).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown in strict mode if callable is not declared.</exception>
    MethodDefinitionHandle GetDeclaredToken(CallableId id);
    
    /// <summary>
    /// Attempts to get the declared method token for a callable.
    /// Used for legacy/migration fallback paths.
    /// </summary>
    bool TryGetDeclaredToken(CallableId id, out MethodDefinitionHandle token);
    
    /// <summary>
    /// Gets the signature for a declared callable.
    /// </summary>
    CallableSignature? GetSignature(CallableId id);
}

/// <summary>
/// Single source of truth for callable declarations in the two-phase compilation pipeline.
/// 
/// The CallableRegistry stores information about all callables in a module:
/// - During Phase 1: callables are discovered and declared (signatures + tokens allocated)
/// - During Phase 2: callable tokens are looked up when emitting IL references
/// 
/// This replaces the ad-hoc "function cache vs arrow cache vs class registry" approach
/// with a unified, CallableId-keyed store.
/// </summary>
public sealed class CallableRegistry : ICallableCatalog, ICallableDeclarationWriter, ICallableDeclarationReader
{
    private readonly Dictionary<CallableId, CallableInfo> _callables = new();
    private readonly object _lock = new();
    
    /// <summary>
    /// Whether to throw on missing callable lookups (strict mode).
    /// When true, GetDeclaredToken throws if the callable is not declared.
    /// </summary>
    public bool StrictMode { get; set; }

    #region ICallableCatalog

    public bool TryGet(CallableId id, out CallableInfo? info)
    {
        lock (_lock)
        {
            return _callables.TryGetValue(id, out info);
        }
    }

    public IReadOnlyCollection<CallableId> AllCallables
    {
        get
        {
            lock (_lock)
            {
                return _callables.Keys.ToList();
            }
        }
    }

    public bool Contains(CallableId id)
    {
        lock (_lock)
        {
            return _callables.ContainsKey(id);
        }
    }

    #endregion

    #region ICallableDeclarationWriter

    public void Declare(CallableId id, CallableSignature signature)
    {
        lock (_lock)
        {
            if (_callables.ContainsKey(id))
            {
                // Allow re-declaration with same signature (idempotent)
                return;
            }
            
            _callables[id] = new CallableInfo
            {
                Id = id,
                Signature = signature,
                MethodToken = null,
                BodyCompiled = false
            };
        }
    }

    public void SetToken(CallableId id, MethodDefinitionHandle token)
    {
        lock (_lock)
        {
            if (!_callables.TryGetValue(id, out var info))
            {
                throw new InvalidOperationException(
                    $"Cannot set token for undeclared callable: {id.DisplayName}. " +
                    "Call Declare() first during Phase 1 discovery.");
            }
            
            _callables[id] = info with { MethodToken = token };
        }
    }

    #endregion

    #region ICallableDeclarationReader

    public MethodDefinitionHandle GetDeclaredToken(CallableId id)
    {
        lock (_lock)
        {
            if (_callables.TryGetValue(id, out var info) && info.MethodToken.HasValue)
            {
                return info.MethodToken.Value;
            }
            
            if (StrictMode)
            {
                throw new InvalidOperationException(
                    $"Missing callable token for {id.DisplayName}. " +
                    "Phase 1 declaration may have been skipped or incomplete.");
            }
            
            return default;
        }
    }

    public bool TryGetDeclaredToken(CallableId id, out MethodDefinitionHandle token)
    {
        lock (_lock)
        {
            if (_callables.TryGetValue(id, out var info) && info.MethodToken.HasValue)
            {
                token = info.MethodToken.Value;
                return true;
            }
            
            token = default;
            return false;
        }
    }

    public CallableSignature? GetSignature(CallableId id)
    {
        lock (_lock)
        {
            return _callables.TryGetValue(id, out var info) ? info.Signature : null;
        }
    }

    #endregion

    #region Phase 2 Support

    /// <summary>
    /// Marks a callable's body as compiled (for diagnostics/invariant checking).
    /// </summary>
    public void MarkBodyCompiled(CallableId id)
    {
        lock (_lock)
        {
            if (_callables.TryGetValue(id, out var info))
            {
                _callables[id] = info with { BodyCompiled = true };
            }
        }
    }

    /// <summary>
    /// Checks if a callable's body has been compiled.
    /// </summary>
    public bool IsBodyCompiled(CallableId id)
    {
        lock (_lock)
        {
            return _callables.TryGetValue(id, out var info) && info.BodyCompiled;
        }
    }

    #endregion

    #region Statistics (for diagnostics)

    /// <summary>Gets the total number of declared callables.</summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _callables.Count;
            }
        }
    }

    /// <summary>Gets the number of callables with tokens allocated.</summary>
    public int TokensAllocated
    {
        get
        {
            lock (_lock)
            {
                return _callables.Values.Count(c => c.MethodToken.HasValue);
            }
        }
    }

    /// <summary>Gets the number of callables with bodies compiled.</summary>
    public int BodiesCompiled
    {
        get
        {
            lock (_lock)
            {
                return _callables.Values.Count(c => c.BodyCompiled);
            }
        }
    }

    #endregion
}
