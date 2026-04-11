using Js2IL.IR;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

public sealed class LIRTypeNormalizationTests
{
    private static TempVariable AddTemp(MethodBodyIR body, ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }

    [Fact]
    public void Normalize_Rewrites_CallIsTruthy_ToCallIsTruthyDouble_WhenValueIsUnboxedDouble()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        var rewritten = Assert.IsType<LIRCallIsTruthyDouble>(body.Instructions[0]);
        Assert.Equal(value, rewritten.Value);
        Assert.Equal(result, rewritten.Result);
    }

    [Fact]
    public void Normalize_Rewrites_CallIsTruthy_ToCallIsTruthyBool_WhenValueIsUnboxedBool()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        var rewritten = Assert.IsType<LIRCallIsTruthyBool>(body.Instructions[0]);
        Assert.Equal(value, rewritten.Value);
        Assert.Equal(result, rewritten.Result);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_CallIsTruthy_WhenValueIsObjectTyped()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        // Should remain as LIRCallIsTruthy (object overload).
        Assert.IsType<LIRCallIsTruthy>(body.Instructions[0]);
    }

    [Fact]
    public void Normalize_SpecializesMultiple_CallIsTruthy_InSameMethod()
    {
        var body = new MethodBodyIR();
        var doubleVal = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var boolVal = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var objVal = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var res0 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var res1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var res2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(doubleVal, res0));
        body.Instructions.Add(new LIRCallIsTruthy(boolVal, res1));
        body.Instructions.Add(new LIRCallIsTruthy(objVal, res2));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.IsType<LIRCallIsTruthyDouble>(body.Instructions[0]);
        Assert.IsType<LIRCallIsTruthyBool>(body.Instructions[1]);
        Assert.IsType<LIRCallIsTruthy>(body.Instructions[2]);
    }

    [Fact]
    public void Normalize_Rewrites_TypeofFunctionStrictEqual_BranchToIsInstanceOf()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var typeofResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var functionString = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var comparisonResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        int targetLabel = 7;

        body.Instructions.Add(new LIRTypeof(value, typeofResult));
        body.Instructions.Add(new LIRConstString("function", functionString));
        body.Instructions.Add(new LIRStrictEqualDynamic(typeofResult, functionString, comparisonResult));
        body.Instructions.Add(new LIRBranchIfFalse(comparisonResult, targetLabel));
        body.Instructions.Add(new LIRLabel(targetLabel));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.Collection(
            body.Instructions,
            instruction =>
            {
                var isInstanceOf = Assert.IsType<LIRIsInstanceOf>(instruction);
                Assert.Equal(typeof(Delegate), isInstanceOf.TargetType);
                Assert.Equal(value, isInstanceOf.Value);
            },
            instruction =>
            {
                var branch = Assert.IsType<LIRBranchIfFalse>(instruction);
                Assert.Equal(4, branch.Condition.Index);
                Assert.Equal(targetLabel, branch.TargetLabel);
            },
            instruction => Assert.IsType<LIRLabel>(instruction));
    }

    [Fact]
    public void Normalize_Rewrites_TypeofFunctionStrictEqual_WhenTypeofAndConstantAreCopiedBeforeCompare()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var typeofResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var typeofCopy = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var functionString = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var functionStringCopy = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var comparisonResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        int targetLabel = 8;

        body.Instructions.Add(new LIRTypeof(value, typeofResult));
        body.Instructions.Add(new LIRCopyTemp(typeofResult, typeofCopy));
        body.Instructions.Add(new LIRConstString("function", functionString));
        body.Instructions.Add(new LIRCopyTemp(functionString, functionStringCopy));
        body.Instructions.Add(new LIRStrictEqualDynamic(typeofCopy, functionStringCopy, comparisonResult));
        body.Instructions.Add(new LIRBranchIfTrue(comparisonResult, targetLabel));
        body.Instructions.Add(new LIRLabel(targetLabel));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.Collection(
            body.Instructions,
            instruction =>
            {
                var isInstanceOf = Assert.IsType<LIRIsInstanceOf>(instruction);
                Assert.Equal(typeof(Delegate), isInstanceOf.TargetType);
                Assert.Equal(value, isInstanceOf.Value);
            },
            instruction =>
            {
                var branch = Assert.IsType<LIRBranchIfTrue>(instruction);
                var conditionDef = Assert.IsType<LIRIsInstanceOf>(body.Instructions[0]);
                Assert.Equal(conditionDef.Result, branch.Condition);
                Assert.Equal(targetLabel, branch.TargetLabel);
            },
            instruction => Assert.IsType<LIRLabel>(instruction));
    }

    [Fact]
    public void Normalize_Rewrites_TypeofFunctionStrictNotEqual_BranchWithInvertedPolarity()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var typeofResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var functionString = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var comparisonResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        int targetLabel = 9;

        body.Instructions.Add(new LIRTypeof(value, typeofResult));
        body.Instructions.Add(new LIRConstString("function", functionString));
        body.Instructions.Add(new LIRStrictNotEqualDynamic(typeofResult, functionString, comparisonResult));
        body.Instructions.Add(new LIRBranchIfTrue(comparisonResult, targetLabel));
        body.Instructions.Add(new LIRLabel(targetLabel));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.Collection(
            body.Instructions,
            instruction => Assert.IsType<LIRIsInstanceOf>(instruction),
            instruction =>
            {
                var branch = Assert.IsType<LIRBranchIfFalse>(instruction);
                Assert.Equal(4, branch.Condition.Index);
                Assert.Equal(targetLabel, branch.TargetLabel);
            },
            instruction => Assert.IsType<LIRLabel>(instruction));
    }

    [Fact]
    public void Normalize_DoesNotRewrite_TypeofFunctionStrictEqual_WhenResultUsedOutsideBranch()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var typeofResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var functionString = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var comparisonResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var copiedComparison = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        int targetLabel = 11;

        body.Instructions.Add(new LIRTypeof(value, typeofResult));
        body.Instructions.Add(new LIRConstString("function", functionString));
        body.Instructions.Add(new LIRStrictEqualDynamic(typeofResult, functionString, comparisonResult));
        body.Instructions.Add(new LIRCopyTemp(comparisonResult, copiedComparison));
        body.Instructions.Add(new LIRBranchIfTrue(comparisonResult, targetLabel));
        body.Instructions.Add(new LIRLabel(targetLabel));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.IsType<LIRTypeof>(body.Instructions[0]);
        Assert.IsType<LIRStrictEqualDynamic>(body.Instructions[2]);
        Assert.IsType<LIRBranchIfTrue>(body.Instructions[4]);
    }
}
