using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise") { }

    [Fact(DisplayName = "executor-not-callable")]
    public Task executor_not_callable() => ExecutionTestFromFile("executor-not-callable");

    [Fact(DisplayName = "get-prototype-abrupt-executor-not-callable")]
    public Task get_prototype_abrupt_executor_not_callable() => ExecutionTestFromFile("get-prototype-abrupt-executor-not-callable");

    [Fact(DisplayName = "get-prototype-abrupt")]
    public Task get_prototype_abrupt() => ExecutionTestFromFile("get-prototype-abrupt");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "reject-function-name")]
    public Task reject_function_name() => ExecutionTestFromFile("reject-function-name");

    [Fact(DisplayName = "reject-function-nonconstructor")]
    public Task reject_function_nonconstructor() => ExecutionTestFromFile("reject-function-nonconstructor");

    [Fact(DisplayName = "reject-function-property-order")]
    public Task reject_function_property_order() => ExecutionTestFromFile("reject-function-property-order");

    [Fact(DisplayName = "resolve-function-name")]
    public Task resolve_function_name() => ExecutionTestFromFile("resolve-function-name");

    [Fact(DisplayName = "resolve-function-nonconstructor")]
    public Task resolve_function_nonconstructor() => ExecutionTestFromFile("resolve-function-nonconstructor");

    [Fact(DisplayName = "resolve-function-property-order")]
    public Task resolve_function_property_order() => ExecutionTestFromFile("resolve-function-property-order");

    [Fact(DisplayName = "undefined-newtarget")]
    public Task undefined_newtarget() => ExecutionTestFromFile("undefined-newtarget");
}
