using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replaceAll;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.replaceAll") { }

    [Fact(DisplayName = "getSubstitution-0x0024-0x0024.js")]
    public Task getSubstitution_0x0024_0x0024()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0024");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0026.js")]
    public Task getSubstitution_0x0024_0x0026()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0026");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0027.js")]
    public Task getSubstitution_0x0024_0x0027()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0027");

    [Fact(DisplayName = "getSubstitution-0x0024-0x003C.js")]
    public Task getSubstitution_0x0024_0x003C()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x003C");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0060.js")]
    public Task getSubstitution_0x0024_0x0060()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0060");

    [Fact(DisplayName = "getSubstitution-0x0024.js")]
    public Task getSubstitution_0x0024()
        => ExecutionTestFromFile("getSubstitution-0x0024");

    [Fact(DisplayName = "getSubstitution-0x0024N.js")]
    public Task getSubstitution_0x0024N()
        => ExecutionTestFromFile("getSubstitution-0x0024N");

    [Fact(DisplayName = "getSubstitution-0x0024NN.js")]
    public Task getSubstitution_0x0024NN()
        => ExecutionTestFromFile("getSubstitution-0x0024NN");

    [Fact(DisplayName = "cstm-replaceall-on-bigint-primitive.js")]
    public Task cstm_replaceall_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-bigint-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-boolean-primitive.js")]
    public Task cstm_replaceall_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-boolean-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-number-primitive.js")]
    public Task cstm_replaceall_on_number_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-number-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-string-primitive.js")]
    public Task cstm_replaceall_on_string_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-string-primitive");
}
