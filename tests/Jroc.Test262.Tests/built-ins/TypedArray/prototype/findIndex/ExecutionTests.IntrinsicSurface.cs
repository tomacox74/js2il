using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findIndex;

public class IntrinsicSurfaceExecutionTests : DiskExecutionTestsBase
{
    public IntrinsicSurfaceExecutionTests() : base("built_ins.TypedArray.prototype.findIndex") { }

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method.js")]
    public Task invoked_as_method() => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
}
