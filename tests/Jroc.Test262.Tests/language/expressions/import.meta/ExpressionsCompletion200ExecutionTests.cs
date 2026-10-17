namespace Jroc.Test262.Tests.language.expressions.import_meta;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.import_meta") { }

    [Fact(DisplayName = "distinct-for-each-module.js")]
    public Task distinct_for_each_module()
        => ExecutionTest("distinct-for-each-module");

    [Fact(DisplayName = "import-meta-is-an-ordinary-object.js")]
    public Task import_meta_is_an_ordinary_object()
        => ExecutionTest("import-meta-is-an-ordinary-object");

    [Fact(DisplayName = "not-accessible-from-direct-eval.js", Skip = "eval is not supported.")]
    public Task not_accessible_from_direct_eval()
        => ExecutionTest("not-accessible-from-direct-eval");

    [Fact(DisplayName = "same-object-returned.js")]
    public Task same_object_returned()
        => ExecutionTest("same-object-returned");
}
