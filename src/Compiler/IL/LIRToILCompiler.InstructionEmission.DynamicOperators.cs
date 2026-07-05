using Jroc.IR;
using Jroc.Services.ILGenerators;
using Jroc.Services.TwoPhaseCompilation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_DynamicOperators(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            // 'in' operator - calls Operators.In
            case LIRInOperator inOp:
                EmitLoadTempAsObject(inOp.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(inOp.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsIn(ilEncoder);
                EmitStoreTemp(inOp.Result, ilEncoder, allocation);
                return true;

            // 'instanceof' operator - calls Operators.InstanceOf
            case LIRInstanceOfOperator instOf:
                EmitLoadTempAsObject(instOf.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(instOf.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsInstanceOf(ilEncoder);
                EmitStoreTemp(instOf.Result, ilEncoder, allocation);
                return true;

            case LIRBinaryDynamicOperator binaryDynamic:
                EmitLoadTempAsObject(binaryDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(binaryDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsDynamicBinary(binaryDynamic.Operator, ilEncoder);
                EmitStoreTemp(binaryDynamic.Result, ilEncoder, allocation);
                return true;

            case LIRNegateNumberDynamic negateDynamic:
                EmitLoadTempAsObject(negateDynamic.Value, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsUnaryMinus(ilEncoder);
                EmitStoreTemp(negateDynamic.Result, ilEncoder, allocation);
                return true;

            case LIRBitwiseNotDynamic bitwiseNotDynamic:
                EmitLoadTempAsObject(bitwiseNotDynamic.Value, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsBitwiseNot(ilEncoder);
                EmitStoreTemp(bitwiseNotDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic equality - calls Operators.Equal
            case LIREqualDynamic equalDynamic:
                EmitLoadTempAsObject(equalDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(equalDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsEqual(ilEncoder);
                EmitStoreTemp(equalDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic inequality - calls Operators.NotEqual
            case LIRNotEqualDynamic notEqualDynamic:
                EmitLoadTempAsObject(notEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(notEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsNotEqual(ilEncoder);
                EmitStoreTemp(notEqualDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic strict equality - calls Operators.StrictEqual
            case LIRStrictEqualDynamic strictEqualDynamic:
                EmitLoadTempAsObject(strictEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(strictEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictEqual(ilEncoder);
                EmitStoreTemp(strictEqualDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic strict inequality - calls Operators.StrictNotEqual
            case LIRStrictNotEqualDynamic strictNotEqualDynamic:
                EmitLoadTempAsObject(strictNotEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(strictNotEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictNotEqual(ilEncoder);
                EmitStoreTemp(strictNotEqualDynamic.Result, ilEncoder, allocation);
                return true;

            default:
                return null;
        }
    }
}
