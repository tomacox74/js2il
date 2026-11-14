using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.apply;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.apply") { }

    [Fact(DisplayName = "S15.3.4.3_A5_T1")]
    public Task S15_3_4_3_A5_T1()
        => ExecutionTestFromFile("S15.3.4.3_A5_T1");

    [Fact(DisplayName = "S15.3.4.3_A5_T2")]
    public Task S15_3_4_3_A5_T2()
        => ExecutionTestFromFile("S15.3.4.3_A5_T2");

    [Fact(DisplayName = "S15.3.4.3_A5_T3")]
    public Task S15_3_4_3_A5_T3()
        => ExecutionTestFromFile("S15.3.4.3_A5_T3");

    [Fact(DisplayName = "get-index-abrupt")]
    public Task get_index_abrupt()
        => ExecutionTestFromFile("get-index-abrupt");

    [Fact(DisplayName = "get-length-abrupt")]
    public Task get_length_abrupt()
        => ExecutionTestFromFile("get-length-abrupt");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "argarray-not-object-realm.js")]
    public Task Port200_argarray_not_object_realm()
        => ExecutionTestFromFile("argarray-not-object-realm");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task Port200_resizable_buffer()
        => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "this-not-callable-realm.js")]
    public Task Port200_this_not_callable_realm()
        => ExecutionTestFromFile("this-not-callable-realm");
}
