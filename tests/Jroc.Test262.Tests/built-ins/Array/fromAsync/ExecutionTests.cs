using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.fromAsync;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.fromAsync") { }

    [Fact(DisplayName = "async-iterable-async-mapped-awaits-once.js")]
    public Task async_iterable_async_mapped_awaits_once() => ExecutionTestFromFile("async-iterable-async-mapped-awaits-once");

    [Fact(DisplayName = "async-iterable-input-does-not-await-input.js")]
    public Task async_iterable_input_does_not_await_input() => ExecutionTestFromFile("async-iterable-input-does-not-await-input");

    [Fact(DisplayName = "async-iterable-input-iteration-err.js")]
    public Task async_iterable_input_iteration_err() => ExecutionTestFromFile("async-iterable-input-iteration-err");

    [Fact(DisplayName = "async-iterable-input.js")]
    public Task async_iterable_input() => ExecutionTestFromFile("async-iterable-input");

    [Fact(DisplayName = "asyncitems-array-add-to-empty.js")]
    public Task asyncitems_array_add_to_empty() => ExecutionTestFromFile("asyncitems-array-add-to-empty");

    [Fact(DisplayName = "asyncitems-array-add-to-singleton.js")]
    public Task asyncitems_array_add_to_singleton() => ExecutionTestFromFile("asyncitems-array-add-to-singleton");

    [Fact(DisplayName = "asyncitems-array-add.js")]
    public Task asyncitems_array_add() => ExecutionTestFromFile("asyncitems-array-add");

    [Fact(DisplayName = "asyncitems-array-mutate.js")]
    public Task asyncitems_array_mutate() => ExecutionTestFromFile("asyncitems-array-mutate");

    [Fact(DisplayName = "asyncitems-array-remove.js")]
    public Task asyncitems_array_remove() => ExecutionTestFromFile("asyncitems-array-remove");

    [Fact(DisplayName = "asyncitems-arraybuffer.js")]
    public Task asyncitems_arraybuffer() => ExecutionTestFromFile("asyncitems-arraybuffer");

    [Fact(DisplayName = "asyncitems-arraylike-holes.js")]
    public Task asyncitems_arraylike_holes() => ExecutionTestFromFile("asyncitems-arraylike-holes");

    [Fact(DisplayName = "asyncitems-arraylike-length-accessor-throws.js")]
    public Task asyncitems_arraylike_length_accessor_throws() => ExecutionTestFromFile("asyncitems-arraylike-length-accessor-throws");

    [Fact(DisplayName = "asyncitems-arraylike-promise.js")]
    public Task asyncitems_arraylike_promise() => ExecutionTestFromFile("asyncitems-arraylike-promise");

    [Fact(DisplayName = "asyncitems-arraylike-too-long.js")]
    public Task asyncitems_arraylike_too_long() => ExecutionTestFromFile("asyncitems-arraylike-too-long");

    [Fact(DisplayName = "asyncitems-asynciterator-exists.js")]
    public Task asyncitems_asynciterator_exists() => ExecutionTestFromFile("asyncitems-asynciterator-exists");

    [Fact(DisplayName = "asyncitems-asynciterator-not-callable.js")]
    public Task asyncitems_asynciterator_not_callable() => ExecutionTestFromFile("asyncitems-asynciterator-not-callable");

    [Fact(DisplayName = "asyncitems-asynciterator-null.js")]
    public Task asyncitems_asynciterator_null() => ExecutionTestFromFile("asyncitems-asynciterator-null");

    [Fact(DisplayName = "asyncitems-asynciterator-sync.js")]
    public Task asyncitems_asynciterator_sync() => ExecutionTestFromFile("asyncitems-asynciterator-sync");

    [Fact(DisplayName = "asyncitems-asynciterator-throws.js")]
    public Task asyncitems_asynciterator_throws() => ExecutionTestFromFile("asyncitems-asynciterator-throws");

    [Fact(DisplayName = "asyncitems-bigint.js")]
    public Task asyncitems_bigint() => ExecutionTestFromFile("asyncitems-bigint");

    [Fact(DisplayName = "asyncitems-boolean.js")]
    public Task asyncitems_boolean() => ExecutionTestFromFile("asyncitems-boolean");

    [Fact(DisplayName = "asyncitems-function.js")]
    public Task asyncitems_function() => ExecutionTestFromFile("asyncitems-function");

    [Fact(DisplayName = "asyncitems-iterator-exists.js")]
    public Task asyncitems_iterator_exists() => ExecutionTestFromFile("asyncitems-iterator-exists");

    [Fact(DisplayName = "asyncitems-iterator-not-callable.js")]
    public Task asyncitems_iterator_not_callable() => ExecutionTestFromFile("asyncitems-iterator-not-callable");

    [Fact(DisplayName = "asyncitems-iterator-null.js")]
    public Task asyncitems_iterator_null() => ExecutionTestFromFile("asyncitems-iterator-null");

    [Fact(DisplayName = "asyncitems-iterator-promise.js")]
    public Task asyncitems_iterator_promise() => ExecutionTestFromFile("asyncitems-iterator-promise");

    [Fact(DisplayName = "asyncitems-iterator-throws.js")]
    public Task asyncitems_iterator_throws() => ExecutionTestFromFile("asyncitems-iterator-throws");

    [Fact(DisplayName = "asyncitems-null-undefined.js")]
    public Task asyncitems_null_undefined() => ExecutionTestFromFile("asyncitems-null-undefined");

    [Fact(DisplayName = "asyncitems-number.js")]
    public Task asyncitems_number() => ExecutionTestFromFile("asyncitems-number");

    [Fact(DisplayName = "asyncitems-object-not-arraylike.js")]
    public Task asyncitems_object_not_arraylike() => ExecutionTestFromFile("asyncitems-object-not-arraylike");

    [Fact(DisplayName = "asyncitems-operations.js")]
    public Task asyncitems_operations() => ExecutionTestFromFile("asyncitems-operations");

    [Fact(DisplayName = "asyncitems-string.js")]
    public Task asyncitems_string() => ExecutionTestFromFile("asyncitems-string");

    [Fact(DisplayName = "asyncitems-symbol.js")]
    public Task asyncitems_symbol() => ExecutionTestFromFile("asyncitems-symbol");

    [Fact(DisplayName = "asyncitems-uses-intrinsic-iterator-symbols.js")]
    public Task asyncitems_uses_intrinsic_iterator_symbols() => ExecutionTestFromFile("asyncitems-uses-intrinsic-iterator-symbols");

    [Fact(DisplayName = "builtin.js")]
    public Task builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "mapfn-async-arraylike.js")]
    public Task mapfn_async_arraylike() => ExecutionTestFromFile("mapfn-async-arraylike");

    [Fact(DisplayName = "mapfn-async-iterable-async.js")]
    public Task mapfn_async_iterable_async() => ExecutionTestFromFile("mapfn-async-iterable-async");

    [Fact(DisplayName = "mapfn-async-iterable-sync.js")]
    public Task mapfn_async_iterable_sync() => ExecutionTestFromFile("mapfn-async-iterable-sync");

    [Fact(DisplayName = "mapfn-async-throws-close-async-iterator.js")]
    public Task mapfn_async_throws_close_async_iterator() => ExecutionTestFromFile("mapfn-async-throws-close-async-iterator");

    [Fact(DisplayName = "mapfn-async-throws-close-sync-iterator.js")]
    public Task mapfn_async_throws_close_sync_iterator() => ExecutionTestFromFile("mapfn-async-throws-close-sync-iterator");

    [Fact(DisplayName = "mapfn-async-throws.js")]
    public Task mapfn_async_throws() => ExecutionTestFromFile("mapfn-async-throws");

    [Fact(DisplayName = "mapfn-not-callable.js")]
    public Task mapfn_not_callable() => ExecutionTestFromFile("mapfn-not-callable");

    [Fact(DisplayName = "mapfn-result-awaited-once-per-iteration.js")]
    public Task mapfn_result_awaited_once_per_iteration() => ExecutionTestFromFile("mapfn-result-awaited-once-per-iteration");

    [Fact(DisplayName = "mapfn-sync-arraylike.js")]
    public Task mapfn_sync_arraylike() => ExecutionTestFromFile("mapfn-sync-arraylike");

    [Fact(DisplayName = "mapfn-sync-iterable-async.js")]
    public Task mapfn_sync_iterable_async() => ExecutionTestFromFile("mapfn-sync-iterable-async");

    [Fact(DisplayName = "mapfn-sync-iterable-sync.js")]
    public Task mapfn_sync_iterable_sync() => ExecutionTestFromFile("mapfn-sync-iterable-sync");

    [Fact(DisplayName = "mapfn-sync-throws-close-async-iterator.js")]
    public Task mapfn_sync_throws_close_async_iterator() => ExecutionTestFromFile("mapfn-sync-throws-close-async-iterator");

    [Fact(DisplayName = "mapfn-sync-throws-close-sync-iterator.js")]
    public Task mapfn_sync_throws_close_sync_iterator() => ExecutionTestFromFile("mapfn-sync-throws-close-sync-iterator");

    [Fact(DisplayName = "mapfn-sync-throws.js")]
    public Task mapfn_sync_throws() => ExecutionTestFromFile("mapfn-sync-throws");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-iterable-input-does-not-use-array-prototype.js")]
    public Task non_iterable_input_does_not_use_array_prototype() => ExecutionTestFromFile("non-iterable-input-does-not-use-array-prototype");

    [Fact(DisplayName = "non-iterable-input-element-access-err.js")]
    public Task non_iterable_input_element_access_err() => ExecutionTestFromFile("non-iterable-input-element-access-err");

    [Fact(DisplayName = "non-iterable-input-with-thenable-async-mapped-awaits-callback-result-once.js")]
    public Task non_iterable_input_with_thenable_async_mapped_awaits_callback_result_once() => ExecutionTestFromFile("non-iterable-input-with-thenable-async-mapped-awaits-callback-result-once");

    [Fact(DisplayName = "non-iterable-input-with-thenable-async-mapped-callback-err.js")]
    public Task non_iterable_input_with_thenable_async_mapped_callback_err() => ExecutionTestFromFile("non-iterable-input-with-thenable-async-mapped-callback-err");

    [Fact(DisplayName = "non-iterable-input-with-thenable-element-rejects.js")]
    public Task non_iterable_input_with_thenable_element_rejects() => ExecutionTestFromFile("non-iterable-input-with-thenable-element-rejects");

    [Fact(DisplayName = "non-iterable-input-with-thenable-sync-mapped-callback-err.js")]
    public Task non_iterable_input_with_thenable_sync_mapped_callback_err() => ExecutionTestFromFile("non-iterable-input-with-thenable-sync-mapped-callback-err");

    [Fact(DisplayName = "non-iterable-input-with-thenable.js")]
    public Task non_iterable_input_with_thenable() => ExecutionTestFromFile("non-iterable-input-with-thenable");

    [Fact(DisplayName = "non-iterable-input.js")]
    public Task non_iterable_input() => ExecutionTestFromFile("non-iterable-input");

    [Fact(DisplayName = "non-iterable-sync-mapped-callback-err.js")]
    public Task non_iterable_sync_mapped_callback_err() => ExecutionTestFromFile("non-iterable-sync-mapped-callback-err");

    [Fact(DisplayName = "non-iterable-with-non-promise-thenable.js")]
    public Task non_iterable_with_non_promise_thenable() => ExecutionTestFromFile("non-iterable-with-non-promise-thenable");

    [Fact(DisplayName = "non-iterable-with-thenable-async-mapped-awaits-once.js")]
    public Task non_iterable_with_thenable_async_mapped_awaits_once() => ExecutionTestFromFile("non-iterable-with-thenable-async-mapped-awaits-once");

    [Fact(DisplayName = "non-iterable-with-thenable-awaits-once.js")]
    public Task non_iterable_with_thenable_awaits_once() => ExecutionTestFromFile("non-iterable-with-thenable-awaits-once");

    [Fact(DisplayName = "non-iterable-with-thenable-sync-mapped-awaits-once.js")]
    public Task non_iterable_with_thenable_sync_mapped_awaits_once() => ExecutionTestFromFile("non-iterable-with-thenable-sync-mapped-awaits-once");

    [Fact(DisplayName = "non-iterable-with-thenable-then-method-err.js")]
    public Task non_iterable_with_thenable_then_method_err() => ExecutionTestFromFile("non-iterable-with-thenable-then-method-err");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "returned-promise-resolves-to-array.js")]
    public Task returned_promise_resolves_to_array() => ExecutionTestFromFile("returned-promise-resolves-to-array");

    [Fact(DisplayName = "returns-promise.js")]
    public Task returns_promise() => ExecutionTestFromFile("returns-promise");

    [Fact(DisplayName = "sync-iterable-input-with-non-promise-thenable.js")]
    public Task sync_iterable_input_with_non_promise_thenable() => ExecutionTestFromFile("sync-iterable-input-with-non-promise-thenable");

    [Fact(DisplayName = "sync-iterable-input-with-thenable.js")]
    public Task sync_iterable_input_with_thenable() => ExecutionTestFromFile("sync-iterable-input-with-thenable");

    [Fact(DisplayName = "sync-iterable-input.js")]
    public Task sync_iterable_input() => ExecutionTestFromFile("sync-iterable-input");

    [Fact(DisplayName = "sync-iterable-iteration-err.js")]
    public Task sync_iterable_iteration_err() => ExecutionTestFromFile("sync-iterable-iteration-err");

    [Fact(DisplayName = "sync-iterable-with-rejecting-thenable-closes.js")]
    public Task sync_iterable_with_rejecting_thenable_closes() => ExecutionTestFromFile("sync-iterable-with-rejecting-thenable-closes");

    [Fact(DisplayName = "sync-iterable-with-rejecting-thenable-rejects.js")]
    public Task sync_iterable_with_rejecting_thenable_rejects() => ExecutionTestFromFile("sync-iterable-with-rejecting-thenable-rejects");

    [Fact(DisplayName = "sync-iterable-with-thenable-async-mapped-awaits-once.js")]
    public Task sync_iterable_with_thenable_async_mapped_awaits_once() => ExecutionTestFromFile("sync-iterable-with-thenable-async-mapped-awaits-once");

    [Fact(DisplayName = "sync-iterable-with-thenable-async-mapped-callback-err.js")]
    public Task sync_iterable_with_thenable_async_mapped_callback_err() => ExecutionTestFromFile("sync-iterable-with-thenable-async-mapped-callback-err");

    [Fact(DisplayName = "sync-iterable-with-thenable-awaits-once.js")]
    public Task sync_iterable_with_thenable_awaits_once() => ExecutionTestFromFile("sync-iterable-with-thenable-awaits-once");

    [Fact(DisplayName = "sync-iterable-with-thenable-sync-mapped-awaits-once.js")]
    public Task sync_iterable_with_thenable_sync_mapped_awaits_once() => ExecutionTestFromFile("sync-iterable-with-thenable-sync-mapped-awaits-once");

    [Fact(DisplayName = "sync-iterable-with-thenable-sync-mapped-callback-err.js")]
    public Task sync_iterable_with_thenable_sync_mapped_callback_err() => ExecutionTestFromFile("sync-iterable-with-thenable-sync-mapped-callback-err");

    [Fact(DisplayName = "sync-iterable-with-thenable-then-method-err.js")]
    public Task sync_iterable_with_thenable_then_method_err() => ExecutionTestFromFile("sync-iterable-with-thenable-then-method-err");

    [Fact(DisplayName = "this-constructor-operations.js")]
    public Task this_constructor_operations() => ExecutionTestFromFile("this-constructor-operations");

    [Fact(DisplayName = "this-constructor-with-bad-length-setter.js")]
    public Task this_constructor_with_bad_length_setter() => ExecutionTestFromFile("this-constructor-with-bad-length-setter");

    [Fact(DisplayName = "this-constructor-with-readonly-elements.js")]
    public Task this_constructor_with_readonly_elements() => ExecutionTestFromFile("this-constructor-with-readonly-elements");

    [Fact(DisplayName = "this-constructor-with-readonly-length.js")]
    public Task this_constructor_with_readonly_length() => ExecutionTestFromFile("this-constructor-with-readonly-length");

    [Fact(DisplayName = "this-constructor-with-unsettable-element-closes-async-iterator.js")]
    public Task this_constructor_with_unsettable_element_closes_async_iterator() => ExecutionTestFromFile("this-constructor-with-unsettable-element-closes-async-iterator");

    [Fact(DisplayName = "this-constructor-with-unsettable-element-closes-sync-iterator.js")]
    public Task this_constructor_with_unsettable_element_closes_sync_iterator() => ExecutionTestFromFile("this-constructor-with-unsettable-element-closes-sync-iterator");

    [Fact(DisplayName = "this-constructor-with-unsettable-element.js")]
    public Task this_constructor_with_unsettable_element() => ExecutionTestFromFile("this-constructor-with-unsettable-element");

    [Fact(DisplayName = "this-constructor.js")]
    public Task this_constructor() => ExecutionTestFromFile("this-constructor");

    [Fact(DisplayName = "this-non-constructor.js")]
    public Task this_non_constructor() => ExecutionTestFromFile("this-non-constructor");

    [Fact(DisplayName = "thisarg-object.js")]
    public Task thisarg_object() => ExecutionTestFromFile("thisarg-object");

    [Fact(DisplayName = "thisarg-omitted-sloppy.js")]
    public Task thisarg_omitted_sloppy() => ExecutionTestFromFile("thisarg-omitted-sloppy");

    [Fact(DisplayName = "thisarg-omitted-strict.js")]
    public Task thisarg_omitted_strict() => ExecutionTestFromFile("thisarg-omitted-strict");

    [Fact(DisplayName = "thisarg-primitive-sloppy.js")]
    public Task thisarg_primitive_sloppy() => ExecutionTestFromFile("thisarg-primitive-sloppy");

    [Fact(DisplayName = "thisarg-primitive-strict.js")]
    public Task thisarg_primitive_strict() => ExecutionTestFromFile("thisarg-primitive-strict");
}
