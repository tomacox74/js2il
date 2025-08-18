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

        private IMethodExpressionEmitter _methodExpressionEmitter;

        public BinaryOperators(MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, IMethodExpressionEmitter methodExpressionEmitter, BaseClassLibraryReferences bclReferences, Runtime runtime)
        {
            _metadataBuilder = metadataBuilder;
            _il = il;
            _variables = variables;
            _bclReferences = bclReferences;
            _runtime = runtime;
            _methodExpressionEmitter = methodExpressionEmitter;
        }

        /// <summary>
        /// Loads a variable value using scope field access.
        /// </summary>
        public void LoadVariable(Variable variable)
        {
            // If this variable belongs to a parent/ancestor scope (captured), always load from scopes[]
            // to respect closure binding, regardless of any local scope instances that might exist.
            if (variable is ScopeVariable sv)
            {
                // scopes[sv.ParentScopeIndex].<field>
                _il.LoadArgument(0); // scopes array
                _il.LoadConstantI4(sv.ParentScopeIndex);
                _il.OpCode(ILOpCode.Ldelem_ref);
                _il.OpCode(ILOpCode.Ldfld);
                _il.Token(sv.FieldHandle);
                return;
            }
            if (variable.IsParameter)
            {
                // Directly load argument (already object). ParameterIndex already accounts for scopes[] at arg0
                _il.LoadArgument(variable.ParameterIndex);
                return;
            }
            // Scope field variable path
            var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
            }
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeLocalIndex.Address);
            }
            else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
            {
                _il.LoadArgument(0); // Load scope array parameter
                _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            _il.OpCode(ILOpCode.Ldfld);
            _il.Token(variable.FieldHandle);
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
                _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion());
                _il.OpCode(ILOpCode.Conv_i4);
                _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion());
                _il.OpCode(ILOpCode.Conv_i4);
            } else {
                // For + choose between string concat, numeric math, or runtime helper for dynamic types
                bool plus = operatorType == Operator.Addition;
                bool minus = operatorType == Operator.Subtraction;
                bool equality = operatorType == Operator.Equality;
                bool staticString = plus && binaryExpression.Left is StringLiteral && (binaryExpression.Right is StringLiteral || binaryExpression.Right is NumericLiteral);

                var leftType = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion());
                if (equality)
                {
                    if (leftType == JavascriptType.Number)
                    {
                        // Only unbox when the left operand value is boxed (variables/expressions),
                        // not when it's a raw numeric literal or simple unary numeric.
                        bool leftIsRawNumeric = binaryExpression.Left is Acornima.Ast.NumericLiteral
                            || (binaryExpression.Left is Acornima.Ast.UnaryExpression ul && ul.Operator.ToString() == "UnaryNegation" && ul.Argument is Acornima.Ast.NumericLiteral);
                        if (!leftIsRawNumeric)
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.DoubleType);
                        }
                    }
                    else if (leftType == JavascriptType.Boolean)
                    {
                        // If this is not a literal boolean, unbox (variables/expressions are boxed)
                        if (!(binaryExpression.Left is Acornima.Ast.BooleanLiteral) && !(binaryExpression.Left is Acornima.Ast.Literal bl && bl.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                        }
                    }
                    else
                    {
                        // If right is a boolean literal/value, coerce left to boolean as well (handles cases like: result == true)
                        if (binaryExpression.Right is Acornima.Ast.BooleanLiteral || (binaryExpression.Right is Acornima.Ast.Literal brl && brl.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                            leftType = JavascriptType.Boolean;
                        }
                    }
                    // otherwise leave as loaded (e.g., string/object) and Ceq will perform ref equality (acceptable for now)
                }
                else
                {
                    if (plus)
                    {
                        if (!staticString)
                        {
                            // Ensure left is boxed as object for runtime Add
                            if (leftType == JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.DoubleType);
                            }
                            else if (leftType == JavascriptType.Boolean)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.BooleanType);
                            }
                        }
                    }
                    else if (minus)
                    {
                        // For subtraction, route through runtime: ensure left is boxed object
                        if (leftType == JavascriptType.Number)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.DoubleType);
                        }
                        else if (leftType == JavascriptType.Boolean)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.BooleanType);
                        }
                        // strings/objects already objects
                    }
                    else if (leftType != JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType);
                    }
                }

                var rightType = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion() { toString = binaryExpression.Left is StringLiteral });
                if (equality)
                {
                    if (rightType == JavascriptType.Number)
                    {
                        // Only unbox when the right operand value is boxed (variables/expressions),
                        // not when it's a raw numeric literal or simple unary numeric.
                        bool rightIsRawNumeric = binaryExpression.Right is Acornima.Ast.NumericLiteral
                            || (binaryExpression.Right is Acornima.Ast.UnaryExpression ur && ur.Operator.ToString() == "UnaryNegation" && ur.Argument is Acornima.Ast.NumericLiteral);
                        if (!rightIsRawNumeric)
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.DoubleType);
                        }
                    }
                    else if (rightType == JavascriptType.Boolean)
                    {
                        if (!(binaryExpression.Right is Acornima.Ast.BooleanLiteral) && !(binaryExpression.Right is Acornima.Ast.Literal br && br.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                        }
                    }
                    else
                    {
                        // If left evaluated as boolean (or is a boolean literal), coerce right to boolean
                        if (leftType == JavascriptType.Boolean || binaryExpression.Left is Acornima.Ast.BooleanLiteral || (binaryExpression.Left is Acornima.Ast.Literal lbl && lbl.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                            rightType = JavascriptType.Boolean;
                        }
                    }
                }
                else
                {
                    if (plus)
                    {
                        if (!staticString)
                        {
                            // Ensure right is boxed as object for runtime Add
                            if (rightType == JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.DoubleType);
                            }
                            else if (rightType == JavascriptType.Boolean)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.BooleanType);
                            }
                        }
                    }
                    else if (minus)
                    {
                        // For subtraction, route through runtime: ensure right is boxed object
                        if (rightType == JavascriptType.Number)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.DoubleType);
                        }
                        else if (rightType == JavascriptType.Boolean)
                        {
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(_bclReferences.BooleanType);
                        }
                    }
                    else if (rightType != JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType);
                    }
                }
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
            else if (op == Operator.Addition)
            {
                // General '+' path: call runtime Operators.Add(object, object) which implements JS semantics
                _runtime.InvokeOperatorsAdd();
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
                            if (op == Operator.Subtraction)
                            {
                                // Route subtraction to runtime to honor JS ToNumber coercion (e.g., "a"-"b" => NaN)
                                _runtime.InvokeOperatorsSubtract();
                                break;
                            }
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
    }
}
