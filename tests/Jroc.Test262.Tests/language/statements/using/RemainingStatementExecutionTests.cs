namespace Jroc.Test262.Tests.language.statements.using_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.using_") { }

    [Fact(DisplayName = "Symbol.dispose-getter.js")]
    public Task test_Symbol_dispose_getter()
        => ExecutionTest("Symbol.dispose-getter");

    [Fact(DisplayName = "Symbol.dispose-method-called-with-correct-this.js")]
    public Task test_Symbol_dispose_method_called_with_correct_this()
        => ExecutionTest("Symbol.dispose-method-called-with-correct-this");

    [Fact(DisplayName = "block-local-closure-get-before-initialization.js")]
    public Task test_block_local_closure_get_before_initialization()
        => ExecutionTest("block-local-closure-get-before-initialization");

    [Fact(DisplayName = "block-local-use-before-initialization-in-declaration-statement.js")]
    public Task test_block_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("block-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "block-local-use-before-initialization-in-prior-statement.js")]
    public Task test_block_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("block-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "cptn-value.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_value()
        => ExecutionTest("cptn-value");

    [Fact(DisplayName = "fn-name-arrow.js")]
    public Task test_fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class.js")]
    public Task test_fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover.js")]
    public Task test_fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn.js")]
    public Task test_fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen.js")]
    public Task test_fn_name_gen()
        => ExecutionTest("fn-name-gen");

    [Fact(DisplayName = "function-local-closure-get-before-initialization.js")]
    public Task test_function_local_closure_get_before_initialization()
        => ExecutionTest("function-local-closure-get-before-initialization");

    [Fact(DisplayName = "function-local-use-before-initialization-in-declaration-statement.js")]
    public Task test_function_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("function-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "function-local-use-before-initialization-in-prior-statement.js")]
    public Task test_function_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("function-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "gets-initializer-Symbol.dispose-property-once.js")]
    public Task test_gets_initializer_Symbol_dispose_property_once()
        => ExecutionTest("gets-initializer-Symbol.dispose-property-once");

    [Fact(DisplayName = "global-closure-get-before-initialization.js")]
    public Task test_global_closure_get_before_initialization()
        => ExecutionTest("global-closure-get-before-initialization");

    [Fact(DisplayName = "global-use-before-initialization-in-declaration-statement.js")]
    public Task test_global_use_before_initialization_in_declaration_statement()
        => ExecutionTest("global-use-before-initialization-in-declaration-statement", allowUnhandledException: true);

    [Fact(DisplayName = "global-use-before-initialization-in-prior-statement.js")]
    public Task test_global_use_before_initialization_in_prior_statement()
        => ExecutionTest("global-use-before-initialization-in-prior-statement", allowUnhandledException: true);

    [Fact(DisplayName = "initializer-disposed-at-end-of-asyncfunctionbody.js")]
    public Task test_initializer_disposed_at_end_of_asyncfunctionbody()
        => ExecutionTest("initializer-disposed-at-end-of-asyncfunctionbody");

    [Fact(DisplayName = "initializer-disposed-at-end-of-asyncgeneratorbody.js")]
    public Task test_initializer_disposed_at_end_of_asyncgeneratorbody()
        => ExecutionTest("initializer-disposed-at-end-of-asyncgeneratorbody");

    [Fact(DisplayName = "initializer-disposed-at-end-of-block.js")]
    public Task test_initializer_disposed_at_end_of_block()
        => ExecutionTest("initializer-disposed-at-end-of-block");

    [Fact(DisplayName = "initializer-disposed-at-end-of-forstatement.js")]
    public Task test_initializer_disposed_at_end_of_forstatement()
        => ExecutionTest("initializer-disposed-at-end-of-forstatement");

    [Fact(DisplayName = "initializer-disposed-at-end-of-functionbody.js")]
    public Task test_initializer_disposed_at_end_of_functionbody()
        => ExecutionTest("initializer-disposed-at-end-of-functionbody");

    [Fact(DisplayName = "initializer-disposed-at-end-of-generatorbody.js")]
    public Task test_initializer_disposed_at_end_of_generatorbody()
        => ExecutionTest("initializer-disposed-at-end-of-generatorbody");

    [Fact(DisplayName = "initializer-disposed-if-subsequent-initializer-throws-in-forstatement-head.js")]
    public Task test_initializer_disposed_if_subsequent_initializer_throws_in_forstatement_head()
        => ExecutionTest("initializer-disposed-if-subsequent-initializer-throws-in-forstatement-head");

    [Fact(DisplayName = "initializer-disposed-if-subsequent-initializer-throws.js")]
    public Task test_initializer_disposed_if_subsequent_initializer_throws()
        => ExecutionTest("initializer-disposed-if-subsequent-initializer-throws");

    [Fact(DisplayName = "multiple-resources-disposed-in-reverse-order.js")]
    public Task test_multiple_resources_disposed_in_reverse_order()
        => ExecutionTest("multiple-resources-disposed-in-reverse-order");

    [Fact(DisplayName = "puts-initializer-on-top-of-disposableresourcestack-multiple-bindings.js")]
    public Task test_puts_initializer_on_top_of_disposableresourcestack_multiple_bindings()
        => ExecutionTest("puts-initializer-on-top-of-disposableresourcestack-multiple-bindings");

    [Fact(DisplayName = "puts-initializer-on-top-of-disposableresourcestack-subsequent-usings.js")]
    public Task test_puts_initializer_on_top_of_disposableresourcestack_subsequent_usings()
        => ExecutionTest("puts-initializer-on-top-of-disposableresourcestack-subsequent-usings");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task test_static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");

    [Fact(DisplayName = "throws-error-as-is-if-only-one-error-during-disposal.js")]
    public Task test_throws_error_as_is_if_only_one_error_during_disposal()
        => ExecutionTest("throws-error-as-is-if-only-one-error-during-disposal");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-is-null.js")]
    public Task test_throws_if_initializer_Symbol_dispose_property_is_null()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-is-null");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-is-undefined.js")]
    public Task test_throws_if_initializer_Symbol_dispose_property_is_undefined()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-is-undefined");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-not-callable.js")]
    public Task test_throws_if_initializer_Symbol_dispose_property_not_callable()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-not-callable");

    [Fact(DisplayName = "throws-if-initializer-missing-Symbol.dispose.js")]
    public Task test_throws_if_initializer_missing_Symbol_dispose()
        => ExecutionTest("throws-if-initializer-missing-Symbol.dispose");

    [Fact(DisplayName = "throws-if-initializer-not-object.js")]
    public Task test_throws_if_initializer_not_object()
        => ExecutionTest("throws-if-initializer-not-object");

    [Fact(DisplayName = "throws-suppressederror-if-multiple-errors-during-disposal.js")]
    public Task test_throws_suppressederror_if_multiple_errors_during_disposal()
        => ExecutionTest("throws-suppressederror-if-multiple-errors-during-disposal");

    [Fact(DisplayName = "using-allows-null-initializer.js")]
    public Task test_using_allows_null_initializer()
        => ExecutionTest("using-allows-null-initializer");

    [Fact(DisplayName = "using-allows-undefined-initializer.js")]
    public Task test_using_allows_undefined_initializer()
        => ExecutionTest("using-allows-undefined-initializer");

}
