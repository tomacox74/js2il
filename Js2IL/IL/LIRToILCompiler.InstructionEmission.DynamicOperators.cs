using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

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
                EmitLoadTemp(inOp.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(inOp.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsIn(ilEncoder);
                EmitStoreTemp(inOp.Result, ilEncoder, allocation);
                return true;

            // 'instanceof' operator - calls Operators.InstanceOf
            case LIRInstanceOfOperator instOf:
                EmitLoadTemp(instOf.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(instOf.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsInstanceOf(ilEncoder);
                EmitStoreTemp(instOf.Result, ilEncoder, allocation);
                return true;

            case LIRBinaryDynamicOperator binaryDynamic:
                EmitLoadTemp(binaryDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(binaryDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsDynamicBinary(binaryDynamic.Operator, ilEncoder);
                EmitStoreTemp(binaryDynamic.Result, ilEncoder, allocation);
                return true;

            case LIRNegateNumberDynamic negateDynamic:
                EmitLoadTemp(negateDynamic.Value, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsUnaryMinus(ilEncoder);
                EmitStoreTemp(negateDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic equality - calls Operators.Equal
            case LIREqualDynamic equalDynamic:
                EmitLoadTemp(equalDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(equalDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsEqual(ilEncoder);
                EmitStoreTemp(equalDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic inequality - calls Operators.NotEqual
            case LIRNotEqualDynamic notEqualDynamic:
                EmitLoadTemp(notEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(notEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsNotEqual(ilEncoder);
                EmitStoreTemp(notEqualDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic strict equality - calls Operators.StrictEqual
            case LIRStrictEqualDynamic strictEqualDynamic:
                EmitLoadTemp(strictEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(strictEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictEqual(ilEncoder);
                EmitStoreTemp(strictEqualDynamic.Result, ilEncoder, allocation);
                return true;

            // Dynamic strict inequality - calls Operators.StrictNotEqual
            case LIRStrictNotEqualDynamic strictNotEqualDynamic:
                EmitLoadTemp(strictNotEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(strictNotEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictNotEqual(ilEncoder);
                EmitStoreTemp(strictNotEqualDynamic.Result, ilEncoder, allocation);
                return true;

            default:
                return null;
        }
    }
}
