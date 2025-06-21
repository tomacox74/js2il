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

            // local variables
            var localSig = new BlobBuilder();
            var localVariableEncoder = new BlobEncoder(localSig).LocalVariableSignature(1);

            _ilGenerator.GenerateStatements(ast.Body, localVariableEncoder);

            _ilGenerator.IL.OpCode(ILOpCode.Ret);

            var localSignature = metadataBuilder.AddStandaloneSignature(metadataBuilder.GetOrAddBlob(localSig));

            return methodBodyStream.AddMethodBody(
                _ilGenerator.IL, 
                localVariablesSignature: localSignature,
                attributes: MethodBodyAttributes.InitLocals);
        }
    }
}
