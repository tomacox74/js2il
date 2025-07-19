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
        private Runtime _runtime;

        public BinaryOperators(MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, BaseClassLibraryReferences bclReferences, Runtime runtime)
        {
            _metadataBuilder = metadataBuilder;
            _il = il;
            _variables = variables;
            _bclReferences = bclReferences;
            _runtime = runtime;
        }

        public void Generate(BinaryExpression binaryExpression, ConditionalBranching? branching = null)
        {
            if (binaryExpression.Left == null || binaryExpression.Right == null)
            {
                throw new ArgumentException("Binary expression must have both left and right operands.");
            }

            var operatorType = binaryExpression.Operator;

            bool isBitwiseOrShift = operatorType == Operator.BitwiseAnd || operatorType == Operator.BitwiseOr || operatorType == Operator.BitwiseXor ||
                                    operatorType == Operator.LeftShift || operatorType == Operator.RightShift || operatorType == Operator.UnsignedRightShift;
            if (isBitwiseOrShift) {
                LoadValue(binaryExpression.Left, new TypeCoercion());
                _il.OpCode(ILOpCode.Conv_i4);
                LoadValue(binaryExpression.Right, new TypeCoercion());
                _il.OpCode(ILOpCode.Conv_i4);
            } else {
                LoadValue(binaryExpression.Left, new TypeCoercion());
                LoadValue(binaryExpression.Right, new TypeCoercion() { toString = binaryExpression.Left is StringLiteral });
            }

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
                    ApplyArithmeticOperator(operatorType, binaryExpression, isBitwiseOrShift);
                    break;
                case Operator.LessThan:
                case Operator.GreaterThan:
                case Operator.LessThanOrEqual:
                case Operator.GreaterThanOrEqual:
                case Operator.Equality:
                    ApplyComparisonOperator(operatorType, branching);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported binary operator: {operatorType}");
            }
        }

        private void ApplyArithmeticOperator(Operator op, BinaryExpression binaryExpression, bool isBitwiseOrShift = false)
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
                            Operator.UnsignedRightShift => ILOpCode.Shr_un,
                            _ => throw new NotSupportedException($"Unsupported bitwise operator: {op}")
                        };
                        _il.OpCode(bitwiseOpCode);
                        if (op == Operator.UnsignedRightShift) {
                            // Convert result to uint32 then to double for JS >>>
                            _il.OpCode(ILOpCode.Conv_u4);
                        }
                        _il.OpCode(ILOpCode.Conv_r8);
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

        private void ApplyComparisonOperator(Operator op, ConditionalBranching? branching = null)
        {
            ILOpCode compareOpCode;
            ILOpCode branchOpCode;
            switch (op)
            {
                case Operator.LessThan:
                    compareOpCode = ILOpCode.Clt;
                    branchOpCode = ILOpCode.Blt;
                    break;
                case Operator.GreaterThan:
                    compareOpCode = ILOpCode.Cgt;
                    branchOpCode = ILOpCode.Bgt;
                    break;
                case Operator.LessThanOrEqual:
                    compareOpCode = ILOpCode.Cgt;
                    branchOpCode = ILOpCode.Ble;
                    break;
                case Operator.GreaterThanOrEqual:
                    compareOpCode = ILOpCode.Clt;
                    branchOpCode = ILOpCode.Bge;
                    break;
                case Operator.Equality:
                    compareOpCode = ILOpCode.Ceq;
                    branchOpCode = ILOpCode.Beq;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported comparison operator: {op}");
            }

            if (branching == null)
            {
                if (op == Operator.LessThanOrEqual || op == Operator.GreaterThanOrEqual)
                {
                    // For <=: !(a > b) => (a > b) == false
                    // For >=: !(a < b) => (a < b) == false
                    // We'll emit the opposite comparison and negate the result
                    _il.OpCode(compareOpCode); // compare

                    // Negate the result (0 -> 1, 1 -> 0)
                    _il.OpCode(ILOpCode.Ldc_i4_0);
                    _il.OpCode(ILOpCode.Ceq);
                    // box as boolean
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.BooleanType);
                }
                else
                {
                    _il.OpCode(compareOpCode); // compare
                    // box it as a boolean result
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.BooleanType);
                }
            }
            else
            {
                var matchBranch = branching.BranchOnTrue;
                var notMatchBranch = branching.BranchOnFalse;

                _il.Branch(branchOpCode, matchBranch); // branch if condition met
                if (notMatchBranch.HasValue)
                {
                    _il.Branch(ILOpCode.Br, notMatchBranch.Value);
                }
                else
                {
                    _il.OpCode(ILOpCode.Pop); // pop the result of the comparison
                }
            }
        }

        /// <summary>
        /// for loading literal expresions onto the IL stack.
        /// i.e. 
        /// x = 5;
        /// x = "hello world";
        /// x = true;
        /// </summary>
        /// <remarks>
        /// This does not belong here.. need to refactor
        /// </remarks>
        public JavascriptType LoadValue(Expression expression, TypeCoercion typeCoercion)
        {
            JavascriptType type = JavascriptType.Unknown;

            switch (expression)
            {
                case Acornima.Ast.NumericLiteral numericLiteral:
                    if (typeCoercion.toString)
                    {
                        //does dotnet ToString behave the same as JavaScript?
                        var numberAsString = numericLiteral.Value.ToString();
                        _il.LoadString(_metadataBuilder.GetOrAddUserString(numberAsString)); // Load numeric literal as string
                    }
                    else
                    {
                        _il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                    }

                    type = JavascriptType.Number;

                    break;
                case Acornima.Ast.StringLiteral stringLiteral:
                    _il.LoadString(_metadataBuilder.GetOrAddUserString(stringLiteral.Value)); // Load string literal
                    break;
                case Acornima.Ast.Identifier identifier:
                    var name = identifier.Name;
                    var variable = _variables[name];
                    _il.LoadLocal(variable.LocalIndex!.Value); // Load variable

                    // this is fragile at the moment.. need to handle all types
                    // need a runtime type check for unknown types
                    // and unboxing for boolean
                    if (variable.Type != JavascriptType.Object && !typeCoercion.boxed)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType); // unbox the variable as a double
                    }

                    type = variable.Type;

                    break;
                case Acornima.Ast.UnaryExpression unaryExpression:
                    // Handle unary expressions like -16
                    if (unaryExpression.Operator.ToString() == "UnaryNegation" && unaryExpression.Argument is Acornima.Ast.NumericLiteral numericArg)
                    {
                        if (typeCoercion.toString)
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
                case MemberExpression memberExpression:
                    LoadValue(memberExpression.Object, new TypeCoercion()); // Load the object part of the member expression

                    // temporary.. need make generic for any object
                    if (memberExpression.Property is Identifier propertyIdentifier)
                    {
                        if (propertyIdentifier.Name == "length")
                        {
                            _runtime.InvokeArrayGetCount();
                            type = JavascriptType.Number;
                        }
                        else if (memberExpression.Computed)
                        {
                            // computed means someObject["propertyName"] or someObject[someIndex]
                            LoadValue(propertyIdentifier, new TypeCoercion());
                            _runtime.InvokeGetItemFromObject();
                            type = JavascriptType.Object;
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported member property expression: {memberExpression.Property}");
                        }

                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported member expression: {memberExpression.Property}");
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }

            return type;
        }
    }
}
