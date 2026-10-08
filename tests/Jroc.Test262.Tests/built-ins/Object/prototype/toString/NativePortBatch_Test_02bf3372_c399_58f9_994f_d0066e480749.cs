using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.toString;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.built_ins.Object.prototype.toString") { }

    [Fact(DisplayName = "proxy-function")]
    public Task proxy_function() => ExecutionTestFromFile("proxy-function");

}
