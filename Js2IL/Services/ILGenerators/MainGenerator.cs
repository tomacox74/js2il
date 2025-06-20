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

            // il code
            var methodIl = new BlobBuilder();
            var il = new InstructionEncoder(methodIl);

            foreach (var expression in ast.Body)
            {
                switch (expression) {
                    case Acornima.Ast.VariableDeclaration variableDeclaration:
                        _ilGenerator.DeclareVariable(variableDeclaration, localVariableEncoder, il);
                        break;
                    case Acornima.Ast.ExpressionStatement expressionStatement:
                        _ilGenerator.GenerateExpressionStatement(expressionStatement, il);
                        break;
                    case Acornima.Ast.ForStatement forStatement:
                        _ilGenerator.GenerateForStatement(forStatement, localVariableEncoder, il);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported AST node type: {expression.Type}");
                }
            }

            il.OpCode(ILOpCode.Ret);

            var localSignature = metadataBuilder.AddStandaloneSignature(metadataBuilder.GetOrAddBlob(localSig));

            return methodBodyStream.AddMethodBody(
                il, 
                localVariablesSignature: localSignature,
                attributes: MethodBodyAttributes.InitLocals);
        }
    }
}
