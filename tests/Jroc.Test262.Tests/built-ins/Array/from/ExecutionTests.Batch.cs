namespace Jroc.Test262.Tests.built_ins.Array.from;

public partial class ExecutionTests
{
    [Fact(DisplayName = "calling-from-valid-1-onlyStrict")]
    public Task calling_from_valid_1_onlyStrict() => ExecutionTestFromFile("calling-from-valid-1-onlyStrict");
    [Fact(DisplayName = "calling-from-valid-2")]
    public Task calling_from_valid_2() => ExecutionTestFromFile("calling-from-valid-2");
    [Fact(DisplayName = "elements-added-after")]
    public Task elements_added_after() => ExecutionTestFromFile("elements-added-after");
    [Fact(DisplayName = "elements-updated-after")]
    public Task elements_updated_after() => ExecutionTestFromFile("elements-updated-after");
    [Fact(DisplayName = "get-iter-method-err")]
    public Task get_iter_method_err() => ExecutionTestFromFile("get-iter-method-err");
    [Fact(DisplayName = "items-is-null-throws")]
    public Task items_is_null_throws() => ExecutionTestFromFile("items-is-null-throws");
    [Fact(DisplayName = "iter-adv-err")]
    public Task iter_adv_err() => ExecutionTestFromFile("iter-adv-err");
    [Fact(DisplayName = "iter-get-iter-err")]
    public Task iter_get_iter_err() => ExecutionTestFromFile("iter-get-iter-err");
    [Fact(DisplayName = "iter-get-iter-val-err")]
    public Task iter_get_iter_val_err() => ExecutionTestFromFile("iter-get-iter-val-err");
    [Fact(DisplayName = "iter-map-fn-args")]
    public Task iter_map_fn_args() => ExecutionTestFromFile("iter-map-fn-args");
    [Fact(DisplayName = "iter-map-fn-return")]
    public Task iter_map_fn_return() => ExecutionTestFromFile("iter-map-fn-return");
    [Fact(DisplayName = "iter-map-fn-this-arg")]
    public Task iter_map_fn_this_arg() => ExecutionTestFromFile("iter-map-fn-this-arg");
    [Fact(DisplayName = "iter-map-fn-this-non-strict")]
    public Task iter_map_fn_this_non_strict() => ExecutionTestFromFile("iter-map-fn-this-non-strict");
    [Fact(DisplayName = "iter-map-fn-this-strict")]
    public Task iter_map_fn_this_strict() => ExecutionTestFromFile("iter-map-fn-this-strict");
    [Fact(DisplayName = "iter-set-elem-prop")]
    public Task iter_set_elem_prop() => ExecutionTestFromFile("iter-set-elem-prop");
    [Fact(DisplayName = "iter-set-elem-prop-non-writable")]
    public Task iter_set_elem_prop_non_writable() => ExecutionTestFromFile("iter-set-elem-prop-non-writable");
    [Fact(DisplayName = "iter-set-length")]
    public Task iter_set_length() => ExecutionTestFromFile("iter-set-length");
    [Fact(DisplayName = "mapfn-throws-exception")]
    public Task mapfn_throws_exception() => ExecutionTestFromFile("mapfn-throws-exception");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "source-array-boundary")]
    public Task source_array_boundary() => ExecutionTestFromFile("source-array-boundary");
    [Fact(DisplayName = "source-object-iterator-1")]
    public Task source_object_iterator_1() => ExecutionTestFromFile("source-object-iterator-1");
    [Fact(DisplayName = "source-object-iterator-2")]
    public Task source_object_iterator_2() => ExecutionTestFromFile("source-object-iterator-2");
    [Fact(DisplayName = "source-object-length")]
    public Task source_object_length() => ExecutionTestFromFile("source-object-length");
    [Fact(DisplayName = "source-object-length-set-elem-prop-non-writable")]
    public Task source_object_length_set_elem_prop_non_writable() => ExecutionTestFromFile("source-object-length-set-elem-prop-non-writable");
    [Fact(DisplayName = "source-object-missing")]
    public Task source_object_missing() => ExecutionTestFromFile("source-object-missing");
    [Fact(DisplayName = "this-null")]
    public Task this_null() => ExecutionTestFromFile("this-null");
}
