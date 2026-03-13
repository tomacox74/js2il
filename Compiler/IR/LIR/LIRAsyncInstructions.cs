using Js2IL.Services.ScopesAbi;

namespace Js2IL.IR;

// ============================================================================
// Async/Await State Machine Instructions
// ============================================================================

/// <summary>
/// Represents an await expression in an async function.
/// The lowering transforms this into:
/// 1. Evaluate the awaited expression
/// 2. Call Promise.resolve() on it
/// 3. Set up .then() continuations that will call MoveNext
/// 4. Return from MoveNext (suspend)
/// 5. Resume label for when the promise settles
/// </summary>
public record LIRAwait(
    /// <summary>The expression being awaited (already evaluated to a temp)</summary>
    TempVariable AwaitedValue,
    /// <summary>
    /// The await ID for this await point (used only for selecting awaited-result storage, e.g. _awaited1).
    /// This is intentionally separate from ResumeStateId so synthetic resume states (e.g. catch resumption)
    /// don't shift awaited-result field numbering.
    /// </summary>
    int AwaitId,
    /// <summary>The state ID to set before suspending (used for resumption)</summary>
    int ResumeStateId,
    /// <summary>The label ID to jump to when resuming</summary>
    int ResumeLabelId,
    /// <summary>The temp variable to store the result value after resumption</summary>
    TempVariable Result,
    /// <summary>Optional state ID to resume at when the awaited promise rejects</summary>
    int? RejectResumeStateId = null,
    /// <summary>Optional scope field name to store the pending exception</summary>
    string? PendingExceptionFieldName = null) : LIRInstruction;

/// <summary>
/// Initializes async function state at the start of an async entry method.
/// Sets up the deferred promise via Promise.withResolvers() and initializes state to 0.
/// Emits: call Promise.withResolvers(), store deferred fields on scope
/// </summary>
public record LIRAsyncInitialize(
    /// <summary>The scope ID where async state fields are stored</summary>
    ScopeId Scope) : LIRInstruction;

/// <summary>
/// Calls MoveNext to start or resume the async state machine.
/// Emits: call MoveNext(scopes)
/// </summary>
public record LIRAsyncCallMoveNext(
    /// <summary>The scopes array to pass to MoveNext</summary>
    TempVariable ScopesArray) : LIRInstruction;

/// <summary>
/// Returns the promise from an async entry method.
/// Emits: ldloc.0 (scope), ldfld _deferred, callvirt get_promise, ret
/// </summary>
public record LIRAsyncReturnPromise(
    /// <summary>The scope ID containing the deferred promise</summary>
    ScopeId Scope) : LIRInstruction;

/// <summary>
/// Loads the async state field from the scope instance.
/// Emits: ldloc.0 (scope), ldfld _asyncState
/// </summary>
public record LIRAsyncLoadState(TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value to the async state field on the scope instance.
/// Emits: ldloc.0 (scope), ldc.i4 stateValue, stfld _asyncState
/// </summary>
public record LIRAsyncStoreState(int StateValue) : LIRInstruction;

/// <summary>
/// Resolves the async function's deferred promise with a value.
/// Called when the async function completes normally.
/// Emits: ldloc.0 (scope), ldfld _deferred, callvirt get_resolve, ldarg value, call Closure.InvokeWithArgs
/// </summary>
public record LIRAsyncResolve(TempVariable Value) : LIRInstruction;

/// <summary>
/// Rejects the async function's deferred promise with a reason.
/// Called when the async function throws an exception.
/// Emits: ldloc.0 (scope), ldfld _deferred, callvirt get_reject, ldarg reason, call Closure.InvokeWithArgs
/// </summary>
public record LIRAsyncReject(TempVariable Reason) : LIRInstruction;

/// <summary>
/// A multi-way branch based on the async state value.
/// Used at the start of MoveNext to dispatch to the correct resume point.
/// Emits: switch instruction or cascading branches depending on state count
/// </summary>
public record LIRAsyncStateSwitch(
    /// <summary>The state value temp to switch on</summary>
    TempVariable StateValue,
    /// <summary>Mapping from state ID to label ID (including state 0 for initial entry)</summary>
    IReadOnlyDictionary<int, int> StateToLabel,
    /// <summary>Label to jump to for invalid/completed states (default case)</summary>
    int DefaultLabel) : LIRInstruction;

/// <summary>
/// Stores the awaited result value to a field on the scope instance.
/// Used in the onFulfilled continuation to store the value before calling MoveNext.
/// </summary>
public record LIRAsyncStoreAwaitedResult(
    /// <summary>The field name to store the result in (e.g., "_awaited1")</summary>
    string FieldName,
    TempVariable Value) : LIRInstruction;

/// <summary>
/// Loads the awaited result value from a field on the scope instance.
/// Used after resumption to get the value that was stored by the onFulfilled continuation.
/// </summary>
public record LIRAsyncLoadAwaitedResult(
    /// <summary>The field name to load the result from (e.g., "_awaited1")</summary>
    string FieldName,
    TempVariable Result) : LIRInstruction;
