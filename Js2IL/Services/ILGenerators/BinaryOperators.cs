using Acornima;
using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates IL for binary operators in JavaScript.
    /// </summary>
    internal class BinaryOperators
    {
        // for building the assembly
        private MetadataBuilder _metadataBuilder;
        private InstructionEncoder _il;

        // shared state for all generators
        private BaseClassLibraryReferences _bclReferences;
        private Variables _variables;

        public BinaryOperators(MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, BaseClassLibraryReferences bclReferences)
        {
            _metadataBuilder = metadataBuilder;
            _il = il;
            _variables = variables;
            _bclReferences = bclReferences;
        }

        public void Generate(BinaryExpression binaryExpression, LabelHandle? matchBranch, LabelHandle? notMatchBranch)
        {
            if (binaryExpression.Left == null || binaryExpression.Right == null)
            {
                throw new ArgumentException("Binary expression must have both left and right operands.");
            }

            var operatorType = binaryExpression.Operator;

            // For all operators, just load the operands as-is (as r8)
            LoadValue(binaryExpression.Left);
            LoadValue(binaryExpression.Right, binaryExpression.Left is StringLiteral);

            switch (operatorType)
            {
                case Operator.Addition:
                case Operator.Subtraction:
                case Operator.Multiplication:
                case Operator.Division:
                case Operator.Remainder:
                case Operator.Exponentiation:
                case Operator.BitwiseAnd:
                case Operator.BitwiseOr:
                case Operator.BitwiseXor:
                case Operator.LeftShift:
                case Operator.RightShift:
                case Operator.UnsignedRightShift:
                    // Handle arithmetic operators
                    ApplyArithmeticOperator(operatorType, binaryExpression);
                    break;
                case Operator.LessThan:
                    // Handle less than operator
                    ApplyComparisonOperator(Operator.LessThan, matchBranch, notMatchBranch);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported binary operator: {operatorType}");
            }
        }

        private void ApplyArithmeticOperator(Operator op, BinaryExpression binaryExpression)
        {
            var isStaticString = binaryExpression.Left is Acornima.Ast.StringLiteral && (binaryExpression.Right is Acornima.Ast.StringLiteral || binaryExpression.Right is Acornima.Ast.NumericLiteral);
            if (isStaticString && op == Operator.Addition)
            {
                // Create method signature: string Concat(string, string)
                var stringSig = new BlobBuilder();
                new BlobEncoder(stringSig)
                    .MethodSignature(isInstanceMethod: false)
                    .Parameters(2,
                        returnType => returnType.Type().String(),
                        parameters =>
                        {
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
                _il.OpCode(ILOpCode.Call);
                _il.Token(stringConcatMethodRef);
            }
            else
            {
                switch (op)
                {
                    case Operator.Addition:
                    case Operator.Subtraction:
                    case Operator.Multiplication:
                    case Operator.Division:
                    case Operator.Remainder:
                        var opCode = op switch
                        {
                            Operator.Addition => ILOpCode.Add,
                            Operator.Subtraction => ILOpCode.Sub,
                            Operator.Multiplication => ILOpCode.Mul,
                            Operator.Division => ILOpCode.Div,
                            Operator.Remainder => ILOpCode.Rem,
                            _ => throw new NotSupportedException($"Unsupported arithmetic operator: {op}")
                        };
                        _il.OpCode(opCode);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType);
                        break;
                    case Operator.BitwiseAnd:
                    case Operator.BitwiseOr:
                    case Operator.BitwiseXor:
                    case Operator.LeftShift:
                    case Operator.RightShift:
                    case Operator.UnsignedRightShift:
                        var bitwiseOpCode = op switch
                        {
                            Operator.BitwiseAnd => ILOpCode.And,
                            Operator.BitwiseOr => ILOpCode.Or,
                            Operator.BitwiseXor => ILOpCode.Xor,
                            Operator.LeftShift => ILOpCode.Shl,
                            Operator.RightShift => ILOpCode.Shr,
                            Operator.UnsignedRightShift => ILOpCode.Shr, // Use signed shift to match expected IL
                            _ => throw new NotSupportedException($"Unsupported bitwise operator: {op}")
                        };
                        _il.OpCode(bitwiseOpCode);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType);
                        break;
                    case Operator.Exponentiation:
                        // Exponentiation requires calling Math.Pow()
                        ApplyExponentiationOperator();
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported arithmetic operator: {op}");
                }
            }
        }

        private void ApplyExponentiationOperator()
        {
            // Create method signature: double Pow(double, double)
            var powSig = new BlobBuilder();
            new BlobEncoder(powSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Type().Double(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().Double();
                        parameters.AddParameter().Type().Double();
                    });
            var powMethodSig = _metadataBuilder.GetOrAddBlob(powSig);

            // Add a MemberRef to Math.Pow(double, double)
            var mathPowMethodRef = _metadataBuilder.AddMemberReference(
                _bclReferences.SystemMathType,
                _metadataBuilder.GetOrAddString("Pow"),
                powMethodSig);

            // Call Math.Pow
            _il.OpCode(ILOpCode.Call);
            _il.Token(mathPowMethodRef);

            // box the result as a double
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
        }

        private void ApplyComparisonOperator(Operator op, LabelHandle? matchBranch, LabelHandle? notMatchBranch)
        {
            switch (op)
            {
                case Operator.LessThan:
                    if (!matchBranch.HasValue)
                    {
                        _il.OpCode(ILOpCode.Clt); // compare less than
                        // box it as a boolean result
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.BooleanType);
                    }
                    else
                    {
                        // instead of a comparison which outputs a 1 or 0 we want to branch
                        _il.Branch(ILOpCode.Blt, matchBranch.Value); // branch if less than
                        if (notMatchBranch.HasValue)
                        {
                            // if we have a not match branch, we need to branch there as well
                            _il.Branch(ILOpCode.Br, notMatchBranch.Value);
                        }
                        else
                        {
                            // if we don't have a not match branch, we just continue
                            _il.OpCode(ILOpCode.Pop); // pop the result of the comparison
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported comparison operator: {op}");
            }
        }

        private void LoadValue(Expression expression, bool forceString = false)
        {
            switch (expression)
            {
                case Acornima.Ast.NumericLiteral numericLiteral:
                    if (forceString)
                    {
                        //does dotnet ToString behave the same as JavaScript?
                        var numberAsString = numericLiteral.Value.ToString();
                        _il.LoadString(_metadataBuilder.GetOrAddUserString(numberAsString)); // Load numeric literal as string
                    }
                    else
                    {
                        _il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                    }
                    break;
                case Acornima.Ast.StringLiteral stringLiteral:
                    _il.LoadString(_metadataBuilder.GetOrAddUserString(stringLiteral.Value)); // Load string literal
                    break;
                case Acornima.Ast.Identifier identifier:
                    var name = identifier.Name;
                    var variable = _variables[name];
                    _il.LoadLocal(variable.LocalIndex!.Value); // Load variable

                    // this is fragile at the moment.. need to handle all types and not assume it is a number
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.DoubleType); // unbox the variable as a double

                    break;
                case Acornima.Ast.UnaryExpression unaryExpression:
                    // Handle unary expressions like -16
                    if (unaryExpression.Operator.ToString() == "UnaryNegation" && unaryExpression.Argument is Acornima.Ast.NumericLiteral numericArg)
                    {
                        if (forceString)
                        {
                            var numberAsString = (-numericArg.Value).ToString();
                            _il.LoadString(_metadataBuilder.GetOrAddUserString(numberAsString));
                        }
                        else
                        {
                            _il.LoadConstantR8(-numericArg.Value);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported unary expression: {unaryExpression.Operator}");
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }
    }
}
