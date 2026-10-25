namespace Jroc.Test262.Tests.language.statements.await_using;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.await_using") { }

    [Fact(DisplayName = "Symbol.asyncDispose-getter.js")]
    public Task Symbol_asyncDispose_getter()
        => ExecutionTest("Symbol.asyncDispose-getter");

    [Fact(DisplayName = "Symbol.asyncDispose-method-called-with-correct-this.js")]
    public Task Symbol_asyncDispose_method_called_with_correct_this()
        => ExecutionTest("Symbol.asyncDispose-method-called-with-correct-this");

    [Fact(DisplayName = "Symbol.asyncDispose-method-not-async.js")]
    public Task Symbol_asyncDispose_method_not_async()
        => ExecutionTest("Symbol.asyncDispose-method-not-async");

    [Fact(DisplayName = "Symbol.dispose-getter.js")]
    public Task Symbol_dispose_getter()
        => ExecutionTest("Symbol.dispose-getter");

    [Fact(DisplayName = "Symbol.dispose-method-called-with-correct-this.js")]
    public Task Symbol_dispose_method_called_with_correct_this()
        => ExecutionTest("Symbol.dispose-method-called-with-correct-this");

    [Fact(DisplayName = "await-using-Symbol.asyncDispose-allows-non-promise-return-value.js")]
    public Task await_using_Symbol_asyncDispose_allows_non_promise_return_value()
        => ExecutionTest("await-using-Symbol.asyncDispose-allows-non-promise-return-value");

    [Fact(DisplayName = "await-using-Symbol.asyncDispose-allows-promiselike-return-value.js")]
    public Task await_using_Symbol_asyncDispose_allows_promiselike_return_value()
        => ExecutionTest("await-using-Symbol.asyncDispose-allows-promiselike-return-value");

    [Fact(DisplayName = "await-using-allows-null-initializer.js")]
    public Task await_using_allows_null_initializer()
        => ExecutionTest("await-using-allows-null-initializer");

    [Fact(DisplayName = "await-using-allows-undefined-initializer.js")]
    public Task await_using_allows_undefined_initializer()
        => ExecutionTest("await-using-allows-undefined-initializer");

    [Fact(DisplayName = "await-using-does-not-imply-await-if-not-evaluated.js")]
    public Task await_using_does_not_imply_await_if_not_evaluated()
        => ExecutionTest("await-using-does-not-imply-await-if-not-evaluated");

    [Fact(DisplayName = "await-using-implies-await-if-evaluated.js")]
    public Task await_using_implies_await_if_evaluated()
        => ExecutionTest("await-using-implies-await-if-evaluated");

    [Fact(DisplayName = "block-local-closure-get-before-initialization.js")]
    public Task block_local_closure_get_before_initialization()
        => ExecutionTest("block-local-closure-get-before-initialization");

    [Fact(DisplayName = "block-local-use-before-initialization-in-declaration-statement.js")]
    public Task block_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("block-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "block-local-use-before-initialization-in-prior-statement.js")]
    public Task block_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("block-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "fn-name-arrow.js")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class.js")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover.js")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn.js")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen.js")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

    [Fact(DisplayName = "function-local-closure-get-before-initialization.js")]
    public Task function_local_closure_get_before_initialization()
        => ExecutionTest("function-local-closure-get-before-initialization");

    [Fact(DisplayName = "function-local-use-before-initialization-in-declaration-statement.js")]
    public Task function_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("function-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "function-local-use-before-initialization-in-prior-statement.js")]
    public Task function_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("function-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "gets-initializer-Symbol.asyncDispose-property-once.js")]
    public Task gets_initializer_Symbol_asyncDispose_property_once()
        => ExecutionTest("gets-initializer-Symbol.asyncDispose-property-once");

    [Fact(DisplayName = "gets-initializer-Symbol.dispose-after-Symbol.asyncDispose-is-null.js")]
    public Task gets_initializer_Symbol_dispose_after_Symbol_asyncDispose_is_null()
        => ExecutionTest("gets-initializer-Symbol.dispose-after-Symbol.asyncDispose-is-null");

    [Fact(DisplayName = "gets-initializer-Symbol.dispose-after-Symbol.asyncDispose-is-undefined.js")]
    public Task gets_initializer_Symbol_dispose_after_Symbol_asyncDispose_is_undefined()
        => ExecutionTest("gets-initializer-Symbol.dispose-after-Symbol.asyncDispose-is-undefined");

    [Fact(DisplayName = "gets-initializer-Symbol.dispose-property-once.js")]
    public Task gets_initializer_Symbol_dispose_property_once()
        => ExecutionTest("gets-initializer-Symbol.dispose-property-once");

    [Fact(DisplayName = "gets-initializer-does-not-read-Symbol.dispose-if-Symbol.asyncDispose-exists.js")]
    public Task gets_initializer_does_not_read_Symbol_dispose_if_Symbol_asyncDispose_exists()
        => ExecutionTest("gets-initializer-does-not-read-Symbol.dispose-if-Symbol.asyncDispose-exists");

    [Fact(DisplayName = "global-closure-get-before-initialization.js")]
    public Task global_closure_get_before_initialization()
        => ExecutionTest("global-closure-get-before-initialization");

    [Fact(DisplayName = "global-use-before-initialization-in-declaration-statement.js")]
    public Task global_use_before_initialization_in_declaration_statement()
        => ExecutionTest("global-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "global-use-before-initialization-in-prior-statement.js")]
    public Task global_use_before_initialization_in_prior_statement()
        => ExecutionTest("global-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-at-end-of-asyncfunctionbody.js")]
    public Task initializer_Symbol_asyncDispose_called_at_end_of_asyncfunctionbody()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-at-end-of-asyncfunctionbody");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-at-end-of-asyncgeneratorbody.js")]
    public Task initializer_Symbol_asyncDispose_called_at_end_of_asyncgeneratorbody()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-at-end-of-asyncgeneratorbody");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-at-end-of-block.js")]
    public Task initializer_Symbol_asyncDispose_called_at_end_of_block()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-at-end-of-block");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-at-end-of-each-iteration-of-forofstatement.js")]
    public Task initializer_Symbol_asyncDispose_called_at_end_of_each_iteration_of_forofstatement()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-at-end-of-each-iteration-of-forofstatement");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-at-end-of-forstatement.js")]
    public Task initializer_Symbol_asyncDispose_called_at_end_of_forstatement()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-at-end-of-forstatement");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-if-subsequent-initializer-throws-in-forstatement-head.js")]
    public Task initializer_Symbol_asyncDispose_called_if_subsequent_initializer_throws_in_forstatement_head()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-if-subsequent-initializer-throws-in-forstatement-head");

    [Fact(DisplayName = "initializer-Symbol.asyncDispose-called-if-subsequent-initializer-throws.js")]
    public Task initializer_Symbol_asyncDispose_called_if_subsequent_initializer_throws()
        => ExecutionTest("initializer-Symbol.asyncDispose-called-if-subsequent-initializer-throws");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-at-end-of-asyncfunctionbody.js")]
    public Task initializer_Symbol_dispose_called_at_end_of_asyncfunctionbody()
        => ExecutionTest("initializer-Symbol.dispose-called-at-end-of-asyncfunctionbody");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-at-end-of-asyncgeneratorbody.js")]
    public Task initializer_Symbol_dispose_called_at_end_of_asyncgeneratorbody()
        => ExecutionTest("initializer-Symbol.dispose-called-at-end-of-asyncgeneratorbody");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-at-end-of-block.js")]
    public Task initializer_Symbol_dispose_called_at_end_of_block()
        => ExecutionTest("initializer-Symbol.dispose-called-at-end-of-block");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-at-end-of-each-iteration-of-forofstatement.js")]
    public Task initializer_Symbol_dispose_called_at_end_of_each_iteration_of_forofstatement()
        => ExecutionTest("initializer-Symbol.dispose-called-at-end-of-each-iteration-of-forofstatement");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-at-end-of-forstatement.js")]
    public Task initializer_Symbol_dispose_called_at_end_of_forstatement()
        => ExecutionTest("initializer-Symbol.dispose-called-at-end-of-forstatement");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-if-subsequent-initializer-throws-in-forstatement-head.js")]
    public Task initializer_Symbol_dispose_called_if_subsequent_initializer_throws_in_forstatement_head()
        => ExecutionTest("initializer-Symbol.dispose-called-if-subsequent-initializer-throws-in-forstatement-head");

    [Fact(DisplayName = "initializer-Symbol.dispose-called-if-subsequent-initializer-throws.js")]
    public Task initializer_Symbol_dispose_called_if_subsequent_initializer_throws()
        => ExecutionTest("initializer-Symbol.dispose-called-if-subsequent-initializer-throws");

    [Fact(DisplayName = "multiple-resources-disposed-in-reverse-order.js")]
    public Task multiple_resources_disposed_in_reverse_order()
        => ExecutionTest("multiple-resources-disposed-in-reverse-order");

    [Fact(DisplayName = "puts-initializer-on-top-of-disposableresourcestack-multiple-bindings.js")]
    public Task puts_initializer_on_top_of_disposableresourcestack_multiple_bindings()
        => ExecutionTest("puts-initializer-on-top-of-disposableresourcestack-multiple-bindings");

    [Fact(DisplayName = "puts-initializer-on-top-of-disposableresourcestack-subsequent-usings.js")]
    public Task puts_initializer_on_top_of_disposableresourcestack_subsequent_usings()
        => ExecutionTest("puts-initializer-on-top-of-disposableresourcestack-subsequent-usings");

    [Fact(DisplayName = "throws-error-as-is-if-only-one-error-during-disposal.js")]
    public Task throws_error_as_is_if_only_one_error_during_disposal()
        => ExecutionTest("throws-error-as-is-if-only-one-error-during-disposal");

    [Fact(DisplayName = "throws-if-initializer-Symbol.asyncDispose-property-is-null.js")]
    public Task throws_if_initializer_Symbol_asyncDispose_property_is_null()
        => ExecutionTest("throws-if-initializer-Symbol.asyncDispose-property-is-null");

    [Fact(DisplayName = "throws-if-initializer-Symbol.asyncDispose-property-is-undefined.js")]
    public Task throws_if_initializer_Symbol_asyncDispose_property_is_undefined()
        => ExecutionTest("throws-if-initializer-Symbol.asyncDispose-property-is-undefined");

    [Fact(DisplayName = "throws-if-initializer-Symbol.asyncDispose-property-not-callable.js")]
    public Task throws_if_initializer_Symbol_asyncDispose_property_not_callable()
        => ExecutionTest("throws-if-initializer-Symbol.asyncDispose-property-not-callable");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-is-null.js")]
    public Task throws_if_initializer_Symbol_dispose_property_is_null()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-is-null");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-is-undefined.js")]
    public Task throws_if_initializer_Symbol_dispose_property_is_undefined()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-is-undefined");

    [Fact(DisplayName = "throws-if-initializer-Symbol.dispose-property-not-callable.js")]
    public Task throws_if_initializer_Symbol_dispose_property_not_callable()
        => ExecutionTest("throws-if-initializer-Symbol.dispose-property-not-callable");

    [Fact(DisplayName = "throws-if-initializer-missing-both-Symbol.asyncDispose-and-Symbol.dispose.js")]
    public Task throws_if_initializer_missing_both_Symbol_asyncDispose_and_Symbol_dispose()
        => ExecutionTest("throws-if-initializer-missing-both-Symbol.asyncDispose-and-Symbol.dispose");

    [Fact(DisplayName = "throws-if-initializer-not-object.js")]
    public Task throws_if_initializer_not_object()
        => ExecutionTest("throws-if-initializer-not-object");

    [Fact(DisplayName = "throws-suppressederror-if-multiple-errors-during-disposal.js")]
    public Task throws_suppressederror_if_multiple_errors_during_disposal()
        => ExecutionTest("throws-suppressederror-if-multiple-errors-during-disposal");
}
