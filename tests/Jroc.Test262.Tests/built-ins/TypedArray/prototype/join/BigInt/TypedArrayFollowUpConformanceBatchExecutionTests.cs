using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.join.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.join.BigInt") { }

    [Fact(DisplayName = "custom-separator-result-from-tostring-on-each-simple-value.js")]
    public Task custom_separator_result_from_tostring_on_each_simple_value() => ExecutionTestFromFile("custom-separator-result-from-tostring-on-each-simple-value");

    [Fact(DisplayName = "empty-instance-empty-string.js")]
    public Task empty_instance_empty_string() => ExecutionTestFromFile("empty-instance-empty-string");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "result-from-tostring-on-each-simple-value.js")]
    public Task result_from_tostring_on_each_simple_value() => ExecutionTestFromFile("result-from-tostring-on-each-simple-value");

}
