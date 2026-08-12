using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // Prefer the existing fast-paths for statically known constructors.
        // If those don't apply, fall back to dynamic construction via JavaScriptRuntime.ObjectRuntime.ConstructValue.
        var calleeVar = newExpr.Callee as HIRVariableExpression;

        if (newExpr.Callee is HIRInitializedUserClassTypeExpression initializedClassExpr)
        {
            var classSemantics = initializedClassExpr.ClassScope.ClassSemantics;
            if (classSemantics == null)
            {
                return false;
            }

            if (!TryLowerExpression(initializedClassExpr, out var initializedClassValue))
            {
                return false;
            }

            return TryLowerNewUserDefinedClass(
                initializedClassExpr.RegistryClassName,
                initializedClassExpr.ClassScope,
                classSemantics,
                newExpr.Arguments,
                EnsureObject(initializedClassValue),
                out resultTempVar);
        }

        // User-defined class: `new ClassName(...)`
        // Note: top-level classes live in the global scope but still have a declaration node.
        if (calleeVar != null
            && TryGetClassSemantics(
                calleeVar.Name.BindingInfo,
                out var classScope,
                out var declaredClassSemantics))
        {
            if (declaredClassSemantics.IsExpression)
            {
                return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
            }

            if (!TryLowerExpression(calleeVar, out var classValue))
            {
                return false;
            }

            return TryLowerNewUserDefinedClass(
                declaredClassSemantics.RegistryClassName,
                classScope,
                declaredClassSemantics,
                newExpr.Arguments,
                EnsureObject(classValue),
                out resultTempVar);
        }

        var ctorName = calleeVar?.Name.Name;

        if (ctorName == null)
        {
            return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
        }

        if (calleeVar?.Name.Kind == BindingKind.Global
            && string.Equals(ctorName, "Function", StringComparison.Ordinal)
            && TryGetDynamicFunctionSyntaxErrorMessage(newExpr.Arguments, out var syntaxErrorMessage)
            && !string.IsNullOrWhiteSpace(syntaxErrorMessage))
        {
            return TryEmitThrownBuiltInError("SyntaxError", syntaxErrorMessage, out resultTempVar);
        }

        // PL3.3a: built-in Error types
        if (BuiltInErrorTypes.IsBuiltInErrorTypeName(ctorName))
        {
            if (newExpr.Arguments.Count > 2)
            {
                return false;
            }

            TempVariable? messageTemp = null;
            if (newExpr.Arguments.Count >= 1)
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var loweredMessage))
                {
                    return false;
                }
                messageTemp = EnsureObject(loweredMessage);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(ctorName, messageTemp, resultTempVar));
            DefineTempStorage(resultTempVar, GetBuiltInErrorStorage(ctorName));

            if (newExpr.Arguments.Count == 2)
            {
                if (!TryLowerExpression(newExpr.Arguments[1], out var optionsTemp))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                    nameof(JavaScriptRuntime.Error),
                    nameof(JavaScriptRuntime.Error.InstallCause),
                    new[]
                    {
                        EnsureObject(resultTempVar),
                        EnsureObject(optionsTemp)
                    }));
            }

            return true;
        }

        // PL3.3d: Array constructor semantics
        if (string.Equals(ctorName, "Array", StringComparison.Ordinal))
        {
            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Array", "Construct", argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // PL3.3e: String constructor sugar
        if (string.Equals(ctorName, "String", StringComparison.Ordinal))
        {
            return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
        }

        // PL3.3f: Boolean constructor object semantics
        if (string.Equals(ctorName, "Boolean", StringComparison.Ordinal))
        {
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            resultTempVar = CreateTempVariable();
            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject("Boolean", argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        if (string.Equals(ctorName, "Number", StringComparison.Ordinal))
        {
            return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
        }

        if (string.Equals(ctorName, "Object", StringComparison.Ordinal))
        {
            return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
        }

        if (string.Equals(ctorName, "Date", StringComparison.Ordinal))
        {
            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Date", "Construct", argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Date)));
            return true;
        }

        // PL3.3g: generic intrinsic constructor support (Date/RegExp/Set/Promise/Int32Array/etc.)
        var intrinsicType = _runtimeIntrinsicCatalog.TryGetIntrinsicObject(ctorName, out var intrinsic) && intrinsic != null
            ? intrinsic.Type
            : null;
        if (intrinsicType != null)
        {
            bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
            if (isStaticClass)
            {
                return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
            }

            if (newExpr.Arguments.Count > 3)
            {
                return false;
            }

            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(ctorName, argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, intrinsicType));
            return true;
        }

        // Dynamic/new-on-value fallback: supports patterns like
        //   const C = require('./lib'); new C(...)
        // and, in general, new expressions where the constructor is not statically known.
        return TryLowerDynamicNewExpression(newExpr, out resultTempVar);
    }

    private bool TryLowerDynamicNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(newExpr.Callee, out var ctorTemp))
        {
            return false;
        }
        ctorTemp = EnsureObject(ctorTemp);

        if (newExpr.Arguments.Any(argument => argument is HIRSpreadElement)
            || newExpr.Arguments.Count
                > JavaScriptRuntime.JsCallArguments.InlineCapacity)
        {
            if (!TryLowerCallArgumentsToArgsArray(
                    newExpr.Arguments,
                    out var argsArrayTemp))
            {
                return false;
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRConstructValue(
                    ctorTemp,
                    argsArrayTemp,
                    resultTempVar));
            DefineTempStorage(
                resultTempVar,
                new ValueStorage(
                    ValueStorageKind.Reference,
                    typeof(object)));
            return true;
        }

        var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
        foreach (var arg in newExpr.Arguments)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }
            argTemps.Add(EnsureObject(argTemp));
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(
            new LIRConstructValueFixed(
                ctorTemp,
                argTemps,
                resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryGetClassSemantics(
        BindingInfo binding,
        out Scope classScope,
        out TwoPhase.ClassSemantics classSemantics)
    {
        classScope = null!;
        classSemantics = null!;

        if (binding.ClassScope?.ClassSemantics is { } discoveredClassSemantics)
        {
            classScope = binding.ClassScope;
            classSemantics = discoveredClassSemantics;
            return true;
        }

        var declaringScope = FindDeclaringScope(binding);
        if (declaringScope == null)
        {
            return false;
        }

        classScope = declaringScope.Children.FirstOrDefault(scope =>
            scope.Kind == ScopeKind.Class
            && string.Equals(scope.Name, binding.Name, StringComparison.Ordinal))!;
        classSemantics = classScope?.ClassSemantics!;
        return classSemantics != null;
    }

    private bool TryLowerNewUserDefinedClass(
        string registryClassName,
        Scope classScope,
        TwoPhase.ClassSemantics classSemantics,
        IReadOnlyList<HIRExpression> args,
        TempVariable newTarget,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        bool needsScopes = DoesClassNeedParentScopes(classScope);

        // If the registered constructor ABI includes a leading scopes array (e.g., because the
        // class or its base class needs parent scopes), ensure call-sites pass it.
        if (_classRegistry != null
            && _classRegistry.TryGetConstructor(registryClassName, out _, out var ctorHasScopesParam, out _, out _))
        {
            needsScopes = ctorHasScopesParam;
        }
        TempVariable? scopesTemp = null;
        if (needsScopes)
        {
            scopesTemp = CreateTempVariable();
            if (!TryBuildScopesArrayForClassConstructor(classScope, scopesTemp.Value))
            {
                return false;
            }
            DefineTempStorage(scopesTemp.Value, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        }

        // Lower arguments (boxed)
        var argTemps = new List<TempVariable>(args.Count);
        foreach (var arg in args)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }
            argTemps.Add(EnsureObject(argTemp));
        }

        int minArgs = 0;
        int maxArgs = 0;
        var jsParamCount = classSemantics.Constructor.JsParamCount;
        if (_classRegistry != null
            && _classRegistry.TryGetConstructor(registryClassName, out _, out _, out var ctorMinArgs, out var ctorMaxArgs))
        {
            // For synthetic/implicit constructors there is no AST parameter list.
            // Use the registered constructor signature to decide how many args are accepted/padded.
            minArgs = ctorMinArgs;
            maxArgs = ctorMaxArgs;
            jsParamCount = ctorMaxArgs;
        }

        var className = classSemantics.Name;
        var ctorCallableId = classSemantics.Constructor;
        var isDerivedConstructor = classSemantics.IsDerived;
        var constructorScope = classScope.Children.FirstOrDefault(scope =>
            scope.Kind == ScopeKind.Function
            && string.Equals(scope.Name, "constructor", StringComparison.Ordinal));
        var parameterClrTypes = new Type?[jsParamCount];
        if (constructorScope != null)
        {
            foreach (var (index, clrType) in constructorScope.StableParameterClrTypes)
            {
                if (index >= 0 && index < parameterClrTypes.Length)
                {
                    parameterClrTypes[index] = clrType;
                }
            }
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewUserClass(
            ClassName: className,
            RegistryClassName: registryClassName,
            ConstructorCallableId: ctorCallableId,
            NewTarget: newTarget,
            NeedsScopes: needsScopes,
            ScopesArray: scopesTemp,
            MinArgCount: minArgs,
            MaxArgCount: maxArgs,
            IsDerivedConstructor: isDerivedConstructor,
            ParameterClrTypes: parameterClrTypes,
            Arguments: argTemps,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
