using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "exec-args")]
    public Task exec_args()
        => ExecutionTestFromFile("exec-args");

    [Fact(DisplayName = "executor-call-context-sloppy")]
    public Task executor_call_context_sloppy()
        => ExecutionTestFromFile("executor-call-context-sloppy");

    [Fact(DisplayName = "executor-call-context-strict")]
    public Task executor_call_context_strict()
        => ExecutionTestFromFile("executor-call-context-strict");

    [Fact(DisplayName = "executor-function-extensible")]
    public Task executor_function_extensible()
        => ExecutionTestFromFile("executor-function-extensible");

    [Fact(DisplayName = "executor-function-property-order")]
    public Task executor_function_property_order()
        => ExecutionTestFromFile("executor-function-property-order");

    [Fact(DisplayName = "executor-function-prototype")]
    public Task executor_function_prototype()
        => ExecutionTestFromFile("executor-function-prototype");

    [Fact(DisplayName = "property-order")]
    public Task property_order()
        => ExecutionTestFromFile("property-order");

    [Fact(DisplayName = "reject-function-extensible")]
    public Task reject_function_extensible()
        => ExecutionTestFromFile("reject-function-extensible");

    [Fact(DisplayName = "resolve-function-extensible")]
    public Task resolve_function_extensible()
        => ExecutionTestFromFile("resolve-function-extensible");
}
