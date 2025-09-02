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
                bool equality = operatorType == Operator.Equality || operatorType == Operator.StrictEquality;
                bool staticString = plus && binaryExpression.Left is StringLiteral && (binaryExpression.Right is StringLiteral || binaryExpression.Right is NumericLiteral);

                var leftType = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion()).JsType;
                // If we're doing an equality comparison and the left is an arithmetic expression,
                // proactively unbox it to a double so subsequent branch compare works on numerics.
                bool leftIsArithmeticExpr = binaryExpression.Left is BinaryExpression lbe &&
                    (lbe.Operator == Operator.Addition || lbe.Operator == Operator.Subtraction ||
                     lbe.Operator == Operator.Multiplication || lbe.Operator == Operator.Division ||
                     lbe.Operator == Operator.Remainder || lbe.Operator == Operator.Exponentiation);
                if (equality && leftIsArithmeticExpr && leftType != JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.DoubleType);
                    leftType = JavascriptType.Number;
                }
                bool leftIsNumericSyntax = binaryExpression.Left is NumericLiteral || (binaryExpression.Left is UnaryExpression ulSyn && ulSyn.Operator.ToString() == "UnaryNegation" && ulSyn.Argument is NumericLiteral) || binaryExpression.Left is Identifier;
                bool rightIsNumericSyntax = binaryExpression.Right is NumericLiteral || (binaryExpression.Right is UnaryExpression urSyn && urSyn.Operator.ToString() == "UnaryNegation" && urSyn.Argument is NumericLiteral) || binaryExpression.Right is Identifier;
                bool likelyNumericSyntax = plus && leftIsNumericSyntax && rightIsNumericSyntax;
                if (equality)
                {
                    if (leftType == JavascriptType.Number)
                    {
                        // Only unbox when the left operand value is boxed (variables/expressions),
                        // not when it's a raw numeric literal or simple unary numeric.
                        bool leftIsRawNumeric = binaryExpression.Left is Acornima.Ast.NumericLiteral
                            || (binaryExpression.Left is Acornima.Ast.UnaryExpression ul && ul.Operator.ToString() == "UnaryNegation" && ul.Argument is Acornima.Ast.NumericLiteral);
                        // If left is a BinaryExpression (arithmetic), we've already ensured numeric; avoid double unbox
                        bool leftIsArithmeticNode = binaryExpression.Left is Acornima.Ast.BinaryExpression;
                        if (!leftIsRawNumeric && !leftIsArithmeticNode)
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
                        if (likelyNumericSyntax && leftType != JavascriptType.Number)
                        {
                            // For numeric-looking 'a + b', unbox left now to keep operand order
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.DoubleType);
                            leftType = JavascriptType.Number;
                        }
                        // otherwise, defer until after right emit
                    }
                    else if (minus)
                    {
                        // Defer decision until we know rightType; if both numeric, keep as doubles; otherwise box for runtime
                    }
                    else if (leftType != JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType);
                    }
                }

                var rightType = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion() { toString = binaryExpression.Left is StringLiteral }).JsType;
                // If equality compare and left resolved to number, make right numeric too when reasonable
                if (equality && leftType == JavascriptType.Number && rightType != JavascriptType.Number)
                {
                    bool rightIsNullLiteral = binaryExpression.Right is Acornima.Ast.Literal rl && rl.Value is null;
                    bool rightIsRawNumeric = binaryExpression.Right is Acornima.Ast.NumericLiteral
                        || (binaryExpression.Right is Acornima.Ast.UnaryExpression ur && ur.Operator.ToString() == "UnaryNegation" && ur.Argument is Acornima.Ast.NumericLiteral);
                    if (!rightIsRawNumeric && !rightIsNullLiteral)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType);
                        rightType = JavascriptType.Number;
                    }
                }
                bool identifiersPair = binaryExpression.Left is Identifier && binaryExpression.Right is Identifier;
                bool preferNumeric = plus && (leftType == JavascriptType.Number && rightType == JavascriptType.Number || identifiersPair || likelyNumericSyntax);
                // Special-case: comparing to JS null literal. Our emitter pushes a boxed JavaScriptRuntime.JsNull for null literals.
                // We want (left == nullLiteral) to be true when left is either CLR null (undefined in our model) OR boxed JsNull.
                if (equality && binaryExpression.Right is Acornima.Ast.Literal rNull && rNull.Value is null)
                {
                    // Stack currently: [left][right(Boxed JsNull)]
                    // Discard the literal and check left
                    _il.OpCode(ILOpCode.Pop); // pop right
                    if (branching == null)
                    {
                        var trueLbl = new LabelHandle();
                        var endLbl = new LabelHandle();
                        trueLbl = _il.DefineLabel();
                        endLbl = _il.DefineLabel();

                        // if (left == null) goto true
                        _il.OpCode(ILOpCode.Dup);
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.Branch(ILOpCode.Brtrue, trueLbl);

                        // else if (left is JsNull) goto true
                        var jsNullTypeRef = _runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.JsNull));
                        _il.OpCode(ILOpCode.Dup);
                        _il.OpCode(ILOpCode.Isinst);
                        _il.Token(jsNullTypeRef);
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.Branch(ILOpCode.Brtrue, trueLbl);

                        // else: false
                        _il.OpCode(ILOpCode.Pop); // pop left
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.BooleanType);
                        _il.Branch(ILOpCode.Br, endLbl);

                        // true path
                        _il.MarkLabel(trueLbl);
                        _il.OpCode(ILOpCode.Pop); // pop left
                        _il.OpCode(ILOpCode.Ldc_i4_1);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.BooleanType);

                        _il.MarkLabel(endLbl);
                    }
                    else
                    {
                        // Branching form: branch to true if left == null or left is JsNull; else branch to false or pop
                        var trueLbl = branching.BranchOnTrue;
                        var falseLbl = branching.BranchOnFalse;

                        // if (left == null) goto true
                        _il.OpCode(ILOpCode.Dup);
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.Branch(ILOpCode.Brtrue, trueLbl);

                        // else if (left is JsNull) goto true
                        var jsNullTypeRef = _runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.JsNull));
                        _il.OpCode(ILOpCode.Dup);
                        _il.OpCode(ILOpCode.Isinst);
                        _il.Token(jsNullTypeRef);
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.Branch(ILOpCode.Brtrue, trueLbl);

                        // else: not equal
                        _il.OpCode(ILOpCode.Pop); // pop left
                        if (falseLbl.HasValue)
                        {
                            _il.Branch(ILOpCode.Br, falseLbl.Value);
                        }
                        else
                        {
                            // No false target: discard and continue
                        }
                    }
                    return;
                }
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
                        // If comparing to null literal, ensure left is an object ref (avoid unbox) and then Ceq will compare ref to null.
                        if (binaryExpression.Right is Acornima.Ast.Literal litNull && litNull.Value is null)
                        {
                            // If left is currently unboxed number, box it back to object for a ref compare that will be false, which is JS-like for strict equality to null.
                            if (leftType == JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.DoubleType);
                            }
                            // If left is boolean, also box to keep object ref compare consistent
                            else if (leftType == JavascriptType.Boolean)
                            {
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(_bclReferences.BooleanType);
                            }
                            // No further coercion for null compare
                        }
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
                        if (likelyNumericSyntax)
                        {
                            // Ensure both sides are numeric (unbox when needed)
                            if (leftType != JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Unbox_any);
                                _il.Token(_bclReferences.DoubleType);
                                leftType = JavascriptType.Number;
                            }
                            if (rightType != JavascriptType.Number)
                            {
                                _il.OpCode(ILOpCode.Unbox_any);
                                _il.Token(_bclReferences.DoubleType);
                                rightType = JavascriptType.Number;
                            }
                        }
                        else if (!staticString)
                        {
                            // Ensure both operands are objects for runtime Add
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
                        if (leftType == JavascriptType.Number && rightType == JavascriptType.Number)
                        {
                            // numeric fast-path; leave as doubles
                        }
                        else
                        {
                            // ensure both operands are objects for runtime subtract
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
                    else if (rightType != JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.DoubleType);
                    }
                }

                // Emit '+' or '-' now based on analysis; others handled below
                if (plus)
                {
                    if (staticString)
                    {
                        // string.Concat(string, string)
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
                        var stringConcatMethodRef = _metadataBuilder.AddMemberReference(
                            _bclReferences.StringType,
                            _metadataBuilder.GetOrAddString("Concat"),
                            concatSig);
                        _il.Call(stringConcatMethodRef);
                    }
                    else if (preferNumeric)
                    {
                        _il.OpCode(ILOpCode.Add);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType);
                    }
                    else
                    {
                        _runtime.InvokeOperatorsAdd();
                    }
                    return;
                }
                if (minus)
                {
                    if (leftType == JavascriptType.Number && rightType == JavascriptType.Number)
                    {
                        _il.OpCode(ILOpCode.Sub);
                        _il.OpCode(ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType);
                    }
                    else
                    {
                        _runtime.InvokeOperatorsSubtract();
                    }
                    return;
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
                case Operator.StrictEquality:
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
                    case Operator.Subtraction:
                        // Route subtraction to runtime to honor JS ToNumber coercion (e.g., "a"-"b" => NaN)
                        _runtime.InvokeOperatorsSubtract();
                        break;
                    case Operator.Multiplication:
                    case Operator.Division:
                    case Operator.Remainder:
                        var opCode = op switch
                        {
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
            _il.Call(mathPowMethodRef);

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
                case Operator.StrictEquality:
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
