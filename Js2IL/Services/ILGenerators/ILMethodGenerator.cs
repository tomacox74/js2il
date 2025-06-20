using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates Intermediate Language (IL) code from a JavaScript Abstract Syntax Tree (AST) for a method
    /// </summary>
    internal class ILMethodGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;

        /*
         * Temporary exposure of private members until refactoring gets cleaner
         * need to determine what the difference is between generating the main method and generating any generic method
         */
        public Variables Variables => _variables;
        public BaseClassLibraryReferences BclReferences => _bclReferences;
        public MetadataBuilder MetadataBuilder => _metadataBuilder;

        public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
        }

        public void DeclareVariable(Acornima.Ast.VariableDeclaration variableDeclaraion, LocalVariablesEncoder localVariableEncoder, InstructionEncoder il)
        {
            // TODO need to handle multiple
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;

            // add the variable to the collection
            var variable = _variables.GetOrCreate(variableName);
            variable.LocalIndex = 0;

            // how do we know the type of the variable?
            // variable 0
            localVariableEncoder.AddVariable().Type().Object();

            // now we need to generate the expession portion
            if (variableAST.Init != null && variable.LocalIndex != null)
            {
                // otherwise we need to generate the expression
                GenerateExpression(variableAST.Init, _metadataBuilder, il, _bclReferences);
                il.StoreLocal(variable.LocalIndex.Value);
            }
        }

        public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement, InstructionEncoder il)
        { 
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
                    GenerateCallExpression(callExpression, il);
                    break;
                case Acornima.Ast.BinaryExpression binaryExpression:
                    // Handle BinaryExpression
                    GenerateBinaryExpression(binaryExpression, il);
                    break;
                case Acornima.Ast.UpdateExpression updateExpression:
                    // Handle UpdateExpression
                    GenerateUpdateExpression(updateExpression, il);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type in statement: {expressionStatement.Expression.Type}");
            }
        }

        public void GenerateForStatement(Acornima.Ast.ForStatement forStatement, LocalVariablesEncoder localVariableEncoder, InstructionEncoder il)
        {
            // first lets encode the initalizer
            if (forStatement.Init is Acornima.Ast.VariableDeclaration variableDeclaration)
            {
                DeclareVariable(variableDeclaration, localVariableEncoder, il);
            }
            else
            {
                throw new NotSupportedException($"Unsupported for statement initializer type: {forStatement.Init?.Type}");
            }

        }

        private void GenerateExpression(Acornima.Ast.Expression expression, MetadataBuilder metadataBuilder, InstructionEncoder il, BaseClassLibraryReferences bclReferences)
        {
            switch (expression)
            {
                case Acornima.Ast.BinaryExpression binaryExpression:
                    GenerateBinaryExpression(binaryExpression, il);
                    break;
                case Acornima.Ast.NumericLiteral numericLiteral:
                    // Load numeric literal
                    il.LoadConstantR8(numericLiteral.Value); 
                    
                    // box numeric values
                    il.OpCode(ILOpCode.Box);
                    il.Token(_bclReferences.DoubleType);

                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }

        private void GenerateBinaryExpression(Acornima.Ast.BinaryExpression binaryExpression, InstructionEncoder il)
        {
            // Assuming binaryExpression is a simple addition for now
            if (binaryExpression.Operator != Acornima.Operator.Addition)
            {
                throw new NotSupportedException($"Unsupported binary operator: {binaryExpression.Operator}");
            }

            if (binaryExpression.Left == null || binaryExpression.Right == null)
            {
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
                            il.LoadString(_metadataBuilder.GetOrAddUserString(numberAsString)); // Load numeric literal as string
                        }
                        else
                        {
                            il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                        }
                        break;
                    case Acornima.Ast.StringLiteral stringLiteral:
                        il.LoadString(_metadataBuilder.GetOrAddUserString(stringLiteral.Value)); // Load string literal
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
                il.Token(_bclReferences.DoubleType);

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
                var concatSig = _metadataBuilder.GetOrAddBlob(stringSig);

                // Add a MemberRef to Console.WriteLine(string)
                var stringConcatMethodRef = _metadataBuilder.AddMemberReference(
                    _bclReferences.StringType,
                    _metadataBuilder.GetOrAddString("Concat"),
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

        private void GenerateUpdateExpression(Acornima.Ast.UpdateExpression updateExpression, InstructionEncoder il)
        {
            if (updateExpression.Operator != Acornima.Operator.Increment || updateExpression.Prefix)
            {
                throw new NotSupportedException($"Unsupported update expression operator: {updateExpression.Operator} or prefix: {updateExpression.Prefix}");
            }
            // Handle postfix increment (e.g., x++)
            var variableName = (updateExpression.Argument as Acornima.Ast.Identifier)!.Name;
            var variable = _variables[variableName];
            // Load the variable
            il.LoadLocal(variable.LocalIndex!.Value);
            // unbox the variable
            il.OpCode(ILOpCode.Unbox_any);
            // Assuming the variable is a double because it is the only option that has parity with javascript numbers
            il.Token(_bclReferences.DoubleType); 
            // increment by 1
            il.LoadConstantR8(1.0);
            il.OpCode(ILOpCode.Add);
            // box the result back to an object
            il.OpCode(ILOpCode.Box);
            il.Token(_bclReferences.DoubleType);
            // Store the result back to the variable because it is a update expression
            il.StoreLocal(variable.LocalIndex.Value);
        }

        private void GenerateCallExpression(Acornima.Ast.CallExpression callExpression, InstructionEncoder il)
        {
            // For simplicity, we assume the call expression is a console write line
            if (callExpression.Callee is not Acornima.Ast.MemberExpression memberExpression ||
                memberExpression.Object is not Acornima.Ast.Identifier objectIdentifier ||
                objectIdentifier.Name != "console" ||
                memberExpression.Property is not Acornima.Ast.Identifier propertyIdentifier ||
                propertyIdentifier.Name != "log")
            {
                throw new NotSupportedException($"Unsupported call expression: {callExpression.Callee.Type}");
            }
            if (callExpression.Arguments.Count != 2)
            {
                throw new ArgumentException("console.log implementation supports two argument.");
            }

            CallConsoleWriteLine(callExpression, il);
        }

        private void CallConsoleWriteLine(Acornima.Ast.CallExpression callConsoleLog, InstructionEncoder il)
        {
            // use formatstring to append the additonal parameters
            var message = (callConsoleLog.Arguments[0] as Acornima.Ast.StringLiteral)!.Value + " {0}";
            var additionalParameterVariable = (callConsoleLog.Arguments[1] as Acornima.Ast.Identifier)!.Name;
            var variable = _variables.GetOrCreate(additionalParameterVariable);


            // Reference to System.Console
            var systemConsoleTypeReference = _metadataBuilder.AddTypeReference(
                _bclReferences.SystemConsoleAssembly,
                _metadataBuilder.GetOrAddString("System"),
                _metadataBuilder.GetOrAddString("Console"));

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
            var writeLineSig = _metadataBuilder.GetOrAddBlob(consoleSig);

            // Add a MemberRef to Console.WriteLine(string)
            var writeLineMemberRef = _metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                _metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);

            var messageHandle = _metadataBuilder.GetOrAddUserString(message);

            // Assuming Console.WriteLine(string, object) is available in the BCL references
            il.LoadString(messageHandle);

            // Load local 0 (which is assumed to be the int x)
            il.LoadLocal(variable.LocalIndex!.Value);

            il.OpCode(ILOpCode.Call);
            il.Token(writeLineMemberRef);
        }
    }
}
