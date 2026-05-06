using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.function;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.function") { }

[Fact(DisplayName = "13.0-12-s")]
    public Task _13_0_12_s()
        => ExecutionTest("13.0-12-s");

[Fact(DisplayName = "13.0-13-s", Skip = "Strict-mode Function constructor/eval SyntaxError handling is incomplete.")]
    public Task _13_0_13_s()
        => ExecutionTest("13.0-13-s");

[Fact(DisplayName = "13.0-14-s", Skip = "Strict-mode Function constructor/eval SyntaxError handling is incomplete.")]
    public Task _13_0_14_s()
        => ExecutionTest("13.0-14-s");

[Fact(DisplayName = "13.0-15-s", Skip = "Strict-mode Function constructor/eval SyntaxError handling is incomplete.")]
    public Task _13_0_15_s()
        => ExecutionTest("13.0-15-s");

[Fact(DisplayName = "13.0-16-s", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _13_0_16_s()
        => ExecutionTest("13.0-16-s");

[Fact(DisplayName = "13.0-17-s", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _13_0_17_s()
        => ExecutionTest("13.0-17-s");

[Fact(DisplayName = "13.0-7-s", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _13_0_7_s()
        => ExecutionTest("13.0-7-s");

[Fact(DisplayName = "13.0-8-s", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _13_0_8_s()
        => ExecutionTest("13.0-8-s");
}
