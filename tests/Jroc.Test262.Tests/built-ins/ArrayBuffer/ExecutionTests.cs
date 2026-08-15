using Jroc.Test262.Tests.built_ins;
namespace Jroc.Test262.Tests.built_ins.ArrayBuffer;
public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayBuffer") { }
    [Fact(DisplayName = "length.js")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "prop-desc.js")] public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "newtarget-prototype-is-not-object.js")] public Task newtarget_prototype_is_not_object() => ExecutionTestFromFile("newtarget-prototype-is-not-object");
}
