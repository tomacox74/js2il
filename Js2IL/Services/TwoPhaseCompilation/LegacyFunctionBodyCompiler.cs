using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.TwoPhaseCompilation;

internal static class LegacyFunctionBodyCompiler
{
    public static CompiledCallableBody CompileFunctionDeclarationBody(
        IServiceProvider serviceProvider,
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bclReferences,
        Variables parentVariables,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        SymbolTable symbolTable,
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        FunctionDeclaration functionDeclaration,
        Scope functionScope,
        string registryScopeName)
    {
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
        if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
        // MethodBodyStreamEncoder is a struct; cannot be null.
        if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
        if (parentVariables == null) throw new ArgumentNullException(nameof(parentVariables));
        if (classRegistry == null) throw new ArgumentNullException(nameof(classRegistry));
        if (functionRegistry == null) throw new ArgumentNullException(nameof(functionRegistry));
        if (symbolTable == null) throw new ArgumentNullException(nameof(symbolTable));
        if (functionDeclaration == null) throw new ArgumentNullException(nameof(functionDeclaration));
        if (functionScope == null) throw new ArgumentNullException(nameof(functionScope));
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));
        if (registryScopeName == null) throw new ArgumentNullException(nameof(registryScopeName));

        var functionName = (functionDeclaration.Id as Identifier)?.Name ?? callable.Name ?? "anonymous";

        if (functionDeclaration.Body is not BlockStatement blockStatement)
        {
            ILEmitHelpers.ThrowNotSupported($"Unsupported function body type: {functionDeclaration.Body.Type}", functionDeclaration.Body);
            throw new InvalidOperationException();
        }

        var paramNames = ILMethodGenerator.ExtractParameterNames(functionDeclaration.Params).ToArray();
        var functionVariables = new Variables(parentVariables, registryScopeName, paramNames, isNestedFunction: false);

        var methodGenerator = new ILMethodGenerator(
            serviceProvider,
            functionVariables,
            bclReferences,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry,
            symbolTable: symbolTable);

        var il = methodGenerator.IL;
        var runtime = new Js2IL.Services.Runtime(
            il,
            serviceProvider.GetRequiredService<TypeReferenceRegistry>(),
            serviceProvider.GetRequiredService<MemberReferenceRegistry>());

        methodGenerator.EmitDefaultParameterInitializers(functionDeclaration.Params, parameterStartIndex: 1);

        var registry = functionVariables.GetVariableRegistry();
        if (registry != null)
        {
            var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
            var hasAnyFields = fields.Any();
            if (hasAnyFields)
            {
                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, metadataBuilder);

                var localScope = functionVariables.GetLocalScopeSlot();
                var fieldNames = localScope.Address >= 0 ? new System.Collections.Generic.HashSet<string>(fields.Select(f => f.Name)) : new System.Collections.Generic.HashSet<string>();

                if (localScope.Address >= 0)
                {
                    ushort jsParamSeq = 1;
                    for (int i = 0; i < functionDeclaration.Params.Count; i++)
                    {
                        var paramNode = functionDeclaration.Params[i];
                        Identifier? pid = paramNode as Identifier;
                        if (pid == null && paramNode is AssignmentPattern ap)
                        {
                            pid = ap.Left as Identifier;
                        }

                        if (pid != null && fieldNames.Contains(pid.Name))
                        {
                            il.LoadLocal(localScope.Address);
                            methodGenerator.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                            var fieldHandle = registry.GetFieldHandle(registryScopeName, pid.Name);
                            il.OpCode(ILOpCode.Stfld);
                            il.Token(fieldHandle);
                        }
                        jsParamSeq++;
                    }

                    MethodBuilder.EmitObjectPatternParameterDestructuring(
                        metadataBuilder,
                        il,
                        runtime,
                        functionVariables,
                        registryScopeName,
                        functionDeclaration.Params,
                        methodGenerator.ExpressionEmitter,
                        startingJsParamSeq: 1,
                        castScopeForStore: false);
                }
            }
        }

        var hasExplicitReturn = blockStatement.Body.Any(s => s is ReturnStatement);

        // Initialize nested function variables (handles should already be declared in Phase 1)
        var nestedFunctions = functionScope.Children
            .Where(scope => scope.Kind == ScopeKind.Function && scope.AstNode is FunctionDeclaration)
            .Select(scope => (FunctionDeclaration)scope.AstNode!)
            .ToList();
        methodGenerator.InitializeLocalFunctionVariables(nestedFunctions);

        methodGenerator.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, blockStatement.Body);

        if (!hasExplicitReturn)
        {
            il.OpCode(ILOpCode.Ldnull);
            il.OpCode(ILOpCode.Ret);
        }

        var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(metadataBuilder, functionVariables, bclReferences);
        var bodyOffset = methodBodyStreamEncoder.AddMethodBody(
            il,
            maxStack: 32,
            localVariablesSignature: localSignature,
            attributes: bodyAttributes);

        var paramCount = 1 + functionDeclaration.Params.Count;
        var methodSig = MethodBuilder.BuildMethodSignature(
            metadataBuilder,
            isInstance: false,
            paramCount: paramCount,
            hasScopesParam: true,
            returnsVoid: false);

        // Param names for metadata finalization: scopes + JS params
        var parameterNames = new string[paramCount];
        parameterNames[0] = "scopes";
        for (int i = 0; i < paramNames.Length; i++)
        {
            parameterNames[i + 1] = paramNames[i];
        }

        return new CompiledCallableBody
        {
            Callable = callable,
            ExpectedMethodDef = expectedMethodDef,
            MethodName = functionName,
            Attributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            Signature = methodSig,
            BodyOffset = bodyOffset,
            ParameterNames = parameterNames
        };
    }
}
