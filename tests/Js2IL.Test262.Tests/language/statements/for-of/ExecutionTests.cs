using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_of;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_of") { }

    [Fact(DisplayName = "ArgumentsObject_mapped-aliasing")]
    public Task ArgumentsObject_mapped_aliasing()
        => ExecutionTest("ArgumentsObject_mapped-aliasing");

    [Fact(DisplayName = "Array.prototype.Symbol.iterator")]
    public Task Array_prototype_Symbol_iterator()
        => ExecutionTest("Array.prototype.Symbol.iterator");

    [Fact(DisplayName = "Array.prototype.entries")]
    public Task Array_prototype_entries()
        => ExecutionTest("Array.prototype.entries");

    [Fact(DisplayName = "Array.prototype.keys")]
    public Task Array_prototype_keys()
        => ExecutionTest("Array.prototype.keys");

    [Fact(DisplayName = "arguments-mapped-aliasing")]
    public Task arguments_mapped_aliasing()
        => ExecutionTest("arguments-mapped-aliasing");

}
