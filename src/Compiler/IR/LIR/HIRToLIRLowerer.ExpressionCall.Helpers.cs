using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private static readonly HashSet<string> NumericMathUnaryFastPathMethods = new(StringComparer.Ordinal)
    {
        "abs",
        "ceil",
        "round",
        "sqrt",
        "sin",
        "cos",
    };

    private static bool HasSpreadArguments(IReadOnlyList<HIRExpression> arguments)
        => arguments.Any(a => a is HIRSpreadElement);

    private static bool IsStableGlobalMathBinding(Symbol symbol)
        => symbol.Kind == BindingKind.Global
           && string.Equals(symbol.Name, "Math", StringComparison.Ordinal)
           && !symbol.BindingInfo.HasWrite;

    private static bool IsNumericMathUnaryFastPathMethod(string methodName)
        => NumericMathUnaryFastPathMethods.Contains(methodName);

    private TempVariable RequireObjectCoercible(TempVariable receiverTemp)
    {
        var checkedReceiver = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.RequireObjectCoercible),
            new[] { EnsureObject(receiverTemp) },
            checkedReceiver));
        DefineTempStorage(checkedReceiver, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return checkedReceiver;
    }

    private bool TryLowerCallArgumentsToArgsArray(IReadOnlyList<HIRExpression> arguments, out TempVariable argsArrayTemp)
    {
        argsArrayTemp = default;

        // Fast-path: no spread elements => build array directly from lowered temps.
        if (!HasSpreadArguments(arguments))
        {
            var callArgTemps = new List<TempVariable>(arguments.Count);
            foreach (var arg in arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                callArgTemps.Add(EnsureObject(argTemp));
            }

            argsArrayTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRBuildArray(callArgTemps, argsArrayTemp));
            DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
            return true;
        }

        // Spread-path: build a JavaScriptRuntime.Array, append elements in order (preserving iterator semantics),
        // then convert to object[] via List<T>.ToArray().

        // Seed with any leading non-spread args so the ctor capacity hint is non-zero.
        int prefixCount = 0;
        while (prefixCount < arguments.Count && arguments[prefixCount] is not HIRSpreadElement)
        {
            prefixCount++;
        }

        var prefixTemps = new List<TempVariable>(prefixCount);
        for (int i = 0; i < prefixCount; i++)
        {
            if (!TryLowerExpression(arguments[i], out var argTemp))
            {
                return false;
            }
            prefixTemps.Add(EnsureObject(argTemp));
        }

        var jsArgsListTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(prefixTemps, jsArgsListTemp));
        DefineTempStorage(jsArgsListTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        for (int i = prefixCount; i < arguments.Count; i++)
        {
            var arg = arguments[i];
            if (arg is HIRSpreadElement spread)
            {
                if (!TryLowerExpression(spread.Argument, out var spreadTemp))
                {
                    return false;
                }

                spreadTemp = EnsureObject(spreadTemp);
                _methodBodyIR.Instructions.Add(new LIRArrayPushRange(jsArgsListTemp, spreadTemp));
                continue;
            }

            if (!TryLowerExpression(arg, out var argTempVar))
            {
                return false;
            }

            argTempVar = EnsureObject(argTempVar);
            _methodBodyIR.Instructions.Add(new LIRArrayAdd(jsArgsListTemp, argTempVar));
        }

        argsArrayTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
            jsArgsListTemp,
            typeof(JavaScriptRuntime.Array),
            "ToArray",
            Array.Empty<TempVariable>(),
            argsArrayTemp));
        DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        return true;
    }

    private bool TryEvaluateCallArguments(IReadOnlyList<HIRExpression> arguments, int usedCount, out List<TempVariable> usedTemps)
    {
        usedTemps = new List<TempVariable>(Math.Max(usedCount, 0));

        var maxUsed = Math.Max(0, usedCount);
        if (arguments.Count < maxUsed)
        {
            maxUsed = arguments.Count;
        }

        for (int i = 0; i < arguments.Count; i++)
        {
            if (!TryLowerExpression(arguments[i], out var argTemp))
            {
                return false;
            }

            if (i < maxUsed)
            {
                usedTemps.Add(EnsureObject(argTemp));
            }
        }

        return true;
    }

    private Jroc.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForClassStaticMethod(
        Symbol classSymbol,
        MethodDefinition methodDef,
        string methodName,
        int declaredParamCount)
    {
        if (_scope == null)
        {
            return null;
        }

        var declaringScope = FindDeclaringScope(classSymbol.BindingInfo);
        if (declaringScope == null)
        {
            return null;
        }

        var root = declaringScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var declaringScopeName = declaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{declaringScope.GetQualifiedName()}";

        var callableName = JavaScriptCallableNaming.MakeClassMethodCallableName(classSymbol.Name, methodName);
        var location = Jroc.Services.TwoPhaseCompilation.SourceLocation.FromNode(methodDef);

        return new Jroc.Services.TwoPhaseCompilation.CallableId
        {
            Kind = Jroc.Services.TwoPhaseCompilation.CallableKind.ClassStaticMethod,
            DeclaringScopeName = declaringScopeName,
            Name = callableName,
            Location = location,
            JsParamCount = declaredParamCount,
            AstNode = methodDef
        };
    }

    private Jroc.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForCurrentClassStaticMethod(
        MethodDefinition methodDef,
        string methodName,
        int declaredParamCount)
    {
        if (_scope == null)
        {
            return null;
        }

        var classScope = _scope;
        while (classScope != null && classScope.Kind != ScopeKind.Class)
        {
            classScope = classScope.Parent;
        }

        if (classScope == null)
        {
            return null;
        }

        var declaringScope = classScope.Parent ?? classScope;
        var root = declaringScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var declaringScopeName = declaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{declaringScope.GetQualifiedName()}";

        var callableName = JavaScriptCallableNaming.MakeClassMethodCallableName(classScope.Name, methodName);
        var location = Jroc.Services.TwoPhaseCompilation.SourceLocation.FromNode(methodDef);

        return new Jroc.Services.TwoPhaseCompilation.CallableId
        {
            Kind = Jroc.Services.TwoPhaseCompilation.CallableKind.ClassStaticMethod,
            DeclaringScopeName = declaringScopeName,
            Name = callableName,
            Location = location,
            JsParamCount = declaredParamCount,
            AstNode = methodDef
        };
    }

    /// <summary>
    /// Builds an object[] scopes array for the current caller context.
    /// This is used for indirect calls where we cannot statically determine the callee scope chain.
    /// </summary>
    private bool TryBuildCurrentScopesArray(TempVariable resultTemp)
    {
        if (_environmentLayout == null || _environmentLayout.ScopeChain.Slots.Count == 0)
        {
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
            return true;
        }

        var slotSources = new List<ScopeSlotSource>(_environmentLayout.ScopeChain.Slots.Count);
        foreach (var slot in _environmentLayout.ScopeChain.Slots)
        {
            if (!TryMapScopeSlotToSource(slot, out var slotSource))
            {
                return false;
            }
            slotSources.Add(slotSource);
        }

        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(slotSources, resultTemp));
        return true;
    }
}
