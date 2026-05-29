using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.function;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.function") { }

    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");

    [Fact(DisplayName = "params-dflt-ref-arguments")]
    public Task params_dflt_ref_arguments()
        => ExecutionTest("params-dflt-ref-arguments");
}
