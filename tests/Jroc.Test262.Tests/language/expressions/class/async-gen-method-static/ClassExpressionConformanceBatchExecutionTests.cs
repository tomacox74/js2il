using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.class_.async_gen_method_static;

public class ClassExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassExpressionConformanceBatchExecutionTests() : base("language/expressions/class/async-gen-method-static", "language.expressions.class_.async_gen_method_static") { }

    [Fact(DisplayName = "async-gen-method-static/dflt-params-abrupt.js")]
    public Task async_gen_method_static_dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "async-gen-method-static/dflt-params-ref-later.js")]
    public Task async_gen_method_static_dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "async-gen-method-static/dflt-params-ref-self.js")]
    public Task async_gen_method_static_dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");
}
