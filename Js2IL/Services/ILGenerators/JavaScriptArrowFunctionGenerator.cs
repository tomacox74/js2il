using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal sealed class JavaScriptArrowFunctionGenerator
    {
        private readonly Variables _variables;
        private readonly BaseClassLibraryReferences _bclReferences;
        private readonly MetadataBuilder _metadataBuilder;
        private readonly MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private readonly ClassRegistry _classRegistry;
        private readonly FunctionRegistry _functionRegistry;

        public JavaScriptArrowFunctionGenerator(
            Variables variables,
            BaseClassLibraryReferences bclReferences,
            MetadataBuilder metadataBuilder,
            MethodBodyStreamEncoder methodBodyStreamEncoder,
            ClassRegistry classRegistry,
            FunctionRegistry functionRegistry)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = classRegistry;
            _functionRegistry = functionRegistry;
        }

        internal MethodDefinitionHandle GenerateArrowFunctionMethod(
            ArrowFunctionExpression arrowFunction,
            string registryScopeName,
            string ilMethodName,
            string[] paramNames)
        {
            var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: true);
            var pnames = paramNames ?? Array.Empty<string>();
            var childGen = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
            var il = childGen.IL;

            void DestructureArrowParamsIfAny()
            {
                var registry = functionVariables.GetVariableRegistry();
                if (registry == null) return;
                var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                if (!fields.Any()) return;
                var localScope = functionVariables.GetLocalScopeSlot();
                if (localScope.Address < 0)
                {
                    ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                    localScope = functionVariables.GetLocalScopeSlot();
                }
                MethodBuilder.EmitObjectPatternParameterDestructuring(
                    _metadataBuilder,
                    il,
                    childGen.Runtime,
                    functionVariables,
                    registryScopeName,
                    arrowFunction.Params,
                    childGen.ExpressionEmitter,
                    startingJsParamSeq: 1,
                    castScopeForStore: false);
            }

            if (arrowFunction.Body is BlockStatement block)
            {
                if (block.Body.Count == 2 &&
                    block.Body[0] is VariableDeclaration vdecl &&
                    (vdecl.Kind == VariableDeclarationKind.Const || vdecl.Kind == VariableDeclarationKind.Let) &&
                    vdecl.Declarations.Count == 1 &&
                    vdecl.Declarations[0].Id is Identifier vid &&
                    vdecl.Declarations[0].Init is Expression initExpr &&
                    block.Body[1] is ReturnStatement rstmt && rstmt.Argument is Identifier rid && rid.Name == vid.Name)
                {
                    bool returnsFunctionInitializer = initExpr is ArrowFunctionExpression || initExpr is FunctionExpression;

                    if (returnsFunctionInitializer)
                    {
                        var registry = functionVariables.GetVariableRegistry();
                        if (registry != null)
                        {
                            var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                            var hasAnyFields = fields.Any();
                            if (hasAnyFields)
                            {
                                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                                var localScope = functionVariables.GetLocalScopeSlot();
                                if (localScope.Address >= 0 && pnames.Length > 0)
                                {
                                    var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                    ushort jsParamSeq = 1;
                                    foreach (var pn in pnames)
                                    {
                                        if (fieldNames.Contains(pn))
                                        {
                                            il.LoadLocal(localScope.Address);
                                            il.LoadArgument(jsParamSeq);
                                            var fh = registry.GetFieldHandle(registryScopeName, pn);
                                            il.OpCode(ILOpCode.Stfld);
                                            il.Token(fh);
                                        }
                                        jsParamSeq++;
                                    }
                                }
                            }
                        }

                        var prevAssignment = childGen.CurrentAssignmentTarget;
                        childGen.CurrentAssignmentTarget = vid.Name;
                        try
                        {
                            _ = childGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion());
                        }
                        finally
                        {
                            childGen.CurrentAssignmentTarget = prevAssignment;
                        }

                        var innerVar = functionVariables.FindVariable(vid.Name);
                        if (innerVar != null)
                        {
                            var neededScopeNames = childGen.GetScopesForClosureBinding(innerVar).ToList();
                            il.EmitNewArray(neededScopeNames.Count, _bclReferences.ObjectType, (eil, idx) =>
                            {
                                var sn = neededScopeNames[idx];
                                var refSlot = functionVariables.GetScopeLocalSlot(sn);
                                if (refSlot.Location == ObjectReferenceLocation.Local)
                                {
                                    eil.LoadLocal(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.Parameter)
                                {
                                    eil.LoadArgument(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.ScopeArray)
                                {
                                    eil.LoadArgument(0);
                                    eil.LoadConstantI4(refSlot.Address);
                                    eil.OpCode(ILOpCode.Ldelem_ref);
                                }
                            });
                            childGen.Runtime.InvokeClosureBindObject();
                        }
                        il.OpCode(ILOpCode.Ret);
                    }
                    else
                    {
                        _ = childGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion() { boxResult = true });
                        il.OpCode(ILOpCode.Ret);
                    }
                }
                else
                {
                    var registry = functionVariables.GetVariableRegistry();
                    if (registry != null)
                    {
                        var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                        if (fields.Any())
                        {
                            childGen.EmitDefaultParameterInitializers(arrowFunction.Params, parameterStartIndex: 1);
                            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                            var localScope = functionVariables.GetLocalScopeSlot();
                            if (localScope.Address >= 0 && arrowFunction.Params.Count > 0)
                            {
                                var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                ushort jsParamSeq = 1;
                                for (int i = 0; i < arrowFunction.Params.Count; i++)
                                {
                                    var paramNode = arrowFunction.Params[i];
                                    Identifier? pid = paramNode as Identifier;
                                    if (pid == null && paramNode is AssignmentPattern ap)
                                    {
                                        pid = ap.Left as Identifier;
                                    }

                                    if (pid != null && fieldNames.Contains(pid.Name))
                                    {
                                        il.LoadLocal(localScope.Address);
                                        childGen.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                                        var fh = registry.GetFieldHandle(registryScopeName, pid.Name);
                                        il.OpCode(ILOpCode.Stfld);
                                        il.Token(fh);
                                    }
                                    jsParamSeq++;
                                }
                            }
                        }
                        else
                        {
                            childGen.EmitDefaultParameterInitializers(arrowFunction.Params, parameterStartIndex: 1);
                        }
                    }
                    DestructureArrowParamsIfAny();
                    childGen.Variables.PushLexicalScope(functionVariables.GetLeafScopeName());
                    try
                    {
                        childGen.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, block.Body);
                    }
                    finally
                    {
                        childGen.Variables.PopLexicalScope(functionVariables.GetLeafScopeName());
                    }
                    il.OpCode(ILOpCode.Ldnull);
                    il.OpCode(ILOpCode.Ret);
                }
            }
            else
            {
                childGen.EmitDefaultParameterInitializers(arrowFunction.Params, parameterStartIndex: 1);

                var registry = functionVariables.GetVariableRegistry();
                if (registry != null)
                {
                    var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                    if (fields.Any())
                    {
                        ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                        var localScope = functionVariables.GetLocalScopeSlot();
                        if (localScope.Address >= 0 && pnames.Length > 0)
                        {
                            var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                            ushort jsParamSeq = 1;
                            foreach (var pn in pnames)
                            {
                                if (fieldNames.Contains(pn))
                                {
                                    il.LoadLocal(localScope.Address);
                                    il.LoadArgument(jsParamSeq);
                                    var fh = registry.GetFieldHandle(registryScopeName, pn);
                                    il.OpCode(ILOpCode.Stfld);
                                    il.Token(fh);
                                }
                                jsParamSeq++;
                            }
                        }
                        DestructureArrowParamsIfAny();
                    }
                }
                var bodyExpr = arrowFunction.Body as Expression ?? throw ILEmitHelpers.NotSupported("Arrow function body is not an expression", arrowFunction.Body);
                childGen.Variables.PushLexicalScope(functionVariables.GetLeafScopeName());
                try
                {
                    _ = childGen.ExpressionEmitter.Emit(bodyExpr, new TypeCoercion() { boxResult = true });
                }
                finally
                {
                    childGen.Variables.PopLexicalScope(functionVariables.GetLeafScopeName());
                }
                il.OpCode(ILOpCode.Ret);
            }

            var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(_metadataBuilder, functionVariables, this._bclReferences);

            var bodyOffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                maxStack: 32,
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);

            var paramCount = 1 + pnames.Length;
            var methodSig = MethodBuilder.BuildMethodSignature(
                _metadataBuilder,
                isInstance: false,
                paramCount: paramCount,
                hasScopesParam: true,
                returnsVoid: false);

            var firstParam = _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString("scopes"), sequenceNumber: 1);
            ushort seq = 2;
            foreach (var p in pnames)
            {
                _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(p), sequenceNumber: seq++);
            }

            var tb = new TypeBuilder(_metadataBuilder, "Functions", ilMethodName);
            var mdh = tb.AddMethodDefinition(MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig, ilMethodName, methodSig, bodyOffset, firstParam);
            tb.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, _bclReferences.ObjectType);
            return mdh;
        }
    }
}
