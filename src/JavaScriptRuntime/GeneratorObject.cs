using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

/// <summary>
/// Runtime representation of a synchronous generator object.
///
/// The generator object stores:
/// - A bound step closure (the compiled generator method, bound to the generator's scopes array)
/// - The scopes array (with the generator leaf scope at index 0)
/// - The original call arguments for the generator function
///
/// Each call to next/throw/return sets resume protocol fields on the leaf scope
/// (which inherits <see cref="GeneratorScope"/>) and then invokes the step closure.
/// </summary>
public sealed class GeneratorObject
{
    private static readonly object Prototype = CreatePrototype();

    // Stable singleton used as %GeneratorPrototype%.constructor.
    // Per ECMA-262, gen.constructor is the same function object for all generator instances.
    private static readonly Func<object[], object?[], object?> _generatorFunctionConstructor =
        static (_, _) => null;

    private readonly object _step;
    private readonly object[] _scopes;
    private readonly object?[] _args;

    public GeneratorObject(object step, object[] scopes, object?[] args)
    {
        _step = step ?? throw new ArgumentNullException(nameof(step));
        _scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        _args = args ?? throw new ArgumentNullException(nameof(args));
        InitializeGeneratorSurface(this);
    }

    /// <summary>
    /// %GeneratorPrototype%.constructor — stable function object, same for all generator instances.
    /// </summary>
    public object constructor => _generatorFunctionConstructor;

    private static object CreatePrototype()
    {
        var prototype = new JsObject();
        PrototypeChain.SetPrototype(prototype, Iterator.Prototype);
        DefineDataProperty(prototype, "constructor", _generatorFunctionConstructor);
        DefineDataProperty(prototype, "next", (Func<object[], object?[]?, object?>)PrototypeNext);
        DefineDataProperty(prototype, "return", (Func<object[], object?[]?, object?>)PrototypeReturn);
        DefineDataProperty(prototype, "throw", (Func<object[], object?[]?, object?>)PrototypeThrow);
        DefineDataProperty(prototype, Symbol.toStringTag.DebugId, "Generator");
        return prototype;
    }

    private static void InitializeGeneratorSurface(object generator)
    {
        if (PrototypeChain.GetPrototypeOrNull(generator) == null)
        {
            PrototypeChain.SetPrototype(generator, Prototype);
        }
    }

    private static void DefineDataProperty(object target, string key, object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    private static GeneratorObject GetReceiver(string methodName)
    {
        var receiver = RuntimeServices.GetCurrentThis();
        if (receiver is GeneratorObject generator)
        {
            return generator;
        }

        throw new TypeError($"Generator.prototype.{methodName} called on incompatible receiver");
    }

    private static object? PrototypeNext(object[] scopes, object?[]? args)
    {
        return GetReceiver("next").next(args != null && args.Length > 0 ? args[0] : null);
    }

    private static object? PrototypeReturn(object[] scopes, object?[]? args)
    {
        return GetReceiver("return").@return(args != null && args.Length > 0 ? args[0] : null);
    }

    private static object? PrototypeThrow(object[] scopes, object?[]? args)
    {
        return GetReceiver("throw").@throw(args != null && args.Length > 0 ? args[0] : null);
    }

    private GeneratorScope GetLeafScope()
    {
        if (_scopes.Length == 0)
        {
            throw new InvalidOperationException("Generator scopes array is empty.");
        }

        if (_scopes[0] is not GeneratorScope gs)
        {
            throw new InvalidOperationException($"Generator scopes[0] is not a GeneratorScope (actual={_scopes[0]?.GetType().FullName ?? "<null>"}).");
        }

        return gs;
    }

    /// <summary>
    /// Implements generator.next(value).
    /// On first next(value), the value is ignored.
    /// </summary>
    public object next(object? value = null)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            return IteratorResult.Create(null, done: true);
        }

        // Clear prior resume protocol.
        scope.HasResumeException = false;
        scope.ResumeException = null;
        scope.HasReturn = false;
        scope.ReturnValue = null;

        // On first next(arg), arg is ignored per JS semantics.
        scope.ResumeValue = scope.Started ? value : null;

        try
        {
            return Closure.InvokeWithArgs(_step, _scopes, _args);
        }
        catch
        {
            scope.Done = true;
            throw;
        }
    }

    /// <summary>
    /// Implements generator.throw(error).
    /// </summary>
    public object @throw(object? error)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            // Spec: throw on completed generator rethrows.
            throw new JsThrownValueException(error);
        }

        scope.ResumeValue = null;
        scope.HasReturn = false;
        scope.ReturnValue = null;

        scope.HasResumeException = true;
        scope.ResumeException = error;

        try
        {
            return Closure.InvokeWithArgs(_step, _scopes, _args);
        }
        catch
        {
            scope.Done = true;
            throw;
        }
    }

    /// <summary>
    /// Implements generator.return(value).
    /// </summary>
    public object @return(object? value)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            return IteratorResult.Create(value, done: true);
        }

        scope.HasResumeException = false;
        scope.ResumeException = null;
        scope.ResumeValue = null;

        scope.HasReturn = true;
        scope.ReturnValue = value;

        try
        {
            return Closure.InvokeWithArgs(_step, _scopes, _args);
        }
        catch
        {
            scope.Done = true;
            throw;
        }
    }
}
