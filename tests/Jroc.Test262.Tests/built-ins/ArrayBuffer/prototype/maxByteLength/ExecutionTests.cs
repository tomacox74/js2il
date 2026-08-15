using Jroc.Test262.Tests.built_ins;
namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.maxByteLength;
public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayBuffer.prototype.maxByteLength") { }
    [Fact(DisplayName = "invoked-as-accessor.js")] public Task invoked_as_accessor() => ExecutionTestFromFile("invoked-as-accessor");
    [Fact(DisplayName = "length.js")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "prop-desc.js")] public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "return-maxbytelength-non-resizable.js")] public Task return_maxbytelength_non_resizable() => ExecutionTestFromFile("return-maxbytelength-non-resizable");
}
