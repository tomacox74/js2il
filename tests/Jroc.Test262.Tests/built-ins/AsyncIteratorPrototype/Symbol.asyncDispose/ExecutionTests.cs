using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncIteratorPrototype.Symbol.asyncDispose;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncIteratorPrototype.Symbol.asyncDispose") { }

    [Fact(DisplayName = "invokes-return")]
    public Task invokes_return() => ExecutionTestFromFile("invokes-return");

    [Fact(DisplayName = "is-function")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-val")]
    public Task return_val() => ExecutionTestFromFile("return-val");

    [Fact(DisplayName = "throw-rejected-return")]
    public Task throw_rejected_return() => ExecutionTestFromFile("throw-rejected-return");

    [Fact(DisplayName = "throw-return-getter")]
    public Task throw_return_getter() => ExecutionTestFromFile("throw-return-getter");

    [Fact(DisplayName = "throw-return")]
    public Task throw_return() => ExecutionTestFromFile("throw-return");
}
