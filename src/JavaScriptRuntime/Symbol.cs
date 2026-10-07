namespace JavaScriptRuntime;

/// <summary>
/// Minimal Symbol callable intrinsic support.
///
/// Symbols are represented as opaque unique reference objects.
/// This is sufficient for typeof/equality semantics used by tests.
/// </summary>
[IntrinsicObject("Symbol")]
public sealed class Symbol
{
    internal static void ConfigureIntrinsicSurface(object constructorValue, object prototypeValue, object objectPrototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        BuiltinFunction0 descriptionGetter = SymbolPrototypeDescription;
        BuiltinFunction0 toPrimitive = SymbolPrototypeToPrimitive;
        PrototypeChain.SetPrototype(prototypeValue, objectPrototype);
        GlobalThis.ConfigureBuiltinFunctionObject(constructorValue);
        // The "description" parameter is optional (Symbol ( [ description ] )), so the
        // spec-mandated length is 0. BuiltinFunction1's automatic length inference always
        // reports 1 (one JS-visible parameter), so it must be overridden explicitly here to
        // preserve the pre-migration length value that the legacy array-based ABI computed.
        JavaScriptRuntime.Function.DefineMetadataProperty(constructorValue, "length", 0d);
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = prototypeValue
        });

        PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = constructorValue
        });
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            descriptionGetter,
            0d,
            "get description",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(descriptionGetter));
        GlobalThis.DefineUndefinedPrototypeProperty(descriptionGetter);
        PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "description", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = descriptionGetter
        });
        GlobalThis.DefineBuiltinFunctionProperty(
            prototypeValue,
            "toString",
            (BuiltinFunction0)(thisArgument =>
                TryGetThisSymbolValue(thisArgument, out var symbol)
                    ? symbol.toString()
                    : throw new TypeError("Symbol.prototype.toString called on incompatible receiver")),
            0d);
        GlobalThis.DefineBuiltinFunctionProperty(
            prototypeValue,
            "valueOf",
            (BuiltinFunction0)(thisArgument =>
                TryGetThisSymbolValue(thisArgument, out var symbol)
                    ? symbol.valueOf()
                    : throw new TypeError("Symbol.prototype.valueOf called on incompatible receiver")),
            0d);
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            toPrimitive,
            1d,
            "[Symbol.toPrimitive]",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(toPrimitive));
        GlobalThis.DefineUndefinedPrototypeProperty(toPrimitive);
        PropertyDescriptorStore.DefineOrUpdate(
            prototypeValue,
            global::JavaScriptRuntime.Symbol.toPrimitive.DebugId,
            new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = toPrimitive
            });
        GlobalThis.DefineIntrinsicToStringTagProperty(prototypeValue, "Symbol");
        GlobalThis.DefineIntrinsicDataProperty(constructorValue, "for", (Func<object?, object>)global::JavaScriptRuntime.Symbol.@for);
        GlobalThis.DefineIntrinsicDataProperty(constructorValue, "keyFor", (Func<object?, object?>)global::JavaScriptRuntime.Symbol.keyFor);
        DefineWellKnownSymbolProperty(constructorValue, "iterator", global::JavaScriptRuntime.Symbol.iterator);
        DefineWellKnownSymbolProperty(constructorValue, "asyncIterator", global::JavaScriptRuntime.Symbol.asyncIterator);
        DefineWellKnownSymbolProperty(constructorValue, "hasInstance", global::JavaScriptRuntime.Symbol.hasInstance);
        DefineWellKnownSymbolProperty(constructorValue, "isConcatSpreadable", global::JavaScriptRuntime.Symbol.isConcatSpreadable);
        DefineWellKnownSymbolProperty(constructorValue, "match", global::JavaScriptRuntime.Symbol.match);
        DefineWellKnownSymbolProperty(constructorValue, "matchAll", global::JavaScriptRuntime.Symbol.matchAll);
        DefineWellKnownSymbolProperty(constructorValue, "replace", global::JavaScriptRuntime.Symbol.replace);
        DefineWellKnownSymbolProperty(constructorValue, "search", global::JavaScriptRuntime.Symbol.search);
        DefineWellKnownSymbolProperty(constructorValue, "species", global::JavaScriptRuntime.Symbol.species);
        DefineWellKnownSymbolProperty(constructorValue, "split", global::JavaScriptRuntime.Symbol.split);
        DefineWellKnownSymbolProperty(constructorValue, "toPrimitive", global::JavaScriptRuntime.Symbol.toPrimitive);
        DefineWellKnownSymbolProperty(constructorValue, "toStringTag", global::JavaScriptRuntime.Symbol.toStringTag);
        DefineWellKnownSymbolProperty(constructorValue, "unscopables", global::JavaScriptRuntime.Symbol.unscopables);
        DefineWellKnownSymbolProperty(constructorValue, "dispose", global::JavaScriptRuntime.Symbol.dispose);
        DefineWellKnownSymbolProperty(constructorValue, "asyncDispose", global::JavaScriptRuntime.Symbol.asyncDispose);
    }

    private static bool TryGetThisSymbolValue(
        object? thisValue,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JavaScriptRuntime.Symbol? symbol)
    {
        if (thisValue is JavaScriptRuntime.Symbol directSymbol)
        {
            symbol = directSymbol;
            return true;
        }

        if (thisValue != null
            && PropertyDescriptorStore.TryGetOwn(thisValue, ObjectRuntime.PrimitiveValuePropertyName, out var descriptor)
            && descriptor.Value is JavaScriptRuntime.Symbol boxedSymbol)
        {
            symbol = boxedSymbol;
            return true;
        }

        symbol = null;
        return false;
    }

    private static object? SymbolPrototypeDescription(object? thisArgument)
    {
        if (!TryGetThisSymbolValue(thisArgument, out var symbol))
        {
            throw new TypeError("Symbol.prototype.description called on incompatible receiver");
        }

        return symbol.Description;
    }

    private static object? SymbolPrototypeToPrimitive(object? thisArgument)
    {
        return TryGetThisSymbolValue(thisArgument, out var symbol)
            ? symbol
            : throw new TypeError("Symbol.prototype[Symbol.toPrimitive] called on incompatible receiver");
    }

    private static void DefineWellKnownSymbolProperty(object constructorValue, string key, global::JavaScriptRuntime.Symbol value)
    {
        PropertyDescriptorStore.DefineOrUpdate(constructorValue, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = value
        });
    }

    private static long _nextId;

    // Well-known symbols used by core language features.
    // These are singletons so identity comparisons work as expected.
    private static readonly Symbol _iterator = new Symbol("Symbol.iterator");
    private static readonly Symbol _asyncIterator = new Symbol("Symbol.asyncIterator");
    private static readonly Symbol _hasInstance = new Symbol("Symbol.hasInstance");
    private static readonly Symbol _isConcatSpreadable = new Symbol("Symbol.isConcatSpreadable");
    private static readonly Symbol _match = new Symbol("Symbol.match");
    private static readonly Symbol _matchAll = new Symbol("Symbol.matchAll");
    private static readonly Symbol _replace = new Symbol("Symbol.replace");
    private static readonly Symbol _search = new Symbol("Symbol.search");
    private static readonly Symbol _species = new Symbol("Symbol.species");
    private static readonly Symbol _split = new Symbol("Symbol.split");
    private static readonly Symbol _toPrimitive = new Symbol("Symbol.toPrimitive");
    private static readonly Symbol _toStringTag = new Symbol("Symbol.toStringTag");
    private static readonly Symbol _unscopables = new Symbol("Symbol.unscopables");
    private static readonly Symbol _dispose = new Symbol("Symbol.dispose");
    private static readonly Symbol _asyncDispose = new Symbol("Symbol.asyncDispose");

    private readonly long _id;
    private readonly string _debugId;

    public string? Description { get; }
    // JS-surface members intentionally use ECMAScript casing for dynamic member lookup.
    public string? description => Description;

    public Symbol()
    {
        _id = System.Threading.Interlocked.Increment(ref _nextId);
        _debugId = $"Symbol({_id})";
        Description = null;
    }

    public Symbol(object? description)
    {
        _id = System.Threading.Interlocked.Increment(ref _nextId);
        _debugId = $"Symbol({_id})";

        // JS: undefined => no description; otherwise ToString.
        if (description is null)
        {
            Description = null;
        }
        else
        {
            Description = DotNet2JSConversions.ToStringRejectingSymbols(description);
        }
    }

    // Callable form: Symbol([description])
    public static object Call()
    {
        return new Symbol();
    }

    public static object Call(object? description)
    {
        return new Symbol(description);
    }

    public override string ToString()
    {
        return Description == null ? "Symbol()" : $"Symbol({Description})";
    }

    public string toString() => ToString();

    public Symbol valueOf() => this;

    // Well-known symbol: Symbol.iterator
    public static Symbol iterator => _iterator;

    // Well-known symbol: Symbol.asyncIterator
    public static Symbol asyncIterator => _asyncIterator;

    // Well-known symbol: Symbol.hasInstance
    public static Symbol hasInstance => _hasInstance;

    // Well-known symbol: Symbol.isConcatSpreadable
    public static Symbol isConcatSpreadable => _isConcatSpreadable;

    // Well-known symbol: Symbol.match
    public static Symbol match => _match;

    // Well-known symbol: Symbol.matchAll
    public static Symbol matchAll => _matchAll;

    // Well-known symbol: Symbol.replace
    public static Symbol replace => _replace;

    // Well-known symbol: Symbol.search
    public static Symbol search => _search;

    // Well-known symbol: Symbol.species
    public static Symbol species => _species;

    // Well-known symbol: Symbol.split
    public static Symbol split => _split;

    // Well-known symbol: Symbol.toPrimitive
    public static Symbol toPrimitive => _toPrimitive;

    // Well-known symbol: Symbol.toStringTag
    public static Symbol toStringTag => _toStringTag;

    // Well-known symbol: Symbol.unscopables
    public static Symbol unscopables => _unscopables;

    // Well-known symbol: Symbol.dispose
    public static Symbol dispose => _dispose;

    // Well-known symbol: Symbol.asyncDispose
    public static Symbol asyncDispose => _asyncDispose;

    // Symbol.for(key)
    public static object @for(object? key)
    {
        var registryKey = DotNet2JSConversions.ToStringRejectingSymbols(key);
        return GetCurrentRegistry().GetOrCreate(registryKey);
    }

    // Symbol.keyFor(sym)
    public static object? keyFor(object? sym)
    {
        if (sym is not Symbol symbol)
        {
            throw new TypeError("Symbol.keyFor requires a symbol");
        }

        return GetCurrentRegistry().GetKey(symbol);
    }

    // Access well-known symbols via property-read lowering (e.g., Symbol.iterator).
    // Returns null (JS undefined) when the well-known symbol is not supported.
    public static object? GetWellKnown(string name)
    {
        return name switch
        {
            "iterator" => iterator,
            "asyncIterator" => asyncIterator,
            "hasInstance" => hasInstance,
            "isConcatSpreadable" => isConcatSpreadable,
            "match" => match,
            "matchAll" => matchAll,
            "replace" => replace,
            "search" => search,
            "species" => species,
            "split" => split,
            "toPrimitive" => toPrimitive,
            "toStringTag" => toStringTag,
            "unscopables" => unscopables,
            "dispose" => dispose,
            "asyncDispose" => asyncDispose,
            _ => null
        };
    }

    public static bool IsWellKnown(string name)
        => GetWellKnown(name) != null;

    internal static bool IsRegistered(Symbol symbol)
        => RuntimeExecutionContext.CurrentOrOverride?.Agent.SymbolRegistry.Contains(symbol)
            ?? false;

    private static RuntimeAgentSymbolRegistry GetCurrentRegistry()
        => RuntimeExecutionContext.CurrentOrOverride?.Agent.SymbolRegistry
            ?? throw new InvalidOperationException(
                "The global symbol registry requires an active JavaScript runtime.");

    // Useful for debugging, but keep ToString() JS-like.
    public string DebugId => _debugId;
}
