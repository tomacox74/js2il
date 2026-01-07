using System;
using System.Linq;
using Js2IL.Services;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Xunit;

namespace Js2IL.Tests.Integration;

public class Milestone2bPlannerTests
{
    private static (SymbolTable symbolTable, TwoPhaseCompilationCoordinator coordinator) BuildCoordinator(string js)
    {
        var parser = new JavaScriptParser();
        var ast = parser.ParseJavaScript(js, "test.js");
        var module = new ModuleDefinition
        {
            Ast = ast,
            Name = "TestModule",
            Path = "test.js"
        };

        var builder = new SymbolTableBuilder();
        builder.Build(module);
        var symbolTable = module.SymbolTable ?? throw new InvalidOperationException("Symbol table was not built.");

        var logger = new TestLogger();
        var options = new CompilerOptions { Verbose = true };
        var registry = new CallableRegistry();
        var coordinator = new TwoPhaseCompilationCoordinator(logger, options, registry);
        coordinator.RunPhase1Discovery(symbolTable);

        return (symbolTable, coordinator);
    }

    [Fact]
    public void Milestone2b_Planner_IsDeterministic()
    {
        var js = @"
function toStr(x) { return '' + x; }

function main() {
  const arr = [1,2,3];
  return arr.map(toStr).join(',');
}
";

        var (symbolTable, coordinator) = BuildCoordinator(js);

        var plan1 = coordinator.ComputeMilestone2bPlan(symbolTable);
        var plan2 = coordinator.ComputeMilestone2bPlan(symbolTable);

        Assert.Equal(plan1.ToDebugString(), plan2.ToDebugString());
    }

    [Fact]
    public void Milestone2b_Collects_FunctionValueDependency_Edge()
    {
        var js = @"
function toStr(x) { return '' + x; }
function main() { return [1].map(toStr)[0]; }
";

        var (symbolTable, coordinator) = BuildCoordinator(js);
        var plan = coordinator.ComputeMilestone2bPlan(symbolTable);

        var main = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.FunctionDeclaration && c.Name == "main");
        var toStr = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.FunctionDeclaration && c.Name == "toStr");

        Assert.Contains(toStr, plan.Graph.GetDependencies(main));
    }

    [Fact]
    public void Milestone2b_Collects_NewClassConstructorDependency_WhenIdentifierResolvable()
    {
        var js = @"
class C { }
function f() { return new C(); }
";

        var (symbolTable, coordinator) = BuildCoordinator(js);
        var plan = coordinator.ComputeMilestone2bPlan(symbolTable);

        var f = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.FunctionDeclaration && c.Name == "f");
        var ctor = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassConstructor && c.Name == "C");

        Assert.Contains(ctor, plan.Graph.GetDependencies(f));
    }
}
