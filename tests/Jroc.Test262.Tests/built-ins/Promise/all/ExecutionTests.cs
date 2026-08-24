using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.all;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.all") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S25.4.4.1_A4.1_T1")]
    public Task S25_4_4_1_A4_1_T1()
        => ExecutionTestFromFile("S25.4.4.1_A4.1_T1");

    [Fact(DisplayName = "call-resolve-element-after-return")]
    public Task call_resolve_element_after_return()
        => ExecutionTestFromFile("call-resolve-element-after-return");

    [Fact(DisplayName = "call-resolve-element-items")]
    public Task call_resolve_element_items()
        => ExecutionTestFromFile("call-resolve-element-items");

    [Fact(DisplayName = "call-resolve-element")]
    public Task call_resolve_element()
        => ExecutionTestFromFile("call-resolve-element");

    [Fact(DisplayName = "capability-executor-called-twice")]
    public Task capability_executor_called_twice()
        => ExecutionTestFromFile("capability-executor-called-twice");

    [Fact(DisplayName = "capability-executor-not-callable")]
    public Task capability_executor_not_callable()
        => ExecutionTestFromFile("capability-executor-not-callable");

    [Fact(DisplayName = "capability-resolve-throws-no-close")]
    public Task capability_resolve_throws_no_close()
        => ExecutionTestFromFile("capability-resolve-throws-no-close");

    [Fact(DisplayName = "ctx-ctor-throws")]
    public Task ctx_ctor_throws()
        => ExecutionTestFromFile("ctx-ctor-throws");

    [Fact(DisplayName = "ctx-ctor")]
    public Task ctx_ctor()
        => ExecutionTestFromFile("ctx-ctor");

    [Fact(DisplayName = "ctx-non-object")]
    public Task ctx_non_object()
        => ExecutionTestFromFile("ctx-non-object");

    [Fact(DisplayName = "invoke-resolve-error-close")]
    public Task invoke_resolve_error_close()
        => ExecutionTestFromFile("invoke-resolve-error-close");
}
