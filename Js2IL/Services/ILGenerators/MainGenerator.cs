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
        public static int GenerateMethod(Acornima.Ast.Program ast, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStream, BaseClassLibraryReferences bclReferences)
        {
            var methodIl = new BlobBuilder();
            var il = new InstructionEncoder(methodIl);

            callConsoleWriteLine(bclReferences, metadataBuilder, il, "Hello World!");

            /*
            foreach (var expression in ast.Body)
            {
                switch (expression.Type) {
                    case Acornima.Ast.NodeType.ExpressionStatement:
                        var exprStmt = (Acornima.Ast.ExpressionStatement)expression;
                        if (exprStmt.Expression.Type == Acornima.Ast.NodeType.CallExpression)
                        {

                            //var callExpr = (Acornima.Ast.CallExpression)exprStmt.Expression;
                            //il.OpCode(ILOpCode.Call, methodBodyStream.GetMethodHandle(callExpr.Callee));
                            foreach (var arg in callExpr.Arguments)
                            {
                                // Assuming all arguments are pushed onto the stack
                                il.OpCode(ILOpCode.Ldarg, methodBodyStream.GetArgumentHandle(arg));
                            }
                        }
                        break;
                }
            }
            */

            il.OpCode(ILOpCode.Ret);
            return methodBodyStream.AddMethodBody(il);
        }

        private static void callConsoleWriteLine(BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, InstructionEncoder il, string message)
        {
            // Reference to System.Console
            var systemConsoleTypeReference = metadataBuilder.AddTypeReference(
                bclReferences.SystemConsole,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Console"));

            // Create method signature: void WriteLine(string)
            var consoleSig = new BlobBuilder();
            new BlobEncoder(consoleSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(1,
                    returnType => returnType.Void(),
                    parameters => parameters.AddParameter().Type().String());
            var writeLineSig = metadataBuilder.GetOrAddBlob(consoleSig);

            // Add a MemberRef to Console.WriteLine(string)
            var writeLineMemberRef = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);

            var messageHandle = metadataBuilder.GetOrAddUserString(message);

            // Assuming Console.WriteLine(string) is available in the BCL references
            il.LoadString(messageHandle);
            il.OpCode(ILOpCode.Call);
            il.Token(writeLineMemberRef);
        }
    }
}
