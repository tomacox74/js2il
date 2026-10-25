using Jroc.HIR;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private enum ResourceCompletionKind { Sync, Async, Generator }
    private sealed record ResourceScopeContext(TempVariable Resources, ResourceCompletionKind CompletionKind);
    private readonly Stack<ResourceScopeContext> _resourceScopes = new();

    private bool TryLowerResourceScope(
        HIRStatement body,
        HIRResourceKind kind,
        Func<bool>? lowerTryBlock = null)
    {
        var resources = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.CreateScope), [], resources));
        DefineTempStorage(resources, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(resources, CreateAnonymousVariableSlot("$resources", GetTempStorage(resources)));

        var completionKind = _isGenerator && CountYieldExpressionsInStatement(body) > 0
            ? ResourceCompletionKind.Generator
            : _isAsync && (kind == HIRResourceKind.Async || CountAwaitExpressionsInStatement(body) > 0)
                ? ResourceCompletionKind.Async
                : ResourceCompletionKind.Sync;
        _resourceScopes.Push(new ResourceScopeContext(resources, completionKind));
        try
        {
            return TryLowerTryStatement(
                new HIRTryStatement(body, null, null, null, new HIRDisposeResourcesStatement(kind)),
                lowerTryBlock);
        }
        finally
        {
            _resourceScopes.Pop();
        }
    }

    private TempVariable AddDisposableResource(TempVariable value, HIRResourceKind kind)
    {
        if (_resourceScopes.Count == 0)
            throw new InvalidOperationException("A resource declaration requires an enclosing disposal scope.");
        var start = _methodBodyIR.Instructions.Count;
        var asynchronous = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(kind == HIRResourceKind.Async, asynchronous));
        DefineTempStorage(asynchronous, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.Add),
            [_resourceScopes.Peek().Resources, EnsureObject(value), asynchronous], result));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        if (_asyncTryCatchStack.Count > 0 || _generatorTryCatchFinallyStack.Count > 0)
            ProtectGeneratorExpression(start);
        return result;
    }

    private bool TryLowerForOfIteration(HIRForOfStatement statement, TempVariable value)
    {
        bool LowerBindingAndBody()
        {
            if (statement.ResourceKind != HIRResourceKind.None)
                value = AddDisposableResource(value, statement.ResourceKind);
            var writeMode = statement.IsDeclaration && statement.DeclarationKind is BindingKind.Let or BindingKind.Const
                ? DestructuringWriteMode.ForDeclarationBindingInitialization
                : DestructuringWriteMode.Assignment;
            return TryLowerDestructuringPattern(statement.Target, value, writeMode, sourceNameForError: null)
                && TryLowerStatement(statement.Body);
        }

        return statement.ResourceKind == HIRResourceKind.None
            ? LowerBindingAndBody()
            : TryLowerResourceScope(statement.Body, statement.ResourceKind, LowerBindingAndBody);
    }

    private bool TryLowerDisposeResources(HIRDisposeResourcesStatement statement)
    {
        var context = _resourceScopes.Peek();
        TempVariable hasError;
        TempVariable error;
        string pendingExceptionField;
        if (context.CompletionKind == ResourceCompletionKind.Generator)
        {
            pendingExceptionField = GeneratorPendingExceptionField;
            hasError = LoadPendingScopeField(GeneratorHasPendingExceptionField, typeof(bool));
            error = LoadPendingScopeField(pendingExceptionField, typeof(object));
        }
        else if (context.CompletionKind == ResourceCompletionKind.Async)
        {
            pendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._pendingException);
            hasError = LoadPendingScopeField(nameof(JavaScriptRuntime.AsyncScope._hasPendingException), typeof(bool));
            error = LoadPendingScopeField(pendingExceptionField, typeof(object));
        }
        else
        {
            var completion = _syncTryFinallyStack.Peek();
            pendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._pendingException);
            hasError = completion.HasPendingException;
            error = completion.PendingException;
        }
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.BeginDisposal),
            [context.Resources, hasError, EnsureObject(error)]));

        if (statement.Kind == HIRResourceKind.Sync)
        {
            var advanced = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.DisposableResourceScope),
                nameof(JavaScriptRuntime.DisposableResourceScope.MoveNext), [context.Resources], advanced));
            DefineTempStorage(advanced, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            return EmitResourceDisposalCompletion(context.Resources);
        }

        var asyncInfo = _methodBodyIR.AsyncInfo
            ?? throw new InvalidOperationException("Asynchronous disposal requires an async state machine.");
        var loop = CreateLabel();
        var finished = CreateLabel();
        var rejected = CreateLabel();
        var rejectedState = asyncInfo.AllocateResumeStateId();
        asyncInfo.RegisterResumeLabel(rejectedState, rejected);
        _methodBodyIR.Instructions.Add(new LIRLabel(loop));
        var hasNext = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.MoveNext), [context.Resources], hasNext));
        DefineTempStorage(hasNext, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasNext, finished));
        var awaitedValue = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.GetAwaitedValue), [context.Resources], awaitedValue));
        DefineTempStorage(awaitedValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _asyncTryCatchStack.Push(new AsyncTryCatchContext(rejectedState, rejected, pendingExceptionField));
        try
        {
            if (!TryLowerAwaitValue(awaitedValue, out _))
                return false;
        }
        finally
        {
            _asyncTryCatchStack.Pop();
        }
        _methodBodyIR.Instructions.Add(new LIRBranch(loop));
        _methodBodyIR.Instructions.Add(new LIRLabel(rejected));
        var reason = LoadPendingScopeField(pendingExceptionField, typeof(object));
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.RecordError), [context.Resources, reason]));
        _methodBodyIR.Instructions.Add(new LIRBranch(loop));
        _methodBodyIR.Instructions.Add(new LIRLabel(finished));
        return EmitResourceDisposalCompletion(context.Resources);
    }

    private bool EmitResourceDisposalCompletion(TempVariable resources)
    {
        var hasError = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.HasError), [resources], hasError));
        DefineTempStorage(hasError, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var completed = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasError, completed));
        var error = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.DisposableResourceScope),
            nameof(JavaScriptRuntime.DisposableResourceScope.GetError), [resources], error));
        DefineTempStorage(error, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        if (!EmitThrowCompletion(error))
            return false;
        _methodBodyIR.Instructions.Add(new LIRLabel(completed));
        return true;
    }
}
