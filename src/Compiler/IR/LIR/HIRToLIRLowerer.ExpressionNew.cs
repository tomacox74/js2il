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
    private bool TryLowerNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // Prefer the existing fast-paths for statically known constructors.
        // If those don't apply, fall back to dynamic construction via JavaScriptRuntime.Object.ConstructValue.
        var calleeVar = newExpr.Callee as HIRVariableExpression;

        if (newExpr.Callee is HIRInitializedUserClassTypeExpression initializedClassExpr)
        {
            return TryLowerNewInitializedUserClass(initializedClassExpr, newExpr.Arguments, out resultTempVar);
        }

        // User-defined class: `new ClassName(...)`
        // Note: top-level classes live in the global scope but still have a declaration node.
        if (calleeVar != null && calleeVar.Name.BindingInfo.DeclarationNode is ClassDeclaration declaredClass)
        {
            return TryLowerNewUserDefinedClass(declaredClass, newExpr.Arguments, out resultTempVar);
        }

        if (calleeVar != null && TryLowerNewDirectFunctionConstructor(calleeVar, newExpr.Arguments, out resultTempVar))
        {
            return true;
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
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            TempVariable? messageTemp = null;
            if (newExpr.Arguments.Count == 1)
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var loweredMessage))
                {
                    return false;
                }
                messageTemp = EnsureObject(loweredMessage);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(ctorName, messageTemp, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
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
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(ctorName);
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

    private bool TryLowerNewDirectFunctionConstructor(
        HIRVariableExpression calleeVar,
        IReadOnlyList<HIRExpression> args,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (args.Any(static arg => arg is HIRSpreadElement))
        {
            return false;
        }

        var symbol = calleeVar.Name;
        if (symbol.BindingInfo.HasExplicitWrite)
        {
            return false;
        }

        if (symbol.BindingInfo.DeclarationNode is FunctionDeclaration { Async: true } or FunctionDeclaration { Generator: true })
        {
            return false;
        }

        TwoPhase.CallableId? callableId = symbol.Kind == BindingKind.Function
            ? TryCreateCallableIdForFunctionDeclaration(symbol)
            : null;
        Scope? bodyScope = null;

        if (callableId == null
            && !TryCreateCallableIdForConstInitializedFunctionExpression(symbol, allowThisBinding: true, out callableId, out bodyScope))
        {
            return false;
        }

        if (callableId == null
            || callableId.NeedsArgumentsObject
            || callableId.HasRestParameters)
        {
            return false;
        }

        if (!TryLowerExpression(calleeVar, out var constructorValueTemp))
        {
            return false;
        }
        constructorValueTemp = EnsureObject(constructorValueTemp);

        var argTemps = new List<TempVariable>(args.Count);
        foreach (var arg in args)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }

            argTemps.Add(EnsureObject(argTemp));
        }

        var scopesTemp = CreateTempVariable();
        var scopesBuilt = bodyScope != null
            ? TryBuildScopesArrayForClosureBinding(bodyScope, scopesTemp)
            : TryBuildScopesArrayForCallee(symbol, scopesTemp);
        if (!scopesBuilt)
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        var receiverTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.CreateFunctionConstructorInstance),
            new[] { constructorValueTemp },
            receiverTemp));
        DefineTempStorage(receiverTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var previousThisTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.SetCurrentThis),
            new[] { receiverTemp },
            previousThisTemp));
        DefineTempStorage(previousThisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var previousNewTargetTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.SetCurrentNewTarget),
            new[] { constructorValueTemp },
            previousNewTargetTemp));
        DefineTempStorage(previousNewTargetTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var tryStart = CreateLabel();
        var tryEnd = CreateLabel();
        var finallyStart = CreateLabel();
        var finallyEnd = CreateLabel();
        var end = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRLabel(tryStart));

        var callResultTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallFunctionWithNewTarget(
            symbol,
            scopesTemp,
            constructorValueTemp,
            argTemps,
            callResultTemp,
            callableId));
        DefineDirectCallResultStorage(callResultTemp, callableId, symbol.BindingInfo);

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.ResolveFunctionConstructorResult),
            new[] { receiverTemp, EnsureObject(callResultTemp) },
            resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        _methodBodyIR.Instructions.Add(new LIRLeave(end));
        _methodBodyIR.Instructions.Add(new LIRLabel(tryEnd));

        _methodBodyIR.Instructions.Add(new LIRLabel(finallyStart));
        var restoreNewTargetTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.SetCurrentNewTarget),
            new[] { previousNewTargetTemp },
            restoreNewTargetTemp));
        DefineTempStorage(restoreNewTargetTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRDiscardTemp(restoreNewTargetTemp));

        var restoreThisTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.SetCurrentThis),
            new[] { previousThisTemp },
            restoreThisTemp));
        DefineTempStorage(restoreThisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRDiscardTemp(restoreThisTemp));
        _methodBodyIR.Instructions.Add(new LIREndFinally());
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyEnd));
        _methodBodyIR.Instructions.Add(new LIRLabel(end));

        _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Finally,
            TryStartLabelId: tryStart,
            TryEndLabelId: tryEnd,
            HandlerStartLabelId: finallyStart,
            HandlerEndLabelId: finallyEnd));

        return true;
    }

    private bool TryLowerNewInitializedUserClass(
        HIRInitializedUserClassTypeExpression initializedClassExpr,
        IReadOnlyList<HIRExpression> args,
        out TempVariable resultTempVar)
    {
        foreach (var initStatement in initializedClassExpr.InitializationStatements)
        {
            if (!TryLowerStatement(initStatement))
            {
                resultTempVar = default;
                return false;
            }
        }

        return TryLowerNewUserDefinedClass(initializedClassExpr.RegistryClassName, initializedClassExpr.ClassScope, args, out resultTempVar);
    }

    private bool TryLowerDynamicNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(newExpr.Callee, out var ctorTemp))
        {
            return false;
        }
        ctorTemp = EnsureObject(ctorTemp);

        var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
        foreach (var arg in newExpr.Arguments)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }
            argTemps.Add(EnsureObject(argTemp));
        }

        var argsArrayTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(argTemps, argsArrayTemp));
        DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstructValue(ctorTemp, argsArrayTemp, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryGetRegistryClassNameForClassDeclaration(ClassDeclaration classDecl, out string registryClassName)
    {
        registryClassName = string.Empty;

        if (_scope == null)
        {
            return false;
        }

        var rootScope = _scope;
        while (rootScope.Parent != null)
        {
            rootScope = rootScope.Parent;
        }

        var classScope = FindScopeByDeclarationNode(classDecl, rootScope);
        if (classScope == null)
        {
            return false;
        }

        registryClassName = $"{(classScope.DotNetNamespace ?? "Classes")}.{(classScope.DotNetTypeName ?? classScope.Name)}";
        return true;
    }

    private bool TryLowerNewUserDefinedClass(ClassDeclaration classDecl, IReadOnlyList<HIRExpression> args, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Resolve the class scope to determine whether it needs parent scopes.
        var rootScope = _scope;
        while (rootScope.Parent != null)
        {
            rootScope = rootScope.Parent;
        }

        var classScope = FindScopeByDeclarationNode(classDecl, rootScope);
        if (classScope == null)
        {
            return false;
        }

        // Match ClassesGenerator registry key convention: "{ns}.{typeName}".
        // This allows IL emission to look up type/field handles for the class.
        var registryClassName = $"{(classScope.DotNetNamespace ?? "Classes")}.{(classScope.DotNetTypeName ?? classScope.Name)}";

        return TryLowerNewUserDefinedClass(registryClassName, classScope, args, out resultTempVar);
    }

    private bool TryLowerNewUserDefinedClass(
        string registryClassName,
        Scope classScope,
        IReadOnlyList<HIRExpression> args,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        if (classScope.AstNode is not (ClassDeclaration or ClassExpression))
        {
            return false;
        }

        var rootScope = _scope;
        while (rootScope.Parent != null)
        {
            rootScope = rootScope.Parent;
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

        // Compute ctor arg range from AST (min required vs max including defaults)
        var classBody = classScope.AstNode switch
        {
            ClassDeclaration classDeclaration => classDeclaration.Body,
            ClassExpression classExpression => classExpression.Body,
            _ => null
        };
        if (classBody == null)
        {
            return false;
        }

        var ctorMember = classBody.Body
            .OfType<MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

        int minArgs = 0;
        int maxArgs = 0;
        int jsParamCount = 0;
        if (ctorMember?.Value is FunctionExpression ctorFunc)
        {
            jsParamCount = ctorFunc.Params.Count;
            foreach (var p in ctorFunc.Params)
            {
                switch (p)
                {
                    case RestElement:
                        return false;
                    case AssignmentPattern:
                        maxArgs++;
                        break;
                    default:
                        minArgs++;
                        maxArgs++;
                        break;
                }
            }
        }
        else if (_classRegistry != null
            && _classRegistry.TryGetConstructor(registryClassName, out _, out _, out var ctorMinArgs, out var ctorMaxArgs))
        {
            // For synthetic/implicit constructors there is no AST parameter list.
            // Use the registered constructor signature to decide how many args are accepted/padded.
            minArgs = ctorMinArgs;
            maxArgs = ctorMaxArgs;
            jsParamCount = ctorMaxArgs;
        }

        // Build a stable CallableId for the constructor so LIR remains AST-free.
        // This mirrors CallableDiscovery.DiscoverClass.
        var moduleName = rootScope.Name;
        string declaringScopeName = (classScope.Parent == null || classScope.Parent.Kind == ScopeKind.Global)
            ? moduleName
            : $"{moduleName}/{classScope.Parent.GetQualifiedName()}";

        var className = classScope.AstNode switch
        {
            ClassDeclaration { Id: Identifier cid } => cid.Name,
            ClassExpression { Id: Identifier cid } => cid.Name,
            _ => classScope.Name
        };
        var ctorCallableId = new TwoPhase.CallableId
        {
            Kind = TwoPhase.CallableKind.ClassConstructor,
            DeclaringScopeName = declaringScopeName,
            Name = className,
            JsParamCount = jsParamCount,
            AstNode = null
        };
        var isDerivedConstructor = classScope.AstNode switch
        {
            ClassDeclaration classDeclaration => classDeclaration.SuperClass != null,
            ClassExpression classExpression => classExpression.SuperClass != null,
            _ => false
        };

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewUserClass(
            ClassName: className,
            RegistryClassName: registryClassName,
            ConstructorCallableId: ctorCallableId,
            NeedsScopes: needsScopes,
            ScopesArray: scopesTemp,
            MinArgCount: minArgs,
            MaxArgCount: maxArgs,
            IsDerivedConstructor: isDerivedConstructor,
            Arguments: argTemps,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
