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

        public MainGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder)
        {
            _ilGenerator = new ILMethodGenerator(variables, bclReferences, metadataBuilder);
        }


        public int GenerateMethod(Acornima.Ast.Program ast, MethodBodyStreamEncoder methodBodyStream)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;
            var bclReferences = _ilGenerator.BclReferences;

            _ilGenerator.GenerateStatements(ast.Body);

            _ilGenerator.IL.OpCode(ILOpCode.Ret);

            // local variables
            int numberOfLocals = variables.GetNumberOfLocals();
            var localSig = new BlobBuilder();
            var localVariableEncoder = new BlobEncoder(localSig).LocalVariableSignature(numberOfLocals);
            for (int i = 0; i < numberOfLocals; i++)
            {
                localVariableEncoder.AddVariable().Type().Object();
            }

            var localSignature = metadataBuilder.AddStandaloneSignature(metadataBuilder.GetOrAddBlob(localSig));

            return methodBodyStream.AddMethodBody(
                _ilGenerator.IL, 
                localVariablesSignature: localSignature,
                attributes: MethodBodyAttributes.InitLocals);
        }
    }
}
