namespace Jroc.Test262.Tests.built_ins.Array.from;

public partial class ExecutionTests
{
    [Fact(DisplayName = "Array.from_forwards-length-for-array-likes.js")]
    public Task Array_from_forwards_length_for_array_likes()
        => ExecutionTestFromFile("Array.from_forwards-length-for-array-likes");

    [Fact(DisplayName = "items-is-arraybuffer.js")]
    public Task items_is_arraybuffer()
        => ExecutionTestFromFile("items-is-arraybuffer");

    [Fact(DisplayName = "iter-cstm-ctor-err.js")]
    public Task iter_cstm_ctor_err() => ExecutionTestFromFile("iter-cstm-ctor-err");

    [Fact(DisplayName = "iter-cstm-ctor.js")]
    public Task iter_cstm_ctor() => ExecutionTestFromFile("iter-cstm-ctor");

    [Fact(DisplayName = "iter-map-fn-err.js")]
    public Task iter_map_fn_err() => ExecutionTestFromFile("iter-map-fn-err");

    [Fact(DisplayName = "iter-set-elem-prop-err.js")]
    public Task iter_set_elem_prop_err() => ExecutionTestFromFile("iter-set-elem-prop-err");

    [Fact(DisplayName = "iter-set-length-err.js")]
    public Task iter_set_length_err() => ExecutionTestFromFile("iter-set-length-err");

    [Fact(DisplayName = "mapfn-is-not-callable-typeerror.js")]
    public Task mapfn_is_not_callable_typeerror()
        => ExecutionTestFromFile("mapfn-is-not-callable-typeerror");

    [Fact(DisplayName = "source-object-length-set-elem-prop-err.js")]
    public Task source_object_length_set_elem_prop_err()
        => ExecutionTestFromFile("source-object-length-set-elem-prop-err");

    [Fact(DisplayName = "source-object-without.js")]
    public Task source_object_without() => ExecutionTestFromFile("source-object-without");
}
