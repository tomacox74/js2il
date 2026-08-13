namespace JavaScriptRuntime.Modules.CommonJS;

internal sealed class RequireFunctionTarget
{
    private readonly Require _requireService;
    private readonly string _parentModuleId;
    private readonly bool _preserveResolvableParentTraversal;
    private readonly RequireDelegate _require;
    private readonly object _functionValueLock = new();
    private BuiltinDelegateFunctionAdapter? _functionValue;
    private Module? _mainModule;

    internal RequireFunctionTarget(
        Require requireService,
        string parentModuleId,
        bool preserveResolvableParentTraversal = false)
    {
        _requireService = requireService;
        _parentModuleId = parentModuleId;
        _preserveResolvableParentTraversal = preserveResolvableParentTraversal;
        _require = Invoke;
    }

    internal RequireDelegate Require => _require;

    internal bool IsFunctionValueMaterializedForTests
        => Volatile.Read(ref _functionValue) != null;

    internal void SetMainModule(Module mainModule)
    {
        ArgumentNullException.ThrowIfNull(mainModule);

        lock (_functionValueLock)
        {
            _mainModule = mainModule;
            if (_functionValue != null)
            {
                ObjectRuntime.SetProperty(_functionValue, "main", mainModule);
            }
        }
    }

    internal BuiltinDelegateFunctionAdapter GetFunctionValue()
    {
        var existing = Volatile.Read(ref _functionValue);
        if (existing != null)
        {
            return existing;
        }

        lock (_functionValueLock)
        {
            if (_functionValue != null)
            {
                return _functionValue;
            }

            var functionValue = BuiltinDelegateFunctionAdapter.FromDelegate(_require);
            Function.InitializeFunctionInstance(
                functionValue,
                1d,
                "require",
                requiresInvocationContext: false);
            if (_mainModule != null)
            {
                ObjectRuntime.SetProperty(functionValue, "main", _mainModule);
            }

            Volatile.Write(ref _functionValue, functionValue);
            return functionValue;
        }
    }

    private object? Invoke(object? moduleId)
    {
        if (moduleId is not string requestedSpecifier)
        {
            throw new TypeError("The \"id\" argument must be of type string.");
        }

        return _preserveResolvableParentTraversal
            && ShouldPreserveRawSpecifier(requestedSpecifier)
                ? _requireService.RequireModule(requestedSpecifier)
                : _requireService.RequireModuleFrom(
                    _parentModuleId,
                    requestedSpecifier);
    }

    private bool ShouldPreserveRawSpecifier(string requestedSpecifier)
    {
        var normalized = requestedSpecifier.Trim().Replace('\\', '/');
        if (!normalized.StartsWith("./", StringComparison.Ordinal)
            && !normalized.StartsWith("../", StringComparison.Ordinal))
        {
            return false;
        }

        var hasParentTraversal = normalized
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Any(segment => string.Equals(segment, "..", StringComparison.Ordinal));
        return hasParentTraversal
            && _requireService.CanResolveLocalModule(requestedSpecifier);
    }
}
