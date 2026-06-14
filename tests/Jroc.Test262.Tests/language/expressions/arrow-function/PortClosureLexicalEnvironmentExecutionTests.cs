using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "scope-param-elem-var-close")]
    public Task scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open")]
    public Task scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");
}
