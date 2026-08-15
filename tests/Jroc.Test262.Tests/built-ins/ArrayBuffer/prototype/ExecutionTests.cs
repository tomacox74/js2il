using Jroc.Test262.Tests.built_ins;
namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype;
public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayBuffer.prototype") { }
    [Fact(DisplayName = "constructor.js")] public Task constructor() => ExecutionTestFromFile("constructor");
    [Fact(DisplayName = "Symbol.toStringTag.js")] public Task symbol_to_string_tag() => ExecutionTestFromFile("Symbol.toStringTag");
}
