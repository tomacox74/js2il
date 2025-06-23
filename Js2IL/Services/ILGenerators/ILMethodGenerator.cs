using Acornima;
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
        private InstructionEncoder _il;
        private BinaryOperators _binaryOperators;

        /*
         * Temporary exposure of private members until refactoring gets cleaner
         * need to determine what the difference is between generating the main method and generating any generic method
         */
        public Variables Variables => _variables;
        public BaseClassLibraryReferences BclReferences => _bclReferences;
        public MetadataBuilder MetadataBuilder => _metadataBuilder;
        public InstructionEncoder IL => _il;

        public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            var methodIl = new BlobBuilder();
            _il = new InstructionEncoder(methodIl, new ControlFlowBuilder());
            _binaryOperators = new BinaryOperators(metadataBuilder, _il, variables, bclReferences);
        }

        public void DeclareVariable(VariableDeclaration variableDeclaraion)
        {
            // TODO need to handle multiple
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;

            // add the variable to the collection
            var variable = _variables.CreateLocal(variableName);

            // now we need to generate the expession portion
            if (variableAST.Init != null && variable.LocalIndex != null)
            {
                // otherwise we need to generate the expression
                GenerateExpression(variableAST.Init, null, null);
                _il.StoreLocal(variable.LocalIndex.Value);
            }
        }

        public void GenerateStatements(NodeList<Statement> statements)
        {
            // Iterate through each statement in the block
            foreach (var statement in statements)
            {
                GenerateStatement(statement);
            }
        }

        public void GenerateStatement(Statement statement)
        {
            switch (statement)
            {
                case VariableDeclaration variableDeclaration:
                    DeclareVariable(variableDeclaration);
                    break;
                case ExpressionStatement expressionStatement:
                    GenerateExpressionStatement(expressionStatement);
                    break;
                case ForStatement forStatement:
                    GenerateForStatement(forStatement);
                    break;
                case BlockStatement blockStatement:
                    // Handle BlockStatement
                    GenerateStatements(blockStatement.Body);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {statement.Type}");
            }
        }

        public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement)
        { 
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
                    GenerateCallExpression(callExpression);
                    break;
                case Acornima.Ast.BinaryExpression binaryExpression:
                    // Handle BinaryExpression
                    _binaryOperators.Generate(binaryExpression, null, null);
                    break;
                case Acornima.Ast.UpdateExpression updateExpression:
                    // Handle UpdateExpression
                    GenerateUpdateExpression(updateExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type in statement: {expressionStatement.Expression.Type}");
            }
        }

        public void GenerateForStatement(Acornima.Ast.ForStatement forStatement)
        {
            // first lets encode the initalizer
            if (forStatement.Init is Acornima.Ast.VariableDeclaration variableDeclaration)
            {
                DeclareVariable(variableDeclaration);
            }
            else
            {
                throw new NotSupportedException($"Unsupported for statement initializer type: {forStatement.Init?.Type}");
            }

            // the labels used in the loop flow control
            var loopStartLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();
            var loopBodyLabel = _il.DefineLabel();

            _il.MarkLabel(loopStartLabel);

            //the test condition in the for loop
            if (forStatement.Test != null)
            {
                GenerateExpression(forStatement.Test, loopBodyLabel, loopEndLabel);
            }

            // now the body
            _il.MarkLabel(loopBodyLabel);

            GenerateStatement(forStatement.Body);

            if (forStatement.Update != null)
            {
                GenerateExpression(forStatement.Update, null, null);
            }

            // branch back to the start of the loop
            _il.Branch(ILOpCode.Br, loopStartLabel);

            // here is the end
            _il.MarkLabel(loopEndLabel);
        }

        /// <summary>
        /// Generate the IL for a expression
        /// </summary>
        /// <param name="expression">the expression to generate the IL for</param>
        /// <param name="matchBranch">optional, this is when the expression is for control flow</param>
        /// <param name="notMatchBranch">optional, this is the expression is for control flow</param>
        /// <exception cref="NotSupportedException"></exception>
        private void GenerateExpression(Expression expression, LabelHandle? matchBranch, LabelHandle? notMatchBranch)
        {
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    _binaryOperators.Generate(binaryExpression, matchBranch, notMatchBranch);
                    break;
                case NumericLiteral numericLiteral:
                    // Load numeric literal
                    _il.LoadConstantR8(numericLiteral.Value);
                    
                    // box numeric values
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);

                    break;
                case UpdateExpression updateExpression:
                    GenerateUpdateExpression(updateExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }

        private void GenerateUpdateExpression(Acornima.Ast.UpdateExpression updateExpression)
        {
            if ((updateExpression.Operator != Acornima.Operator.Increment && updateExpression.Operator != Acornima.Operator.Decrement) || updateExpression.Prefix)
            {
                throw new NotSupportedException($"Unsupported update expression operator: {updateExpression.Operator} or prefix: {updateExpression.Prefix}");
            }
            // Handle postfix increment (x++) and decrement (x--)
            var variableName = (updateExpression.Argument as Acornima.Ast.Identifier)!.Name;
            var variable = _variables[variableName];
            // Load the variable
            _il.LoadLocal(variable.LocalIndex!.Value);
            // unbox the variable
            _il.OpCode(ILOpCode.Unbox_any);
            // Assuming the variable is a double because it is the only option that has parity with javascript numbers
            _il.Token(_bclReferences.DoubleType);
            // increment or decrement by 1
            _il.LoadConstantR8(1.0);
            if (updateExpression.Operator == Acornima.Operator.Increment)
            {
                _il.OpCode(ILOpCode.Add);
            }
            else // Decrement
            {
                _il.OpCode(ILOpCode.Sub);
            }
            // box the result back to an object
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
            // Store the result back to the variable because it is a update expression
            _il.StoreLocal(variable.LocalIndex.Value);
        }

        private void GenerateCallExpression(Acornima.Ast.CallExpression callExpression)
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

            CallConsoleWriteLine(callExpression);
        }

        private void CallConsoleWriteLine(Acornima.Ast.CallExpression callConsoleLog)
        {
            // use formatstring to append the additonal parameters
            var message = (callConsoleLog.Arguments[0] as Acornima.Ast.StringLiteral)!.Value + " {0}";
            var additionalParameterVariable = (callConsoleLog.Arguments[1] as Acornima.Ast.Identifier)!.Name;
            var variable = _variables.Get(additionalParameterVariable);

            var messageHandle = _metadataBuilder.GetOrAddUserString(message);

            // Assuming Console.WriteLine(string, object) is available in the BCL references
            _il.LoadString(messageHandle);

            // Load local 0 (which is assumed to be the int x)
            _il.LoadLocal(variable.LocalIndex!.Value);

            _il.OpCode(ILOpCode.Call);
            _il.Token(_bclReferences.ConsoleWriteLine_StringObject_Ref);
        }
    }
}
