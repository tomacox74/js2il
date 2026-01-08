using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.TwoPhaseCompilation;

internal static class LegacyClassBodyCompiler
{
    private static string GetRegistryClassName(Variables variables, Scope classScope)
    {
        var ns = classScope.DotNetNamespace ?? "Classes";
        var name = classScope.DotNetTypeName ?? classScope.Name;
        return $"{ns}.{name}";
    }

    private static List<string> DetermineParentScopesForClassMethod(Variables variables, Scope classScope)
    {
        var scopeNames = new List<string>();
        var moduleName = variables.GetGlobalScopeName();

        var current = classScope.Parent;
        var ancestors = new Stack<string>();
        while (current != null)
        {
            var name = current.Name;
            if (!string.IsNullOrEmpty(name) && !name.Contains('/') && name != moduleName)
            {
                name = $"{moduleName}/{name}";
            }

            ancestors.Push(name);
            current = current.Parent;
        }

        while (ancestors.Count > 0)
        {
            scopeNames.Add(ancestors.Pop());
        }

        return scopeNames;
    }

    public static CompiledCallableBody CompileConstructorBody(
        IServiceProvider serviceProvider,
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bcl,
        ClassRegistry classRegistry,
        Variables rootVariables,
        SymbolTable symbolTable,
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        Scope classScope,
        ClassDeclaration classDecl,
        FunctionExpression? ctorFunc,
        bool needsScopes)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var className = GetRegistryClassName(rootVariables, classScope);

        // Build instance + static field initializer lists from the AST and registry.
        var fieldsWithInits = new List<(FieldDefinitionHandle Field, Expression? Init)>();
        foreach (var element in classDecl.Body.Body.OfType<Acornima.Ast.PropertyDefinition>())
        {
            if (element.Static) continue;
            if (element.Key is PrivateIdentifier priv)
            {
                if (classRegistry.TryGetPrivateField(className, priv.Name, out var fh))
                {
                    fieldsWithInits.Add((fh, element.Value as Expression));
                }
            }
            else if (element.Key is Identifier pid)
            {
                if (classRegistry.TryGetField(className, pid.Name, out var fh))
                {
                    fieldsWithInits.Add((fh, element.Value as Expression));
                }
            }
        }

        var userParamCount = ctorFunc?.Params.Count ?? 0;
        var totalParamCount = needsScopes ? userParamCount + 1 : userParamCount;

        var ctorSig = MethodBuilder.BuildMethodSignature(
            metadataBuilder,
            isInstance: true,
            paramCount: totalParamCount,
            hasScopesParam: needsScopes,
            returnsVoid: true);

        var paramNames = ctorFunc != null
            ? ILMethodGenerator.ExtractParameterNames(ctorFunc.Params)
            : Enumerable.Empty<string>();

        Variables methodVariables;
        var constructorScopeName = $"{rootVariables.GetGlobalScopeName()}/constructor";
        if (needsScopes)
        {
            var parentScopeNames = DetermineParentScopesForClassMethod(rootVariables, classScope);
            methodVariables = new Variables(rootVariables, constructorScopeName, paramNames, parentScopeNames, parameterStartIndex: 2);
        }
        else
        {
            methodVariables = new Variables(rootVariables, constructorScopeName, paramNames, isNestedFunction: false);
        }

        var ilGen = new ILMethodGenerator(
            serviceProvider,
            methodVariables,
            bcl,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry: null,
            inClassMethod: true,
            currentClassName: className,
            symbolTable: symbolTable);

        ilGen.IL.OpCode(ILOpCode.Ldarg_0);
        ilGen.IL.Call(bcl.Object_Ctor_Ref);

        if (needsScopes)
        {
            ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            ilGen.IL.OpCode(ILOpCode.Ldarg_1);
            ilGen.IL.OpCode(ILOpCode.Stfld);
            classRegistry.TryGetPrivateField(className, "_scopes", out var scopesField);
            ilGen.IL.Token(scopesField);
        }

        // Field initializers
        foreach (var (field, initExpr) in fieldsWithInits)
        {
            ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            if (initExpr is null)
            {
                ilGen.IL.OpCode(ILOpCode.Pop);
            }
            else
            {
                ilGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion { boxResult = true });
                ilGen.IL.OpCode(ILOpCode.Stfld);
                ilGen.IL.Token(field);
            }
        }

        if (ctorFunc != null)
        {
            ushort paramStartIndex = (ushort)(needsScopes ? 2 : 1);

            // Default parameter values must be initialized before destructuring.
            ilGen.EmitDefaultParameterInitializers(ctorFunc.Params, parameterStartIndex: paramStartIndex);

            bool hasDestructuredParams = ctorFunc.Params.Any(p => p is ObjectPattern || (p is AssignmentPattern ap && ap.Left is ObjectPattern));

            // When destructuring parameters (or when this scope has field-backed vars), create the leaf scope instance
            // and initialize parameter fields/destructured bindings before emitting the body.
            var registry = methodVariables.GetVariableRegistry();
            var fields = registry?.GetVariablesForScope(constructorScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
            var fieldNames = new HashSet<string>(fields.Select(f => f.Name));

            bool createdLeafScope = false;
            if (hasDestructuredParams || (registry != null && fields.Any()))
            {
                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(methodVariables, ilGen.IL, metadataBuilder);
                createdLeafScope = true;

                // Initialize captured identifier parameters into scope fields.
                var localScope = methodVariables.GetLocalScopeSlot();
                if (localScope.Address >= 0 && ctorFunc.Params.Count > 0)
                {
                    ushort jsParamSeq = paramStartIndex;
                    for (int i = 0; i < ctorFunc.Params.Count; i++)
                    {
                        var paramNode = ctorFunc.Params[i];
                        Identifier? pid = paramNode as Identifier;
                        if (pid == null && paramNode is AssignmentPattern ap)
                        {
                            pid = ap.Left as Identifier;
                        }

                        if (pid != null && fieldNames.Contains(pid.Name) && registry != null)
                        {
                            ilGen.IL.LoadLocal(localScope.Address);
                            ilGen.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                            var fh = registry.GetFieldHandle(constructorScopeName, pid.Name);
                            ilGen.IL.OpCode(ILOpCode.Stfld);
                            ilGen.IL.Token(fh);
                        }

                        jsParamSeq++;
                    }

                    if (hasDestructuredParams)
                    {
                        // Destructure object-pattern parameters into scope fields/locals (supports defaults like { host = "localhost" }).
                        MethodBuilder.EmitObjectPatternParameterDestructuring(
                            metadataBuilder,
                            ilGen.IL,
                            ilGen.Runtime,
                            methodVariables,
                            constructorScopeName,
                            ctorFunc.Params,
                            ilGen.ExpressionEmitter,
                            startingJsParamSeq: paramStartIndex,
                            castScopeForStore: true);
                    }
                }
            }

            if (ctorFunc.Body is BlockStatement bstmt)
            {
                // If we already created the scope, the body emitter must not create another one.
                ilGen.GenerateStatementsForBody(methodVariables.GetLeafScopeName(), !createdLeafScope && hasDestructuredParams, bstmt.Body);
            }
        }

        ilGen.IL.OpCode(ILOpCode.Ret);

        var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(metadataBuilder, methodVariables, bcl);
        var bodyOffset = methodBodyStreamEncoder.AddMethodBody(ilGen.IL, maxStack: 32, localVariablesSignature: localSignature, attributes: bodyAttributes);

        // Parameter metadata: keep parity with legacy constructor emission.
        var ctorParamNames = new List<string>();
        if (needsScopes)
        {
            ctorParamNames.Add("scopes");
        }
        if (ctorFunc != null)
        {
            ctorParamNames.AddRange(paramNames);
        }

        return new CompiledCallableBody
        {
            Callable = callable,
            ExpectedMethodDef = expectedMethodDef,
            MethodName = ".ctor",
            Attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
            Signature = ctorSig,
            BodyOffset = bodyOffset,
            ParameterNames = ctorParamNames.ToArray()
        };
    }

    public static CompiledCallableBody CompileMethodBody(
        IServiceProvider serviceProvider,
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bcl,
        ClassRegistry classRegistry,
        Variables rootVariables,
        SymbolTable symbolTable,
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        Scope classScope,
        Acornima.Ast.MethodDefinition methodDef,
        string clrMethodName)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var className = GetRegistryClassName(rootVariables, classScope);

        var funcExpr = methodDef.Value as FunctionExpression;
        var paramCount = funcExpr?.Params.Count ?? 0;
        var msig = MethodBuilder.BuildMethodSignature(
            metadataBuilder,
            isInstance: !methodDef.Static,
            paramCount: paramCount,
            hasScopesParam: false,
            returnsVoid: false);

        var paramNames = funcExpr != null
            ? ILMethodGenerator.ExtractParameterNames(funcExpr.Params)
            : Enumerable.Empty<string>();

        // IMPORTANT: The Variables scope name must match the VariableRegistry scope name.
        // For class methods, the registry uses the actual method scope name (e.g. "<ClassName>/<method>").
        // Using a synthetic name like "GlobalScope/<method>" breaks scope-local slot lookup.
        // VariableRegistry keys are module-qualified for all non-global scopes.
        // IMPORTANT: do NOT use scope.GetQualifiedName() here; TypeGenerator currently uses only scope.Name.
        // This mirrors TypeGenerator.GetRegistryScopeName.
        string? methodScopeName = null;
        if (funcExpr != null)
        {
            var methodScope = symbolTable.FindScopeByAstNode(funcExpr);
            if (methodScope != null)
            {
                methodScopeName = methodScope.Kind == ScopeKind.Global
                    ? methodScope.Name
                    : $"{rootVariables.GetGlobalScopeName()}/{methodScope.Name}";
            }
        }
        methodScopeName ??= $"{rootVariables.GetGlobalScopeName()}/{clrMethodName}";
        Variables methodVariables;
        if (!methodDef.Static && classRegistry.TryGetPrivateField(className, "_scopes", out var _))
        {
            var parentScopeNames = DetermineParentScopesForClassMethod(rootVariables, classScope);
            methodVariables = new Variables(rootVariables, methodScopeName, paramNames, parentScopeNames);
        }
        else
        {
            methodVariables = new Variables(rootVariables, methodScopeName, paramNames, isNestedFunction: false);
        }

        var ilGen = new ILMethodGenerator(
            serviceProvider,
            methodVariables,
            bcl,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry: null,
            inClassMethod: true,
            currentClassName: className,
            symbolTable: symbolTable);

        bool hasExplicitReturn = false;
        if (funcExpr != null)
        {
            ushort paramStartIndex = (ushort)(methodDef.Static ? 0 : 1);

            // When the method scope has field-backed variables, we must create the leaf scope instance
            // and initialize parameter fields / destructured params before emitting the body.
            // This mirrors the behavior in ILMethodGenerator.GenerateFunctionExpressionMethod.
            var registry = methodVariables.GetVariableRegistry();
            var fields = registry?.GetVariablesForScope(methodScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
            if (registry != null && fields.Any())
            {
                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(methodVariables, ilGen.IL, metadataBuilder);
                // Default parameter values must be initialized before field initialization.
                ilGen.EmitDefaultParameterInitializers(funcExpr.Params, parameterStartIndex: paramStartIndex);

                var localScope = methodVariables.GetLocalScopeSlot();
                if (localScope.Address >= 0 && funcExpr.Params.Count > 0)
                {
                    var fieldNames = new HashSet<string>(fields.Select(f => f.Name));

                    // Initialize captured identifier parameters into scope fields.
                    ushort jsParamSeq = paramStartIndex;
                    for (int i = 0; i < funcExpr.Params.Count; i++)
                    {
                        var paramNode = funcExpr.Params[i];
                        Identifier? pid = paramNode as Identifier;
                        if (pid == null && paramNode is AssignmentPattern ap)
                        {
                            pid = ap.Left as Identifier;
                        }

                        if (pid != null && fieldNames.Contains(pid.Name))
                        {
                            ilGen.IL.LoadLocal(localScope.Address);
                            ilGen.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                            var fh = registry.GetFieldHandle(methodScopeName, pid.Name);
                            ilGen.IL.OpCode(ILOpCode.Stfld);
                            ilGen.IL.Token(fh);
                        }

                        jsParamSeq++;
                    }

                    // Destructure object-pattern parameters into scope fields/locals (supports defaults like { host = "localhost" }).
                    MethodBuilder.EmitObjectPatternParameterDestructuring(
                        metadataBuilder,
                        ilGen.IL,
                        ilGen.Runtime,
                        methodVariables,
                        methodScopeName,
                        funcExpr.Params,
                        ilGen.ExpressionEmitter,
                        startingJsParamSeq: paramStartIndex,
                        castScopeForStore: true);
                }
            }
            else
            {
                ilGen.EmitDefaultParameterInitializers(funcExpr.Params, parameterStartIndex: paramStartIndex);
            }

            if (funcExpr.Body is BlockStatement bstmt)
            {
                hasExplicitReturn = bstmt.Body.Any(s => s is ReturnStatement);
                ilGen.GenerateStatementsForBody(methodVariables.GetLeafScopeName(), false, bstmt.Body);
            }
        }

        if (!hasExplicitReturn)
        {
            if (methodDef.Kind is PropertyKind.Get or PropertyKind.Set)
            {
                ilGen.IL.OpCode(ILOpCode.Ldnull);
            }
            else if (!methodDef.Static)
            {
                ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            }
            else
            {
                ilGen.IL.OpCode(ILOpCode.Ldnull);
            }
            ilGen.IL.OpCode(ILOpCode.Ret);
        }

        var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(metadataBuilder, methodVariables, bcl);
        var bodyOffset = methodBodyStreamEncoder.AddMethodBody(ilGen.IL, maxStack: 32, localVariablesSignature: localSignature, attributes: bodyAttributes);

        var attrs = MethodAttributes.Public | MethodAttributes.HideBySig;
        if (methodDef.Static)
        {
            attrs |= MethodAttributes.Static;
        }

        return new CompiledCallableBody
        {
            Callable = callable,
            ExpectedMethodDef = expectedMethodDef,
            MethodName = clrMethodName,
            Attributes = attrs,
            Signature = msig,
            BodyOffset = bodyOffset,
            // Preserve legacy: no Param rows for methods.
            ParameterNames = Array.Empty<string>()
        };
    }

    public static CompiledCallableBody CompileStaticInitializerBody(
        IServiceProvider serviceProvider,
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bcl,
        ClassRegistry classRegistry,
        Variables rootVariables,
        SymbolTable symbolTable,
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        Scope classScope,
        ClassDeclaration classDecl)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var className = GetRegistryClassName(rootVariables, classScope);

        var sigBuilder = new BlobBuilder();
        new BlobEncoder(sigBuilder)
            .MethodSignature(isInstanceMethod: false)
            .Parameters(0, r => r.Void(), p => { });
        var cctorSig = metadataBuilder.GetOrAddBlob(sigBuilder);

        var ilGen = new ILMethodGenerator(
            serviceProvider,
            rootVariables,
            bcl,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry: null,
            inClassMethod: false,
            currentClassName: className,
            symbolTable: symbolTable);

        foreach (var element in classDecl.Body.Body.OfType<Acornima.Ast.PropertyDefinition>())
        {
            if (!element.Static || element.Value == null) continue;

            FieldDefinitionHandle field;
            if (element.Key is PrivateIdentifier priv)
            {
                if (!classRegistry.TryGetStaticField(className, priv.Name, out field))
                {
                    continue;
                }
            }
            else if (element.Key is Identifier pid)
            {
                if (!classRegistry.TryGetStaticField(className, pid.Name, out field))
                {
                    continue;
                }
            }
            else
            {
                continue;
            }

            ilGen.ExpressionEmitter.Emit((Expression)element.Value, new TypeCoercion { boxResult = true });
            ilGen.IL.OpCode(ILOpCode.Stsfld);
            ilGen.IL.Token(field);
        }

        ilGen.IL.OpCode(ILOpCode.Ret);
        var bodyOffset = methodBodyStreamEncoder.AddMethodBody(ilGen.IL);

        return new CompiledCallableBody
        {
            Callable = callable,
            ExpectedMethodDef = expectedMethodDef,
            MethodName = ".cctor",
            Attributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
            Signature = cctorSig,
            BodyOffset = bodyOffset,
            ParameterNames = Array.Empty<string>()
        };
    }
}
