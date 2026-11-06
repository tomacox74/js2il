using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettledKeyed;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.allSettledKeyed") { }

    [Fact(DisplayName = "arg-is-function.js")] public Task arg_is_function() => ExecutionTestFromFile("arg-is-function");
    [Fact(DisplayName = "arg-not-object-reject-bigint.js")] public Task arg_not_object_reject_bigint() => ExecutionTestFromFile("arg-not-object-reject-bigint");
    [Fact(DisplayName = "arg-not-object-reject.js")] public Task arg_not_object_reject() => ExecutionTestFromFile("arg-not-object-reject");
    [Fact(DisplayName = "key-order-preserved.js")] public Task key_order_preserved() => ExecutionTestFromFile("key-order-preserved");
    [Fact(DisplayName = "non-enumerable-properties-ignored.js")] public Task non_enumerable_properties_ignored() => ExecutionTestFromFile("non-enumerable-properties-ignored");
    [Fact(DisplayName = "prototype-keys-ignored.js")] public Task prototype_keys_ignored() => ExecutionTestFromFile("prototype-keys-ignored");
    [Fact(DisplayName = "resolve-not-callable-reject-with-typeerror.js")] public Task resolve_not_callable_reject_with_typeerror() => ExecutionTestFromFile("resolve-not-callable-reject-with-typeerror");
    [Fact(DisplayName = "resolved-all-fulfilled.js")] public Task resolved_all_fulfilled() => ExecutionTestFromFile("resolved-all-fulfilled");
    [Fact(DisplayName = "resolved-all-mixed.js")] public Task resolved_all_mixed() => ExecutionTestFromFile("resolved-all-mixed");
    [Fact(DisplayName = "resolved-all-rejected.js")] public Task resolved_all_rejected() => ExecutionTestFromFile("resolved-all-rejected");
    [Fact(DisplayName = "resolves-empty-object.js")] public Task resolves_empty_object() => ExecutionTestFromFile("resolves-empty-object");
    [Fact(DisplayName = "symbol-keys.js")] public Task symbol_keys() => ExecutionTestFromFile("symbol-keys");
}
