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

        public MainGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder)
        {
            _ilGenerator = new ILMethodGenerator(variables, bclReferences, metadataBuilder, methodBodyStreamEncoder);
            this._methodBodyStreamEncoder = methodBodyStreamEncoder;
        }


        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;
            var bclReferences = _ilGenerator.BclReferences;

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

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL, 
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }
    }
}
