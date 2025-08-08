using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    internal class JavaScriptFunctionGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private MethodDefinitionHandle _firstMethod = default;

        public MethodDefinitionHandle FirstMethod => _firstMethod;

        public JavaScriptFunctionGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator;
        }

        public void DeclareFunctions(IEnumerable<FunctionDeclaration> functionDeclarations)
        {
            // Iterate through each function declaration in the block
            foreach (var functionDeclaration in functionDeclarations)
            {
                DeclareFunction(functionDeclaration);
            }
        }

        public void DeclareFunction(FunctionDeclaration functionDeclaration)
        {
            var functionVariables = new Variables(_variables);
            var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);

            var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator);
            if (this._firstMethod.IsNil)
            {
                this._firstMethod = methodDefinition;
            }
        }

        public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration, Variables functionVariables, ILMethodGenerator methodGenerator)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;

            if (functionDeclaration.Body is not BlockStatement blockStatement)
            {
                throw new NotSupportedException($"Unsupported function body type: {functionDeclaration.Body.Type}");
            }

            var variables = functionVariables;
            var il = methodGenerator.IL;

            // Register parameters in Variables before emitting body so identifiers resolve
            int paramBaseIndex = 1; // 0 = scope
            for (int i = 0; i < functionDeclaration.Params.Count; i++)
            {
                if (functionDeclaration.Params[i] is Acornima.Ast.Identifier pidPre)
                {
                    variables.AddParameter(pidPre.Name, paramBaseIndex + i);
                }
            }

            // Create a scope instance for the function itself so that local vars (declared within the function)
            // have a backing scope object to store their fields. The Variables instance for the function only
            // carries the parent (global) scope as parameter 0, so we must allocate a local slot + instantiate.
            var registry = variables.GetVariableRegistry();
            if (registry != null)
            {
                try
                {
                    var functionScopeTypeHandle = registry.GetScopeTypeHandle(functionName);
                    if (!functionScopeTypeHandle.IsNil)
                    {
                        // Build constructor member reference (parameterless instance .ctor)
                        var ctorSigBuilder = new BlobBuilder();
                        new BlobEncoder(ctorSigBuilder)
                            .MethodSignature(isInstanceMethod: true)
                            .Parameters(0, rt => rt.Void(), p => { });
                        var ctorRef = _metadataBuilder.AddMemberReference(
                            functionScopeTypeHandle,
                            _metadataBuilder.GetOrAddString(".ctor"),
                            _metadataBuilder.GetOrAddBlob(ctorSigBuilder));

                        // newobj scope
                        il.OpCode(ILOpCode.Newobj);
                        il.Token(ctorRef);
                        // store into a new local slot associated with this function scope name
                        var scopeLocal = variables.CreateScopeInstance(functionName);
                        il.StoreLocal(scopeLocal.Address);
                    }
                }
                catch
                {
                    // If scope type not found we silently continue (no locals used)
                }
            }

            // Emit body statements
            var hasExplicitReturn = blockStatement.Body.Any(s => s is ReturnStatement);
            methodGenerator.GenerateStatements(blockStatement.Body);
            if (!hasExplicitReturn)
            {
                // Implicit return undefined => null
                il.OpCode(ILOpCode.Ldnull);
                il.OpCode(ILOpCode.Ret);
            }

            // Add method body including any scope locals we created (function scope instance)
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = variables.GetNumberOfLocals();
            if (localCount > 0)
            {
                var localSig = new BlobBuilder();
                var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(localCount);
                for (int i = 0; i < localCount; i++)
                {
                    localEncoder.AddVariable().Type().Object();
                }
                localSignature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
                bodyAttributes = MethodBodyAttributes.InitLocals;
            }

            var bodyoffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);
            // Build method signature: static object (object[] scopes, object param1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + functionDeclaration.Params.Count; // scope array + declared params
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    // scope array parameter
                    parameters.AddParameter().Type().SZArray().Object();
                    // each JS parameter as System.Object for now
                    foreach (var p in functionDeclaration.Params)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameters with names
            var scopeParamName = "scopes";
            ParameterHandle firstParamHandle = _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(scopeParamName),
                sequenceNumber: 1);
            // subsequent params
            ushort seq = 2;
            foreach (var p in functionDeclaration.Params)
            {
                if (p is Acornima.Ast.Identifier pid)
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(pid.Name), sequenceNumber: seq++);
                }
                else
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString($"param{seq-1}"), sequenceNumber: seq++);
                }
            }

            var methodDefinition = _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString(functionName),
                methodSig,
                bodyoffset,
                parameterList: firstParamHandle);

            // Register with dispatch table
            _dispatchTableGenerator.SetMethodDefinitionHandle(functionName, methodDefinition);

            return methodDefinition;
        }
    }
}
