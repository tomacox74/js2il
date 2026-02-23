namespace Js2IL.IR;

// ============================================================================
// Arithmetic Binary Operators
// ============================================================================

/// <summary>
/// Addition of two double values using native IL add instruction.
/// </summary>
public record LIRAddNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// String concatenation using String.Concat. Used when both operands are known to be strings.
/// </summary>
public record LIRConcatStrings(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic addition using Operators.Add runtime helper. Used when operand types are unknown.
/// </summary>
public record LIRAddDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic addition using Operators.Add(double, object) to avoid boxing the left double operand.
/// </summary>
public record LIRAddDynamicDoubleObject(TempVariable LeftDouble, TempVariable RightObject, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic addition using Operators.Add(object, double) to avoid boxing the right double operand.
/// </summary>
public record LIRAddDynamicObjectDouble(TempVariable LeftObject, TempVariable RightDouble, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic addition followed by immediate ToNumber coercion via Operators.AddAndToNumber.
/// </summary>
public record LIRAddAndToNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Subtraction of two double values using native IL sub instruction.
/// </summary>
public record LIRSubNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Multiplication of two double values using native IL mul instruction.
/// </summary>
public record LIRMulNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic multiplication using Operators.Multiply runtime helper. Used when operand types are unknown.
/// </summary>
public record LIRMulDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Division of two double values using native IL div instruction.
/// </summary>
public record LIRDivNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Remainder (modulo) of two double values using native IL rem instruction.
/// </summary>
public record LIRModNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Exponentiation using Math.Pow runtime call. JavaScript ** operator.
/// </summary>
public record LIRExpNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// ============================================================================
// Bitwise Binary Operators
// ============================================================================

/// <summary>
/// Bitwise AND of two numbers. Converts to int32, performs AND, converts back to double.
/// </summary>
public record LIRBitwiseAnd(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Bitwise OR of two numbers. Converts to int32, performs OR, converts back to double.
/// </summary>
public record LIRBitwiseOr(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Bitwise XOR of two numbers. Converts to int32, performs XOR, converts back to double.
/// </summary>
public record LIRBitwiseXor(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// ============================================================================
// Shift Operators
// ============================================================================

/// <summary>
/// Left shift of two numbers. Converts to int32, performs shift, converts back to double.
/// </summary>
public record LIRLeftShift(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Signed right shift of two numbers. Converts to int32, performs shift, converts back to double.
/// </summary>
public record LIRRightShift(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Unsigned right shift of two numbers. Converts to int32, performs unsigned shift, converts back to double.
/// </summary>
public record LIRUnsignedRightShift(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

public enum DynamicBinaryOperatorKind
{
    Subtract,
    Divide,
    Remainder,
    Exponentiate,
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor,
    LeftShift,
    SignedRightShift,
    UnsignedRightShift,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
}

/// <summary>
/// Dynamic binary operation using JavaScriptRuntime.Operators with boxed operands.
/// Used when operand types are not known to be unboxed doubles.
/// </summary>
public record LIRBinaryDynamicOperator(
    DynamicBinaryOperatorKind Operator,
    TempVariable Left,
    TempVariable Right,
    TempVariable Result) : LIRInstruction;

// ============================================================================
// Comparison Operators
// ============================================================================

// Comparison operators for numeric values (result is bool)
public record LIRCompareNumberLessThan(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberGreaterThan(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberLessThanOrEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberGreaterThanOrEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberNotEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// Comparison operators for boolean values (result is bool)
public record LIRCompareBooleanEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareBooleanNotEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// ============================================================================
// Dynamic Equality Operators (runtime type checking)
// ============================================================================

/// <summary>
/// Dynamic equality comparison using Operators.Equal runtime helper.
/// Used when operand types are unknown at compile time.
/// </summary>
public record LIREqualDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic inequality comparison using Operators.NotEqual runtime helper.
/// Used when operand types are unknown at compile time.
/// </summary>
public record LIRNotEqualDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic strict equality comparison using Operators.StrictEqual runtime helper.
/// Used when operand types are unknown at compile time.
/// </summary>
public record LIRStrictEqualDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic strict inequality comparison using Operators.StrictNotEqual runtime helper.
/// Used when operand types are unknown at compile time.
/// </summary>
public record LIRStrictNotEqualDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// ============================================================================
// Logical Operator Helpers
// ============================================================================

/// <summary>
/// Calls Operators.IsTruthy to check if a value is truthy according to JavaScript semantics.
/// Result is a boolean (true/false).
/// </summary>
public record LIRCallIsTruthy(TempVariable Value, TempVariable Result) : LIRInstruction;

/// <summary>
/// Copies a temp variable value to another temp variable.
/// Used for short-circuit evaluation where we need to assign the same value to result from different branches.
/// </summary>
public record LIRCopyTemp(TempVariable Source, TempVariable Destination) : LIRInstruction;

// ============================================================================
// Special Operators
// ============================================================================

/// <summary>
/// JavaScript 'in' operator. Checks if property exists in object.
/// Calls Operators.In runtime helper.
/// </summary>
public record LIRInOperator(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// JavaScript 'instanceof' operator. Checks if an object's prototype chain includes ctor.prototype.
/// Calls Operators.InstanceOf runtime helper.
/// </summary>
public record LIRInstanceOfOperator(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
