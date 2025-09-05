﻿using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is teh entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private ILMethodGenerator _ilGenerator;
        private JavaScriptFunctionGenerator _functionGenerator;
        private ClassesGenerator _classesGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private SymbolTable _symbolTable;
        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private readonly ClassRegistry _classRegistry = new();

        public MainGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator, SymbolTable symbolTable)
        {
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));

            _ilGenerator = new ILMethodGenerator(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _dispatchTableGenerator, _classRegistry);
            _functionGenerator = new JavaScriptFunctionGenerator(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _dispatchTableGenerator, _classRegistry);
            _classesGenerator = new ClassesGenerator(metadataBuilder, bclReferences, methodBodyStreamEncoder, _classRegistry, variables, _dispatchTableGenerator);
            this._methodBodyStreamEncoder = methodBodyStreamEncoder;
        }

        public MainGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator, SymbolTable symbolTable, TypeBuilder programTypeBuilder)
            : this(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, dispatchTableGenerator, symbolTable)
        {
        }

        /// <summary>
        /// Creates the global scope instance.
        /// The instance is stored in a local variable that can be accessed by variable operations.
        /// </summary>
        private void CreateGlobalScopeInstance(Variables variables)
        {
            // Delegate to shared helper; safe no-op if no registry or scope type is available
            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(variables, _ilGenerator.IL, _ilGenerator.MetadataBuilder);
        }

        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;

            // Step 1: Create the global scope instance
            CreateGlobalScopeInstance(variables);

            // Note: Do not pre-instantiate function or nested scopes in Main.
            // Function scopes are created at call-time by the function generator.

            // First, declare classes so their types exist under the Classes namespace
            _classesGenerator.DeclareClasses(_symbolTable);

            // create the dispatch
            // functions are hosted so we need to declare them first
            _functionGenerator.DeclareFunctions(_symbolTable);

            var loadDispatchTableMethod = _dispatchTableGenerator.GenerateLoadDispatchTableMethod();
            if (!loadDispatchTableMethod.IsNil)
            {
                _ilGenerator.IL.Call(loadDispatchTableMethod);
                _ilGenerator.InitializeLocalFunctionVariables(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());
            }

            _ilGenerator.GenerateStatementsForBody(variables.GetLeafScopeName(), false, ast.Body);

            _ilGenerator.IL.OpCode(ILOpCode.Ret);

            // local variables
            MethodBodyAttributes methodBodyAttributes = MethodBodyAttributes.None;
            StandaloneSignatureHandle localSignature = default;
            int numberOfLocals = variables.GetNumberOfLocals();
            if (numberOfLocals > 0)
            {
                var localSig = new BlobBuilder();
                var localVariableEncoder = new BlobEncoder(localSig).LocalVariableSignature(numberOfLocals);
                for (int i = 0; i < numberOfLocals; i++)
                {
                    localVariableEncoder.AddVariable().Type().Object();
                }

                localSignature = metadataBuilder.AddStandaloneSignature(metadataBuilder.GetOrAddBlob(localSig));
                methodBodyAttributes = MethodBodyAttributes.InitLocals;
            }

            // First method tracking is now handled by the specific generators that own method emission.

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL,
                maxStack: 32,
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }
    }
}
