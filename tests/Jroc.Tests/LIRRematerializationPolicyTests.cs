using Acornima.Ast;
using Jroc.IL;
using Jroc.IR;
using Jroc.Services;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class LIRRematerializationPolicyTests
{
    [Fact]
    public void AllocationPolicy_ConstantAndParameter_AreRematerializable()
    {
        var body = new MethodBodyIR();
        var constant = new LIRConstNumber(3, new TempVariable(0));
        var parameter = new LIRLoadParameter(1, new TempVariable(1));
        var definitions = new Dictionary<int, LIRInstruction>
        {
            [0] = constant,
            [1] = parameter
        };

        Assert.True(
            LIRRematerializationPolicy.CanRematerializeForAllocation(
                constant, body, definitions));
        Assert.True(
            LIRRematerializationPolicy.CanRematerializeForAllocation(
                parameter, body, definitions));
    }

    [Fact]
    public void AllocationPolicy_ConvertFromMutableSlot_IsNotRematerializable()
    {
        var body = new MethodBodyIR();
        body.TempVariableSlots.Add(0);
        var parameter = new LIRLoadParameter(1, new TempVariable(0));
        var convert = new LIRConvertToObject(
            new TempVariable(0),
            typeof(double),
            new TempVariable(1));
        var definitions = new Dictionary<int, LIRInstruction>
        {
            [0] = parameter,
            [1] = convert
        };

        Assert.False(
            LIRRematerializationPolicy.CanRematerializeForAllocation(
                convert, body, definitions));
    }

    [Fact]
    public void StackifyPolicy_TdzCheckedLoad_IsNotRematerializable()
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript("let value;", "tdz.js");
        var declaration = Assert.IsType<VariableDeclaration>(Assert.Single(program.Body));
        var scope = new Scope("global", ScopeKind.Global, parent: null, program);
        var binding = new BindingInfo("value", BindingKind.Let, scope, declaration)
        {
            RequiresRuntimeTemporalDeadZoneChecks = true
        };
        var load = new LIRLoadLeafScopeField(
            binding,
            default,
            new ScopeId("global"),
            new TempVariable(0));

        Assert.False(
            LIRRematerializationPolicy.CanRematerializeForStackify(
                load,
                new MethodBodyIR(),
                new LIRInstruction?[] { load }));
    }

    [Fact]
    public void StackifyPolicy_Call_IsNotRematerializable()
    {
        var call = new LIRCallRuntimeServicesStatic(
            "Call",
            System.Array.Empty<TempVariable>(),
            new TempVariable(0));

        Assert.False(
            LIRRematerializationPolicy.CanRematerializeForStackify(
                call,
                new MethodBodyIR(),
                new LIRInstruction?[] { call }));
    }

    [Fact]
    public void AllocationPolicy_TwoUseConstant_IsOwnedByRematerialization()
    {
        var body = new MethodBodyIR();
        var constant = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.Instructions.Add(new LIRConstNumber(3, constant));
        body.Instructions.Add(new LIRAddNumber(constant, constant, result));
        body.Instructions.Add(new LIRReturn(result));
        var schedule = LIRStackScheduler.Identity(body);
        var plan = TempMaterializationPlan.Create(
            body,
            schedule,
            requiresConstructorResultOverride: _ => false);

        _ = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.Equal(
            TempResidency.Rematerialized,
            plan.GetResidency(constant.Index));
        Assert.Equal(
            TempValueOwner.Rematerialization,
            plan.GetOwner(constant.Index));
    }

    private static TempVariable AddTemp(
        MethodBodyIR body,
        ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }
}
