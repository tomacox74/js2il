using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.assignment.destructuring;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.assignment.destructuring") { }

    [Fact(DisplayName = "default-expr-throws-iterator-return-get-throws")]
    public Task default_expr_throws_iterator_return_get_throws()
        => ExecutionTest("default-expr-throws-iterator-return-get-throws");

    [Fact(DisplayName = "default-expr-throws-iterator-return-is-not-callable")]
    public Task default_expr_throws_iterator_return_is_not_callable()
        => ExecutionTest("default-expr-throws-iterator-return-is-not-callable");

    [Fact(DisplayName = "iterator-destructuring-property-reference-target-evaluation-order")]
    public Task iterator_destructuring_property_reference_target_evaluation_order()
        => ExecutionTest("iterator-destructuring-property-reference-target-evaluation-order");

    [Fact(DisplayName = "keyed-destructuring-property-reference-target-evaluation-order-with-bindings")]
    public Task keyed_destructuring_property_reference_target_evaluation_order_with_bindings()
        => ExecutionTest("keyed-destructuring-property-reference-target-evaluation-order-with-bindings");

    [Fact(DisplayName = "keyed-destructuring-property-reference-target-evaluation-order")]
    public Task keyed_destructuring_property_reference_target_evaluation_order()
        => ExecutionTest("keyed-destructuring-property-reference-target-evaluation-order");

    [Fact(DisplayName = "obj-prop-__proto__dup")]
    public Task obj_prop_proto_dup()
        => ExecutionTest("obj-prop-__proto__dup");

    [Fact(DisplayName = "target-assign-throws-iterator-return-get-throws")]
    public Task target_assign_throws_iterator_return_get_throws()
        => ExecutionTest("target-assign-throws-iterator-return-get-throws");

    [Fact(DisplayName = "target-assign-throws-iterator-return-is-not-callable")]
    public Task target_assign_throws_iterator_return_is_not_callable()
        => ExecutionTest("target-assign-throws-iterator-return-is-not-callable");

    [Fact(DisplayName = "array-empty-val-array")]
    public Task array_empty_val_array()
        => ExecutionTest("array-empty-val-array");

    [Fact(DisplayName = "array-empty-val-bool")]
    public Task array_empty_val_bool()
        => ExecutionTest("array-empty-val-bool");

    [Fact(DisplayName = "array-empty-val-num")]
    public Task array_empty_val_num()
        => ExecutionTest("array-empty-val-num");

    [Fact(DisplayName = "array-empty-val-string")]
    public Task array_empty_val_string()
        => ExecutionTest("array-empty-val-string");

    [Fact(DisplayName = "array-empty-val-symbol")]
    public Task array_empty_val_symbol()
        => ExecutionTest("array-empty-val-symbol");

    [Fact(DisplayName = "array-empty-val-undef")]
    public Task array_empty_val_undef()
        => ExecutionTest("array-empty-val-undef");

    [Fact(DisplayName = "array-elision-val-array")]
    public Task array_elision_val_array()
        => ExecutionTest("array-elision-val-array");

    [Fact(DisplayName = "array-elision-val-bool")]
    public Task array_elision_val_bool()
        => ExecutionTest("array-elision-val-bool");

    [Fact(DisplayName = "array-elision-val-num")]
    public Task array_elision_val_num()
        => ExecutionTest("array-elision-val-num");

    [Fact(DisplayName = "array-elision-val-string")]
    public Task array_elision_val_string()
        => ExecutionTest("array-elision-val-string");

    [Fact(DisplayName = "array-elision-val-symbol")]
    public Task array_elision_val_symbol()
        => ExecutionTest("array-elision-val-symbol");

    [Fact(DisplayName = "array-elision-val-undef")]
    public Task array_elision_val_undef()
        => ExecutionTest("array-elision-val-undef");

    [Fact(DisplayName = "array-iteration")]
    public Task array_iteration()
        => ExecutionTest("array-iteration");

    [Fact(DisplayName = "array-rest-iteration")]
    public Task array_rest_iteration()
        => ExecutionTest("array-rest-iteration");

    [Fact(DisplayName = "array-rest-lref")]
    public Task array_rest_lref()
        => ExecutionTest("array-rest-lref");

    [Fact(DisplayName = "array-rest-put-prop-ref")]
    public Task array_rest_put_prop_ref()
        => ExecutionTest("array-rest-put-prop-ref");

    [Fact(DisplayName = "obj-rest-empty-obj")]
    public Task obj_rest_empty_obj()
        => ExecutionTest("obj-rest-empty-obj");

    [Fact(DisplayName = "obj-rest-valid-object")]
    public Task obj_rest_valid_object()
        => ExecutionTest("obj-rest-valid-object");

    [Fact(DisplayName = "obj-rest-order")]
    public Task obj_rest_order()
        => ExecutionTest("obj-rest-order");

    [Fact(DisplayName = "obj-rest-getter")]
    public Task obj_rest_getter()
        => ExecutionTest("obj-rest-getter");

    [Fact(DisplayName = "obj-rest-skip-non-enumerable")]
    public Task obj_rest_skip_non_enumerable()
        => ExecutionTest("obj-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-rest-same-name")]
    public Task obj_rest_same_name()
        => ExecutionTest("obj-rest-same-name");

    [Fact(DisplayName = "obj-rest-symbol-val")]
    public Task obj_rest_symbol_val()
        => ExecutionTest("obj-rest-symbol-val");

    [Fact(DisplayName = "obj-rest-number")]
    public Task obj_rest_number()
        => ExecutionTest("obj-rest-number");

    [Fact(DisplayName = "obj-rest-to-property")]
    public Task obj_rest_to_property()
        => ExecutionTest("obj-rest-to-property");
}
