using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.from;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.from") { }

        [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method()
        => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "mapfn-is-not-callable")]
    public Task mapfn_is_not_callable()
        => ExecutionTestFromFile("mapfn-is-not-callable");

    [Fact(DisplayName = "this-is-not-constructor")]
    public Task this_is_not_constructor()
        => ExecutionTestFromFile("this-is-not-constructor");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func()
        => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "arylk-get-length-error")]
    public Task arylk_get_length_error()
        => ExecutionTestFromFile("arylk-get-length-error");

    [Fact(DisplayName = "arylk-to-length-error")]
    public Task arylk_to_length_error()
        => ExecutionTestFromFile("arylk-to-length-error");

    [Fact(DisplayName = "from-array-mapper-detaches-result")]
    public Task from_array_mapper_detaches_result()
        => ExecutionTestFromFile("from-array-mapper-detaches-result");

    [Fact(DisplayName = "from-typedarray-into-itself-mapper-detaches-result")]
    public Task from_typedarray_into_itself_mapper_detaches_result()
        => ExecutionTestFromFile("from-typedarray-into-itself-mapper-detaches-result");

    [Fact(DisplayName = "from-typedarray-mapper-detaches-result")]
    public Task from_typedarray_mapper_detaches_result()
        => ExecutionTestFromFile("from-typedarray-mapper-detaches-result");

    [Fact(DisplayName = "iter-access-error")]
    public Task iter_access_error()
        => ExecutionTestFromFile("iter-access-error");

    [Fact(DisplayName = "iter-invoke-error")]
    public Task iter_invoke_error()
        => ExecutionTestFromFile("iter-invoke-error");

    [Fact(DisplayName = "iter-next-error")]
    public Task iter_next_error()
        => ExecutionTestFromFile("iter-next-error");

    [Fact(DisplayName = "iter-next-value-error")]
    public Task iter_next_value_error()
        => ExecutionTestFromFile("iter-next-value-error");
}
