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
        public static int GenerateMethod(Acornima.Ast.Program ast, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStream, BaseClassLibraryReferences bclReferences, Variables variables)
        {
            // local variables
            var localSig = new BlobBuilder();
            var localVariableEncoder = new BlobEncoder(localSig).LocalVariableSignature(1);

            // il code
            var methodIl = new BlobBuilder();
            var il = new InstructionEncoder(methodIl);

            foreach (var expression in ast.Body)
            {
                switch (expression) {
                    case Acornima.Ast.VariableDeclaration:
                        DeclareVariable((expression as Acornima.Ast.VariableDeclaration)!, variables, bclReferences, localVariableEncoder, metadataBuilder, il);
                        break;
                    case Acornima.Ast.ExpressionStatement:
                        GenerateExpressionStatement((expression as Acornima.Ast.ExpressionStatement)!, metadataBuilder, il, variables, bclReferences);
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

        private static void GenerateExpression(Acornima.Ast.Expression expression, MetadataBuilder metadataBuilder, InstructionEncoder il, BaseClassLibraryReferences bclReferences)
        {
            switch (expression) {
                case Acornima.Ast.BinaryExpression binaryExpression:
                    GenerateBinaryExpression(binaryExpression, metadataBuilder, il, bclReferences);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }

        private static void DeclareVariable(Acornima.Ast.VariableDeclaration variableDeclaraion, Variables variables, BaseClassLibraryReferences bclReferences, LocalVariablesEncoder localVariableEncoder, MetadataBuilder metadataBuilder, InstructionEncoder il)
        {
            // TODO need to handle multiple
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;

            // add the variable to the collection
            var variable = variables.GetOrCreate(variableName);
            variable.LocalIndex = 0;

            // how do we know the type of the variable?
            // variable 0
            localVariableEncoder.AddVariable().Type().Object();

            // now we need to generate the expession portion
            if (variableAST.Init != null && variable.LocalIndex != null) {
                // otherwise we need to generate the expression
                GenerateExpression(variableAST.Init, metadataBuilder, il, bclReferences);
                il.StoreLocal(variable.LocalIndex.Value);
            }
        }

        private static void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement, MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, BaseClassLibraryReferences bclReferences)
        {
            if (expressionStatement.Expression is Acornima.Ast.CallExpression callExpression) {
                // Handle CallExpression
                GenerateCallExpression(callExpression, metadataBuilder, il, variables, bclReferences);
            } else if (expressionStatement.Expression is Acornima.Ast.BinaryExpression binaryExpression) {
                // Handle BinaryExpression
                GenerateBinaryExpression(binaryExpression, metadataBuilder, il, bclReferences);
            } else {
                throw new NotSupportedException($"Unsupported expression type in statement: {expressionStatement.Expression.Type}");
            }
        }

        private static void GenerateCallExpression(Acornima.Ast.CallExpression callExpression, MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, BaseClassLibraryReferences bclReferences)
        {
            // For simplicity, we assume the call expression is a console write line
            if (callExpression.Callee is not Acornima.Ast.MemberExpression memberExpression || 
                memberExpression.Object is not Acornima.Ast.Identifier objectIdentifier || 
                objectIdentifier.Name != "console" ||
                memberExpression.Property is not Acornima.Ast.Identifier propertyIdentifier ||
                propertyIdentifier.Name != "log") {
                throw new NotSupportedException($"Unsupported call expression: {callExpression.Callee.Type}");
            }
            if (callExpression.Arguments.Count != 2) {
                throw new ArgumentException("console.log implementation supports two argument.");
            }
            CallConsoleWriteLine(callExpression, variables, bclReferences, metadataBuilder, il);
        }

        private static void GenerateBinaryExpression(Acornima.Ast.BinaryExpression binaryExpression, MetadataBuilder metadataBuilder, InstructionEncoder il, BaseClassLibraryReferences bclReferences)
        {
            // Assuming binaryExpression is a simple addition for now
            if (binaryExpression.Operator != Acornima.Operator.Addition) {
                throw new NotSupportedException($"Unsupported binary operator: {binaryExpression.Operator}");
            }

            if (binaryExpression.Left == null || binaryExpression.Right == null) {
                throw new ArgumentException("Binary expression must have both left and right operands.");
            }

            var loadLiteral = (Acornima.Ast.Expression literalExpression, bool forceString = false) =>
            {
                switch (literalExpression)
                {
                    case Acornima.Ast.NumericLiteral numericLiteral:
                        if (forceString)
                        {
                            //does dotnet ToString behave the same as JavaScript?
                            var numberAsString = numericLiteral.Value.ToString();
                            il.LoadString(metadataBuilder.GetOrAddUserString(numberAsString)); // Load numeric literal as string
                        }
                        else
                        {
                            il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                        }
                        break;
                    case Acornima.Ast.StringLiteral stringLiteral:
                        il.LoadString(metadataBuilder.GetOrAddUserString(stringLiteral.Value)); // Load string literal
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported expression type: {literalExpression.Type}");
                }
            };

            loadLiteral(binaryExpression.Left);
            loadLiteral(binaryExpression.Right, binaryExpression.Left is Acornima.Ast.StringLiteral);

            if (binaryExpression.Left is Acornima.Ast.NumericLiteral && binaryExpression.Right is Acornima.Ast.NumericLiteral)
            {
                // If both are numeric literals, we can directly add them
                il.OpCode(ILOpCode.Add);

                // box numeric values
                il.OpCode(ILOpCode.Box);
                il.Token(bclReferences.DoubleType);

            }
            else if (binaryExpression.Left is Acornima.Ast.StringLiteral && (binaryExpression.Right is Acornima.Ast.StringLiteral || binaryExpression.Right is Acornima.Ast.NumericLiteral))
            {

                // Create method signature: string Concat(string, string)
                var stringSig = new BlobBuilder();
                new BlobEncoder(stringSig)
                    .MethodSignature(isInstanceMethod: false)
                    .Parameters(2,
                        returnType => returnType.Type().String(),
                        parameters => {
                            parameters.AddParameter().Type().String();
                            parameters.AddParameter().Type().String();
                        });
                var concatSig = metadataBuilder.GetOrAddBlob(stringSig);

                // Add a MemberRef to Console.WriteLine(string)
                var stringConcatMethodRef = metadataBuilder.AddMemberReference(
                    bclReferences.StringType,
                    metadataBuilder.GetOrAddString("Concat"),
                    concatSig);


                // If either is a string literal, we need to concatenate them
                il.OpCode(ILOpCode.Call);
                il.Token(stringConcatMethodRef);
            }
            else
            {
                throw new NotSupportedException($"Unsupported binary expression types: {binaryExpression.Left.Type} and {binaryExpression.Right.Type}");
            }
        }

        private static void CallConsoleWriteLine(Acornima.Ast.CallExpression callConsoleLog, Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, InstructionEncoder il)
        {
            // use formatstring to append the additonal parameters
            var message = (callConsoleLog.Arguments[0] as Acornima.Ast.StringLiteral)!.Value + " {0}";
            var additionalParameterVariable = (callConsoleLog.Arguments[1] as Acornima.Ast.Identifier)!.Name;
            var variable = variables.GetOrCreate(additionalParameterVariable);


            // Reference to System.Console
            var systemConsoleTypeReference = metadataBuilder.AddTypeReference(
                bclReferences.SystemConsole,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Console"));

            // Create method signature: void WriteLine(string)
            var consoleSig = new BlobBuilder();
            new BlobEncoder(consoleSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });
            var writeLineSig = metadataBuilder.GetOrAddBlob(consoleSig);

            // Add a MemberRef to Console.WriteLine(string)
            var writeLineMemberRef = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);

            var messageHandle = metadataBuilder.GetOrAddUserString(message);

            // Assuming Console.WriteLine(string, object) is available in the BCL references
            il.LoadString(messageHandle);

            // Load local 0 (which is assumed to be the int x)
            il.LoadLocal(variable.LocalIndex!.Value);

            il.OpCode(ILOpCode.Call);
            il.Token(writeLineMemberRef);
        }
    }
}
