using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.from;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.from") { }

    [Fact(DisplayName = "from-array-mapper-makes-result-out-of-bounds.js")]
    public Task from_array_mapper_makes_result_out_of_bounds() => ExecutionTestFromFile("from-array-mapper-makes-result-out-of-bounds");

    [Fact(DisplayName = "from-typedarray-into-itself-mapper-makes-result-out-of-bounds.js")]
    public Task from_typedarray_into_itself_mapper_makes_result_out_of_bounds() => ExecutionTestFromFile("from-typedarray-into-itself-mapper-makes-result-out-of-bounds");

    [Fact(DisplayName = "from-typedarray-mapper-makes-result-out-of-bounds.js")]
    public Task from_typedarray_mapper_makes_result_out_of_bounds() => ExecutionTestFromFile("from-typedarray-mapper-makes-result-out-of-bounds");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
