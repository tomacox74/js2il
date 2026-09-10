using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.all;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.all") { }

    [Fact(DisplayName = "S25.4.4.1_A1.1_T1")]
    public Task S25_4_4_1_A1_1_T1() => ExecutionTestFromFile("S25.4.4.1_A1.1_T1");

    [Fact(DisplayName = "S25.4.4.1_A2.1_T1")]
    public Task S25_4_4_1_A2_1_T1() => ExecutionTestFromFile("S25.4.4.1_A2.1_T1");

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "invoke-resolve-get-once-no-calls")]
    public Task invoke_resolve_get_once_no_calls() => ExecutionTestFromFile("invoke-resolve-get-once-no-calls");

    [Fact(DisplayName = "invoke-resolve-return")]
    public Task invoke_resolve_return() => ExecutionTestFromFile("invoke-resolve-return");

    [Fact(DisplayName = "invoke-resolve")]
    public Task invoke_resolve() => ExecutionTestFromFile("invoke-resolve");

    [Fact(DisplayName = "invoke-then")]
    public Task invoke_then() => ExecutionTestFromFile("invoke-then");

    [Fact(DisplayName = "new-resolve-function")]
    public Task new_resolve_function() => ExecutionTestFromFile("new-resolve-function");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "resolve-before-loop-exit-from-same")]
    public Task resolve_before_loop_exit_from_same() => ExecutionTestFromFile("resolve-before-loop-exit-from-same");

    [Fact(DisplayName = "resolve-before-loop-exit")]
    public Task resolve_before_loop_exit() => ExecutionTestFromFile("resolve-before-loop-exit");

    [Fact(DisplayName = "resolve-element-function-extensible")]
    public Task resolve_element_function_extensible() => ExecutionTestFromFile("resolve-element-function-extensible");

    [Fact(DisplayName = "resolve-element-function-length")]
    public Task resolve_element_function_length() => ExecutionTestFromFile("resolve-element-function-length");

    [Fact(DisplayName = "resolve-element-function-name")]
    public Task resolve_element_function_name() => ExecutionTestFromFile("resolve-element-function-name");

    [Fact(DisplayName = "resolve-element-function-nonconstructor")]
    public Task resolve_element_function_nonconstructor() => ExecutionTestFromFile("resolve-element-function-nonconstructor");

    [Fact(DisplayName = "resolve-element-function-property-order")]
    public Task resolve_element_function_property_order() => ExecutionTestFromFile("resolve-element-function-property-order");

    [Fact(DisplayName = "resolve-element-function-prototype")]
    public Task resolve_element_function_prototype() => ExecutionTestFromFile("resolve-element-function-prototype");

    [Fact(DisplayName = "resolve-from-same-thenable")]
    public Task resolve_from_same_thenable() => ExecutionTestFromFile("resolve-from-same-thenable");

    [Fact(DisplayName = "same-reject-function")]
    public Task same_reject_function() => ExecutionTestFromFile("same-reject-function");

    [Fact(DisplayName = "species-get-error")]
    public Task species_get_error() => ExecutionTestFromFile("species-get-error");
}
