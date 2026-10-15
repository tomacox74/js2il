using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Date") { }

    [Fact(DisplayName = "proto-from-ctor-realm-one.js")]
    public Task test_proto_from_ctor_realm_one() => ExecutionTest("proto-from-ctor-realm-one");

    [Fact(DisplayName = "proto-from-ctor-realm-two.js")]
    public Task test_proto_from_ctor_realm_two() => ExecutionTest("proto-from-ctor-realm-two");

    [Fact(DisplayName = "proto-from-ctor-realm-zero.js")]
    public Task test_proto_from_ctor_realm_zero() => ExecutionTest("proto-from-ctor-realm-zero");

    [Fact(DisplayName = "value-symbol-to-prim-invocation.js")]
    public Task test_value_symbol_to_prim_invocation() => ExecutionTest("value-symbol-to-prim-invocation");

    [Fact(DisplayName = "value-symbol-to-prim-return-prim.js")]
    public Task test_value_symbol_to_prim_return_prim() => ExecutionTest("value-symbol-to-prim-return-prim");

    [Fact(DisplayName = "value-to-primitive-call.js")]
    public Task test_value_to_primitive_call() => ExecutionTest("value-to-primitive-call");

    [Fact(DisplayName = "value-to-primitive-result-string.js")]
    public Task test_value_to_primitive_result_string() => ExecutionTest("value-to-primitive-result-string");
}
