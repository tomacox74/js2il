using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toJSON;

public partial class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.toJSON") { }

    [Fact(DisplayName = "builtin")]
    public Task builtin()
        => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "invoke-abrupt")]
    public Task invoke_abrupt()
        => ExecutionTestFromFile("invoke-abrupt");

    [Fact(DisplayName = "invoke-arguments")]
    public Task invoke_arguments()
        => ExecutionTestFromFile("invoke-arguments");

    [Fact(DisplayName = "invoke-result")]
    public Task invoke_result()
        => ExecutionTestFromFile("invoke-result");

    [Fact(DisplayName = "non-finite")]
    public Task non_finite()
        => ExecutionTestFromFile("non-finite");

    [Fact(DisplayName = "to-object")]
    public Task to_object()
        => ExecutionTestFromFile("to-object");

    [Fact(DisplayName = "to-primitive-abrupt")]
    public Task to_primitive_abrupt()
        => ExecutionTestFromFile("to-primitive-abrupt");

    [Fact(DisplayName = "to-primitive-symbol")]
    public Task to_primitive_symbol()
        => ExecutionTestFromFile("to-primitive-symbol");
}
