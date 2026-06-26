using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.from;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.from") { }

    [Fact(DisplayName = "calling-from-valid-1-noStrict")]
    public Task calling_from_valid_1_noStrict()
        => ExecutionTest("calling-from-valid-1-noStrict");

    [Fact(DisplayName = "array-like-has-length-but-no-indexes-with-values")]
    public Task array_like_has_length_but_no_indexes_with_values()
        => ExecutionTest("array-like-has-length-but-no-indexes-with-values");

    [Fact(DisplayName = "Array.from-descriptor")]
    public Task Array_from_descriptor()
        => ExecutionTest("Array.from-descriptor");

    [Fact(DisplayName = "Array.from-name")]
    public Task Array_from_name()
        => ExecutionTest("Array.from-name");

    [Fact(DisplayName = "Array.from_arity")]
    public Task Array_from_arity()
        => ExecutionTest("Array.from_arity");

    [Fact(DisplayName = "from-string")]
    public Task from_string()
        => ExecutionTest("from-string");

    [Fact(DisplayName = "from-array")]
    public Task from_array()
        => ExecutionTest("from-array");

}
