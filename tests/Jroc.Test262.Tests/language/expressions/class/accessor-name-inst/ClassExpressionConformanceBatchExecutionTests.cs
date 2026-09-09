using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.class_.accessor_name_inst;

public class ClassExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassExpressionConformanceBatchExecutionTests() : base("language/expressions/class/accessor-name-inst", "language.expressions.class_.accessor_name_inst") { }

    [Fact(DisplayName = "accessor-name-inst/computed-err-to-prop-key.js")]
    public Task accessor_name_inst_computed_err_to_prop_key()
        => ExecutionTest("computed-err-to-prop-key");
}
