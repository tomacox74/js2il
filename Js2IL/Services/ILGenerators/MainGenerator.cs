using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is teh entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private ILMethodGenerator _ilGenerator;
        private JavaScriptFunctionGenerator _functionGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;

        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;

        public MethodDefinitionHandle FirstMethod { get; private set; }

        public MainGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));

            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));

            _ilGenerator = new ILMethodGenerator(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _dispatchTableGenerator);
            _functionGenerator = new JavaScriptFunctionGenerator(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _dispatchTableGenerator);
            this._methodBodyStreamEncoder = methodBodyStreamEncoder;
        }

        /// <summary>
        /// Creates scope instances for all scopes that contain variables.
        /// Each scope instance is stored in a local variable that can be accessed by variable operations.
        /// </summary>
        private void CreateScopeInstances(Variables variables)
        {
            var registry = variables.GetVariableRegistry();
            if (registry == null)
                return; // No registry means we're using the old local variable system

            foreach (var scopeName in registry.GetAllScopeNames())
            {
                var scopeTypeHandle = registry.GetScopeTypeHandle(scopeName);
                
                // Create constructor reference for the scope type
                var ctorRef = _ilGenerator.MetadataBuilder.AddMemberReference(
                    scopeTypeHandle,
                    _ilGenerator.MetadataBuilder.GetOrAddString(".ctor"),
                    CreateConstructorSignature()
                );

                // Generate IL: new ScopeType()
                _ilGenerator.IL.OpCode(ILOpCode.Newobj);
                _ilGenerator.IL.Token(ctorRef);

                // Store the scope instance in a local variable for this scope
                var scopeLocalIndex = variables.CreateScopeInstance(scopeName);
                _ilGenerator.IL.StoreLocal(scopeLocalIndex.Address);
            }
        }

        /// <summary>
        /// Creates a constructor signature blob for parameterless constructors.
        /// </summary>
        private BlobHandle CreateConstructorSignature()
        {
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            return _ilGenerator.MetadataBuilder.GetOrAddBlob(sigBuilder);
        }


        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;

            // Step 1: Create scope instances for all scopes that have variables
            CreateScopeInstances(variables);

            // create the dispatch
            // functions are hosted so we need to declare them first
            _functionGenerator.DeclareFunctions(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());

            var loadDispatchTableMethod = _dispatchTableGenerator.GenerateLoadDispatchTableMethod();
            if (!loadDispatchTableMethod.IsNil)
            {
                _ilGenerator.IL.OpCode(ILOpCode.Call);
                _ilGenerator.IL.Token(loadDispatchTableMethod);
                _ilGenerator.InitializeLocalFunctionVariables(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());
            }

            _ilGenerator.GenerateStatements(ast.Body);

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

            FirstMethod = !_functionGenerator.FirstMethod.IsNil ? _functionGenerator.FirstMethod : _ilGenerator.FirstMethod;

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL,
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }
    }
}
