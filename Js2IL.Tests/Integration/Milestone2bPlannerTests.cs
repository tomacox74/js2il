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

        [Fact]
        public void Milestone2b1_Collects_ThisMethodDependency_WhenResolvable()
        {
                var js = @"
class C {
    a() { return this.b(); }
    b() { return 1; }
}
function f() { return new C().a(); }
";

                var (symbolTable, coordinator) = BuildCoordinator(js);
                var plan = coordinator.ComputeMilestone2bPlan(symbolTable);

                var a = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "C.a");
                var b = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "C.b");

                Assert.Contains(b, plan.Graph.GetDependencies(a));
        }

        [Fact]
        public void Milestone2b1_Collects_SuperMethodDependency_WhenResolvable()
        {
                var js = @"
class Base {
    m() { return 1; }
}
class Derived extends Base {
    n() { return super.m(); }
}
function f() { return new Derived().n(); }
";

                var (symbolTable, coordinator) = BuildCoordinator(js);
                var plan = coordinator.ComputeMilestone2bPlan(symbolTable);

                var n = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "Derived.n");
                var m = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "Base.m");

                Assert.Contains(m, plan.Graph.GetDependencies(n));
        }

        [Fact]
        public void Milestone2b1_DoesNotCollect_ObjMemberCallDependency()
        {
                var js = @"
class C {
    a(obj) { return obj.b(); }
    b() { return 1; }
}
function f() { return new C().a({ b: () => 2 }); }
";

                var (symbolTable, coordinator) = BuildCoordinator(js);
                var plan = coordinator.ComputeMilestone2bPlan(symbolTable);

                var a = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "C.a");
                var b = plan.Graph.NodesInStableOrder.Single(c => c.Kind == CallableKind.ClassMethod && c.Name == "C.b");

                Assert.DoesNotContain(b, plan.Graph.GetDependencies(a));
        }
}
