using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
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

    private Js2IL.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForClassStaticMethod(
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
        var location = Js2IL.Services.TwoPhaseCompilation.SourceLocation.FromNode(methodDef);

        return new Js2IL.Services.TwoPhaseCompilation.CallableId
        {
            Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.ClassStaticMethod,
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
