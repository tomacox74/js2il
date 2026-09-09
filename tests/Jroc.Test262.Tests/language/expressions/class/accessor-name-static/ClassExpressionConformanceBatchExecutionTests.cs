using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.class_.accessor_name_static;

public class ClassExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassExpressionConformanceBatchExecutionTests() : base("language/expressions/class/accessor-name-static", "language.expressions.class_.accessor_name_static") { }

    [Fact(DisplayName = "accessor-name-static/computed-err-to-prop-key.js")]
    public Task accessor_name_static_computed_err_to_prop_key()
        => ExecutionTest("computed-err-to-prop-key");
}
