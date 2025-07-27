using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is teh entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private ILMethodGenerator _ilGenerator;
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
            this._methodBodyStreamEncoder = methodBodyStreamEncoder;
        }


        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;

            // create the dispatch
            // functions are hosted so we need to declare them first
            _ilGenerator.DeclareFunctions(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());

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

            FirstMethod = _ilGenerator.FirstMethod;

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL,
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }
    }
}
