using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_") { }

[Fact(DisplayName = "__proto__-fn-name")]
    public Task _proto_fn_name()
        => ExecutionTest("__proto__-fn-name");

[Fact(DisplayName = "__proto__-permitted-dup-shorthand")]
    public Task _proto_permitted_dup_shorthand()
        => ExecutionTest("__proto__-permitted-dup-shorthand");

[Fact(DisplayName = "__proto__-permitted-dup")]
    public Task _proto_permitted_dup()
        => ExecutionTest("__proto__-permitted-dup");

[Fact(DisplayName = "11.1.5_3-3-1")]
    public Task _11_1_5_3_3_1()
        => ExecutionTest("11.1.5_3-3-1");

[Fact(DisplayName = "11.1.5-0-1", Skip = "Legacy object literal accessor forms are not compiled yet.")]
    public Task _11_1_5_0_1()
        => ExecutionTest("11.1.5-0-1");

[Fact(DisplayName = "11.1.5-0-2", Skip = "Legacy object literal accessor forms are not compiled yet.")]
    public Task _11_1_5_0_2()
        => ExecutionTest("11.1.5-0-2");

[Fact(DisplayName = "11.1.5-2gs")]
    public Task _11_1_5_2gs()
        => ExecutionTest("11.1.5-2gs");
}
