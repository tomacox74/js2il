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
    private bool TryLowerCallExpression(HIRCallExpression callExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        bool hasSpreadArgs = callExpr.Arguments.Any(a => a is HIRSpreadElement);

        // Case 0: super(...) call in a derived class constructor.
        if (callExpr.Callee is HIRSuperExpression)
        {
            if (_callableKind != CallableKind.Constructor || !_isDerivedConstructor)
            {
                return false;
            }

            if (_superConstructorCalled)
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

                // Lower JS arguments (extras are evaluated for side effects, but ignored).
                for (int i = 0; i < callExpr.Arguments.Length; i++)
                {
                    if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                    {
                        return false;
                    }

                    if (i < baseCtorMaxParamCount)
                    {
                        callArgs.Add(EnsureObject(argTemp));
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
                    baseCtorMaxParamCount,
                    callArgs));
            }
            else
            {
                // Fallback: intrinsic base class (e.g., `extends Array`).
                // For intrinsics, we preserve JS argument list semantics (do not truncate/pad).
                var intrinsicName = GetEnclosingSuperClassIntrinsicName();
                if (intrinsicName == null)
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

                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicBaseConstructor(intrinsicName, callArgs));
            }

            // After super() the constructor is considered initialized.
            _superConstructorCalled = true;

            // In JS, super(...) returns the derived `this` value.
            _methodBodyIR.Instructions.Add(new LIRLoadThis(resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 1: User-defined function call (callee is a variable referencing a function)
        if (callExpr.Callee is HIRVariableExpression funcVarExpr)
        {
            var symbol = funcVarExpr.Name;

            // PL8.1: Primitive conversion callables: String(x), Number(x), Boolean(x).
            // These are CallExpression forms (not NewExpression) and should lower to runtime conversions.
            // Semantics:
            // - No args: String() => "", Number() => 0, Boolean() => false
            // - Extra args are evaluated for side-effects and ignored
            if (symbol.Kind == BindingKind.Global)
            {
                var name = symbol.Name;
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

                var intrinsicInfo = JavaScriptRuntime.IntrinsicObjectRegistry.GetInfo(name);
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
                if (string.Equals(globalFunctionName, "String", StringComparison.OrdinalIgnoreCase))
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

                if (string.Equals(globalFunctionName, "Number", StringComparison.OrdinalIgnoreCase))
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

                    var source = EnsureObject(args[0]);
                    _methodBodyIR.Instructions.Add(new LIRConvertToNumber(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Boolean", StringComparison.OrdinalIgnoreCase))
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

                if (string.Equals(globalFunctionName, "Symbol", StringComparison.OrdinalIgnoreCase))
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

                if (string.Equals(globalFunctionName, "BigInt", StringComparison.OrdinalIgnoreCase))
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
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);

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

            // If the callee is not a direct function binding (e.g., it's a local/const holding a closure),
            // invoke via runtime dispatch (Closure.InvokeWithArgs).
            if (symbol.Kind != BindingKind.Function)
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

            // Check if the function has simple identifier parameters (no defaults, destructuring, rest).
            // If the function uses complex params, it will be compiled via traditional generator
            // with a different calling convention, so we must bail out to ensure Main is also
            // compiled traditionally to maintain calling convention consistency.
            if (!FunctionHasSimpleParams(symbol))
            {
                return false;
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

                var callableIdForSpread = TryCreateCallableIdForFunctionDeclaration(symbol);
                _methodBodyIR.Instructions.Add(new LIRCallFunctionWithArgsArray(symbol, scopesTempForSpread, argsArrayTemp, resultTempVar, callableIdForSpread));
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
            var callableId = TryCreateCallableIdForFunctionDeclaration(symbol);
            _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, scopesTempVar, arguments, resultTempVar, callableId));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            return true;
        }

        // Case 1b: Indirect call where the callee is an expression value (e.g., IIFE:
        // (function() { ... })(), (() => 1)(), or getFn()()).
        // Exclude property-access calls here to avoid accidentally breaking method-call semantics
        // that are handled by intrinsic/typed-member lowering below.
        if (callExpr.Callee is not HIRVariableExpression && callExpr.Callee is not HIRPropertyAccessExpression)
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

        // Case 2b: Intrinsic static method call (e.g., Array.isArray, Math.abs, JSON.parse)
        // Check if the object is a global variable that maps to an intrinsic type
        if (calleePropAccess.Object is HIRVariableExpression calleeGlobalVar &&
            calleeGlobalVar.Name.Kind == BindingKind.Global)
        {
            var intrinsicName = calleeGlobalVar.Name.Name;
            var methodName = calleePropAccess.PropertyName;

            // Try to resolve the intrinsic type via IntrinsicObjectRegistry
            var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(intrinsicName);
            if (intrinsicType != null)
            {
                // Check if there's a matching static method
                var staticMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (staticMethods.Count > 0)
                {
                    // Choose the same overload we will emit in IL (see LIRToILCompiler.EmitIntrinsicStaticCall)
                    var argCount = callExpr.Arguments.Count();

                    bool ExactArityMatch(System.Reflection.MethodInfo mi) => mi.GetParameters().Length == argCount;

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

                // Resumable static class methods (async/generator) follow the js2il calling convention and
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

                _methodBodyIR.Instructions.Add(new LIRCallDeclaredCallable(callableId, callArgTemps, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }
        }

        // Lower receiver once for instance/member call cases below.
        // IMPORTANT: Do not lower the receiver more than once; it may have side effects (e.g. promise chaining).
        if (!TryLowerExpression(calleePropAccess.Object, out var receiverTempVar))
        {
            return false;
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

                _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                    receiverTempVar,
                    typeof(JavaScriptRuntime.Array),
                    calleePropAccess.PropertyName,
                    arrayArgTemps,
                    resultTempVar));

                // Determine the CLR return type of the chosen intrinsic method so we can allocate
                // an appropriately-typed temp (and box later only when JS semantics require object).
                // This is critical for value-type returns (bool/double), which must not be stored into
                // object temps without boxing.
                var returnClrType = ResolveTypedInstanceCallReturnClrType(
                    typeof(JavaScriptRuntime.Array),
                    calleePropAccess.PropertyName,
                    arrayArgTemps.Count);

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

            // Case 2a.3: Direct calls to known instance methods on the current user-defined class.
            // Example: `this.setBitTrue(x)` inside a class method can be emitted as a direct callvirt
            // rather than runtime dispatch through Object.CallMember.
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
        receiverTempVar = EnsureObject(receiverTempVar);

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

    private static Type ResolveTypedInstanceCallReturnClrType(Type receiverType, string methodName, int argCount)
    {
        var allMethods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var methods = allMethods
            .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Prefer JS-style variadic methods taking object[] args.
        var chosen = methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
        });

        // Else: exact arity match with object parameters.
        chosen ??= methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == argCount && ps.All(p => p.ParameterType == typeof(object));
        });

        return chosen?.ReturnType ?? typeof(object);
    }

}
