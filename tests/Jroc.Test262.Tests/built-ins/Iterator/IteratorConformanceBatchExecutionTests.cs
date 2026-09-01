using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator") { }

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "newtarget-or-active-function-object.js")]
    public Task newtarget_or_active_function_object() => ExecutionTestFromFile("newtarget-or-active-function-object");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

}
