using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_") { }

    [Fact(DisplayName = "__proto__-fn-name")]
    public Task _proto_fn_name()
        => ExecutionTest("__proto__-fn-name");

    [Fact(DisplayName = "__proto__-permitted-dup-shorthand", Skip = "Known JS2IL defect")]
    public Task _proto_permitted_dup_shorthand()
        => ExecutionTest("__proto__-permitted-dup-shorthand");

    [Fact(DisplayName = "__proto__-permitted-dup")]
    public Task _proto_permitted_dup()
        => ExecutionTest("__proto__-permitted-dup");

    [Fact(DisplayName = "11.1.5_3-3-1", Skip = "Known JS2IL defect")]
    public Task _11_1_5_3_3_1()
        => ExecutionTest("11.1.5_3-3-1");
}
