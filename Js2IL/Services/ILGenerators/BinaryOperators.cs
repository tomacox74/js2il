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
        private ILMethodGenerator _methodGenerator;

        public BinaryOperators(MetadataBuilder metadataBuilder, InstructionEncoder il, Variables variables, IMethodExpressionEmitter methodExpressionEmitter, BaseClassLibraryReferences bclReferences, Runtime runtime, ILMethodGenerator methodGenerator)
        {
            _metadataBuilder = metadataBuilder;
            _il = il;
            _variables = variables;
            _bclReferences = bclReferences;
            _runtime = runtime;
            _methodExpressionEmitter = methodExpressionEmitter;
            _methodGenerator = methodGenerator;
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
            return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null, IsBoxed = false };
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
                        ? new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true }
                        : new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null, IsBoxed = false };
                default:
                    ILEmitHelpers.ThrowNotSupported($"Unsupported binary operator: {operatorType}", binaryExpression);
                    return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null, IsBoxed = false }; // unreachable
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
                return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null, IsBoxed = false };
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
            return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true };
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
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true };
            }
            else
            {
                _il.Branch(ILOpCode.Brtrue, branching.BranchOnTrue);
                if (branching.BranchOnFalse.HasValue)
                {
                    _il.Branch(ILOpCode.Br, branching.BranchOnFalse.Value);
                }
                return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null, IsBoxed = false };
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

            var leftResult = _methodExpressionEmitter.Emit(binaryExpression.Left, new TypeCoercion());
            var leftType = leftResult.JsType;
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
                if (leftType == JavascriptType.Number && leftResult.IsBoxed)
                {
                    // Unbox when the left operand value is boxed
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.DoubleType);
                }
                else if (leftType == JavascriptType.Boolean && leftResult.IsBoxed)
                {
                    // Unbox when the left operand value is boxed
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.BooleanType);
                }
                else if ((leftType == JavascriptType.Object || leftType == JavascriptType.Unknown) && (binaryExpression.Right is Acornima.Ast.BooleanLiteral || (binaryExpression.Right is Acornima.Ast.Literal brl && brl.Value is bool)))
                {
                    // If right is a boolean literal and left is object/unknown (e.g., function return, captured var), convert left to boolean
                    var toBool = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toBool);
                    leftType = JavascriptType.Boolean;
                }
                else if ((leftType == JavascriptType.Object || leftType == JavascriptType.Unknown) && (binaryExpression.Right is Acornima.Ast.NumericLiteral || (binaryExpression.Right is Acornima.Ast.UnaryExpression rur && rur.Operator.ToString() == "UnaryNegation" && rur.Argument is Acornima.Ast.NumericLiteral)))
                {
                    // If right is a numeric literal and left is object/unknown (e.g., parameter, captured var), convert left to number
                    var toNum = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToNumber), typeof(double), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toNum);
                    leftType = JavascriptType.Number;
                }
                else if ((leftType == JavascriptType.Object || leftType == JavascriptType.Unknown) && binaryExpression.Right is Acornima.Ast.Identifier)
                {
                    // If right is an identifier (which will load a boxed value) and left is object/unknown, convert left to number
                    // This handles cases like: methodResult == variableName
                    var toNum = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToNumber), typeof(double), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toNum);
                    leftType = JavascriptType.Number;
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
                    // Note: do NOT pre-box the left operand here when not likelyNumericSyntax.
                    // Boxing too early can leave a boxed left-hand value on the stack and later
                    // choose the numeric fast-path after evaluating the right operand, causing
                    // IL 'add' to operate on mismatched types. Defer boxing to the non-numeric
                    // path after both operands are analyzed.
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

            var rightResult = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion() { toString = binaryExpression.Left is StringLiteral });
            var rightType = rightResult.JsType;
            // If equality compare and left resolved to number, make right numeric too when reasonable
            if (equality && leftType == JavascriptType.Number && rightType != JavascriptType.Number)
            {
                bool rightIsNullLiteral = binaryExpression.Right is Acornima.Ast.Literal rl && rl.Value is null;
                bool rightIsRawNumeric =
                    // Numeric literal or -numeric literal
                    (binaryExpression.Right is Acornima.Ast.NumericLiteral
                        || (binaryExpression.Right is Acornima.Ast.UnaryExpression ur && ur.Operator.ToString() == "UnaryNegation" && ur.Argument is Acornima.Ast.NumericLiteral))
                    // Common numeric member: *.length yields an unboxed double
                    || (binaryExpression.Right is Acornima.Ast.MemberExpression rme && !rme.Computed && rme.Property is Acornima.Ast.Identifier rid && string.Equals(rid.Name, "length", StringComparison.Ordinal));
                // NOTE: Removed Identifier check - Identifiers load boxed values from fields that need type conversion
                if (!rightIsRawNumeric && !rightIsNullLiteral)
                {
                    // Generic numeric coercion when operand isn't already a raw double
                    var toNum = _runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        typeof(double), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toNum);
                    rightType = JavascriptType.Number;
                }
            }
            // For equality comparisons, unbox right operand if needed
            if (equality && rightType == JavascriptType.Number && rightResult.IsBoxed)
            {
                // Unbox when the right operand value is boxed
                _il.OpCode(ILOpCode.Unbox_any);
                _il.Token(_bclReferences.DoubleType);
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
                    ? new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true }
                    : new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null, IsBoxed = false };
            }
            if (equality)
            {
                if (rightType == JavascriptType.Number && rightResult.IsBoxed)
                {
                    // Unbox when the right operand value is boxed
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.DoubleType);
                }
                else if (rightType == JavascriptType.Boolean && rightResult.IsBoxed)
                {
                    // Unbox when the right operand value is boxed
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_bclReferences.BooleanType);
                }
                else if (rightType == JavascriptType.Object && (binaryExpression.Left is Acornima.Ast.BooleanLiteral || (binaryExpression.Left is Acornima.Ast.Literal lbl && lbl.Value is bool)))
                {
                    // If left is a boolean literal and right is object (e.g., function return), convert right to boolean
                    var toBool = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toBool);
                    rightType = JavascriptType.Boolean;
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
                    // When comparing two non-literal, non-numeric, non-boolean values (likely boxed objects),
                    // convert both to numbers to ensure value comparison instead of reference comparison.
                    // This handles cases where type tracking returns Unknown or Object.
                    else if (leftType != JavascriptType.Number && leftType != JavascriptType.Boolean &&
                             rightType != JavascriptType.Number && rightType != JavascriptType.Boolean &&
                             !(binaryExpression.Left is Acornima.Ast.Literal) && 
                             !(binaryExpression.Right is Acornima.Ast.Literal))
                    {
                        // Store right operand in temp since we need to convert both
                        int rhsTemp = _variables.AllocateBlockScopeLocal($"EqTmp_RHS_L{binaryExpression.Location.Start.Line}C{binaryExpression.Location.Start.Column}");
                        _il.StoreLocal(rhsTemp);
                        
                        // Convert left operand to number
                        var toNum = _runtime.GetStaticMethodRef(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                            typeof(double), typeof(object));
                        _il.OpCode(ILOpCode.Call);
                        _il.Token(toNum);
                        
                        // Load and convert right operand to number
                        _il.LoadLocal(rhsTemp);
                        _il.OpCode(ILOpCode.Call);
                        _il.Token(toNum);
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
                            // Coerce to number via runtime to avoid invalid casts (e.g., arrays)
                            var toNum = _runtime.GetStaticMethodRef(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                typeof(double), typeof(object));
                            _il.OpCode(ILOpCode.Call);
                            _il.Token(toNum);
                        }
                        if (rightType != JavascriptType.Number)
                        {
                            var toNum = _runtime.GetStaticMethodRef(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                typeof(double), typeof(object));
                            _il.OpCode(ILOpCode.Call);
                            _il.Token(toNum);
                        }
                    }
                    else if (!staticString && !(leftType == JavascriptType.Number && rightType == JavascriptType.Number))
                    {
                        // We are going to call runtime Operators.Add(object, object).
                        // Ensure BOTH operands are boxed objects. Since the right operand is on top of the stack,
                        // if the left needs boxing we must first stash the right into a temp local, box left, then reload right.
                        bool leftNeedsBox = leftType == JavascriptType.Number || leftType == JavascriptType.Boolean;
                        bool rightNeedsBox = rightType == JavascriptType.Number || rightType == JavascriptType.Boolean;

                        if (leftNeedsBox)
                        {
                            // If right also needs boxing, box it before storing into object local
                            if (rightNeedsBox)
                            {
                                var rt = rightType == JavascriptType.Number ? _bclReferences.DoubleType : _bclReferences.BooleanType;
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(rt);
                            }
                            int rhsTemp = _variables.AllocateBlockScopeLocal($"PlusTmp_RHS_L{binaryExpression.Location.Start.Line}C{binaryExpression.Location.Start.Column}");
                            _il.StoreLocal(rhsTemp);

                            // Box left primitive now (it is on top after storing RHS)
                            var lt = leftType == JavascriptType.Number ? _bclReferences.DoubleType : _bclReferences.BooleanType;
                            _il.OpCode(ILOpCode.Box);
                            _il.Token(lt);

                            // Reload RHS (already boxed if it needed to be)
                            _il.LoadLocal(rhsTemp);
                        }
                        else
                        {
                            // Left is already an object; only ensure right is boxed if needed
                            if (rightNeedsBox)
                            {
                                var rt = rightType == JavascriptType.Number ? _bclReferences.DoubleType : _bclReferences.BooleanType;
                                _il.OpCode(ILOpCode.Box);
                                _il.Token(rt);
                            }
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
                    var toNum = _runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        typeof(double), typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(toNum);
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
                    return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null, IsBoxed = false };
                }
                else
                {
                    _runtime.InvokeOperatorsAdd();
                }
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true };
            }
            if (minus)
            {
                if (leftType == JavascriptType.Number && rightType == JavascriptType.Number)
                {
                    // Numeric fast-path: leave unboxed double on stack
                    _il.OpCode(ILOpCode.Sub);
                    return new ExpressionResult { JsType = JavascriptType.Number, ClrType = null, IsBoxed = false };
                }
                else
                {
                    // Runtime path returns object
                    _runtime.InvokeOperatorsSubtract();
                    return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null, IsBoxed = true };
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
                // Use JS ToNumber to safely coerce any object (arrays, strings, etc.) to a number
                var toNum = _runtime.GetStaticMethodRef(
                    typeof(JavaScriptRuntime.TypeUtilities),
                    nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                    typeof(double), typeof(object));
                _il.OpCode(ILOpCode.Call);
                _il.Token(toNum);
            }

            // Prepare right operand
            var rightType = _methodExpressionEmitter.Emit(binaryExpression.Right, new TypeCoercion()).JsType;
            if (rightType != JavascriptType.Number)
            {
                var toNum = _runtime.GetStaticMethodRef(
                    typeof(JavaScriptRuntime.TypeUtilities),
                    nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                    typeof(double), typeof(object));
                _il.OpCode(ILOpCode.Call);
                _il.Token(toNum);
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
