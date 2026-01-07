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

        var className = classScope.Name;

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
            ilGen.EmitDefaultParameterInitializers(ctorFunc.Params, parameterStartIndex: paramStartIndex);

            bool hasDestructuredParams = ctorFunc.Params.Any(p => p is ObjectPattern);
            bool needScopeInstance = hasDestructuredParams;

            bool scopeCreated = false;
            if (needScopeInstance && ctorFunc.Params.Count > 0)
            {
                // Reuse ILMethodGenerator helper via reflection? Keep minimal: let GenerateStatementsForBody create as needed.
                scopeCreated = false;
            }

            if (ctorFunc.Body is BlockStatement bstmt)
            {
                bool shouldCreateScope = needScopeInstance && !scopeCreated;
                ilGen.GenerateStatementsForBody(methodVariables.GetLeafScopeName(), shouldCreateScope, bstmt.Body);
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

        var className = classScope.Name;

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

        var methodScopeName = $"{rootVariables.GetGlobalScopeName()}/{clrMethodName}";
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
            ilGen.EmitDefaultParameterInitializers(funcExpr.Params, parameterStartIndex: paramStartIndex);

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

        var className = classScope.Name;

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
