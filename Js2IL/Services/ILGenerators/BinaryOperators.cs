﻿using Acornima;
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
        /// <param name="variable">The variable symbol to load.</param>
        /// <param name="typeCoercion">Required coercion hints for the load. Currently informational; load behavior is unchanged.</param>
        public void LoadVariable(Variable variable, TypeCoercion typeCoercion)
        {
            // Local helper to reduce duplicate unboxing code
            Action<JavascriptType> unboxIfNeeded = (jsType) =>
            {
                if (typeCoercion.boxResult) return;
                if (jsType == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.DoubleType);
                }
                else if (jsType == JavascriptType.Boolean)
                {
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.BooleanType);
                }
            };

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
                // Optional auto-unbox when caller does not want a boxed result
                unboxIfNeeded(sv.Type);
                return;
            }
            if (variable.IsParameter)
            {
                // Directly load argument (already object). ParameterIndex already accounts for scopes[] at arg0
                _il.LoadArgument(variable.ParameterIndex);
                // Optional auto-unbox when caller does not want a boxed result
                unboxIfNeeded(variable.Type);
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
            // Optional auto-unbox when caller does not want a boxed result
            unboxIfNeeded(variable.Type);
        }


        public ExpressionResult Generate(BinaryExpression binaryExpression, TypeCoercion typeCoercion, ConditionalBranching? branching = null)
        {
            if (binaryExpression.Left == null || binaryExpression.Right == null)
            {
                throw new ArgumentException("Binary expression must have both left and right operands.");
            }

            var operatorType = binaryExpression.Operator;

            // Handle short-circuit logical operators first
            if (operatorType == Operator.LogicalOr || operatorType == Operator.LogicalAnd)
            {
                return EmitLogicalOperator(binaryExpression, branching);
            }

            // Handle 'in' operator early to avoid generic arithmetic/comparison pipeline
            if (operatorType == Operator.In)
            {
                return EmitInOperator(binaryExpression, branching);
            }

            bool isBitwiseOrShift = operatorType == Operator.BitwiseAnd || operatorType == Operator.BitwiseOr || operatorType == Operator.BitwiseXor ||
                                    operatorType == Operator.LeftShift || operatorType == Operator.RightShift || operatorType == Operator.UnsignedRightShift;
            if (isBitwiseOrShift)
            {
                EmitBitwiseOrShiftOperands(binaryExpression);
            }
            else
            {
                // Delegate non-bitwise handling to focused helpers for clarity
                switch (operatorType)
                {
                    case Operator.Addition:
                        {
                            var early = EmitAdditionOperandsAndShortCircuits(binaryExpression, branching);
                            if (early != null) return early;
                        }
                        break;
                    case Operator.Subtraction:
                        {
                            var early = EmitSubtractionOperandsAndShortCircuits(binaryExpression, branching);
                            if (early != null) return early;
                        }
                        break;
                    case Operator.Equality:
                    case Operator.StrictEquality:
                        {
                            var early = EmitEqualityOperandsAndShortCircuits(binaryExpression, branching);
                            if (early != null) return early;
                        }
                        // not fully handled: operands prepared → fall through to ApplyComparisonOperator
                        break;
                    case Operator.Inequality:
                    case Operator.StrictInequality:
                        // For now, prepare operands generically (numeric) to unblock inequality for numeric scenarios
                        EmitGenericOperands(binaryExpression);
                        break;
                    default:
                        // Relational operators and other arithmetic (mul/div/rem/exp) operand prep
                        EmitGenericOperands(binaryExpression);
                        break;
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
            // Arithmetic and bitwise operators yield a numeric value in JS (Number in our model)
            // '+' is handled above (may be string or number) and returned early.
            return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null };
                case Operator.LessThan:
                case Operator.GreaterThan:
                case Operator.LessThanOrEqual:
                case Operator.GreaterThanOrEqual:
                case Operator.Equality:
                case Operator.StrictEquality:
                case Operator.Inequality:
                case Operator.StrictInequality:
                    ApplyComparisonOperator(operatorType, binaryExpression, branching);
                    // For comparisons, value form leaves a boxed boolean; branching leaves stack neutral.
                    return branching == null
                        ? new ExpressionResult { JsType = JavascriptType.Object, ClrType = null }
                        : new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
                default:
                    ILEmitHelpers.ThrowNotSupported($"Unsupported binary operator: {operatorType}", binaryExpression);
                    return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null }; // unreachable
            }
        }

        /// <summary>
        /// Emits IL for short-circuit logical operators (|| and &&), supporting both value and branching forms.
        /// </summary>
        private ExpressionResult EmitLogicalOperator(BinaryExpression binaryExpression, ConditionalBranching? branching)
        {
            var operatorType = binaryExpression.Operator;

            // Branching form: we can short-circuit without materializing a value
            if (branching != null)
            {
                // Emit left truthiness check
                // Load left (boxed) and call ToBoolean(object)
                // Ensure the left operand is boxed as object so ToBoolean(object) can consume it
                _ = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion() { boxResult = true });
                var toBoolRef = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                _il.OpCode(ILOpCode.Call);
                _il.Token(toBoolRef);

                if (operatorType == Operator.LogicalOr)
                {
                    // If left is truthy, branch to true
                    _il.Branch(ILOpCode.Brtrue, branching.BranchOnTrue);
                    // Else evaluate right with the same branching
                    _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion(), CallSiteContext.Expression, branching);
                }
                else // LogicalAnd
                {
                    // If left is falsy, branch to false (if available) or fall-through
                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(ILOpCode.Brfalse, branching.BranchOnFalse.Value);
                    }
                    else
                    {
                        // No explicit false target; create a local end label to skip right when false
                        var skipRight = _il.DefineLabel();
                        _il.Branch(ILOpCode.Brfalse, skipRight);
                        _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion(), CallSiteContext.Expression, branching);
                        _il.MarkLabel(skipRight);
                        return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
                    }
                    // Left is true: evaluate right with same branching
                    _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion(), CallSiteContext.Expression, branching);
                }
                return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
            }

            // Value form: result is one of the operands (not coerced to boolean)
            // We'll box operands as object and choose with short-circuiting.
            var useLeftLabel = _il.DefineLabel();
            var endLabel = _il.DefineLabel();

            // Push left (boxed object)
            _ = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion() { boxResult = true });

            // Duplicate left, test truthiness on the copy
            _il.OpCode(ILOpCode.Dup);
            var toBool = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
            _il.OpCode(ILOpCode.Call);
            _il.Token(toBool);

            if (operatorType == Operator.LogicalOr)
            {
                // If truthy, jump to useLeft (brtrue pops the bool, leaving left on stack)
                _il.Branch(ILOpCode.Brtrue, useLeftLabel);
            }
            else // LogicalAnd
            {
                // If falsy, jump to useLeft (brfalse pops the bool, leaving left on stack)
                _il.Branch(ILOpCode.Brfalse, useLeftLabel);
            }

            // Else: branch not taken; condition has been popped by the branch instruction. Pop left and evaluate right
            _il.OpCode(ILOpCode.Pop); // pop left

            // Push right (boxed object) and finish
            _ = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion() { boxResult = true });
            _il.Branch(ILOpCode.Br, endLabel);

            // Use left as result
            _il.MarkLabel(useLeftLabel);
            // Stack already has the chosen left object (bool was consumed by branch)

            _il.MarkLabel(endLabel);
            return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
        }

        /// <summary>
        /// Emits IL for the JavaScript 'in' operator.
        /// </summary>
        private ExpressionResult EmitInOperator(BinaryExpression binaryExpression, ConditionalBranching? branching)
        {
            // Evaluate left (property key) boxed
            _ = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion() { boxResult = true });
            // Evaluate right (object) boxed
            _ = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion() { boxResult = true });
            var hasPropRef = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.HasPropertyIn), typeof(bool), typeof(object), typeof(object));
            _il.OpCode(ILOpCode.Call);
            _il.Token(hasPropRef);
            if (branching == null)
            {
                _il.OpCode(ILOpCode.Box);
                _il.Token(_bclReferences.BooleanType);
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
            }
            else
            {
                _il.Branch(ILOpCode.Brtrue, branching.BranchOnTrue);
                if (branching.BranchOnFalse.HasValue)
                {
                    _il.Branch(ILOpCode.Br, branching.BranchOnFalse.Value);
                }
                return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
            }
        }

        /// <summary>
        /// Prepares operands for bitwise and shift operators by coercing both sides to int32.
        /// </summary>
        private void EmitBitwiseOrShiftOperands(BinaryExpression binaryExpression)
        {
            // Fast/slow path per JS semantics: ToInt32(ToNumber(x)) for bitwise/shift.
            // Use a tiny helper to avoid duplicating the coercion logic for each side.
            Action<Acornima.Ast.Expression> emitInt32 = expr =>
            {
                var jsType = _methodExpressionEmitter.Emit(expr, new TypeCoercion()).JsType;
                if (jsType == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Conv_i4);
                }
                else if (jsType == JavascriptType.Boolean)
                {
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.BooleanType);
                    _il.OpCode(ILOpCode.Conv_i4);
                }
                else
                {
                    // Slow path: call TypeUtilities.ToNumber(object) then convert to int32
                    var toNum = _runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        typeof(double), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toNum);
                    _il.OpCode(ILOpCode.Conv_i4);
                }
            };

            emitInt32(binaryExpression.Left);
            emitInt32(binaryExpression.Right);
        }

        /// <summary>
        /// Emits operands and short-circuits for non-bitwise operators.
        /// Returns true if the operator was fully handled (e.g., '+', '-', special equality cases), false to continue.
        /// </summary>
        private ExpressionResult? EmitNonBitwiseOperandsAndShortCircuits(BinaryExpression binaryExpression, Operator operatorType, ConditionalBranching? branching)
        {
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
                    else if (!likelyNumericSyntax)
                    {
                        // Not a numeric fast-path candidate: pre-box the left primitive (if any)
                        // while it is on top of the stack, so we won't accidentally box the right later.
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
                    var userTrueLbl = branching.BranchOnTrue;
                    var falseLbl = branching.BranchOnFalse;
                    // Ensure we don't leave the duplicated 'left' on the stack when branching to true
                    var cleanupTrue = _il.DefineLabel();

                    // if (left == null) goto cleanupTrue
                    _il.OpCode(ILOpCode.Dup);
                    _il.OpCode(ILOpCode.Ldnull);
                    _il.OpCode(ILOpCode.Ceq);
                    _il.Branch(ILOpCode.Brtrue, cleanupTrue);

                    // else if (left is JsNull) goto cleanupTrue
                    var jsNullTypeRef = _runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.JsNull));
                    _il.OpCode(ILOpCode.Dup);
                    _il.OpCode(ILOpCode.Isinst);
                    _il.Token(jsNullTypeRef);
                    _il.OpCode(ILOpCode.Ldnull);
                    _il.OpCode(ILOpCode.Ceq);
                    _il.OpCode(ILOpCode.Ldc_i4_0);
                    _il.OpCode(ILOpCode.Ceq);
                    _il.Branch(ILOpCode.Brtrue, cleanupTrue);

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

                    // Cleanup for true branch: pop 'left' then branch to user label
                    _il.MarkLabel(cleanupTrue);
                    _il.OpCode(ILOpCode.Pop);
                    _il.Branch(ILOpCode.Br, userTrueLbl);
                }
                return branching == null
                    ? new ExpressionResult { JsType = JavascriptType.Object, ClrType = null }
                    : new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
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
                    else if (!staticString && !(leftType == JavascriptType.Number && rightType == JavascriptType.Number))
                    {
                        // Only box when we are not in a numeric fast-path.
                        // Left operand (if primitive) was pre-boxed above when not likelyNumericSyntax.
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

            // Emit '+' or '-' now based on analysis; others handled by the dispatcher
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
                    // Numeric fast-path: leave unboxed double on stack
                    _il.OpCode(ILOpCode.Add);
                    return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null };
                }
                else
                {
                    _runtime.InvokeOperatorsAdd();
                }
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
            }
            if (minus)
            {
                if (leftType == JavascriptType.Number && rightType == JavascriptType.Number)
                {
                    // Numeric fast-path: leave unboxed double on stack
                    _il.OpCode(ILOpCode.Sub);
                    return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null };
                }
                else
                {
                    // Runtime path returns object
                    _runtime.InvokeOperatorsSubtract();
                    return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
                }
            }

            // Not fully handled; let the caller dispatch to arithmetic/comparison emitters.
            return null;
        }

        // Focused wrappers for readability: delegate to the comprehensive helper above
        private ExpressionResult? EmitAdditionOperandsAndShortCircuits(BinaryExpression binaryExpression, ConditionalBranching? branching)
            => EmitNonBitwiseOperandsAndShortCircuits(binaryExpression, Operator.Addition, branching);

        private ExpressionResult? EmitSubtractionOperandsAndShortCircuits(BinaryExpression binaryExpression, ConditionalBranching? branching)
            => EmitNonBitwiseOperandsAndShortCircuits(binaryExpression, Operator.Subtraction, branching);

        private ExpressionResult? EmitEqualityOperandsAndShortCircuits(BinaryExpression binaryExpression, ConditionalBranching? branching)
            => EmitNonBitwiseOperandsAndShortCircuits(binaryExpression, Operator.Equality, branching);

        

        /// <summary>
        /// Prepares operands for binary expressions that are not handled by addition, subtraction, or equality logic.
        /// Extracts the common numeric operand preparation for relational and other arithmetic operators.
        /// </summary>
        private void EmitGenericOperands(BinaryExpression binaryExpression)
        {
            // Prepare left operand
            var leftType = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion()).JsType;
            if (leftType != JavascriptType.Number)
            {
                _il.OpCode(ILOpCode.Unbox_any);
                _il.Token(_bclReferences.DoubleType);
            }

            // Prepare right operand
            var rightType = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion()).JsType;
            if (rightType != JavascriptType.Number)
            {
                _il.OpCode(ILOpCode.Unbox_any);
                _il.Token(_bclReferences.DoubleType);
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
                            _ => throw ILEmitHelpers.NotSupported($"Unsupported arithmetic operator: {op}", binaryExpression)
                        };
                        // Leave unboxed double on stack; callers that need an object will box explicitly
                        _il.OpCode(opCode);
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
                            _ => throw ILEmitHelpers.NotSupported($"Unsupported bitwise operator: {op}", binaryExpression)
                        };
                        _il.OpCode(bitwiseOpCode);
                        if (op == Operator.UnsignedRightShift) {
                            // Convert result to uint32 then to double for JS >>>
                            _il.OpCode(ILOpCode.Conv_u4);
                        }
                        // Normalize to double but leave unboxed
                        _il.OpCode(ILOpCode.Conv_r8);
                        break;
                    case Operator.Exponentiation:
                        // Exponentiation requires calling Math.Pow()
                        ApplyExponentiationOperator();
                        break;
                    default:
                        ILEmitHelpers.ThrowNotSupported($"Unsupported arithmetic operator: {op}", binaryExpression);
                        return; // unreachable
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
            // Leave unboxed double on stack; callers can box if needed
        }

    private void ApplyComparisonOperator(Operator op, BinaryExpression binaryExpression, ConditionalBranching? branching = null)
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
                case Operator.Inequality:
                case Operator.StrictInequality:
                    compareOpCode = ILOpCode.Ceq; // we'll invert the result for value form
                    branchOpCode = ILOpCode.Bne_un;
                    break;
                default:
                    ILEmitHelpers.ThrowNotSupported($"Unsupported comparison operator: {op}", binaryExpression);
                    return; // unreachable
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
                    if (op == Operator.Inequality || op == Operator.StrictInequality)
                    {
                        // invert equality result: result = (a == b) == false
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Ceq);
                    }
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
                    // No false target provided; fall through. No stack cleanup required.
                }
            }
        }
    }
}
