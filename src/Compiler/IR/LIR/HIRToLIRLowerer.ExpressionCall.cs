using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.IL;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerCallExpression(HIRCallExpression callExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        bool hasSpreadArgs = callExpr.Arguments.Any(a => a is HIRSpreadElement);

        // Case 0: super(...) call in a derived class constructor.
        if (callExpr.Callee is HIRSuperExpression)
        {
            var usesLexicalReceiver = _callableKind == CallableKind.Function
                && _isLexicallyEnclosedByDerivedConstructor;
            if (!usesLexicalReceiver
                && (_callableKind != CallableKind.Constructor || !_isDerivedConstructor))
            {
                return false;
            }

            if (!usesLexicalReceiver && _superConstructorCalled)
            {
                return false;
            }

            // First try: user-defined base class in the ClassRegistry.
            if (_classRegistry != null
                && TryGetEnclosingBaseClassRegistryName(out var baseRegistryClassName)
                && baseRegistryClassName != null
                && _classRegistry.TryGetConstructor(baseRegistryClassName, out var baseCtorHandle, out var baseCtorHasScopesParam, out var _, out var baseCtorMaxParamCount))
            {
                var callArgs = new List<TempVariable>();
                var allJsArgs = new List<TempVariable>();

                // Lower JS arguments. All args are captured for AllJsArguments (arguments object);
                // only args within MaxParamCount are passed as formal parameters to the .NET method.
                for (int i = 0; i < callExpr.Arguments.Length; i++)
                {
                    if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                    {
                        return false;
                    }

                    var objArg = EnsureObject(argTemp);
                    allJsArgs.Add(objArg);
                    if (i < baseCtorMaxParamCount)
                    {
                        callArgs.Add(objArg);
                    }
                }

                // Pad missing args with undefined (null).
                while (callArgs.Count < baseCtorMaxParamCount)
                {
                    var undefTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefTemp));
                    DefineTempStorage(undefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    callArgs.Add(undefTemp);
                }

                _methodBodyIR.Instructions.Add(new LIRCallUserClassBaseConstructor(
                    baseRegistryClassName,
                    baseCtorHandle,
                    baseCtorHasScopesParam,
                    usesLexicalReceiver,
                    baseCtorMaxParamCount,
                    callArgs,
                    allJsArgs));
            }
            else
            {
                // Fallback: intrinsic base class (e.g., `extends Array`).
                // For intrinsics, we preserve JS argument list semantics (do not truncate/pad).
                var intrinsicName = GetEnclosingSuperClassIntrinsicName();
                if (intrinsicName != null)
                {
                    var callArgs = new List<TempVariable>();
                    for (int i = 0; i < callExpr.Arguments.Length; i++)
                    {
                        if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                        {
                            return false;
                        }
                        callArgs.Add(EnsureObject(argTemp));
                    }

                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicBaseConstructor(intrinsicName, callArgs, usesLexicalReceiver));
                }
                else
                {
                    if (!TryGetEnclosingSuperClassExpression(out var superClassExpression)
                        || superClassExpression == null
                        || !TryLowerExpression(superClassExpression, out var constructorTemp))
                    {
                        return false;
                    }

                    var callArgs = new List<TempVariable>();
                    for (int i = 0; i < callExpr.Arguments.Length; i++)
                    {
                        if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                        {
                            return false;
                        }

                        callArgs.Add(EnsureObject(argTemp));
                    }

                    var argsArrayTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRBuildArray(callArgs, argsArrayTemp));
                    DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                    _methodBodyIR.Instructions.Add(new LIRCallFunctionBaseConstructor(
                        EnsureObject(constructorTemp),
                        argsArrayTemp,
                        usesLexicalReceiver));
                }
            }

            // After super() the constructor is considered initialized.
            if (!usesLexicalReceiver)
            {
                _superConstructorCalled = true;
            }

            // In JS, super(...) returns the derived `this` value.
            _methodBodyIR.Instructions.Add(new LIRLoadThis(resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 1: User-defined function call (callee is a variable referencing a function)
        if (callExpr.Callee is HIRVariableExpression funcVarExpr)
        {
            var symbol = funcVarExpr.Name;

            if (!hasSpreadArgs
                && IsSafeInjectedCommonJsRequireBinding(symbol.BindingInfo)
                && callExpr.Arguments.Length > 0
                && callExpr.Arguments[0] is HIRLiteralExpression
                {
                    Kind: JavascriptType.String,
                    Value: string moduleSpecifier
                }
                && JavaScriptRuntime.Node.NodeModuleRegistry.TryGetModuleContractType(
                    moduleSpecifier,
                    out var contractType)
                && contractType != null)
            {
                if (!TryLowerExpression(funcVarExpr, out var requireValue)
                    || !TryEvaluateCallArguments(callExpr.Arguments, 1, out var requireArguments))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRCallRequire(
                    requireValue,
                    requireArguments[0],
                    resultTempVar,
                    contractType));
                DefineTempStorage(
                    resultTempVar,
                    new ValueStorage(ValueStorageKind.Reference, contractType));
                return true;
            }

            // PL8.1: Primitive conversion callables: String(x), Number(x), Boolean(x).
            // These are CallExpression forms (not NewExpression) and should lower to runtime conversions.
            // Semantics:
            // - No args: String() => "", Number() => 0, Boolean() => false
            // - Extra args are evaluated for side-effects and ignored
            if (symbol.Kind == BindingKind.Global)
            {
                var name = symbol.Name;
                if (string.Equals(name, "Function", StringComparison.Ordinal)
                    && TryGetDynamicFunctionSyntaxErrorMessage(callExpr.Arguments, out var syntaxErrorMessage)
                    && !string.IsNullOrWhiteSpace(syntaxErrorMessage))
                {
                    return TryEmitThrownBuiltInError("SyntaxError", syntaxErrorMessage, out resultTempVar);
                }

                if (string.Equals(name, "String", StringComparison.Ordinal)
                    || string.Equals(name, "Number", StringComparison.Ordinal)
                    || string.Equals(name, "Boolean", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var conversionArgs))
                    {
                        return false;
                    }

                    var firstArg = conversionArgs.Count > 0 ? conversionArgs[0] : (TempVariable?)null;

                    if (string.Equals(name, "String", StringComparison.Ordinal))
                    {
                        if (firstArg == null)
                        {
                            _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                            return true;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConvertToString(firstArg.Value, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;
                    }

                    if (string.Equals(name, "Number", StringComparison.Ordinal))
                    {
                        if (firstArg == null)
                        {
                            _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                            return true;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConvertToNumber(firstArg.Value, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;
                    }

                    // Boolean
                    if (firstArg == null)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;
                    }

                    _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(firstArg.Value, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

                // PL8.7 / #305: Callable-only intrinsics: Symbol([description]) and BigInt(value)
                if (string.Equals(name, "Symbol", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var symbolArgs))
                    {
                        return false;
                    }

                    if (symbolArgs.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Symbol", "Call", Array.Empty<TempVariable>(), resultTempVar));
                    }
                    else
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Symbol", "Call", new[] { symbolArgs[0] }, resultTempVar));
                    }

                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }

                if (string.Equals(name, "BigInt", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var bigIntArgs))
                    {
                        return false;
                    }

                    if (bigIntArgs.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("BigInt", "Call", Array.Empty<TempVariable>(), resultTempVar));
                    }
                    else
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("BigInt", "Call", new[] { bigIntArgs[0] }, resultTempVar));
                    }

                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(System.Numerics.BigInteger)));
                    return true;
                }

                var intrinsicInfo = _runtimeIntrinsicCatalog.TryGetIntrinsicObject(name, out var catalogIntrinsic) && catalogIntrinsic != null
                    ? catalogIntrinsic
                    : null;
                if (intrinsicInfo != null && intrinsicInfo.CallKind != JavaScriptRuntime.IntrinsicCallKind.None)
                {
                    switch (intrinsicInfo.CallKind)
                    {
                        case JavaScriptRuntime.IntrinsicCallKind.BuiltInError:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var errorArgs))
                                {
                                    return false;
                                }

                                var messageTemp = errorArgs.Count > 0 ? errorArgs[0] : (TempVariable?)null;

                                _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(intrinsicInfo.Name, messageTemp, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ArrayConstruct:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, callExpr.Arguments.Length, out var argTemps))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicInfo.Name, "Construct", argTemps, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ObjectConstruct:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var objectArgs))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicInfo.Name, "Construct", objectArgs, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.DateToString:
                            {
                                // ECMAScript Date() called as a function ignores all arguments and
                                // returns the current date/time as a string. Arguments are still
                                // evaluated for side effects, but none are passed to the constructor.
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 0, out var _))
                                {
                                    return false;
                                }

                                var dateTemp = CreateTempVariable();
                                _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(intrinsicInfo.Name, Array.Empty<TempVariable>(), dateTemp));
                                DefineTempStorage(dateTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Date)));

                                _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                                    dateTemp,
                                    typeof(JavaScriptRuntime.Date),
                                    nameof(JavaScriptRuntime.Date.toISOString),
                                    Array.Empty<TempVariable>(),
                                    resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ConstructorLike:
                            {
                                var maxUsed = Math.Min(callExpr.Arguments.Length, 2);
                                if (!TryEvaluateCallArguments(callExpr.Arguments, maxUsed, out var argTemps))
                                {
                                    return false;
                                }

                                if (string.Equals(intrinsicInfo.Name, "RegExp", StringComparison.OrdinalIgnoreCase))
                                {
                                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                                        "RegExp",
                                        "Call",
                                        argTemps,
                                        resultTempVar));
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    return true;
                                }

                                _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(intrinsicInfo.Name, argTemps, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }
                    }
                }
            }

            // Case 1.0: Intrinsic global function call (e.g., setTimeout(...)).
            // These are exposed as public static methods on JavaScriptRuntime.GlobalThis.
            // We lower them directly rather than trying to load them as a value.
            if (symbol.Kind == BindingKind.Global)
            {
                var globalFunctionName = symbol.Name;

                // PL8.1: Primitive conversion callables: String(x), Number(x), Boolean(x)
                // Distinct from `new String(...)` sugar handled in NewExpression lowering.
                if (string.Equals(globalFunctionName, "String", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var args))
                    {
                        return false;
                    }

                    // String() with no args returns empty string.
                    if (args.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;
                    }

                    var source = EnsureObject(args[0]);
                    _methodBodyIR.Instructions.Add(new LIRConvertToString(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Number", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var args))
                    {
                        return false;
                    }

                    // Number() with no args returns +0.
                    if (args.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;
                    }

                    // Fast path: if the argument is already an unboxed double, Number() is a no-op.
                    // This avoids a redundant box+ToNumber round-trip when the value was previously
                    // proven numeric (e.g. flow-sensitive refinement after Number(x) assignment).
                    var argStorage = GetTempStorage(args[0]);
                    if (argStorage.Kind == ValueStorageKind.UnboxedValue && argStorage.ClrType == typeof(double))
                    {
                        _methodBodyIR.Instructions.Add(new LIRCopyTemp(args[0], resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;
                    }

                    var source = EnsureObject(args[0]);
                    _methodBodyIR.Instructions.Add(new LIRConvertToNumber(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Boolean", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var args))
                    {
                        return false;
                    }

                    // Boolean() with no args returns false.
                    if (args.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;
                    }

                    var source = EnsureObject(args[0]);
                    _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Symbol", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var args))
                    {
                        return false;
                    }

                    if (args.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Symbol", "Call", Array.Empty<TempVariable>(), resultTempVar));
                    }
                    else
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Symbol", "Call", new[] { args[0] }, resultTempVar));
                    }

                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "BigInt", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var args))
                    {
                        return false;
                    }

                    if (args.Count == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("BigInt", "Call", Array.Empty<TempVariable>(), resultTempVar));
                    }
                    else
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("BigInt", "Call", new[] { args[0] }, resultTempVar));
                    }

                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(System.Numerics.BigInteger)));
                    return true;
                }

                var gvType = typeof(JavaScriptRuntime.GlobalThis);
                var gvMethod = gvType.GetMethod(
                    globalFunctionName,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                if (gvMethod != null)
                {
                    var argTemps = new List<TempVariable>();
                    foreach (var arg in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(arg, out var argTemp))
                        {
                            return false;
                        }
                        argTemps.Add(EnsureObject(argTemp));
                    }

                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicGlobalFunction(globalFunctionName, argTemps, resultTempVar));
                    if (gvMethod.ReturnType == typeof(double))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    }
                    else if (gvMethod.ReturnType == typeof(bool))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    }
                    else if (gvMethod.ReturnType == typeof(string))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                    }
                    else
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }
                    return true;
                }
            }

            if (!hasSpreadArgs
                && TryCreateCallableIdForConstInitializedArrow(symbol, out var constArrowCallableId, out var constArrowScope))
            {
                var constArrowArguments = new List<TempVariable>(callExpr.Arguments.Length);
                foreach (var arg in callExpr.Arguments)
                {
                    if (!TryLowerExpression(arg, out var argTemp))
                    {
                        return false;
                    }
                    constArrowArguments.Add(EnsureObject(argTemp));
                }

                var constArrowScopesTemp = CreateTempVariable();
                if (!TryBuildScopesArrayForClosureBinding(constArrowScope, constArrowScopesTemp))
                {
                    return false;
                }
                DefineTempStorage(constArrowScopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, constArrowScopesTemp, constArrowArguments, resultTempVar, constArrowCallableId));
                DefineDirectCallResultStorage(resultTempVar, constArrowCallableId, symbol.BindingInfo);
                return true;
            }

            var callableId = TryCreateCallableIdForFunctionDeclaration(symbol);

            // Strict bare calls must preserve undefined `this`; route them through the function-value
            // dispatch path instead of the direct static-call fast path.
            // Non-function bindings also use runtime dispatch (e.g., locals/consts holding closures).
            if (symbol.Kind != BindingKind.Function
                || callableId?.HasRestrictedFunctionProperties == true)
            {
                // Lower callee value
                if (!TryLowerExpression(funcVarExpr, out var calleeTemp))
                {
                    return false;
                }
                calleeTemp = EnsureObject(calleeTemp);

                // Build a scopes array for the current context. Bound closures ignore the passed scopes,
                // but unbound function values still require a scopes array.
                var scopesTemp = CreateTempVariable();
                if (!TryBuildCurrentScopesArray(scopesTemp))
                {
                    return false;
                }
                DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                // Check if we can use arity-specific instruction (no spread, 0-3 args)
                if (!HasSpreadArguments(callExpr.Arguments) && callExpr.Arguments.Length <= 3)
                {
                    // Lower arguments individually
                    var argTemps = new List<TempVariable>(callExpr.Arguments.Length);
                    foreach (var arg in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(arg, out var argTemp))
                        {
                            return false;
                        }
                        argTemps.Add(EnsureObject(argTemp));
                    }

                    // Emit arity-specific instruction
                    LIRInstruction callInstr = callExpr.Arguments.Length switch
                    {
                        0 => new LIRCallFunctionValue0(calleeTemp, scopesTemp, resultTempVar),
                        1 => new LIRCallFunctionValue1(calleeTemp, scopesTemp, argTemps[0], resultTempVar),
                        2 => new LIRCallFunctionValue2(calleeTemp, scopesTemp, argTemps[0], argTemps[1], resultTempVar),
                        3 => new LIRCallFunctionValue3(calleeTemp, scopesTemp, argTemps[0], argTemps[1], argTemps[2], resultTempVar),
                        _ => throw new InvalidOperationException("Unexpected arity")
                    };
                    _methodBodyIR.Instructions.Add(callInstr);
                }
                else
                {
                    // Fall back to array-based call for >3 args or spread
                    if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTemp))
                    {
                        return false;
                    }

                    _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(calleeTemp, scopesTemp, argsArrayTemp, resultTempVar));
                }

                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }

            // Spread in call arguments requires runtime args array construction.
            if (hasSpreadArgs)
            {
                if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTemp))
                {
                    return false;
                }

                var scopesTempForSpread = CreateTempVariable();
                if (!TryBuildScopesArrayForCallee(symbol, scopesTempForSpread))
                {
                    return false;
                }
                DefineTempStorage(scopesTempForSpread, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallFunctionWithArgsArray(symbol, scopesTempForSpread, argsArrayTemp, resultTempVar, callableId));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }

            // Lower all arguments first (no spread)
            var arguments = new List<TempVariable>(callExpr.Arguments.Length);
            foreach (var arg in callExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                // Ensure arguments are boxed as object for function calls
                arguments.Add(EnsureObject(argTemp));
            }

            // Build the scopes array for the callee
            var scopesTempVar = CreateTempVariable();
            if (!TryBuildScopesArrayForCallee(symbol, scopesTempVar))
            {
                return false;
            }
            DefineTempStorage(scopesTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            // Emit the function call with arguments
            _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, scopesTempVar, arguments, resultTempVar, callableId));
            DefineDirectCallResultStorage(resultTempVar, callableId, symbol.BindingInfo);

            return true;
        }

        // Case 1b: Indirect call where the callee is an expression value (e.g., IIFE:
        // (function() { ... })(), (() => 1)(), or getFn()()).
        // Exclude property-access calls here to avoid accidentally breaking method-call semantics
        // that are handled by intrinsic/typed-member lowering below.
        if (callExpr.Callee is not HIRVariableExpression
            && callExpr.Callee is not HIRPropertyAccessExpression
            && callExpr.Callee is not HIRIndexAccessExpression)
        {
            if (!TryLowerExpression(callExpr.Callee, out var calleeTemp))
            {
                return false;
            }
            calleeTemp = EnsureObject(calleeTemp);

            if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTemp))
            {
                return false;
            }

            var scopesTemp = CreateTempVariable();
            if (!TryBuildCurrentScopesArray(scopesTemp))
            {
                return false;
            }
            DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(calleeTemp, scopesTemp, argsArrayTemp, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        if (callExpr.Callee is HIRIndexAccessExpression calleeIndexAccess)
        {
            if (!TryLowerExpression(calleeIndexAccess.Object, out var computedReceiverTemp))
            {
                return false;
            }

            computedReceiverTemp = RequireObjectCoercible(computedReceiverTemp);

            if (!TryLowerExpression(calleeIndexAccess.Index, out var propertyKeyTemp))
            {
                return false;
            }

            if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var computedArgsArrayTemp))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.CallComputedMember),
                new[]
                {
                    EnsureObject(computedReceiverTemp),
                    EnsureObject(propertyKeyTemp),
                    computedArgsArrayTemp
                },
                resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 2: Property access call (e.g., console.log, Array.isArray, Math.abs)
        if (callExpr.Callee is not HIRPropertyAccessExpression calleePropAccess)
        {
            return false;
        }

        // Case 2.0: super.m(...) call in a derived class method.
        if (_classRegistry != null
            && calleePropAccess.Object is HIRSuperExpression
            && TryGetEnclosingBaseClassRegistryName(out var baseClass)
            && baseClass != null
            && _classRegistry.TryGetMethod(baseClass, calleePropAccess.PropertyName, out var baseMethodHandle, out _, out var baseReturnClrType, out var baseReturnTypeHandle, out var baseHasScopesParam, out _, out var baseMaxParamCount))
        {
            var argTemps = new List<TempVariable>();
            foreach (var argExpr in callExpr.Arguments)
            {
                if (!TryLowerExpression(argExpr, out var argTempVar))
                {
                    return false;
                }

                argTemps.Add(EnsureObject(argTempVar));
            }

            _methodBodyIR.Instructions.Add(new LIRCallUserClassBaseInstanceMethod(
                baseClass,
                calleePropAccess.PropertyName,
                baseMethodHandle,
                baseHasScopesParam,
                baseMaxParamCount,
                argTemps,
                resultTempVar));

            if (!baseReturnTypeHandle.IsNil)
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object), baseReturnTypeHandle));
            }
            else if (baseReturnClrType == typeof(double))
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            }
            else if (baseReturnClrType == typeof(bool))
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            }
            else if (baseReturnClrType == typeof(string))
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            }
            else
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }

            return true;
        }

        // Eligible object-literal function member call. Resolve the member through the
        // generated getter, then invoke the returned closure using the existing function-value
        // call instructions. Shape analysis only permits this when the member is never replaced
        // and its body does not observe method-call `this`.
        if (calleePropAccess.Object is HIRVariableExpression { Name.BindingInfo.IsCaptured: true }
            && TryGetInferredObjectLiteralMember(
                calleePropAccess.Object,
                calleePropAccess.PropertyName,
                out var inferredShape,
                out var inferredMember,
                allowVarBinding: true)
            && inferredMember.IsFunction)
        {
            if (!TryLowerExpression(calleePropAccess.Object, out var inferredReceiver))
            {
                return false;
            }

            // A var binding may still be undefined before its initializer. Preserve the generic
            // member-call TypeError before casting to the generated object-literal type.
            inferredReceiver = RequireObjectCoercible(inferredReceiver);

            var functionValue = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetInferredMember(
                inferredShape,
                inferredMember.Name,
                EnsureObject(inferredReceiver),
                functionValue));
            DefineTempStorage(functionValue, GetInferredMemberStorage(inferredMember));

            var scopesTemp = CreateTempVariable();
            if (!TryBuildCurrentScopesArray(scopesTemp))
            {
                return false;
            }
            DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            if (!hasSpreadArgs && callExpr.Arguments.Length <= 3)
            {
                var argTemps = new List<TempVariable>(callExpr.Arguments.Length);
                foreach (var arg in callExpr.Arguments)
                {
                    if (!TryLowerExpression(arg, out var argTemp))
                    {
                        return false;
                    }
                    argTemps.Add(EnsureObject(argTemp));
                }

                LIRInstruction callInstruction = callExpr.Arguments.Length switch
                {
                    0 => new LIRCallFunctionValue0(functionValue, scopesTemp, resultTempVar),
                    1 => new LIRCallFunctionValue1(functionValue, scopesTemp, argTemps[0], resultTempVar),
                    2 => new LIRCallFunctionValue2(functionValue, scopesTemp, argTemps[0], argTemps[1], resultTempVar),
                    3 => new LIRCallFunctionValue3(functionValue, scopesTemp, argTemps[0], argTemps[1], argTemps[2], resultTempVar),
                    _ => throw new InvalidOperationException("Unexpected arity")
                };
                _methodBodyIR.Instructions.Add(callInstruction);
            }
            else
            {
                if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTemp))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(
                    new LIRCallFunctionValue(functionValue, scopesTemp, argsArrayTemp, resultTempVar));
            }

            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 2b: Intrinsic static method call (e.g., Array.isArray, Math.abs, JSON.parse)
        // Check if the object is a global variable that maps to an intrinsic type
        if (calleePropAccess.Object is HIRVariableExpression calleeGlobalVar &&
            calleeGlobalVar.Name.Kind == BindingKind.Global)
        {
            var intrinsicName = calleeGlobalVar.Name.Name;
            var methodName = calleePropAccess.PropertyName;
            var canUseIntrinsicStatic = !string.Equals(intrinsicName, "Math", StringComparison.Ordinal)
                || IsStableGlobalMathBinding(calleeGlobalVar.Name);

            // Try to resolve the intrinsic type via IntrinsicObjectRegistry
            var intrinsicType = _runtimeIntrinsicCatalog.TryGetIntrinsicObject(intrinsicName, out var intrinsic) && intrinsic != null
                ? intrinsic.Type
                : null;
            if (intrinsicType != null && canUseIntrinsicStatic)
            {
                // Check if there's a matching static method
                var staticMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (staticMethods.Count > 0)
                {
                    // Choose the same overload we will emit in IL (see LIRToILCompiler.EmitIntrinsicStaticCall)
                    var argCount = callExpr.Arguments.Count();

                    bool ExactArityMatch(System.Reflection.MethodInfo mi)
                    {
                        var ps = mi.GetParameters();
                        return ps.Length == argCount
                            && !(ps.Length == 1 && ps[0].ParameterType == typeof(object[]));
                    }

                    bool ParamsArrayMatch(System.Reflection.MethodInfo mi)
                    {
                        var ps = mi.GetParameters();
                        return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                    }

                    var chosen = staticMethods.Find(ExactArityMatch)
                        ?? staticMethods.Find(ParamsArrayMatch);
                    // If we can't select a compatible overload for the intrinsic static call,
                    // fall back to generic member-dispatch below.
                    if (chosen != null)
                    {
                        if (!hasSpreadArgs
                            && string.Equals(intrinsicName, "Math", StringComparison.Ordinal)
                            && callExpr.Arguments.Length == 1
                            && IsNumericMathUnaryFastPathMethod(methodName))
                        {
                            if (!TryLowerExpression(callExpr.Arguments[0], out var mathArgTemp))
                            {
                                return false;
                            }

                            var mathArgStorage = GetTempStorage(mathArgTemp);
                            var mathArg = (mathArgStorage.Kind == ValueStorageKind.UnboxedValue
                                           && mathArgStorage.ClrType == typeof(double))
                                ? mathArgTemp
                                : EnsureObject(mathArgTemp);

                            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicName, methodName, new[] { mathArg }, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                            return true;
                        }

                        if (hasSpreadArgs)
                        {
                            // Spread call-sites must route through an args array.
                            // Only support this optimization when the intrinsic exposes a params object[] overload.
                            if (ParamsArrayMatch(chosen))
                            {
                                if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTemp))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticWithArgsArray(intrinsicName, methodName, argsArrayTemp, resultTempVar));

                                // Track the correct CLR type to prevent invalid IL (e.g., storing bool into an object local).
                                var retType = chosen.ReturnType;
                                if (retType == typeof(void))
                                {
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                }
                                else if (retType.IsValueType)
                                {
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, retType));
                                }
                                else
                                {
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, retType));
                                }
                                return true;
                            }

                            // No compatible params-array overload; fall back to generic member-dispatch below.
                        }
                        else
                        {
                            // Lower all arguments
                            var staticArgTemps = new List<TempVariable>();
                            foreach (var argExpr in callExpr.Arguments)
                            {
                                if (!TryLowerExpression(argExpr, out var argTempVar))
                                {
                                    return false;
                                }
                                argTempVar = EnsureObject(argTempVar);
                                staticArgTemps.Add(argTempVar);
                            }

                            // Emit the intrinsic static call
                            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicName, methodName, staticArgTemps, resultTempVar));

                            // Track the correct CLR type to prevent invalid IL (e.g., storing bool into an object local).
                            var retType = chosen.ReturnType;
                            if (retType == typeof(void))
                            {
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            }
                            else if (retType.IsValueType)
                            {
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, retType));
                            }
                            else
                            {
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, retType));
                            }
                            return true;
                        }
                    }
                }
            }
        }

        // Case 2b.2: User-defined class static method call (e.g., Greeter.helloWorld()).
        // If the receiver is a class identifier (ClassDeclaration binding) and the member is a static method,
        // emit a direct call to the declared method token via CallableRegistry.
        if ((calleePropAccess.Object is HIRThisExpression { StaticClassRegistryName: not null }
                || (_callableKind == CallableKind.ClassStaticMethod && calleePropAccess.Object is HIRThisExpression))
            && _scope != null)
        {
            var classScope = _scope;
            while (classScope != null && classScope.Kind != ScopeKind.Class)
            {
                classScope = classScope.Parent;
            }

            var classBody = classScope?.AstNode switch
            {
                ClassDeclaration enclosingClassDecl => enclosingClassDecl.Body,
                ClassExpression enclosingClassExpr => enclosingClassExpr.Body,
                _ => null
            };

            if (classBody != null)
            {
                var memberName = calleePropAccess.PropertyName;
                var member = classBody.Body
                    .OfType<MethodDefinition>()
                    .FirstOrDefault(m =>
                        m.Static
                        && string.Equals(ClassElementNames.GetMethodRegistryName(m), memberName, StringComparison.Ordinal));

                if (member?.Value is FunctionExpression memberFunc)
                {
                    var callableId = TryCreateCallableIdForCurrentClassStaticMethod(member, memberName, memberFunc.Params.Count);
                    if (callableId == null)
                    {
                        return false;
                    }

                    TempVariable? scopesArgTemp = null;
                    bool needsScopesArg = memberFunc.Async
                        || memberFunc.Generator
                        || (memberFunc.Body != null && ContainsYieldExpression(memberFunc.Body, memberFunc));
                    if (needsScopesArg)
                    {
                        scopesArgTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), scopesArgTemp.Value));
                        DefineTempStorage(scopesArgTemp.Value, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                    }

                    var callArgTemps = new List<TempVariable>(memberFunc.Params.Count + (scopesArgTemp.HasValue ? 2 : 0));

                    if (scopesArgTemp.HasValue)
                    {
                        callArgTemps.Add(scopesArgTemp.Value);

                        var newTargetUndefTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstUndefined(newTargetUndefTemp));
                        DefineTempStorage(newTargetUndefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        callArgTemps.Add(newTargetUndefTemp);
                    }

                    for (int i = 0; i < callExpr.Arguments.Length; i++)
                    {
                        if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                        {
                            return false;
                        }

                        argTemp = EnsureObject(argTemp);

                        if (i < memberFunc.Params.Count)
                        {
                            callArgTemps.Add(argTemp);
                        }
                    }

                    var expectedArgs = memberFunc.Params.Count + (scopesArgTemp.HasValue ? 2 : 0);
                    while (callArgTemps.Count < expectedArgs)
                    {
                        var undefTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefTemp));
                        DefineTempStorage(undefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        callArgTemps.Add(undefTemp);
                    }

                    if (_callableKind == CallableKind.ClassStaticMethod
                        && calleePropAccess.Object is HIRThisExpression { StaticClassRegistryName: null }
                        && member.Key is PrivateIdentifier)
                    {
                        var receiverTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRLoadThis(receiverTemp));
                        DefineTempStorage(receiverTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        var ownerTypeTemp = CreateTempVariable();
                        var registryClassName = $"{(classScope!.DotNetNamespace ?? "Classes")}.{(classScope.DotNetTypeName ?? classScope.Name)}";
                        _methodBodyIR.Instructions.Add(new LIRGetUserClassType(registryClassName, ownerTypeTemp));
                        DefineTempStorage(ownerTypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));

                        var validationTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.ValidateDirectClassPrivateMethodReceiver),
                            Arguments: new[] { EnsureObject(receiverTemp), EnsureObject(ownerTypeTemp) },
                            Result: validationTemp));
                        DefineTempStorage(validationTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }

                    _methodBodyIR.Instructions.Add(new LIRCallDeclaredCallable(callableId, callArgTemps, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }
            }
        }

        if (calleePropAccess.Object is HIRVariableExpression classVarExpr &&
            classVarExpr.Name.BindingInfo.DeclarationNode is ClassDeclaration classDecl)
        {
            var memberName = calleePropAccess.PropertyName;

            var member = classDecl.Body.Body
                .OfType<MethodDefinition>()
                .FirstOrDefault(m =>
                    m.Static &&
                    m.Key is Identifier kid &&
                    string.Equals(kid.Name, memberName, StringComparison.Ordinal));

            if (member?.Value is FunctionExpression memberFunc)
            {
                // Create a CallableId that matches CallableDiscovery conventions.
                var callableId = TryCreateCallableIdForClassStaticMethod(classVarExpr.Name, member, memberName, memberFunc.Params.Count);
                if (callableId == null)
                {
                    return false;
                }

                // Resumable static class methods (async/generator) follow the jroc calling convention and
                // require a leading scopes array.
                // Use an ABI-compatible empty scopes array (1-element array with null) for now.
                TempVariable? scopesArgTemp = null;
                bool needsScopesArg = memberFunc.Async
                    || memberFunc.Generator
                    || (memberFunc.Body != null && ContainsYieldExpression(memberFunc.Body, memberFunc));
                if (needsScopesArg)
                {
                    scopesArgTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), scopesArgTemp.Value));
                    DefineTempStorage(scopesArgTemp.Value, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                }

                // Lower all arguments (evaluate extras for side effects, but only pass up to declared param count).
                var declaredParamCount = memberFunc.Params.Count;
                var callArgTemps = new List<TempVariable>(declaredParamCount + (scopesArgTemp.HasValue ? 2 : 0));

                if (scopesArgTemp.HasValue)
                {
                    callArgTemps.Add(scopesArgTemp.Value);

                    var newTargetUndefTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(newTargetUndefTemp));
                    DefineTempStorage(newTargetUndefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    callArgTemps.Add(newTargetUndefTemp);
                }

                for (int i = 0; i < callExpr.Arguments.Length; i++)
                {
                    if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                    {
                        return false;
                    }

                    argTemp = EnsureObject(argTemp);

                    if (i < declaredParamCount)
                    {
                        callArgTemps.Add(argTemp);
                    }
                    // else: evaluated for side effects, result intentionally ignored
                }

                // Pad missing args with undefined (null) to match the declared signature.
                var expectedArgs = declaredParamCount + (scopesArgTemp.HasValue ? 2 : 0);
                while (callArgTemps.Count < expectedArgs)
                {
                    var undefTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefTemp));
                    DefineTempStorage(undefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    callArgTemps.Add(undefTemp);
                }

                if (!TryLowerExpression(calleePropAccess.Object, out var staticReceiverTemp))
                {
                    return false;
                }

                var previousThisTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                    nameof(JavaScriptRuntime.RuntimeServices.SetCurrentThis),
                    new[] { EnsureObject(staticReceiverTemp) },
                    previousThisTemp));
                DefineTempStorage(previousThisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                _methodBodyIR.Instructions.Add(new LIRCallDeclaredCallable(callableId, callArgTemps, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                var restoreThisTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                    nameof(JavaScriptRuntime.RuntimeServices.SetCurrentThis),
                    new[] { EnsureObject(previousThisTemp) },
                    restoreThisTemp));
                DefineTempStorage(restoreThisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }
        }

        // Lower receiver once for instance/member call cases below.
        // IMPORTANT: Do not lower the receiver more than once; it may have side effects (e.g. promise chaining).
        if (!TryLowerExpression(calleePropAccess.Object, out var receiverTempVar))
        {
            return false;
        }

        var nodeContractType = TryGetNodeModuleContractType(GetTempStorage(receiverTempVar).ClrType);
        if (nodeContractType == null
            && calleePropAccess.Object is HIRVariableExpression contractReceiver
            && contractReceiver.Name.BindingInfo.IsStableType)
        {
            nodeContractType = TryGetNodeModuleContractType(contractReceiver.Name.BindingInfo.ClrType);
        }

        if (!hasSpreadArgs
            && nodeContractType != null
            && LIRToILCompiler.ResolveTypedInstanceMethodOverload(
                nodeContractType,
                calleePropAccess.PropertyName,
                callExpr.Arguments.Length) is { } contractMethod)
        {
            var contractArguments = new List<TempVariable>(callExpr.Arguments.Length);
            foreach (var argument in callExpr.Arguments)
            {
                if (!TryLowerExpression(argument, out var argumentTemp))
                {
                    return false;
                }

                contractArguments.Add(EnsureObject(argumentTemp));
            }

            if (CanDirectlyCallNodeContractMethod(contractMethod, contractArguments))
            {
                _methodBodyIR.Instructions.Add(new LIRCallNodeModuleContractMember(
                    receiverTempVar,
                    nodeContractType,
                    calleePropAccess.PropertyName,
                    calleePropAccess.PropertyName,
                    IsPropertyGet: false,
                    RequiresOverrideGuard:
                        calleePropAccess.Object is not HIRVariableExpression contractBindingReceiver
                        || !contractBindingReceiver.Name.BindingInfo.CanSkipNodeModuleOverrideGuard,
                    contractArguments,
                    resultTempVar));
            }
            else
            {
                EmitDynamicMemberCall(
                    receiverTempVar,
                    calleePropAccess.PropertyName,
                    contractArguments,
                    resultTempVar);
            }

            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 2a.0: Stable string local/member receiver for substring(...) calls.
        // This avoids late-bound ObjectRuntime.CallMember* dispatch in hot loops (e.g., dromaeo generateTestStrings).
        if (!hasSpreadArgs
            && callExpr.Arguments.Length <= 2
            && string.Equals(calleePropAccess.PropertyName, "substring", StringComparison.Ordinal)
            && calleePropAccess.Object is HIRVariableExpression receiverVarExpr
            && receiverVarExpr.Name.BindingInfo.IsStableType
            && receiverVarExpr.Name.BindingInfo.ClrType == typeof(string))
        {
            // Ensure the receiver temp is strongly typed as string for the intrinsic static call signature.
            if (GetTempStorage(receiverTempVar).Kind != ValueStorageKind.Reference
                || GetTempStorage(receiverTempVar).ClrType != typeof(string))
            {
                var receiverAsString = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConvertToString(EnsureObject(receiverTempVar), receiverAsString));
                DefineTempStorage(receiverAsString, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                receiverTempVar = receiverAsString;
            }

            var substringArgs = new List<TempVariable>(1 + callExpr.Arguments.Length) { receiverTempVar };
            foreach (var argExpr in callExpr.Arguments)
            {
                if (!TryLowerExpression(argExpr, out var argTemp))
                {
                    return false;
                }

                substringArgs.Add(EnsureObject(argTemp));
            }

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                "String",
                "substring",
                substringArgs,
                resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        // Case 2a: Typed Array instance method calls (e.g., arr.join(), arr.push(...)).
        // If the receiver CLR type is known to be JavaScriptRuntime.Array, emit a typed instance call.
        {
            var receiverStorage = GetTempStorage(receiverTempVar);
            if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
            {
                if (hasSpreadArgs)
                {
                    // Spread argument count is not statically known; fall back to runtime member-dispatch.
                }
                else
                {
                    var arrayArgTemps = new List<TempVariable>();
                    foreach (var argExpr in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(argExpr, out var argTempVar))
                        {
                            return false;
                        }
                        arrayArgTemps.Add(EnsureObject(argTempVar));
                    }

                    if (TryResolveTypedInstanceCallReturnClrType(
                            typeof(JavaScriptRuntime.Array),
                            calleePropAccess.PropertyName,
                            arrayArgTemps.Count,
                            out var returnClrType))
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                            receiverTempVar,
                            typeof(JavaScriptRuntime.Array),
                            calleePropAccess.PropertyName,
                            arrayArgTemps,
                            resultTempVar));

                        if (returnClrType == typeof(bool))
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                            return true;
                        }

                        if (returnClrType == typeof(double))
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                            return true;
                        }

                        if (returnClrType == typeof(string))
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                            return true;
                        }

                        // Track a more precise runtime type when we know it, so chained calls can lower.
                        // Example: arr.slice(...).join(',') requires the result of slice() to be treated as an Array receiver.
                        if (returnClrType == typeof(JavaScriptRuntime.Array))
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
                            return true;
                        }

                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        return true;
                    }
                }
            }

            // Case 2a.2: Typed Console instance method calls (e.g., console.log(...)).
            // The console intrinsic is a known runtime type and exposes instance methods; calling them directly
            // avoids generic dispatch and keeps generator output stable.
            if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Console))
            {
                if (hasSpreadArgs)
                {
                    // Spread argument count is not statically known; fall back to runtime member-dispatch.
                }
                else
                {
                    var consoleArgTemps = new List<TempVariable>();
                    foreach (var argExpr in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(argExpr, out var argTempVar))
                        {
                            return false;
                        }
                        consoleArgTemps.Add(EnsureObject(argTempVar));
                    }

                    if (TryResolveTypedInstanceCallReturnClrType(
                            typeof(JavaScriptRuntime.Console),
                            calleePropAccess.PropertyName,
                            consoleArgTemps.Count,
                            out _))
                    {
                        _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                            receiverTempVar,
                            typeof(JavaScriptRuntime.Console),
                            calleePropAccess.PropertyName,
                            consoleArgTemps,
                            resultTempVar));

                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        return true;
                    }
                }
            }

            // Case 2a.3: Direct calls to known instance methods on the current user-defined class.
            // Example: `this.setBitTrue(x)` inside a class method can be emitted as a direct callvirt
            // rather than runtime dispatch through ObjectRuntime.CallMember.
            if (_classRegistry != null
                && calleePropAccess.Object is HIRThisExpression
                && TryGetEnclosingClassRegistryName(out var currentClass)
                && currentClass != null
                && _classRegistry.TryGetMethod(currentClass, calleePropAccess.PropertyName, out var methodHandle, out _, out var methodReturnClrType, out var methodReturnTypeHandle, out var hasScopesParam, out _, out var maxParamCount))
            {
                if (hasSpreadArgs)
                {
                    // Spread argument count is not statically known; fall back to runtime member-dispatch.
                }
                else
                {
                var argTemps = new List<TempVariable>();
                foreach (var argExpr in callExpr.Arguments)
                {
                    if (!TryLowerExpression(argExpr, out var argTempVar))
                    {
                        return false;
                    }

                    argTemps.Add(EnsureObject(argTempVar));
                }

                _methodBodyIR.Instructions.Add(new LIRCallUserClassInstanceMethod(
                    currentClass,
                    calleePropAccess.PropertyName,
                    methodHandle,
                    hasScopesParam,
                    calleePropAccess.PropertyName.StartsWith("__jroc_priv_method_", StringComparison.Ordinal),
                    maxParamCount,
                    argTemps,
                    resultTempVar));

                // Propagate typed return storage when available.
                if (!methodReturnTypeHandle.IsNil)
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object), methodReturnTypeHandle));
                }
                else if (methodReturnClrType == typeof(double))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                }
                else if (methodReturnClrType == typeof(bool))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                }
                else if (methodReturnClrType == typeof(string))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                }
                else
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                }
                return true;
                }
                }
        }

        // Case 2c: Generic member call via runtime dispatcher.
        // This is a catch-all for calls like `output.join(',')` where `output` may be boxed as object,
        // so typed receiver lowering can't prove the receiver type.
        receiverTempVar = RequireObjectCoercible(receiverTempVar);

        // Check if we can use arity-specific instruction (no spread, 0-3 args)
        if (!HasSpreadArguments(callExpr.Arguments) && callExpr.Arguments.Length <= 3)
        {
            // Lower arguments individually
            var argTemps = new List<TempVariable>(callExpr.Arguments.Length);
            foreach (var arg in callExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            // Emit arity-specific instruction
            LIRInstruction callInstr = callExpr.Arguments.Length switch
            {
                0 => new LIRCallMember0(receiverTempVar, calleePropAccess.PropertyName, resultTempVar),
                1 => new LIRCallMember1(receiverTempVar, calleePropAccess.PropertyName, argTemps[0], resultTempVar),
                2 => new LIRCallMember2(receiverTempVar, calleePropAccess.PropertyName, argTemps[0], argTemps[1], resultTempVar),
                3 => new LIRCallMember3(receiverTempVar, calleePropAccess.PropertyName, argTemps[0], argTemps[1], argTemps[2], resultTempVar),
                _ => throw new InvalidOperationException("Unexpected arity")
            };
            _methodBodyIR.Instructions.Add(callInstr);
        }
        else
        {
            // Fall back to array-based call for >3 args or spread
            if (!TryLowerCallArgumentsToArgsArray(callExpr.Arguments, out var argsArrayTempVar))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRCallMember(receiverTempVar, calleePropAccess.PropertyName, argsArrayTempVar, resultTempVar));
        }

        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private void DefineDirectCallResultStorage(TempVariable resultTempVar, TwoPhase.CallableId? callableId, BindingInfo? symbol)
    {
        Type? returnClrType = null;
        if (callableId != null && _callableRegistry != null)
        {
            returnClrType = _callableRegistry.GetSignature(callableId)?.ReturnClrType;
        }

        returnClrType ??= GetStableDirectFunctionReturnClrType(symbol, callableId);

        if (returnClrType == typeof(double))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        }
        else if (returnClrType == typeof(bool))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        }
        else if (returnClrType == typeof(string))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        }
        else if (returnClrType == typeof(JavaScriptRuntime.Array))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
        }
        else
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
    }

    private static Type? GetStableDirectFunctionReturnClrType(BindingInfo? symbol, TwoPhase.CallableId? callableId)
    {
        if (symbol?.Kind != BindingKind.Function
            || callableId?.JsParamCount != 0)
        {
            return null;
        }

        var functionScope = FindFunctionScope(symbol.DeclaringScope, symbol);

        if (functionScope == null
            || functionScope.ReferencesParentScopeVariables)
        {
            return null;
        }

        return functionScope.StableReturnClrType == typeof(double)
            ? typeof(double)
            : null;
    }

    private static Scope? FindFunctionScope(Scope root, BindingInfo symbol)
    {
        foreach (var child in root.Children)
        {
            if (child.Kind == ScopeKind.Function
                && (ReferenceEquals(child.AstNode, symbol.DeclarationNode)
                    || string.Equals(child.Name, symbol.Name, StringComparison.Ordinal)
                    || (child.AstNode is FunctionDeclaration functionDeclaration
                        && functionDeclaration.Id?.Name == symbol.Name)))
            {
                return child;
            }

            var descendant = FindFunctionScope(child, symbol);
            if (descendant != null)
            {
                return descendant;
            }
        }

        return null;
    }

    private static bool TryResolveTypedInstanceCallReturnClrType(Type receiverType, string methodName, int argCount, out Type returnClrType)
    {
        var chosen = LIRToILCompiler.ResolveTypedInstanceMethodOverload(receiverType, methodName, argCount);
        if (chosen == null)
        {
            returnClrType = typeof(object);
            return false;
        }

        returnClrType = chosen.ReturnType;
        return true;
    }

    private static Type? TryGetNodeModuleContractType(Type? type)
    {
        if (type == null || !type.IsInterface)
        {
            return null;
        }

        return type.GetCustomAttributes(
                typeof(Jroc.Runtime.Node.Contracts.NodeModuleInterfaceAttribute),
                inherit: false)
            .Length == 1
            ? type
            : null;
    }

    private bool CanDirectlyCallNodeContractMethod(
        System.Reflection.MethodInfo method,
        IReadOnlyList<TempVariable> arguments)
    {
        var parameters = method.GetParameters();
        var hasParamsArray = parameters.Length > 0
            && parameters[^1].GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false).Length > 0;
        var fixedParameterCount = hasParamsArray ? parameters.Length - 1 : parameters.Length;

        for (var i = 0; i < fixedParameterCount; i++)
        {
            var parameterType = parameters[i].ParameterType;
            if (parameterType == typeof(object))
            {
                continue;
            }

            var argumentType = GetTempStorage(arguments[i]).ClrType;
            if (argumentType == null
                || (argumentType != parameterType && !parameterType.IsAssignableFrom(argumentType)))
            {
                return false;
            }
        }

        return true;
    }

    private void EmitDynamicMemberCall(
        TempVariable receiver,
        string methodName,
        IReadOnlyList<TempVariable> arguments,
        TempVariable result)
    {
        LIRInstruction instruction;
        switch (arguments.Count)
        {
            case 0:
                instruction = new LIRCallMember0(receiver, methodName, result);
                break;
            case 1:
                instruction = new LIRCallMember1(receiver, methodName, arguments[0], result);
                break;
            case 2:
                instruction = new LIRCallMember2(receiver, methodName, arguments[0], arguments[1], result);
                break;
            case 3:
                instruction = new LIRCallMember3(receiver, methodName, arguments[0], arguments[1], arguments[2], result);
                break;
            default:
                var argumentsArray = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRBuildArray(arguments, argumentsArray));
                DefineTempStorage(argumentsArray, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                instruction = new LIRCallMember(receiver, methodName, argumentsArray, result);
                break;
        }

        _methodBodyIR.Instructions.Add(instruction);
    }

    private static bool TryResolveNodeModuleContractProperty(
        Type contractType,
        string propertyName,
        out string getterName,
        out Type propertyType)
    {
        foreach (var property in contractType.GetProperties(
                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            var attribute = property.GetCustomAttributes(
                    typeof(Jroc.Runtime.Node.Contracts.NodeModuleMemberAttribute),
                    inherit: false)
                .OfType<Jroc.Runtime.Node.Contracts.NodeModuleMemberAttribute>()
                .SingleOrDefault();
            var javaScriptName = attribute?.MemberName ?? property.Name;
            if (!string.Equals(javaScriptName, propertyName, StringComparison.Ordinal))
            {
                continue;
            }

            var getter = property.GetMethod;
            if (getter == null)
            {
                break;
            }

            getterName = getter.Name;
            propertyType = property.PropertyType;
            return true;
        }

        getterName = string.Empty;
        propertyType = typeof(object);
        return false;
    }

}
