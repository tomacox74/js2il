using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Promise.allKeyed;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.built_ins.Promise.allKeyed") { }

    [Fact(DisplayName = "arg-is-function")]
    public Task arg_is_function() => ExecutionTestFromFile("arg-is-function");

    [Fact(DisplayName = "arg-not-object-reject")]
    public Task arg_not_object_reject() => ExecutionTestFromFile("arg-not-object-reject");

    [Fact(DisplayName = "arg-not-object-reject-bigint")]
    public Task arg_not_object_reject_bigint() => ExecutionTestFromFile("arg-not-object-reject-bigint");

    [Fact(DisplayName = "key-order-preserved")]
    public Task key_order_preserved() => ExecutionTestFromFile("key-order-preserved");

    [Fact(DisplayName = "non-enumerable-properties-ignored")]
    public Task non_enumerable_properties_ignored() => ExecutionTestFromFile("non-enumerable-properties-ignored");

    [Fact(DisplayName = "prototype-keys-ignored")]
    public Task prototype_keys_ignored() => ExecutionTestFromFile("prototype-keys-ignored");

    [Fact(DisplayName = "reject-deferred")]
    public Task reject_deferred() => ExecutionTestFromFile("reject-deferred");

    [Fact(DisplayName = "reject-immed")]
    public Task reject_immed() => ExecutionTestFromFile("reject-immed");

    [Fact(DisplayName = "resolve-not-callable-reject-with-typeerror")]
    public Task resolve_not_callable_reject_with_typeerror() => ExecutionTestFromFile("resolve-not-callable-reject-with-typeerror");

    [Fact(DisplayName = "resolves-empty-object")]
    public Task resolves_empty_object() => ExecutionTestFromFile("resolves-empty-object");

    [Fact(DisplayName = "symbol-keys")]
    public Task symbol_keys() => ExecutionTestFromFile("symbol-keys");

}
